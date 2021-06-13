using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class DashboardMasterController : Controller
    {
        // GET: DashboardMaster
        GlobalData globalData = new GlobalData();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        General generalObj = new General();
        decimal shopId = 0, lineId = 0, shiftId = 0, dashId = 0,setupId = 0;
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Dashboard_Config;
            globalData.controllerName = "DashboardMaster";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var RS_Dashboard_Master = db.RS_Dashboard_Master.Where(p => p.Plant_ID == plantID);
            return View(RS_Dashboard_Master.ToList());
        }

        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Dashboard_Master RS_Dashboard_Master = db.RS_Dashboard_Master.Find(id);
            if (RS_Dashboard_Master == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Dashboard_Config;
            //globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "DashboardMaster";
            globalData.actionName = ResourceGlobal.Create;
            //globalData.contentTitle = ResourceModules.Employee_Skill_Set;
            //globalData.contentFooter = ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Details; ;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Dashboard_Master);
        }

        // GET: DashboardMaster/Create
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Dashboard_Config;
            //globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "DashboardMaster";
            globalData.actionName = ResourceGlobal.Create;
            //globalData.contentTitle = ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Lists;
            //globalData.contentFooter = ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Setup_ID = new SelectList(db.RS_Setup.Where(m => m.Setup_ID == 0), "Setup_ID", "Setup_Name");
            ViewBag.Shift_ID = new SelectList(db.RS_Shift.Where(m => m.Shift_ID == 0), "Shift_ID", "Shift_Name");
            return View();
        }

        // POST: EmployeeSkillSet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Dashboard_Master RS_Dashboard_Master)
        {
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                shopId = Convert.ToDecimal(RS_Dashboard_Master.Shop_ID);
                lineId = Convert.ToDecimal(RS_Dashboard_Master.Line_ID);
                shiftId = Convert.ToDecimal(RS_Dashboard_Master.Shift_ID);
                setupId = Convert.ToDecimal(RS_Dashboard_Master.Setup_ID);
                var HostName = RS_Dashboard_Master.Host_Name;
                if (!RS_Dashboard_Master.IsDuplicate(lineId, shiftId, HostName))
                {
                    ModelState.AddModelError("Host_Name", ResourceValidation.Exist);
                }

                else
                {
                    RS_Dashboard_Master.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    RS_Dashboard_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Dashboard_Master.Inserted_Date = DateTime.Now;
                    RS_Dashboard_Master.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Dashboard_Master.Add(RS_Dashboard_Master);
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "Record Added Successfully";
                    globalData.messageDetail = ResourceModules.Plant + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
                   
                //}
                
            }

            globalData.pageTitle = ResourceModules.Dashboard_Config;
            //globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Dashboard Master";
            globalData.actionName = ResourceGlobal.Create;
            //globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            //globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Dashboard_Master.Shop_ID), "Line_ID", "Line_Name", RS_Dashboard_Master.Line_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Employee_SkillSet.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", RS_Dashboard_Master.Shop_ID);
            ViewBag.Shift_ID = new SelectList(db.RS_Shift.Where(p => p.Shop_ID == RS_Dashboard_Master.Shop_ID), "Shift_ID", "Shift_Name", RS_Dashboard_Master.Shift_ID);
            ViewBag.Setup_ID = new SelectList(db.RS_Setup.Where(p => p.Line_ID == RS_Dashboard_Master.Line_ID), "Setup_ID", "Setup_Name", RS_Dashboard_Master.Setup_ID);
            return View(RS_Dashboard_Master);
        }

        // GET: EmployeeSkillSet/Edit/5
        public ActionResult Edit(decimal id)
        {
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Dashboard_Master RS_Dashboard_Master = db.RS_Dashboard_Master.Find(id);
            if (RS_Dashboard_Master == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Dashboard_Config;
            //globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "DashboardMaster";
            globalData.actionName = ResourceGlobal.Edit;
            //globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            //globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Dashboard_Master.Shop_ID), "Line_ID", "Line_Name", RS_Dashboard_Master.Line_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", RS_Dashboard_Master.Shop_ID);
            ViewBag.Shift_ID = new SelectList(db.RS_Shift.Where(p => p.Shop_ID == RS_Dashboard_Master.Shop_ID), "Shift_ID", "Shift_Name", RS_Dashboard_Master.Shift_ID);
            ViewBag.Setup_ID = new SelectList(db.RS_Setup.Where(p => p.Line_ID == RS_Dashboard_Master.Line_ID), "Setup_ID", "Setup_Name", RS_Dashboard_Master.Setup_ID);
            return View(RS_Dashboard_Master);
        }

        // POST: EmployeeSkillSet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Dashboard_Master RS_Dashboard_Master)
        {
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            RS_Dashboard_Master mmObj = new RS_Dashboard_Master();
            if (ModelState.IsValid)
            {
                shopId = Convert.ToDecimal(RS_Dashboard_Master.Shop_ID);
                lineId = Convert.ToDecimal(RS_Dashboard_Master.Line_ID);
                shiftId = Convert.ToDecimal(RS_Dashboard_Master.Shift_ID);
                setupId = Convert.ToDecimal(RS_Dashboard_Master.Setup_ID);
                dashId = RS_Dashboard_Master.Dash_ID;
                var HostName = RS_Dashboard_Master.Host_Name;
                if (RS_Dashboard_Master.IsEditDuplicate(dashId, lineId, shiftId, HostName))
                {
                    ModelState.AddModelError("Host_Name", ResourceValidation.Exist);
                }
                else
                {
                    mmObj = db.RS_Dashboard_Master.Find(RS_Dashboard_Master.Dash_ID);
                    mmObj.Shop_ID = RS_Dashboard_Master.Shop_ID;
                    mmObj.Line_ID = RS_Dashboard_Master.Line_ID;
                    mmObj.Setup_ID = RS_Dashboard_Master.Setup_ID;
                    mmObj.Shift_ID = RS_Dashboard_Master.Shift_ID;
                    mmObj.Plant_ID = plant_ID;// RS_AM_Employee_SkillSet.Plant_ID;
                    mmObj.Dashboard_Name = RS_Dashboard_Master.Dashboard_Name;
                    mmObj.Host_Name = RS_Dashboard_Master.Host_Name;
                    mmObj.Is_Edited = true;
                    mmObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmObj.Updated_Date = DateTime.Now;
                    mmObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceDashboardMaster.Dashboard_Master;
                    globalData.messageDetail = ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;


                    db.Entry(mmObj).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }

                //}
            }
            globalData.pageTitle = ResourceModules.Dashboard_Config;
            //globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "DashboardMaster";
            globalData.actionName = ResourceGlobal.Edit;
            //globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            //globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Dashboard_Master.Shop_ID), "Line_ID", "Line_Name", RS_Dashboard_Master.Line_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", RS_Dashboard_Master.Shop_ID);
            ViewBag.Shift_ID = new SelectList(db.RS_Shift.Where(p => p.Shop_ID == RS_Dashboard_Master.Shop_ID), "Shift_ID", "Shift_Name", RS_Dashboard_Master.Shift_ID);
            ViewBag.Setup_ID = new SelectList(db.RS_Setup.Where(p => p.Line_ID == RS_Dashboard_Master.Line_ID), "Setup_ID", "Setup_Name", RS_Dashboard_Master.Setup_ID);
            return View(RS_Dashboard_Master);
        }

        // GET: EmployeeSkillSet/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Dashboard_Master RS_Dashboard_Master = db.RS_Dashboard_Master.Find(id);
            if (RS_Dashboard_Master == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Dashboard_Config;
            //globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "DashboardMaster";
            globalData.actionName = ResourceGlobal.Delete;
            //globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            //globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Dashboard_Master);
        }

        // POST: EmployeeSkillSet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_Dashboard_Master RS_Dashboard_Master = db.RS_Dashboard_Master.Find(id);
                db.RS_Dashboard_Master.Remove(RS_Dashboard_Master);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Dashboard_Master", "Row_Id", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceDashboardMaster.Dashboard_Master;
                globalData.messageDetail = ResourceDashboardMaster.Dashboard_Master + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceDashboardMaster.Dashboard_Master;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete");
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

        public ActionResult Getsetupbylineid(int LineId, int ShopID)
        {
            try
            {
                decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from setup in db.RS_Setup
                         where setup.Shop_ID == ShopID && setup.Line_ID == LineId
                         && setup.Plant_ID == plant_ID
                         select new
                         {
                             Id = setup.Setup_ID,
                             Value = setup.Setup_Name,
                         };
                return Json(st);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}