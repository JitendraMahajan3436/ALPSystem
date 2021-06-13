using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.BaseManagement
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void ExecuteCore()
        {
            int culture = 0;
            if (this.Session == null || this.Session["CurrentCulture"] == null)
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Culture"], out culture);
                this.Session["CurrentCulture"] = culture;
            }
            else
            {
                culture = (int)this.Session["CurrentCulture"];
            }
            // calling CultureHelper class properties for setting  
            CultureHelper.CurrentCulture = culture;

            base.ExecuteCore();

            //this.IsUserLogin();

        }
        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            FDSession fdSession = (FDSession)this.Session["FDSession"];

            if (fdSession == null || fdSession.userId == 0)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new JsonResult { Data = "LogOut", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    filterContext.Result = RedirectToAction("Index", "User");
                    //filterContext.Result = RedirectToAction("Index", "AppStart");
                }

            }
            else
            {
                //base.Execute(filterContext.RequestContext);
            }
        }

        public ActionResult IsSession()
        {
            FDSession fdSession = (FDSession)this.Session["FDSession"];

            if (fdSession == null || fdSession.userId == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}