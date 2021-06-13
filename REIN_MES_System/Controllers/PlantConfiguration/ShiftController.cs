using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using REIN_MES_System.Controllers.BaseManagement;
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.PlantConfiguration
{
    public class ShiftController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        //private REIN_SOLUTION_MEntities db_1 = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();

        int plantId = 0, shopId = 0, shiftId = 0;
        String shiftName = "";
        RS_Shift mmShiftObj = new RS_Shift();

        General generalObj = new General();

        // GET: /Shift/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Shift_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Shift";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shift + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shift + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            var RS_Shift = db.RS_Shift.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Plants).Include(m => m.RS_Shops);
            return View(RS_Shift.ToList());
        }

        // GET: /Shift/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Shift RS_Shift = db.RS_Shift.Find(id);
            if (RS_Shift == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Shift_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Shift";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Shift + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Shift + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Shift);
        }

        // GET: /Shift/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.Shift_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Shift";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m=>m.Plant_ID==plantId), "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m=>m.Plant_ID==plantId), "Shop_ID", "Shop_Name");
            return View();
        }

        // POST: /Shift/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Shift RS_Shift)
        {
            bool isFormValid = true;
            if (ModelState.IsValid)
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                shopId = Convert.ToInt16(RS_Shift.Shop_ID);
                shiftName = RS_Shift.Shift_Name;
                if (RS_Shift.isShiftExists(shiftName, shopId, 0))
                {
                    ModelState.AddModelError("Shift_Name", ResourceValidation.Exist);
                }
                else
                {
                    // check shift start time is correct, other shift is added in that time
                    if (RS_Shift.isShiftStartTimeCorrect(0, shopId, RS_Shift.Shift_Start_Time))
                    {
                        isFormValid = false;
                        ModelState.AddModelError("Shift_Start_Time", ResourceValidation.Shift_Shop_Time_Error);
                    }

                    // check shift end time is correct
                    if (RS_Shift.isShiftEndTimeCorrect(0, shopId, RS_Shift.Shift_End_Time))
                    {
                        isFormValid = false;
                        ModelState.AddModelError("Shift_End_Time", ResourceValidation.Shift_Shop_Time_Error);
                    }
                    //shift time validation
                    if (RS_Shift.Shift_Start_Time != null){ 
                         if (RS_Shift.Shift_End_Time != null && (RS_Shift.Shift_End_Time <= RS_Shift.Shift_Start_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Shift_End_Time", ResourceValidation.Shift_Invalid_Time_Error);
                        }
                    }
                    // check break time 1 validaition
                    if (RS_Shift.Break1_Time != null)
                    {
                        if ((RS_Shift.Shift_Start_Time > RS_Shift.Break1_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Break1_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_Time", ResourceValidation.Shift_Break_Time_Error);
                        }
                        if (RS_Shift.Break1_End_Time != null && (RS_Shift.Break1_End_Time < RS_Shift.Break1_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_End_Time", ResourceValidation.Shift_Invalid_Time_Error);
                        }

                        if (RS_Shift.Break1_End_Time == null)
                        {
                            // if break 1 time is added then, enter break 1 end time
                            isFormValid = false;
                            ModelState.AddModelError("Break1_End_Time", ResourceValidation.Required);
                        }
                        else if (RS_Shift.Break1_End_Time != null && ((RS_Shift.Shift_Start_Time > RS_Shift.Break1_End_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Break1_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_End_Time", ResourceValidation.Shift_Break_Time_Error);
                        }
                        //lunch time overlap validation
                        if (RS_Shift.Break1_End_Time != null && ((RS_Shift.Lunch_Time <= RS_Shift.Break1_Time) && (RS_Shift.Lunch_End_Time >= RS_Shift.Break1_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_Time", ResourceValidation.Shift_Lunch_Time_Overlap_Error);
                        }
                        if (RS_Shift.Break1_End_Time != null && ((RS_Shift.Lunch_Time <= RS_Shift.Break1_End_Time) && (RS_Shift.Lunch_End_Time >= RS_Shift.Break1_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_End_Time", ResourceValidation.Shift_Lunch_Time_Overlap_Error);
                        }
                    }

                    // check break time 2 validaition
                    if (RS_Shift.Break2_Time != null)
                    {
                        if ((RS_Shift.Shift_Start_Time > RS_Shift.Break2_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Break2_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_Time", ResourceValidation.Shift_Break_Time_Error);
                        }

                        if (RS_Shift.Break2_End_Time == null)
                        {
                            // if break 1 time is added then, enter break 1 end time
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Break_Time_Error);
                        }
                        else if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Shift_Start_Time > RS_Shift.Break2_End_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Break2_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Break_Time_Error);
                        }
                        if (RS_Shift.Break2_End_Time != null && (RS_Shift.Break2_End_Time < RS_Shift.Break2_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Invalid_Time_Error);
                        }
                        //else if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Break1_Time > RS_Shift.Break2_End_Time) || (RS_Shift.Break1_End_Time < RS_Shift.Break2_End_Time)))
                        if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Break1_Time <= RS_Shift.Break2_Time) && (RS_Shift.Break1_End_Time >= RS_Shift.Break2_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_Time", ResourceValidation.Shift_Break2_Time_Error);
                        }
                        if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Break1_Time <= RS_Shift.Break2_End_Time) && (RS_Shift.Break1_End_Time >= RS_Shift.Break2_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Break2_Time_Error);
                        }
                        //lunch time overlap validation
                        if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Lunch_Time <= RS_Shift.Break2_Time) && (RS_Shift.Lunch_End_Time >= RS_Shift.Break2_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_Time", ResourceValidation.Shift_Lunch_Time_Overlap_Error);
                        }
                        if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Lunch_Time <= RS_Shift.Break2_End_Time) && (RS_Shift.Lunch_End_Time >= RS_Shift.Break2_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Lunch_Time_Overlap_Error);
                        }
                    }

                    // check lunch time
                    if (RS_Shift.Lunch_Time != null)
                    {
                        if ((RS_Shift.Shift_Start_Time > RS_Shift.Lunch_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Lunch_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Lunch_Time", ResourceValidation.Shift_Lunch_Time_Error);
                        }
                        if (RS_Shift.Lunch_End_Time != null && (RS_Shift.Lunch_End_Time < RS_Shift.Lunch_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Lunch_End_Time", ResourceValidation.Shift_Invalid_Time_Error);
                        }
                        if (RS_Shift.Lunch_End_Time == null)
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Lunch_End_Time", ResourceValidation.Shift_Lunch_Time_Error);
                        }
                        else if (RS_Shift.Lunch_End_Time != null && ((RS_Shift.Shift_Start_Time > RS_Shift.Lunch_End_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Lunch_End_Time) || (RS_Shift.Lunch_End_Time < RS_Shift.Lunch_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Lunch_End_Time", ResourceValidation.Shift_Lunch_Time_Error);
                        }
                    }

                    if (isFormValid)
                    {
                        RS_Shift.Inserted_Date = DateTime.Now;
                        RS_Shift.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_Shift.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.RS_Shift.Add(RS_Shift);
                        db.SaveChanges();

                        ////insert record into Shift table of MTTUW db
                        //mmShift.Break1_End_Time = RS_Shift.Break1_End_Time;
                        //mmShift.Break1_Time = RS_Shift.Break1_Time;
                        //mmShift.Break2_End_Time = RS_Shift.Break2_End_Time;
                        //mmShift.Break2_Time = RS_Shift.Break2_Time;
                        //mmShift.End_Time = RS_Shift.End_Time;
                        //mmShift.Inserted_Date = RS_Shift.Inserted_Date;
                        //mmShift.Inserted_Host = RS_Shift.Inserted_Host;
                        //mmShift.Inserted_User_ID = RS_Shift.Inserted_User_ID;
                        //mmShift.Is_Purgeable = false;
                        //mmShift.Is_Transfered = false;
                        //mmShift.Lunch_End_Time = RS_Shift.Lunch_End_Time;
                        //mmShift.Lunch_Time = RS_Shift.Lunch_Time;
                        //mmShift.Plant_ID = RS_Shift.Plant_ID;
                        //mmShift.Shift_End_Time = RS_Shift.Shift_End_Time;
                        //mmShift.Shift_ID = RS_Shift.Shift_ID;
                        //mmShift.Shift_Name = RS_Shift.Shift_Name;
                        //mmShift.Shift_Start_Time = RS_Shift.Shift_Start_Time;
                        //mmShift.Shop_ID = RS_Shift.Shop_ID;
                        //db_1.MM_MTTUW_Shift.Add(mmShift);
                        //db_1.SaveChanges();
                        ////End

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Shift;
                        globalData.messageDetail = ResourceModules.Shift + " " + ResourceMessages.Add_Success;
                        TempData["globalData"] = globalData;

                        return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Shift.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Shift.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Shift.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Shift.Shop_ID);

            globalData.pageTitle = ResourceModules.Shift_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Shift";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Shift);
        }

        // GET: /Shift/Edit/5
        public ActionResult Edit(decimal id)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Shift RS_Shift = db.RS_Shift.Find(id);
            if (RS_Shift == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Shift_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Shift";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Shift.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Shift.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m => m.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_Shift.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Shift.Shop_ID);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m => m.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            //ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            return View(RS_Shift);
        }

        // POST: /Shift/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Shift_ID,Shift_Name,Plant_ID,Shop_ID,Shift_Start_Time,Shift_End_Time,Break1_Time,Break2_Time,Lunch_Time,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Inserted_Host,Updated_Host,Break2_End_Time,Break1_End_Time,Lunch_End_Time")] RS_Shift RS_Shift)
        {
            bool isFormValid = true;
            if (ModelState.IsValid)
            {
                plantId = Convert.ToInt16(RS_Shift.Plant_ID);
                shopId = Convert.ToInt16(RS_Shift.Shop_ID);

                shiftName = RS_Shift.Shift_Name;
                shiftId = Convert.ToInt16(RS_Shift.Shift_ID);
                if (RS_Shift.isShiftExists(shiftName, shopId, shiftId))
                {
                    ModelState.AddModelError("Shift_Name", ResourceValidation.Exist);
                }
                else
                {

                    // check shift start time is correct, other shift is added in that time
                    if (RS_Shift.isShiftStartTimeCorrect(shiftId, shopId, RS_Shift.Shift_Start_Time))
                    {
                        isFormValid = false;
                        ModelState.AddModelError("Shift_Start_Time", ResourceValidation.Shift_Shop_Time_Error);
                    }

                    // check shift end time is correct
                    if (RS_Shift.isShiftEndTimeCorrect(shiftId, shopId, RS_Shift.Shift_End_Time))
                    {
                        isFormValid = false;
                        ModelState.AddModelError("Shift_End_Time", ResourceValidation.Shift_Shop_Time_Error);
                    }
                    //shift time validation
                    if (RS_Shift.Shift_Start_Time != null)
                    {
                        if (RS_Shift.Shift_End_Time != null && (RS_Shift.Shift_End_Time <= RS_Shift.Shift_Start_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Shift_End_Time", ResourceValidation.Shift_Invalid_Time_Error);
                        }
                    }
                    // check break time 1 validaition
                    if (RS_Shift.Break1_Time != null)
                    {
                        if ((RS_Shift.Shift_Start_Time > RS_Shift.Break1_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Break1_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_Time", ResourceValidation.Shift_Break_Time_Error);
                        }
                        if(RS_Shift.Break1_End_Time != null && (RS_Shift.Break1_End_Time < RS_Shift.Break1_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_End_Time", ResourceValidation.Shift_Invalid_Time_Error);
                        }

                        if (RS_Shift.Break1_End_Time == null)
                        {
                            // if break 1 time is added then, enter break 1 end time
                            isFormValid = false;
                            ModelState.AddModelError("Break1_End_Time", ResourceValidation.Required);
                        }
                        else if (RS_Shift.Break1_End_Time != null && ((RS_Shift.Shift_Start_Time > RS_Shift.Break1_End_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Break1_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_End_Time", ResourceValidation.Shift_Break_Time_Error);
                        }
                        //lunch time overlap validation
                        if (RS_Shift.Break1_End_Time != null && ((RS_Shift.Lunch_Time <= RS_Shift.Break1_Time) && (RS_Shift.Lunch_End_Time >= RS_Shift.Break1_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_Time", ResourceValidation.Shift_Lunch_Time_Overlap_Error);
                        }
                        if (RS_Shift.Break1_End_Time != null && ((RS_Shift.Lunch_Time <= RS_Shift.Break1_End_Time) && (RS_Shift.Lunch_End_Time >= RS_Shift.Break1_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break1_End_Time", ResourceValidation.Shift_Lunch_Time_Overlap_Error);
                        }
                    }

                    // check break time 2 validaition
                    if (RS_Shift.Break2_Time != null)
                    {
                        if ((RS_Shift.Shift_Start_Time > RS_Shift.Break2_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Break2_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_Time", ResourceValidation.Shift_Break_Time_Error);
                        }

                        if (RS_Shift.Break2_End_Time == null)
                        {
                            // if break 1 time is added then, enter break 1 end time
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Break_Time_Error);
                        }
                        else if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Shift_Start_Time > RS_Shift.Break2_End_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Break2_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Break_Time_Error );
                        }
                        if(RS_Shift.Break2_End_Time != null && (RS_Shift.Break2_End_Time < RS_Shift.Break2_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Invalid_Time_Error);
                        }
                        //else if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Break1_Time > RS_Shift.Break2_End_Time) || (RS_Shift.Break1_End_Time < RS_Shift.Break2_End_Time)))
                        if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Break1_Time <= RS_Shift.Break2_Time)&&(RS_Shift.Break1_End_Time>=RS_Shift.Break2_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_Time", ResourceValidation.Shift_Break2_Time_Error);
                        }
                        if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Break1_Time <= RS_Shift.Break2_End_Time) && (RS_Shift.Break1_End_Time >= RS_Shift.Break2_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Break2_Time_Error);
                        }
                        //lunch time overlap validation
                        if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Lunch_Time <= RS_Shift.Break2_Time) && (RS_Shift.Lunch_End_Time >= RS_Shift.Break2_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_Time", ResourceValidation.Shift_Lunch_Time_Overlap_Error);
                        }
                        if (RS_Shift.Break2_End_Time != null && ((RS_Shift.Lunch_Time <= RS_Shift.Break2_End_Time) && (RS_Shift.Lunch_End_Time >= RS_Shift.Break2_End_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Break2_End_Time", ResourceValidation.Shift_Lunch_Time_Overlap_Error);
                        }
                    }

                    // check lunch time
                    if (RS_Shift.Lunch_Time != null)
                    {
                        if ((RS_Shift.Shift_Start_Time > RS_Shift.Lunch_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Lunch_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Lunch_Time", ResourceValidation.Shift_Lunch_Time_Error);
                        }
                        if (RS_Shift.Lunch_End_Time != null && (RS_Shift.Lunch_End_Time < RS_Shift.Lunch_Time))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Lunch_End_Time", ResourceValidation.Shift_Invalid_Time_Error);
                        }
                        if (RS_Shift.Lunch_End_Time == null)
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Lunch_End_Time", ResourceValidation.Shift_Lunch_Time_Error);
                        }
                        else if (RS_Shift.Lunch_End_Time != null && ((RS_Shift.Shift_Start_Time > RS_Shift.Lunch_End_Time) || (RS_Shift.Shift_End_Time < RS_Shift.Lunch_End_Time) || (RS_Shift.Lunch_End_Time < RS_Shift.Lunch_Time)))
                        {
                            isFormValid = false;
                            ModelState.AddModelError("Lunch_End_Time", ResourceValidation.Shift_Lunch_Time_Error);
                        }
                    }

                    if (isFormValid)
                    {
                        mmShiftObj = db.RS_Shift.Find(RS_Shift.Shift_ID);
                        mmShiftObj.Shift_Name = RS_Shift.Shift_Name;
                        mmShiftObj.Plant_ID = RS_Shift.Plant_ID;
                        mmShiftObj.Shop_ID = RS_Shift.Shop_ID;
                        mmShiftObj.Shift_Start_Time = RS_Shift.Shift_Start_Time;
                        mmShiftObj.Shift_End_Time = RS_Shift.Shift_End_Time;
                        mmShiftObj.Break1_Time = RS_Shift.Break1_Time;
                        mmShiftObj.Break1_End_Time = RS_Shift.Break1_End_Time;
                        mmShiftObj.Break2_Time = RS_Shift.Break2_Time;
                        mmShiftObj.Break2_End_Time = RS_Shift.Break2_End_Time;
                        mmShiftObj.Lunch_Time = RS_Shift.Lunch_Time;
                        mmShiftObj.Lunch_End_Time = RS_Shift.Lunch_End_Time;
                        mmShiftObj.Is_Edited = true;



                        mmShiftObj.Updated_Date = DateTime.Now;
                        mmShiftObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmShiftObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                        //RS_Shift.Updated_Date = DateTime.Now;
                        //RS_Shift.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        //RS_Shift.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                        db.Entry(mmShiftObj).State = EntityState.Modified;
                        db.SaveChanges();

                        ////Edit record from Shift table in MTTUW db
                        //mmShift = db_1.MM_MTTUW_Shift.Find(mmShiftObj.Shift_ID);
                        //mmShift.Shift_Name = mmShiftObj.Shift_Name;
                        //mmShift.Shift_Start_Time = mmShiftObj.Shift_Start_Time;
                        //mmShift.Shop_ID = mmShiftObj.Shop_ID;
                        //mmShift.Shift_End_Time = mmShiftObj.Shift_End_Time;
                        //mmShift.Plant_ID = mmShiftObj.Plant_ID;
                        //mmShift.Lunch_Time = mmShiftObj.Lunch_Time;
                        //mmShift.Lunch_End_Time = mmShiftObj.Lunch_End_Time;
                        //mmShift.Is_Edited = true;
                        //mmShift.End_Time = mmShiftObj.End_Time;
                        //mmShift.Break2_Time = mmShiftObj.Break2_Time;
                        //mmShift.Break2_End_Time = mmShiftObj.Break2_End_Time;
                        //mmShift.Break1_Time = mmShiftObj.Break1_Time;
                        //mmShift.Break1_End_Time = mmShiftObj.Break1_End_Time;
                        //mmShift.Updated_Date = mmShiftObj.Updated_Date;
                        //mmShift.Updated_Host = mmShiftObj.Updated_Host;
                        //mmShift.Updated_User_ID = mmShiftObj.Updated_User_ID;
                        //db_1.Entry(mmShift).State = EntityState.Modified;
                        //db_1.SaveChanges();
                        ////End

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Shift;
                        globalData.messageDetail = ResourceModules.Shift + " " + ResourceMessages.Edit_Success;
                        TempData["globalData"] = globalData;

                        return RedirectToAction("Index");
                    }
                }
            }

           globalData.pageTitle = ResourceModules.Shift_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Shift";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Shift.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Shift.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Shift.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Shift.Shop_ID);
            return View(RS_Shift);
        }

        // GET: /Shift/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Shift RS_Shift = db.RS_Shift.Find(id);
            if (RS_Shift == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Shift_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Shift";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Shift + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Shift);
        }

        // POST: /Shift/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Shift RS_Shift = db.RS_Shift.Find(id);
            try
            {
                db.RS_Shift.Remove(RS_Shift);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Shift", "Shift_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Shift;
                globalData.messageDetail = ResourceModules.Shift + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Shift;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete", RS_Shift);
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Shift;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete", RS_Shift);
            }

        }

        public ActionResult GetShopByPlantID(int plantId)
        {
            try
            {
                var st = from shop in db.RS_Shops
                         where shop.Plant_ID == plantId
                         orderby shop.Shop_Name
                         select new
                         {
                             Id = shop.Shop_ID,
                             Value = shop.Shop_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getRunningShift()
        {
            try
            {
                RS_Quality_Captures mmQualityCapturesObj = new RS_Quality_Captures();
                var ShiftDetails = mmQualityCapturesObj.getCurrentRunningShiftByShopID(((FDSession)this.Session["FDSession"]).shopId);
                if (ShiftDetails != null)
                {
                    return Json(ShiftDetails.Shift_Name, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
