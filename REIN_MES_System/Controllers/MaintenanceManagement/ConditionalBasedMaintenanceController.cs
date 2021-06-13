using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Controllers.BaseManagement;
using System.Web.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace ZHB_AD.Controllers
{
    /* Class Name               : ConditionalBasedMaintenance
  *  Description                : To create,edit,delete and show all machines against Station for Conditional Based Maintenance 
  *  Author, Timestamp          : Ajay Wagh      
  */
    public class ConditionalBasedMaintenanceController : BaseController
    {
        #region Variables declaration
        //private MTTUWEntities db1 = new MTTUWEntities();
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        public GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        MM_MT_Conditional_Based_Maintenance cbm = new MM_MT_Conditional_Based_Maintenance();
        #endregion

        #region Get Details of Conditional Based Maintenance
        /*
         * Action Name          : Index
         * Input Parameter      : None
         * Return Type          : ActionResult
         * Author & Time Stamp  : Ajay Wagh
         * Description          : Get the list of Machines(Conditional Based Maintenance)
         */

        // GET: ConditionalBasedMaintenance
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var MM_MT_Conditional_Based_Maintenance = db.MM_MT_Conditional_Based_Maintenance.Include(m => m.MM_MTTUW_Employee).Include(m => m.MM_MTTUW_Employee1).Include(m => m.MM_MT_MTTUW_Machines);

            globalData.pageTitle = ResourceModules.CBM_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Conditional Based Maintenance";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceDisplayName.Machine + " " + ResourceGlobal.Lists;
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            ViewBag.GlobalDataModel = globalData;
            // TempData["globalData"] = globalData;

            var plantId = ((FDSession)this.Session["FDSession"]).plantId;

            return View(MM_MT_Conditional_Based_Maintenance.Where(m => m.Plant_ID == plantId).ToList());
        }


        /*
        * Action Name          : Details
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Get Details of Machines(Conditional Based Maintenance)
        */
        // GET: ConditionalBasedMaintenance/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Conditional_Based_Maintenance MM_MT_Conditional_Based_Maintenance = db.MM_MT_Conditional_Based_Maintenance.Find(id);
            if (MM_MT_Conditional_Based_Maintenance == null)
            {
                return HttpNotFound();
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.CBM_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Conditional Based Maintenance";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceDisplayName.Machine + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            //TempData["globalData"] = globalData;

            return View(MM_MT_Conditional_Based_Maintenance);
        }
        #endregion
        public List<browseResults> GetCBMIds()
        {
            try
            {
                string url = WebConfigurationManager.AppSettings["IoTBrowseURL"];

                var response = new WebClient().DownloadString(url);
                browseIOTResults browserResultObj = (browseIOTResults)JsonConvert.DeserializeObject(response, typeof(browseIOTResults));
                if (browserResultObj.succeeded)
                {
                    return browserResultObj.browseResults;
                }
                else
                {
                    return browserResultObj.browseResults;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #region Create Conditional Based Maintenance
        /*
        * Action Name          : Create
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Load form (Add Machines to Conditional Based Maintenance)
        */
        // GET: ConditionalBasedMaintenance/Create
        public class browseIOTResults
        {

            public List<browseResults> browseResults { get; set; }
            public Boolean succeeded { get; set; }
            public string reason { get; set; }

            // public string browseResult { get; set; }
        }
        public class browseResults
        {
            public string id { get; set; }
            // public string browseResult { get; set; }
        }

        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();

            globalData.pageTitle = ResourceModules.CBM_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Conditional Based Maintenance";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            var obj = GetCBMIds();
            //foreach(var item in obj)
            //{

            //}
            //ViewBag.IOT_Tag_Name = obj.ToList();
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name");
            if (obj != null)
                ViewBag.IOT_Tag_Name = new SelectList(obj, "id", "id");
            else
                ViewBag.IOT_Tag_Name = new SelectList(db.MM_MTTUW_IoT_Tags, "Tag_Name", "Tag_Name");
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new {Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")", Machine_ID = a.Machine_ID})
            //    , "Machine_ID", "Machine_Name");
            ViewBag.C_ID = new SelectList(db.MM_Parameter_Category, "C_ID", "C_Name", 0);
            //ViewBag.Category_ID = new SelectList(db.MM_CBM_Parameter_Category, "Category_ID", "Category_Name");
            ViewBag.Image_ID = new SelectList(db.MM_CBM_Category_Image.Where(m => m.C_ID == 0), "Image_ID", "Image_Name");
            ViewBag.M_ID = new SelectList(db.MM_MTTUW_Unit_OF_Measurement, "M_ID", "Measurement_Name");
            ViewBag.Type_ID = new SelectList(db.MM_CBM_Parameter_Type, "Type_ID", "Type_Name");

            return View();
        }

        /*
      * Action Name          : Create
      * Input Parameter      : MM_MT_Conditional_Based_Maintenance object
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Add Machines to Conditional Based Maintenance
      */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CBM_ID,Machine_ID,M_ID,Sequence_No,IsActive,Type_ID,Category_ID,Maintenance_User_ID,Data_Retention_Period,Machine_Parameter,Min_Val,Max_Val,Green_Min_Val,Green_Max_Val,Warning_Min_Val,Warning_Max_Val,Parameter_Category,IOT_Tag_Name,Scale_Denominator,CBM_Check_Type,UOM,Receipient_Email,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,mails,users,Is_InterLock,C_ID,Image_ID,Data_Retention_Period")] MM_MT_Conditional_Based_Maintenance MM_MT_Conditional_Based_Maintenance)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                    if (MM_MT_Conditional_Based_Maintenance.Scale_Denominator == null)
                    {
                        MM_MT_Conditional_Based_Maintenance.Scale_Denominator = 1;
                    }
                    var mId = MM_MT_Conditional_Based_Maintenance.M_ID;
                    var unit = db.MM_MTTUW_Unit_OF_Measurement.Where(m => m.M_ID == mId).Select(m => m.Measurement_Name).FirstOrDefault();
                    MM_MT_Conditional_Based_Maintenance.UOM = unit.ToString();
                    var cId = MM_MT_Conditional_Based_Maintenance.C_ID;
                    //var cId = MM_MT_Conditional_Based_Maintenance.Category_ID;
                    var category = db.MM_Parameter_Category.Where(m => m.C_ID == cId).Select(m => m.C_Name).FirstOrDefault();
                    MM_MT_Conditional_Based_Maintenance.Parameter_Category = category.ToString();
                    MM_MT_Conditional_Based_Maintenance.Plant_ID = plantId;

                    MM_MT_Conditional_Based_Maintenance.Data_Retention_Period = MM_MT_Conditional_Based_Maintenance.Data_Retention_Period;
                    MM_MT_Conditional_Based_Maintenance.Inserted_Date = DateTime.Now;
                    MM_MT_Conditional_Based_Maintenance.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    MM_MT_Conditional_Based_Maintenance.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.MM_MT_Conditional_Based_Maintenance.Add(MM_MT_Conditional_Based_Maintenance);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.CBM;
                    globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CBM;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
            }


            globalData.pageTitle = ResourceModules.CBM_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Conditional Based Maintenance";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", MM_MT_Conditional_Based_Maintenance.Machine_ID);
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new { Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")" , Machine_ID = a.Machine_ID})
            //    , "Machine_ID", "Machine_Name");
            ViewBag.M_ID = new SelectList(db.MM_MTTUW_Unit_OF_Measurement, "M_ID", "Measurement_Name", MM_MT_Conditional_Based_Maintenance.M_ID);
            ViewBag.C_ID = new SelectList(db.MM_Parameter_Category, "C_ID", "C_Name", MM_MT_Conditional_Based_Maintenance.C_ID);
            //ViewBag.Category_ID = new SelectList(db.MM_CBM_Parameter_Category, "Category_ID", "Category_Name", MM_MT_Conditional_Based_Maintenance.Category_ID);
            ViewBag.Image_ID = new SelectList(db.MM_CBM_Category_Image, "Image_ID", "Image_Name", MM_MT_Conditional_Based_Maintenance.Image_ID);
            ViewBag.Type_ID = new SelectList(db.MM_CBM_Parameter_Type, "Type_ID", "Type_Name", MM_MT_Conditional_Based_Maintenance.Type_ID);
            return View(MM_MT_Conditional_Based_Maintenance);
        }
        #endregion

        #region Edit Conditional Based Maintenance
        /*
      * Action Name          : Edit
      * Input Parameter      : id
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Load Conditional Based Maintenance edit form
      */
        // GET: ConditionalBasedMaintenance/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Conditional_Based_Maintenance MM_MT_Conditional_Based_Maintenance = db.MM_MT_Conditional_Based_Maintenance.Find(id);
            if (MM_MT_Conditional_Based_Maintenance == null)
            {
                return HttpNotFound();
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.CBM_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Conditional Based Maintenance";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", MM_MT_Conditional_Based_Maintenance.Machine_ID);
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new { Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")", Machine_ID = a.Machine_ID }),
            //    "Machine_ID", "Machine_Name", MM_MT_Conditional_Based_Maintenance.Machine_ID);
            ViewBag.M_ID = new SelectList(db.MM_MTTUW_Unit_OF_Measurement, "M_ID", "Measurement_Name", MM_MT_Conditional_Based_Maintenance.M_ID);
            ViewBag.C_ID = new SelectList(db.MM_Parameter_Category, "C_ID", "C_Name", MM_MT_Conditional_Based_Maintenance.C_ID);
            //ViewBag.Category_ID = new SelectList(db.MM_CBM_Parameter_Category, "Category_ID", "Category_Name", MM_MT_Conditional_Based_Maintenance.Category_ID);
            ViewBag.Image_ID = new SelectList(db.MM_CBM_Category_Image.Where(m => m.C_ID == MM_MT_Conditional_Based_Maintenance.C_ID), "Image_ID", "Image_Name", MM_MT_Conditional_Based_Maintenance.Image_ID);
            ViewBag.CategoryImage = (from data in db.MM_CBM_Category_Image
                                     where data.C_ID == MM_MT_Conditional_Based_Maintenance.C_ID
                                     select new CategoryImage
                                     {
                                         Id = data.Image_ID,
                                         Content = data.Image_Content,
                                         Content_Type = data.Content_Type
                                     }).ToList();
            ViewBag.Type_ID = new SelectList(db.MM_CBM_Parameter_Type, "Type_ID", "Type_Name", MM_MT_Conditional_Based_Maintenance.Type_ID);
            return View(MM_MT_Conditional_Based_Maintenance);
        }

        /*
      * Action Name          : Edit
      * Input Parameter      : MM_MT_Conditional_Based_Maintenance 
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Edit Conditional Based Maintenance of Machine
      */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CBM_ID,Machine_ID,M_ID,Sequence_No,IsActive,Maintenance_User_ID,Data_Retention_Period,Type_ID,Category_ID,IOT_Tag_Name,Machine_Parameter,Min_Val,Max_Val,Green_Min_Val,Green_Max_Val,Warning_Min_Val,Warning_Max_Val,Parameter_Category,Scale_Denominator,CBM_Check_Type,UOM,Receipient_Email,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,mails,users,Is_InterLock,C_ID,Image_ID,Data_Retention_Period")] MM_MT_Conditional_Based_Maintenance MM_MT_Conditional_Based_Maintenance)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cbm = db.MM_MT_Conditional_Based_Maintenance.Find(MM_MT_Conditional_Based_Maintenance.CBM_ID);

                    var mId = MM_MT_Conditional_Based_Maintenance.M_ID;
                    var unit = db.MM_MTTUW_Unit_OF_Measurement.Where(m => m.M_ID == mId).Select(m => m.Measurement_Name).FirstOrDefault();
                    cbm.UOM = unit.ToString();
                    var cId = MM_MT_Conditional_Based_Maintenance.C_ID;
                    //var cId = cbm.Category_ID;
                    var category = db.MM_Parameter_Category.Where(m => m.C_ID == cId).Select(m => m.C_Name).FirstOrDefault();
                    cbm.Parameter_Category = category.ToString();

                    cbm.Machine_Parameter = MM_MT_Conditional_Based_Maintenance.Machine_Parameter;
                    cbm.Max_Val = MM_MT_Conditional_Based_Maintenance.Max_Val;
                    cbm.Min_Val = MM_MT_Conditional_Based_Maintenance.Min_Val;
                    cbm.Green_Max_Val = MM_MT_Conditional_Based_Maintenance.Green_Max_Val;
                    cbm.Green_Min_Val = MM_MT_Conditional_Based_Maintenance.Green_Min_Val;
                    cbm.Warning_Max_Val = MM_MT_Conditional_Based_Maintenance.Warning_Max_Val;
                    cbm.Warning_Min_Val = MM_MT_Conditional_Based_Maintenance.Warning_Min_Val;
                    cbm.Scale_Denominator = MM_MT_Conditional_Based_Maintenance.Scale_Denominator;
                    //cbm.UOM = MM_MT_Conditional_Based_Maintenance.UOM;
                    cbm.Sequence_No = MM_MT_Conditional_Based_Maintenance.Sequence_No;
                    cbm.Machine_ID = MM_MT_Conditional_Based_Maintenance.Machine_ID;
                    cbm.Type_ID = MM_MT_Conditional_Based_Maintenance.Type_ID;
                    cbm.Category_ID = MM_MT_Conditional_Based_Maintenance.Category_ID;
                    cbm.C_ID = MM_MT_Conditional_Based_Maintenance.C_ID;
                    cbm.M_ID = MM_MT_Conditional_Based_Maintenance.M_ID;
                    cbm.Image_ID = MM_MT_Conditional_Based_Maintenance.Image_ID;
                    // cbm.Data_Retention_Period = MM_MT_Conditional_Based_Maintenance.Data_Retention_Period;
                    cbm.IsActive = MM_MT_Conditional_Based_Maintenance.IsActive;
                    cbm.Updated_Date = DateTime.Now;
                    cbm.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    cbm.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    cbm.Is_Deleted = MM_MT_Conditional_Based_Maintenance.Is_Deleted;
                    cbm.Is_Purgeable = MM_MT_Conditional_Based_Maintenance.Is_Purgeable;
                    cbm.Is_Transfered = MM_MT_Conditional_Based_Maintenance.Is_Transfered;
                    cbm.Data_Retention_Period = MM_MT_Conditional_Based_Maintenance.Data_Retention_Period;
                    cbm.Is_Edited = true;
                    db.Entry(cbm).State = EntityState.Modified;
                    db.SaveChanges();


                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.CBM;
                    globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;
                    ViewBag.GlobalDataModel = globalData;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exp)
            {

                generalHelper.addControllerException(exp, "ConditionalBasedMaintenanceController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CBM_Config;
                globalData.messageDetail = exp.Message;
                this.Session["globalData"] = globalData;
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.CBM_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Conditional Based Maintenance";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", MM_MT_Conditional_Based_Maintenance.Machine_ID);
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new { Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")" , Machine_ID = a.Machine_ID}),
            //    "Machine_ID", "Machine_Name", MM_MT_Conditional_Based_Maintenance.Machine_ID);
            ViewBag.M_ID = new SelectList(db.MM_MTTUW_Unit_OF_Measurement, "M_ID", "Measurement_Name", MM_MT_Conditional_Based_Maintenance.M_ID);
            ViewBag.C_ID = new SelectList(db.MM_Parameter_Category, "C_ID", "C_Name", MM_MT_Conditional_Based_Maintenance.C_ID);
            // ViewBag.Category_ID = new SelectList(db.MM_CBM_Parameter_Category, "Category_ID", "Category_Name", MM_MT_Conditional_Based_Maintenance.Category_ID);
            ViewBag.Image_ID = new SelectList(db.MM_CBM_Category_Image, "Image_ID", "Image_Name", MM_MT_Conditional_Based_Maintenance.Image_ID);
            ViewBag.Type_ID = new SelectList(db.MM_CBM_Parameter_Type, "Type_ID", "Type_Name", MM_MT_Conditional_Based_Maintenance.Type_ID);
            return View(MM_MT_Conditional_Based_Maintenance);
        }
        #endregion

        #region Delete Conditional Based maintenance
        /*
      * Action Name          : Delete
      * Input Parameter      : id
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : load specified machine maintenance for deletion 
      */

        // GET: ConditionalBasedMaintenance/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Conditional_Based_Maintenance MM_MT_Conditional_Based_Maintenance = db.MM_MT_Conditional_Based_Maintenance.Find(id);
            if (MM_MT_Conditional_Based_Maintenance == null)
            {
                return HttpNotFound();
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.CBM_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Conditional Based Maintenance";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(MM_MT_Conditional_Based_Maintenance);
        }

        /*
     * Action Name          : Delete
     * Input Parameter      : id
     * Return Type          : ActionResult
     * Author & Time Stamp  : Ajay Wagh
     * Description          : Delete machine maintenance 
     */
        // POST: ConditionalBasedMaintenance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                MM_MT_Conditional_Based_Maintenance MM_MT_Conditional_Based_Maintenance = db.MM_MT_Conditional_Based_Maintenance.Find(id);
                db.MM_MT_Conditional_Based_Maintenance.Remove(MM_MT_Conditional_Based_Maintenance);
                db.SaveChanges();


                globalData.pageTitle = ResourceModules.CBM_Config;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "Conditional Based Maintenance";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.CBM;
                globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
                ViewBag.GlobalDataModel = globalData;
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CBM;
                globalData.messageDetail = ex.Message.ToString();
                TempData["globalData"] = globalData;
                ViewBag.GlobalDataModel = globalData;
            }
            globalData.pageTitle = "Conditional Based Maintenance";
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Conditional Based Maintenance";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return RedirectToAction("Index");
        }
        #endregion

        #region releasing object
        /*
         * Action Name          : Dispose
         * Input Parameter      : true/false
         * Return Type          : ActionResult
         * Author & Time Stamp  : Ajay Wagh
         * Description          : Delete machine maintenance 
         */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Check Conditional Based maintenance
        public ActionResult GetParameterCategoryImageName(decimal category)
        {
            var st = (from img in db.MM_CBM_Category_Image
                      where img.C_ID == category
                      select
                      new
                      {
                          Id = img.Image_ID,
                          Value = img.Image_Name
                      });
            return Json(st, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetSelectedParameterCategoryImage(decimal ID)
        //{

        //    var JsonResult = Json(ImageContent, JsonRequestBehavior.AllowGet);
        //    JsonResult.MaxJsonLength = Int32.MaxValue;
        //    return JsonResult;
        //}

        public ActionResult GetParameterCategoryImage(decimal CID, decimal CBMID)
        {
            // var selectedImageId = db.MM_CBM_Category_Image.Find(CID);
            decimal selectedImageId = 0;
            if (CBMID > 0)
                selectedImageId = Convert.ToDecimal(db.MM_MT_Conditional_Based_Maintenance.Find(CBMID).Image_ID);
            var ImageContent = (from data in db.MM_CBM_Category_Image
                                where data.C_ID == CID
                                select
                                new
                                {
                                    Id = data.Image_ID,
                                    Content = data.Image_Content,
                                    isSelected = data.Image_ID == selectedImageId ? true : false
                                }).ToList();

            //var ImageContent = db.MM_CBM_Category_Image.Where(m => m.C_ID == CID).Select(m => m.Image_Content).ToList();
            var JsonResult = Json(ImageContent, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = Int32.MaxValue;
            return JsonResult;
        }
        //public ActionResult GetParameterCategoryImage(decimal ImageID)
        //{
        //    var ImageContent = (from data in db.MM_CBM_Category_Image
        //                        where data.Image_ID == ImageID
        //                        select data.Image_Content).FirstOrDefault();
        //    //new
        //    //{
        //    //    Id = data.Image_ID,
        //    //    Content = data.Image_Content
        //    //}).ToList();

        //    //var ImageContent = db.MM_CBM_Category_Image.Where(m => m.C_ID == CID).Select(m => m.Image_Content).ToList();
        //    var JsonResult = Json(ImageContent, JsonRequestBehavior.AllowGet);
        //    JsonResult.MaxJsonLength = Int32.MaxValue;
        //    return JsonResult;
        //}
        public ActionResult CBM()
        {
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.CBM_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Conditional Based Maintenance";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceDisplayName.CBM_CheckPoint;
            globalData.contentFooter = ResourceDisplayName.CBM_CheckPoint;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Number");
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name");
            ViewBag.Machine_Parameter = new SelectList(db.MM_MT_Conditional_Based_Maintenance, "Machine_ID", "Machine_Parameter");
            ViewBag.UOM = new SelectList(db.MM_MT_Conditional_Based_Maintenance, "Machine_ID", "UOM");

            return View();
        }

        [HttpPost]
        public ActionResult CBM(FormCollection form)
        {
            string mid = form["Machine_ID"].ToString();
            string mparam = form["Machine_Parameter"].ToString();
            string u_om = form["UOM"].ToString();
            string email = "";

            decimal machineid = db.MM_MT_MTTUW_Machines.Where(x => x.Machine_ID.ToString() == mid).FirstOrDefault().Machine_ID;
            string Pid = db.MM_MT_Conditional_Based_Maintenance.Where(x => x.Machine_Parameter.ToString() == mparam).FirstOrDefault().Machine_Parameter;
            string uom = db.MM_MT_Conditional_Based_Maintenance.Where(x => x.UOM.ToString() == u_om).FirstOrDefault().UOM;
            decimal tresholdval = Convert.ToDecimal(form["Treshold"].ToString());
            Nullable<decimal> min = db.MM_MT_Conditional_Based_Maintenance.Where(x => x.Machine_ID == machineid).FirstOrDefault().Min_Val;
            Nullable<decimal> max = db.MM_MT_Conditional_Based_Maintenance.Where(x => x.Machine_ID == machineid).FirstOrDefault().Max_Val;
            bool flagAleart = true;
            email = db.MM_MT_Conditional_Based_Maintenance.Where(x => x.Machine_ID.ToString() == mid).FirstOrDefault().Receipient_Email;

            if (tresholdval >= min && tresholdval <= max)
            {
                flagAleart = true;
                globalData.stationIPAddress = GlobalOperations.GetIP();
                globalData.hostName = GlobalOperations.GetHostName();
                globalData.pageTitle = ResourceModules.CBM_Config;
                globalData.subTitle = ResourceGlobal.Details;
                globalData.controllerName = "Conditional Based Maintenance";
                globalData.actionName = ResourceGlobal.Details;
                globalData.contentTitle = ResourceDisplayName.CBM_CheckPoint;
                globalData.contentFooter = ResourceDisplayName.CBM_CheckPoint;

                globalData.isSuccessMessage = true;

                globalData.messageTitle = ResourceDisplayName.CBM_CheckPoint;
                globalData.messageDetail = ResourceMessages.Machine_Not_Healthy;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;

            }
            else
            {
                flagAleart = false;
                globalData.stationIPAddress = GlobalOperations.GetIP();
                globalData.hostName = GlobalOperations.GetHostName();
                globalData.pageTitle = ResourceModules.CBM_Config;
                globalData.subTitle = ResourceGlobal.Details;
                globalData.controllerName = "Conditional Based Maintenance";
                globalData.actionName = ResourceGlobal.Details;
                globalData.contentTitle = ResourceDisplayName.CBM_CheckPoint;
                globalData.contentFooter = ResourceDisplayName.CBM_CheckPoint;

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceDisplayName.CBM_CheckPoint;
                globalData.messageDetail = ResourceMessages.Machine_Not_Healthy;
                foreach (var item in email.Split(';').ToList())
                {
                    string bdy = "Dear All,<br/><br/>Machine has Resumed in a shop, details are given below:<br/><br/><br/>" +
                   "<table border=1><tr><th>Alert ID</th><th>Machine Name</th><th>Machine_Parameter</th><th>Machine UOM</th><th>Stop Time</th><th>Remarks</th></tr>" +
                   "<tr><td>A14511436</td><td>" + db.MM_MT_MTTUW_Machines.Where(x => x.Machine_ID.ToString() == mid).FirstOrDefault().Machine_Name.ToString() +
                   "</td><td>" + db.MM_MT_Conditional_Based_Maintenance.Where(x => x.Machine_ID.ToString() == mid).FirstOrDefault().Machine_Parameter.ToString() + "</td><td>"
                   + db.MM_MT_Conditional_Based_Maintenance.Where(x => x.Machine_ID.ToString() == mid).FirstOrDefault().UOM.ToString() +
                   "</td><td>" + " " + DateTime.Now.ToString() + "</td><td>Oil Leakage</td></tr></table><br/><br/> Reagrds,<br/>MES Team";

                    string subject = "CBM ALERT " + DateTime.Now.ToString();
                    GlobalOperations.sendMail("ipms_automail@mahindra.com", item, subject, bdy);
                }

                TempData["globalData"] = globalData;
                ViewBag.GlobalDataModel = globalData;
            }



            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Number");
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name");
            ViewBag.Machine_Parameter = new SelectList(db.MM_MT_Conditional_Based_Maintenance, "Machine_ID", "Machine_Parameter");
            ViewBag.UOM = new SelectList(db.MM_MT_Conditional_Based_Maintenance, "Machine_ID", "UOM");

            return View();
        }
        #endregion

        #region Fill DropDown on Selecion
        /*
        * Action Name          : GetParameterByMachine
        * Input Parameter      : Datatable and MM_MT_Machines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action used to Get Parameters with respect to machine
        */
        public ActionResult GetParameterByMachine(int machineid)
        {
            var Parameters = db.MM_MT_Conditional_Based_Maintenance.Where(c => c.Machine_ID == machineid).Select(a => new { a.Machine_ID, a.Machine_Parameter });
            return Json(Parameters, JsonRequestBehavior.AllowGet);
        }

        /*
      * Action Name          : GetUOMbyMachineID
      * Input Parameter      : ID(Machine_ID)
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Action used to Get UOM by machine
      */
        public ActionResult GetUOMbyMachineID(int machineid)
        {
            var UOM = db.MM_MT_Conditional_Based_Maintenance.Where(c => c.Machine_ID == machineid).Select(a => new { a.Machine_ID, a.Machine_Parameter, a.UOM });
            return Json(UOM, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult CheckValidData(FormCollection fc)
        {
            if (fc["Is_InterLock"].ToString() == "true")
            {

            }
            else if (fc["Is_InterLock"].ToString() == "false")
            {
                if ((fc["Max_Val"].ToString() != "" || fc["Max_Val"].ToString() != null))
                {
                    ModelState.AddModelError("Max_Val", "Maximum Value is Required");
                }
                else if ((fc["Min_Val"].ToString() != "" || fc["Min_Val"].ToString() != null))
                {
                    ModelState.AddModelError("Min_Val", "Minimum Value is Required");
                }
                else if ((fc["UOM"].ToString() != "" || fc["UOM"].ToString() != null))
                {
                    ModelState.AddModelError("UOM", "Unit is Required");
                }
            }
            return RedirectToAction("Create");
        }
        public class CategoryImage
        {
            public decimal Id { get; set; }
            public byte[] Content { get; set; }
            public string Content_Type { get; set; }
        }
    }
}
