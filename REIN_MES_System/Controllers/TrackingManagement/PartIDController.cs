using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Helper;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Models;
using System.Net;
using System.Data.Entity;
namespace REIN_MES_System.Controllers.TrackingManagement
{
    public class PartIDController : Controller
    {
        // GET: PartID

        FDSession fdSession = new FDSession();
        General general = new General();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_PartID mmpartIdObj = new RS_PartID();
        String PartID = "";
        int  plantId = 0;
        General generalObj = new General();
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle =ResourceModules.Part_ID + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "PartID";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;

            ViewBag.GlobalDataModel = globalData;
            var RS_PartID = db.RS_PartID.Where(p => p.PlantID == plantID);
            return View(RS_PartID.ToList());
        }

        public ActionResult Create()
        {
             int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceModules.Part_ID + " " + ResourceGlobal.Config;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "PartID";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;



                ViewBag.PlantID = plantID;

                return View();
          
          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_PartID obj)
        {
            if (ModelState.IsValid)
            {
               
                plantId = Convert.ToInt16(obj.PlantID);
                if (obj.isPartIDExists(obj.PartID, plantId, 0))
                {
                    ModelState.AddModelError("PartID", ResourceValidation.Exist);
                }
                else
                {
                    obj.Inserted_Date = DateTime.Now;
                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //obj. = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_PartID.Add(obj);
                    db.SaveChanges();                            
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Part_ID;
                    globalData.messageDetail = ResourceModules.Part_ID + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Part_ID;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Part_ID";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

           
            return View(obj);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PartID obj = db.RS_PartID.Find(Convert.ToInt32(id));
            if (obj == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Part_ID;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PartID";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

        
          
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_PartID obj)
        {
            if (ModelState.IsValid)
            {
                
                plantId = Convert.ToInt16(obj.PlantID);
               
                if (obj.isPartIDExists(obj.PartID, plantId, Convert.ToInt32(obj.RowID)))
                {
                    ModelState.AddModelError("PartID", ResourceValidation.Exist);
                }
                else
                {
                    mmpartIdObj = db.RS_PartID.Find(obj.RowID);
                    mmpartIdObj.PartID = obj.PartID;
                    mmpartIdObj.PartIDDescription = obj.PartIDDescription;
                    mmpartIdObj.Updated_Date = DateTime.Now;
                    mmpartIdObj.Active = obj.Active;
                    mmpartIdObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //mmShopObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                    mmpartIdObj.Is_Edited = true;
                    db.Entry(mmpartIdObj).State = EntityState.Modified;
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Part_ID;
                    globalData.messageDetail = ResourceModules.Part_ID + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;


                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Shop;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Part_ID";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.PlantID = plantId;      
            return View(obj);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PartID obj = db.RS_PartID.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Part_ID;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "PartID";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Part_ID + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Part_ID + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(obj);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PartID obj = db.RS_PartID.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Part_ID;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Shop";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Part_ID + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {

            RS_PartID obj = db.RS_PartID.Find(id);
            try
            {
                db.RS_PartID.Remove(obj);
                db.SaveChanges();


                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_PartID", "PartID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Part_ID;
                globalData.messageDetail = ResourceModules.Part_ID + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                //var reg = new Regex("\".*?\"");
                //var matches = reg.Matches(msg);
                //Session["isDbUpdateException"] = msg;
                //globalData.dbUpdateExceptionDetail = ex.InnerException.InnerException.Message.ToString();


                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_PartID", "PartID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isAlertMessage = true;
                globalData.messageTitle = ResourceModules.Part_ID;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Shop;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return View("Delete", obj);
            }

        }
    }
}