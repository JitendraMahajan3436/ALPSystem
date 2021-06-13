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
using System.Text.RegularExpressions;

namespace REIN_MES_System.Controllers.PlantConfiguration
{
    /* Controller Name            : ShopController
    *  Description                : This controller is used to define and manage the shop
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class ShopController : BaseController
    {
        //private REIN_SOLUTION_MEntities db_1 = new REIN_SOLUTION_MEntities();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        RS_Shops mmShopObj = new RS_Shops();

        String shopName = "";
        int shopId = 0, plantId = 0;

        General generalObj = new General();

        /* Action Name                : Index
        *  Description                : Action used to show the list shop
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Shop/
        public ActionResult Index()
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceModules.Shop_Config;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "Shop";
                globalData.actionName = ResourceGlobal.Lists;
                globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;

                var RS_Shops = db.RS_Shops.Include(m => m.RS_Plants).Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Where(p => p.Plant_ID == plantID);
                return View(RS_Shops.ToList());
            }
            catch(Exception ex)
            {
                return RedirectToAction("User", "Index");
            }
            }


        /* Action Name                : Details
        *  Description                : Action used to show the detail of shop
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (shop id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Shop/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Shops RS_Shops = db.RS_Shops.Find(id);
            if (RS_Shops == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Shop_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Shop";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Shops);
        }

        /* Action Name                : Create
        *  Description                : Action used to show the create shop form
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Shop/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Shop_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Shop";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            //ViewBag.ShopsCat_ID = new SelectList(db_1.RS_ShopsCategory, "ShopsCat_ID", "ShopsCategory_Name");
            //ViewBag.Business_ID = new SelectList(db_1.MM_Business, "Business_Id", "Business_Name");
            //ViewBag.Spec_Unit_ID = new SelectList(db_1.MM_Specific_Cosume_Unit, "Spec_Unit_ID", "Unit_Name");
            ViewBag.Plant_ID = plantId;
            return View();
        }

        /* Action Name                : Create
        *  Description                : Action used to add the shop detail in database. Validaiton of shop name.
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Shops
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Shop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Shops RS_Shops)
        {
            if (ModelState.IsValid)
            {
                shopName = RS_Shops.Shop_Name;
                plantId = Convert.ToInt16(RS_Shops.Plant_ID);
                if (RS_Shops.isShopExists(shopName, plantId, 0))
                {
                    ModelState.AddModelError("Shop_Name", ResourceValidation.Exist);
                }
                else
                {
                    RS_Shops.Inserted_Date = DateTime.Now;
                    RS_Shops.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Shops.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Shops.Add(RS_Shops);
                    db.SaveChanges();
                  
                        ////Insert Shop records in  MMTUW DB
                        //mtShopsObj.Shop_ID = RS_Shops.Shop_ID;
                        //mtShopsObj.Shop_Name = RS_Shops.Shop_Name;
                        //mtShopsObj.Shop_SAP = RS_Shops.Shop_SAP;
                        //mtShopsObj.Plant_ID = RS_Shops.Plant_ID;
                        //mtShopsObj.Inserted_Date = RS_Shops.Inserted_Date;
                        //mtShopsObj.Inserted_Host = RS_Shops.Inserted_Host;
                        //mtShopsObj.Inserted_User_ID = RS_Shops.Inserted_User_ID;
                        //mtShopsObj.Is_Edited = RS_Shops.Is_Edited;
                        //mtShopsObj.Is_Transferred = RS_Shops.Is_Transferred;
                        //mtShopsObj.Is_Purgeable = RS_Shops.Is_Purgeable;
                        ////mtShopsObj.Business_ID = RS_Shops.Business_ID;
                        ////mtShopsObj.ShopsCat_ID = RS_Shops.ShopsCat_ID;
                        ////mtShopsObj.Spec_Unit_ID = RS_Shops.Spec_Unit_ID;
                        ////mtShopsObj.Energy = RS_Shops.Energy;

                        //db_1.MM_MTTUW_Shops.Add(mtShopsObj);
                        //db_1.SaveChanges();
                        //End 
                   
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Shop;
                    globalData.messageDetail = ResourceModules.Shop + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Shop_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Shop";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name", RS_Shops.Sub_Assembly_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Shops.Plant_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Shops.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Shops.Updated_User_ID);
            //ViewBag.ShopsCat_ID = new SelectList(db_1.RS_ShopsCategory, "ShopsCat_ID", "ShopsCategory_Name",RS_Shops.ShopsCat_ID);
            //ViewBag.Business_ID = new SelectList(db_1.MM_Business, "Business_Id", "Business_Name",RS_Shops.Business_ID);
            //ViewBag.Spec_Unit_ID = new SelectList(db_1.MM_Specific_Cosume_Unit, "Spec_Unit_ID", "Unit_Name",RS_Shops.Spec_Unit_ID);
            return View(RS_Shops);
        }

        /* Action Name                : Edit
        *  Description                : Action used to show edit the shop details
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (shop id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Shop/Edit/5
        public ActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Shops RS_Shops = db.RS_Shops.Find(id);
            if (RS_Shops == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Shop_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Shop";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name", RS_Shops.Sub_Assembly_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Shops.Plant_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Shops.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Shops.Updated_User_ID);
            //ViewBag.ShopsCat_ID = new SelectList(db_1.RS_ShopsCategory, "ShopsCat_ID", "ShopsCategory_Name",RS_Shops.ShopsCat_ID);
            //ViewBag.Business_ID = new SelectList(db_1.MM_Business, "Business_Id", "Business_Name",RS_Shops.Business_ID);
            //ViewBag.Spec_Unit_ID = new SelectList(db_1.MM_Specific_Cosume_Unit, "Spec_Unit_ID", "Unit_Name",RS_Shops.Spec_Unit_ID);
            return View(RS_Shops);
        }

        /* Action Name                : Edit
        *  Description                : Action used to update the shop detail
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Shops
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Shop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Shops RS_Shops)
        {
            if (ModelState.IsValid)
            {
                shopName = RS_Shops.Shop_Name;
                plantId = Convert.ToInt16(RS_Shops.Plant_ID);
                shopId = Convert.ToInt16(RS_Shops.Shop_ID);
                if (RS_Shops.isShopExists(shopName, plantId, shopId))
                {
                    ModelState.AddModelError("Shop_Name", ResourceValidation.Exist);
                }
                else
                {
                    mmShopObj = db.RS_Shops.Find(shopId);
                    mmShopObj.Shop_Name = RS_Shops.Shop_Name;
                    mmShopObj.Shop_SAP = RS_Shops.Shop_SAP;
                    mmShopObj.Is_Main = RS_Shops.Is_Main;
                    mmShopObj.Plant_ID = RS_Shops.Plant_ID;
                    mmShopObj.Is_Parallel_Order = RS_Shops.Is_Parallel_Order;
                    mmShopObj.Sub_Assembly_ID = RS_Shops.Sub_Assembly_ID;
                    mmShopObj.Updated_Date = DateTime.Now;
                    mmShopObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmShopObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mmShopObj.ShopsCat_ID = RS_Shops.ShopsCat_ID;
                    mmShopObj.Spec_Unit_ID = RS_Shops.Spec_Unit_ID;
                    mmShopObj.Business_ID = RS_Shops.Business_ID;
                    mmShopObj.Energy = RS_Shops.Energy;
                    mmShopObj.Is_Edited = true;
                    db.Entry(mmShopObj).State = EntityState.Modified;
                    db.SaveChanges();

                    ////Insert Shop records in  MMTUW DB
                    //mtShopsObj = db_1.MM_MTTUW_Shops.Find(mmShopObj.Shop_ID);
                    //mtShopsObj.Shop_ID = mmShopObj.Shop_ID;
                    //mtShopsObj.Shop_Name = mmShopObj.Shop_Name;
                    //mtShopsObj.Shop_SAP = mmShopObj.Shop_SAP;
                    //mtShopsObj.Plant_ID = mmShopObj.Plant_ID;
                    //mtShopsObj.Updated_Date = mmShopObj.Updated_Date;
                    //mtShopsObj.Updated_Host = mmShopObj.Updated_Host;
                    //mtShopsObj.Updated_User_ID = mmShopObj.Updated_User_ID;
                    ////mtShopsObj.ShopsCat_ID = mmShopObj.ShopsCat_ID;
                    ////mtShopsObj.Spec_Unit_ID = mmShopObj.Spec_Unit_ID;
                    ////mtShopsObj.Business_ID = mmShopObj.Business_ID;
                    ////mtShopsObj.Energy = mmShopObj.Energy;
                    //db_1.Entry(mtShopsObj).State = EntityState.Modified;
                    //db_1.SaveChanges();
                    ////End 

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Shop;
                    globalData.messageDetail = ResourceModules.Shop + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;


                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Shop_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Shop";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name", RS_Shops.Sub_Assembly_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Shops.Plant_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Shops.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Shops.Updated_User_ID);
            //ViewBag.ShopsCat_ID = new SelectList(db_1.RS_ShopsCategory, "ShopsCat_ID", "ShopsCategory_Name",RS_Shops.ShopsCat_ID);
            //ViewBag.Business_ID = new SelectList(db_1.MM_Business, "Business_Id", "Business_Name",RS_Shops.Business_ID);
            //ViewBag.Spec_Unit_ID = new SelectList(db_1.MM_Specific_Cosume_Unit, "Spec_Unit_ID", "Unit_Name",RS_Shops.Spec_Unit_ID);
            return View(RS_Shops);
        }

        /* Action Name                : Delete
        *  Description                : Action used to show delete shop for user confirmation
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (shop id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Shop/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Shops RS_Shops = db.RS_Shops.Find(id);
            if (RS_Shops == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Shop_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Shop";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Shop + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Shops);
        }

        /* Action Name                : DeleteConfirmed
        *  Description                : Action used to delete shop
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (shop id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Shop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
           
            RS_Shops RS_Shops = db.RS_Shops.Find(id);
            try
            {
                db.RS_Shops.Remove(RS_Shops);
                db.SaveChanges();

                ////Delete from MTTUW
                //mtShopsObj = db_1.MM_MTTUW_Shops.Find(id);
                //db_1.MM_MTTUW_Shops.Remove(mtShopsObj);
                //db_1.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Shops", "Shop_ID,Shop_Name", id.ToString() + "," + RS_Shops.Shop_Name, ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Shop;
                globalData.messageDetail = ResourceModules.Shop + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                //var reg = new Regex("\".*?\"");
                //var matches = reg.Matches(msg);
                //Session["isDbUpdateException"] = msg;
                //globalData.dbUpdateExceptionDetail = ex.InnerException.InnerException.Message.ToString();
               

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Shops", "Shop_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                
                globalData.isAlertMessage = true;
                globalData.messageTitle = ResourceModules.Shop;
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
                return View("Delete", RS_Shops);
            }

        }

        /* Action Name                : Dispose
        *  Description                : Action used to dispose the shop controller object
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : disponsing
        *  Return Type                : void
        *  Revision                   : 1.0
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetShiftByShopID(int shopId)
        {
            var st = from shift in db.RS_Shift
                     where shift.Shop_ID == shopId
                     orderby shift.Shift_Name
                     select new
                     {
                         Id = shift.Shift_ID,
                         Value = shift.Shift_Name,
                     };
            return Json(st, JsonRequestBehavior.AllowGet);
        }

        /* Action Name                : GetShopByPlantID
        *  Description                : Action used to return the list of shop added under plant
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : plantId (plant id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
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
    }
}
