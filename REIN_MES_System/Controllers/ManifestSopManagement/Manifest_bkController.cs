using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Controllers.BaseManagement;
using System.IO;
using ZHB_AD.Helper;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Collections;
using ZHB_AD.Controllers.TrackingManagement;
using System.Data.Entity.Infrastructure;
using ZHB_AD.Helper.IoT;

namespace ZHB_AD.Controllers.ManifestSopManagement
{
    public class ManifestController : BaseController
    {
        ImageServerUpload setimg = new ImageServerUpload();
        ImageUpload imageUpload = new ImageUpload();
        private ZHB_ADEntities db = new ZHB_ADEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        // GET: Manifest
        public ActionResult Index()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var mM_Manifest = db.MM_Manifest.Include(m => m.MM_Employee).Include(m => m.MM_Employee1).Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations).Where(p => p.Plant_ID == plantID);

            if (this.Session["globalData"] != null)
            {
                globalData = (GlobalData)this.Session["globalData"];
            }
            this.Session["globalData"] = null;
            globalData.pageTitle = ResourceModules.Manifest;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Manifest";
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceModules.Manifest + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Manifest + " " + ResourceGlobal.Lists;
            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;

            return View(mM_Manifest.ToList().OrderByDescending(a => a.Inserted_Date));
        }

        // GET: Manifest/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Manifest mM_Manifest = db.MM_Manifest.Find(id);
            if (mM_Manifest == null)
            {
                return HttpNotFound();
            }

            ViewBag.imgurl = setimg.getimageFromService(mM_Manifest.Image_Name, "MANIFEST");

            return View(mM_Manifest);
        }

        // GET: Manifest/Create
        public ActionResult Create()
        {
            if (this.Session["globalData"] != null)
            {
                globalData = (GlobalData)this.Session["globalData"];
            }
            this.Session["globalData"] = null;
            globalData.pageTitle = ResourceModules.Manifest;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Manifest";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Manifest + " " + ResourceGlobal.Upload;
            globalData.contentFooter = "";
            ViewBag.GlobalDataModel = globalData;

            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var zb_plantObj = db.MM_Plants.Find(plantID);

            ViewBag.Part_No = new SelectList(db.MM_PartList_View.OrderBy(a => a.Part_No), "Part_No", "Part_No");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID).OrderBy(a => a.Plant_Name), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name");

            return View();
        }

        // POST: Manifest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Manifest_ID,Part_No,Identification,Image_Name,Plant_ID,Shop_ID,Line_ID,Station_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,ModelCodes,Attribute_ID,Is_ParentModel_Manifest")] MM_Manifest mM_Manifest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int imageCntr = 0;
                    Random rnd = new Random();
                    string randomIdentifier = rnd.Next(0, 500).ToString();
                    List<string> inValidModels = new List<String>();
                    int Cntr = 0;
                    foreach (string imageName in Request.Files)
                    {
                        imageCntr++;
                        Cntr++;
                        HttpPostedFileBase PartImage = Request.Files[imageName];
                        if (PartImage != null && PartImage.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(PartImage.FileName);

                            if (mM_Manifest.Is_ParentModel_Manifest == true)
                            {
                                // width will increase the height proportionally
                                ImageUpload imageUpload = new ImageUpload { Width = 2000 };

                                // rename, resize, and upload
                                //return object that contains {bool Success,string ErrorMessage,string ImageName}
                                // ImageResult imageResult = imageUpload.RenameUploadFile(PartImage, "ManifestImages/" + mM_Manifest.Station_ID + "_" + mM_Manifest.Attribute_ID + "_" + randomIdentifier + "_" + imageCntr.ToString());
                                //ImageResult imageResult = imageUpload.RenameUploadFile(PartImage, "ManifestImages/" + mM_Manifest.Station_ID + "_" + mM_Manifest.Attribute_ID + "_" + randomIdentifier + "_" + imageCntr.ToString());

                                ImageResult imageResult = setimg.RenameUploadFile(PartImage, mM_Manifest.Station_ID + "_" + mM_Manifest.Attribute_ID + "_" + randomIdentifier + "_" + imageCntr.ToString(), "MANIFEST");

                                foreach (string modelCode in mM_Manifest.ModelCodes)
                                {
                                    if (!String.IsNullOrWhiteSpace(modelCode))
                                    {
                                        if (Cntr == 1 && db.MM_Manifest.Any(a => a.Part_No == modelCode && a.Station_ID == mM_Manifest.Station_ID) == true)
                                        {
                                            string Series = db.MM_Model_Master.Find(modelCode).MM_Series.Series_Description;
                                            inValidModels.Add(modelCode + " (" + Series + ")");
                                        }
                                        else
                                        {
                                            mM_Manifest.Inserted_Date = DateTime.Now;
                                            mM_Manifest.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                                            string IP = Request.UserHostName;
                                            string compName = ((FDSession)this.Session["FDSession"]).userHost;
                                            mM_Manifest.Inserted_Host = compName;
                                            mM_Manifest.Part_No = modelCode;
                                            mM_Manifest.Identification = mM_Manifest.Station_ID + "_" + mM_Manifest.Attribute_ID + "_" + randomIdentifier;
                                            MM_PartList_View partlist = db.MM_PartList_View.Where(a => a.Part_No == modelCode).FirstOrDefault();
                                            if (partlist != null)
                                            {
                                                mM_Manifest.Part_Description = partlist.Part_Description;
                                            }
                                            else
                                            {
                                                mM_Manifest.Part_Description = "";
                                            }

                                            mM_Manifest.Image_Name = imageResult.ImageName;

                                            db.MM_Manifest.Add(mM_Manifest);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (db.MM_Manifest.Any(a => a.Part_No == mM_Manifest.Part_No && a.Station_ID == mM_Manifest.Station_ID))
                                {
                                    return Json(new { success = false, responseText = "Cannot add duplicate Manifest! This Part No. " + mM_Manifest.Part_No + " is already configured for the station" }, JsonRequestBehavior.AllowGet);
                                }
                                // width will increase the height proportionally
                                ImageUpload imageUpload = new ImageUpload { Width = 700 };

                                // rename, resize, and upload
                                //return object that contains {bool Success,string ErrorMessage,string ImageName}
                                //ImageResult imageResult = imageUpload.RenameUploadFile(PartImage, "ManifestImages/" + mM_Manifest.Station_ID + "_" + mM_Manifest.Part_No + "_" + imageCntr.ToString());
                                ImageResult imageResult = setimg.RenameUploadFile(PartImage, mM_Manifest.Station_ID + "_" + mM_Manifest.Part_No + "_" + imageCntr.ToString(), "MANIFEST");
                                mM_Manifest.Inserted_Date = DateTime.Now;
                                mM_Manifest.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                                string compName = ((FDSession)this.Session["FDSession"]).userHost;
                                mM_Manifest.Inserted_Host = compName;

                                MM_PartList_View partlist = db.MM_PartList_View.Where(a => a.Part_No == mM_Manifest.Part_No).FirstOrDefault();
                                if (partlist != null)
                                {
                                    mM_Manifest.Part_Description = partlist.Part_Description;
                                }
                                else
                                {
                                    mM_Manifest.Part_Description = "";
                                }

                                mM_Manifest.Image_Name = imageResult.ImageName;
                                mM_Manifest.Identification = mM_Manifest.Station_ID + "_" + mM_Manifest.Attribute_ID + "_" + rnd.Next(0, 500).ToString();
                                db.MM_Manifest.Add(mM_Manifest);
                                db.SaveChanges();
                            }
                        }
                    }
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Manifest + " " + ResourceGlobal.Upload;

                    if (inValidModels.Count() > 0)
                    {
                        globalData.messageDetail = "Manifest Creation Successfull!<br/>Manifest for Following Models [" + String.Join(" , ", inValidModels.Distinct().ToArray()) + "] are already present for this Station.";
                        this.Session["globalData"] = globalData;
                        return Json(new { success = true, responseText = "Manifest Created Successfully!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        globalData.messageDetail = ResourceModules.Manifest + " " + ResourceMessages.Add_Success;
                        this.Session["globalData"] = globalData;
                        return Json(new { success = true, responseText = "Manifest Created Successfully!" }, JsonRequestBehavior.AllowGet);
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
                }
                catch (DbUpdateException exp)
                {
                    generalHelper.addControllerException(exp, "ManifestController", "Create(Post)", ((FDSession)this.Session["FDSession"]).userId);
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Manifest;
                    globalData.messageDetail = ResourceValidation.Manifest_DBUpdate_Exception;
                    this.Session["globalData"] = globalData;
                }
                catch (Exception exp)
                {
                    generalHelper.addControllerException(exp, "ManifestController", "Create(Post)", ((FDSession)this.Session["FDSession"]).userId);
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Manifest;
                    globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.Manifest + " " + ResourceGlobal.Create;
                    this.Session["globalData"] = globalData;
                }
            }

            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var zb_plantObj = db.MM_Plants.Find(plantID);

            ViewBag.Part_No = new SelectList(db.MM_PartList_View, "Part_No", "Part_No");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.OrderBy(a => a.Shop_Name).Where(a => a.Plant_ID == zb_plantObj.Plant_ID), "Shop_ID", "Shop_Name");
            return Json(new { success = false, responseText = "An Exception Occurred while uploading Manifest.Please Try Again." }, JsonRequestBehavior.AllowGet);
        }

        // GET: Manifest/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (this.Session["globalData"] != null)
            {
                globalData = (GlobalData)this.Session["globalData"];
            }
            this.Session["globalData"] = null;
            globalData.pageTitle = ResourceModules.Manifest;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Manifest";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceModules.Manifest + " " + ResourceGlobal.Edit;
            globalData.contentFooter = "";
            ViewBag.GlobalDataModel = globalData;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Manifest mM_Manifest = db.MM_Manifest.Find(id);
            if (mM_Manifest == null)
            {
                return HttpNotFound();
            }
            ViewBag.Models = null;

            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var zb_plantObj = db.MM_Plants.Find(plantID);
            var manifestObj = db.MM_Manifest
                                .Where(a => a.Part_No == mM_Manifest.Part_No && a.Station_ID == mM_Manifest.Station_ID)
                                .Select(a => new { a.Part_No, a.Part_Description })
                                .OrderBy(a => a.Part_No)
                                .Distinct();

            ViewBag.Attribute_ID = new SelectList(db.MM_Attribution_Parameters.Where(a => a.Attribute_Type == "Family" && a.Shop_ID == mM_Manifest.Shop_ID).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc");
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(a => a.Shop_ID == mM_Manifest.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", mM_Manifest.Line_ID);
            ViewBag.Part_No = new SelectList(manifestObj, "Part_No", "Part_No", mM_Manifest.Part_No);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(a => a.Plant_ID == plantID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name", mM_Manifest.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(a => a.Line_ID == mM_Manifest.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", mM_Manifest.Station_ID);

            if (mM_Manifest.Is_ParentModel_Manifest == true)
            {
                List<String> selectedModelList = db.MM_Manifest.Where(a => a.Identification == mM_Manifest.Identification && a.Is_ParentModel_Manifest == true).Select(a => a.Part_No).Distinct().ToList();
                List<CheckModel> modelList = new List<CheckModel>();

                IEnumerable<MM_Model_Master> modelCodeObj = db.MM_Model_Master.Where(a => a.Family == mM_Manifest.Attribute_ID && a.Shop_ID == mM_Manifest.Shop_ID).OrderBy(a => a.MM_Series.Series_Description);
                foreach (MM_Model_Master obj in modelCodeObj)
                {
                    modelList.Add(new CheckModel { Id = obj.Model_Code, Name = (obj.MM_Series != null) ? obj.MM_Series.Series_Description : obj.Model_Code, Checked = selectedModelList.Contains(obj.Model_Code) });
                }
                ViewBag.Models = modelList;
                ViewBag.Attribute_ID = new SelectList(db.MM_Attribution_Parameters.Where(a => a.Attribute_Type == "Family" && a.Shop_ID == mM_Manifest.Shop_ID).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc", mM_Manifest.Attribute_ID);
            }
            return View(mM_Manifest);
        }

        // POST: Manifest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Manifest_ID,Part_No,Identification,Image_Name,Plant_ID,Shop_ID,Line_ID,Station_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,ModelCodes,Attribute_ID,Is_ParentModel_Manifest")] MM_Manifest mM_Manifest, decimal StationID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Files.Count > 0)
                    {
                        int imageCntr = db.MM_Manifest.Where(a => a.Part_No == mM_Manifest.Part_No && a.Station_ID == mM_Manifest.Station_ID).ToList().Count();

                        Random rnd = new Random();
                        string randomIdentifier = rnd.Next(0, 500).ToString();
                        foreach (string imageName in Request.Files)
                        {
                            imageCntr++;

                            HttpPostedFileBase PartImage = Request.Files[imageName];

                            if (PartImage != null && PartImage.ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(PartImage.FileName);
                                if (mM_Manifest.Is_ParentModel_Manifest == true)
                                {
                                    // width will increase the height proportionally
                                    ImageUpload imageUpload = new ImageUpload { Width = 2000 };

                                    // rename, resize, and upload
                                    //return object that contains {bool Success,string ErrorMessage,string ImageName}
                                    // ImageResult imageResult = imageUpload.RenameUploadFile(PartImage, "ManifestImages/" + mM_Manifest.Station_ID + "_" + mM_Manifest.Attribute_ID + "_" + randomIdentifier + "_" + imageCntr.ToString());
                                    ImageResult imageResult = setimg.RenameUploadFile(PartImage, mM_Manifest.Station_ID + "_" + mM_Manifest.Attribute_ID + "_" + randomIdentifier + "_" + imageCntr.ToString(), "MANIFEST");
                                    foreach (string modelCode in mM_Manifest.ModelCodes)
                                    {
                                        if (!String.IsNullOrWhiteSpace(modelCode))
                                        {
                                            mM_Manifest.Inserted_Date = DateTime.Now;
                                            mM_Manifest.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                                            string IP = Request.UserHostName;
                                            string compName = ((FDSession)this.Session["FDSession"]).userHost;
                                            mM_Manifest.Inserted_Host = compName;
                                            mM_Manifest.Part_No = modelCode;
                                            mM_Manifest.Identification = mM_Manifest.Identification;
                                            MM_PartList_View partlist = db.MM_PartList_View.Where(a => a.Part_No == modelCode).FirstOrDefault();
                                            if (partlist != null)
                                            {
                                                mM_Manifest.Part_Description = partlist.Part_Description;
                                            }
                                            else
                                            {
                                                mM_Manifest.Part_Description = "";
                                            }

                                            mM_Manifest.Image_Name = imageResult.ImageName;

                                            db.MM_Manifest.Add(mM_Manifest);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    // width will increase the height proportionally
                                    ImageUpload imageUpload = new ImageUpload { Width = 700 };

                                    // rename, resize, and upload
                                    //return object that contains {bool Success,string ErrorMessage,string ImageName}
                                    // ImageResult imageResult = imageUpload.RenameUploadFile(PartImage, "ManifestImages/" + mM_Manifest.Station_ID + "_" + mM_Manifest.Part_No + "_" + imageCntr.ToString());
                                    ImageResult imageResult = setimg.RenameUploadFile(PartImage, mM_Manifest.Station_ID + "_" + mM_Manifest.Part_No + "_" + imageCntr.ToString(), "MANIFEST");

                                    mM_Manifest.Inserted_Date = DateTime.Now;
                                    mM_Manifest.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                                    //string IP = Request.UserHostName;
                                    string compName = ((FDSession)this.Session["FDSession"]).userHost;
                                    mM_Manifest.Inserted_Host = compName;

                                    mM_Manifest.Image_Name = imageResult.ImageName;

                                    db.MM_Manifest.Add(mM_Manifest);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        //THIS BLOCK RUNS WHEN THERE ARE NO NEW IMAGES UPLOADED
                        if (mM_Manifest.Is_ParentModel_Manifest == true)
                        {
                            List<String> currentModelList = db.MM_Manifest.Where(a => a.Identification == mM_Manifest.Identification).Select(a => a.Part_No).Distinct().ToList();
                            List<String> newModelList = new List<string>();
                            List<String> deletedModelList = new List<string>();
                            //mM_Manifest.ModelCodes
                            if (mM_Manifest.ModelCodes != null)
                            {
                                newModelList = (mM_Manifest.ModelCodes.ToList()).Except(currentModelList).ToList();
                                deletedModelList = (currentModelList).Except(mM_Manifest.ModelCodes.ToList()).ToList();
                            }
                            else
                            {
                                //IF MODELCODES ARE NULL MEANS USER DESELECTED ALL MODEL CODES
                                deletedModelList = currentModelList;
                            }

                            if (newModelList.Count() > 0)
                            {
                                List<string> imageList = db.MM_Manifest.Where(a => a.Identification == mM_Manifest.Identification).Select(a => a.Image_Name).Distinct().ToList();
                                foreach (var manifestImage in imageList)
                                {
                                    foreach (string modelCode in newModelList)
                                    {
                                        MM_PartList_View partlist = db.MM_PartList_View.Where(a => a.Part_No == modelCode).FirstOrDefault();
                                        if (partlist != null)
                                        {
                                            mM_Manifest.Part_Description = partlist.Part_Description;
                                        }
                                        else
                                        {
                                            mM_Manifest.Part_Description = "";
                                        }
                                        MM_Manifest manifestObj = new MM_Manifest();
                                        manifestObj.Part_No = modelCode;
                                        manifestObj.Image_Name = manifestImage;
                                        manifestObj.Identification = mM_Manifest.Identification;
                                        manifestObj.Plant_ID = mM_Manifest.Plant_ID;
                                        manifestObj.Shop_ID = mM_Manifest.Shop_ID;
                                        manifestObj.Line_ID = mM_Manifest.Line_ID;
                                        manifestObj.Station_ID = mM_Manifest.Station_ID;
                                        manifestObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                        manifestObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        manifestObj.Inserted_Date = DateTime.Now;
                                        manifestObj.Is_ParentModel_Manifest = mM_Manifest.Is_ParentModel_Manifest;
                                        db.MM_Manifest.Add(manifestObj);
                                        db.SaveChanges();
                                    }
                                }
                            }

                            if (deletedModelList.Count() > 0)
                            {
                                foreach (string modelCode in deletedModelList)
                                {
                                    List<MM_Manifest> manifestObjList = db.MM_Manifest.Where(a => a.Part_No == modelCode && a.Station_ID == mM_Manifest.Station_ID).ToList();
                                    db.MM_Manifest.RemoveRange(manifestObjList);
                                    db.SaveChanges();
                                }
                            }

                            db.MM_Manifest
                              .Where(a => a.Identification == mM_Manifest.Identification)
                              .ToList()
                              .ForEach(a =>
                              {
                                  a.Shop_ID = mM_Manifest.Shop_ID;
                                  a.Line_ID = mM_Manifest.Line_ID;
                                  a.Station_ID = mM_Manifest.Station_ID;
                                  a.Attribute_ID = mM_Manifest.Attribute_ID;
                                  a.Updated_Date = DateTime.Now;
                                  a.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                  a.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                              });
                            db.SaveChanges();
                        }
                        else
                        {
                            //if there is no new image added just update the records
                            db.MM_Manifest
                              .Where(a => a.Part_No == mM_Manifest.Part_No && a.Station_ID == StationID)
                              .ToList()
                              .ForEach(a =>
                              {
                                  a.Shop_ID = mM_Manifest.Shop_ID;
                                  a.Line_ID = mM_Manifest.Line_ID;
                                  a.Station_ID = mM_Manifest.Station_ID;
                                  a.Updated_Date = DateTime.Now;
                                  a.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                  a.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                              });

                            //MM_Manifest mmManifestObj = db.MM_Manifest.Find(mM_Manifest.Manifest_ID);
                            //mmManifestObj.Shop_ID = mM_Manifest.Shop_ID;
                            //mmManifestObj.Line_ID = mM_Manifest.Line_ID;
                            //mmManifestObj.Station_ID = mM_Manifest.Station_ID;
                            //db.Entry(mmManifestObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Manifest + " " + ResourceGlobal.Edit;
                    globalData.messageDetail = ResourceModules.Manifest + " " + ResourceMessages.Edit_Success;
                    this.Session["globalData"] = globalData;
                    // return RedirectToAction("Index");
                    if (Request.Files.Count > 0)
                    {
                        return Json(new { success = true, responseText = "Manifest Created Successfully!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return RedirectToAction("Index");
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
                    generalHelper.addControllerException(dbEx, "ManifestController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceDisplayName.SOP;
                    globalData.messageDetail = ResourceValidation.Manifest_DBUpdate_Exception;
                    this.Session["globalData"] = globalData;
                }
                catch (DbUpdateException exp)
                {
                    generalHelper.addControllerException(exp, "ManifestController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceSOPManifest.ManifestTitle;
                    globalData.messageDetail = ResourceSOPManifest.Manifest_Edit_DbUpdateException;
                    this.Session["globalData"] = globalData;
                }
                catch (Exception exp)
                {
                    generalHelper.addControllerException(exp, "ManifestController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Manifest;
                    globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.Manifest + " " + ResourceGlobal.Edit;
                    this.Session["globalData"] = globalData;
                }
            }

            if (Request.Files.Count > 0)
            {
                return Json(new { success = false, responseText = mM_Manifest.Manifest_ID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Models = null;
                ViewBag.Attribute_ID = new SelectList(db.MM_Attribution_Parameters.Where(a => a.Attribute_Type == "Family" && a.Shop_ID == mM_Manifest.Shop_ID).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc");
                decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var zb_plantObj = db.MM_Plants.Find(plantID);
                ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(a => a.Shop_ID == mM_Manifest.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", mM_Manifest.Line_ID);
                ViewBag.Part_No = new SelectList(db.MM_Manifest.Where(a => a.Part_No == mM_Manifest.Part_No).OrderBy(a => a.Part_No), "Part_No", "Part_No", mM_Manifest.Part_No);
                ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(a => a.Plant_ID == plantID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name", mM_Manifest.Shop_ID);
                ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(a => a.Line_ID == mM_Manifest.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", mM_Manifest.Station_ID);
                if (mM_Manifest.Is_ParentModel_Manifest == true)
                {
                    List<String> selectedModelList = db.MM_Manifest.Where(a => a.Identification == mM_Manifest.Identification && a.Is_ParentModel_Manifest == true).Select(a => a.Part_No).Distinct().ToList();
                    List<CheckModel> modelList = new List<CheckModel>();

                    IEnumerable<MM_Model_Master> modelCodeObj = db.MM_Model_Master.Where(a => a.Family == mM_Manifest.Attribute_ID && a.Shop_ID == mM_Manifest.Shop_ID).OrderBy(a => a.MM_Series.Series_Description);
                    foreach (MM_Model_Master obj in modelCodeObj)
                    {
                        modelList.Add(new CheckModel { Id = obj.Model_Code, Name = (obj.MM_Series != null) ? obj.MM_Series.Series_Description : obj.Model_Code, Checked = selectedModelList.Contains(obj.Model_Code) });
                    }
                    ViewBag.Models = modelList;
                    ViewBag.Attribute_ID = new SelectList(db.MM_Attribution_Parameters.Where(a => a.Attribute_Type == "Family" && a.Shop_ID == mM_Manifest.Shop_ID).OrderBy(a => a.Attribute_Desc), "Attribute_ID", "Attribute_Desc", mM_Manifest.Attribute_ID);
                }
                return View(mM_Manifest);
            }
        }


        public ActionResult ShopScreen()
        {
            try
            {
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                //if (userID == 36 || userID == 23100023 || userID == 5)
                //{
                //    ViewBag.userID = userID;
                //    return View("ShopEngineUserManifest");
                //}
                //else if (userID == 33)
                //{
                //    ViewBag.userID = userID;
                //    return View("ShopTransmissionUserManifest");
                //}
                //else if (userID == 52)
                //{
                //    ViewBag.userID = userID;
                //    return View("ShopTractorUserManifest");
                //}
                //else
                //{
                int stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                // int stationID = 52;
                ViewBag.StationID = stationID;
                int lineID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                var stationDataObj = (from a in db.MM_Stations
                                      join c in db.MM_Shops on a.Shop_ID equals c.Shop_ID
                                      join d in db.MM_Lines on a.Line_ID equals d.Line_ID
                                      join b in db.MM_Route_Configurations on a.Station_ID equals b.Station_ID into ab
                                      from b in ab.DefaultIfEmpty()
                                      where a.Station_ID == stationID
                                      select new
                                      {
                                          a.Is_Buffer,
                                          b.Is_Start_Station,
                                          c.Shop_Name,
                                          a.Line_ID,
                                          d.Line_Name,
                                          a.Station_Name
                                      }).Distinct().FirstOrDefault();
                if (stationDataObj == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ShopName = stationDataObj.Shop_Name;
                ViewBag.LineName = stationDataObj.Line_Name;
                ViewBag.StationName = stationDataObj.Station_Name;
                ViewBag.ShopScreenTitle = "Manifest Screen";

                MM_Quality_Station_List mmQualityStationListObj = new MM_Quality_Station_List();
                if (mmQualityStationListObj.isStationAddedInQualityList(stationID))
                {
                    ViewBag.ShopScreenTitle = "Quality Capture";
                    ViewBag.IsQualityStation = true;
                }
                else
                {
                    ViewBag.IsQualityStation = false;
                }

                // var stationDataObj = db.MM_Stations.Find(stationID);
                string serialNo = "";
                if (stationDataObj.Is_Buffer == null || stationDataObj.Is_Buffer == false)
                {
                    var geSerialNoObj = db.MM_Station_Tracking
                                                  .Where(a => a.Station_ID == stationID)
                                                  .Select(a => new { a.SerialNo }).Distinct().FirstOrDefault();
                    serialNo = geSerialNoObj.SerialNo;

                    if (!String.IsNullOrWhiteSpace(serialNo) && serialNo.ToUpper().Trim() != "EMPTYPITCH")
                    {
                        ViewBag.PartsImage = getImageNameFromSerialNo(serialNo, stationID);
                        ErrorProofing efObj = new ErrorProofing();
                        int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
                        String mCode = efObj.getModelcodeBySerialNumber(serialNo, plantID);
                        MM_Partmaster[] partObj = efObj.getStationPartListForStation(stationID, mCode, serialNo);
                        MM_Partgroup[] partGroups = getNoPartPartGroup(stationID);
                        ViewBag.EFGeneologyParts = partObj;
                        ViewBag.EFpartGroups = partGroups;
                        ViewBag.ModelCode = mCode;
                        ViewBag.SerialNo = serialNo;
                    }
                    //if(stationID==127 ||stationID==128)
                    //{
                    //    return View("TyreErrorProofing",stationDataObj);
                    //}
                    return View("ManifestScreen", stationDataObj);

                    //if (stationDataObj.Is_Start_Station == null || stationDataObj.Is_Start_Station == false)
                    //{
                    //    return View("ManifestScreen", stationDataObj);
                    //}
                    //else
                    //{
                    //    //CHECK IF THE LINE IS ORDER START LINE 
                    //    if (db.MM_Partgroup.Any(a => a.Line_ID == stationDataObj.Line_ID))
                    //    {
                    //        return View("ManifestScreen", stationDataObj);
                    //    }
                    //    //return View("FirstStationManifestScreen", stationDataObj);
                    //}
                }
                else
                {
                    return View("ManifestScreen", stationDataObj);
                }
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "ManifestController", "ShopScreen()", ((FDSession)this.Session["FDSession"]).userId);
            }
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }

        private MM_Partgroup[] getNoPartPartGroup(int stationID, string serial = null)
        {
            try
            {
                if (serial == null)
                {
                    serial = db.MM_Station_Tracking.Where(a => a.Station_ID == stationID).FirstOrDefault().SerialNo;
                }
                ErrorProofing epObj = new ErrorProofing();
                int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
                string modelCode = epObj.getChildModelcodeBySerialNumber(serial, plantID);

                string omConfigID = db.MM_Model_Master.Where(a => a.Model_Code == modelCode && a.Plant_ID== plantID).Select(a => a.OMconfig_ID).FirstOrDefault();

                //MM_Partgroup[] mmpartgroups = (from a in db.MM_Partgroup
                //                               join b in db.MM_OM_Configuration on a.Partgroup_ID equals b.Partgroup_ID
                //                               where a.Consumption_Station_ID == stationID && b.OMconfig_ID == omConfigID
                //                               && a.Partgroup_ID != null && b.Partgroup_ID != null
                //                               && !(from mmGeneology in db.MM_Geneaology
                //                                    where mmGeneology.Main_Order_Serial_No == serial && mmGeneology.Station_ID == stationID
                //                                    select mmGeneology.PartGroup_ID).Contains(a.Partgroup_ID)
                //                               select a).ToArray();

                MM_Partgroup[] mmpartgroups = (from a in db.MM_Partgroup
                                               join b in db.MM_OM_Configuration on a.Partgroup_ID equals b.Partgroup_ID
                                               where a.Consumption_Station_ID == stationID && b.OMconfig_ID == omConfigID
                                               && a.Partgroup_ID != null && b.Partgroup_ID != null
                                               select a).ToArray();

                foreach (MM_Partgroup mmpartGroupObj in mmpartgroups)
                {
                    int geneologyDoneCount = db.MM_Geneaology.Where(a => a.Main_Order_Serial_No == serial && a.Station_ID == stationID && a.PartGroup_ID == mmpartGroupObj.Partgroup_ID).ToList().Count();
                    mmpartGroupObj.Qty = Math.Abs(mmpartGroupObj.Qty.GetValueOrDefault(1) - geneologyDoneCount);
                }

                return mmpartgroups;

            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "ManifestController", "getNoPartPartGroup(StationID: " + stationID + ")", ((FDSession)this.Session["FDSession"]).userId);
            }
            return null;
        }

        //code added by jagdish
        //code added by jagdish 11-04-2017

        //code 
        //code
        //code
        // [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        //public ActionResult TyreErrorProofing(string mainserialNo, string childSerialNo1, string childSerialNo2, string partgroupID)
        //{
        //    JSONData jsondata = new JSONData();
        //    jsondata.status = false;
        //    decimal stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

        //    var mainModelCode = db.MM_OM_Order_List.Where(mm => mm.Serial_No == mainserialNo).Select(mcode => mcode.Model_Code).FirstOrDefault();
        //    var Child1 = db.MM_OM_Order_List.Where(cm_code => cm_code.Serial_No == childSerialNo1).FirstOrDefault();
        //    var Child2 = db.MM_OM_Order_List.Where(cm_code => cm_code.Serial_No == childSerialNo2).FirstOrDefault();


        //    if (mainModelCode == Child1.Model_Code && mainModelCode == Child2.Model_Code)
        //    {
        //        if (Child1.Order_Status != "Closed" && Child2.Order_Status != "Closed")
        //        {
        //            // check make and size of both child serial number
        //            //code for featch details in another table
        //            var MakeInNew1 = db.MM_OM_Make_Tyre_Details.Where(mm => mm.Model_Code == Child1.Model_Code && mm.Serial_No == childSerialNo1).FirstOrDefault();
        //            var MakeInNew2 = db.MM_OM_Make_Tyre_Details.Where(mm => mm.Model_Code == Child2.Model_Code && mm.Serial_No == childSerialNo2).FirstOrDefault();

        //            if ((MakeInNew1 != null && MakeInNew2 != null) && (MakeInNew1.Make_ID == MakeInNew2.Make_ID))
        //            {
        //                if (MakeInNew1.Tyre_Size.ToString() == MakeInNew2.Tyre_Size.ToString())
        //                {
        //                    jsondata.status = true;
        //                    jsondata.message = "TYRE size Matched............";
        //                }
        //                else
        //                {
        //                    jsondata.status = false;
        //                    jsondata.message = "TYRE size Not Matched............";
        //                }
        //            }
        //            else
        //            {
        //                jsondata.status = false;
        //                jsondata.message = "Tyre order data not found";
        //                // process to stop line
        //            }

        //        }
        //        else
        //        {
        //            jsondata.status = false;
        //            jsondata.message = "Child order is already used";
        //            // process to stop line
        //        }

        //    }
        //    else
        //    {
        //        jsondata.status = false;
        //        jsondata.message = "Model Code Not Matched............";
        //        // process to stop line
        //    }


        //    return Json(jsondata, JsonRequestBehavior.AllowGet);
        //}

        ////end



        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult isErrorProofingOK(string mainserialNo, string childSerialNo, string partNumb)
        {
            ErrorProofing efObj = new ErrorProofing();
            mainserialNo = mainserialNo.Trim().ToUpper();
            childSerialNo = childSerialNo.Trim().ToUpper();

            //GET PARENT MODEL CODE FROM SERIAL NO
            int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
            String modelCode = efObj.getModelcodeBySerialNumber(mainserialNo,plantID);
            try
            {
                bool isOK = false;
                decimal shopID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                decimal lineID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                decimal stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                //GET CHILD PARTNO FROM SERIAL NO 
                MM_OM_Order_List orderListObj = db.MM_OM_Order_List.Where(a => a.Serial_No == childSerialNo).FirstOrDefault();
                if (orderListObj != null)
                { // THIS BLOCK RUNS WHEN THE CHILD SERIAL NO IS ON THE ORDDER LIST TABLE

                    String partNumber = orderListObj.partno;//child Part No./Model Code
                    MM_Partmaster partmasterObj = db.MM_Partmaster.Find(partNumber);
                    if (partmasterObj != null)
                    {// THIS BLOCK RUNS WHEN THE CHILD PART NO IS IN THE PART MASTER
                        if (partmasterObj.Error_Proofing == true)
                        {
                            //CHECK IF THE PARTNO IS IN THE PARENT BOM AND THE CONSUMPTION STATIONID IS THE SAME AS CURRENT STATION ID
                            var bomObj = (from bomItem in db.MM_BOM_Item
                                          join partmaster in db.MM_Partmaster on bomItem.Part_No equals partmaster.Part_No
                                          where bomItem.Model_Code == modelCode && partmaster.Part_No == partNumber && partmaster.Station_ID == stationID
                                          select bomItem).ToList();

                            if (bomObj != null)
                            {
                                //CHECK IF THE CHILD SERIALNO IS ALREADY FITTED TO ANOTHER MAIN PART
                                if (bomObj.Count() > 0 && db.MM_Geneaology.Any(a => a.Child_Order_Serial_No == childSerialNo && a.Main_Order_Serial_No != mainserialNo) == false)
                                {
                                    //CHECK IF THE CHILD SERIALNO IS ALREADY FITTED TO THE SAME MAIN PART
                                    if (db.MM_Geneaology.Any(a => a.Main_Order_Serial_No == mainserialNo && a.Child_Order_Serial_No == childSerialNo) == false)
                                    {
                                        MM_Geneaology geneologyObj = new MM_Geneaology();
                                        geneologyObj.Plant_ID = plantID;
                                        geneologyObj.Shop_ID = shopID;
                                        geneologyObj.Line_ID = lineID;
                                        geneologyObj.Station_ID = stationID;
                                        geneologyObj.Main_Model_Code = modelCode;
                                        geneologyObj.Child_Model_Code = partNumber;
                                        geneologyObj.Main_Order_Serial_No = mainserialNo;
                                        geneologyObj.Child_Order_Serial_No = childSerialNo;
                                        geneologyObj.PartGroup_ID = partmasterObj.Partgroup_ID;

                                        geneologyObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                        geneologyObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        geneologyObj.Inserted_Date = DateTime.Now;

                                        db.MM_Geneaology.Add(geneologyObj);
                                        db.SaveChanges();

                                        //UPDATE THE ORDER STATUS TO "Closed"
                                        UpdateModel(orderListObj);
                                        orderListObj.Order_Status = "Closed";
                                        orderListObj.Is_Edited = true;
                                        db.Entry(orderListObj).State = EntityState.Modified;
                                        db.SaveChanges();

                                        //RESUME THE LINE IF THERE IS ALREADY LINSESTOP DUE TO SAME REASON AND THERE ARE NO ERROR PROOFING PARTS AGAINST THE MAIN SERIAL NO
                                        //bool isInLineStopHistory = db.MM_History_LineStop.Any(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.isLineStop == true);
                                        bool isPendingEFParts = isPendingErrorProofing(Convert.ToInt32(stationID), Convert.ToInt32(lineID), mainserialNo);
                                        MM_History_LineStop lineStopByEPNotScannedObj = db.MM_History_LineStop.Where(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Pending ErrorProofing or Geneaology Scan" && a.isLineStop == true).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                                        //if (isInLineStopHistory && isPendingEFParts == false)
                                        //{
                                        if (isPendingEFParts == false)
                                        {//IF THERE ARE NO PENDING PARTS TO BE SCANNED RESUME THE LINE
                                            Kepware kepwareObj = new Kepware();
                                            kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                            kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");

                                            if (lineStopByEPNotScannedObj != null)
                                            {//UPDATE THE RESUME TIME IF THERE IS ANY LINE STOP DUE TO NOT SCANNING THE EF PARTS WIHTIN TIME LIMIT
                                                DateTime dateTimeNow;
                                                TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                                                if (dateObj != null)
                                                {
                                                    dateTimeNow = dateObj.todayDate;
                                                }
                                                else
                                                {
                                                    dateTimeNow = DateTime.Now;
                                                }
                                                UpdateModel(lineStopByEPNotScannedObj);
                                                lineStopByEPNotScannedObj.Resume_Time = dateTimeNow;
                                                lineStopByEPNotScannedObj.Is_Edited = true;
                                                db.Entry(lineStopByEPNotScannedObj).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            if (lineStopByEPNotScannedObj == null)
                                            {//IF THERE ARE SOME PENDING ORDERS AND THE LINE IS NOT STOPPPED DUE TO PENDING SCANNING OF EP PARTS WIHTIN TIME LIMIT
                                                // THEN RESUME THE LINE 
                                                Kepware kepwareObj = new Kepware();
                                                kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                                kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                                            }
                                        }

                                        //var checkCountObj = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Line_ID == lineID).ToList();
                                        //if (checkCountObj.Count > 1)
                                        //{

                                        MM_History_LineStop originalLSData = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.Line_ID == lineID && a.Station_ID == stationID).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                                        if (originalLSData != null)
                                        {
                                            DateTime dateTimeNow;
                                            TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                                            if (dateObj != null)
                                            {
                                                dateTimeNow = dateObj.todayDate;
                                            }
                                            else
                                            {
                                                dateTimeNow = DateTime.Now;
                                            }
                                            UpdateModel(originalLSData);
                                            originalLSData.Resume_Time = dateTimeNow;
                                            originalLSData.Is_Edited = true;
                                            db.Entry(originalLSData).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        //}
                                    }
                                    return Json("true", JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    //TODO: CALL LINESTOP FUNCTION
                                    generalHelper.logLineStopData(plantID, shopID, lineID, stationID, "Error Proofing", "Part No. Not Valid (NOT FOUND IN BOM OR ALREADY IN GENEOLOGY) ! Child PartNo: " + partNumber + " :: Child Serial No. : " + childSerialNo + " :: Parent SrlNo. : " + mainserialNo + " :: Parent ModelCode :" + modelCode);
                                    return Json("false", JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                //TODO: CALL LINESTOP FUNCTION
                                generalHelper.logLineStopData(plantID, shopID, lineID, stationID, "Error Proofing", "Part No. Not Valid (NOT FOUND IN BOM) ! Child PartNo: " + partNumber + " :: Child Serial No. : " + childSerialNo + " :: Parent SrlNo. : " + mainserialNo + " :: Parent ModelCode :" + modelCode);
                                return Json("false", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {//THIS BLOCK WILL RUN WHEN THE PART IS ONLY A GENEOLOGY PART AND NOT ERROR PROOFING

                            //CHECK IF ALREADY EXIST IN GENEOLOGY TABLE AGAINST ANOTHER MAIN PART
                            if (db.MM_Geneaology.Any(a => a.Child_Order_Serial_No == childSerialNo && a.Main_Order_Serial_No != mainserialNo) == false)
                            {
                                if (db.MM_Geneaology.Any(a => a.Main_Order_Serial_No == mainserialNo && a.Child_Order_Serial_No == childSerialNo) == false)
                                {
                                    MM_Geneaology geneologyObj = new MM_Geneaology();
                                    geneologyObj.Plant_ID = plantID;
                                    geneologyObj.Shop_ID = shopID;
                                    geneologyObj.Line_ID = lineID;
                                    geneologyObj.Station_ID = stationID;
                                    geneologyObj.Main_Model_Code = modelCode;
                                    geneologyObj.Child_Model_Code = partNumber;
                                    geneologyObj.Main_Order_Serial_No = mainserialNo;
                                    geneologyObj.Child_Order_Serial_No = childSerialNo;
                                    geneologyObj.PartGroup_ID = partmasterObj.Partgroup_ID;

                                    geneologyObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    geneologyObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    geneologyObj.Inserted_Date = DateTime.Now;

                                    db.MM_Geneaology.Add(geneologyObj);
                                    db.SaveChanges();

                                    //UPDATE THE ORDER STATUS TO "Closed"
                                    UpdateModel(orderListObj);
                                    orderListObj.Order_Status = "Closed";
                                    orderListObj.Is_Edited = true;
                                    db.Entry(orderListObj).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //RESUME THE LINE IF THERE IS ALREADY LINSESTOP DUE TO SAME REASON AND THERE ARE NO ERROR PROOFING PARTS AGAINST THE MAIN SERIAL NO
                                    //bool isInLineStopHistory = db.MM_History_LineStop.Any(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.isLineStop == true);
                                    bool isPendingEFParts = isPendingErrorProofing(Convert.ToInt32(stationID), Convert.ToInt32(lineID), mainserialNo);
                                    MM_History_LineStop lineStopByEPNotScannedObj = db.MM_History_LineStop.Where(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Pending ErrorProofing or Geneaology Scan" && a.isLineStop == true).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                                    //if (isInLineStopHistory && isPendingEFParts == false)
                                    //{
                                    if (isPendingEFParts == false)
                                    {//IF THERE ARE NO PENDING PARTS TO BE SCANNED RESUME THE LINE
                                        Kepware kepwareObj = new Kepware();
                                        kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                        kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                                        if (lineStopByEPNotScannedObj != null)
                                        {//UPDATE THE RESUME TIME IF THERE IS ANY LINE STOP DUE TO NOT SCANNING THE EF PARTS WIHTIN TIME LIMIT
                                            DateTime dateTimeNow;
                                            TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                                            if (dateObj != null)
                                            {
                                                dateTimeNow = dateObj.todayDate;
                                            }
                                            else
                                            {
                                                dateTimeNow = DateTime.Now;
                                            }
                                            UpdateModel(lineStopByEPNotScannedObj);
                                            lineStopByEPNotScannedObj.Resume_Time = dateTimeNow;
                                            lineStopByEPNotScannedObj.Is_Edited = true;
                                            db.Entry(lineStopByEPNotScannedObj).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (lineStopByEPNotScannedObj == null)
                                        {//IF THERE ARE SOME PENDING ORDERS AND THE LINE IS NOT STOPPPED DUE TO PENDING SCANNING THE EF PARTS WIHTIN TIME LIMIT
                                            // THEN RESUME THE LINE 
                                            Kepware kepwareObj = new Kepware();
                                            kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                            kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                                        }
                                    }

                                    //var checkCountObj = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Line_ID == lineID).ToList();
                                    //if (checkCountObj.Count > 1)
                                    //{
                                    MM_History_LineStop OriginalLSData = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.Line_ID == lineID && a.Station_ID == stationID).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                                    if (OriginalLSData != null)
                                    {
                                        DateTime dateTimeNow;
                                        TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                                        if (dateObj != null)
                                        {
                                            dateTimeNow = dateObj.todayDate;
                                        }
                                        else
                                        {
                                            dateTimeNow = DateTime.Now;
                                        }
                                        UpdateModel(OriginalLSData);
                                        OriginalLSData.Resume_Time = dateTimeNow;
                                        OriginalLSData.Is_Edited = true;
                                        db.Entry(OriginalLSData).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                //}
                                return Json("true", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                //TODO: CALL LINESTOP FUNCTION
                                generalHelper.logLineStopData(plantID, shopID, lineID, stationID, "Error Proofing", "Part No. Not Valid (SERIAL ALREADY SCANNED FOR GENEOLOGY) ! Child PartNo: " + partNumber + " :: Child Serial No. : " + childSerialNo + " :: Parent SrlNo. : " + mainserialNo + " :: Parent ModelCode :" + modelCode);
                                return Json("false", JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        //TODO: CALL LINESTOP FUNCTION
                        generalHelper.logLineStopData(plantID, shopID, lineID, stationID, "Error Proofing", "Part No. " + partNumber + " Not found in PART MASTER for Serial No. " + childSerialNo + " !");
                        return Json("false", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {//THIS BLOCK RUNS IF THE SERIAL NO IS NOT PRESENT IN THE ORDER LIST (MM_OM_ORDER_LIST)

                    partNumb = partNumb.Trim().ToUpper();
                    MM_Partmaster partmasterObj = db.MM_Partmaster.Where(a => a.Part_No == partNumb && a.is_Non_MM_Barcode == true && a.Std_Char != null && a.Start_Position != null).FirstOrDefault();

                    //CHECK IF THE PART NUMBER IS VENDOR PART
                    if (partmasterObj != null)
                    {
                        if (partmasterObj.Error_Proofing == true)
                        {
                            //CHECK IF THE SERIAL NO ALREADY EXIST IN GENEOLOGY TABLE
                            if (db.MM_Geneaology.Any(a => a.Child_Order_Serial_No == childSerialNo && a.Main_Order_Serial_No != mainserialNo) == false)
                            {
                                string stdChar = partmasterObj.Std_Char;
                                int startPos = Convert.ToInt16(partmasterObj.Start_Position);
                                int monthStartPos = Convert.ToInt16(partmasterObj.Month_Start_Position);
                                int yearStartPos = Convert.ToInt16(partmasterObj.Year_Start_Position);

                                string monthChar = db.MM_Month.Where(a => a.Identifier_ID == partmasterObj.Month_Identifier).Select(a => a.Month_Code).FirstOrDefault().Trim();
                                string yearChar = db.MM_Year.Where(a => a.Identifier_ID == partmasterObj.Year_Identifier).Select(a => a.Year_Code).FirstOrDefault().Trim();

                                string generatedStdChar = childSerialNo.Substring(startPos, stdChar.Length);
                                string generatedMonthChar = childSerialNo.Substring(monthStartPos, monthChar.Length);
                                string generatedYearChar = childSerialNo.Substring(yearStartPos, yearChar.Length);

                                int monthInChildSerialNo = db.MM_Month.Where(a => a.Identifier_ID == partmasterObj.Month_Identifier && a.Month_Code == generatedMonthChar).FirstOrDefault().Month_No.GetValueOrDefault(0);
                                int yearInChildSerialNo = Convert.ToInt16(db.MM_Year.Where(a => a.Identifier_ID == partmasterObj.Year_Identifier && a.Year_Code == generatedYearChar).FirstOrDefault().Year.GetValueOrDefault(0));

                                DateTime childSerialNoDate = Convert.ToDateTime(yearInChildSerialNo.ToString() + "-" + monthInChildSerialNo.ToString().PadLeft(2, '0') + "-" + DateTime.Today.Day.ToString());

                                MM_Model_Master modelMasterObj = db.MM_Model_Master.Find(modelCode);
                                DateTime validOldDate;
                                if (modelMasterObj.Is_Domestic.GetValueOrDefault(true) == true)
                                {
                                    //IF IT IS DOMESTIC MODEL
                                    validOldDate = DateTime.Today.AddMonths(-6);
                                }
                                else
                                {
                                    //IF IT IS EXPORT MODEL
                                    validOldDate = DateTime.Today.AddYears(-1);
                                }

                                if (generatedStdChar.Equals(stdChar, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (childSerialNoDate.Date >= validOldDate.Date)
                                    {
                                        if (db.MM_Geneaology.Any(a => a.Main_Order_Serial_No == mainserialNo && a.Child_Order_Serial_No == childSerialNo) == false)
                                        {
                                            MM_Geneaology geneologyObj = new MM_Geneaology();
                                            geneologyObj.Plant_ID = plantID;
                                            geneologyObj.Shop_ID = shopID;
                                            geneologyObj.Line_ID = lineID;
                                            geneologyObj.Station_ID = stationID;
                                            geneologyObj.Main_Model_Code = modelCode;
                                            geneologyObj.Child_Model_Code = partNumb;
                                            geneologyObj.Main_Order_Serial_No = mainserialNo;
                                            geneologyObj.Child_Order_Serial_No = childSerialNo;
                                            geneologyObj.PartGroup_ID = partmasterObj.Partgroup_ID;

                                            geneologyObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                            geneologyObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                            geneologyObj.Inserted_Date = DateTime.Now;

                                            db.MM_Geneaology.Add(geneologyObj);
                                            db.SaveChanges();

                                            bool isPendingEFParts = isPendingErrorProofing(Convert.ToInt32(stationID), Convert.ToInt32(lineID), mainserialNo);
                                            MM_History_LineStop lineStopByEPNotScannedObj = db.MM_History_LineStop.Where(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Pending ErrorProofing or Geneaology Scan" && a.isLineStop == true).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                                            if (isPendingEFParts == false)
                                            {//IF THERE ARE NO PENDING PARTS TO BE SCANNED RESUME THE LINE
                                                Kepware kepwareObj = new Kepware();
                                                kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                                kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                                                if (lineStopByEPNotScannedObj != null)
                                                {//UPDATE THE RESUME TIME IF THERE IS ANY LINE STOP DUE TO NOT SCANNING THE EF PARTS WIHTIN TIME LIMIT
                                                    DateTime dateTimeNow;
                                                    TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                                                    if (dateObj != null)
                                                    {
                                                        dateTimeNow = dateObj.todayDate;
                                                    }
                                                    else
                                                    {
                                                        dateTimeNow = DateTime.Now;
                                                    }
                                                    UpdateModel(lineStopByEPNotScannedObj);
                                                    lineStopByEPNotScannedObj.Resume_Time = dateTimeNow;
                                                    lineStopByEPNotScannedObj.Is_Edited = true;
                                                    db.Entry(lineStopByEPNotScannedObj).State = EntityState.Modified;
                                                    db.SaveChanges();
                                                }
                                            }
                                            else
                                            {
                                                if (lineStopByEPNotScannedObj == null)
                                                {//IF THERE ARE SOME PENDING ORDERS AND THE LINE IS NOT STOPPPED DUE TO PENDING SCANNING THE EF PARTS WIHTIN TIME LIMIT
                                                    // THEN RESUME THE LINE 
                                                    Kepware kepwareObj = new Kepware();
                                                    kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                                    kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                                                }
                                            }

                                            MM_History_LineStop OriginalLSData = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.Line_ID == lineID && a.Station_ID == stationID).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                                            if (OriginalLSData != null)
                                            {
                                                DateTime dateTimeNow;
                                                TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                                                if (dateObj != null)
                                                {
                                                    dateTimeNow = dateObj.todayDate;
                                                }
                                                else
                                                {
                                                    dateTimeNow = DateTime.Now;
                                                }
                                                UpdateModel(OriginalLSData);
                                                OriginalLSData.Resume_Time = dateTimeNow;
                                                OriginalLSData.Is_Edited = true;
                                                db.Entry(OriginalLSData).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                        return Json("true", JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        //TODO: CALL LINESTOP FUNCTION
                                        generalHelper.logLineStopData(plantID, shopID, lineID, stationID, "Error Proofing", "Vendor Part No. Not Valid (Serial No Date:" + childSerialNoDate.Month + "-" + childSerialNoDate.Year + " :: Least Valid Date:" + validOldDate.Month + "-" + validOldDate.Year + " )! Child PartNo: " + partNumb + " :: Child Serial No. : " + childSerialNo + " :: Parent SrlNo. : " + mainserialNo + " :: Parent ModelCode :" + modelCode);
                                        return Json("false", JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    //TODO: CALL LINESTOP FUNCTION
                                    generalHelper.logLineStopData(plantID, shopID, lineID, stationID, "Error Proofing", "Vendor Part No. Not Valid (DB Std_Char:" + stdChar + " :: Generated Std_Char:" + generatedStdChar + " )! Child PartNo: " + partNumb + " :: Child Serial No. : " + childSerialNo + " :: Parent SrlNo. : " + mainserialNo + " :: Parent ModelCode :" + modelCode);
                                    return Json("false", JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                //TODO: CALL LINESTOP FUNCTION
                                generalHelper.logLineStopData(plantID, shopID, lineID, stationID, "Error Proofing", "Part No. Not Valid (SERIAL ALREADY SCANNED FOR GENEOLOGY) ! Child PartNo: " + partNumb + " :: Child Serial No. : " + childSerialNo + " :: Parent SrlNo. : " + mainserialNo + " :: Parent ModelCode :" + modelCode);
                                return Json("false", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            //THIS BLOCK WILL RUN IF THE SCANNED Serial IS NOT IN ORDER LIST AND THE PARTNUMBER IS MARKED ONLY FOR GENEOLOGY
                            if (db.MM_Geneaology.Any(a => a.Main_Order_Serial_No == mainserialNo && a.Child_Order_Serial_No == childSerialNo) == false)
                            {
                                MM_Geneaology geneologyObj = new MM_Geneaology();
                                geneologyObj.Plant_ID = plantID;
                                geneologyObj.Shop_ID = shopID;
                                geneologyObj.Line_ID = lineID;
                                geneologyObj.Station_ID = stationID;
                                geneologyObj.Main_Model_Code = modelCode;
                                geneologyObj.Child_Model_Code = partNumb;
                                geneologyObj.Main_Order_Serial_No = mainserialNo;
                                geneologyObj.Child_Order_Serial_No = childSerialNo;
                                geneologyObj.PartGroup_ID = partmasterObj.Partgroup_ID;

                                geneologyObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                geneologyObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                geneologyObj.Inserted_Date = DateTime.Now;

                                db.MM_Geneaology.Add(geneologyObj);
                                db.SaveChanges();

                                //RESUME THE LINE IF THERE IS ALREADY LINSESTOP DUE TO SAME REASON AND THERE ARE NO ERROR PROOFING PARTS AGAINST THE MAIN SERIAL NO
                                //bool isInLineStopHistory = db.MM_History_LineStop.Any(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.isLineStop == true);
                                bool isPendingEFParts = isPendingErrorProofing(Convert.ToInt32(stationID), Convert.ToInt32(lineID), mainserialNo);
                                MM_History_LineStop lineStopByEPNotScannedObj = db.MM_History_LineStop.Where(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Pending ErrorProofing or Geneaology Scan" && a.isLineStop == true).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                                //if (isInLineStopHistory && isPendingEFParts == false)
                                //{
                                if (isPendingEFParts == false)
                                {//IF THERE ARE NO PENDING PARTS TO BE SCANNED RESUME THE LINE
                                    Kepware kepwareObj = new Kepware();
                                    kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                    kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                                    if (lineStopByEPNotScannedObj != null)
                                    {//UPDATE THE RESUME TIME IF THERE IS ANY LINE STOP DUE TO NOT SCANNING THE EF PARTS WIHTIN TIME LIMIT
                                        DateTime dateTimeNow;
                                        TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                                        if (dateObj != null)
                                        {
                                            dateTimeNow = dateObj.todayDate;
                                        }
                                        else
                                        {
                                            dateTimeNow = DateTime.Now;
                                        }
                                        UpdateModel(lineStopByEPNotScannedObj);
                                        lineStopByEPNotScannedObj.Resume_Time = dateTimeNow;
                                        lineStopByEPNotScannedObj.Is_Edited = true;
                                        db.Entry(lineStopByEPNotScannedObj).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    if (lineStopByEPNotScannedObj == null)
                                    {//IF THERE ARE SOME PENDING ORDERS AND THE LINE IS NOT STOPPPED DUE TO PENDING SCANNING THE EF PARTS WIHTIN TIME LIMIT
                                        // THEN RESUME THE LINE 
                                        Kepware kepwareObj = new Kepware();
                                        kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                        kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                                    }
                                }

                                //var checkCountObj = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Line_ID == lineID).ToList();
                                //if (checkCountObj.Count > 1)
                                //{
                                MM_History_LineStop OriginalLSData = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.Line_ID == lineID && a.Station_ID == stationID).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                                if (OriginalLSData != null)
                                {
                                    DateTime dateTimeNow;
                                    TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                                    if (dateObj != null)
                                    {
                                        dateTimeNow = dateObj.todayDate;
                                    }
                                    else
                                    {
                                        dateTimeNow = DateTime.Now;
                                    }
                                    UpdateModel(OriginalLSData);
                                    OriginalLSData.Resume_Time = dateTimeNow;
                                    OriginalLSData.Is_Edited = true;
                                    db.Entry(OriginalLSData).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            //}
                            return Json("true", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //TODO: CALL LINESTOP FUNCTION
                        generalHelper.logLineStopData(plantID, shopID, lineID, stationID, "Error Proofing", "Serial No. " + childSerialNo + " Not found in Order List and also PartNumber : " + partNumb + " not defined as Vendor Part(Non MM Barcode)");
                        return Json("false", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                generalHelper.addShopControllerException(ex, "Manifest", "isErrorProofingOK(model : " + modelCode + ", Main SrNo: " + mainserialNo + ",Child SrNo: " + childSerialNo + ")", ((FDSession)this.Session["FDSession"]).stationId, ((FDSession)this.Session["FDSession"]).plantId, ((FDSession)this.Session["FDSession"]).shopId, ((FDSession)this.Session["FDSession"]).lineId, ((FDSession)this.Session["FDSession"]).userId);
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult isErrorProofingOKNoPartNo(string mainserialNo, string childSerialNo, decimal partgroupID)
        {
            ErrorProofing efObj = new ErrorProofing();
            mainserialNo = mainserialNo.Trim().ToUpper();
            childSerialNo = childSerialNo.Trim().ToUpper();
            partgroupID = partgroupID;
            //added by ketan Date 07/07/17
            bool IsKitting = IsKittingPartGroup(Convert.ToInt16(partgroupID));
            if (IsKitting)
            {
                string BarcodeString = childSerialNo;
                if (db.MM_Kitt_Barcode_Master.Any(m => m.Barcode_String == BarcodeString))
                {
                    var barcodeSerialNo = db.MM_Kitt_Barcode_Master.Where(m => m.Barcode_String == BarcodeString).
                    Select(m => m.Serial_No).FirstOrDefault();
                    if (barcodeSerialNo != null)
                    {
                        if (barcodeSerialNo.Length > 0)
                        {
                            childSerialNo = barcodeSerialNo;
                        }
                        else
                        {
                            //error mesg for wrong error proffing 
                            //line stop logic
                            return Json(new { status = false, msg="Serial Number is not genrated against this kitt !... "}, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //error mesg for wrong error proffing 
                        //line stop logic
                        return Json(new { status = false, msg = "Serial Number is not genrated against this kitt !..." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //wrong barcode Number... please enter valid kitting barcode Number
                    return Json(new { status =false,msg= "Please enter valid kitting barcode Number" }, JsonRequestBehavior.AllowGet);
                }

            }
            int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
            String modelCode = efObj.getModelcodeBySerialNumber(mainserialNo,plantID);
            try
            {
                bool isOK = false;
                decimal shopID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                decimal lineID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                decimal stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

                isOK = db.MM_OM_Order_List.Any(a => a.Serial_No == childSerialNo && a.Model_Code == modelCode && a.Partgroup_ID == partgroupID
                    && !(db.MM_Geneaology.Where(b => b.Main_Order_Serial_No != mainserialNo).Select(b => b.Child_Order_Serial_No)).Contains(childSerialNo));
                if (isOK)
                {
                    if (db.MM_Geneaology.Any(a => a.Main_Order_Serial_No == mainserialNo && a.Child_Order_Serial_No == childSerialNo) == false)
                    {
                        MM_Geneaology geneologyObj = new MM_Geneaology();
                        geneologyObj.Plant_ID = plantID;
                        geneologyObj.Shop_ID = shopID;
                        geneologyObj.Line_ID = lineID;
                        geneologyObj.Station_ID = stationID;
                        geneologyObj.Main_Model_Code = modelCode;
                        geneologyObj.Child_Model_Code = modelCode;
                        geneologyObj.Main_Order_Serial_No = mainserialNo;
                        geneologyObj.Child_Order_Serial_No = childSerialNo;
                        geneologyObj.PartGroup_ID = partgroupID;

                        geneologyObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        geneologyObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        geneologyObj.Inserted_Date = DateTime.Now;

                        db.MM_Geneaology.Add(geneologyObj);
                        db.SaveChanges();

                        //UPDATE THE ORDER STATUS TO "Closed"
                        MM_OM_Order_List orderListObj = db.MM_OM_Order_List.Where(a => a.Serial_No == childSerialNo).FirstOrDefault();
                        UpdateModel(orderListObj);
                        orderListObj.Order_Status = "Closed";
                        orderListObj.Is_Edited = true;
                        db.Entry(orderListObj).State = EntityState.Modified;
                        db.SaveChanges();

                        //RESUME THE LINE IF IT WAS STOPPED DUE TO ERROR PROOFING AND THE SAME STATION ID
                        bool isPendingEFParts = isPendingErrorProofing(Convert.ToInt32(stationID), Convert.ToInt32(lineID), mainserialNo);
                        MM_History_LineStop lineStopByEPNotScannedObj = db.MM_History_LineStop.Where(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Pending ErrorProofing or Geneaology Scan" && a.isLineStop == true).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                        //if (isInLineStopHistory && isPendingEFParts == false)
                        //{
                        if (isPendingEFParts == false)
                        {//IF THERE ARE NO PENDING EP PARTS TO BE SCANNED THEN RESUME THE LINE
                            //if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("IsCallToIoT")))
                            //{
                            Kepware kepwareObj = new Kepware();
                            kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                            kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                            //}
                            //else
                            //{
                            //    MM_Ctrl_LineStopEmergency lineCtrlStopObj = new MM_Ctrl_LineStopEmergency();
                            //    lineCtrlStopObj.Plant_ID = plantID;
                            //    lineCtrlStopObj.Shop_ID = shopID;
                            //    lineCtrlStopObj.Line_ID = lineID;
                            //    lineCtrlStopObj.Station_ID = stationID;
                            //    lineCtrlStopObj.Line_Stop = false;
                            //    lineCtrlStopObj.Emergency_Call = false;
                            //    lineCtrlStopObj.Heart_Bit = false;
                            //    lineCtrlStopObj.Update_Cntr = 0;
                            //    lineCtrlStopObj.Inserted_Date = DateTime.Now;
                            //    db.MM_Ctrl_LineStopEmergency.Add(lineCtrlStopObj);
                            //    db.SaveChanges();
                            //}
                            if (lineStopByEPNotScannedObj != null)
                            {//UPDATE THE RESUME TIME IF THERE IS ANY LINE STOP DUE TO NOT SCANNING THE EF PARTS WIHTIN TIME LIMIT
                                DateTime dateTimeNow;
                                TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                                if (dateObj != null)
                                {
                                    dateTimeNow = dateObj.todayDate;
                                }
                                else
                                {
                                    dateTimeNow = DateTime.Now;
                                }
                                UpdateModel(lineStopByEPNotScannedObj);
                                lineStopByEPNotScannedObj.Resume_Time = dateTimeNow;
                                lineStopByEPNotScannedObj.Is_Edited = true;
                                db.Entry(lineStopByEPNotScannedObj).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (lineStopByEPNotScannedObj == null)
                            {//IF THERE ARE SOME PENDING ORDERS AND THE LINE IS NOT STOPPPED DUE TO PENDING SCANNING THE EF PARTS WIHTIN TIME LIMIT
                                // THEN RESUME THE LINE 
                                //if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("IsCallToIoT")))
                                //{
                                Kepware kepwareObj = new Kepware();
                                kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                                //}
                                //else
                                //{
                                //    MM_Ctrl_LineStopEmergency lineCtrlStopObj = new MM_Ctrl_LineStopEmergency();
                                //    lineCtrlStopObj.Plant_ID = plantID;
                                //    lineCtrlStopObj.Shop_ID = shopID;
                                //    lineCtrlStopObj.Line_ID = lineID;
                                //    lineCtrlStopObj.Station_ID = stationID;
                                //    lineCtrlStopObj.Line_Stop = false;
                                //    lineCtrlStopObj.Emergency_Call = false;
                                //    lineCtrlStopObj.Heart_Bit = false;
                                //    lineCtrlStopObj.Update_Cntr = 0;
                                //    lineCtrlStopObj.Inserted_Date = DateTime.Now;
                                //    db.MM_Ctrl_LineStopEmergency.Add(lineCtrlStopObj);
                                //    db.SaveChanges();
                                //}
                            }
                        }

                        //CHECK IF RESUME TIME HAS TO BE SAVED BY OUR SYSTEM OR WE HAVE TO WAIT FOR PLC SIGNAL
                        //var checkCountObj = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Line_ID == lineID).ToList();
                        //if (checkCountObj.Count > 1)
                        //{
                        MM_History_LineStop OriginalLSData = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.Line_ID == lineID && a.Station_ID == stationID && a.EFPartGroup_ID == partgroupID).FirstOrDefault();
                        if (OriginalLSData != null)
                        {
                            UpdateModel(OriginalLSData);
                            OriginalLSData.Resume_Time = DateTime.Now;
                            OriginalLSData.Is_Edited = true;
                            db.Entry(OriginalLSData).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        //}
                    }
                    return Json("true", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    generalHelper.logLineStopData(plantID, shopID, lineID, stationID, "Error Proofing", "Serial No. " + childSerialNo + " Not found in Order List for PartGroup ID : " + partgroupID + " or already got fitted for some other Parent Part,ModelCode : " + modelCode + " !", partgroupID);
                    return Json("false", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                generalHelper.addShopControllerException(ex, "Manifest", "isErrorProofingOKNoPartNo(model : " + modelCode + ", Main SrNo: " + mainserialNo + ",Child SrNo: " + childSerialNo + ",partgroupID: " + partgroupID + ")", ((FDSession)this.Session["FDSession"]).stationId, ((FDSession)this.Session["FDSession"]).plantId, ((FDSession)this.Session["FDSession"]).shopId, ((FDSession)this.Session["FDSession"]).lineId, ((FDSession)this.Session["FDSession"]).userId);
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public bool IsKittingPartGroup(int partgroupID)
        {
            return db.MM_Partgroup.Any(p => p.Partgroup_ID == partgroupID && p.Is_Kitting == true);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult manualErrorProofingOKForPart(string mainserialNo, string childSerialNo, string partNumb)
        {
            ErrorProofing efObj = new ErrorProofing();
            mainserialNo = mainserialNo.Trim().ToUpper();
            childSerialNo = childSerialNo.Trim().ToUpper();
            int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
            String modelCode = efObj.getModelcodeBySerialNumber(mainserialNo,plantID);
            try
            {
                bool isOK = false;
                decimal shopID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                decimal lineID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                decimal stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

                partNumb = partNumb.Trim().ToUpper();
                decimal? partgroupID = db.MM_Partmaster.Where(a => a.Part_No == partNumb).Select(a => a.Partgroup_ID).FirstOrDefault();

                MM_Geneaology geneologyObj = new MM_Geneaology();
                geneologyObj.Plant_ID = plantID;
                geneologyObj.Shop_ID = shopID;
                geneologyObj.Line_ID = lineID;
                geneologyObj.Main_Model_Code = modelCode;
                geneologyObj.Main_Order_Serial_No = mainserialNo;
                geneologyObj.Child_Model_Code = partNumb;
                geneologyObj.Child_Order_Serial_No = "MANUAL OK";
                geneologyObj.Inserted_Date = DateTime.Now;
                geneologyObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                geneologyObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                geneologyObj.PartGroup_ID = partgroupID;
                db.MM_Geneaology.Add(geneologyObj);
                db.SaveChanges();

                //RESUME THE LINE IF THERE IS ALREADY LINSESTOP DUE TO SAME REASON AND THERE ARE NO ERROR PROOFING PARTS AGAINST THE MAIN SERIAL NO
                //bool isInLineStopHistory = db.MM_History_LineStop.Any(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.isLineStop == true);
                bool isPendingEFParts = isPendingErrorProofing(Convert.ToInt32(stationID), Convert.ToInt32(lineID), mainserialNo);
                //if (isInLineStopHistory && isPendingEFParts == false)
                //{
                if (isPendingEFParts == false)
                {
                    Kepware kepwareObj = new Kepware();
                    kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                    kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                }

                //var checkCountObj = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Line_ID == lineID).ToList();
                //if (checkCountObj.Count > 1)
                //{
                MM_History_LineStop OriginalLSData = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == "Pending ErrorProofing/Geneaology Scan" && a.Line_ID == lineID && a.Station_ID == stationID).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                if (OriginalLSData != null)
                {
                    DateTime dateTimeNow;
                    TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                    if (dateObj != null)
                    {
                        dateTimeNow = dateObj.todayDate;
                    }
                    else
                    {
                        dateTimeNow = DateTime.Now;
                    }
                    UpdateModel(OriginalLSData);
                    OriginalLSData.Resume_Time = dateTimeNow;
                    OriginalLSData.Is_Edited = true;
                    db.Entry(OriginalLSData).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                generalHelper.addShopControllerException(ex, "Manifest", "manualErrorProofingOKForPart(model : " + modelCode + ", Main SrNo: " + mainserialNo + ",Child SrNo: " + childSerialNo + ",Part Number: " + partNumb + ")", ((FDSession)this.Session["FDSession"]).stationId, ((FDSession)this.Session["FDSession"]).plantId, ((FDSession)this.Session["FDSession"]).shopId, ((FDSession)this.Session["FDSession"]).lineId, ((FDSession)this.Session["FDSession"]).userId);
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult manualErrorProofingOKForPartGroup(string mainserialNo, string childSerialNo, decimal partgroupID)
        {
            ErrorProofing efObj = new ErrorProofing();
            mainserialNo = mainserialNo.Trim().ToUpper();
            childSerialNo = childSerialNo.Trim().ToUpper();
            int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);

            String modelCode = efObj.getModelcodeBySerialNumber(mainserialNo,plantID);
            try
            {
                bool isOK = false;
                decimal shopID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                decimal lineID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                decimal stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

                MM_Geneaology geneologyObj = new MM_Geneaology();
                geneologyObj.Plant_ID = plantID;
                geneologyObj.Shop_ID = shopID;
                geneologyObj.Line_ID = lineID;
                geneologyObj.Main_Model_Code = modelCode;
                geneologyObj.Main_Order_Serial_No = mainserialNo;
                geneologyObj.Child_Model_Code = modelCode;
                geneologyObj.Child_Order_Serial_No = "MANUAL OK";
                geneologyObj.Inserted_Date = DateTime.Now;
                geneologyObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                geneologyObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                geneologyObj.PartGroup_ID = partgroupID;
                db.MM_Geneaology.Add(geneologyObj);
                db.SaveChanges();

                //RESUME THE LINE IF THERE IS ALREADY LINSESTOP DUE TO SAME REASON AND THERE ARE NO ERROR PROOFING PARTS AGAINST THE MAIN SERIAL NO
                //bool isInLineStopHistory = db.MM_History_LineStop.Any(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Error Proofing" && a.isLineStop == true);
                bool isPendingEFParts = isPendingErrorProofing(Convert.ToInt32(stationID), Convert.ToInt32(lineID), mainserialNo);
                //if (isInLineStopHistory && isPendingEFParts == false)
                //{
                if (isPendingEFParts == false)
                {
                    Kepware kepwareObj = new Kepware();
                    kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                    kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                }

                //var checkCountObj = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Line_ID == lineID).ToList();
                //if (checkCountObj.Count > 1)
                //{
                MM_History_LineStop OriginalLSData = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == "Pending ErrorProofing/Geneaology Scan" && a.Line_ID == lineID && a.Station_ID == stationID).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                if (OriginalLSData != null)
                {
                    DateTime dateTimeNow;
                    TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                    if (dateObj != null)
                    {
                        dateTimeNow = dateObj.todayDate;
                    }
                    else
                    {
                        dateTimeNow = DateTime.Now;
                    }
                    UpdateModel(OriginalLSData);
                    OriginalLSData.Resume_Time = dateTimeNow;
                    OriginalLSData.Is_Edited = true;
                    db.Entry(OriginalLSData).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json("true", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                generalHelper.addShopControllerException(ex, "Manifest", "manualErrorProofingOKForPartGroup(model : " + modelCode + ", Main SrNo: " + mainserialNo + ",Child SrNo: " + childSerialNo + ",partgroupID: " + partgroupID + ")", ((FDSession)this.Session["FDSession"]).stationId, ((FDSession)this.Session["FDSession"]).plantId, ((FDSession)this.Session["FDSession"]).shopId, ((FDSession)this.Session["FDSession"]).lineId, ((FDSession)this.Session["FDSession"]).userId);
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ManifestScreen()
        {
            return View("ManifestScreen");
        }
        public ActionResult FirstStationManifestScreen()
        {
            return View("FirstStationManifestScreen");
        }
        class JSONData
        {
            public bool status { get; set; }
            public string type { get; set; }
            public string message { get; set; }
            public int minutes { get; set; }
            public int seconds { get; set; }
            public int tactSeconds { get; set; }
            public Double currentProgress { get; set; }
            public DateTime TodayDate { get; set; }
        }

        [HttpPost]
        public ActionResult InsertSerialNo(string SerialNo)
        {
            SerialNo = SerialNo.Trim().ToUpper();
            JSONData jsondata = new JSONData();
            try
            {

                int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
                int shopID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                int stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                int lineID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                //nil
                if (CheckWhetherTakeout(SerialNo, stationID))
                {
                    jsondata.status = false;
                    jsondata.message = "Current serial No is Takeout !";
                    return Json(jsondata, JsonRequestBehavior.DenyGet);
                }
                //nil
                MM_Lines thisLineObj = db.MM_Lines.Find(lineID);

                var stationObj = db.MM_Route_Configurations.Where(a => a.Station_ID == stationID).FirstOrDefault();


                if (stationObj == null)
                {
                    jsondata.status = false;
                    jsondata.message = "No Route config found for station !";
                    return Json(jsondata, JsonRequestBehavior.DenyGet);
                }
                MM_Station_Tracking stationTrackObj = db.MM_Station_Tracking.Where(a => a.Station_ID == stationID).FirstOrDefault();
                string trackingSerial = stationTrackObj.Track_SerialNo.Trim().ToUpper();
                //CHECK IF IT IS NOT ORDERSTART STATION
                if (!(db.MM_Partgroup.Any(a => a.Line_ID == lineID) && stationObj.Is_Start_Station == true))
                {
                    //CHECK IF STATION IS START/FIRST STATION
                    //if (stationObj.Is_Start_Station.GetValueOrDefault(false) == true)
                    //{
                    if (String.IsNullOrWhiteSpace(trackingSerial) || thisLineObj.isPLC.GetValueOrDefault(false) == false || stationObj.Is_Start_Station.GetValueOrDefault(false) == false)
                    {

                        decimal nextStationID = stationObj.Next_Station_ID.GetValueOrDefault(0);

                        //----------------------------------------------------------------------------------------------

                        bool isValidSerialNo = isSerialNoValid(SerialNo, shopID);

                        if (isValidSerialNo || SerialNo == "EMPTYPITCH")
                        {
                            //CHECK IF THE SERIAL HAS ALREADY PASSED ITS NEXT STATION
                            bool isSerialNoPassed = (db.MM_OM_Order_Status.Any(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Serial_No == SerialNo) && stationObj.Is_Start_Station.GetValueOrDefault(false) == true);
                            if (isSerialNoPassed)
                            {
                                jsondata.status = false;
                                jsondata.message = "Serial No.-" + SerialNo + " already scanned on this Station!";
                                return Json(jsondata, JsonRequestBehavior.DenyGet);
                            }
                            else
                            {
                                bool emptyPitch = false;
                                if (SerialNo == "EMPTYPITCH")
                                {
                                    emptyPitch = true;
                                }
                                string sqlQry;
                                //IF STATION IS START STATION THEN ENTER DATA IN Track_SerialNo field in MM_STATION_TRACKING table
                                if (stationObj.Is_Start_Station.GetValueOrDefault(false) == true)
                                {
                                    //GET PREVIOUS LINE ID WHICH IS NOT A SUB ASSEMBLY AND PART OF THE MAIN PART PRODUCTION FLOW
                                    decimal previousLineID = (from a in db.MM_Route_Marriage_Station
                                                              join b in db.MM_Lines on a.Sub_Line_ID equals b.Line_ID
                                                              where a.Marriage_Station_ID == stationID && b.Is_Sub_Assembly != true
                                                              select a).Select(a => a.Sub_Line_ID).FirstOrDefault();
                                    //CHECK IF SERIAL NO IS PRESENT IN LINE COMPLETE BUFFER
                                    //bool isSerialInPreviousBuffer = db.MM_Line_Complete_Buffer.Any(a => a.Line_ID == previousLineID && a.isConsumed == false && a.SerialNo == SerialNo);
                                    //if (isSerialInPreviousBuffer == true)
                                    //{
                                    bool isThisLineConveyor = thisLineObj.Is_Conveyor.GetValueOrDefault(false);
                                    bool isThisLinePLC = thisLineObj.isPLC.GetValueOrDefault(false);
                                    if (isThisLineConveyor == true && isThisLinePLC == true)
                                    {
                                        //JUST ADD THE SERIAL INTO FIRST STATION AND LET THE TRACKING SERVICE DO THE TRACKING FROM CONVEYOR INDEX
                                        sqlQry = "UPDATE MM_STATION_TRACKING SET SerialNo = @p2,Track_SerialNo = @p2, isEmptyPitch = @p3" +
                                                 " WHERE Line_ID = @p0 AND Station_ID = @p1";
                                        db.Database.ExecuteSqlCommand(sqlQry, lineID, stationID, SerialNo, emptyPitch);

                                        //bool isInLineStopHistory = db.MM_History_LineStop.Any(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.Stop_Reason == "Serial Not Scanned" && a.isLineStop == true);
                                        //if (isInLineStopHistory)
                                        //{
                                        Kepware kepwareObj = new Kepware();
                                        kepwareObj.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                                        kepwareObj.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                                        //}
                                        string sqlQuery = "UPDATE MM_History_LineStop SET Resume_Time = @p1 WHERE Station_ID = @p0 AND isLineStop = 1 AND PLC_Ack = 1 AND Resume_Time IS NULL AND Stop_Reason = 'Serial Not Scanned' AND Line_ID = @p2";
                                        db.Database.ExecuteSqlCommand(sqlQuery, stationID, DateTime.Now, lineID);

                                        //UPDATE THE CONSUMED STATUS IN LINE_COMPLETE_BUFFER 
                                        sqlQry = "UPDATE MM_Line_Complete_Buffer SET isConsumed = 1 " +
                                                " WHERE Line_ID = @p0 AND SerialNo = @p1";
                                        db.Database.ExecuteSqlCommand(sqlQry, previousLineID, SerialNo);

                                        if (!String.IsNullOrWhiteSpace(SerialNo) && SerialNo != "EMPTYPITCH")
                                        {
                                            //INSERT INTO ORDER STATUS LOG ENTRY
                                            MM_OM_Order_Status orderStatusObj = new MM_OM_Order_Status();
                                            orderStatusObj.Plant_ID = plantID;
                                            orderStatusObj.Shop_ID = shopID;
                                            orderStatusObj.Line_ID = lineID;
                                            orderStatusObj.Station_ID = stationID;
                                            orderStatusObj.Serial_No = SerialNo;
                                            orderStatusObj.Entry_Date = DateTime.Now;
                                            orderStatusObj.Action_Status = "Move";
                                            orderStatusObj.Inserted_Date = DateTime.Now;
                                            orderStatusObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                            orderStatusObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                            orderStatusObj.Order_No = db.MM_OM_Order_List.Where(a => a.Serial_No == SerialNo).FirstOrDefault().Order_No;
                                            orderStatusObj.Is_Purgeable = true;
                                            db.MM_OM_Order_Status.Add(orderStatusObj);
                                            db.SaveChanges();
                                        }
                                        jsondata.status = true;
                                        jsondata.message = "Successfully Scanned SrNo: " + SerialNo + " ! ";
                                    }
                                    else
                                    {
                                        //MANUAL LOGICAL TRACKING
                                        moveOrdersForward(lineID, stationID, SerialNo);
                                        jsondata.status = true;
                                        jsondata.message = "Successfully Scanned SrNo: " + SerialNo + " ! ";
                                    }
                                    //}
                                    //else
                                    //{
                                    //    jsondata.status = false;
                                    //    jsondata.message = "Serial No. Invalid! Not present in previous Buffer .";
                                    //    return Json(jsondata, JsonRequestBehavior.DenyGet);
                                    //}
                                }
                                else
                                {//THIS BLOCK WILL RUN WHEN SCAN IS DONE IN STATION WHICH IS NOT START STATION

                                    sqlQry = "UPDATE MM_STATION_TRACKING SET SerialNo = @p2, isEmptyPitch = @p3" +
                                             " WHERE Line_ID = @p0 AND Station_ID = @p1";
                                    db.Database.ExecuteSqlCommand(sqlQry, lineID, stationID, SerialNo, emptyPitch);

                                    //LOG IF THERE IS ANY MISMATCH IN TRACKING AND ORIGINAL SERIAL NO
                                    if (!(trackingSerial.Equals(SerialNo, StringComparison.InvariantCultureIgnoreCase)))
                                    {
                                        MM_History_Mismatch mismatchObj = new MM_History_Mismatch();
                                        mismatchObj.Line_ID = lineID;
                                        mismatchObj.Station_ID = stationID;
                                        mismatchObj.Tracking_Serial = trackingSerial;
                                        mismatchObj.Scanned_Serial = SerialNo;
                                        mismatchObj.Inserted_Date = DateTime.Now;
                                        db.MM_History_Mismatch.Add(mismatchObj);
                                        db.SaveChanges();
                                    }
                                    jsondata.status = true;
                                    jsondata.message = "Successfully Scanned SrNo: " + SerialNo + " ! ";
                                }
                            }
                            //moveOrdersForward(lineID, stationID, SerialNo);
                        }
                        else
                        {
                            jsondata.status = false;
                            jsondata.message = "Serial No.-" + SerialNo + " is invalid !";
                            return Json(jsondata, JsonRequestBehavior.DenyGet);
                        }
                    }
                    else
                    {
                        if (trackingSerial != SerialNo)
                        {
                            jsondata.status = false;
                            jsondata.message = "Scan after current SrNo." + trackingSerial + " moves forward !";
                            return Json(jsondata, JsonRequestBehavior.DenyGet);
                        }
                        else
                        {
                            jsondata.status = true;
                        }
                    }
                    //} //CHECK IF START STATION IF ELSE END
                }
                else
                {
                    jsondata.status = false;
                    jsondata.message = "Cannot Scan on Order Start Station !";
                    return Json(jsondata, JsonRequestBehavior.DenyGet);
                }//CHECK IF ORDER START STATION IF ELSE END

                return Json(jsondata, JsonRequestBehavior.DenyGet);
            }
            catch (Exception exp)
            {
                generalHelper.addShopControllerException(exp, "Manifest", "InsertSerialNo(" + SerialNo + ")", ((FDSession)this.Session["FDSession"]).stationId, ((FDSession)this.Session["FDSession"]).plantId, ((FDSession)this.Session["FDSession"]).shopId, ((FDSession)this.Session["FDSession"]).lineId, ((FDSession)this.Session["FDSession"]).userId);
                jsondata.status = false;
                jsondata.message = "";
                return Json(jsondata, JsonRequestBehavior.DenyGet);
            }
        }

        private bool isSerialNoValid(string serialNo, int shopID)
        {
            try
            {
                if (serialNo.Equals("EMPTYPITCH", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                int plantId= Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
                ErrorProofing ep = new ErrorProofing();
                MM_OM_Order_List mmOrderObj = db.MM_OM_Order_List.Where(a => a.Serial_No == serialNo.Trim().ToUpper() && a.Order_Status == "Started" && a.Plant_ID==plantId).FirstOrDefault();

                if (mmOrderObj != null)
                {
                    int stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                    if (stationID == 77)
                    {
                        return true;
                    }
                    //CHECK IN PARTGROUP
                    decimal? partGroupID = mmOrderObj.Partgroup_ID;
                    bool isParentPart = db.MM_Partgroup.Any(a => a.Partgroup_ID == partGroupID && a.Order_Create == true && a.Shop_ID == shopID);
                    return isParentPart;
                }
                else
                {
                    return false;
                }
                //string partNo = ep.getChildModelcodeBySerialNumber(serialNo);
                //var parentPartList = (from a in db.MM_Partgroup
                //                      join b in db.MM_PartgroupItem on a.Partgroup_ID equals b.Partgroup_ID
                //                      where b.Part_No == partNo && a.Order_Create == true && a.Shop_ID == shopID && a.Serial_Config_ID != 0
                //                      select a).ToList();
                //bool isParentPart = parentPartList.Count() > 0;

            }
            catch (Exception exp)
            {
                General genObj = new General();
                genObj.addControllerException(exp, "Manifest", "isSerialNoValid(serialNo: " + serialNo + ",shopID: " + shopID + ")");
                return false;
            }
        }

        private void moveOrdersForward(int lineID, int stationID, string SerialNo)
        {
            Hashtable allStationData = new Hashtable();

            //Get all stations Except Buffer Station
            string sqlQry = "SELECT b.Station_ID AS Station_ID,ISNULL(b.Track_SerialNo,'') AS SerialNo FROM MM_Stations a JOIN MM_STATION_TRACKING b" +
                           " ON a.Station_ID = b.Station_ID" +
                           " WHERE (a.is_Buffer IS NULL OR a.Is_Buffer = 0) AND a.Line_ID = @p0 ";
            var stationDataObj = db.Database.SqlQuery<TrackingFields>(sqlQry, lineID).ToList();
            if (stationDataObj.Count > 0)
            {
                //Store the current Order Position Data into a Hashtable
                foreach (var item in stationDataObj)
                {
                    int currStationID = Decimal.ToInt32(item.Station_ID);

                    Hashtable stationData = new Hashtable();
                    stationData = getLinkedStations(currStationID);
                    allStationData.Add(currStationID, stationData);
                }
            }

            //Move all Orders One Step Ahead
            IDictionaryEnumerator ide = allStationData.GetEnumerator();
            while (ide.MoveNext())
            {
                int currStationID = Convert.ToInt32(ide.Key.ToString());
                Hashtable stationData = (Hashtable)ide.Value;
                int currPlantID = Convert.ToInt32(stationData["PlantID"].ToString());
                int currShopID = Convert.ToInt32(stationData["ShopID"].ToString());
                int nextStationID = Convert.ToInt32(stationData["Next"].ToString());
                string serialNo = stationData["SRNO"].ToString();
                Boolean isLastStation = Convert.ToBoolean(stationData["IsEndStation"].ToString());
                Boolean isStartStation = Convert.ToBoolean(stationData["IsStartStation"].ToString());
                Boolean isEmptyPitch = Convert.ToBoolean(stationData["isEmptyPitch"].ToString());

                if (nextStationID == 0 && isLastStation)
                {
                    //This block will get executed when the station is the last station of the Line

                    //If this serialno is taken out then don't move it forward.
                    if (!isSerialTakeOut(serialNo) && (serialNo.Trim().ToUpper()) != "EMPTYPITCH")
                    {
                        nextStationID = getNextLineStationID(lineID, currStationID);


                        if (!String.IsNullOrWhiteSpace(serialNo) && (serialNo.Trim().ToUpper()) != "EMPTYPITCH")
                        {
                            //Logical Buffer Table (MM_LINE_COMPLETE_BUFFER)
                            sqlQry = "INSERT INTO MM_LINE_COMPLETE_BUFFER (Plant_ID,Shop_ID,Line_ID,Station_ID,SerialNo) " +
                                     " VALUES (@p0,@p1,@p2,@p3,@p4)";
                            db.Database.ExecuteSqlCommand(sqlQry, currPlantID, currShopID, lineID, nextStationID, serialNo.Trim().ToUpper());
                        }

                        if (!String.IsNullOrWhiteSpace(serialNo) && (serialNo.Trim().ToUpper()) != "EMPTYPITCH")
                        {
                            //UPDATE ORDER STATUS LOG ENTRY
                            MM_OM_Order_Status orderStatusObj = new MM_OM_Order_Status();
                            orderStatusObj.Plant_ID = currPlantID;
                            orderStatusObj.Shop_ID = currShopID;
                            orderStatusObj.Line_ID = lineID;
                            orderStatusObj.Station_ID = currStationID;
                            orderStatusObj.Serial_No = serialNo.Trim().ToUpper();
                            orderStatusObj.Entry_Date = DateTime.Now;
                            orderStatusObj.Action_Status = "Move";
                            orderStatusObj.Inserted_Date = DateTime.Now;
                            orderStatusObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            orderStatusObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            orderStatusObj.Order_No = db.MM_OM_Order_List.Where(a => a.Serial_No == serialNo).FirstOrDefault().Order_No;
                            orderStatusObj.Is_Purgeable = true;
                            db.MM_OM_Order_Status.Add(orderStatusObj);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        //move take out body to rework area if necessary
                    }
                }
                else // THIS BLOCK RUNS WHEN THE STATION IS NOT THE LAST STATION
                {
                    if (isStartStation)
                    {
                        bool isPLC = db.MM_Lines.Find(lineID).isPLC.GetValueOrDefault(false);
                        if (SerialNo.Trim().ToUpper() == "EMPTYPITCH")
                        {
                            sqlQry = "UPDATE MM_STATION_TRACKING SET SerialNo = @p2, Track_SerialNo = @p2, isEmptyPitch = 1" +
                                " WHERE Line_ID = @p0 AND Station_ID = @p1";
                        }
                        else
                        {
                            sqlQry = "UPDATE MM_STATION_TRACKING SET SerialNo = @p2, Track_SerialNo = @p2, isEmptyPitch = 0" +
                                " WHERE Line_ID = @p0 AND Station_ID = @p1";
                        }
                        db.Database.ExecuteSqlCommand(sqlQry, lineID, stationID, SerialNo.Trim().ToUpper());

                        //IF IT IS NOT A PLC LINE (i.e we dont get conveyor index from this line) THEN UPDATE THE MOVE TIME MANUALLY
                        if (isPLC == false)
                        {
                            DateTime dateTimeNow;
                            sqlQry = "SELECT GETDATE() AS todayDate";
                            TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>(sqlQry).FirstOrDefault();
                            if (dateObj != null)
                            {
                                dateTimeNow = dateObj.todayDate;
                            }
                            else
                            {
                                dateTimeNow = DateTime.Now;
                            }

                            sqlQry = "Update MM_Lines SET LineMove_Time = @p1,Current_Stoppage_Seconds = 0 WHERE Line_ID = @p0";
                            db.Database.ExecuteSqlCommand(sqlQry, lineID, dateTimeNow);
                        }
                    }

                    bool checkValue = db.MM_Station_Tracking.Any(a => a.Station_ID == nextStationID);
                    if (checkValue)
                    {
                        sqlQry = "UPDATE MM_STATION_TRACKING SET Track_SerialNo = @p0,isEmptyPitch = @p3" +
                                 " WHERE Line_ID = @p1 AND Station_ID = @p2";
                        db.Database.ExecuteSqlCommand(sqlQry, serialNo, lineID, nextStationID, isEmptyPitch);
                    }
                    else
                    {
                        sqlQry = "INSERT INTO MM_STATION_TRACKING (Plant_ID,Shop_ID,Line_ID,Station_ID,Track_SerialNo) " +
                                 " VALUES (@p0,@p1,@p2,@p3,@p4,@p5)";
                        db.Database.ExecuteSqlCommand(sqlQry, currPlantID, currShopID, lineID, nextStationID, serialNo, isEmptyPitch);
                    }


                    if ((!String.IsNullOrWhiteSpace(serialNo)) && serialNo.Trim().ToUpper() != "EMPTYPITCH")
                    {
                        if (updateTakeInComplete(serialNo, nextStationID))
                        {
                            //UPDATE ORDER STATUS LOG ENTRY
                            MM_OM_Order_Status orderStatusObj = new MM_OM_Order_Status();
                            orderStatusObj.Plant_ID = currPlantID;
                            orderStatusObj.Shop_ID = currShopID;
                            orderStatusObj.Line_ID = lineID;
                            orderStatusObj.Station_ID = currStationID;
                            orderStatusObj.Serial_No = serialNo;
                            orderStatusObj.Entry_Date = DateTime.Now;
                            orderStatusObj.Action_Status = "Take In";
                            orderStatusObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            orderStatusObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            orderStatusObj.Inserted_Date = DateTime.Now;
                            orderStatusObj.Order_No = db.MM_OM_Order_List.Where(a => a.Serial_No == serialNo).FirstOrDefault().Order_No;
                            orderStatusObj.Is_Purgeable = true;
                            db.MM_OM_Order_Status.Add(orderStatusObj);
                            db.SaveChanges();
                        }
                        else
                        {
                            //UPDATE ORDER STATUS LOG ENTRY
                            MM_OM_Order_Status orderStatusObj = new MM_OM_Order_Status();
                            orderStatusObj.Plant_ID = currPlantID;
                            orderStatusObj.Shop_ID = currShopID;
                            orderStatusObj.Line_ID = lineID;
                            orderStatusObj.Station_ID = currStationID;
                            orderStatusObj.Serial_No = serialNo;
                            orderStatusObj.Entry_Date = DateTime.Now;
                            orderStatusObj.Action_Status = "Move";
                            orderStatusObj.Inserted_Date = DateTime.Now;
                            orderStatusObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            orderStatusObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            orderStatusObj.Order_No = db.MM_OM_Order_List.Where(a => a.Serial_No == serialNo).FirstOrDefault().Order_No;
                            orderStatusObj.Is_Purgeable = true;
                            db.MM_OM_Order_Status.Add(orderStatusObj);
                            db.SaveChanges();
                        }
                    }
                }
            }//WHILE END (allStationData Hashtable parsing)
        }

        public bool updateTakeInComplete(string SerialNo, decimal nextStationID)
        {
            bool isNextTakeInStation = db.MM_Quality_Take_In_Take_Out.Any(a => a.Serial_No == SerialNo && a.Take_In_Station_ID == nextStationID);
            if (isNextTakeInStation)
            {
                string sqlQry = "UPDATE MM_Quality_Take_In_Take_Out SET Rework_Status = 'Closed'" +
                               " WHERE Serial_No = @p0 AND Take_In_Station_ID = @p1 AND Rework_Status != 'Closed'";
                db.Database.ExecuteSqlCommand(sqlQry, SerialNo, nextStationID);
            }
            return isNextTakeInStation;
        }

        public Hashtable getLinkedStations(int stationID)
        {
            try
            {
                string sqlQry = "SELECT TOP 1 ISNULL(a.Next_Station_ID,0) AS Next_Station_ID,ISNULL(a.Prev_Station_ID,0) AS Prev_Station_ID," +
                               " ISNULL(b.Track_SerialNo,'') AS SerialNo,ISNULL(a.Is_End_Station,0) AS Is_End_Station,ISNULL(a.Is_Start_Station,0) AS Is_Start_Station,b.Plant_ID,b.Shop_ID,b.isEmptyPitch FROM MM_Route_Configurations a JOIN MM_STATION_TRACKING b" +
                               " ON a.Station_ID = b.Station_ID WHERE a.Station_ID = @p0";
                var dataObj = db.Database.SqlQuery<TrackingFields>(sqlQry, stationID).ToList();
                if (dataObj.Count > 0)
                {
                    foreach (var item in dataObj)
                    {
                        Hashtable stationData = new Hashtable();
                        stationData.Add("Next", Convert.ToInt32(item.Next_Station_ID.ToString()));
                        stationData.Add("Prev", Convert.ToInt32(item.Prev_Station_ID.ToString()));
                        stationData.Add("SRNO", item.SerialNo.ToString());
                        stationData.Add("IsEndStation", Convert.ToBoolean(item.Is_End_Station.ToString()));
                        stationData.Add("IsStartStation", Convert.ToBoolean(item.Is_Start_Station.ToString()));
                        stationData.Add("PlantID", Convert.ToInt32(item.Plant_ID.ToString()));
                        stationData.Add("ShopID", Convert.ToInt32(item.Shop_ID.ToString()));
                        stationData.Add("isEmptyPitch", Convert.ToBoolean(item.isEmptyPitch.ToString()));
                        return stationData;
                    }
                }
            }
            catch (Exception exp)
            {

                General genObj = new General();
                genObj.addControllerException(exp, "Tracking Service", "HelperLibrary.getLinkedStations(station: " + stationID + ")");
            }
            return null;
        }

        private int getNextLineStationID(int lineID, int stationID)
        {
            try
            {
                string sqlQry = "SELECT TOP 1 Marriage_Station_ID FROM MM_Route_Marriage_Station WHERE " +
                                " Sub_Line_ID = @p0 AND Sub_Line_Station_ID = @p1";

                var dataObj = db.Database.SqlQuery<TrackingFields>(sqlQry, lineID, stationID).ToList();
                if (dataObj.Count > 0)
                {
                    foreach (var item in dataObj)
                    {
                        return Convert.ToInt32(item.Marriage_Station_ID.ToString());
                    }
                }
            }
            catch (Exception exp)
            {

                General genObj = new General();
                genObj.addControllerException(exp, "Manifest", "getNextLineStationID()");
            }
            return 0;
        }//END getNextLineStationID() method  -----

        public Boolean isSerialTakeOut(string SerialNo)
        {
            return db.MM_Quality_Take_In_Take_Out.Any(a => a.Serial_No == SerialNo && a.Take_Out_Station_ID != null && a.Take_In_Station_ID == null);
        }

        private bool isPendingErrorProofing(int stationID, int lineID, string serialNo)
        {
            try
            {
                if (db.MM_ErrorProofing_LineStop.Any(a => a.Ef_Line_Stop_Enabled == true && a.Station_ID == stationID && a.Line_ID == lineID))
                {
                    string modelcode = db.MM_OM_Order_List.Where(a => a.Serial_No == serialNo).FirstOrDefault().partno;

                    MM_Partmaster[] mmPartMasterObj = (from mmPartMaster in db.MM_Partmaster
                                                       where (mmPartMaster.Genealogy == true || mmPartMaster.Error_Proofing == true)
                                                       && mmPartMaster.Station_ID == stationID
                                                       && (from mmBomItem in db.MM_BOM_Item where mmBomItem.Model_Code == modelcode select mmBomItem.Part_No)
                                                       .Contains(mmPartMaster.Part_No)
                                                       select mmPartMaster).ToArray();
                    string configID = db.MM_Model_Master.Where(a => a.Model_Code == modelcode).Select(a => a.OMconfig_ID).FirstOrDefault();

                    int totalPartGroupQty = (from a in db.MM_Partgroup
                                             join b in db.MM_OM_Configuration on a.Partgroup_ID equals b.Partgroup_ID
                                             where a.Consumption_Station_ID == stationID && b.OMconfig_ID == configID
                                             && a.Partgroup_ID != null && b.Partgroup_ID != null
                                             select a).AsEnumerable().Sum(a => a.Qty.GetValueOrDefault(0));

                    int totalParts = mmPartMasterObj.Count() + totalPartGroupQty;

                    int scannedParts = db.MM_Geneaology.Where(a => a.Main_Order_Serial_No == serialNo && a.Station_ID == stationID).ToList().Count();
                    if (scannedParts >= totalParts)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                General genObj = new General();
                genObj.addControllerException(exp, "Manifest", "isPendingErrorProofing(stationID: " + stationID + ", lineID: " + lineID + ", serialNo: " + serialNo + ") ");
                return false;
            }
        }

        private IEnumerable getImageNameFromSerialNo(string serialNo, int stationID)
        {
            List<MM_Manifest> partImagesObj = new List<MM_Manifest>();
            string partNo;
            var getModelfromSerialObj = db.MM_OM_Order_List
                                          .Where(a => a.Serial_No == serialNo)
                                          .Select(a => new { a.partno }).FirstOrDefault();

            if (getModelfromSerialObj != null)
            {
                partNo = getModelfromSerialObj.partno;
            }
            else
            {
                partNo = "";
            }
            List<MM_Manifest> manifestObjList = db.MM_Manifest.Where(a => a.Part_No == partNo && a.Station_ID == stationID).ToList();
            if (manifestObjList != null)
            {
                partImagesObj.AddRange(manifestObjList);
            }
            partImagesObj.AddRange((from a in db.MM_Manifest
                                    join b in db.MM_BOM_Item on a.Part_No equals b.Part_No
                                    where b.Model_Code == partNo && a.Station_ID == stationID && a.Image_Name != null
                                    select a).AsEnumerable().Select(a => new MM_Manifest
                                    {
                                        Part_No = a.Part_No,
                                        Image_Name = a.Image_Name,
                                        Is_ParentModel_Manifest = a.Is_ParentModel_Manifest
                                    }).Distinct().ToList());

            return partImagesObj;
        }

        // GET: Manifest/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Manifest mM_Manifest = db.MM_Manifest.Find(id);
            if (mM_Manifest == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceGlobal.Delete + " " + ResourceModules.Manifest;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Manifest";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Manifest;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Manifest;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;

            ViewBag.imgurl = setimg.getimageFromService(mM_Manifest.Image_Name, "MANIFEST");

            return View(mM_Manifest);
        }

        // POST: Manifest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_Manifest mM_Manifest = db.MM_Manifest.Find(id);
            try
            {

                db.MM_Manifest.Remove(mM_Manifest);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceGlobal.Delete + " " + ResourceModules.Manifest;
                globalData.messageDetail = ResourceModules.Manifest + " " + ResourceMessages.Delete_Success;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
            }
            catch (DbUpdateException exp)
            {
                globalData.pageTitle = ResourceModules.Manifest; ;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "Manifest";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Manifest;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Manifest;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Manifest;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.Manifest + " " + ResourceGlobal.Delete;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return View(mM_Manifest);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "ManifestController", "DeleteConfirmed(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.pageTitle = ResourceModules.Manifest;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "Manifest";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Manifest;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Manifest;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Manifest;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.Manifest + " " + ResourceGlobal.Delete;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return View(mM_Manifest);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public void DeleteFile(string filename)
        {
            string fullPath = Request.MapPath("~/Content/images/" + filename);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            List<MM_Manifest> mM_Manifest = db.MM_Manifest.Where(a => a.Image_Name == filename).ToList();
            db.MM_Manifest.RemoveRange(mM_Manifest);
            db.SaveChanges();
        }

        class ManifestEditImagesFields
        {
            public string Name { get; set; }
            public long Size { get; set; }
            public string Base64Img { get; set; }
        }

        public ActionResult getManifestImages(decimal manifestID)
        {
            var ManifestObj = db.MM_Manifest.Find(manifestID);
            string partNo = ManifestObj.Part_No;

            List<String> manifestImages = db.MM_Manifest.Where(a => a.Part_No == partNo && a.Station_ID == ManifestObj.Station_ID).Select(a => a.Image_Name).Distinct().ToList();

            List<ManifestEditImagesFields> imageDataList = new List<ManifestEditImagesFields>();

            foreach (var manifestImage in manifestImages)
            {
                ManifestEditImagesFields imageObj = new ManifestEditImagesFields();
                imageObj.Name = manifestImage;

                var imgSrc = setimg.getimageFromService(manifestImage, "MANIFEST");


                //  FileInfo image = new FileInfo(HttpContext.Request.MapPath("~/Content/images/" + manifestImage));
                long imageSize = imgSrc.Length;
                imageObj.Size = imageSize;
                imageObj.Name = manifestImage;
                imageObj.Base64Img = imgSrc;
                imageDataList.Add(imageObj);
            }

            return new JsonResult()
            {
                Data = imageDataList,
                ContentType = "application/json",
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getCurrentShopDetails(decimal lineId, decimal stationId, string SerialNo)
        {
            StationCurrentData stationDataObj = new StationCurrentData();
            if (String.IsNullOrEmpty(SerialNo))
            {
                stationDataObj = (from a in db.MM_Station_Tracking
                                  join b in db.MM_OM_Order_List on a.SerialNo equals b.Serial_No into ac
                                  from b in ac.DefaultIfEmpty()
                                  join c in db.MM_Model_Master on b.partno equals c.Model_Code into bc
                                  from c in bc.DefaultIfEmpty()
                                  where a.Station_ID == stationId
                                  select new StationCurrentData
                                  {
                                      serialNo = a.SerialNo,
                                      modelSeries = c.MM_Series.Series_Description,
                                      modelFamily = c.MM_Attribution_Parameters.Attribute_Desc,
                                      modelCode = b.partno
                                  }).Distinct().FirstOrDefault();
            }
            else if (SerialNo.Trim().ToUpper() != "EMPTYPITCH")
            {
                stationDataObj = (from b in db.MM_OM_Order_List
                                  join c in db.MM_Model_Master on b.partno equals c.Model_Code into bc
                                  from c in bc.DefaultIfEmpty()
                                  where b.Serial_No == SerialNo
                                  select new StationCurrentData
                                  {
                                      modelSeries = c.MM_Series.Series_Description,
                                      modelFamily = c.MM_Attribution_Parameters.Attribute_Desc,
                                      modelCode = b.partno
                                  }).Distinct().FirstOrDefault();
            }

            if (!String.IsNullOrWhiteSpace(SerialNo) &&( SerialNo.Trim().ToUpper() != "EMPTYPITCH") && stationDataObj!=null)
            {
                stationDataObj.serialNo = SerialNo;
            }
            else if (SerialNo.Trim().ToUpper() == "EMPTYPITCH")
            {
                stationDataObj.serialNo = SerialNo.Trim().ToUpper();
            }

           // stationDataObj.isStartStation = db.MM_Route_Configurations.Where(a => a.Station_ID == stationId).Select(a => a.Is_Start_Station).FirstOrDefault();
            //if (db.MM_Quality_Take_In_Take_Out.Any(a => a.Serial_No == stationDataObj.serialNo && a.Take_Out_Station_ID != null && a.Take_In_Station_ID == null))
            //{
            //    stationDataObj.TakeInTakeOut = "Take Out";
            //}
            //else if (db.MM_Quality_Take_In_Take_Out.Any(a => a.Serial_No == stationDataObj.serialNo && a.Take_Out_Station_ID != null && a.Take_In_Station_ID != null && a.Rework_Status != "Closed"))
            //{
            //    stationDataObj.TakeInTakeOut = "Take In";
            //}
            //if (!String.IsNullOrWhiteSpace(stationDataObj.serialNo) && (stationDataObj.serialNo.Trim().ToUpper()) != "EMPTYPITCH")
            //{
            //    IEnumerable<MM_OM_Order_List> nextTwoSerialNo = db.MM_OM_Order_List.Where(a => a.Inserted_Date > (db.MM_OM_Order_List.Where(c => c.Serial_No == stationDataObj.serialNo).Select(d => d.Inserted_Date).FirstOrDefault()))
            //                                                      .OrderBy(a => a.Inserted_Date).Take(2).ToList();
            //    decimal? prevStationId1 = (db.MM_Route_Configurations.Where(a => a.Next_Station_ID == stationId).Select(a => a.Station_ID).FirstOrDefault());
            //    decimal? prevStationId2 = (db.MM_Route_Configurations.Where(a => a.Next_Station_ID == prevStationId1).Select(a => a.Station_ID).FirstOrDefault());
            //    string nextSerial1 = "";
            //    string nextSerial2 = "";
            //    if (prevStationId1 != null)
            //    {
            //        nextSerial1 = db.MM_Station_Tracking.Where(b => b.Station_ID == prevStationId1).Select(b => b.Track_SerialNo).FirstOrDefault();
            //    }
            //    if (prevStationId2 != null)
            //    {
            //        nextSerial2 = db.MM_Station_Tracking.Where(b => b.Station_ID == prevStationId2).Select(b => b.Track_SerialNo).FirstOrDefault();
            //    }
            //    stationDataObj.nextSerial1 = nextSerial1;
            //    stationDataObj.nextSerial2 = nextSerial2;
            //}
            return Json(stationDataObj, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult refreshManifest(string SerialNo)
        {
            SerialNo = SerialNo.Trim().ToUpper();
            int userID = ((FDSession)this.Session["FDSession"]).userId;
            int stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            ViewBag.StationID = stationID;

            bool? stationIsBuffer = db.MM_Stations.Find(stationID).Is_Buffer;

            if (stationIsBuffer.GetValueOrDefault(false) == false)
            {
                if (String.IsNullOrEmpty(SerialNo))
                {
                    var geSerialNoObj = db.MM_Station_Tracking
                                          .Where(a => a.Station_ID == stationID)
                                          .Select(a => new { a.SerialNo, a.Track_SerialNo }).Distinct().FirstOrDefault();

                    if (String.IsNullOrWhiteSpace(geSerialNoObj.SerialNo))
                    {
                        SerialNo = geSerialNoObj.Track_SerialNo;
                    }
                    else
                    {
                        SerialNo = geSerialNoObj.SerialNo;
                    }
                }
                if (!String.IsNullOrWhiteSpace(SerialNo) && SerialNo.Trim().ToUpper() != "EMPTYPITCH")
                {
                    ViewBag.PartsImage = getImageNameFromSerialNo(SerialNo, stationID);
                    ErrorProofing efObj = new ErrorProofing();
                    int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
                    String mCode = efObj.getModelcodeBySerialNumber(SerialNo, plantID);
                    MM_Partmaster[] partObj = efObj.getStationPartListForStation(stationID, mCode, SerialNo);
                    MM_Partgroup[] partGroups = getNoPartPartGroup(stationID, SerialNo);
                    ViewBag.EFGeneologyParts = partObj;
                    ViewBag.EFpartGroups = partGroups;
                }
                return PartialView("PVManifest");
            }
            else
            {
                return PartialView("PVManifest");
            }
        }

        public JsonResult isLineStopByLine(decimal lineId)
        {
            return Json(db.MM_Lines.Any(a => a.Line_ID == lineId && a.isLineStop == true), JsonRequestBehavior.AllowGet);
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public JsonResult showMESLineStopNotification()
        {
            string stopReason = "";
            string reasonIdentifier = "";
            try
            {
                decimal lineID = ((FDSession)this.Session["FDSession"]).lineId;
                decimal stationID = ((FDSession)this.Session["FDSession"]).stationId;

                bool isLineStop = db.MM_Lines.Any(a => a.Line_ID == lineID && a.isLineStop == true);
                if (isLineStop)
                {
                    MM_History_LineStop mesLineStopObj = db.MM_History_LineStop.Where(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Resume_Time == null && a.isLineStop == true && (a.Stop_Reason == "Serial Not Scanned" || a.Stop_Reason == "Pending ErrorProofing or Geneaology Scan")).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                    if (mesLineStopObj != null)
                    {
                        stopReason = mesLineStopObj.Remarks.Trim();
                        reasonIdentifier = mesLineStopObj.Stop_Reason.Trim();
                    }
                }

                return Json(new { reasonIdentifier = reasonIdentifier, reasonText = stopReason }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "ManifestController", "showMESLineStopNotification()", ((FDSession)this.Session["FDSession"]).userId);
                return Json(new { reasonIdentifier = reasonIdentifier, reasonText = stopReason }, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public JsonResult getCurrentTactData()
        {
            JSONData jsondata = new JSONData();
            decimal stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            decimal lineID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);

            MM_Lines lineObj = db.MM_Lines.Where(a => a.Line_ID == lineID).FirstOrDefault();

          //  DateTime lineStartTime = Convert.ToDateTime(lineObj.LineStart_Time);
            //DateTime lineStopTime = Convert.ToDateTime(lineObj.LineStop_Time);
            //DateTime lastMoveTime = Convert.ToDateTime(lineObj.LineMove_Time);
            //Double lineTactSeconds = Convert.ToDouble(lineObj.Tact_Time_Sec);
            //Int64 totalStoppageSeconds = lineObj.Current_Stoppage_Seconds.GetValueOrDefault(0);
            DateTime dateTimeNow;

            //if (lineObj.Is_Conveyor.GetValueOrDefault(false) && lineObj.isPLC.GetValueOrDefault(false))
            //{
            //    string sqlQry = "SELECT GETDATE() AS TodayDate";
            //    JSONData dateObj = db.Database.SqlQuery<JSONData>(sqlQry).FirstOrDefault();
            //    if (dateObj != null)
            //    {
            //        dateTimeNow = dateObj.TodayDate;
            //    }
            //    else
            //    {
            //        dateTimeNow = DateTime.Now;
            //    }

            //    //Double currentPassedSeconds = Math.Abs((dateTimeNow - lastMoveTime).TotalSeconds);
            //    //if (lineObj.isLineStop == true && lineStopTime > lastMoveTime)
            //    //{
            //    //    //currentPassedSeconds = Math.Abs((lineStopTime - lastMoveTime).TotalSeconds);
            //    //    currentPassedSeconds = Math.Abs((Math.Abs((lineStopTime - lastMoveTime).TotalSeconds)) - totalStoppageSeconds);
            //    //}
            //    //else
            //    //{
            //    //    currentPassedSeconds = Math.Abs((Math.Abs((dateTimeNow - lastMoveTime).TotalSeconds)) - totalStoppageSeconds);
            //    //}

            //    double currentPassedSeconds = Math.Abs((dateTimeNow - lastMoveTime).TotalSeconds);

            //    //if (lineStartTime < lineStopTime && lineStopTime > lastMoveTime)
            //    //{
            //    //    currentPassedSeconds = Math.Abs((lineStopTime - lastMoveTime).TotalSeconds);
            //    //}
            //    currentPassedSeconds = currentPassedSeconds - totalStoppageSeconds;

            //    //double secondsBetweenStop_Start = 0;
            //    //CHECK IF LINE HAS STOPED IN BETWEEN TACT AND LAST MOVE TIME(LineStartTime will get updated when line resumes after line stop)
            //    //if (lineStopTime >= lastMoveTime && lineStopTime <= lastMoveTime.AddSeconds(lineTactSeconds) && lineStartTime >= lineStopTime)
            //    //{
            //    //    secondsBetweenStop_Start = Math.Abs((lineStartTime - lineStopTime).TotalSeconds);
            //    //    currentPassedSeconds = Math.Abs(currentPassedSeconds - secondsBetweenStop_Start);
            //    //}
            //    if (currentPassedSeconds > lineTactSeconds)
            //    {
            //        currentPassedSeconds = lineTactSeconds;
            //    }
            //    jsondata.currentProgress = Math.Round((currentPassedSeconds / lineTactSeconds) * 100);
            //    jsondata.minutes = Convert.ToInt32(Math.Floor(Math.Abs((lineTactSeconds - currentPassedSeconds) / 60)));
            //    jsondata.seconds = Convert.ToInt32(Math.Abs((lineTactSeconds - currentPassedSeconds) % 60));
            //    jsondata.tactSeconds = Convert.ToInt32(lineTactSeconds);
            //}
            //else
            //{
            //    TimeSpan ts = TimeSpan.FromSeconds(lineTactSeconds);
            //    jsondata.currentProgress = 0;
            //    jsondata.minutes = ts.Minutes;
            //    jsondata.seconds = ts.Seconds;
            //    jsondata.tactSeconds = Convert.ToInt16(lineTactSeconds);
            //}

            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //nil
        public bool CheckWhetherTakeout(string SerialNo, int stationId)
        {
            try
            {
                IQueryable<MM_Quality_Take_In_Take_Out> result1;

                result1 = db.MM_Quality_Take_In_Take_Out.Where(S => S.Station_ID == stationId && S.Serial_No == SerialNo && S.Take_In_Date == null
                                   );


                if (result1.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //nil
    }
}
