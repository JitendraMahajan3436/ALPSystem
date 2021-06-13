using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers
{
    public class HomeController : BaseController
    {
        GlobalData globalData = new GlobalData();
        REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        public ActionResult Index()
        {
            FDSession fdSession = (FDSession)this.Session["FDSession"];
            decimal employeeUserID = fdSession.userId;
            try
            {

               IEnumerable<RS_Roles> empRoleObj = db.RS_User_Roles.Where(a => a.Employee_ID == employeeUserID).Select(a => a.RS_Roles).OrderBy(a => a.Sort_Order);
               fdSession.rolesObj = empRoleObj;
                IEnumerable<RS_Menus> menuObj = db.RS_Menu_Role.Where(a => a.Role_ID == 3).OrderBy(a => a.Sort_Order).Select(a => a.RS_Menus);
                this.Session["FDSession"] = fdSession;

            }
            catch (Exception ex)
            {
                General genObj = new General();
                genObj.addControllerException(ex, "Home", "Index", 1);
            }
            return View();
        }

        public ActionResult Shop()
        {
            globalData = new GlobalData();
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult GetImages()
        {

            List<string> imglist = new List<string>();

            string path = System.Web.HttpContext.Current.Server.MapPath("~/slider/");
            //string path = HostingEnvironment.MapPath("~/img/");

            if (path.EndsWith("\\"))
            {

                path = path.Remove(path.Length - 1);

            }


            Uri pathUri = new Uri(path, UriKind.Absolute);

            //DirectoryInfo dirinfo = new DirectoryInfo(HostingEnvironment.MapPath("~/img/"));

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {

                Uri filePathUri = new Uri(file, UriKind.Absolute);

                imglist.Add("/" + pathUri.MakeRelativeUri(filePathUri).ToString());
            }


            return Json(new
            {
                imglist = imglist
            }, JsonRequestBehavior.AllowGet);

        }
    }
}