using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.OpenEdge.Infrastructure;

public class RouteProvider : IRouteProvider
{
	public int Priority => -1;

	public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
	{
		endpointRouteBuilder.MapControllerRoute("Plugin.Payments.OpenEdge.Payment", "Plugins/PaymentOpenEdge/Payment", new
		{
			controller = "PaymentOpenEdge",
			action = "Payment"
		});
		endpointRouteBuilder.MapControllerRoute("Plugin.Payments.OpenEdge.Return", "Plugins/PaymentOpenEdge/Return", new
		{
			controller = "PaymentOpenEdge",
			action = "Return"
		});
	}
}
