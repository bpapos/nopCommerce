using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.OpenEdge.Components
{
	[ViewComponent(Name = "PaymentOpenEdge")]
	public class PaymentOpenEdgeViewComponent : NopViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View("~/Plugins/Payments.OpenEdge/Views/PaymentInfo.cshtml");
		}
	}
}
