using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ranglerz_project.Services
{
    public class SessionCheck : ActionFilterAttribute  
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ses = filterContext.HttpContext.Session["username"];
            if (ses == null)
            {
              
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index"}));
            
            }
            base.OnActionExecuting(filterContext);
        }

    }
}