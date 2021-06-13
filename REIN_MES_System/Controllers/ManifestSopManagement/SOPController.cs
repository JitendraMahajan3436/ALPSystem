using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using System.IO;
using REIN_MES_System.Helper;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing;

namespace REIN_MES_System.Controllers
{
    public class SOPController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        string logFile = string.Empty;
        string logFile2 = string.Empty;

        // GET: SOP
        public ActionResult Index()
        {
            var RS_SOP = db.RS_SOP.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Model_Master).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations);

            if (this.Session["globalData"] != null)
            {
                globalData = (GlobalData)this.Session["globalData"];
            }
            this.Session["globalData"] = null;

            globalData.pageTitle = ResourceSOPManifest.SOPTitle;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "SOP";
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceSOPManifest.SOP_Title_SOP_Lists;
            globalData.contentFooter = ResourceSOPManifest.SOP_Title_SOP_Lists;
            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_SOP.ToList());
        }

        // GET: SOP/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_SOP RS_SOP = db.RS_SOP.Find(id);
            if (RS_SOP == null)
            {
                return HttpNotFound();
            }
            return View(RS_SOP);
        }

        [OutputCache(Duration = 0)]
        public ActionResult getLineID(decimal shopId)
        {
            var lineObj = db.RS_Lines
              .Where(c => c.Shop_ID == shopId)
              .Select(c => new { c.Line_ID, c.Line_Name })
              .OrderBy(c => c.Line_Name);
            return Json(lineObj, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0)]
        public ActionResult getFamilyList(decimal shopID)
        {
            var familyObj = db.RS_Attribution_Parameters
            .Where(c => c.Shop_ID == shopID && c.Attribute_Type == "Family")
            .Select(c => new { c.Attribute_ID, c.Attribute_Desc })
            .OrderBy(c => c.Attribute_Desc);
            return Json(familyObj, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0)]
        public ActionResult getStationID(decimal lineId)
        {
            var stationObj = (from station in db.RS_Stations
                              where (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                              select new
                              {
                                  Station_ID = station.Station_ID,
                                  Station_Name = station.Station_Name
                              }
                             );
            //var stationObj1 = db.RS_Stations
            //  .Where(c => c.Line_ID == lineId)
            //  .Select(c => new { c.Station_ID, c.Station_Name })
            //  .OrderBy(c => c.Station_Name);
            return Json(stationObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getPlatformID(decimal lineId)
        {
            var platformObj = db.RS_OM_Platform
              .Where(c => c.Line_ID == lineId)
              .Select(c => new { c.Platform_ID, c.Platform_Name })
              .OrderBy(c => c.Platform_Name);
            return Json(platformObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getModelAttributeData(decimal ShopId, decimal lineId, decimal PlatformId)
        {
            var attributeObj = db.RS_Model_Attribute_Master
              .Where(c => c.Line_ID == lineId && c.Shop_ID == ShopId && c.Platform_ID == PlatformId)
              .Select(c => new { c.Model_Attribute_ID, c.Attribution })
              .OrderBy(c => c.Attribution);
            return Json(attributeObj, JsonRequestBehavior.AllowGet);
        }

        // GET: SOP/Create
        public ActionResult Create()
        {
            try
            {
                if (this.Session["globalData"] != null)
                {
                    globalData = (GlobalData)this.Session["globalData"];
                }
                this.Session["globalData"] = null;

                globalData.pageTitle = ResourceSOPManifest.SOPTitle;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "SOP";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceSOPManifest.SOP_Title_Add_SOP;
                globalData.contentFooter = "";
                ViewBag.GlobalDataModel = globalData;
                decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var zb_plantObj = db.RS_Plants.Find(plantID);

                //ViewBag.Attribute_ID = new SelectList(db.RS_Attribution_Parameters.Where(a => a.Attribute_Type == "Family").OrderBy(a => a.Attribute_Desc).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc");
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name");
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == 0), "Line_ID", "Line_Name");
                ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(m => m.Line_ID == 0), "Station_ID", "Station_Name");
                ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(m => m.Line_ID == 0), "Platform_ID", "Platform_Name");
                ViewBag.Model_Attribute_ID = new SelectList(db.RS_Model_Attribute_Master.Where(m => m.Platform_ID == 0), "Model_Attribute_ID", "Attribution");
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }

        public ActionResult getModelCheckBoxList(string modelFamilyCode, decimal shopID)
        {
            List<CheckModel> modelList = new List<CheckModel>();
            IEnumerable<RS_Model_Master> modelCodeObj = db.RS_Model_Master.Where(a => /*a.Family == modelFamilyCode &&*/ a.Shop_ID == shopID).OrderBy(a => a.RS_Series.Series_Description);
            foreach (RS_Model_Master obj in modelCodeObj)
            {
                modelList.Add(new CheckModel { Id = obj.Model_Code, Name = /*(obj.RS_Series != null) ? obj.RS_Series.Series_Description :*/ obj.Model_Code, Checked = true });
            }
            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModelCheckBoxList(decimal attributeID, decimal shopID)
        {
            List<CheckModel> modelList = new List<CheckModel>();
            IEnumerable<RS_Model_Master> modelCodeObj = db.RS_Model_Master.Where(a => a.Model_Attribute_ID == attributeID && a.Shop_ID == shopID);
            foreach (RS_Model_Master obj in modelCodeObj)
            {
                modelList.Add(new CheckModel { Id = obj.Model_Code, Name = /*(obj.RS_Series != null) ? obj.RS_Series.Series_Description :*/ obj.Model_Code, Checked = true });
            }
            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        // POST: SOP/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_SOP RS_SOP, HttpPostedFileBase SOPImage)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (SOPImage != null && SOPImage.ContentLength > 0)
                    {
                        using (Image sopImage = Image.FromStream(SOPImage.InputStream, true, true))
                        {
                            if (sopImage.Width < 2500)
                            {
                                return Json(new { success = false, responseText = "Image Cannot be less than 2500 pixels resolution" }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        string fileName = Path.GetFileName(SOPImage.FileName);

                        // width will increase the height proportionally
                        ImageUpload imageUpload = new ImageUpload { Width = 2500 };

                        // rename, resize, and upload
                        //return object that contains {bool Success,string ErrorMessage,string ImageName}

                        ImageResult imageResult = imageUpload.RenameUploadFile(SOPImage, "SOP_IMAGE_TEMP/" + RS_SOP.Station_ID.ToString() + "_" + RS_SOP.Model_Attribute_ID + "_" + RS_SOP.SOP_Name);

                        DateTime today = DateTime.Now;
                        decimal insertedUserID = ((FDSession)this.Session["FDSession"]).userId;
                        string duplicateModels = "";
                        int validCnt = 0;
                        int invalidCount = 0;
                        foreach (string modelCode in RS_SOP.ModelCodes)
                        {
                            if (!String.IsNullOrWhiteSpace(modelCode))
                            {
                                if (!(db.RS_SOP.Any(a => a.SOP_Name == RS_SOP.SOP_Name && a.Station_ID == RS_SOP.Station_ID && a.Model_Code == modelCode)))
                                {
                                    validCnt++;
                                    RS_SOP.Inserted_Date = today;
                                    RS_SOP.Inserted_User_ID = insertedUserID;
                                    RS_SOP.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

                                    RS_SOP.Model_Code = modelCode;
                                    RS_SOP.Image_Name = imageResult.ImageName;

                                    // Added by Ajay on 27042019
                                    //RS_SOP.Attribute_ID = RS_SOP.Attribute_ID;
                                    RS_SOP.Model_Attribute_ID = RS_SOP.Model_Attribute_ID;
                                    RS_SOP.SOP_Identifier = RS_SOP.Station_ID.ToString() + RS_SOP.Model_Attribute_ID.ToString() + RS_SOP.SOP_Name;
                                    //End
                                    db.RS_SOP.Add(RS_SOP);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    invalidCount++;
                                    duplicateModels += modelCode + ",";
                                }
                            }
                        }
                        if (validCnt > 0)
                        {
                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceSOPManifest.SOP_Title_Add_SOP;
                            globalData.messageDetail = ResourceSOPManifest.SOP_Add_Success;
                            this.Session["globalData"] = globalData;
                            if (invalidCount > 0)
                            {
                                return Json(new { success = true, responseText = "SOP Created Successfully! Duplicate SOP found for following Models : " + duplicateModels }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { success = true, responseText = "SOP Created Successfully!" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            globalData.isSuccessMessage = false;
                            globalData.messageTitle = ResourceSOPManifest.SOP_Title_Add_SOP;
                            globalData.messageDetail = ResourceSOPManifest.SOP_Add_Success;
                            this.Session["globalData"] = globalData;
                            if (invalidCount > 0)
                            {
                                return Json(new { success = false, responseText = "Duplicate SOP found for following Models : " + duplicateModels }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { success = false, responseText = "Unable to Save SOP !Please try again ." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                catch (DbUpdateException exp)
                {
                    generalHelper.addControllerException(exp, "SOPController", "Create(Post)", ((FDSession)this.Session["FDSession"]).userId);
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceSOPManifest.SOPTitle;
                    globalData.messageDetail = ResourceSOPManifest.SOP_Create_DbUpdateException;
                    this.Session["globalData"] = globalData;
                }
                catch (Exception exp)
                {
                    generalHelper.addControllerException(exp, "SOPController", "Create(Post)", ((FDSession)this.Session["FDSession"]).userId);
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceSOPManifest.SOPTitle;
                    globalData.messageDetail = ResourceSOPManifest.SOP_Create_Exception;
                    this.Session["globalData"] = globalData;
                }
            }
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var zb_plantObj = db.RS_Plants.Find(plantID);
            ViewBag.Attribute_ID = new SelectList(db.RS_Attribution_Parameters.Where(a => a.Attribute_Type == "Family").OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name");

            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == RS_SOP.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", RS_SOP.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(m => m.Line_ID == RS_SOP.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", RS_SOP.Station_ID);
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(m => m.Line_ID == RS_SOP.Line_ID), "Platform_ID", "Platform_Name", RS_SOP.Platform_ID);
            ViewBag.Model_Attribute_ID = new SelectList(db.RS_Model_Attribute_Master.Where(m => m.Platform_ID == RS_SOP.Platform_ID), "Model_Attribute_ID", "Attribution", RS_SOP.Model_Attribute_ID);
            return Json(new { success = false, responseText = RS_SOP.SOP_ID }, JsonRequestBehavior.AllowGet);
        }

        // GET: SOP/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (this.Session["globalData"] != null)
            {
                globalData = (GlobalData)this.Session["globalData"];
            }
            this.Session["globalData"] = null;

            globalData.pageTitle = ResourceSOPManifest.SOPTitle;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "SOP";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceSOPManifest.SOP_Title_Edit_SOP;
            globalData.contentFooter = "";
            ViewBag.GlobalDataModel = globalData;

            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_SOP RS_SOP = db.RS_SOP.Find(id);
            if (RS_SOP == null)
            {
                return HttpNotFound();
            }
            List<String> selectedModelList = db.RS_SOP.Where(a => a.SOP_Name == RS_SOP.SOP_Name).Select(a => a.Model_Code).Distinct().ToList();
            var zb_plantObj = db.RS_Plants.Find(plantID);
            ViewBag.ModelCodes = new MultiSelectList(db.RS_Model_Master, "Model_Code", "Model_Code", selectedModelList);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name", RS_SOP.Shop_ID);

            ViewBag.Attribute_ID = new SelectList(db.RS_Attribution_Parameters.Where(a => a.Attribute_Type == "Family" && a.Shop_ID == RS_SOP.Shop_ID).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc", RS_SOP.Attribute_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == RS_SOP.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", RS_SOP.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(m => m.Line_ID == RS_SOP.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", RS_SOP.Station_ID);
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(m => m.Line_ID == RS_SOP.Line_ID), "Platform_ID", "Platform_Name", RS_SOP.Platform_ID);
            ViewBag.Model_Attribute_ID = new SelectList(db.RS_Model_Attribute_Master.Where(m => m.Platform_ID == RS_SOP.Platform_ID), "Model_Attribute_ID", "Attribution", RS_SOP.Model_Attribute_ID);
            List<CheckModel> modelList = new List<CheckModel>();
            IEnumerable<RS_Model_Master> modelCodeObj = db.RS_Model_Master.Where(a => a.Model_Attribute_ID == RS_SOP.Model_Attribute_ID && a.Shop_ID == RS_SOP.Shop_ID);
            foreach (RS_Model_Master obj in modelCodeObj)
            {
                modelList.Add(new CheckModel { Id = obj.Model_Code, Name = (obj.RS_Series != null) ? obj.RS_Series.Series_Description : obj.Model_Code, Checked = selectedModelList.Contains(obj.Model_Code) });
            }
            ViewBag.Models = modelList;
            return View(RS_SOP);
        }

        public string getImage(RS_SOP RS_SOP, HttpPostedFileBase SOPImage)
        {
            string fileName = Path.GetFileName(SOPImage.FileName);

            // width will increase the height proportionally
            ImageUpload imageUpload = new ImageUpload { Width = 2500 };

            // rename, resize, and upload
            //return object that contains {bool Success,string ErrorMessage,string ImageName}
            ImageResult imageResult = imageUpload.RenameUploadFile(SOPImage, "SOPImages/" + RS_SOP.Station_ID + "_" + RS_SOP.SOP_Name);
            return imageResult.ImageName;
        }

        // POST: SOP/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SOP_ID,SOP_Name,Image_Name,Plant_ID,Shop_ID,Line_ID,Station_ID,Platform_ID,Model_Attribute_ID,Model_Code,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,ModelCodes,Attribute_ID,SOP_Identifier")] RS_SOP RS_SOP, HttpPostedFileBase SOPImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RS_SOP mmSOP = db.RS_SOP.Find(RS_SOP.SOP_ID);
                    string imageName = mmSOP.Image_Name;
                    DateTime insertedDate = mmSOP.Inserted_Date;
                    string insertedHost = mmSOP.Inserted_Host;
                    decimal insertedUserID = mmSOP.Inserted_User_ID;

                    if (SOPImage != null && SOPImage.ContentLength > 0)
                    {
                        using (Image sopImage = Image.FromStream(SOPImage.InputStream, true, true))
                        {
                            if (sopImage.Width < 2500)
                            {
                                return Json(new { success = false, responseText = "Image Cannot be less than 2500 pixels resolution" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        string fullPath = Request.MapPath("~/Content/images/" + imageName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        imageName = getImage(RS_SOP, SOPImage);
                    }

                    string nameSOP = mmSOP.SOP_Name;
                    string sopIdentifier = mmSOP.SOP_Identifier;
                    db.RS_SOP.RemoveRange(db.RS_SOP.Where(a => a.SOP_Identifier == sopIdentifier));
                    db.SaveChanges();

                    DateTime today = DateTime.Now;
                    decimal updatedUserID = ((FDSession)this.Session["FDSession"]).userId;
                    string duplicateModels = "";
                    int validCnt = 0;
                    int invalidCount = 0;
                    foreach (string modelCode in RS_SOP.ModelCodes)
                    {
                        if (!String.IsNullOrWhiteSpace(modelCode))
                        {
                            if (!(db.RS_SOP.Any(a => a.SOP_Name == RS_SOP.SOP_Name && a.Station_ID == RS_SOP.Station_ID && a.Model_Code == modelCode)))
                            {
                                validCnt++;
                                RS_SOP.Inserted_Date = insertedDate;
                                RS_SOP.Inserted_Host = insertedHost;
                                RS_SOP.Inserted_User_ID = insertedUserID;
                                RS_SOP.Updated_Date = today;
                                RS_SOP.Updated_User_ID = updatedUserID;
                                RS_SOP.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                RS_SOP.Model_Code = modelCode;
                                RS_SOP.Image_Name = imageName;
                                RS_SOP.SOP_Identifier = RS_SOP.Station_ID.ToString() + RS_SOP.Model_Attribute_ID.ToString() + RS_SOP.SOP_Name;
                                db.RS_SOP.Add(RS_SOP);
                                db.SaveChanges();
                            }
                            else
                            {
                                invalidCount++;
                                duplicateModels += modelCode + ",";
                            }
                        }
                    }
                    if (validCnt > 0)
                    {
                        if (SOPImage != null && SOPImage.ContentLength > 0)
                        {
                            if (invalidCount > 0)
                            {
                                return Json(new { success = true, responseText = "SOP Created Successfully! Duplicate SOP found for following Models : " + duplicateModels }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { success = true, responseText = "SOP Created Successfully!" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (invalidCount > 0)
                            {
                                globalData.isSuccessMessage = true;
                                globalData.messageTitle = ResourceSOPManifest.SOP_Title_Edit_SOP;
                                globalData.messageDetail = "SOP Created Successfully! Duplicate SOP found for following Models : " + duplicateModels;
                                this.Session["globalData"] = globalData;
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                globalData.isSuccessMessage = true;
                                globalData.messageTitle = ResourceSOPManifest.SOP_Title_Edit_SOP;
                                globalData.messageDetail = ResourceSOPManifest.SOP_Edit_Success;
                                this.Session["globalData"] = globalData;
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    else
                    {
                        globalData.isSuccessMessage = false;
                        globalData.messageTitle = ResourceSOPManifest.SOP_Title_Add_SOP;
                        globalData.messageDetail = ResourceSOPManifest.SOP_Add_Success;
                        this.Session["globalData"] = globalData;
                        if (invalidCount > 0)
                        {
                            return Json(new { success = false, responseText = "Duplicate SOP found for following Models : " + duplicateModels }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = false, responseText = "Unable to Save SOP !Please try again ." }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }

                generalHelper.addControllerException(dbEx, "SOPController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceSOPManifest.SOPTitle;
                globalData.messageDetail = ResourceSOPManifest.SOP_Edit_DbEntityValidationException;
                this.Session["globalData"] = globalData;

            }
            catch (DbUpdateException exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceSOPManifest.SOPTitle;
                globalData.messageDetail = ResourceSOPManifest.SOP_Edit_DbUpdateException;
                this.Session["globalData"] = globalData;
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceSOPManifest.SOPTitle;
                globalData.messageDetail = ResourceSOPManifest.SOP_Edit_Exception;
                this.Session["globalData"] = globalData;
            }
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            List<String> selectedModelList = db.RS_SOP.Where(a => a.SOP_Name == RS_SOP.SOP_Name).Select(a => a.Model_Code).Distinct().ToList();
            var zb_plantObj = db.RS_Plants.Find(plantID);
            ViewBag.ModelCodes = new MultiSelectList(db.RS_Model_Master, "Model_Code", "Model_Code", selectedModelList);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name", RS_SOP.Shop_ID);

            ViewBag.Attribute_ID = new SelectList(db.RS_Attribution_Parameters.Where(a => a.Attribute_Type == "Family" && a.Shop_ID == RS_SOP.Shop_ID).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc", RS_SOP.Attribute_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", RS_SOP.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", RS_SOP.Station_ID);

            List<CheckModel> modelList = new List<CheckModel>();
            IEnumerable<RS_Model_Master> modelCodeObj = db.RS_Model_Master.Where(a => a.Family == RS_SOP.Attribute_ID && a.Shop_ID == RS_SOP.Shop_ID).OrderBy(a => a.RS_Series.Series_Description);
            foreach (RS_Model_Master obj in modelCodeObj)
            {
                modelList.Add(new CheckModel { Id = obj.Model_Code, Name = (obj.RS_Series != null) ? obj.RS_Series.Series_Description : obj.Model_Code, Checked = selectedModelList.Contains(obj.Model_Code) });
            }
            ViewBag.Models = modelList;
            if (SOPImage != null && SOPImage.ContentLength > 0)
            {
                return Json(new { success = false, responseText = RS_SOP.SOP_ID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View(RS_SOP);
            }
        }

        // GET: SOP/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_SOP RS_SOP = db.RS_SOP.Find(id);
            if (RS_SOP == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceSOPManifest.SOPTitle;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "SOP";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceSOPManifest.SOP_Title_Delete_SOP;
            globalData.contentFooter = ResourceSOPManifest.SOP_Title_Delete_SOP;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;

            return View(RS_SOP);

        }

        // POST: SOP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_SOP RS_SOP = db.RS_SOP.Find(id);
            try
            {

                db.RS_SOP.Remove(RS_SOP);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceSOPManifest.SOPTitle;
                globalData.messageDetail = ResourceSOPManifest.SOP_Delete_Success;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
            }
            catch (DbUpdateException exp)
            {
                globalData.pageTitle = ResourceSOPManifest.SOPTitle;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "SOP";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceSOPManifest.SOP_Title_Delete_SOP;
                globalData.contentFooter = ResourceSOPManifest.SOP_Title_Delete_SOP;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceSOPManifest.SOPTitle;
                globalData.messageDetail = ResourceSOPManifest.SOP_Delete_Dependency_Failure;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return View(RS_SOP);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "DeleteConfirmed(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.pageTitle = ResourceSOPManifest.SOPTitle;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "SOP";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceSOPManifest.SOP_Title_Delete_SOP;
                globalData.contentFooter = ResourceSOPManifest.SOP_Title_Delete_SOP;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceSOPManifest.SOPTitle;
                globalData.messageDetail = ResourceSOPManifest.SOP_Delete_Failure;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return View(RS_SOP);
            }

            return RedirectToAction("Index");
        }


        public ActionResult ShopScreen()
        {
            int userID = ((FDSession)this.Session["FDSession"]).userId;
            //if (userID == 36 || userID == 23100023 || userID == 5)
            //{
            //    ViewBag.userID = userID;
            //    return View("ShopEngineUserSOP");
            //}
            //else if (userID == 33)
            //{
            //    ViewBag.userID = userID;
            //    return View("ShopTransmissionUserSOP");
            //}
            //else if (userID == 52)
            //{
            //    ViewBag.userID = userID;
            //    return View("ShopTractorUserSOP");
            //}
            //else
            //{
            int stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            // int stationID = 96;
            ViewBag.StationID = stationID;

            var stationDataObj = (from a in db.RS_Stations
                                  join b in db.RS_Route_Configurations on a.Station_ID equals b.Station_ID
                                  join c in db.RS_Shops on a.Shop_ID equals c.Shop_ID
                                  join d in db.RS_Lines on a.Line_ID equals d.Line_ID
                                  where a.Station_ID == stationID
                                  select new
                                  {
                                      a.Is_Buffer,
                                      b.Is_Start_Station,
                                      c.Shop_Name,
                                      d.Line_Name,
                                      a.Station_Name
                                  }).Distinct().FirstOrDefault();

            ViewBag.ShopName = stationDataObj.Shop_Name;
            ViewBag.LineName = stationDataObj.Line_Name;
            ViewBag.StationName = stationDataObj.Station_Name;
            ViewBag.ShopScreenTitle = "SOP Screen";

            // var stationDataObj = db.RS_Stations.Find(stationID);
            string serialNo = "";

            if (stationDataObj.Is_Buffer == null || stationDataObj.Is_Buffer == false)
            {

                var geSerialNoObj = db.RS_Station_Tracking
                                      .Where(a => a.Station_ID == stationID)
                                      .Select(a => new { a.SerialNo }).Distinct().FirstOrDefault();
                serialNo = geSerialNoObj.SerialNo;
                ViewBag.SerialNo = serialNo;
                if (!String.IsNullOrWhiteSpace(serialNo))
                {
                    ViewBag.sopModel = getImagesFromSerialNo(serialNo, stationID);
                }
                return View("SOPScreen", stationDataObj);
            }

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
            //}
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult refreshSOP()
        {
            int stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            ViewBag.StationID = stationID;

            var stationDataObj = (from a in db.RS_Stations
                                  join b in db.RS_Route_Configurations on a.Station_ID equals b.Station_ID
                                  join c in db.RS_Shops on a.Shop_ID equals c.Shop_ID
                                  join d in db.RS_Lines on a.Line_ID equals d.Line_ID
                                  where a.Station_ID == stationID
                                  select new
                                  {
                                      a.Is_Buffer,
                                      b.Is_Start_Station,
                                      c.Shop_Name,
                                      d.Line_Name,
                                      a.Station_Name
                                  }).Distinct().FirstOrDefault();

            ViewBag.ShopName = stationDataObj.Shop_Name;
            ViewBag.LineName = stationDataObj.Line_Name;
            ViewBag.StationName = stationDataObj.Station_Name;
            ViewBag.ShopScreenTitle = "SOP Screen";

            // var stationDataObj = db.RS_Stations.Find(stationID);
            string serialNo = "";

            if (stationDataObj.Is_Buffer == null || stationDataObj.Is_Buffer == false)
            {

                var geSerialNoObj = db.RS_Station_Tracking
                                      .Where(a => a.Station_ID == stationID)
                                      .Select(a => new { a.SerialNo }).Distinct().FirstOrDefault();
                serialNo = geSerialNoObj.SerialNo;
                ViewBag.SerialNo = serialNo;
                if (!String.IsNullOrWhiteSpace(serialNo) && serialNo.Trim().ToUpper() != "EMPTYPITCH")
                {
                    ViewBag.sopModel = getImagesFromSerialNo(serialNo, stationID);
                }
                return PartialView("PVrefreshSOP", stationDataObj);
            }
            return null;
        }

        private IEnumerable<RS_SOP> getImagesFromSerialNo(string serialNo, int stationID)
        {
            string modelCode = db.RS_OM_Order_List
                                 .Where(a => a.Serial_No == serialNo)
                                 .Select(a => new { a.Model_Code }).FirstOrDefault().Model_Code;

            IEnumerable<RS_SOP> sopObj = db.RS_SOP.Where(a => a.Station_ID == stationID && a.Model_Code == modelCode)
                                                  .ToList();
            return sopObj;
        }

        class SOPEditImagesFields
        {
            public string Name { get; set; }
            public long Size { get; set; }
        }

        public ActionResult getSOPImages(decimal sopID)
        {
            var SOPObj = db.RS_SOP.Find(sopID);
            long imageSize = 0;
            SOPEditImagesFields imageObj = new SOPEditImagesFields();
            imageObj.Name = SOPObj.Image_Name;
            try
            {
                FileInfo image = new FileInfo(HttpContext.Request.MapPath("~/Content/images/" + SOPObj.Image_Name));
                if (image != null)
                {
                    imageSize = image.Length;
                }
            }
            catch (FileNotFoundException exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "getSOPImages(" + sopID + ")", ((FDSession)this.Session["FDSession"]).userId);
            }
            imageObj.Size = imageSize;
            return Json(imageObj, JsonRequestBehavior.AllowGet);
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
