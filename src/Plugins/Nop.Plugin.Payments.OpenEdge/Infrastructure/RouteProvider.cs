//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Routing;
//using Nop.Web.Framework.Mvc.Routing;

//namespace Nop.Plugin.Payments.OpenEdge.Infrastructure;

//public class RouteProvider : IRouteProvider
//{
//	public int Priority => -1;

//	public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
//	{
//		endpointRouteBuilder.MapControllerRoute("Plugin.Payments.OpenEdge.Payment", "Admin/PaymentOpenEdge/Payment", new
//		{
//			controller = "PaymentOpenEdge",
//			action = "Payment"
//		});
//		endpointRouteBuilder.MapControllerRoute("Plugin.Payments.OpenEdge.Return", "Admin/PaymentOpenEdge/Return", new
//		{
//			controller = "PaymentOpenEdge",
//			action = "Return"
//		});
//	}
//}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Payments.OpenEdge.Infrastructure;

/// <summary>
/// Represents the plugin route provider
/// </summary>
public class RouteProvider : BaseRouteProvider, IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var lang = GetLanguageRoutePattern();

        endpointRouteBuilder.MapControllerRoute(name: OpenEdgeDefaults.Route.Configuration,
            pattern: "Admin/PaymentOpenEdge/Configure",
            defaults: new { controller = "PaymentOpenEdge", action = "Configure", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: OpenEdgeDefaults.Route.Payment,
            pattern: "Admin/PaymentOpenEdge/Payment",
            defaults: new { controller = "PaymentOpenEdge", action = "Payment", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: OpenEdgeDefaults.Route.Return,
            pattern: "Admin/PaymentOpenEdge/Return",
            defaults: new { controller = "PaymentOpenEdge", action = "Return", area = AreaNames.ADMIN });

        //		endpointRouteBuilder.MapControllerRoute("Plugin.Payments.OpenEdge.Payment", "Admin/PaymentOpenEdge/Payment", new
        //		{
        //			controller = "PaymentOpenEdge",
        //			action = "Payment"
        //		});
        //		endpointRouteBuilder.MapControllerRoute("Plugin.Payments.OpenEdge.Return", "Admin/PaymentOpenEdge/Return", new
        //		{
        //			controller = "PaymentOpenEdge",
        //			action = "Return"
        //		});


    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}
