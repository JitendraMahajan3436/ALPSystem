using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class HelpCategoryController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        int plantId = 0, shopId = 0, lineId = 0, stationId = 0, categoryId = 0;
        String categoryName = "";

        RS_Help_Category mmHelpCategoryObj = new RS_Help_Category();

        General generalObj = new General();

        // GET: /HelpCategory/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Help_Category;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "HelpCategory";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Help_Category + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Help_Category + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            var RS_Help_Category = db.RS_Help_Category.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations);
            return View(RS_Help_Category.ToList());
        }

        // GET: /HelpCategory/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Help_Category RS_Help_Category = db.RS_Help_Category.Find(id);
            if (RS_Help_Category == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Help_Category;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "HelpCategory";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Help_Category + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Help_Category + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Help_Category);
        }

        // GET: /HelpCategory/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Shop_ID == shopId), "Station_ID", "Station_Name");
            globalData.pageTitle = ResourceModules.Help_Category;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "HelpCategory";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        // POST: /HelpCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Help_Category_ID,Help_Category_Name,Plant_ID,Shop_ID,Line_ID,Station_ID,Sequence,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Help_Category RS_Help_Category)
        {
            if (ModelState.IsValid)
            {
                categoryName = RS_Help_Category.Help_Category_Name;
                plantId = Convert.ToInt16(RS_Help_Category.Plant_ID);

                if (RS_Help_Category.isHelpCategoryExists(categoryName, plantId))
                {
                    ModelState.AddModelError("Help_Category_Name", ResourceValidation.Exist);
                }
                else
                {
                    RS_Help_Category.Inserted_Date = DateTime.Now;
                    RS_Help_Category.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Help_Category.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Help_Category.Add(RS_Help_Category);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Help_Category ;
                    globalData.messageDetail = ResourceModules.Help_Category + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Category.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Category.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Help_Category.Shop_ID), "Line_ID", "Line_Name", RS_Help_Category.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Help_Category.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == RS_Help_Category.Plant_ID), "Shop_ID", "Shop_Name", RS_Help_Category.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Line_ID == RS_Help_Category.Line_ID), "Station_ID", "Station_Name", RS_Help_Category.Station_ID);

            globalData.pageTitle = ResourceModules.Help_Category ;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "HelpCategory";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Help_Category);
        }

        // GET: /HelpCategory/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Help_Category RS_Help_Category = db.RS_Help_Category.Find(id);
            if (RS_Help_Category == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Category.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Category.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Help_Category.Shop_ID), "Line_ID", "Line_Name", RS_Help_Category.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Help_Category.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == RS_Help_Category.Plant_ID), "Shop_ID", "Shop_Name", RS_Help_Category.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Line_ID == RS_Help_Category.Line_ID), "Station_ID", "Station_Name", RS_Help_Category.Station_ID);

            globalData.pageTitle = ResourceModules.Help_Category ;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "HelpCategory";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Help_Category);
        }

        // POST: /HelpCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Help_Category_ID,Help_Category_Name,Plant_ID,Shop_ID,Line_ID,Station_ID,Sequence,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Help_Category RS_Help_Category)
        {
            if (ModelState.IsValid)
            {
                plantId = Convert.ToInt16(RS_Help_Category.Plant_ID);
                categoryName = RS_Help_Category.Help_Category_Name;
                categoryId = Convert.ToInt16(RS_Help_Category.Help_Category_ID);
                if (RS_Help_Category.isHelpCategoryExists(categoryName, plantId, categoryId))
                {
                    ModelState.AddModelError("Help_Category_Name", ResourceValidation.Exist);
                }
                else
                {
                    mmHelpCategoryObj = db.RS_Help_Category.Find(categoryId);
                    mmHelpCategoryObj.Help_Category_Name = RS_Help_Category.Help_Category_Name;
                    mmHelpCategoryObj.Updated_Date= DateTime.Now;
                    mmHelpCategoryObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmHelpCategoryObj.Updated_Host= ((FDSession)this.Session["FDSession"]).userHost;
                    mmHelpCategoryObj.Is_Edited = true;
                    db.Entry(mmHelpCategoryObj).State = EntityState.Modified;
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Help_Category ;
                    globalData.messageDetail = ResourceModules.Help_Category+" "+ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Category.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Category.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Help_Category.Shop_ID), "Line_ID", "Line_Name", RS_Help_Category.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Help_Category.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == RS_Help_Category.Plant_ID), "Shop_ID", "Shop_Name", RS_Help_Category.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Line_ID == RS_Help_Category.Line_ID), "Station_ID", "Station_Name", RS_Help_Category.Station_ID);


            globalData.pageTitle = ResourceModules.Help_Category ;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "HelpCategory";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Help_Category);
        }

        // GET: /HelpCategory/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Help_Category RS_Help_Category = db.RS_Help_Category.Find(id);
            if (RS_Help_Category == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Help_Category ;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "HelpCategory";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Help_Category + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Help_Category);
        }

        // POST: /HelpCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Help_Category RS_Help_Category = db.RS_Help_Category.Find(id);
            try
            {
                db.RS_Help_Category.Remove(RS_Help_Category);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Help_Category", "Help_Category_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Help_Category ;
                globalData.messageDetail = ResourceModules.Help_Category+" "+ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Help_Category ;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", id);
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

        
        public ActionResult getCategoryListByPlantId(int plantId)
        {
            try
            {
                var mmCategoryObj = from categoryObj in db.RS_Help_Category
                                    where categoryObj.Plant_ID == plantId
                                    select new
                                    {
                                        Id = categoryObj.Help_Category_ID,
                                        Value = categoryObj.Help_Category_Name
                                    };

                return Json(mmCategoryObj, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
