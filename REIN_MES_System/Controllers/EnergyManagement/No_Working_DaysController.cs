using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace ZHB_AD.Controllers.ZHB_AD
{
    public class MM_EM_NPDController : Controller
    {

        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();     
        FDSession adSession = new FDSession();
        General generalObj = new General();
        MM_No_Working_Day mM_No_Working = new MM_No_Working_Day();


        DateTime day;
        int dayId = 0, plantId = 0;

       
        // GET: No_Working_Days
        public ActionResult Index()
        {
            try {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                globalData.pageTitle = ResourceModules.No_Working_Day_Config;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "No_Working_Days";
                globalData.actionName = ResourceGlobal.Lists;
                globalData.contentTitle = ResourceModules.No_Working_Day + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceModules.No_Working_Day + " " + ResourceGlobal.Lists;
               
                ViewBag.GlobalDataModel = globalData;






                // ViewBag.Model_ID = new SelectList(db.MM_Model.Where(m => m.Model_ID == modelId), "Model_ID", "Model_Name", mM_Plan.Model_ID);

                return View(db.MM_No_Working_Day.ToList());
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.No_Working_Day;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index", "User");
            }
        }

   

        public ActionResult Details(decimal? id)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MM_No_Working_Day mM_No_Working = db.MM_No_Working_Day.Find(id);
                if (mM_No_Working == null)
                {
                    return HttpNotFound();
                }


                globalData.pageTitle = ResourceModules.No_Working_Day_Config;
                globalData.subTitle = ResourceGlobal.Details;
                globalData.controllerName = "No_Working_Days";
                globalData.actionName = ResourceGlobal.Details;
                globalData.contentTitle = ResourceModules.No_Working_Day + " " + ResourceGlobal.Details;
                globalData.contentFooter = ResourceModules.No_Working_Day + " " + ResourceGlobal.Details;
             
                ViewBag.GlobalDataModel = globalData;

                return View(mM_No_Working);
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
      
        public ActionResult Create()
        {
            try {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceModules.No_Working_Day_Config;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "No_Working_Days";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
               
                ViewBag.GlobalDataModel = globalData;
                MM_Holiday_Reason mM_Holiday_Reason = new MM_Holiday_Reason();
                //ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Reason_ID = new SelectList(db.MM_Holiday_Reason, "Reason_ID", "Reason_Name");
                ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Plant_ID = plantId;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_No_Working_Day _No_Working_Days)
        {
            if (ModelState.IsValid)
            {
                var remark = _No_Working_Days.Remark.Trim();

                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                day = (_No_Working_Days.Day_Date);
               
                if (_No_Working_Days.isDayexists(day, plantId, 0))
                {
                    ModelState.AddModelError("Day_Date", ResourceValidation.Exist);
                }
                else
                {
                    day = (_No_Working_Days.Day_Date);
                    _No_Working_Days.Plant_ID = plantId;
                    _No_Working_Days.Inserted_Date = DateTime.Now;
                    _No_Working_Days.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    _No_Working_Days.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    _No_Working_Days.Remark = remark;
                    db.MM_No_Working_Day.Add(_No_Working_Days);
                    db.SaveChanges();


                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.No_Working_Day;
                    globalData.messageDetail = ResourceModules.No_Working_Day + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.No_Working_Day_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "No_Working_Days";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", _No_Working_Days.Plant_ID);
            ViewBag.Reason_ID = new SelectList(db.MM_Holiday_Reason, "Reason_ID", "Reason_Name");

            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", _No_Working_Days.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", _No_Working_Days.Updated_User_ID);
            return View(_No_Working_Days);
        }



    
        public ActionResult Edit(decimal? id)
        {
            try {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MM_No_Working_Day mM_No_Working = db.MM_No_Working_Day.Find(id);
                if (mM_No_Working == null)
                {
                    return HttpNotFound();
                }

                globalData.pageTitle = ResourceModules.No_Working_Day_Config;
                globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "No_Working_Days";
                globalData.actionName = ResourceGlobal.Edit;
                globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
           
                ViewBag.GlobalDataModel = globalData;
                //DateTime holDate = mM_No_Working.Day_Date.Date;
                ViewBag.Reason_ID = new SelectList(db.MM_Holiday_Reason, "Reason_ID", "Reason_Name", mM_No_Working.Reason_ID);
                ViewBag.Day_Date = mM_No_Working.Day_Date.ToShortDateString();
                //ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
                ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_No_Working.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_No_Working.Updated_User_ID);
                return View(mM_No_Working);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_No_Working_Day mM_No_Working_)
        {
            if (ModelState.IsValid)
            {

                var remark = mM_No_Working_.Remark.Trim();
                day = (mM_No_Working_.Day_Date);
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                dayId = Convert.ToInt16(mM_No_Working_.Day_ID);
                if (mM_No_Working_.isDayexists(day, plantId, dayId))
                {
                    ModelState.AddModelError("Day_Date", ResourceValidation.Exist);
                }
                else
                {
                    mM_No_Working = db.MM_No_Working_Day.Find(dayId);
                    mM_No_Working.Day_Date = mM_No_Working_.Day_Date;
                    mM_No_Working.Reason_ID = mM_No_Working_.Reason_ID;
                    mM_No_Working.Remark = remark;
                    mM_No_Working.Plant_ID = plantId;
                    mM_No_Working.Updated_Date = DateTime.Now;
                    mM_No_Working.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mM_No_Working.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                    mM_No_Working. Is_Edited= true;
                    db.Entry(mM_No_Working).State = EntityState.Modified;
                    db.SaveChanges();


                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.No_Working_Day;
                    globalData.messageDetail = ResourceModules.No_Working_Day + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;


                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.No_Working_Day_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "No_Working_Days";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mM_No_Working_.Plant_ID);
            ViewBag.Reason_ID = new SelectList(db.MM_Holiday_Reason, "Reason_ID", "Reason_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_No_Working.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_No_Working.Updated_User_ID);
            return View(mM_No_Working_);
        }





  
        public ActionResult Delete(decimal? id)
        {
            try {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MM_No_Working_Day mM_No_Working = db.MM_No_Working_Day.Find(id);
                if (mM_No_Working == null)
                {
                    return HttpNotFound();
                }

                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = ResourceModules.No_Working_Day_Config;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "No_Working_Days";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.No_Working_Day + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;
                return View(mM_No_Working);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
       
     
    
        public ActionResult DeleteConfirmed(decimal id)
        {

            MM_No_Working_Day mM_No_Working = db.MM_No_Working_Day.Find(id);
            try
            {
                db.MM_No_Working_Day.Remove(mM_No_Working);
                db.SaveChanges();


                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_No_Working_Day", "Day_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.No_Working_Day;
                globalData.messageDetail = ResourceModules.No_Working_Day + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {


                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_No_Working_Day", "Day_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isAlertMessage = true;
                globalData.messageTitle = ResourceModules.No_Working_Day;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.No_Working_Day;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return View("Delete", mM_No_Working);
            }

        }


    }
}