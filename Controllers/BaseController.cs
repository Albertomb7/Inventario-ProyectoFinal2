using System.Web.Mvc;

namespace ProyectoWebInventario.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            ViewBag.CurrentMenu = controllerName;
            base.OnActionExecuting(filterContext);
        }
    }
}