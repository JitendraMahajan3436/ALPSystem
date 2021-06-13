using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mahindra_FD.Models;
using Mahindra_FD.Helper;
using Mahindra_FD.App_LocalResources;
using Mahindra_FD.Controllers.BaseManagement;

namespace Mahindra_FD.Controllers.StyleCodeConfiguration
{
    
    public class StyleCodeCheckPointsController : BaseController
    {
        GlobalData globalData = new GlobalData();


        int plantId = 0;
        private DRONA_NGPEntities db = new DRONA_NGPEntities();

        // GET: StyleCodeCheckPoints
        public ActionResult Index()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckPoint;
            globalData.subTitle = ResourceGlobal.Index;
            globalData.controllerName = "StyleCodeCheckPointsController";
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceGlobal.Lists + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Lists + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(db.MM_StyleCode_CheckPoints.ToList());
        }

        // GET: StyleCodeCheckPoints/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_StyleCode_CheckPoints mM_StyleCode_CheckPoints = db.MM_StyleCode_CheckPoints.Find(id);
            if (mM_StyleCode_CheckPoints == null)
            {
                return HttpNotFound();
            }
            return View(mM_StyleCode_CheckPoints);
        }

        // GET: StyleCodeCheckPoints/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckPoint;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StyleCodeCheckPointsController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "User_ID", "User_Name");
            ViewBag.Line_Type_Id = new SelectList(db.MM_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            return View();
        }

        // POST: StyleCodeCheckPoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_StyleCode_CheckPoints mM_StyleCode_CheckPoints)
        {
            if (ModelState.IsValid)
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int CheckPointID = Convert.ToInt16(mM_StyleCode_CheckPoints.Check_Point_ID);
                bool result = mM_StyleCode_CheckPoints.IsDuplicateStyleCodeCheckPointName(CheckPointID, plantId,mM_StyleCode_CheckPoints.Check_Point_Name);
                if (result)
                {
                    mM_StyleCode_CheckPoints.Inserted_Date = DateTime.Now;
                    mM_StyleCode_CheckPoints.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mM_StyleCode_CheckPoints.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    db.MM_StyleCode_CheckPoints.Add(mM_StyleCode_CheckPoints);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Check_Point_Name", ResourceValidation.Exist);
                }
              
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckPoint;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StyleCodeCheckPointsController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            return View(mM_StyleCode_CheckPoints);
        }

        // GET: StyleCodeCheckPoints/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_StyleCode_CheckPoints mM_StyleCode_CheckPoints = db.MM_StyleCode_CheckPoints.Find(id);
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckPoint;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "StyleCodeCheckPointsController";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            if (mM_StyleCode_CheckPoints == null)
            {
                return HttpNotFound();
            }
            return View(mM_StyleCode_CheckPoints);
        }

        // POST: StyleCodeCheckPoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_StyleCode_CheckPoints mM_StyleCode_CheckPoints)
        {
            if (ModelState.IsValid)
            {
                int CheckPointID = Convert.ToInt16(mM_StyleCode_CheckPoints.Check_Point_ID);
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                bool result = mM_StyleCode_CheckPoints.IsDuplicateStyleCodeCheckPointName(CheckPointID, plantId, mM_StyleCode_CheckPoints.Check_Point_Name);
                if (result)
                {
                    MM_StyleCode_CheckPoints objStyleCodeCheckPoints = new MM_StyleCode_CheckPoints();
                    objStyleCodeCheckPoints = db.MM_StyleCode_CheckPoints.Find(mM_StyleCode_CheckPoints.Check_Point_ID);

                    objStyleCodeCheckPoints.Check_Point_ID = mM_StyleCode_CheckPoints.Check_Point_ID;
                    objStyleCodeCheckPoints.Check_Point_Name = mM_StyleCode_CheckPoints.Check_Point_Name;
                    objStyleCodeCheckPoints.Updated_Date= DateTime.Now;
                    objStyleCodeCheckPoints.Updated_User_ID= ((FDSession)this.Session["FDSession"]).userId;
                    objStyleCodeCheckPoints.Plant_ID = plantId;
                    db.Entry(objStyleCodeCheckPoints).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("Check_Point_Name", ResourceValidation.Exist);
                }
                return RedirectToAction("Index");
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckPoint;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "StyleCodeCheckPointsController";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            return View(mM_StyleCode_CheckPoints);
        }

        // GET: StyleCodeCheckPoints/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_StyleCode_CheckPoints mM_StyleCode_CheckPoints = db.MM_StyleCode_CheckPoints.Find(id);
            if (mM_StyleCode_CheckPoints == null)
            {
                return HttpNotFound();
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckPoint;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "StyleCodeCheckPointsController";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Style_Code_CheckPoint + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_StyleCode_CheckPoints);
        }

        // POST: StyleCodeCheckPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_StyleCode_CheckPoints mM_StyleCode_CheckPoints = db.MM_StyleCode_CheckPoints.Find(id);
            db.MM_StyleCode_CheckPoints.Remove(mM_StyleCode_CheckPoints);
            db.SaveChanges();
            return RedirectToAction("Index");
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
