using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Nop.Plugin.Payments.OpenEdge.Models;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;

namespace Nop.Plugin.Payments.OpenEdge.HostPay
{
	public class HostPayClient
	{
		private OpenEdgePaymentSettings _openEdgePaymentSettings;
		private HostPayTransaction _hostPayTransaction;
		private readonly ILocalizationService _localizationService;
		private readonly INotificationService _notificationService;
		private readonly ILogger _logger;

		public HostPayClient(OpenEdgePaymentSettings openEdgePaymentSettings, HostPayTransaction hostPayTransaction, ILocalizationService localizationService, ILogger logger, INotificationService notificationService)
		{
			_openEdgePaymentSettings = openEdgePaymentSettings;
			_hostPayTransaction = hostPayTransaction;
			_localizationService = localizationService;
			_notificationService = notificationService;
			_logger = logger;
		}

		public async Task<string> SetupTransaction(HostPayTransaction hostPayTransaction)
		{
			if (string.IsNullOrEmpty(hostPayTransaction.TransactionKey))
			{
				hostPayTransaction.TransactionKey = UniqueId.GenerateId();
			}


			HostPayConfig hpConfig = new HostPayConfig();
			hpConfig.UseSandbox = _openEdgePaymentSettings.UseSandbox;
			string hostPayUrl = hpConfig.SetupUrl;

			HostPayHelper helper = new HostPayHelper(_openEdgePaymentSettings, hpConfig, _logger);
			string xmlPost = helper.XMLPostWriter(hostPayTransaction, "SetupOTK");

			string result = await helper.GatewayCallAsync(xmlPost, RequestType.Setup);


			XmlDocument xml = new XmlDocument();
			xml.LoadXml(result);
			string paypageURL;
			XmlNode node = xml.SelectSingleNode("RESULT/PAYPAGEURL");
			if (node != null)
			{
				paypageURL = xml.SelectSingleNode("RESULT/PAYPAGEURL").InnerText;
			}
			else
			{
				_notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.Payments.OpenEdge.PaymentFailed"), true);
				paypageURL = "/OrderDetails/" + hostPayTransaction.TransactionKey;
			}

			hostPayTransaction.PaymentUrl = paypageURL;

			return paypageURL;
		}


		public async Task<bool> GetTransactionStatus(HostPayTransaction hostPayTransaction)
		{
			hostPayTransaction.TransactionType = TransactionType.QUERYPAYMENT;
			HostPayHelper helper = new HostPayHelper(hostPayConfig: new HostPayConfig
			{
				UseSandbox = _openEdgePaymentSettings.UseSandbox
			}, openEdgePaymentSettings: _openEdgePaymentSettings, logger: _logger);
			string xmlPost = helper.XMLPostWriter(hostPayTransaction, "QueryStatus");
			return ParseResults(await helper.GatewayCallAsync(xmlPost, RequestType.Direct));
		}

		public async Task<bool> CaptureTransaction(HostPayTransaction hostPayTransaction)
		{
			HostPayHelper helper = new HostPayHelper(hostPayConfig: new HostPayConfig
			{
				UseSandbox = _openEdgePaymentSettings.UseSandbox
			}, openEdgePaymentSettings: _openEdgePaymentSettings, logger: _logger);
			string xmlPost = helper.XMLPostWriter(hostPayTransaction, "Capture");
			return ParseResults(await helper.GatewayCallAsync(xmlPost, RequestType.Direct));
		}

		public async Task<bool> RefundTransaction(HostPayTransaction hostPayTransaction)
		{
			HostPayHelper helper = new HostPayHelper(hostPayConfig: new HostPayConfig
			{
				UseSandbox = _openEdgePaymentSettings.UseSandbox
			}, openEdgePaymentSettings: _openEdgePaymentSettings, logger: _logger);
			string xmlPost = helper.XMLPostWriter(hostPayTransaction, "Refund");
			return ParseResults(await helper.GatewayCallAsync(xmlPost, RequestType.Direct));
		}

		private bool ParseResults(string xmlResults)
		{
			GatewayResponse gatewayResponse = new GatewayResponse();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GatewayResponse));
			XmlReader xmlReader = new XmlTextReader(new StringReader(xmlResults));
			gatewayResponse = (GatewayResponse)xmlSerializer.Deserialize(xmlReader);

			bool returnVal = false;

			switch (gatewayResponse.ResponseCode)
			//switch (myResult.RESPONSECODE)
			{
				case "000":
					returnVal = true;
					//PaymentResult.PaymentStatus = PaymentStatus.SUCCESS;
					break;
				case "100":
					returnVal = true;
					//PaymentResult.PaymentStatus = PaymentStatus.SUCCESS;
					break;

				case "102":
					returnVal = false;
					//PaymentResult.PaymentStatus = PaymentStatus.PENDING;
					break;

				default:
					returnVal = false;
					//PaymentResult.PaymentStatus = PaymentStatus.FAILED;
					break;
			}

			return returnVal;
		}
	}
}