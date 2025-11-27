using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.OpenEdge.Models;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.OpenEdge.HostPay
{
	internal class HostPayHelper
	{
		private OpenEdgePaymentSettings _openEdgePaymentSettings;
		private HostPayConfig _hostPayConfig;
		private readonly ILogger _logger;


		public HostPayHelper(OpenEdgePaymentSettings openEdgePaymentSettings,
			HostPayConfig hostPayConfig, 
			ILogger logger)
		{
			_openEdgePaymentSettings = openEdgePaymentSettings;
			_hostPayConfig = hostPayConfig;
			_logger = logger;
		}


		internal string XMLPostWriter(HostPayTransaction request, string xmlPostUse)
        //internal string XMLPostWriter(HostPayTransaction request)
		{
			//XML post data sent to Gateway for result polling.
			StringBuilder xmlPostData = new StringBuilder();

			//Create new instance of XML settings, set indent.
			XmlWriterSettings ws = new XmlWriterSettings();
			ws.Indent = false;

			//Write XML Post.
			using (XmlWriter hpfXmlWriter = XmlWriter.Create(xmlPostData, ws))
			{
				hpfXmlWriter.WriteStartDocument();
				hpfXmlWriter.WriteStartElement("Request");

				AddProperty(hpfXmlWriter, "XWebID", _openEdgePaymentSettings.XWebId);
				AddProperty(hpfXmlWriter, "XWebTerminalID", _openEdgePaymentSettings.TerminalId);
				AddProperty(hpfXmlWriter, "XWebAuthKey", _openEdgePaymentSettings.AuthorizationKey);
				AddProperty(hpfXmlWriter, "XWebIndustry", _openEdgePaymentSettings.Industry);

				switch (xmlPostUse)
				{
                    case ("QueryStatus"):
						AddProperty(hpfXmlWriter, "OrderId", request.TransactionKey);
						AddProperty(hpfXmlWriter, "TransactionType", request.TransactionType.ToString());
						break;

                    case ("SetupOTK"):
						AddProperty(hpfXmlWriter, "OrderId", request.TransactionKey);
						AddProperty(hpfXmlWriter, "Amount", request.Amount);
						AddProperty(hpfXmlWriter, "TransactionType", request.TransactionType.ToString());
                        //AddProperty(hpfXmlWriter, "OrderDescription", "Order information for BPA order.");
                        
                        //
                        // Payment page configuration
                        //
						hpfXmlWriter.WriteStartElement("HostPaySetting");
                        AddProperty(hpfXmlWriter, "DisableFraming", false);

                        //
                        // Return Options
                        //
						hpfXmlWriter.WriteStartElement("PosDevice");
						AddProperty(hpfXmlWriter, "Type", "KEYED");
						hpfXmlWriter.WriteEndElement();

                        //
                        // Return Options
                        //
						hpfXmlWriter.WriteStartElement("ReturnOption");
						AddProperty(hpfXmlWriter, "ReturnUrl", request.SuccessURL);
						hpfXmlWriter.WriteEndElement();

                        //
                        // Page customization
                        //
						hpfXmlWriter.WriteStartElement("Customization");
						hpfXmlWriter.WriteStartElement("Page");
                        AddFieldProperties(hpfXmlWriter, "CustomerInfo", "Customer Information", false, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingCustomerTitle", "Customer Title", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingFirstName", "First Name", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingMiddleName", "Middle Name", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingLastName", "Last Name", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingCompany", "Company", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingAddressOne", "Address Line 1", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingAddressTwo", "Address Line 2", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingCity", "City", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingStateOrProvince", "State or Province", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingCountryCode", "Country", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "BillingPostalCode", "Postal Code", true, false, false);
                        AddFieldProperties(hpfXmlWriter, "OrderInfo", "Order Information", false, false, true);
                        //AddFieldProperties(hpfXmlWriter, "OrderDescription", "Description", false, false, true);
                        AddFieldProperties(hpfXmlWriter, "CancelButton", "Cancel", false, false, true);
                        hpfXmlWriter.WriteEndElement(); // Page
                        hpfXmlWriter.WriteEndElement(); // Customization

                        hpfXmlWriter.WriteEndElement(); // HostPaySetting
						break;

                    case ("Capture"):
						AddProperty(hpfXmlWriter, "OrderId", request.TransactionKey);
						AddProperty(hpfXmlWriter, "Amount", request.Amount);
						AddProperty(hpfXmlWriter, "TransactionType", request.TransactionType.ToString());
						break;


                    case ("Refund"):
						AddProperty(hpfXmlWriter, "OrderId", request.TransactionKey);
						AddProperty(hpfXmlWriter, "Amount", request.Amount);
						AddProperty(hpfXmlWriter, "TransactionType", request.TransactionType.ToString());
						break;
				}
				hpfXmlWriter.WriteEndElement();
				hpfXmlWriter.WriteEndDocument();
			}

			return xmlPostData.ToString();
		}

		private void AddFieldProperties(XmlWriter xmlWriter, string fieldName, string labelText, bool editable, bool required, bool visible)
		{
			xmlWriter.WriteStartElement(fieldName);
			AddProperty(xmlWriter, "Label", labelText);
			AddProperty(xmlWriter, "Edit", editable);
			AddProperty(xmlWriter, "Required", required);
			AddProperty(xmlWriter, "Visible", visible);
			xmlWriter.WriteEndElement();
		}

		private static void AddProperty(XmlWriter xmlWriter, string property, string value)
		{
			if (value != null)
			{
				xmlWriter.WriteStartElement(property);
				xmlWriter.WriteString(value);
				xmlWriter.WriteEndElement();
			}
		}

		private static void AddProperty(XmlWriter xmlWriter, string property, bool value)
		{
			xmlWriter.WriteStartElement(property);
			xmlWriter.WriteString(value.ToString().ToUpper());
			xmlWriter.WriteEndElement();
		}

		private static void AddProperty(XmlWriter xmlWriter, string property, decimal value)
		{
			xmlWriter.WriteStartElement(property);
			xmlWriter.WriteString(value.ToString("0.00"));
			xmlWriter.WriteEndElement();
		}


        //Gateway Call, used in OTK call and result polling
        //		internal async Task<string> GatewayCall(string GatewayPostDataString, XwebTransaction request)
		internal async Task<string> GatewayCallAsync(string gatewayPostDataString, RequestType requestType)
		{
			await _logger.InformationAsync("Gateway request: " + gatewayPostDataString, (Exception)null, (Customer)null);
			string result;

			try
			{
                //Create connection to XWeb test gateway
				string requestUrl = ((requestType == RequestType.Setup) ? _hostPayConfig.SetupUrl : _hostPayConfig.ServiceUrl);
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);

                //Connection settings
				request.KeepAlive = false;
				request.Method = "POST";
				request.ContentType = "application/xml";
				request.Accept = "application/xml";

                //Connection settings
				byte[] tokByteArray = Encoding.UTF8.GetBytes(gatewayPostDataString);
				request.ContentLength = tokByteArray.Length;

                //Create OTK request stream to gateway
				Stream gatewayDataStream = await request.GetRequestStreamAsync();
                //Stream settings 
				gatewayDataStream.Write(tokByteArray, 0, tokByteArray.Length);
				gatewayDataStream.Close();
				gatewayDataStream = (await request.GetResponseAsync()).GetResponseStream();
				StreamReader gatewayResponseReader = new StreamReader(gatewayDataStream);
                //Read and save connection response
				result = gatewayResponseReader.ReadToEnd();
                //Close connections
				gatewayResponseReader.Close();
				gatewayDataStream.Close();

				await _logger.InformationAsync("Gateway result: " + result, (Exception)null, (Customer)null);
			}
			catch (Exception ex)
			{
                //Saves error message to GatewayResponse variable, passed back to parent application.
				result = GenerateXmlError(ex.Message.ToString());
				await _logger.ErrorAsync("Gateway request failed: " + result, ex, (Customer)null);
			}
			return result;
		}

		private string GenerateXmlError(string errorMessage)
		{
            //XML post data sent to Gateway for result polling.
			StringBuilder stringBuilder = new StringBuilder();

			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = false;
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
			{
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("GatewayResponse");
				AddProperty(xmlWriter, "ResponseCode", "ERR");
				AddProperty(xmlWriter, "ResponseDescription", errorMessage);
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
			}
			return stringBuilder.ToString();
		}
	}
}