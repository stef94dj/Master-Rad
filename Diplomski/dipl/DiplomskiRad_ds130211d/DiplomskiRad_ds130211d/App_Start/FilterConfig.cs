using System.Web;
using System.Web.Mvc;

namespace DiplomskiRad_ds130211d
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
