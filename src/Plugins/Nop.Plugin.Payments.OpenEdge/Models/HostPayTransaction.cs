using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.OpenEdge.HostPay;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.OpenEdge.Models;

public record HostPayTransaction : BaseNopModel
{
	public string PaymentUrl { get; set; }

	public string CartId { get; set; }

	public TransactionType TransactionType { get; set; }

	public string TransactionKey { get; set; }

	public decimal Amount { get; set; }

	public bool StoreInVault { get; set; }

	public string VaultId { get; set; }

	public decimal Gratuity { get; set; }

	public string Address { get; set; }

	public string ZipCode { get; set; }

	public string InvoiceNumber { get; set; }

	public int CustomerId { get; set; }

	internal bool AmountLocked { get; set; }

	internal bool GratuityAmountLocked { get; set; }

	internal bool AddressLocked { get; set; }

	internal bool ZipCodeLocked { get; set; }

	internal bool InvoiceNumberLocked { get; set; }

	internal bool EnablePartialApprovals { get; set; }

	public string SuccessURL { get; set; }

	public string FailureURL { get; set; }

	private string ResultString { get; set; }

	public Order Order { get; set; }

}
