using System.Xml.Serialization;

namespace Nop.Plugin.Payments.OpenEdge.Models;

[XmlRoot("RESULT")]
public class GatewayResponse
{
	[XmlElement(ElementName = "ALIAS")]
	public string Alias { get; set; }

	[XmlElement(ElementName = "APPROVALCODE")]
	public string ApprovalCode { get; set; }

	[XmlElement(ElementName = "APPROVEDAMOUNT")]
	public decimal ApprovedAmount { get; set; }

	[XmlElement(ElementName = "AUTHORIZEDAMOUNT")]
	public decimal AuthorizedAmount { get; set; }

	[XmlElement(ElementName = "AVSRESPONSECODE")]
	public string AvsResponseCode { get; set; }

	[XmlElement(ElementName = "BATCHAMOUNT")]
	public decimal BatchAmount { get; set; }

	[XmlElement(ElementName = "BATCHNO")]
	public string BatchNo { get; set; }

	[XmlElement(ElementName = "CAPTUREDAMOUNT")]
	public decimal CapturedAmount { get; set; }

	[XmlElement(ElementName = "CARDBRAND")]
	public string CardBrand { get; set; }

	[XmlElement(ElementName = "CARDBRANDSHORT")]
	public string CardBrandShort { get; set; }

	[XmlElement(ElementName = "CARDCODERESPONSE")]
	public string CardCodeResponse { get; set; }

	[XmlElement(ElementName = "")]
	public string CardHolderName { get; set; }

	[XmlElement(ElementName = "CARDTYPE")]
	public string CardType { get; set; }

	[XmlElement(ElementName = "CUSTOMERRECEIPT")]
	public string CustomerReceipt { get; set; }

	[XmlElement(ElementName = "ENTRYMETHOD")]
	public string EntryMethod { get; set; }

	[XmlElement(ElementName = "")]
	public string ExpMonth { get; set; }

	[XmlElement(ElementName = "EXPYEAR")]
	public string ExpYear { get; set; }

	[XmlElement(ElementName = "INDUSTRY")]
	public string Industry { get; set; }

	[XmlElement(ElementName = "INVOICENUMBER")]
	public string InvoiceNumber { get; set; }

	[XmlElement(ElementName = "MASKEDCARDNUMBER")]
	public string MaskedCardNumber { get; set; }

	[XmlElement(ElementName = "MERCHANTRECEIPT")]
	public string MerchantReceipt { get; set; }

	[XmlElement(ElementName = "ORDERID")]
	public string OrderId { get; set; }

	[XmlElement(ElementName = "ORIGINALAUTHORIZEDAMOUNT")]
	public decimal OriginalAuthorizedAmount { get; set; }

	[XmlElement(ElementName = "ORIGINALPROCESSORRESPONSE")]
	public string OriginalProcessorResponse { get; set; }

	[XmlElement(ElementName = "ORIGINALREQUESTEDAMOUNT")]
	public decimal OriginalRequestedAmount { get; set; }

	[XmlElement(ElementName = "ORIGINALRESPONSECODE")]
	public string OriginalResponseCode { get; set; }

	[XmlElement(ElementName = "ORIGINALRESPONSEDESCRIPTION")]
	public string OriginalResponseDescription { get; set; }

	[XmlElement(ElementName = "ORIGINALTRANSACTIONDATETIME")]
	public string OriginalTransactionDateTime { get; set; }

	[XmlElement(ElementName = "ORIGINALTRANSACTIONTYPE")]
	public string OriginalTransactionType { get; set; }

	[XmlElement(ElementName = "PROCESSORRESPONSE")]
	public string ProcessorResponse { get; set; }

	[XmlElement(ElementName = "RECEIPTID")]
	public string ReceiptId { get; set; }

	[XmlElement(ElementName = "RESPONSECODE")]
	public string ResponseCode { get; set; }

	[XmlElement(ElementName = "RESPONSEDESCRIPTION")]
	public string ResponseDescription { get; set; }

	[XmlElement(ElementName = "RETURNID")]
	public string ReturnId { get; set; }

	[XmlElement(ElementName = "STATE")]
	public string State { get; set; }

	[XmlElement(ElementName = "TAXAMOUNT")]
	public decimal TaxAmount { get; set; }

	[XmlElement(ElementName = "")]
	public decimal TipAmount { get; set; }

	[XmlElement(ElementName = "TRANSACTIONDATETIME")]
	public string TransactionDateTime { get; set; }

	[XmlElement(ElementName = "TRANSACTIONID")]
	public string TransactionId { get; set; }

	[XmlElement(ElementName = "")]
	public string TransactionType { get; set; }

	[XmlElement(ElementName = "USERDEFINED1")]
	public string UserDefined1 { get; set; }

	[XmlElement(ElementName = "")]
	public string UserDefined2 { get; set; }

	[XmlElement(ElementName = "USERDEFINED3")]
	public string UserDefined3 { get; set; }
}
