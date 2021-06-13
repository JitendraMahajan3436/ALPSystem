using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.BaseManagement
{
    public class AppStartController : Controller
    {
        // GET: AppStart
        public ActionResult Index()
        {
            ViewBag.Title = "App Starting...";
            if (!String.IsNullOrWhiteSpace(HttpContext.User.Identity.Name))
            {
                ViewBag.LogonUser = HttpContext.User.Identity.Name;
            }
            else if (!String.IsNullOrWhiteSpace(HttpContext.Request.LogonUserIdentity.Name))
            {
                ViewBag.LogonUser = HttpContext.Request.LogonUserIdentity.Name;
            }
            return View();
        }
    }
}