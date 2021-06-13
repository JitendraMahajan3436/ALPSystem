using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.BaseManagement
{
    public class DocumentationsController : BaseController
    {
        // GET: Documentations
        public ActionResult Index(String documentName)
        {
            ViewBag.documentName = documentName;
            return View();
        }
    }
}