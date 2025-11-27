using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.OpenEdge;

public class OpenEdgePaymentSettings : ISettings
{
	public bool UseSandbox { get; set; }
	public string XWebId { get; set; }
	public string TerminalId { get; set; }
	public string AuthorizationKey { get; set; }
	public string Industry { get; set; }
}
