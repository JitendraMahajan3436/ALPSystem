using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Text;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /*               Controller Name           : OMConfigurationController
     *               Description               : Controller used to Crete,update,delete and details of all Order configuration. 
     *               Author, Timestamp         : Jitendra Mahajan
     */
    public class OMConfigurationController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        int plantId = 0, shopId = 0;

        General generalObj = new General();

        /* Action Name                 : Index
         *  Description                : Action used to show the list of Order Configuration
         *  Author, Timestamp          : Jitendra Mahajan
         *  Input parameter            : None
         *  Return Type                : ActionResult
         *  Revision                   : 1.0
         */
        // GET: OMConfiguration
        public ActionResult Index()
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = ResourceModules.OM_Configuration;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "OMConfiguration";
                globalData.actionName = ResourceGlobal.Lists;
                globalData.contentTitle = ResourceModules.OM_Configuration;
                globalData.contentFooter = ResourceModules.OM_Configuration;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.GlobalDataModel = globalData;

                var RS_OM_Configuration = db.RS_OM_Configuration.Include(m => m.RS_Partgroup).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Where(p=>p.Plant_ID== plantID);
                
                //old sql Query
                String SQLQuery = "";
                SQLQuery = "select RS_OM_Configuration.OMconfig_ID,RS_OM_Configuration.OMconfig_Desc, RS_Shops.Shop_Name,RS_OM_Configuration.Config_Name Config_Name1 ";
                SQLQuery += "from RS_OM_Configuration,RS_Shops ";
                SQLQuery += "where RS_OM_Configuration.Shop_ID=RS_Shops.Shop_ID and ";
                SQLQuery += "RS_OM_Configuration.Plant_ID=" + plantID;
                SQLQuery += "group by RS_OM_Configuration.OMconfig_ID,RS_OM_Configuration.OMconfig_Desc, RS_Shops.Shop_Name,RS_OM_Configuration.Config_Name";
                //string SQLQuery = "call MangerModule();";
                var objectContext = ((IObjectContextAdapter)db).ObjectContext;
                List<object> listobj = new List<object>();
                List<OM_ConfigurationIndex> data = objectContext.ExecuteStoreQuery<OM_ConfigurationIndex>(SQLQuery).AsQueryable().ToList();
                //return data;
                ViewBag.config_data = data;


                return View(RS_OM_Configuration.ToList());
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.OM_Configuration;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return View();
            }

        }

        // GET: OMConfiguration/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Configuration RS_OM_Configuration = db.RS_OM_Configuration.Find(id);
            if (RS_OM_Configuration == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_Configuration);
        }



        /*               Action Name               : GetShopID
         *               Description               : Action used to return the list of Shop ID for Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Plant_Id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Shop 
        public ActionResult GetShopID(int Plant_Id)
        {
            var Shop_Id = db.RS_Shops
                                       .Where(c => c.Plant_ID == Plant_Id)
                                       .Select(c => new { c.Shop_ID, c.Shop_Name })
                                       .Distinct()
                                       .OrderBy(c => c.Shop_Name);
            return Json(Shop_Id, JsonRequestBehavior.AllowGet);
        }


        /*               Action Name               : GetPlatformID
         *               Description               : Action used to return the list of Platform ID for OMConfiguration
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Plant_Id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Platform 
        public ActionResult GetPlatformID(int Plant_Id)
        {
            var Platform_Id = db.RS_Platform
                                       .Where(c => c.Plant_ID == Plant_Id)
                                       .Select(c => new { c.Platform_Id, c.Platform_Description })
                                       .Distinct()
                                       .OrderBy(c => c.Platform_Description);
            return Json(Platform_Id, JsonRequestBehavior.AllowGet);
        }

        // GET: OMConfiguration/Create
        public ActionResult Create()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.OM_Configuration;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "OMConfiguration";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.SelectedPartgroup_ID = new SelectList(db.RS_Partgroup.Where(p => p.Shop_ID == shopId && p.Plant_ID == plantID), "Partgroup_ID", "Partgrup_Desc");
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup.Where(p => p.Shop_ID == shopId && p.Plant_ID == plantID), "Partgroup_ID", "Partgrup_Desc");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantID), "Shop_ID", "Shop_Name");
            ViewBag.Platform_Id = new SelectList(db.RS_Platform, "Platform_Id", "Platform_Description");
            return View();
        }


        /*               Action Name               : Create
         *               Description               : Action used to add the Order Management Configuartaion for the plant
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : RS_OM_Configuration
         *               Return Type               : ActionResult
         *               Revision                  : 1
         */
        // POST: OMConfiguration/Create
        // To protect from overposting attacks, pl  ease enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_OM_Configuration RS_OM_Configuration)
        {
            // if (ModelState.IsValid)
            //{
            try
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                var config = db.RS_OM_Configuration.Where(om => om.Shop_ID == RS_OM_Configuration.Shop_ID && om.Plant_ID == plantId && om.OMconfig_Desc == RS_OM_Configuration.OMconfig_Desc).Select(om => om).ToList();
                if (config.Count > 0)
                {
                    ModelState.AddModelError("OMconfig_Desc", ResourceValidation.Exist);
                }
                else
                {
                    int plant_Id, Shop_Id;
                    string config_Desc, Platform_Id, Config_Name;

                    plant_Id = Convert.ToInt16(RS_OM_Configuration.Plant_ID);
                    Shop_Id = Convert.ToInt16(RS_OM_Configuration.Shop_ID);
                    config_Desc = RS_OM_Configuration.OMconfig_Desc;
                    Config_Name = RS_OM_Configuration.Config_Name;
                    Platform_Id = "1";

                    String config_Id = RS_OM_Configuration.GetLastOMConfigNumber(plant_Id, Shop_Id);

                    RS_OM_Configuration.Platform_Id = Convert.ToDecimal(Platform_Id);
                    for (int i = 0; i < RS_OM_Configuration.SelectedPartgroup_ID.Count(); i++)
                    {
                        RS_OM_Configuration.OMconfig_ID = config_Id;
                        //Common fileds
                        RS_OM_Configuration.Inserted_Date = DateTime.Now;
                        RS_OM_Configuration.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_OM_Configuration.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_OM_Configuration.Updated_Date = DateTime.Now;



                        decimal partId = Convert.ToDecimal(RS_OM_Configuration.SelectedPartgroup_ID[i]);
                        RS_OM_Configuration.Partgroup_ID = partId;

                        //}
                        //else
                        {
                            db.RS_OM_Configuration.Add(RS_OM_Configuration);
                            db.SaveChanges();
                        }

                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.OM_Configuration;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return Create();
            }

            //}

            globalData.pageTitle = ResourceModules.OM_Configuration;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "OMConfiguration";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            var shopName = db.RS_Shops.Where(shop => shop.Shop_ID == RS_OM_Configuration.Shop_ID).Select(shop => shop.Shop_Name).FirstOrDefault();
            if (shopName.Contains("Tractor"))
            {
                ViewBag.SelectedPartgroup_ID = new MultiSelectList(db.RS_Partgroup.Where(p => p.Plant_ID == plantId && p.Shop_ID == RS_OM_Configuration.Shop_ID && p.Order_Create == true), "Partgroup_ID", "Partgrup_Desc");
            }
            else
            {
                ViewBag.SelectedPartgroup_ID = new MultiSelectList(db.RS_Partgroup.Where(p => p.Plant_ID == plantId && p.Shop_ID == RS_OM_Configuration.Shop_ID && p.Order_Create == false), "Partgroup_ID", "Partgrup_Desc");
            }

            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup.Where(p => p.Plant_ID == plantId), "Partgroup_ID", "Partgrup_Desc", RS_OM_Configuration.Partgroup_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_OM_Configuration.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_OM_Configuration.Shop_ID);
            ViewBag.Platform_Id = new SelectList(db.RS_Platform.Where(p => p.Plant_ID == plantId), "Platform_Id", "Platform_Description", RS_OM_Configuration.Platform_Id);
            return View(RS_OM_Configuration);
        }


        // GET: OMConfiguration/Edit/5
        public ActionResult Edit(string id)
        {
            int plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Configuration[] RS_OM_Configuration = db.RS_OM_Configuration.Where(p => p.OMconfig_ID == id && p.Plant_ID == plant_ID).ToArray();
            shopId = Convert.ToInt16(RS_OM_Configuration[0].Shop_ID);
            if (RS_OM_Configuration == null)
            {
                return HttpNotFound();
            }

            RS_Partgroup[] selectedPartGroup = (from mmPartGroupObj in db.RS_Partgroup
                                                where (from mmOmConfigurationObj in db.RS_OM_Configuration
                                                       where mmOmConfigurationObj.OMconfig_ID == id
                                                       select mmOmConfigurationObj.Partgroup_ID).Contains(mmPartGroupObj.Partgroup_ID)
                                                select mmPartGroupObj).ToArray();

            decimal[] selectedId = new decimal[selectedPartGroup.Count()];

            for (int i = 0; i < selectedPartGroup.Count(); i++)
            {
                selectedId[i] = selectedPartGroup[i].Partgroup_ID;
            }

            globalData.pageTitle = ResourceModules.OM_Configuration;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "OMConfiguration";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = Convert.ToInt16(RS_OM_Configuration[0].Plant_ID);
            shopId = Convert.ToInt16(RS_OM_Configuration[0].Shop_ID);
            // int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.selectedPartGroup = selectedPartGroup;
            ViewBag.SelectedPartgroup_ID = new MultiSelectList(db.RS_Partgroup.Where(p => p.Shop_ID == shopId && p.Plant_ID == plantId), "Partgroup_ID", "Partgrup_Desc", selectedId);
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup.Where(p => p.Shop_ID == shopId && p.Plant_ID == plantId), "Partgroup_ID", "Partgrup_Desc", selectedId);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_OM_Configuration[0].Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_OM_Configuration[0].Shop_ID);
            return View(RS_OM_Configuration[0]);
        }

        // POST: OMConfiguration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Plant_ID,Shop_ID,OMconfig_ID,OMconfig_Desc,Partgroup_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,SelectedPartgroup_ID,Config_Name")] RS_OM_Configuration RS_OM_Configuration)
        {

            if (RS_OM_Configuration.SelectedPartgroup_ID.Count() > 0)
            {


                // process to delete all the cofiguration first
                String omConfigId = RS_OM_Configuration.OMconfig_ID;
                RS_OM_Configuration[] mmOmConfigurationObj = db.RS_OM_Configuration.Where(p => p.OMconfig_ID == omConfigId).ToArray();
                for (int i = 0; i < mmOmConfigurationObj.Count(); i++)
                {
                    db.RS_OM_Configuration.Remove(mmOmConfigurationObj[i]);
                    db.SaveChanges();
                    generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_Configuration", "Row_ID", mmOmConfigurationObj[i].Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                }

                for (int i = 0; i < RS_OM_Configuration.SelectedPartgroup_ID.Count(); i++)
                {
                    RS_OM_Configuration.OMconfig_ID = omConfigId;
                    //Common fileds
                    RS_OM_Configuration.Inserted_Date = DateTime.Now;
                    RS_OM_Configuration.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_OM_Configuration.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_OM_Configuration.Updated_Date = DateTime.Now;

                    decimal partId = Convert.ToDecimal(RS_OM_Configuration.SelectedPartgroup_ID[i]);
                    RS_OM_Configuration.Partgroup_ID = partId;
                    RS_OM_Configuration.Platform_Id = Convert.ToDecimal("1");

                    db.RS_OM_Configuration.Add(RS_OM_Configuration);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            { }

            String id = RS_OM_Configuration.OMconfig_ID;
            RS_Partgroup[] selectedPartGroup = (from mmPartGroupObj in db.RS_Partgroup
                                                where (from mmOmConfigurationObj in db.RS_OM_Configuration
                                                       where mmOmConfigurationObj.OMconfig_ID == id
                                                       select mmOmConfigurationObj.Partgroup_ID).Contains(mmPartGroupObj.Partgroup_ID)
                                                select mmPartGroupObj).ToArray();

            decimal[] selectedId = new decimal[selectedPartGroup.Count()];

            for (int i = 0; i < selectedPartGroup.Count(); i++)
            {
                selectedId[i] = selectedPartGroup[i].Partgroup_ID;
            }

            globalData.pageTitle = ResourceModules.OM_Configuration;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "OMConfiguration";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = Convert.ToInt16(RS_OM_Configuration.Plant_ID);
            shopId = Convert.ToInt16(RS_OM_Configuration.Shop_ID);
            ViewBag.selectedPartGroup = selectedPartGroup;
            ViewBag.SelectedPartgroup_ID = new MultiSelectList(db.RS_Partgroup.Where(p => p.Shop_ID == shopId), "Partgroup_ID", "Partgrup_Desc", selectedId);
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup.Where(p => p.Shop_ID == shopId), "Partgroup_ID", "Partgrup_Desc", selectedId);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_OM_Configuration.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_OM_Configuration.Shop_ID);

            //ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup, "Partgroup_ID", "Partgrup_Desc", RS_OM_Configuration.Partgroup_ID);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_Configuration.Plant_ID);
            //ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_OM_Configuration.Shop_ID);
            return View(RS_OM_Configuration);
        }

        // GET: OMConfiguration/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var Row_ID = db.RS_OM_Configuration.Where(oc => oc.OMconfig_ID == id).Select(c => c.Row_ID).FirstOrDefault();
            RS_OM_Configuration RS_OM_Configuration = db.RS_OM_Configuration.Find(Row_ID);
            if (RS_OM_Configuration == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_Configuration);
        }

        // POST: OMConfiguration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try { 
            var Row_ID = db.RS_OM_Configuration.Where(oc => oc.OMconfig_ID == id).Select(c => c.Row_ID).ToList();
            foreach (var item in Row_ID)
            {
                RS_OM_Configuration RS_OM_Configuration = db.RS_OM_Configuration.Find(item);
                db.RS_OM_Configuration.Remove(RS_OM_Configuration);
                db.SaveChanges();
            }

            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.OM_Configuration;
            globalData.messageDetail = ResourceModules.OM_Configuration + " " + ResourceMessages.Delete_Success;
            globalData.pageTitle = ResourceModules.OM_Configuration;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "OMConfiguration";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            }
            catch (DbUpdateException ex)
            {
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.OM_Configuration;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                globalData.pageTitle = ResourceModules.OM_Configuration;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "OMConfiguration";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.OM_Configuration + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;
            }
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


        public ActionResult GetPartgroupByShopId(int shopId)
        {
            try
            {

                int plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                var Is_Main = db.RS_Shops.Where(shop => shop.Shop_ID == shopId).Select(shop => shop.Is_Main).FirstOrDefault();
                if (Is_Main)
                {
                    var res = from partgroupObj in db.RS_Partgroup
                              where (partgroupObj.Shop_ID == shopId || (partgroupObj.Order_Create == true)) && partgroupObj.Plant_ID == plant_ID
                              select new
                              {
                                  Id = partgroupObj.Partgroup_ID,
                                  Value = partgroupObj.Partgrup_Desc
                              };
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Is_Parallel = db.RS_Shops.Where(shop => shop.Shop_ID == shopId).Select(shop => shop.Is_Parallel_Order).FirstOrDefault();
                    if (Is_Parallel)
                    {
                        var res = from partgroupObj in db.RS_Partgroup
                                   where (partgroupObj.Shop_ID == shopId && (partgroupObj.Order_Create == true)) && partgroupObj.Plant_ID == plant_ID
                                   select new
                                   {
                                       Id = partgroupObj.Partgroup_ID,
                                       Value = partgroupObj.Partgrup_Desc
                                   };
                                   
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var res = from partgroupObj in db.RS_Partgroup
                                   where (partgroupObj.Shop_ID == shopId && (partgroupObj.Order_Create == false)) && partgroupObj.Plant_ID == plant_ID && (from shopObj in db.RS_Shops where shopObj.Is_Main == false select shopObj.Shop_ID).Contains(partgroupObj.Shop_ID)
                                   select new
                                   {
                                       Id = partgroupObj.Partgroup_ID,
                                       Value = partgroupObj.Partgrup_Desc
                                   };
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
