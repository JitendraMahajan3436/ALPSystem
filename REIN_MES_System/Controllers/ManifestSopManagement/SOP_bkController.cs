using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using System.IO;
using ZHB_AD.Helper;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Controllers.BaseManagement;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing;

namespace ZHB_AD.Controllers
{
    public class SOPController : BaseController
    {
        ImageServerUpload getimg = new ImageServerUpload();
        ImageUpload imageUpload = new ImageUpload();
        private ZHB_ADEntities db = new ZHB_ADEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();

        // GET: SOP
        public ActionResult Index()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var mM_SOP = db.MM_SOP.Include(m => m.MM_Employee).Include(m => m.MM_Employee1).Include(m => m.MM_Lines).Include(m => m.MM_Model_Master).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Where(p => p.Plant_ID == plantID);

            if (this.Session["globalData"] != null)
            {
                globalData = (GlobalData)this.Session["globalData"];
            }
            this.Session["globalData"] = null;

            globalData.pageTitle = ResourceModules.SOP;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "SOP";
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceModules.SOP + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.SOP + " " + ResourceGlobal.Lists;
            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;

            return View(mM_SOP.ToList());
        }

        // GET: SOP/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_SOP mM_SOP = db.MM_SOP.Find(id);
            if (mM_SOP == null)
            {
                return HttpNotFound();
            }

            //code for get image from server in byte format

            ViewBag.imgurl = getimg.getimageFromService(mM_SOP.Image_Name, "SOP");

            return View(mM_SOP);
        }

        [OutputCache(Duration = 0)]
        public ActionResult getLineID(decimal shopId)
        {
            var lineObj = db.MM_Lines
              .Where(c => c.Shop_ID == shopId)
              .Select(c => new { c.Line_ID, c.Line_Name })
              .OrderBy(c => c.Line_Name);
            return Json(lineObj, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0)]
        public ActionResult getFamilyList(decimal shopID)
        {
            var familyObj = db.MM_Attribution_Parameters
            .Where(c => c.Shop_ID == shopID && c.Attribute_Type == "Family")
            .Select(c => new { c.Attribute_ID, c.Attribute_Desc })
            .OrderBy(c => c.Attribute_Desc);
            return Json(familyObj, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0)]
        public ActionResult getStationID(decimal lineId)
        {
            var stationObj = db.MM_Stations
              .Where(c => c.Line_ID == lineId)
              .Select(c => new { c.Station_ID, c.Station_Name })
              .OrderBy(c => c.Station_Name);
            return Json(stationObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPlatformID(decimal lineId)
        {
            var platformnObj = db.MM_OM_Platform
              .Where(c => c.Line_ID == lineId)
              .Select(c => new { c.Platform_ID, c.Platform_Name })
              .OrderBy(c => c.Platform_Name);
            return Json(platformnObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getModelCode(decimal Platform_ID)
        {
            int plantid = ((FDSession)this.Session["FDSession"]).plantId;
            var Line = db.MM_Model_Master
                                          .Where(c => c.Platform_Id == Platform_ID && c.Plant_ID == plantid)
                                          .Select(c => new { Id = c.Model_Code, Value = c.Model_Code });
            return Json(Line, JsonRequestBehavior.AllowGet);
        }
        // GET: SOP/Create
        public ActionResult Create()
        {
            if (this.Session["globalData"] != null)
            {
                globalData = (GlobalData)this.Session["globalData"];
            }
            this.Session["globalData"] = null;

            globalData.pageTitle = ResourceModules.SOP;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "SOP";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
            globalData.contentFooter = "";
            ViewBag.GlobalDataModel = globalData;
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var zb_plantObj = db.MM_Plants.Find(plantID);

            //ViewBag.Attribute_ID = new SelectList(db.MM_Attribution_Parameters.Where(a => a.Attribute_Type == "Family").OrderBy(a => a.Attribute_Desc).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name");

            return View();
        }

        public ActionResult getModelCheckBoxList(decimal modelFamilyCode, decimal shopID)
        {
            List<CheckModel> modelList = new List<CheckModel>();
            IEnumerable<MM_Model_Master> modelCodeObj = db.MM_Model_Master.Where(a => a.Family == modelFamilyCode && a.Shop_ID == shopID).OrderBy(a => a.MM_Series.Series_Description);
            foreach (MM_Model_Master obj in modelCodeObj)
            {
                modelList.Add(new CheckModel { Id = obj.Model_Code, Name = (obj.MM_Series != null) ? obj.MM_Series.Series_Description : obj.Model_Code, Checked = true });
            }
            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        // POST: SOP/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( MM_SOP mM_SOP, HttpPostedFileBase SOPImage)
        {
            try
            {
                if (ModelState.IsValid)
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
                        // string imageName = mM_SOP.Station_ID.ToString() + "_" + mM_SOP.Attribute_ID + "_" + mM_SOP.SOP_Name + ".Jpg";
                        // width will increase the height proportionally
                        ImageUpload imageUpload = new ImageUpload { Width = 2500 };

                        // rename, resize, and upload
                        //return object that contains {bool Success,string ErrorMessage,string ImageName}
                        //ImageResult imageResult = imageUpload.RenameUploadFile(SOPImage, "SOPImages/" + mM_SOP.Station_ID.ToString() + "_" + mM_SOP.Attribute_ID + "_" + mM_SOP.SOP_Name);
                        ImageResult imageResult = getimg.RenameUploadFile(SOPImage, mM_SOP.Platform_ID.ToString() + "_" + mM_SOP.Model_Code + "_" + mM_SOP.SOP_Name, "SOP");

                        DateTime today = DateTime.Now;
                        decimal insertedUserID = ((FDSession)this.Session["FDSession"]).userId;
                        string duplicateModels = "";
                        int validCnt = 0;
                        int invalidCount = 0;
                        //foreach (string modelCode in mM_SOP.ModelCodes)
                        //{
                        //    if (!String.IsNullOrWhiteSpace(modelCode))
                        //    {
                                if (!(db.MM_SOP.Any(a => a.SOP_Name == mM_SOP.SOP_Name && a.Platform_ID == mM_SOP.Platform_ID && a.Model_Code == mM_SOP.Model_Code)))
                                {
                                    validCnt++;
                                    mM_SOP.Inserted_Date = today;
                                    mM_SOP.Inserted_User_ID = insertedUserID;
                                    mM_SOP.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

                                    mM_SOP.Model_Code = mM_SOP.Model_Code;
                                    mM_SOP.Image_Name = imageResult.ImageName;
                                    mM_SOP.Attribute_ID = mM_SOP.Attribute_ID;
                                    mM_SOP.Platform_ID = mM_SOP.Platform_ID;
                                    mM_SOP.SOP_Identifier = mM_SOP.Model_Code.ToString() + mM_SOP.SOP_Name;
                                    db.MM_SOP.Add(mM_SOP);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    invalidCount++;
                                    duplicateModels += mM_SOP.Model_Code + ",";
                                }
                        //    }
                        //}
                        if (validCnt > 0)
                        {
                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                            globalData.messageDetail = ResourceModules.SOP + " " + ResourceMessages.Add_Success;
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
                            globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                            globalData.messageDetail = ResourceModules.SOP + " " + ResourceMessages.Add_Success;
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
            }
            catch (DbUpdateException exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "Create(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                globalData.messageDetail = ResourceValidation.Manifest_DBUpdate_Exception;
                this.Session["globalData"] = globalData;
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "Create(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.SOP;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.SOP;
                this.Session["globalData"] = globalData;
            }
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var zb_plantObj = db.MM_Plants.Find(plantID);
            ViewBag.Attribute_ID = new SelectList(db.MM_Attribution_Parameters.Where(a => a.Attribute_Type == "Family").OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name");

            ViewBag.Line_ID = new SelectList(db.MM_Lines.OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", mM_SOP.Line_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations.OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", mM_SOP.Station_ID);
            return Json(new { success = false, responseText = mM_SOP.SOP_ID }, JsonRequestBehavior.AllowGet);
        }

        // GET: SOP/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (this.Session["globalData"] != null)
            {
                globalData = (GlobalData)this.Session["globalData"];
            }
            this.Session["globalData"] = null;

            globalData.pageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "SOP";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.SOP;
            globalData.contentFooter = "";
            ViewBag.GlobalDataModel = globalData;

            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_SOP mM_SOP = db.MM_SOP.Find(id);
            if (mM_SOP == null)
            {
                return HttpNotFound();
            }
            List<String> selectedModelList = db.MM_SOP.Where(a => a.SOP_Name == mM_SOP.SOP_Name).Select(a => a.Model_Code).Distinct().ToList();
            var zb_plantObj = db.MM_Plants.Find(plantID);
            ViewBag.Model_Code = new SelectList(db.MM_Model_Master.Where(m=> m.Plant_ID==plantID && m.Platform_Id==mM_SOP.Platform_ID), "Model_Code", "Model_Code", mM_SOP.Model_Code);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name", mM_SOP.Shop_ID);

            //ViewBag.Attribute_ID = new SelectList(db.MM_Attribution_Parameters.Where(a => a.Attribute_Type == "Family" && a.Shop_ID == mM_SOP.Shop_ID).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc", mM_SOP.Attribute_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(m=>m.Shop_ID==mM_SOP.Shop_ID), "Line_ID", "Line_Name", mM_SOP.Line_ID);
            //ViewBag.Station_ID = new SelectList(db.MM_Stations.OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", mM_SOP.Station_ID);

            //List<CheckModel> modelList = new List<CheckModel>();
            //IEnumerable<MM_Model_Master> modelCodeObj = db.MM_Model_Master.Where(a => a.Family == mM_SOP.Attribute_ID && a.Shop_ID == mM_SOP.Shop_ID).OrderBy(a => a.MM_Series.Series_Description);
            //foreach (MM_Model_Master obj in modelCodeObj)
            //{
            //    modelList.Add(new CheckModel { Id = obj.Model_Code, Name = (obj.MM_Series != null) ? obj.MM_Series.Series_Description : obj.Model_Code, Checked = selectedModelList.Contains(obj.Model_Code) });
            //}
            //ViewBag.Models = modelList;
            //commenred due to nashik change
            //ViewBag.imgurl = getimg.getimageFromService(mM_SOP.Image_Name, "SOP");
            ViewBag.Platform_ID = new SelectList(db.MM_OM_Platform.Where(m => m.Line_ID == mM_SOP.Line_ID), "Platform_ID", "Platform_Name", mM_SOP.Platform_ID);
           // ViewBag.
            FileInfo image = new FileInfo(HttpContext.Request.MapPath("~/Content/images/SOP_IMAGE_TEMP/" + mM_SOP.Image_Name));
            if (image!= null)
            {
                ViewBag.imgurl = image;

            }


            return View(mM_SOP);
        }

        public string getImage(MM_SOP mM_SOP, HttpPostedFileBase SOPImage)
        {
            string fileName = Path.GetFileName(SOPImage.FileName);

            // width will increase the height proportionally
            ImageUpload imageUpload = new ImageUpload { Width = 2500 };

            // rename, resize, and upload
            //return object that contains {bool Success,string ErrorMessage,string ImageName}
            //  ImageResult imageResult = imageUpload.RenameUploadFile(SOPImage, "SOPImages/" + mM_SOP.Station_ID + "_" + mM_SOP.SOP_Name);
            ImageResult imageResult = getimg.RenameUploadFile(SOPImage, mM_SOP.Station_ID.ToString() + "_" + mM_SOP.Attribute_ID + "_" + mM_SOP.SOP_Name, "SOP");


            return imageResult.ImageName;
        }

        // POST: SOP/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SOP_ID,SOP_Name,Image_Name,Plant_ID,Shop_ID,Line_ID,Station_ID,Model_Code,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,ModelCodes,Attribute_ID,SOP_Identifier")] MM_SOP mM_SOP, HttpPostedFileBase SOPImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MM_SOP mmSOP = db.MM_SOP.Find(mM_SOP.SOP_ID);
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
                        imageName = getImage(mM_SOP, SOPImage);
                    }

                    string nameSOP = mmSOP.SOP_Name;
                    string sopIdentifier = mmSOP.SOP_Identifier;
                    db.MM_SOP.RemoveRange(db.MM_SOP.Where(a => a.SOP_Identifier == sopIdentifier));
                    db.SaveChanges();

                    DateTime today = DateTime.Now;
                    decimal updatedUserID = ((FDSession)this.Session["FDSession"]).userId;
                    string duplicateModels = "";
                    int validCnt = 0;
                    int invalidCount = 0;
                    foreach (string modelCode in mM_SOP.ModelCodes)
                    {
                        if (!String.IsNullOrWhiteSpace(modelCode))
                        {
                            if (!(db.MM_SOP.Any(a => a.SOP_Name == mM_SOP.SOP_Name && a.Station_ID == mM_SOP.Station_ID && a.Model_Code == modelCode)))
                            {
                                validCnt++;
                                mM_SOP.Inserted_Date = insertedDate;
                                mM_SOP.Inserted_Host = insertedHost;
                                mM_SOP.Inserted_User_ID = insertedUserID;
                                mM_SOP.Updated_Date = today;
                                mM_SOP.Updated_User_ID = updatedUserID;
                                mM_SOP.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                mM_SOP.Model_Code = modelCode;
                                mM_SOP.Image_Name = imageName;
                                mM_SOP.SOP_Identifier = mM_SOP.Station_ID.ToString() + mM_SOP.Attribute_ID.ToString() + mM_SOP.SOP_Name;
                                db.MM_SOP.Add(mM_SOP);
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
                                globalData.messageTitle = ResourceGlobal.Edit + " " + ResourceModules.SOP;
                                globalData.messageDetail = "SOP Created Successfully! Duplicate SOP found for following Models : " + duplicateModels;
                                this.Session["globalData"] = globalData;
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                globalData.isSuccessMessage = true;
                                globalData.messageTitle = ResourceGlobal.Edit + " " + ResourceModules.SOP; ;
                                globalData.messageDetail = ResourceModules.SOP + " " + ResourceMessages.Edit_Success;
                                this.Session["globalData"] = globalData;
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    else
                    {
                        globalData.isSuccessMessage = false;
                        globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                        globalData.messageDetail = ResourceModules.SOP + " " + ResourceMessages.Add_Success;
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
                globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.SOP + ResourceGlobal.Upload;
                this.Session["globalData"] = globalData;

            }
            catch (DbUpdateException exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.SOP;
                this.Session["globalData"] = globalData;
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.SOP + " " + ResourceGlobal.Edit;
                this.Session["globalData"] = globalData;
            }
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            List<String> selectedModelList = db.MM_SOP.Where(a => a.SOP_Name == mM_SOP.SOP_Name).Select(a => a.Model_Code).Distinct().ToList();
            var zb_plantObj = db.MM_Plants.Find(plantID);
            ViewBag.ModelCodes = new MultiSelectList(db.MM_Model_Master, "Model_Code", "Model_Code", selectedModelList);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name", mM_SOP.Shop_ID);

            ViewBag.Attribute_ID = new SelectList(db.MM_Attribution_Parameters.Where(a => a.Attribute_Type == "Family" && a.Shop_ID == mM_SOP.Shop_ID).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc", mM_SOP.Attribute_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", mM_SOP.Line_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations.OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", mM_SOP.Station_ID);

            List<CheckModel> modelList = new List<CheckModel>();
            IEnumerable<MM_Model_Master> modelCodeObj = db.MM_Model_Master.Where(a => a.Family == mM_SOP.Attribute_ID && a.Shop_ID == mM_SOP.Shop_ID).OrderBy(a => a.MM_Series.Series_Description);
            foreach (MM_Model_Master obj in modelCodeObj)
            {
                modelList.Add(new CheckModel { Id = obj.Model_Code, Name = (obj.MM_Series != null) ? obj.MM_Series.Series_Description : obj.Model_Code, Checked = selectedModelList.Contains(obj.Model_Code) });
            }
            ViewBag.Models = modelList;
            if (SOPImage != null && SOPImage.ContentLength > 0)
            {
                return Json(new { success = false, responseText = mM_SOP.SOP_ID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View(mM_SOP);
            }
        }

        // GET: SOP/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_SOP mM_SOP = db.MM_SOP.Find(id);
            if (mM_SOP == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "SOP";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.SOP;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.SOP;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;

            ViewBag.imgurl = getimg.getimageFromService(mM_SOP.Image_Name, "SOP");

            return View(mM_SOP);

        }

        // POST: SOP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_SOP mM_SOP = db.MM_SOP.Find(id);
            try
            {

                db.MM_SOP.Remove(mM_SOP);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                globalData.messageDetail = ResourceModules.SOP + " " + ResourceMessages.Delete_Success;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
            }
            catch (DbUpdateException exp)
            {
                globalData.pageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "SOP";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.SOP;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.SOP;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.SOP + " " + ResourceGlobal.Delete;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return View(mM_SOP);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "DeleteConfirmed(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.pageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "SOP";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.SOP;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.SOP;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.SOP + " " + ResourceGlobal.Upload;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.SOP + " " + ResourceGlobal.Delete;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return View(mM_SOP);
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

            var stationDataObj = (from a in db.MM_Stations
                                  join b in db.MM_Route_Configurations on a.Station_ID equals b.Station_ID
                                  join c in db.MM_Shops on a.Shop_ID equals c.Shop_ID
                                  join d in db.MM_Lines on a.Line_ID equals d.Line_ID
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

            // var stationDataObj = db.MM_Stations.Find(stationID);
            string serialNo = "";

            if (stationDataObj.Is_Buffer == null || stationDataObj.Is_Buffer == false)
            {

                var geSerialNoObj = db.MM_Station_Tracking
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

            var stationDataObj = (from a in db.MM_Stations
                                  join b in db.MM_Route_Configurations on a.Station_ID equals b.Station_ID
                                  join c in db.MM_Shops on a.Shop_ID equals c.Shop_ID
                                  join d in db.MM_Lines on a.Line_ID equals d.Line_ID
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

            // var stationDataObj = db.MM_Stations.Find(stationID);
            string serialNo = "";

            if (stationDataObj.Is_Buffer == null || stationDataObj.Is_Buffer == false)
            {

                var geSerialNoObj = db.MM_Station_Tracking
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

        private IEnumerable<MM_SOP> getImagesFromSerialNo(string serialNo, int stationID)
        {
            string modelCode = db.MM_OM_Order_List
                                 .Where(a => a.Serial_No == serialNo)
                                 .Select(a => new { a.Model_Code }).FirstOrDefault().Model_Code;

            IEnumerable<MM_SOP> sopObj = db.MM_SOP.Where(a => a.Station_ID == stationID && a.Model_Code == modelCode)
                                                  .ToList();
            return sopObj;
        }

        class SOPEditImagesFields
        {
            public string Name { get; set; }
            public long Size { get; set; }
            public string base64Img { get; set; }

        }

        public ActionResult getSOPImages(decimal sopID)
        {
            var SOPObj = db.MM_SOP.Find(sopID);
            long imageSize = 0;
            string base64Img = "";
            SOPEditImagesFields imageObj = new SOPEditImagesFields();
            imageObj.Name = SOPObj.Image_Name;

            //var imgSrc = getimg.getimageFromService(SOPObj.Image_Name, "SOP");


            try
            {
                FileInfo image = new FileInfo(HttpContext.Request.MapPath("~/Content/images/SOP_IMAGE_TEMP/" + SOPObj.Image_Name));
                if (image != null)
                    if (image != null)
                {
                    imageSize = image.Length;
                    base64Img = image.FullName;
                }
            }
            catch (FileNotFoundException exp)
            {
                generalHelper.addControllerException(exp, "SOPController", "getSOPImages(" + sopID + ")", ((FDSession)this.Session["FDSession"]).userId);
            }
            imageObj.Size = imageSize;
            imageObj.base64Img = base64Img;
            return new JsonResult()
            {
                Data = imageObj,
                ContentType = "application/json",
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };


            // return Json(imageObj, JsonRequestBehavior.AllowGet);
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
