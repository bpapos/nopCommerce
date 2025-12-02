using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Payments.OpenEdge.HostPay;
using Nop.Plugin.Payments.OpenEdge.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
//using Nop.Web.Areas.Admin.Factories;

using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Order;

namespace Nop.Plugin.Payments.OpenEdge.Controllers
{
	public class PaymentOpenEdgeController : BasePaymentController
	{

		#region Fields
		private readonly ILocalizationService _localizationService;
		private readonly ILogger _logger;
		private readonly INotificationService _notificationService;
		private readonly IOrderModelFactory _orderModelFactory;
		private readonly IOrderProcessingService _orderProcessingService;
		private readonly IOrderService _orderService;
		private readonly IOrderTotalCalculationService _orderTotalCalculationService;
		private readonly IPaymentPluginManager _paymentPluginManager;
		private readonly IPermissionService _permissionService;
		private readonly ISettingService _settingService;
		private readonly IStoreContext _storeContext;
		private readonly IWebHelper _webHelper;
		private readonly IWorkContext _workContext;

		private readonly OpenEdgePaymentSettings _openEdgePaymentSettings;

		#endregion

        #region Ctor

		public PaymentOpenEdgeController(
			ILocalizationService localizationService, 
			ILogger logger, 
			INotificationService notificationService, 
			IOrderModelFactory orderModelFactory, 
			IOrderProcessingService orderProcessingService, 
			IOrderService orderService, 
			IOrderTotalCalculationService orderTotalCalculationService, 
			IPaymentPluginManager paymentPluginManager, 
			IPermissionService permissionService, 
			ISettingService settingService, 
			IStoreContext storeContext, 
			IWebHelper webHelper, 
			IWorkContext workContext,
			 OpenEdgePaymentSettings openEdgePaymentSettings)
		{
			_localizationService = localizationService;
			_logger = logger;
			_notificationService = notificationService;
			_orderModelFactory = orderModelFactory;
			_orderProcessingService = orderProcessingService;
			_orderService = orderService;
			_orderTotalCalculationService = orderTotalCalculationService;
			_paymentPluginManager = paymentPluginManager;
			_permissionService = permissionService;
			_settingService = settingService;
			_storeContext = storeContext;
			_webHelper = webHelper;
			_workContext = workContext;
			_openEdgePaymentSettings = openEdgePaymentSettings;
		}

        #endregion

        #region Methods

        [AutoValidateAntiforgeryToken]
        [AuthorizeAdmin] //confirms access to the admin panel
        [Area(AreaNames.ADMIN)] //specifies the area containing a controller or action
        //[CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
        public async Task<IActionResult> Configure()
        {
			//if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
			//{
			//	return AccessDeniedView();
			//}

			//load settings for a chosen store scope
			int storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
			OpenEdgePaymentSettings openEdgePaymentSettings = await _settingService.LoadSettingAsync<OpenEdgePaymentSettings>(storeScope);

            //prepare model
			ConfigurationModel model = new ConfigurationModel
			{
				UseSandbox = openEdgePaymentSettings.UseSandbox,
				XWebId = openEdgePaymentSettings.XWebId,
				TerminalId = openEdgePaymentSettings.TerminalId,
				AuthorizationKey = openEdgePaymentSettings.AuthorizationKey,
				Industry = openEdgePaymentSettings.Industry,
				ActiveStoreScopeConfiguration = storeScope
			};

			if (storeScope > 0)
			{
                model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(openEdgePaymentSettings, x => x.UseSandbox, storeScope);
                model.XWebId_OverrideForStore = await _settingService.SettingExistsAsync(openEdgePaymentSettings, x => x.XWebId, storeScope);
                model.TerminalId_OverrideForStore = await _settingService.SettingExistsAsync(openEdgePaymentSettings, x => x.TerminalId, storeScope);
                model.AuthorizationKey_OverrideForStore = await _settingService.SettingExistsAsync(openEdgePaymentSettings, x => x.AuthorizationKey, storeScope);
                model.Industry_OverrideForStore = await _settingService.SettingExistsAsync(openEdgePaymentSettings, x => x.Industry, storeScope);
			}
			return View("~/Plugins/Payments.OpenEdge/Views/Configure.cshtml", model);
		}


		[HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]

        public async Task<IActionResult> Configure(ConfigurationModel model)
		{
   //         //whether user has the authority
			//if (!(await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods)))
			//{
			//	return AccessDeniedView();
			//}

			if (!ModelState.IsValid)
			{
				return await Configure();
			}

            //load settings for a chosen store scope
			int storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
			OpenEdgePaymentSettings openEdgePaymentSettings = await _settingService.LoadSettingAsync<OpenEdgePaymentSettings>(storeScope);
			
            //save settings
			openEdgePaymentSettings.UseSandbox = model.UseSandbox;
			openEdgePaymentSettings.XWebId = model.XWebId;
			openEdgePaymentSettings.TerminalId = model.TerminalId;
			openEdgePaymentSettings.AuthorizationKey = model.AuthorizationKey;
			openEdgePaymentSettings.Industry = model.Industry;


            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(openEdgePaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(openEdgePaymentSettings, x => x.XWebId, model.XWebId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(openEdgePaymentSettings, x => x.TerminalId, model.TerminalId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(openEdgePaymentSettings, x => x.AuthorizationKey, model.AuthorizationKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(openEdgePaymentSettings, x => x.Industry, model.Industry_OverrideForStore, storeScope, false);
			// await _settingService.SaveSettingOverridablePerStoreAsync<OpenEdgePaymentSettings, bool>(openEdgePaymentSettings, (Expression<Func<OpenEdgePaymentSettings, bool>>)((OpenEdgePaymentSettings x) => x.UseSandbox), model.UseSandbox_OverrideForStore, storeScope, false);
			// await _settingService.SaveSettingOverridablePerStoreAsync<OpenEdgePaymentSettings, string>(openEdgePaymentSettings, (Expression<Func<OpenEdgePaymentSettings, string>>)((OpenEdgePaymentSettings x) => x.XWebId), model.XWebId_OverrideForStore, storeScope, false);
			// await _settingService.SaveSettingOverridablePerStoreAsync<OpenEdgePaymentSettings, string>(openEdgePaymentSettings, (Expression<Func<OpenEdgePaymentSettings, string>>)((OpenEdgePaymentSettings x) => x.TerminalId), model.TerminalId_OverrideForStore, storeScope, false);
			// await _settingService.SaveSettingOverridablePerStoreAsync<OpenEdgePaymentSettings, string>(openEdgePaymentSettings, (Expression<Func<OpenEdgePaymentSettings, string>>)((OpenEdgePaymentSettings x) => x.AuthorizationKey), model.AuthorizationKey_OverrideForStore, storeScope, false);
			// await _settingService.SaveSettingOverridablePerStoreAsync<OpenEdgePaymentSettings, string>(openEdgePaymentSettings, (Expression<Func<OpenEdgePaymentSettings, string>>)((OpenEdgePaymentSettings x) => x.Industry), model.Industry_OverrideForStore, storeScope, false);

            //now clear settings cache
			await _settingService.ClearCacheAsync();

			_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"), true);
			return await Configure();
		}


		public async Task<IActionResult> Payment(int orderId, string hostPayUrl)
		{
			//return "hello";
			Order order = await _orderService.GetOrderByIdAsync(orderId);
            OrderDetailsModel model = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);
		
			ViewBag.PaymentUrl = hostPayUrl;
			return View("~/Plugins/Payments.OpenEdge/Views/Payment.cshtml", model);
		}

		public async Task<IActionResult> Return([FromQuery(Name = "params")] string xmlResult, [FromQuery(Name = "action")] string action)
		{
			Order order;
			if (action != null && string.Compare(action, "cancel") == 0)
			{
				Store store = await _storeContext.GetCurrentStoreAsync();
				Customer customer = await _workContext.GetCurrentCustomerAsync();

				// order = ((IEnumerable<Order>)(await _orderService.SearchOrdersAsync(((BaseEntity)store).Id, 0, ((BaseEntity)customer).Id, 0, 0, 0, 0, (string)null, (DateTime?)null, (DateTime?)null, (List<int>)null, (List<int>)null, (List<int>)null, (string)null, (string)null, "", (string)null, 0, 1, false))).FirstOrDefault();

				order = (await _orderService.SearchOrdersAsync(store.Id,
                    customerId: customer.Id)).FirstOrDefault();

				if (order != null)
				{
					_notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.Payments.OpenEdge.PaymentFailed"), true);
					return RedirectToRoute("OrderDetails", new
					{
						orderId = order.Id
					});
				}
				return RedirectToRoute("Homepage");
			}
			if (!(await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.OpenEdge") is OpenEdgeProcessor processor) || !_paymentPluginManager.IsPluginActive(processor))
			{
				throw new NopException("OpenEdge Payment module cannot be loaded");
			}

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlResult);

            XmlNode resultNode;
            XmlNode node;
            string orderId = "";
            string resultCode = "";

			resultNode = xml.SelectSingleNode("RESULT");
			if (resultNode != null)
			{
				node = resultNode.SelectSingleNode("RESPONSECODE");
				if (node != null)
				{
					resultCode = resultNode.SelectSingleNode("RESPONSECODE").InnerText;
				}

				node = resultNode.SelectSingleNode("ORDERID");
				if (node != null)
				{
					orderId = resultNode.SelectSingleNode("ORDERID").InnerText;
				}
			}

			order = await _orderService.GetOrderByIdAsync(Convert.ToInt32(orderId));
			if (order == null)
			{
				throw new NopException($"The order ID {orderId} doesn't exists");
			}
			
			HostPayTransaction model = new HostPayTransaction
			{
				TransactionKey = orderId,
				TransactionType = TransactionType.QUERYPAYMENT,
				Amount = order.OrderTotal
			};

			HostPayClient client = new HostPayClient(_openEdgePaymentSettings, model, _localizationService, _logger, _notificationService);
			bool success = await client.GetTransactionStatus(model);

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("OpenEdge details:");
			sb.AppendLine("OrderID: " + orderId);

			await _orderService.InsertOrderNoteAsync(new OrderNote
			{
				OrderId = order.Id,
				Note = sb.ToString(),
				DisplayToCustomer = false,
				CreatedOnUtc = DateTime.UtcNow
			});

			await _orderService.UpdateOrderAsync(order);

			if (!success)
			{
				return Content($"The transaction status received from the payment processor for the order {orderId} was declined.");
			}

			if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
			{
				await _orderProcessingService.MarkAsAuthorizedAsync(order);
			}
			
			return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
		}

		#endregion
	}
}