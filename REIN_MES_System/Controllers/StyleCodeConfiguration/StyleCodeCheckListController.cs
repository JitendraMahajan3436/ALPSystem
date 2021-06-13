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
    public class StyleCodeCheckListController : BaseController
    {
        GlobalData globalData = new GlobalData();


        int plantId = 0;
        private DRONA_NGPEntities db = new DRONA_NGPEntities();

        // GET: StyleCodeCheckList
        public ActionResult Index()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckSheet;
            globalData.subTitle = ResourceGlobal.Index;
            globalData.controllerName = "StyleCodeCheckListController";
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceGlobal.Index + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Index + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            ViewBag.CheckPoints = new SelectList(db.MM_StyleCode_CheckPoints.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            return View(db.MM_StyleCode_CheckList.ToList());
        }

        // GET: StyleCodeCheckList/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_StyleCode_CheckList mM_StyleCode_CheckList = db.MM_StyleCode_CheckList.Find(id);

            if (mM_StyleCode_CheckList == null)
            {
                return HttpNotFound();
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckSheet;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "StyleCodeCheckListController";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceGlobal.Details + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Details + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            ViewBag.CheckPoints = new SelectList(db.MM_StyleCode_CheckPoints.Where(p => p.Plant_ID == plantId), "Check_Point_ID", "Check_Point_Name");
            return View(mM_StyleCode_CheckList);
        }

        // GET: StyleCodeCheckList/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckSheet;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StyleCodeCheckListController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Create + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Create + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            return View();
        }

        // POST: StyleCodeCheckList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_StyleCode_CheckList mM_StyleCode_CheckList)
        {
            if (ModelState.IsValid)
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                 int CheckListID = Convert.ToInt16(mM_StyleCode_CheckList.CheckList_ID);
                bool result = mM_StyleCode_CheckList.IsDuplicateStyleCodeCheckListName(CheckListID, plantId, mM_StyleCode_CheckList.CheckList_Name);
                if (result)
                {
                    mM_StyleCode_CheckList.Inserted_Date = DateTime.Now;
                    mM_StyleCode_CheckList.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mM_StyleCode_CheckList.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    db.MM_StyleCode_CheckList.Add(mM_StyleCode_CheckList);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("CheckList_Name", ResourceValidation.Exist);
                }

            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckSheet;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StyleCodeCheckListController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Create + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Create + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            return View(mM_StyleCode_CheckList);
        }

        // GET: StyleCodeCheckList/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_StyleCode_CheckList mM_StyleCode_CheckList = db.MM_StyleCode_CheckList.Find(id);
            if (mM_StyleCode_CheckList == null)
            {
                return HttpNotFound();
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckSheet;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "StyleCodeCheckListController";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            return View(mM_StyleCode_CheckList);
        }

        // POST: StyleCodeCheckList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_StyleCode_CheckList mM_StyleCode_CheckList)
        {
            if (ModelState.IsValid)
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int CheckListID = Convert.ToInt16(mM_StyleCode_CheckList.CheckList_ID);
                bool result = mM_StyleCode_CheckList.IsDuplicateStyleCodeCheckListName(CheckListID, plantId, mM_StyleCode_CheckList.CheckList_Name);
                if (result)
                {
                    MM_StyleCode_CheckList objMM_StyleCode_CheckList = new MM_StyleCode_CheckList();
                    objMM_StyleCode_CheckList = db.MM_StyleCode_CheckList.Find(mM_StyleCode_CheckList.CheckList_ID);
                    objMM_StyleCode_CheckList.CheckList_Name = mM_StyleCode_CheckList.CheckList_Name;

                    objMM_StyleCode_CheckList.Updated_Date= DateTime.Now; ;
                    objMM_StyleCode_CheckList.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;


                  
                    db.Entry(objMM_StyleCode_CheckList).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("CheckList_Name", ResourceValidation.Exist);
                }

            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckSheet;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "StyleCodeCheckListController";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            return View(mM_StyleCode_CheckList);
        }

        // GET: StyleCodeCheckList/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_StyleCode_CheckList mM_StyleCode_CheckList = db.MM_StyleCode_CheckList.Find(id);
            if (mM_StyleCode_CheckList == null)
            {
                return HttpNotFound();
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Style_Code_CheckSheet;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "StyleCodeCheckListController";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Style_Code_CheckSheet + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_StyleCode_CheckList);
        }

        // POST: StyleCodeCheckList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_StyleCode_CheckList mM_StyleCode_CheckList = db.MM_StyleCode_CheckList.Find(id);
            db.MM_StyleCode_CheckList.Remove(mM_StyleCode_CheckList);
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
