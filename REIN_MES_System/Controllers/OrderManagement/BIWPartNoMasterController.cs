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
using System.IO;
using Newtonsoft.Json;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class BIWPartNoMasterController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        General generalObj = new General();
        int plantId = 0;
        // GET: BIWPartNoMaster
        public ActionResult Index()
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                globalData.pageTitle = ResourceModules.BIW_Part_Master_Config;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "BIWPartNoMasterController";
                globalData.actionName = ResourceGlobal.Lists;
                globalData.contentTitle = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;
                return View(db.RS_BIW_Part_Master.Where(m => m.Plant_ID == plantId).ToList());
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
            
        }

        // GET: BIWPartNoMaster/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_BIW_Part_Master RS_BIW_Part_Master = db.RS_BIW_Part_Master.Find(id);
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.BIW_Part_Master_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "BIWPartNoMasterController";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            if (RS_BIW_Part_Master == null)
            {
                return HttpNotFound();
            }
            return View(RS_BIW_Part_Master);
        }

        // GET: BIWPartNoMaster/Create
        public ActionResult Create()
        {
            try
            {
                globalData.pageTitle = ResourceModules.BIW_Part_Master_Config;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "BIWPartNoMasterController";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;

                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(m=> m.Shop_ID == 0), "Platform_ID", "Platform_Name");
                ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");
                //ViewBag.Model_Attribute_ID = new SelectList(db.RS_Model_Attribute_Master.Where(m=>m.Sub_Assembly_ID == 0 && m.Platform_ID == 0), "Model_Attribute_ID", "Attribution");
                //ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration.Where(m=> m.Platform_ID == 0), "Config_ID", "Display_Name");
                //ViewBag.OM_Config_ID = new SelectList(db.RS_OM_Configuration.Where(m => m.Plant_ID == plantId), "OMconfig_ID", "OMconfig_Desc");
                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
               // return View();
            }
            catch(Exception Ex)
            {
                //if (Ex.Message == "Object reference not set to an instance of an object.")
                return RedirectToAction("Index", "User");
            }
            return View();
        }

        // POST: BIWPartNoMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(RS_BIW_Part_Master RS_BIW_Part_Master, HttpPostedFileBase upload)
        //{
        //    try {
        //        plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //        if (ModelState.IsValid)
        //        {
        //            if (upload != null && upload.ContentLength > 0)
        //            {
        //                var extension = Path.GetExtension(upload.FileName);
        //                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
        //                {
        //                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
        //                    {
        //                        RS_BIW_Part_Master.Image_Name = System.IO.Path.GetFileName(upload.FileName);
        //                        RS_BIW_Part_Master.Image_Type = Path.GetExtension(upload.FileName);
        //                        RS_BIW_Part_Master.Content_Type = upload.ContentType;
        //                        RS_BIW_Part_Master.Image_Content = reader.ReadBytes(upload.ContentLength);
        //                    }
        //                }
        //                else
        //                {
        //                    globalData.isErrorMessage = true;
        //                    globalData.messageTitle = ResourceModules.BIW_Part_Master_Config;
        //                    globalData.messageDetail = "Image file format is uploaded";
        //                    TempData["globalData"] = globalData;
        //                    ViewBag.GlobalDataModel = globalData;
        //                    ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
        //                    ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
        //                    ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name");
        //                    ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");
        //                    ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
        //                    ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
        //                    return View(RS_BIW_Part_Master);
        //                }
        //            }
        //            bool isvalid = true;
        //            if (RS_BIW_Part_Master.isPartExists(RS_BIW_Part_Master.Variant_Code, plantId, 0))
        //            {
        //                ModelState.AddModelError("Variant_Code", ResourceValidation.Exist);
        //                isvalid = false;
        //            }
        //            if (isvalid == true)
        //            {
        //                RS_BIW_Part_Master.Inserted_Date = DateTime.Now;
        //                RS_BIW_Part_Master.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
        //                RS_BIW_Part_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                //RS_BIW_Part_Master.VARIANT_TYPE.Trim();
        //                RS_BIW_Part_Master.Variant_Code.Trim();
        //                RS_BIW_Part_Master.VARIANT_DESC.Trim();
        //                RS_BIW_Part_Master.LONG_DESC.Trim();
        //                db.RS_BIW_Part_Master.Add(RS_BIW_Part_Master);
        //                db.SaveChanges();

        //                globalData.isSuccessMessage = true;
        //                globalData.messageTitle = ResourceModules.BIW_Part_Master_Config;
        //                globalData.messageDetail = ResourceModules.BIW_Part_Master_Config + " " + ResourceMessages.Create_Success;
        //                TempData["globalData"] = globalData;
        //                return RedirectToAction("Index");
        //            }
                    
        //        }


        //        globalData.pageTitle = ResourceModules.BIW_Part_Master_Config;
        //        globalData.subTitle = ResourceGlobal.Create;
        //        globalData.controllerName = "BIWPartNoMasterController";
        //        globalData.actionName = ResourceGlobal.Create;
        //        globalData.contentTitle = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Create;
        //        globalData.contentFooter = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Create;
        //        ViewBag.GlobalDataModel = globalData;

        //        plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //    }
        //    catch(Exception ex)
        //    {
        //        globalData.isErrorMessage = true;
        //        globalData.messageTitle = ResourceModules.BIW_Part_Master_Config;
        //        globalData.messageDetail = ResourceModules.BIW_Part_Master_Config + " " + ResourceMessages.Is_Error;
        //        TempData["globalData"] = globalData;
        //    }
        //    ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
        //    ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
        //    ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name");
        //    ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");
        //    ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
        //    ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
        //    return View(RS_BIW_Part_Master);
        //}

        public ActionResult GetChildPartMasterData(int Shop_ID)
        {
            var ChildPart = (from s in db.RS_Shops join
                             m in db.RS_Major_Sub_Assembly on
                             s.Sub_Assembly_ID equals m.Sub_Assembly_ID
                             where s.Shop_ID == Shop_ID
                             select new
                             {
                              id =  s.Sub_Assembly_ID,
                              value = m.Sub_Assembly_Name
                             }).ToList();

            return Json(ChildPart, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlatformByShopID(int Shop_ID)
        {
            var platform = (from s in db.RS_Shops
                            join m in db.RS_OM_Platform on
                            s.Shop_ID equals m.Shop_ID
                            where s.Shop_ID == Shop_ID
                            select new
                            {
                                id = m.Platform_ID,
                                value = m.Platform_Name
                            }).ToList();

            return Json(platform, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSeriesData(int Platform_ID)
        {
            var Series = (from s in db.RS_Serial_Number_Configuration
                          where s.Platform_ID == Platform_ID
                          select new
                          {
                              id = s.Config_ID,
                              value = s.Display_Name
                          }).ToList();
            return Json(Series, JsonRequestBehavior.AllowGet);
        }
        public class ChlidPartMaster
        {
            public decimal Shop_ID { get; set; }
            public decimal Platform_ID { get; set; }
            public decimal Sub_Assembly_ID { get; set; }
            public string Variant_Code { get; set; }
            public string VARIANT_DESC { get; set; }
            public string LONG_DESC { get; set; }
            public string Attribution_parmeter { get; set; }
            public long Config_ID { get; set; }

            public string OMConfig_ID { get; set; }

            public bool Is_Colour_Applicable { get; set; }
            public bool Is_Spare { get; set; }
            public bool In_Use { get; set; }
            public bool Color_Code { get; set; }
            public decimal Model_Attribute_ID { get; set; }
        }

        public class JSONData
        {
            public bool status { get; set; }
            public string type { get; set; }
            public string message { get; set; }

            public decimal ModelID { get; set; }
            public int Id { get; set; }
           
        }

        [HttpPost]
        public ActionResult SaveChildPartMasterData(string dataModal, List<string> modeldata)
        {
            JSONData objJSONData = new JSONData();
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
           
            //int ShopID = ((FDSession)this.Session["FDSession"]).shopId;
            try
            {

                RS_Model_Master objModel_Master = new RS_Model_Master();
                    RS_BIW_Part_Master objRS_BIW_Part_Master = new RS_BIW_Part_Master();
                List<ChlidPartMaster> objModelMaster = (List<ChlidPartMaster>)JsonConvert.DeserializeObject(modeldata[0], typeof(List<ChlidPartMaster>));
                var VariantCode = objModelMaster[0].Variant_Code;

                // Added Data in Model Master table
                //if(!(db.RS_Model_Master.Any(m=>m.Model_Code == VariantCode && m.Plant_ID == plantID)))
                //{
                //    objModel_Master.Plant_ID = plantID;
                //    objModel_Master.Shop_ID = objModelMaster[0].Shop_ID;
                //    objModel_Master.Model_Code = objModelMaster[0].Variant_Code;
                //    objModel_Master.Sub_Assembly_ID = objModelMaster[0].Sub_Assembly_ID;
                //    objModel_Master.Model_Description = objModelMaster[0].VARIANT_DESC;
                //    objModel_Master.Auto_Remarks = objModelMaster[0].LONG_DESC;
                //    objModel_Master.OMconfig_ID = objModelMaster[0].OMConfig_ID;
                //    objModel_Master.Platform_Id = objModelMaster[0].Platform_ID;
                //    objModel_Master.Config_ID = objModelMaster[0].Config_ID;
                //    objModel_Master.Color_Code = objModelMaster[0].Color_Code;
                //    objModel_Master.Is_Colour_Applicable = objModelMaster[0].Is_Colour_Applicable;
                //    objModel_Master.In_Use = objModelMaster[0].In_Use;
                //    objModel_Master.Is_Spare = objModelMaster[0].Is_Spare;
                //    objModel_Master.Model_Attribute_ID = objModelMaster[0].Model_Attribute_ID;
                //    objModel_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                //    objModel_Master.Inserted_Date = DateTime.Now;
                //    objModel_Master.Attribution_Parameters = dataModal;
                //    db.RS_Model_Master.Add(objModel_Master);
                //    db.SaveChanges();
                //}
                //End


                if (!(db.RS_BIW_Part_Master.Any(m => m.Variant_Code == VariantCode && m.Plant_ID == plantID)))
                {
                    objRS_BIW_Part_Master.Shop_ID = objModelMaster[0].Shop_ID;
                    objRS_BIW_Part_Master.Sub_Assembly_ID = objModelMaster[0].Sub_Assembly_ID;
                    objRS_BIW_Part_Master.Platform_ID = objModelMaster[0].Platform_ID;
                    objRS_BIW_Part_Master.Variant_Code = objModelMaster[0].Variant_Code;
                    objRS_BIW_Part_Master.VARIANT_DESC = objModelMaster[0].VARIANT_DESC;
                    objRS_BIW_Part_Master.LONG_DESC = objModelMaster[0].LONG_DESC;
                    //objRS_BIW_Part_Master.Config_ID = objModelMaster[0].Config_ID;
                    //objRS_BIW_Part_Master.OM_Config_ID = objModelMaster[0].OMConfig_ID;
                    //objRS_BIW_Part_Master.Plant_ID = plantID;
                    //objRS_BIW_Part_Master.Color_Code = objModelMaster[0].Color_Code;
                    //objRS_BIW_Part_Master.Is_Colour_Applicable = objModelMaster[0].Is_Colour_Applicable;
                    //objRS_BIW_Part_Master.Is_Spare = objModelMaster[0].Is_Spare;
                    //objRS_BIW_Part_Master.In_Use = objModelMaster[0].In_Use;
                    //objRS_BIW_Part_Master.Model_Attribute_ID = objModelMaster[0].Model_Attribute_ID;
                    //objRS_BIW_Part_Master.Attribution_Parameters = dataModal;



                    objRS_BIW_Part_Master.Inserted_Date = DateTime.Now;
                    objRS_BIW_Part_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    objRS_BIW_Part_Master.Plant_ID = plantID;
                    objRS_BIW_Part_Master.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_BIW_Part_Master.Add(objRS_BIW_Part_Master);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "Aggregate Part";
                    globalData.messageDetail = "Aggregate Part" + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    
                }
                else
                {
                    objJSONData.message = "Aggregate Part Already Exits!...";
                    objJSONData.type = "duplicate";
                    objJSONData.status = false;
                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }

                objJSONData.ModelID = objModel_Master.Model_ID;
                objJSONData.Id = objRS_BIW_Part_Master.Row_ID;
                objJSONData.status = true;
                objJSONData.message = "Aggregate Part Master Saved successfully!...";
                objJSONData.type = "Success";
                return Json(objJSONData, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                    }
                }
                throw raise;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                objJSONData.status = false;
                objJSONData.message = "Error in saving Aggregate Part Master!...";
                objJSONData.type = "fail";
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }
            //return View();
        }
        // GET: BIWPartNoMaster/Edit/5


        public ActionResult saveImage()
        {
            short userId;
            try
            {
                HttpFileCollectionBase files = Request.Files;
                if (Request.Files.Count > 0)
                {
                    string fileName = null;
                    string fileExtension = null;
                    var partImage = Request["Image_Name"];
                    var Variant_Code = Convert.ToString(Request["Variant_Code"]);
                    var plantID = ((FDSession)this.Session["FDSession"]).plantId;
                    
                    // var partNo = Request["partNo"];

                    userId = Convert.ToInt16(((FDSession)this.Session["FDSession"]).userId);
                    HttpPostedFileBase file = files[0];
                    var ID = Convert.ToInt32(Request["Id"]);
                    var ModelID = Convert.ToInt32(Request["ModelID"]);
                    fileName = Path.GetFileName(file.FileName);
                    fileExtension = Path.GetExtension(file.FileName);
                    var fileContent = file.ContentLength;
                    if (file != null && file.ContentLength > 0)
                    {
                        if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == "jpeg")
                        {
                            using (var reader = new System.IO.BinaryReader(file.InputStream))
                            {
                                //Upload Aggregate Part Image
                                var result = db.RS_Aggregate_Part_Image.Where(m => m.Part_ID == ID).Select(m => m.Part_ID).ToList();
                                var bytes = reader.ReadBytes(file.ContentLength);
                                if (result.Count > 0)
                                {
                                    var imageId = db.RS_Aggregate_Part_Image.Where(m => m.Part_ID == ID).Select(m => m.Image_ID).FirstOrDefault();
                                    RS_Aggregate_Part_Image imageObjPart = db.RS_Aggregate_Part_Image.Find(imageId);
                                    imageObjPart.Part_ID = ID;
                                    imageObjPart.Image_Content = bytes;
                                    imageObjPart.Image_Name = fileName;
                                    imageObjPart.Image_Type = Path.GetExtension(file.FileName);
                                    imageObjPart.Content_Type = file.ContentType;
                                    imageObjPart.Updated_Date = DateTime.Now;
                                    imageObjPart.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    imageObjPart.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    imageObjPart.Is_Edited = true;
                                    db.Entry(imageObjPart).State = EntityState.Modified;
                                    db.SaveChanges();
                                 
                                }
                                else
                                {
                                    RS_Aggregate_Part_Image imageObjPart1 = new RS_Aggregate_Part_Image();

                                    imageObjPart1.Part_ID = ID;
                                    imageObjPart1.Image_Name = fileName;
                                    imageObjPart1.Image_Type = Path.GetExtension(file.FileName);
                                    imageObjPart1.Image_Content = bytes;
                                    imageObjPart1.Content_Type = file.ContentType;
                                    imageObjPart1.Inserted_Date = DateTime.Now;
                                    imageObjPart1.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    imageObjPart1.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    db.RS_Aggregate_Part_Image.Add(imageObjPart1);
                                    db.SaveChanges();
                                }

                                //End

                                //Upload Model Master Image
                                //var result1 = db.RS_Model_Master_Image.Where(m => m.Model_ID == ModelID).Select(m => m.Model_ID).ToList();
                                //if (result1.Count > 0)
                                //{
                                //    var imageId = db.RS_Model_Master_Image.Where(m => m.Model_ID == ModelID).Select(m => m.Image_ID).FirstOrDefault();
                                //    RS_Model_Master_Image imageObjModel = db.RS_Model_Master_Image.Find(imageId);
                                //    imageObjModel.Model_ID = ModelID;
                                //    imageObjModel.Image_Content = bytes;
                                //    imageObjModel.Image_Name = fileName;
                                //    imageObjModel.Image_Type = Path.GetExtension(file.FileName);
                                //    imageObjModel.Content_Type = file.ContentType;
                                //    imageObjModel.Updated_Date = DateTime.Now;
                                //    imageObjModel.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                //    imageObjModel.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                //    imageObjModel.Is_Edited = true;
                                //    db.Entry(imageObjModel).State = EntityState.Modified;
                                //    db.SaveChanges();
                                //}
                                //else
                                //{
                                //    RS_Model_Master_Image imageObjModel1 = new RS_Model_Master_Image();

                                //    imageObjModel1.Model_ID = ModelID;
                                //    imageObjModel1.Image_Content = bytes;
                                //    imageObjModel1.Image_Name = fileName;
                                //    imageObjModel1.Image_Type = Path.GetExtension(file.FileName);
                                //    imageObjModel1.Content_Type = file.ContentType;
                                //    imageObjModel1.Inserted_Date = DateTime.Now;
                                //    imageObjModel1.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                //    imageObjModel1.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                //    db.RS_Model_Master_Image.Add(imageObjModel1);
                                //    db.SaveChanges();
                                //}
                                //End
                            }
                        }
                        else {
                            globalData.isErrorMessage = true;
                            globalData.messageTitle = "Aggregate Part Master";

                            globalData.messageDetail = "Image is not uploaded. Valid Image Format(.png|.jpg|.jpeg)";
                            TempData["globalData"] = globalData;
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = "Aggregate Part Master";//ResourceQualityImageGroups.Quality_ImageGroups;

                        globalData.messageDetail = "Aggregate Part Save Successfully but image is not uploaded";
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Create");
                    }
                    // width will increase the height proportionally
                    //ImageUpload imageUpload = new ImageUpload { Width = 2500 };
                    // mmQualityImageGroupObj.addRecordsInQualityImages(plantId, shopId, groupId, userId, ((FDSession)this.Session["FDSession"]).userHost);

                    // Returns message that successfully uploaded  

                    //to save using above parameters
                   

                    //MM_Quality_Image_Group mmQualityImageGroupObj = new MM_Quality_Image_Group();
                    //userId = ((FDSession)this.Session["FDSession"]).userId;

                    //imageId = mmQualityImageGroupObj.getImageId(Convert.ToInt16(imageId), userId, ((FDSession)this.Session["FDSession"]).userHost, true);

                    //String fileNameExtension = file.FileName.Split('.')[1];
                    //var fileName = DateTime.Now.Ticks + "." + fileNameExtension;

                    //var path = Path.Combine(Server.MapPath("~/Content/ModelMaster"), Variant_Code + "_" + plantID + fileExtension);
                    //if (path != null) System.IO.File.Delete(path);
                    //path = Path.Combine(Server.MapPath("~/Content/ModelMaster"), Variant_Code + "_" + plantID + fileExtension);
                    //file.SaveAs(path);

                    // process to add image in quality image table

                    //RS_BIW_Part_Master RS_Model_Master = db.RS_BIW_Part_Master.Where(m => m.Variant_Code.ToLower() == Variant_Code.ToLower() && m.Plant_ID == plantID).FirstOrDefault();
                    //if (RS_Model_Master != null)
                    //{
                    //    RS_Model_Master.Image_Name = Variant_Code + "_" + plantID + fileExtension;
                    //    RS_Model_Master.Updated_Date = DateTime.Now;
                    //    RS_Model_Master.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //    //db.MM_Quality_Images.Add(mmQualityImagesObj);
                    //    db.Entry(RS_Model_Master).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //    return Json(true, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(false, JsonRequestBehavior.AllowGet);
                    //}

                    //decimal imageID = mmQualityImagesObj.Image_ID;
                    ////db.Entry(mmQualityImagesObj).State = EntityState.Modified;
                    ////db.SaveChanges();


                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                    }
                }
                throw raise;
                //return Json(false, JsonRequestBehavior.AllowGet);
                //return Json(new { success = false, responseText = "Error in upaloding!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_BIW_Part_Master RS_BIW_Part_Master = db.RS_BIW_Part_Master.Find(id);
            globalData.pageTitle = ResourceModules.BIW_Part_Master_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "BIWPartNoMasterController";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Edit;
            globalData.contentFooter = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            //DirectoryInfo di = new DirectoryInfo("~/Content/ModelMaster/");
            //var c1 = "/Content/ModelMaster/" + RS_BIW_Part_Master.Variant_Code + "_" + RS_BIW_Part_Master.Plant_ID + ".png";
            //var pngfile = RS_BIW_Part_Master.Variant_Code + "_" + RS_BIW_Part_Master.Plant_ID + ".png";
            //var path = "";

            //path = "/Content/ModelMaster/" + RS_BIW_Part_Master.Variant_Code + "_" + RS_BIW_Part_Master.Plant_ID + ".jpg";
            //ViewBag.Image_Name = path;

            

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration.Where(m=> m.Platform_ID == RS_BIW_Part_Master.Platform_ID), "Config_ID", "Display_Name", RS_BIW_Part_Master.Config_ID);
            ViewBag.OM_Config_ID = new SelectList(db.RS_OM_Configuration.Where(m => m.Plant_ID == plantId), "OMconfig_ID", "OMconfig_Desc");
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(m => m.Shop_ID == RS_BIW_Part_Master.Shop_ID), "Platform_ID", "Platform_Name");
            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly.Where(s=>s.Sub_Assembly_ID== RS_BIW_Part_Master.Sub_Assembly_ID), "Sub_Assembly_ID", "Sub_Assembly_Name");
            ViewBag.Model_Attribute_ID = new SelectList(db.RS_Model_Attribute_Master.Where(m=>m.Sub_Assembly_ID == RS_BIW_Part_Master.Sub_Assembly_ID && m.Platform_ID == RS_BIW_Part_Master.Platform_ID), "Model_Attribute_ID", "Attribution");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            if (RS_BIW_Part_Master == null)
            {
                return HttpNotFound();
            }
            return View(RS_BIW_Part_Master);
        }

        // POST: BIWPartNoMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_BIW_Part_Master RS_BIW_Part_Master,HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RS_BIW_Part_Master obj_RS_BIW_Part_Master = db.RS_BIW_Part_Master.Find(RS_BIW_Part_Master.Row_ID);

                    if(upload != null && upload.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(upload.FileName);
                        if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                        {

                            using (var reader = new System.IO.BinaryReader(upload.InputStream))
                            {
                                obj_RS_BIW_Part_Master.Image_Name = System.IO.Path.GetFileName(upload.FileName);
                                obj_RS_BIW_Part_Master.Image_Type = Path.GetExtension(upload.FileName);
                                obj_RS_BIW_Part_Master.Content_Type = upload.ContentType;
                                obj_RS_BIW_Part_Master.Image_Content = reader.ReadBytes(upload.ContentLength);
                            }

                        }
                        else
                        {
                            globalData.isErrorMessage = true;
                            globalData.messageTitle = ResourceModules.BIW_Part_Master_Config;
                            globalData.messageDetail = "Image file format is uploaded";
                            TempData["globalData"] = globalData;
                            ViewBag.GlobalDataModel = globalData;
                            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name");
                            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");
                            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
                            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
                            return View(RS_BIW_Part_Master);
                        }
                    }
                    
                    obj_RS_BIW_Part_Master.Platform_ID = RS_BIW_Part_Master.Platform_ID;
                    obj_RS_BIW_Part_Master.Sub_Assembly_ID = RS_BIW_Part_Master.Sub_Assembly_ID;
                    obj_RS_BIW_Part_Master.Variant_Code = RS_BIW_Part_Master.Variant_Code.Trim();
                    obj_RS_BIW_Part_Master.VARIANT_DESC = RS_BIW_Part_Master.VARIANT_DESC.Trim();
                    //obj_RS_BIW_Part_Master.VARIANT_TYPE = RS_BIW_Part_Master.VARIANT_TYPE.Trim();
                    obj_RS_BIW_Part_Master.LONG_DESC = RS_BIW_Part_Master.LONG_DESC.Trim();
                    obj_RS_BIW_Part_Master.Updated_Date = DateTime.Now;
                    obj_RS_BIW_Part_Master.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    obj_RS_BIW_Part_Master.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.Entry(obj_RS_BIW_Part_Master).State = EntityState.Modified;

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.BIW_Part_Master_Config;
                    globalData.messageDetail = ResourceModules.BIW_Part_Master_Config + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                globalData.pageTitle = ResourceModules.BIW_Part_Master_Config;
                globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "BIWPartNoMasterController";
                globalData.actionName = ResourceGlobal.Edit;
                globalData.contentTitle = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Edit;
                globalData.contentFooter = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Edit;
                ViewBag.GlobalDataModel = globalData;

                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.BIW_Part_Master_Config;
                globalData.messageDetail = ResourceModules.BIW_Part_Master_Config + " " + ResourceMessages.Is_Error;
                TempData["globalData"] = globalData;

                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                    }
                }

                throw raise;
            }
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name",RS_BIW_Part_Master.Shop_ID);
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name", RS_BIW_Part_Master.Platform_ID);
            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name", RS_BIW_Part_Master.Sub_Assembly_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            return View(RS_BIW_Part_Master);
        }

        // GET: BIWPartNoMaster/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_BIW_Part_Master RS_BIW_Part_Master = db.RS_BIW_Part_Master.Find(id);
            globalData.pageTitle = ResourceModules.BIW_Part_Master_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "BIWPartNoMasterController";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Delete;
            globalData.contentFooter = ResourceModules.BIW_Part_Master_Config + " " + ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;

            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            if (RS_BIW_Part_Master == null)
            {
                return HttpNotFound();
            }
            return View(RS_BIW_Part_Master);
        }

        // POST: BIWPartNoMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RS_BIW_Part_Master biwobj = null;
            try { 
            RS_BIW_Part_Master RS_BIW_Part_Master = db.RS_BIW_Part_Master.Find(id);
                biwobj = RS_BIW_Part_Master;
            db.RS_BIW_Part_Master.Remove(RS_BIW_Part_Master);
            db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                //globalData.dbUpdateExceptionDetail = ex.InnerException.InnerException.Message.ToString();


                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_BIW_Part_Master", "Row_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isAlertMessage = true;
                globalData.messageTitle = ResourceModules.BIW_Part_Master_Config;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;


            }
            catch(Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.BIW_Part_Master_Config;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return View("Delete", biwobj);
            }
            return RedirectToAction("Index");
        }

        public ActionResult SaveChildPartMasterDataEdit(string dataModal, List<string> modeldata)
        {
            JSONData objJSONData = new JSONData();
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            //int ShopID = ((FDSession)this.Session["FDSession"]).shopId;
            try
            {
               
                RS_BIW_Part_Master objRS_Model_Master = new RS_BIW_Part_Master();
                List<ChlidPartMaster> objModelMaster = (List<ChlidPartMaster>)JsonConvert.DeserializeObject(modeldata[0], typeof(List<ChlidPartMaster>));
                string Variate_Code = objModelMaster[0].Variant_Code;
                RS_BIW_Part_Master obj_RS_Model_Master_Edit = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == Variate_Code && m.Plant_ID == plantID).FirstOrDefault();
                RS_Model_Master obj = db.RS_Model_Master.Where(m => m.Model_Code == Variate_Code && m.Plant_ID == plantID).FirstOrDefault();
               objRS_Model_Master.Attribution_Parameters = objModelMaster[0].Attribution_parmeter;

                //Edit Model Master Data
                //if (!(db.RS_Model_Master.Any(m => m.Model_Code == Variate_Code && m.Plant_ID == plantID)))
                //{

                //    objJSONData.status = false;
                //    objJSONData.message = "Model Code not found!...";
                //    objJSONData.type = "Errror";
                //    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    obj.Attribution_Parameters = dataModal;

                //    obj.Platform_Id = objModelMaster[0].Platform_ID;
                //    obj.OMconfig_ID = objModelMaster[0].OMConfig_ID;
                //    obj.Model_Code = objModelMaster[0].Variant_Code;
                //    obj.Model_Description = objModelMaster[0].VARIANT_DESC;
                //    obj.Shop_ID = objModelMaster[0].Shop_ID;
                //    obj.Config_ID = objModelMaster[0].Config_ID;
                //    obj.Sub_Assembly_ID = objModelMaster[0].Sub_Assembly_ID;
                //    obj.Plant_ID = plantID;
                //    obj.Color_Code = objModelMaster[0].Color_Code;
                //    obj.Auto_Remarks = objModelMaster[0].LONG_DESC;
                //    obj.Is_Colour_Applicable = objModelMaster[0].Is_Colour_Applicable;
                //    obj.In_Use = objModelMaster[0].In_Use;
                //    obj.Is_Spare = objModelMaster[0].Is_Spare;
                //    obj.Model_Attribute_ID = objModelMaster[0].Model_Attribute_ID;
                //    obj.Updated_Date = DateTime.Now;
                //    obj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                //    db.Entry(obj).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
                //End

                //Edit Model Master Data
                if (!(db.RS_BIW_Part_Master.Any(m => m.Variant_Code == Variate_Code && m.Plant_ID == plantID)))
                {

                    objJSONData.status = false;
                    objJSONData.message = "Child part master not found!...";
                    objJSONData.type = "Errror";
                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    obj_RS_Model_Master_Edit.Attribution_Parameters = dataModal;

                    obj_RS_Model_Master_Edit.Platform_ID = objModelMaster[0].Platform_ID;
                    obj_RS_Model_Master_Edit.Sub_Assembly_ID = objModelMaster[0].Sub_Assembly_ID;
                    obj_RS_Model_Master_Edit.Variant_Code = objModelMaster[0].Variant_Code;
                    obj_RS_Model_Master_Edit.VARIANT_DESC = objModelMaster[0].VARIANT_DESC;
                    obj_RS_Model_Master_Edit.LONG_DESC = objModelMaster[0].LONG_DESC;
                    obj_RS_Model_Master_Edit.Shop_ID = objModelMaster[0].Shop_ID;
                    obj_RS_Model_Master_Edit.Config_ID = objModelMaster[0].Config_ID;
                    obj_RS_Model_Master_Edit.Model_Attribute_ID = objModelMaster[0].Model_Attribute_ID;
                    obj_RS_Model_Master_Edit.Color_Code = objModelMaster[0].Color_Code;
                    obj_RS_Model_Master_Edit.Is_Colour_Applicable = objModelMaster[0].Is_Colour_Applicable;
                    obj_RS_Model_Master_Edit.In_Use = objModelMaster[0].In_Use;
                    obj_RS_Model_Master_Edit.Is_Spare = objModelMaster[0].Is_Spare;
                    obj_RS_Model_Master_Edit.Plant_ID = plantID;
                    obj_RS_Model_Master_Edit.Updated_Date = DateTime.Now;
                    obj_RS_Model_Master_Edit.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.Entry(obj_RS_Model_Master_Edit).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //End

                globalData.isSuccessMessage = true;
                globalData.messageTitle = "Aggregate Part";
                globalData.messageDetail = "Aggregate Part" + " " + ResourceMessages.Edit_Success;
                TempData["globalData"] = globalData;

                objJSONData.ModelID = obj.Model_ID;
                objJSONData.Id = obj_RS_Model_Master_Edit.Row_ID;
                objJSONData.message = "Aggregate Part Master Edited Successfully!...";
                objJSONData.type = "Success";
                objJSONData.status = true;
                return Json(objJSONData, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                    }
                }
                throw raise;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                objJSONData.status = false;
                objJSONData.message = "Error in saving Aggregate Part!...";
                objJSONData.type = "fail";
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }

        }


        //[OutputCache(Duration = 6)]
        public ActionResult ShowChildPartMasterData(string Part_No)
        {
            JSONData objJSONData = new JSONData();
            try
            {
                //var data = db.RS_Model_Master.Where(m => m.Model_Code == Model_Code).FirstOrDefault();
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var data = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == Part_No && m.Plant_ID == plantID).Select(m => new
                {
                    m.Attribution_Parameters,
                    m.Shop_ID,
                    m.Platform_ID,
                    m.Sub_Assembly_ID,
                    m.Variant_Code,
                    m.VARIANT_DESC,
                    m.LONG_DESC,
                    m.OM_Config_ID,
                    m.Config_ID,
                    m.Is_Spare,
                    m.In_Use,
                    m.Color_Code,
                    m.Is_Colour_Applicable,
                    m.Model_Attribute_ID
                });
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                    }
                }
                throw raise;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                objJSONData.status = false;
                objJSONData.message = "Erro in saving Child Part Master!...";
                objJSONData.type = "fail";
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }

        }
        public ActionResult GetData(int Sub_Assembly_ID)
        {
            // int plantId = ((FDSession)this.Session["FDSession"]).plantId;

            var ModelDetail = (from AttributeItem in db.RS_AttributionType_Master
                               where AttributeItem.Sub_Assembly_ID == Sub_Assembly_ID && AttributeItem.IsActive == true
                               orderby AttributeItem.ToolBox_Post ascending
                               select new
                               {
                                   ToolBox = AttributeItem.ToolBox,
                                   ToolBoxPost = AttributeItem.ToolBox_Post,
                                   Attribution_Type = AttributeItem.Attribution_Type,
                                   IsActive = AttributeItem.IsActive
                               });


            return Json(ModelDetail, JsonRequestBehavior.AllowGet);

        }
        public ActionResult RemoteVariantCode(int? Row_ID, string Variant_Code, int? Shop_ID, int Plant_ID)
        {
            var result = false;
            if((Row_ID == null && Row_ID==0) && Shop_ID != null && Plant_ID!=null)
            {
                result = !(db.RS_BIW_Part_Master.Any(m => m.Variant_Code.ToLower() == Variant_Code.ToLower() && m.Shop_ID == Shop_ID && m.Plant_ID == Plant_ID));

            }
            else
            {
                result = !(db.RS_BIW_Part_Master.Any(m => m.Variant_Code.ToLower() ==  Variant_Code.ToLower() && m.Shop_ID == Shop_ID && m.Plant_ID == Plant_ID && m.Row_ID !=Row_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAttributionParameters(int ID)
        {
            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var ModelDetail = (from AttributeItem in db.RS_AttributionType_Master
                               where AttributeItem.Sub_Assembly_ID == ID && AttributeItem.IsActive == true
                               orderby AttributeItem.ToolBox_Post ascending
                               select new
                               {
                                   ToolBox = AttributeItem.ToolBox,
                                   ToolBoxPost = AttributeItem.ToolBox_Post,
                                   Attribution_Type = AttributeItem.Attribution_Type
                               });


            return Json(ModelDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropDownLoad(string Attributetype)
        {
            var dyanamic_Data = db.RS_Attribution_Parameters
           .Where(c => c.Attribute_Type == Attributetype)
           .Select(c => new { c.Attribute_ID, c.Attribute_Desc })
           .Distinct()
           .OrderBy(c => c.Attribute_Desc);
            return Json(dyanamic_Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAttributeData(int SubAssyID, int PlatformID)
        {
            var attribute = (from attr in db.RS_Model_Attribute_Master
                         where attr.Sub_Assembly_ID == SubAssyID && attr.Platform_ID == PlatformID
                         select new
                         {
                             Id = attr.Model_Attribute_ID,
                             Value = attr.Attribution
                         }
                         ).ToList();
            return Json(attribute, JsonRequestBehavior.AllowGet);
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
