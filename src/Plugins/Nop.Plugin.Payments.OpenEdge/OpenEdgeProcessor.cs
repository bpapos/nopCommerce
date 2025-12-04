using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.OpenEdge.Components;
using Nop.Plugin.Payments.OpenEdge.HostPay;
using Nop.Plugin.Payments.OpenEdge.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.OpenEdge
{

    public class OpenEdgeProcessor : BasePlugin, IPaymentMethod, IPlugin
    {
        #region fields
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INopUrlHelper _nopUrlHelper;
        private readonly INotificationService _notificationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly OpenEdgePaymentSettings _openEdgePaymentSettings;


        #endregion


        #region Properties

        public bool SupportCapture => true;
        public bool SupportPartiallyRefund => false;
        public bool SupportRefund => true;
        public bool SupportVoid => true;
        public RecurringPaymentType RecurringPaymentType => (RecurringPaymentType)0;
        public PaymentMethodType PaymentMethodType => (PaymentMethodType)15;
        public bool SkipPaymentInfo => false;

        #endregion


        #region Ctor


        public OpenEdgeProcessor(IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            ILogger logger,
            INopUrlHelper nopUrlHelper,
            INotificationService notificationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            ISettingService settingService,
            IWebHelper webHelper,
            OpenEdgePaymentSettings openEdgePaymentSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _logger = logger;
            _nopUrlHelper = nopUrlHelper;
            _notificationService = notificationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _settingService = settingService;
            _webHelper = webHelper;
            _openEdgePaymentSettings = openEdgePaymentSettings;

            _logger.InformationAsync("New instance of OpenedgeProcessor").GetAwaiter().GetResult();
        }

        #endregion


        #region Methods
        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }


        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            HostPayTransaction model = new HostPayTransaction
            {
                Amount = postProcessPaymentRequest.Order.OrderTotal,
                TransactionKey = postProcessPaymentRequest.Order.Id.ToString(),
                TransactionType = TransactionType.CREDITAUTH
            };
            string returnUrl = _webHelper.GetStoreLocation(true) + "Plugins/PaymentOpenEdge/Return";
            model.SuccessURL = returnUrl;
            HostPayClient client = new HostPayClient(_openEdgePaymentSettings, model, _localizationService, _logger, _notificationService);
            string hostPayUrl = client.SetupTransaction(model).GetAwaiter().GetResult();

            string paymentUrl = _webHelper.GetStoreLocation(true) + "Plugins/PaymentOpenEdge/Payment";
            await _logger.InformationAsync("Payment link: " + paymentUrl);

            Dictionary<string, string> payParams = new Dictionary<string, string>
            {
                ["orderId"] = postProcessPaymentRequest.Order.Id.ToString(),
                ["hostPayUrl"] = hostPayUrl
            };
            paymentUrl = QueryHelpers.AddQueryString(paymentUrl, payParams);
            _httpContextAccessor.HttpContext.Response.Redirect(paymentUrl);
        }


        public async Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }


        public async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            HostPayTransaction model = new HostPayTransaction
            {
                Amount = capturePaymentRequest.Order.OrderTotal,
                TransactionKey = capturePaymentRequest.Order.Id.ToString(),
                TransactionType = TransactionType.CREDITONLINECAPTURE
            };

            HostPayClient client = new HostPayClient(_openEdgePaymentSettings, model, _localizationService, _logger, _notificationService);
            bool success = client.CaptureTransaction(model).GetAwaiter().GetResult();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("OpenEdge details:");
            sb.AppendLine("OrderID: " + model.TransactionKey);
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = capturePaymentRequest.Order.Id,
                Note = sb.ToString(),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            await _orderService.UpdateOrderAsync(capturePaymentRequest.Order);

            return new CapturePaymentResult
            {
                CaptureTransactionResult = "result",
                NewPaymentStatus = PaymentStatus.Paid
            };
        }

        public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return default(decimal);
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            return true;
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/PaymentOpenEdge/Configure";
            //return _nopUrlHelper.RouteUrl(OpenEdgeDefaults.Route.Configuration);

        }

        //public string GetPublicViewComponentName()
        //{
        //    return "PaymentOpenEdge";
        //}

        public async Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            HostPayTransaction model = new HostPayTransaction
            {
                Amount = refundPaymentRequest.AmountToRefund,
                TransactionKey = refundPaymentRequest.Order.Id.ToString(),
                TransactionType = TransactionType.CREDITRETURN
            };

            HostPayClient client = new HostPayClient(_openEdgePaymentSettings, model, _localizationService, _logger, _notificationService);
            bool success = await client.RefundTransaction(model);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("OpenEdge refund details:");
            sb.AppendLine("OrderID: " + model.TransactionKey);
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = refundPaymentRequest.Order.Id,
                Note = sb.ToString(),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await _orderService.UpdateOrderAsync(refundPaymentRequest.Order);

            return new RefundPaymentResult
            {
                NewPaymentStatus = (PaymentStatus)40
            };
        }

        public async Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            List<string> result = new List<string>();
            return result;
        }

        public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public override async Task InstallAsync()
        {
            // Settings
            await _settingService.SaveSettingAsync<OpenEdgePaymentSettings>(new OpenEdgePaymentSettings
            {
                UseSandbox = false
            });

            // Locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payments.OpenEdge.PaymentMethodDescription"] = "OpenEdge Secured Payment",
                ["Plugins.Payments.OpenEdge.RedirectionTip"] = "You will be redirected to OpenEdge site to complete the order.",
                ["Plugins.Payments.OpenEdge.UseSandbox"] = "Account is a sandbox account",
                ["Plugins.Payments.OpenEdge.XWebId"] = "XWeb ID",
                ["Plugins.Payments.OpenEdge.TerminalId"] = "Terminal ID",
                ["Plugins.Payments.OpenEdge.AuthorizationKey"] = "Authorization Key",
                ["Plugins.Payments.OpenEdge.Industry"] = "Industry",
                ["Plugins.Payments.OpenEdge.Fields.SaveCard"] = "Save card",
                ["Plugins.Payments.OpenEdge.PaymentFailed"] = "Payment processing failed. Please retry or contact BSS to arrange payment."
            });
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<OpenEdgePaymentSettings>();
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.OpenEdge");
            await base.InstallAsync();
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Payments.OpenEdge.PaymentMethodDescription");
        }

        public Type GetPublicViewComponent()
        {
            return typeof(PaymentOpenEdgeViewComponent);

        }

        #endregion
    }
}