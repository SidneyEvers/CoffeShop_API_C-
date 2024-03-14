using System.Web;
using System.Web.Mvc;

namespace Sistema_Gerenciamento_Cafeteria
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
