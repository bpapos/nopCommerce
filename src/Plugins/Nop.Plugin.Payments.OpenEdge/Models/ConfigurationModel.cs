using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.OpenEdge.Models;

public record ConfigurationModel : BaseNopModel
{
	public int ActiveStoreScopeConfiguration { get; set; }

	[NopResourceDisplayName("Plugins.Payments.OpenEdge.UseSandbox")]
	public bool UseSandbox { get; set; }

	public bool UseSandbox_OverrideForStore { get; set; }

	[NopResourceDisplayName("Plugins.Payments.OpenEdge.XWebId")]
	public string XWebId { get; set; }

	public bool XWebId_OverrideForStore { get; set; }

	[NopResourceDisplayName("Plugins.Payments.OpenEdge.TerminalId")]
	public string TerminalId { get; set; }

	public bool TerminalId_OverrideForStore { get; set; }

	[NopResourceDisplayName("Plugins.Payments.OpenEdge.AuthorizationKey")]
	public string AuthorizationKey { get; set; }

	public bool AuthorizationKey_OverrideForStore { get; set; }

	[NopResourceDisplayName("Plugins.Payments.OpenEdge.Industry")]
	public string Industry { get; set; }

	public bool Industry_OverrideForStore { get; set; }


}
