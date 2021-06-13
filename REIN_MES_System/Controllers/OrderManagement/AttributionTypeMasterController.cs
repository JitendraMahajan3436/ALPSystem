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

    public class AttributionTypeMasterController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_Attribution_Parameters mmAttributionParametersObj = new RS_Attribution_Parameters();
        GlobalData globalData = new GlobalData();

        int plantId = 0, shopId = 0;

        General generalObj = new General();

        // GET: AttributionTypeMaster
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceDisplayName.Attribution_Type_List;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "AttributionTypeMasterController";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceDisplayName.Attribution_Type_List + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceDisplayName.Attribution_Type_List + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View(db.RS_AttributionType_Master.ToList());
        }

        // GET: AttributionTypeMaster/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AttributionType_Master mM_Attribution_Type_Parameters = db.RS_AttributionType_Master.Find(id);
            if (mM_Attribution_Type_Parameters == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceDisplayName.Attribution_Type_Details;
            globalData.subTitle = ResourceDisplayName.Attribution_Type_Details;
            globalData.controllerName = "AttributionTypeMasterController";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceDisplayName.Attribution_Type_Details + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceDisplayName.Attribution_Type_Details + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Attribution_Type_Parameters);
        }

        // GET: AttributionTypeMaster/Create
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceDisplayName.Attribution_Type_Master;
            globalData.subTitle = ResourceGlobal.Add;
            globalData.controllerName = "AttributionTypeMasterController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceDisplayName.Attribution_Type_Master;
            globalData.contentFooter = ResourceDisplayName.Attribution_Type_Master;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m => m.Plant_ID == plantId), "Plant_ID", "Plant_Name");

            var list = new SelectList(new[]
        {
                new {id="Text",Name="Text"},
                new {id="Button",Name="Button" },
                new {id="Radio",Name="Radio" },
                new {id="checkbox",Name="checkbox" },
                new {id="DropDownList",Name="DropDownList" },
                new {id="Label",Name="Label" },
            }, "id", "Name", 1);

            ViewBag.ToolBox = list;
            return View();
        }

        // POST: AttributionTypeMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_AttributionType_Master RS_AttributionType_Master)
        {
            if (ModelState.IsValid)
            {
                RS_AttributionType_Master.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                int plant_id = Convert.ToInt32(RS_AttributionType_Master.Plant_ID);
                int ChildPartId = Convert.ToInt32(RS_AttributionType_Master.Sub_Assembly_ID);
                if (RS_AttributionType_Master.isDublicateAttributeName(RS_AttributionType_Master.Attribution_Type, RS_AttributionType_Master.Attribution_ID,plant_id))
                {
                    ModelState.AddModelError("Attribution_Type", ResourceMessages.Attribute_Type_Already_Exist);
                }
                else if(RS_AttributionType_Master.isDuplicateTextPosition(RS_AttributionType_Master.ToolBox_Post,RS_AttributionType_Master.Attribution_ID,plant_id, ChildPartId))
                {
                    ModelState.AddModelError("ToolBox_Post", ResourceValidation.Exist);
                }
                else
                {
                    RS_AttributionType_Master.Inserted_Date = DateTime.Now;
                    RS_AttributionType_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_AttributionType_Master.Updated_Date = DateTime.Now;
                    RS_AttributionType_Master.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                    RS_AttributionType_Master.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

                    db.RS_AttributionType_Master.Add(RS_AttributionType_Master);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Attribution_Parameter;
                    globalData.messageDetail = ResourceModules.Attribution_Parameter + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Create");
                }
            }
            globalData.pageTitle = ResourceDisplayName.Attribution_Type_Master + "" + ResourceMessages.Add_Success;
            globalData.subTitle = ResourceDisplayName.Attribution_Type_Master + "" + ResourceMessages.Add_Success;
            globalData.controllerName = "AttributionParameters";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceDisplayName.Attribution_Type_Master + "" + ResourceMessages.Add_Success;
            globalData.contentFooter = ResourceDisplayName.Attribution_Type_Master + "" + ResourceMessages.Add_Success;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name",RS_AttributionType_Master.Sub_Assembly_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AttributionType_Master.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AttributionType_Master.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AttributionType_Master.Plant_ID);

            var list = new SelectList(new[]
        {
                new {id="Text",Name="Text"},
                new {id="Button",Name="Button" },
                new {id="Radio",Name="Radio" },
                new {id="checkbox",Name="checkbox" },
                new {id="DropDownList",Name="DropDownList" },
                new {id="Label",Name="Label" },
            }, "id", "Name", 1);

            ViewBag.ToolBox = list;

            return View(RS_AttributionType_Master);
        }

        // GET: AttributionTypeMaster/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AttributionType_Master RS_AttributionType_Master = db.RS_AttributionType_Master.Find(id);
            if (RS_AttributionType_Master == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceDisplayName.Attribution_Type_Master;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "AttributionTypeMaster";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceDisplayName.Attribution_Type_Master + "" + ResourceGlobal.Edit;
            globalData.contentFooter = ResourceDisplayName.Attribution_Type_Master + "" + ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name",RS_AttributionType_Master.Sub_Assembly_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_AttributionType_Master.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_AttributionType_Master.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AttributionType_Master.Plant_ID);

            var list = new SelectList(new[]
             {
                new {id="Text",Name="Text"},
                new {id="Button",Name="Button" },
                new {id="Radio",Name="Radio" },
                new {id="checkbox",Name="checkbox" },
                new {id="DropDownList",Name="DropDownList" },
                new {id="Label",Name="Label" },
            }, "id", "Name", RS_AttributionType_Master.ToolBox);

            ViewBag.ToolBox = list;
            //return View("Create");

            return View(RS_AttributionType_Master);
        }

        // POST: AttributionTypeMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_AttributionType_Master RS_AttributionType_Master)
        {
            if (ModelState.IsValid)
            {
                RS_AttributionType_Master.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                int plant_id = Convert.ToInt32(RS_AttributionType_Master.Plant_ID);
                int ChildPartId = Convert.ToInt32(RS_AttributionType_Master.Sub_Assembly_ID);
                if (RS_AttributionType_Master.isDublicateAttributeName(RS_AttributionType_Master.Attribution_Type, RS_AttributionType_Master.Attribution_ID, plant_id))
                {
                    ModelState.AddModelError("Attribution_Type", ResourceValidation.Exist);
                }
                else if(RS_AttributionType_Master.isDuplicateTextPosition(RS_AttributionType_Master.ToolBox_Post,RS_AttributionType_Master.Attribution_ID,plant_id, ChildPartId))
                {
                    ModelState.AddModelError("ToolBox_Post", ResourceValidation.Exist);
                }
                else
                {
                    db.Entry(RS_AttributionType_Master).State = EntityState.Modified;
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Attribution_Parameter;
                    globalData.messageDetail = ResourceModules.Attribution_Parameter + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;
                    ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name",RS_AttributionType_Master.Sub_Assembly_ID);
                    return RedirectToAction("Create");
                }
            }

            globalData.pageTitle = ResourceDisplayName.Attribution_Type_Master + "" + ResourceMessages.Add_Success;
            globalData.subTitle = ResourceDisplayName.Attribution_Type_Master + "" + ResourceMessages.Add_Success;
            globalData.controllerName = "AttributionParameters";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceDisplayName.Attribution_Type_Master + "" + ResourceMessages.Add_Success;
            globalData.contentFooter = ResourceDisplayName.Attribution_Type_Master + "" + ResourceMessages.Add_Success;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name",RS_AttributionType_Master.Sub_Assembly_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AttributionType_Master.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AttributionType_Master.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AttributionType_Master.Plant_ID);

            var list = new SelectList(new[]
          {
                new {id="Text",Name="Text"},
                new {id="Button",Name="Button" },
                new {id="Radio",Name="Radio" },
                new {id="checkbox",Name="checkbox" },
                new {id="DropDownList",Name="DropDownList" },
                new {id="Label",Name="Label" },
            }, "id", "Name", RS_AttributionType_Master.ToolBox);

            ViewBag.ToolBox = list;

            return View(RS_AttributionType_Master);
        }

        // GET: AttributionTypeMaster/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            RS_AttributionType_Master RS_AttributionType_Master = db.RS_AttributionType_Master.Find(id);
            globalData.pageTitle = ResourceDisplayName.Attribution_Type_Master;
            globalData.subTitle = ResourceGlobal.Delete; 
            globalData.controllerName = "AttributionTypeMasterController";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceDisplayName.Attribution_Type_Master + " " + ResourceGlobal.Delete;
            globalData.contentFooter = ResourceDisplayName.Attribution_Type_Master + " " + ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            if (RS_AttributionType_Master == null)
            {
                return HttpNotFound();
            }
            return View(RS_AttributionType_Master);
        }

        // POST: AttributionTypeMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try { 
            RS_AttributionType_Master RS_AttributionType_Master = db.RS_AttributionType_Master.Find(id);
                var plantId = Convert.ToInt32(RS_AttributionType_Master.Plant_ID);
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
                            if (attributionParameter.label.Equals(RS_AttributionType_Master.Attribution_Type, StringComparison.InvariantCultureIgnoreCase))
                            {
                                

                                    generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_AttributionType_Master", "Attribution_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                                    globalData.isAlertMessage = true;
                                    globalData.messageTitle = ResourceModules.AttributionType_Master;
                                    globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                                    TempData["globalData"] = globalData;
                                    return RedirectToAction("Create");
                                
                                //       attributionParameter.Value;
                            }
                        }
                    }
                }
                /////

                db.RS_AttributionType_Master.Remove(RS_AttributionType_Master);
            db.SaveChanges();
            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.Attribution_Parameter;
            globalData.messageDetail = ResourceModules.Attribution_Parameter + " " + ResourceMessages.Delete_Success;
            TempData["globalData"] = globalData;
            }
            catch(DbUpdateException ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Attribution_Type;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;

            }
            return RedirectToAction("Create");
        }


        /*               Action Name               : showAttributeType
         *               Description               : Action used to show the Attribute Type details.
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Plant ID
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */



        public ActionResult LoadPartial()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

            // IEnumerable<RS_AttributionType_Master> AttributeTypeDetail = null;

            var AttributeTypeDetail = (from AttributeTypeItem in db.RS_AttributionType_Master
                                       where AttributeTypeItem.Plant_ID == plantId
                                       select AttributeTypeItem).ToList();


            return PartialView("LoadPartial", AttributeTypeDetail);

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
