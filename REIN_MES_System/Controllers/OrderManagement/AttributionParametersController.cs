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
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class AttributionParametersController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_Attribution_Parameters mmAttributionParametersObj = new RS_Attribution_Parameters();
        GlobalData globalData = new GlobalData();

        int plantId = 0, shopId = 0;

        General generalObj = new General();

        // GET: AttributionParameters
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceAttributionParameters.AttributionParameters;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "AttributionParameters";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceAttributionParameters.AttributionParameters_Title_Lists;
            globalData.contentFooter = ResourceAttributionParameters.AttributionParameters_Title_Lists;
            ViewBag.GlobalDataModel = globalData;
            var RS_DispatchDetails = db.RS_DispatchDetails.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Plants).Include(m => m.RS_Shops);
            return View(db.RS_Attribution_Parameters.ToList());
        }

        // GET: AttributionParameters/Details/5
        public ActionResult Details(string Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int id = Convert.ToInt32(Id);
            RS_Attribution_Parameters RS_Attribution_Parameters = db.RS_Attribution_Parameters.Find(id);
            if (RS_Attribution_Parameters == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceAttributionParameters.AttributionParameters;
            globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_AttributionParameters_Detail;
            globalData.controllerName = "AttributionParameters";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceAttributionParameters.AttributionParameters_Title_AttributionParameters_Detail;
            globalData.contentFooter = ResourceAttributionParameters.AttributionParameters_Title_AttributionParameters_Detail;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Attribution_Parameters);
        }

        // GET: AttributionParameters/Create
        public ActionResult Create()
        {
            try
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                globalData.pageTitle = ResourceAttributionParameters.AttributionParameters;
                globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
                globalData.controllerName = "DispatchDetails";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
                globalData.contentFooter = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m => m.Plant_ID == plantId), "Plant_ID", "Plant_Name");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                ViewBag.Attribute_Type = new SelectList(db.RS_AttributionType_Master.Where(a => a.Plant_ID == plantId).Select(m => m.Attribution_Type));
                ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");

                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }

        // POST: AttributionParameters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Plant_ID,Shop_ID,Sub_Assembly_ID,Attribute_ID,Attribute_Type,Attribute_Desc,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Attribution_Parameters RS_Attribution_Parameters)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                string type = RS_Attribution_Parameters.Attribute_Type;
                string desc = RS_Attribution_Parameters.Attribute_Desc;
                shopId = Convert.ToInt32(RS_Attribution_Parameters.Shop_ID);
                if (RS_Attribution_Parameters.IsAttributionDescExists(type, desc, shopId))
                {
                    ModelState.AddModelError("Attribute_Desc", ResourceValidation.Exist);
                }
                else
                {
                    RS_Attribution_Parameters.Inserted_Date = DateTime.Now;
                    RS_Attribution_Parameters.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Attribution_Parameters.Updated_Date = DateTime.Now;
                    RS_Attribution_Parameters.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    // RS_Attribution_Parameters.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;sss
                    RS_Attribution_Parameters.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    // RS_Attribution_Parameters.Shop_ID = ((FDSession)this.Session["FDSession"]).shopId;

                    //MM_Quality_Checklist obj = new MM_Quality_Checklist();
                    //decimal Attribute_ID = obj.getAttributeId(shopId);
                    //RS_Attribution_Parameters.Attribute_ID = Attribute_ID;
                    db.RS_Attribution_Parameters.Add(RS_Attribution_Parameters);
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceAttributionParameters.AttributionParameters;
                    globalData.messageDetail = ResourceAttributionParameters.AttributionParameters_Success_Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Create");
                }
            }
            globalData.pageTitle = ResourceAttributionParameters.AttributionParameters;
            globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "AttributionParameters";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.contentFooter = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            ViewBag.GlobalDataModel = globalData;


            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Attribution_Parameters.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Attribution_Parameters.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Attribution_Parameters.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Attribution_Parameters.Shop_ID);
            ViewBag.Attribute_Type = new SelectList(db.RS_AttributionType_Master.Where(a => a.Plant_ID == plantId).Select(m => m.Attribution_Type));
            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name",RS_Attribution_Parameters.Sub_Assembly_ID);
            return View(RS_Attribution_Parameters);
        }

        // GET: AttributionParameters/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Attribution_Parameters RS_Attribution_Parameters = db.RS_Attribution_Parameters.Find(id);
            if (RS_Attribution_Parameters == null)
            {
                return HttpNotFound();
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceAttributionParameters.AttributionParameters;
            // globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "AttributionParameters";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceAttributionParameters.AttributionParameters_Title_Edit_AttributionParameters;
            globalData.contentFooter = ResourceAttributionParameters.AttributionParameters_Title_Edit_AttributionParameters;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Attribution_Parameters.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Attribution_Parameters.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m => m.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_Attribution_Parameters.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Attribution_Parameters.Shop_ID);
            ViewBag.Attribute_Type = new SelectList(db.RS_AttributionType_Master.Where(a => a.Plant_ID == plantId).Select(m => m.Attribution_Type));
            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name",RS_Attribution_Parameters.Sub_Assembly_ID);
            return View(RS_Attribution_Parameters);
        }

        // POST: AttributionParameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Plant_ID,Shop_ID,Attribute_ID,Attribute_Type,Attribute_Desc,Sub_Assembly_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Attribution_Parameters RS_Attribution_Parameters)
        {
            try
            {
                mmAttributionParametersObj = new RS_Attribution_Parameters();
                if (ModelState.IsValid)
                {
                    string type = RS_Attribution_Parameters.Attribute_Type;
                    string desc = RS_Attribution_Parameters.Attribute_Desc;
                    shopId = Convert.ToInt32(RS_Attribution_Parameters.Shop_ID);
                    decimal attributeId = RS_Attribution_Parameters.Attribute_ID;
                    if (RS_Attribution_Parameters.IsDescriptionExistEdit(type, desc, shopId, attributeId))
                    {
                        ModelState.AddModelError("Attribute_Desc", ResourceValidation.Exist);
                    }
                    else
                    {
                        mmAttributionParametersObj = db.RS_Attribution_Parameters.Find(RS_Attribution_Parameters.Attribute_ID);
                        mmAttributionParametersObj.Shop_ID = RS_Attribution_Parameters.Shop_ID;
                        mmAttributionParametersObj.Sub_Assembly_ID = RS_Attribution_Parameters.Sub_Assembly_ID;
                        mmAttributionParametersObj.Attribute_Type = RS_Attribution_Parameters.Attribute_Type;
                        mmAttributionParametersObj.Attribute_Desc = RS_Attribution_Parameters.Attribute_Desc;

                        plantId = ((FDSession)this.Session["FDSession"]).plantId;

                        mmAttributionParametersObj.Inserted_Date = db.RS_Attribution_Parameters.Find(RS_Attribution_Parameters.Attribute_ID).Inserted_Date;
                        mmAttributionParametersObj.Inserted_User_ID = db.RS_Attribution_Parameters.Find(RS_Attribution_Parameters.Attribute_ID).Inserted_User_ID;
                        //mmAttributionParametersObj.Inserted_Host = db.RS_DispatchDetails.Find(RS_DispatchDetails.Plan_ID).Inserted_Host;


                        mmAttributionParametersObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmAttributionParametersObj.Updated_Date = DateTime.Now;
                        // mmAttributionParametersObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.Entry(mmAttributionParametersObj).State = EntityState.Modified;
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceAttributionParameters.AttributionParameters;
                        globalData.messageDetail = ResourceAttributionParameters.AttributionParameters_Success_Edit_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Create");
                    }
                }
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceAttributionParameters.AttributionParameters;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");

            }
            globalData.pageTitle = ResourceAttributionParameters.AttributionParameters;
            globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Edit_AttributionParameters;
            globalData.controllerName = "AttributeParameters";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceAttributionParameters.AttributionParameters_Title_Edit_AttributionParameters;
            globalData.contentFooter = ResourceAttributionParameters.AttributionParameters_Title_Edit_AttributionParameters;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Attribution_Parameters.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Attribution_Parameters.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Attribution_Parameters.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Attribution_Parameters.Shop_ID);
            ViewBag.Attribute_Type = new SelectList(db.RS_AttributionType_Master.Where(a => a.Plant_ID == plantId).Select(m => m.Attribution_Type));
            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name", RS_Attribution_Parameters.Sub_Assembly_ID);
            return View(RS_Attribution_Parameters);
        }

        // GET: AttributionParameters/Delete/5
        public ActionResult Delete(string id)
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int ID = Convert.ToInt32(id);
            RS_Attribution_Parameters RS_Attribution_Parameters = db.RS_Attribution_Parameters.Find(ID);
            if (RS_Attribution_Parameters == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
           
            globalData.pageTitle = ResourceAttributionParameters.AttributionParameters;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "AttributionParameters";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceAttributionParameters.AttributionParameters_Title_Delete_AttributionParameters;
            globalData.contentFooter = ResourceAttributionParameters.AttributionParameters_Title_Delete_AttributionParameters;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Attribution_Parameters);
        }

        // POST: AttributionParameters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            int ID = Convert.ToInt32(id);
            RS_Attribution_Parameters RS_Attribution_Parameters = db.RS_Attribution_Parameters.Find(ID);
            var plantId = Convert.ToInt32(RS_Attribution_Parameters.Plant_ID);
            try
            {

                //check already configured in model master before deletion
                var modelObj = (from model in db.RS_Model_Master
                                where model.Plant_ID == plantId
                                select model).ToList();
                if (modelObj != null)
                {
                    foreach (var model in modelObj)
                    {
                        List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
                        for (int i = 0; i < attributionParameters.Count; i++)
                        {
                            AttributionParameters attributionParameter = attributionParameters[i];
                            try
                            {
                                Convert.ToInt32(attributionParameter.Value);
                            }
                            catch (Exception)
                            {

                                continue;
                            }
                            if (attributionParameter.label.Equals(RS_Attribution_Parameters.Attribute_Type, StringComparison.InvariantCultureIgnoreCase))
                            {
                                int attrId = Convert.ToInt32(attributionParameter.Value);
                                if (attrId == RS_Attribution_Parameters.Attribute_ID)
                                {

                                    generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Attribution_Parameters", "Attribute_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                                    globalData.isAlertMessage = true;
                                    globalData.messageTitle = ResourceModules.Attribution_Parameter;
                                    globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                                    TempData["globalData"] = globalData;
                                    return RedirectToAction("Create");
                                }
                                //       attributionParameter.Value;
                            }
                        }
                    }
                }
                /////


                db.RS_Attribution_Parameters.Remove(RS_Attribution_Parameters);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Attribution_Parameters", "Attribute_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceAttributionParameters.AttributionParameters;
                globalData.messageDetail = ResourceAttributionParameters.AttributionParameters_Success_Delete_Success;

                TempData["globalData"] = globalData;
                return RedirectToAction("Create");
            }
            catch (DbUpdateException ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Attribution_Parameter;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceAttributionParameters.AttributionParameters;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", RS_Attribution_Parameters);

            }

            return RedirectToAction("Index");
        }

        public ActionResult GetAttributeType(int Sub_Assembly_ID)
        {
            var attributeType = (from type in db.RS_AttributionType_Master
                             where type.Sub_Assembly_ID == Sub_Assembly_ID 
                             select new
                             {
                                 Id = type.Attribution_ID,
                                 Value = type.Attribution_Type
                             }
                         ).ToList();
            return Json(attributeType, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadAttributeData()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            // int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            //int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            //int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            // IEnumerable<RS_AttributionType_Master> AttributeTypeDetail = null;

            var AttributeDetail = (from AttributeItem in db.RS_Attribution_Parameters
                                   where AttributeItem.Plant_ID == plantId
                                   select AttributeItem).ToList();


            return PartialView("LoadAttributeData", AttributeDetail);

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
