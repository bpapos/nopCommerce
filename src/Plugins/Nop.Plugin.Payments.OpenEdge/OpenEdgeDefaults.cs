using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.OpenEdge;

public class OpenEdgeDefaults
{
    #region Route names

    /// <summary>
    /// Represents the route names
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string Configuration => "Plugin.Payments.OpenEdge.Configure";

        /// <summary>
        /// Gets the payment route name        /// </summary>
        public static string Payment => "Plugin.Payments.OpenEdge.Payment";

        /// <summary>
        /// Gets the return route name
        /// </summary>
        public static string Return => "Plugin.Payments.Openedge.Return";

    }

    #endregion

}
