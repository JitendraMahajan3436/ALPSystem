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
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class NewModelMasterController : BaseController
    {
        //private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_Model_Master mmAttributionParametersObj = new RS_Model_Master();
        GlobalData globalData = new GlobalData();

        int plantId = 0, shopId = 0;

        General generalObj = new General();

        // GET: NewModelMaster
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Model_Master;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Model Master";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            var RS_Model_Master = db.RS_Model_Master.Include(m => m.RS_Plants).Include(m => m.RS_Shops).Where(p => p.Plant_ID == plantID);
            return View(RS_Model_Master.ToList());

        }

        // GET: NewModelMaster/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Model_Master RS_Model_Master = db.RS_Model_Master.Find(id);
            if (RS_Model_Master == null)
            {
                return HttpNotFound();
            }
            return View(RS_Model_Master);
        }

        // GET: NewModelMaster/Create
        public ActionResult Create()
        {
            try
            {

                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                globalData.pageTitle = ResourceModules.Model_Master;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Model Master";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Create + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Create + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Family = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type");
                ViewBag.Model_Type = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type");
                ViewBag.Varient = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type");
                ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc");
                ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
                ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration.Where(m => m.Plant_ID == plantId), "Config_ID", "Display_Name");
                ViewBag.Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description");
                ViewBag.Old_Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");
                ViewBag.Model_Attribute_ID = new SelectList(db.RS_Model_Attribute_Master.Where(m => m.Platform_ID == 0), "Model_Attribute_ID", "Attribution");
                ViewBag.Country_ID = new SelectList(db.RS_Country, "Country_ID", "Country_Name");

                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.plantID = plantId;
                ViewBag.Platform_Id = new SelectList(db.RS_OM_Platform.Where(m => m.Plant_ID == plantId), "Platform_ID", "Platform_Name");

                RS_Model_Master obj_MM_AD_Model_Master = db.RS_Model_Master.Where(m => m.Plant_ID == plantId).FirstOrDefault();
                //ViewBag.Model_Type = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type");
                ViewBag.Model_Code = new SelectList(db.RS_Model_Master.Where(m => m.Plant_ID == plantId), "Model_Code", "Model_Code");
                ViewBag.BIW_Part_No = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 1), "Variant_Code", "Variant_Code");
                ViewBag.Engine_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 2), "Variant_Code", "Variant_Code");
                ViewBag.TA_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 3), "Variant_Code", "Variant_Code");
                ViewBag.FA_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 4), "Variant_Code", "Variant_Code");
                ViewBag.RA_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 5), "Variant_Code", "Variant_Code");
                ViewBag.Chasis_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 6), "Variant_Code", "Variant_Code");
                ViewBag.Machine_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 7), "Variant_Code", "Variant_Code");


                ViewBag.OMConfigID = new SelectList(db.RS_OM_Configuration.Where(m => m.Plant_ID == plantId), "OMconfig_ID", "OMconfig_Desc");
                //ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration.Where(m => m.Plant_ID == plantId), "Config_ID", "Serial_Logic");
                var BodyLine = (from line in db.RS_Lines
                                where line.Shop_ID == 0 //&& shop.Shop_Name == "BIW SHOP" && 
                                select new { line.Line_ID, line.Line_Name }).ToList();
                //List<SelectList> Body_line = BodyLine.ToList();
                ViewBag.Body_Line = new SelectList(BodyLine, "Line_ID", "Line_Name");
                var Paint = (from line in db.RS_Lines
                             join shop in db.RS_Shops
                             on line.Shop_ID equals shop.Shop_ID
                             where shop.Shop_Name == "Paint Shop" && shop.Plant_ID == plantId
                             select new { line.Line_ID, line.Line_Name }).ToList();
                ViewBag.Paint_Line = new SelectList(Paint, "Line_ID", "Line_Name");
                var Assembly_Line = (from line in db.RS_Lines
                                     join shop in db.RS_Shops
                                     on line.Shop_ID equals shop.Shop_ID
                                     where shop.Shop_Name == "TCF" && shop.Plant_ID == plantId
                                     select new { line.Line_ID, line.Line_Name }).ToList();
                ViewBag.Assembly_Line = new SelectList(Assembly_Line, "Line_ID", "Line_Name");
                ViewBag.Color_Code1 = new SelectList(db.RS_Colour, "Row_ID", "Colour_Desc");
                //ViewBag.Platform = new SelectList(db.RS_OM_Platform,"Platform_ID","Platform_Name");
                //ViewBag.Style_Code = new SelectList(db.RS_Style_Code.Where(m => m.Plant_ID == plantId), "Style_Code", "Style_Code");
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }

        // POST: NewModelMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Plant_ID,Shop_ID,Model_Code,Model_Description,OMconfig_ID,Family,Model_Type,Varient,Series_Code,Tyre_Make_ID,Tyre_Make_Size_Front,Tyre_Make_Size_Rear,Old_Series_Code,Platform_Id,Config_ID,Colour_ID,Is_Domestic,Is_Deleted,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Model_Master RS_Model_Master)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.RS_Model_Master.Add(RS_Model_Master);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Family = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type", RS_Model_Master.Family);
        //    ViewBag.Model_Type = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type", RS_Model_Master.Model_Type);
        //    ViewBag.Varient = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type", RS_Model_Master.Varient);
        //    ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc", RS_Model_Master.Colour_ID);
        //    ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Model_Master.Plant_ID);
        //    ViewBag.Platform_Id = new SelectList(db.RS_Platform, "Platform_Id", "Platform_Description", RS_Model_Master.Platform_Id);
        //    ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration, "Config_ID", "Display_Name", RS_Model_Master.Config_ID);
        //    ViewBag.Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description", RS_Model_Master.Series_Code);
        //    ViewBag.Old_Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description", RS_Model_Master.Old_Series_Code);
        //    ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Model_Master.Shop_ID);
        //    return View(RS_Model_Master);
        //}

        // GET: NewModelMaster/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.Model_Master;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Model Master";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Create + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Create + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Family = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type");
            ViewBag.Model_Type = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type");
            ViewBag.Varient = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type");
            ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Platform_Id = new SelectList(db.RS_Platform.Where(m => m.Plant_ID == plantId), "Platform_Id", "Platform_Description");
            ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration.Where(m => m.Plant_ID == plantId), "Config_ID", "Display_Name");
            ViewBag.Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description");
            ViewBag.Old_Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            var assyIDs = db.RS_Model_Child_Part_Mapping.Where(m => m.Model_ID == id);

            RS_Major_Sub_Assembly []Assy = null;
            
            Assy = (from assembly in db.RS_Major_Sub_Assembly
                    where (from asy in db.RS_Model_Child_Part_Mapping where asy.Model_ID == id select asy.Sub_Assembly_ID).Contains(assembly.Sub_Assembly_ID)
                    select assembly).ToArray();
            //ViewBag.Sub_Assembly_ID = new MultiSelectList(db.RS_Major_Sub_Assembly.ToList(), "Sub_Assembly_ID", "Sub_Assembly_Name", Assy.Select(m => m.Sub_Assembly_ID).ToArray());
            
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.ImageContent = db.RS_Model_Master_Image.Where(m => m.Model_ID == id).Select(m => m.Image_Content).FirstOrDefault();
            ViewBag.plantID = plantId;
            RS_Model_Master obj_MM_AD_Model_Master = db.RS_Model_Master.Find(id);
            //var path = "/Content/ModelMaster/" + obj_MM_AD_Model_Master.Model_Code + "_"+ obj_MM_AD_Model_Master.Plant_ID + ".png";
            //if(path == null)
            //    path = "/Content/ModelMaster/" + obj_MM_AD_Model_Master.Model_Code + "_" + obj_MM_AD_Model_Master.Plant_ID + ".jpg";
            //if(path == null)
            //    path = "/Content/ModelMaster/" + obj_MM_AD_Model_Master.Model_Code + "_" + obj_MM_AD_Model_Master.Plant_ID + ".jpeg";

            //ViewBag.Image_Name = path;
                
           //ViewBag.Model_Type = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type");
           ViewBag.Model_Code = new SelectList(db.RS_Model_Master.Where(m => m.Plant_ID == plantId), "Model_Code", "Model_Code");
            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly.Where(m=>m.Sub_Assembly_ID == obj_MM_AD_Model_Master.Sub_Assembly_ID), "Sub_Assembly_ID", "Sub_Assembly_Name", obj_MM_AD_Model_Master.Sub_Assembly_ID);
            ViewBag.BIW_Part_No = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 1), "Variant_Code", "Variant_Code");
            ViewBag.Engine_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 2), "Variant_Code", "Variant_Code");
            ViewBag.TA_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 3), "Variant_Code", "Variant_Code");
            ViewBag.FA_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 4), "Variant_Code", "Variant_Code");
            ViewBag.RA_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 5), "Variant_Code", "Variant_Code");
            ViewBag.Chasis_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 6), "Variant_Code", "Variant_Code");
            ViewBag.Machine_PartNo = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Sub_Assembly_ID == 7), "Variant_Code", "Variant_Code");

            ViewBag.Model_Attribute_ID = new SelectList(db.RS_Model_Attribute_Master.Where(m => m.Platform_ID == obj_MM_AD_Model_Master.Platform_Id), "Model_Attribute_ID", "Attribution");
            ViewBag.OMConfigID = new SelectList(db.RS_OM_Configuration.Where(m => m.Plant_ID == plantId), "OMconfig_ID", "OMconfig_Desc");
            //ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration.Where(m => m.Plant_ID == plantId), "Config_ID", "Serial_Logic");
            var BodyLine = (from line in db.RS_Lines
                            join shop in db.RS_Shops
                            on line.Shop_ID equals shop.Shop_ID
                            where shop.Shop_ID == obj_MM_AD_Model_Master.Shop_ID //&& shop.Shop_Name == "BIW SHOP" && 
                            select new { line.Line_ID, line.Line_Name }).ToList();
            //List<SelectList> Body_line = BodyLine.ToList();
            ViewBag.Body_Line = new SelectList(BodyLine, "Line_ID", "Line_Name");
            var Paint = (from line in db.RS_Lines
                         join shop in db.RS_Shops
                         on line.Shop_ID equals shop.Shop_ID
                         where shop.Shop_Name == "Paint Shop" && shop.Plant_ID == plantId
                         select new { line.Line_ID, line.Line_Name }).ToList();
            ViewBag.Paint_Line = new SelectList(Paint, "Line_ID", "Line_Name");
            var Assembly_Line = (from line in db.RS_Lines
                                 join shop in db.RS_Shops
                                 on line.Shop_ID equals shop.Shop_ID
                                 where shop.Shop_Name == "TCF" && shop.Plant_ID == plantId
                                 select new { line.Line_ID, line.Line_Name }).ToList();
            ViewBag.Assembly_Line = new SelectList(Assembly_Line, "Line_ID", "Line_Name");
            var countryObj = (from a in db.RS_Country
                           join b in db.RS_ModelCode_Country_Mapping on a.Country_ID equals b.Country_ID
                           where b.Model_ID == obj_MM_AD_Model_Master.Model_ID
                           select a.Country_ID).ToList();
            ViewBag.Countries = new MultiSelectList(db.RS_Country, "Country_ID", "Country_Name", countryObj);
            //ViewBag.Color_Code = new SelectList(new List<object> { new { value = "False", text = "False" }, new { value = "True", text = "True" } }, "value", "text", 2);
            ViewBag.Color_Code1 = new SelectList(db.RS_Colour, "Row_ID", "Colour_Desc");
            //ViewBag.Platform = new SelectList(db.RS_OM_Platform,"Platform_ID","Platform_Name");
            //ViewBag.Style_Code = new SelectList(db.RS_Style_Code.Where(m => m.Plant_ID == plantId), "Style_Code", "Style_Code");
            return View(obj_MM_AD_Model_Master);
        }
        //public ActionResult Edit(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //    ViewBag.plantID = plantId;
        //    RS_Model_Master RS_Model_Master = db.RS_Model_Master.Find(id);
        //    if (RS_Model_Master == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc", RS_Model_Master.Colour_ID);
        //    ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Model_Master.Plant_ID);
        //    ViewBag.Platform_Id = new SelectList(db.RS_Platform, "Platform_Id", "Platform_Description", RS_Model_Master.Platform_Id);
        //    ViewBag.BIW_Part_No = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Plant_ID == plantId), "Variant_Code", "Variant_Code");
        //    ViewBag.OMConfigID = new SelectList(db.RS_OM_Configuration.Where(m => m.Plant_ID == plantId), "OMconfig_ID", "OMconfig_Desc");
        //    ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration.Where(m => m.Plant_ID == plantId), "Config_ID", "Serial_Logic");
        //    ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Model_Master.Shop_ID);
        //    return View("Create", RS_Model_Master);
        //}

        // POST: NewModelMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Plant_ID,Shop_ID,Model_Code,Model_Description,OMconfig_ID,Family,Model_Type,Varient,Series_Code,Tyre_Make_ID,Tyre_Make_Size_Front,Tyre_Make_Size_Rear,Old_Series_Code,Platform_Id,Config_ID,Colour_ID,Is_Domestic,Is_Deleted,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Model_Master RS_Model_Master)
        {
            if (ModelState.IsValid)
            {
                db.Entry(RS_Model_Master).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Family = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type", RS_Model_Master.Family);
            ViewBag.Model_Type = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type", RS_Model_Master.Model_Type);
            ViewBag.Varient = new SelectList(db.RS_Attribution_Parameters, "Attribute_ID", "Attribute_Type", RS_Model_Master.Varient);
            ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc", RS_Model_Master.Colour_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Model_Master.Plant_ID);
            ViewBag.Platform_Id = new SelectList(db.RS_Platform, "Platform_Id", "Platform_Description", RS_Model_Master.Platform_Id);
            ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration, "Config_ID", "Display_Name", RS_Model_Master.Config_ID);
            ViewBag.Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description", RS_Model_Master.Series_Code);
            ViewBag.Old_Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description", RS_Model_Master.Old_Series_Code);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Model_Master.Shop_ID);
            ViewBag.BIW_Part_No = new SelectList(db.RS_BIW_Part_Master.Where(m => m.Plant_ID == plantId), "Variant_Code", "Variant_Code");
            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");
            return View("Create", RS_Model_Master);
        }

        // GET: NewModelMaster/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.Model_Master;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Model Master";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RS_Model_Master RS_Model_Master = db.RS_Model_Master.Find(id);
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            if (RS_Model_Master == null)
            {
                return HttpNotFound();
            }
            var PlatformName = db.RS_OM_Platform.Where(m => m.Plant_ID == RS_Model_Master.Plant_ID && m.Platform_ID == RS_Model_Master.Platform_Id).Select(m => m.Platform_Name).FirstOrDefault();
            if (PlatformName != null)
            {
                ViewBag.PlatformName = PlatformName;
            }

            return View(RS_Model_Master);
        }

        // POST: NewModelMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal? id)
        {
            try
            {
                RS_Model_Master RS_Model_Master = db.RS_Model_Master.Find(id);
                db.RS_Model_Master.Remove(RS_Model_Master);
                db.SaveChanges();
                globalData.pageTitle = ResourceModules.Model_Master;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "Model Master";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Model_Master;
                globalData.messageDetail = "Model " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
            }
            catch (DbUpdateException ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Model_Master + " " + ResourceGlobal.Config;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                //generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Model_Master", "Part_No", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
            }
            catch(Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Model_Master + " " + ResourceGlobal.Config;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
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


        public ActionResult GetLineByShopID(int Shop_ID)
        {
            var line = (from s in db.RS_Lines
                            where s.Shop_ID == Shop_ID
                            select new
                            {
                                id = s.Line_ID,
                                value = s.Line_Name
                            }).ToList();

            return Json(line, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetImageData(string[] AssembyID, int ModelID)
        {
            byte[] BIWContent = null, EngineContent = null, TAContent = null, FAContent = null, RAContent = null, CHContent = null, MachineContent = null;
            List<ModelMaster1> obj = new List<ModelMaster1>();
            string[] AssID = null;
            AssID = AssembyID.ToString().Split(',');
            var length = AssID.Length;
            //int[] IDs = new int[length];
            RS_Model_Master mmobj = db.RS_Model_Master.Find(ModelID);
            if (AssembyID != null)
            {
                foreach (var id in AssembyID)
                { 
                    var BIWPartNo = db.RS_Model_Master.Where(m => m.Model_ID == ModelID).Select(m => m.BIW_Part_No).FirstOrDefault();
                    if (Convert.ToInt32(id) == 1 && BIWPartNo != "")
                    {
                        var path = "/Content/ModelMaster/" + mmobj.BIW_Part_No + "_" + mmobj.Plant_ID + ".png";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.BIW_Part_No + "_" + mmobj.Plant_ID + ".jpg";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.BIW_Part_No + "_" + mmobj.Plant_ID + ".jpeg";

                        var BIWPartID = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == BIWPartNo && m.Sub_Assembly_ID == 1).Select(m => m.Row_ID).FirstOrDefault();
                        BIWContent = db.RS_Aggregate_Part_Image.Where(m => m.Part_ID == BIWPartID).Select(m => m.Image_Content).FirstOrDefault();
                        ModelMaster1 BIWData = new ModelMaster1(1, BIWContent,path);
                        obj.Add(BIWData);
                        continue;
                    }

                    var ENPartNo = db.RS_Model_Master.Where(m => m.Model_ID == ModelID).Select(m => m.Engine_PartNo).FirstOrDefault();
                    if (Convert.ToInt32(id) == 2 && ENPartNo != "")
                    {
                        var path = "/Content/ModelMaster/" + mmobj.Engine_PartNo + "_" + mmobj.Plant_ID + ".png";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.Engine_PartNo + "_" + mmobj.Plant_ID + ".jpg";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.Engine_PartNo + "_" + mmobj.Plant_ID + ".jpeg";

                        var EnginePartID = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == ENPartNo && m.Sub_Assembly_ID == 2).Select(m => m.Row_ID).FirstOrDefault();
                        EngineContent = db.RS_Aggregate_Part_Image.Where(m => m.Part_ID == EnginePartID).Select(m => m.Image_Content).FirstOrDefault();
                        ModelMaster1 EngineData = new ModelMaster1(2, EngineContent,path);
                        obj.Add(EngineData);
                        continue;
                    }

                    var TAPartNo = db.RS_Model_Master.Where(m => m.Model_ID == ModelID).Select(m => m.TA_PartNo).FirstOrDefault();
                    if (Convert.ToInt32(id) == 3 && TAPartNo != "")
                    {
                        var path = "/Content/ModelMaster/" + mmobj.TA_PartNo + "_" + mmobj.Plant_ID + ".png";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.TA_PartNo + "_" + mmobj.Plant_ID + ".jpg";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.TA_PartNo + "_" + mmobj.Plant_ID + ".jpeg";

                        var TAPartID = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == TAPartNo && m.Sub_Assembly_ID == 3).Select(m => m.Row_ID).FirstOrDefault();
                        TAContent = db.RS_Aggregate_Part_Image.Where(m => m.Part_ID == TAPartID).Select(m => m.Image_Content).FirstOrDefault();
                        ModelMaster1 TAData = new ModelMaster1(3, TAContent,path);
                        obj.Add(TAData);
                        continue;
                    }

                    var FAPartNo = db.RS_Model_Master.Where(m => m.Model_ID == ModelID).Select(m => m.FA_PartNo).FirstOrDefault();
                    if (Convert.ToInt32(id) == 4 && FAPartNo != "")
                    {
                        var path = "/Content/ModelMaster/" + mmobj.TA_PartNo + "_" + mmobj.Plant_ID + ".png";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.TA_PartNo + "_" + mmobj.Plant_ID + ".jpg";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.TA_PartNo + "_" + mmobj.Plant_ID + ".jpeg";

                        var FAPartID = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == FAPartNo && m.Sub_Assembly_ID == 4).Select(m => m.Row_ID).FirstOrDefault();
                        FAContent = db.RS_Aggregate_Part_Image.Where(m => m.Part_ID == FAPartID).Select(m => m.Image_Content).FirstOrDefault();
                        ModelMaster1 FAData = new ModelMaster1(4, FAContent,path);
                        obj.Add(FAData);
                        continue;
                    }

                    var RAPartNo = db.RS_Model_Master.Where(m => m.Model_ID == ModelID).Select(m => m.RA_PartNo).FirstOrDefault();
                    if (Convert.ToInt32(id) == 5 && RAPartNo != "")
                    {
                        var path = "/Content/ModelMaster/" + mmobj.RA_PartNo + "_" + mmobj.Plant_ID + ".png";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.RA_PartNo + "_" + mmobj.Plant_ID + ".jpg";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.RA_PartNo + "_" + mmobj.Plant_ID + ".jpeg";

                        var RAPartID = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == RAPartNo && m.Sub_Assembly_ID == 5).Select(m => m.Row_ID).FirstOrDefault();
                        RAContent = db.RS_Aggregate_Part_Image.Where(m => m.Part_ID == RAPartID ).Select(m => m.Image_Content).FirstOrDefault();
                        ModelMaster1 RAData = new ModelMaster1(5, RAContent,path);
                        obj.Add(RAData);
                        continue;
                    }

                    var ChasisPartNo = db.RS_Model_Master.Where(m => m.Model_ID == ModelID).Select(m => m.Chasis_PartNo).FirstOrDefault();
                    if (Convert.ToInt32(id) == 6 && ChasisPartNo != "")
                    {
                        var path = "/Content/ModelMaster/" + mmobj.Chasis_PartNo + "_" + mmobj.Plant_ID + ".png";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.Chasis_PartNo + "_" + mmobj.Plant_ID + ".jpg";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.Chasis_PartNo + "_" + mmobj.Plant_ID + ".jpeg";

                        var CHPartID = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == ChasisPartNo && m.Sub_Assembly_ID == 6).Select(m => m.Row_ID).FirstOrDefault();
                        CHContent = db.RS_Aggregate_Part_Image.Where(m => m.Part_ID == CHPartID).Select(m => m.Image_Content).FirstOrDefault();
                        ModelMaster1 ChData = new ModelMaster1(6, CHContent,path);
                        obj.Add(ChData);
                        continue;
                    }

                    var MachinePartNo = db.RS_Model_Master.Where(m => m.Model_ID == ModelID).Select(m => m.Machine_PartNo).FirstOrDefault();
                    if (Convert.ToInt32(id) == 7 && MachinePartNo != "")
                    {
                        var path = "/Content/ModelMaster/" + mmobj.Machine_PartNo + "_" + mmobj.Plant_ID + ".png";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.Machine_PartNo + "_" + mmobj.Plant_ID + ".jpg";
                        if (path == null)
                            path = "/Content/ModelMaster/" + mmobj.Machine_PartNo + "_" + mmobj.Plant_ID + ".jpeg";

                        var MachinePartID = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == MachinePartNo && m.Sub_Assembly_ID == 7).Select(m => m.Row_ID).FirstOrDefault();
                        MachineContent = db.RS_Aggregate_Part_Image.Where(m => m.Part_ID == MachinePartID).Select(m => m.Image_Content).FirstOrDefault();
                        ModelMaster1 ChData = new ModelMaster1(7, MachineContent, path);
                        obj.Add(ChData);
                        continue;
                    }
                }
            }
            var jsonresult = Json(obj, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = Int32.MaxValue;
            return jsonresult;
        }

        [HttpPost]
        public ActionResult GetImagedataPart(String PartNo)
        {
            var partId = db.RS_BIW_Part_Master.Where(M => M.Variant_Code == PartNo).Select(m => m.Row_ID).FirstOrDefault();
            var st = (from partimage in db.RS_Aggregate_Part_Image
                      where partimage.Part_ID == partId
                      select new
                      {
                          Image_ID = partimage.Image_ID,
                          Image_Content = partimage.Image_Content
                      }
                    ).ToList().Distinct();
            return Json(st, JsonRequestBehavior.AllowGet);
        }

        public ActionResult fillCountries()
        {
            var countryObj = from a in db.RS_Country
                          select new
                          {
                              Country_ID = a.Country_ID,
                              Country_Name = a.Country_Name
                          };

            return Json(countryObj, JsonRequestBehavior.AllowGet);
        }


        //[OutputCache(Duration = 4)]
        public ActionResult GetData(int SubAssemblyID)
        {
            // int plantId = ((FDSession)this.Session["FDSession"]).plantId;

            var ModelDetail = (from AttributeItem in db.RS_AttributionType_Master
                               where AttributeItem.Sub_Assembly_ID == SubAssemblyID && AttributeItem.IsActive == true
                               orderby AttributeItem.ToolBox_Post ascending
                               select new
                               {
                                   DisplayName = AttributeItem.RS_Major_Sub_Assembly.Sub_Assembly_Name + " - Attributes",
                                   ID = AttributeItem.Sub_Assembly_ID,
                                   ToolBox = AttributeItem.ToolBox,
                                   ToolBoxPost = AttributeItem.ToolBox_Post,
                                   Attribution_Type = AttributeItem.Attribution_Type
                               });


            return Json(ModelDetail, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetDropDownLoad(string Attributetype)
        {
            string Shop_name = "";
            if (Attributetype == "Body Line")
            {
                Shop_name = "BIW SHOP";
                var dyanamic_Data = (from line in db.RS_Lines
                                     join shop in db.RS_Shops
                                on line.Shop_ID equals shop.Shop_ID
                                     where shop.Shop_Name == Shop_name
                                     select new
                                     {
                                         Attribute_ID = line.Line_ID,
                                         Attribute_Desc = line.Line_Name
                                     });

                return Json(dyanamic_Data, JsonRequestBehavior.AllowGet);
            }
            else if (Attributetype == "Paint Line")
            {
                Shop_name = "Paint Shop";
                var dyanamic_Data = (from line in db.RS_Lines
                                     join shop in db.RS_Shops
                                on line.Shop_ID equals shop.Shop_ID
                                     where shop.Shop_Name == Shop_name
                                     select new
                                     {
                                         Attribute_ID = line.Line_ID,
                                         Attribute_Desc = line.Line_Name
                                     });

                return Json(dyanamic_Data, JsonRequestBehavior.AllowGet);
            }
            else if (Attributetype == "Assembly Line")
            {
                Shop_name = "TCF";
                var dyanamic_Data = (from line in db.RS_Lines
                                     join shop in db.RS_Shops
                                on line.Shop_ID equals shop.Shop_ID
                                     where shop.Shop_Name == Shop_name
                                     select new
                                     {
                                         Attribute_ID = line.Line_ID,
                                         Attribute_Desc = line.Line_Name
                                     });

                return Json(dyanamic_Data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dyanamic_Data = db.RS_Attribution_Parameters
               .Where(c => c.Attribute_Type == Attributetype)
               .Select(c => new { c.Attribute_ID, c.Attribute_Desc })
               .Distinct()
               .OrderBy(c => c.Attribute_Desc);
                return Json(dyanamic_Data, JsonRequestBehavior.AllowGet);
            }


        }
        public class ModelMaster1
        {
            public int Assembly_ID { get; set; }
            public byte[] Image_Content { get; set; }

            public string Path { get; set; }
            public ModelMaster1(int Assembly_ID, byte[] Image_Content,string Path)
            {
                this.Image_Content = Image_Content;
                this.Assembly_ID = Assembly_ID;
                this.Path = Path;
            }
        }
        public class ModelMaster
        {
            public string Model_Code { get; set; }
            public string BIW_PartNo { get; set; }
            public string Vin_Code { get; set; }
            public string BIW_Desription { get; set; }
            public string Model_Description { get; set; }
            public string Auto_Remarks { get; set; }
            public bool In_Use { get; set; }
            public string Attribution_parmeter { get; set; }
            public string OMconfig_ID { get; set; }

            public decimal Model_Attribute_ID { get; set; }
            public int? Config_ID { get; set; }

            public bool Color_Code { get; set; }
            public decimal? Colour_ID { get; set; }
            public bool Is_Colour_Applicable { get; set; }
            public bool Is_Spare { get; set; }
            public string Style_Code { get; set; }
            public int? Body_Line { get; set; }
            public int? Paint_Line { get; set; }
            public int? Assembly_Line { get; set; }
            public int Platform_Id { get; set; }
            public decimal Shop_ID { get; set; }
            public decimal Sub_Assembly_ID { get; set; }
            public string Engine_PartNo { get; set; }
            public string Engine_Part_Desc { get; set; }
            public string TA_PartNo { get; set; }
            public string TA_Part_Desc { get; set; }
            public string FA_PartNo { get; set; }
            public string FA_Part_Desc { get; set; }
            public string RA_PartNo { get; set; }
            public string RA_PartDesc { get; set; }
            public string Chasis_PartNo { get; set; }
            public string Chasis_PartDesc { get; set; }
            public string Machine_PartNo { get; set; }
            public string Machine_PartDesc { get; set; }
            public int[] Countries { get; set; }
        }
        public class JSONData
        {
            public bool status { get; set; }
            public string type { get; set; }
            public string message { get; set; }

            public decimal Id { get; set; }
        }
        public ActionResult SaveModelMasterDataCreate(string dataModal,List<string> modeldata, string formData)
        {
            JSONData objJSONData = new JSONData();
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            //int ShopID = ((FDSession)this.Session["FDSession"]).shopId;
            try
            {
                
                RS_Model_Master objRS_Model_Master = new RS_Model_Master();
                List<ModelMaster> objModelMaster = (List<ModelMaster>)JsonConvert.DeserializeObject(modeldata[0], typeof(List<ModelMaster>));
                string Model_code = objModelMaster[0].Model_Code;
                string OMConfigID = objModelMaster[0].OMconfig_ID;
                string biwpartno = objModelMaster[0].BIW_PartNo;

                List<string> ModalIDs = new List<string>();
                if(objModelMaster[0].BIW_PartNo != "")
                {
                    var BIWPartNo = objModelMaster[0].BIW_PartNo;
                    //var modalID = db.RS_Model_Master.Where(m => m.Model_Code == BIWPartNo).Select(m => m.Model_ID).FirstOrDefault();
                    //ModalIDs.Add("ModelID:"+modalID);
                }
                if (objModelMaster[0].Engine_PartNo != "")
                {
                    var EnginePartNo = objModelMaster[0].Engine_PartNo;
                    //var modalID = db.RS_Model_Master.Where(m => m.Model_Code == EnginePartNo).Select(m => m.Model_ID).FirstOrDefault();
                    //ModalIDs.Add("ModelID:" + modalID);
                }
                if (objModelMaster[0].TA_PartNo != "")
                {
                    var TAPartNo = objModelMaster[0].TA_PartNo;
                   // var modalID = db.RS_Model_Master.Where(m => m.Model_Code == TAPartNo).Select(m => m.Model_ID).FirstOrDefault();
                    //ModalIDs.Add("ModelID:" + modalID);
                }
                if (objModelMaster[0].FA_PartNo != "")
                {
                    var FAPartNo = objModelMaster[0].FA_PartNo;
                    //var modalID = db.RS_Model_Master.Where(m => m.Model_Code == FAPartNo).Select(m => m.Model_ID).FirstOrDefault();
                    //ModalIDs.Add("ModelID:" + modalID);
                }
                if (objModelMaster[0].RA_PartNo != "")
                {
                    var RAPartNo = objModelMaster[0].RA_PartNo;
                    //var modalID = db.RS_Model_Master.Where(m => m.Model_Code == RAPartNo).Select(m => m.Model_ID).FirstOrDefault();
                    //ModalIDs.Add("ModelID:" + modalID);
                }
                if (objModelMaster[0].Chasis_PartNo != "")
                {
                    var CHPartNo = objModelMaster[0].Chasis_PartNo;
                    //var modalID = db.RS_Model_Master.Where(m => m.Model_Code == CHPartNo).Select(m => m.Model_ID).FirstOrDefault();
                    //ModalIDs.Add("ModelID:" + modalID);
                }
                //RS_OM_Configuration shop = db.RS_OM_Configuration.Where(m => m.OMconfig_ID == OMConfigID).FirstOrDefault();

                //string[] IDs = ModalIDs.ToArray();

                //string AttributionParameters = Newtonsoft.Json.JsonConvert.SerializeObject(IDs);

                //objRS_Model_Master.Attribution_Parameters = AttributionParameters;

                if (!(db.RS_Model_Master.Any(m => m.Model_Code == Model_code && m.Plant_ID == plantID)))
                {

                    //m.Variant_Code.Equals(objModelMaster[0].BIW_PartNo.ToString(), StringComparison.CurrentCultureIgnoreCase) &&

                    if (biwpartno != "")
                    {
                        var stylecodeid = db.RS_BIW_Part_Master.Where(m => m.Variant_Code.Equals(biwpartno, StringComparison.CurrentCultureIgnoreCase) && m.Plant_ID == plantID).Select(m => m.StyleCode_ID).FirstOrDefault();
                        var stylecode = stylecodeid != null ?db.RS_Style_Code.Find(stylecodeid).Style_Code:"";
                        objRS_Model_Master.Style_Code = stylecode;
                    }


                    //var stylecode = db.RS_BIW_Part_Master.Where(m =>  m.Plant_ID == plantID && m.).Select(m => m.RS_Style_Code.Style_Code).FirstOrDefault();

                    objRS_Model_Master.Attribution_Parameters = dataModal;
                    
                    objRS_Model_Master.Model_Code = objModelMaster[0].Model_Code;
                    objRS_Model_Master.BIW_Part_No = objModelMaster[0].BIW_PartNo;
                    objRS_Model_Master.VIN_Code = objModelMaster[0].Vin_Code;
                    objRS_Model_Master.BIW_Description = objModelMaster[0].BIW_Desription;
                    objRS_Model_Master.Model_Description = objModelMaster[0].Model_Description;
                    objRS_Model_Master.Auto_Remarks = objModelMaster[0].Auto_Remarks;
                    objRS_Model_Master.In_Use = objModelMaster[0].In_Use;
                    objRS_Model_Master.OMconfig_ID = objModelMaster[0].OMconfig_ID;
                    objRS_Model_Master.Config_ID = objModelMaster[0].Config_ID;
                    objRS_Model_Master.Model_Attribute_ID = objModelMaster[0].Model_Attribute_ID;
                    //objRS_Model_Master.Family = "NULL";

                    // Added by Ajay 07032019
                    
                    objRS_Model_Master.Engine_PartNo = objModelMaster[0].Engine_PartNo;
                    objRS_Model_Master.Engine_Part_Desc = objModelMaster[0].Engine_Part_Desc;
                    objRS_Model_Master.TA_PartNo = objModelMaster[0].TA_PartNo;
                    objRS_Model_Master.TA_Part_Desc = objModelMaster[0].TA_Part_Desc;
                    objRS_Model_Master.FA_PartNo = objModelMaster[0].FA_PartNo;
                    objRS_Model_Master.FA_Part_Desc = objModelMaster[0].FA_Part_Desc;
                    objRS_Model_Master.RA_PartNo = objModelMaster[0].RA_PartNo;
                    objRS_Model_Master.RA_PartDesc = objModelMaster[0].RA_PartDesc;
                    objRS_Model_Master.Chasis_PartNo = objModelMaster[0].Chasis_PartNo;
                    objRS_Model_Master.Chasis_PartDesc = objModelMaster[0].Chasis_PartDesc;
                    // Added by Ajay 07032019

                    objRS_Model_Master.Tyre_Make_Size_Front = null;//"NULL";
                    objRS_Model_Master.Tyre_Make_Size_Rear = null;//"NULL";

                    //objRS_Model_Master.Platform_Id = "1";
                    objRS_Model_Master.Platform_Id = objModelMaster[0].Platform_Id;

                    objRS_Model_Master.Inserted_Date = DateTime.Now;
                    objRS_Model_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    objRS_Model_Master.Plant_ID = plantID;
                    //objRS_Model_Master.Shop_ID = shop.Shop_ID;

                    objRS_Model_Master.Color_Code = objModelMaster[0].Color_Code;
                    objRS_Model_Master.Colour_ID = objModelMaster[0].Colour_ID;
                    objRS_Model_Master.Is_Colour_Applicable = objModelMaster[0].Is_Colour_Applicable;

                    objRS_Model_Master.Is_Spare = objModelMaster[0].Is_Spare;

                    objRS_Model_Master.Paint_Line = objModelMaster[0].Paint_Line;
                    objRS_Model_Master.Assembly_Line = objModelMaster[0].Assembly_Line;
                    objRS_Model_Master.Body_Line = objModelMaster[0].Body_Line;
                    objRS_Model_Master.Shop_ID = objModelMaster[0].Shop_ID;
                    objRS_Model_Master.Image_Name = "";
                    objRS_Model_Master.Sub_Assembly_ID = objModelMaster[0].Sub_Assembly_ID;
                    db.RS_Model_Master.Add(objRS_Model_Master);
                    db.SaveChanges();

                    RS_ModelCode_Country_Mapping RS_ModelCode_Country_Mapping = new RS_ModelCode_Country_Mapping();
                    foreach(var id in objModelMaster[0].Countries)
                    {
                        RS_ModelCode_Country_Mapping.Model_ID = objRS_Model_Master.Model_ID;
                        RS_ModelCode_Country_Mapping.Country_ID = id;
                        RS_ModelCode_Country_Mapping.Inserted_Date = DateTime.Now;
                        RS_ModelCode_Country_Mapping.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        RS_ModelCode_Country_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.RS_ModelCode_Country_Mapping.Add(RS_ModelCode_Country_Mapping);
                        db.SaveChanges();
                    }

                    RS_Model_Child_Part_Mapping obj = new RS_Model_Child_Part_Mapping();
                    //foreach(var item in objModelMaster[0].Sub_Assembly_ID)
                    {
                        obj.Model_ID = objRS_Model_Master.Model_ID;
                        obj.Sub_Assembly_ID = objModelMaster[0].Sub_Assembly_ID;
                        var item = objModelMaster[0].Sub_Assembly_ID;
                        if (item == 1)
                        {
                            obj.Part_No = objModelMaster[0].BIW_PartNo;
                            obj.Part_Description = objModelMaster[0].BIW_Desription;
                        }
                        if(item == 2)
                        {
                            obj.Part_No = objModelMaster[0].Engine_PartNo;
                            obj.Part_Description = objModelMaster[0].Engine_Part_Desc;
                        }
                        if(item == 3)
                        {
                            obj.Part_No = objModelMaster[0].TA_PartNo;
                            obj.Part_Description = objModelMaster[0].TA_Part_Desc;
                        }
                        if(item == 4)
                        {
                            obj.Part_No = objModelMaster[0].FA_PartNo;
                            obj.Part_Description = objModelMaster[0].FA_Part_Desc;
                        }
                        if(item == 5)
                        {
                            obj.Part_No = objModelMaster[0].RA_PartNo;
                            obj.Part_Description = objModelMaster[0].RA_PartDesc;
                        }
                        if(item == 6)
                        {
                            obj.Part_No = objModelMaster[0].Chasis_PartNo;
                            obj.Part_Description = objModelMaster[0].Chasis_PartDesc;
                        }
                        obj.Inserted_Date = DateTime.Now;
                        obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.RS_Model_Child_Part_Mapping.Add(obj);
                        db.SaveChanges();
                    }


                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Model_Master;
                    globalData.messageDetail = ResourceModules.Model_Master + " " + ResourceMessages.Create_Success;
                    TempData["globalData"] = globalData;



                    //return RedirectToAction("Index");
                    objJSONData.Id = objRS_Model_Master.Model_ID;
                    objJSONData.status = true;
                    objJSONData.message = "Model Master Saved successfully!...";
                    objJSONData.type = "Success";
                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    //globalData.isAlertMessage = true;
                    //globalData.messageTitle = ResourceModules.Model_Master;
                    //globalData.messageDetail = ResourceModules.Model_Master + " " + ResourceMessages.Is_Error;
                    //TempData["globalData"] = globalData;

                    objJSONData.message = "Model Master Already Exits!...";
                    objJSONData.type = "duplicate";
                    objJSONData.status = false;
                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }

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
                objJSONData.message = "Error in saving Model Master!...";
                objJSONData.type = "fail";
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }

        }
        public ActionResult SaveModelMasterDataEdit(string dataModal, List<string> modeldata)
        {
            JSONData objJSONData = new JSONData();
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            //int ShopID = ((FDSession)this.Session["FDSession"]).shopId;
            try
            {

                RS_Model_Master objRS_Model_Master = new RS_Model_Master();
                List<ModelMaster> objModelMaster = (List<ModelMaster>)JsonConvert.DeserializeObject(modeldata[0], typeof(List<ModelMaster>));
                string Model_code = objModelMaster[0].Model_Code;
                string OMConfigID = objModelMaster[0].OMconfig_ID;
                string biwpartno = objModelMaster[0].BIW_PartNo;

                //RS_OM_Configuration shop = db.RS_OM_Configuration.Where(m => m.OMconfig_ID == OMConfigID).FirstOrDefault();


                objRS_Model_Master.Attribution_Parameters = objModelMaster[0].Attribution_parmeter;

                if (!(db.RS_Model_Master.Any(m => m.Model_Code == Model_code && m.Plant_ID == plantID)))
                {

                    objJSONData.status = false;
                    objJSONData.message = "Model master not found!...";
                    objJSONData.type = "Errror";
                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    RS_Model_Master obj_RS_Model_Master_Edit = db.RS_Model_Master.Where(m => m.Model_Code == Model_code && m.Plant_ID == plantID).FirstOrDefault();
                    if(biwpartno != "")
                    {
                        var stylecodeid = db.RS_BIW_Part_Master.Where(m => m.Variant_Code.Equals(biwpartno, StringComparison.CurrentCultureIgnoreCase) && m.Plant_ID == plantID).Select(m => m.StyleCode_ID).FirstOrDefault();
                        var stylecode = db.RS_Style_Code.Find(stylecodeid).Style_Code;
                        obj_RS_Model_Master_Edit.Style_Code = stylecode;
                    }
                   
                    obj_RS_Model_Master_Edit.Attribution_Parameters = dataModal;
                    obj_RS_Model_Master_Edit.Model_Code = objModelMaster[0].Model_Code;
                    obj_RS_Model_Master_Edit.VIN_Code = objModelMaster[0].Vin_Code;
                    obj_RS_Model_Master_Edit.Model_Description = objModelMaster[0].Model_Description;
                    obj_RS_Model_Master_Edit.Auto_Remarks = objModelMaster[0].Auto_Remarks;
                    obj_RS_Model_Master_Edit.In_Use = objModelMaster[0].In_Use;
                    obj_RS_Model_Master_Edit.OMconfig_ID = objModelMaster[0].OMconfig_ID;
                    obj_RS_Model_Master_Edit.Config_ID = objModelMaster[0].Config_ID;
                    obj_RS_Model_Master_Edit.Model_Attribute_ID = objModelMaster[0].Model_Attribute_ID;
                    //objRS_Model_Master.Family = "NULL";


                    // Added by Ajay 07032019
                    obj_RS_Model_Master_Edit.BIW_Part_No = null;
                    obj_RS_Model_Master_Edit.BIW_Description = null;
                    obj_RS_Model_Master_Edit.Engine_PartNo = null;
                    obj_RS_Model_Master_Edit.Engine_Part_Desc = null;
                    obj_RS_Model_Master_Edit.TA_PartNo = null;
                    obj_RS_Model_Master_Edit.TA_Part_Desc = null;
                    obj_RS_Model_Master_Edit.FA_PartNo = null;
                    obj_RS_Model_Master_Edit.FA_Part_Desc = null;
                    obj_RS_Model_Master_Edit.RA_PartNo = null;
                    obj_RS_Model_Master_Edit.RA_PartDesc = null;
                    obj_RS_Model_Master_Edit.Chasis_PartNo = null;
                    obj_RS_Model_Master_Edit.Chasis_PartDesc = null;

                    obj_RS_Model_Master_Edit.BIW_Part_No = objModelMaster[0].BIW_PartNo;
                    obj_RS_Model_Master_Edit.BIW_Description = objModelMaster[0].BIW_Desription;
                    obj_RS_Model_Master_Edit.Engine_PartNo = objModelMaster[0].Engine_PartNo;
                    obj_RS_Model_Master_Edit.Engine_Part_Desc = objModelMaster[0].Engine_Part_Desc;
                    obj_RS_Model_Master_Edit.TA_PartNo = objModelMaster[0].TA_PartNo;
                    obj_RS_Model_Master_Edit.TA_Part_Desc = objModelMaster[0].TA_Part_Desc;
                    obj_RS_Model_Master_Edit.FA_PartNo = objModelMaster[0].FA_PartNo;
                    obj_RS_Model_Master_Edit.FA_Part_Desc = objModelMaster[0].FA_Part_Desc;
                    obj_RS_Model_Master_Edit.RA_PartNo = objModelMaster[0].RA_PartNo;
                    obj_RS_Model_Master_Edit.RA_PartDesc = objModelMaster[0].RA_PartDesc;
                    obj_RS_Model_Master_Edit.Chasis_PartNo = objModelMaster[0].Chasis_PartNo;
                    obj_RS_Model_Master_Edit.Chasis_PartDesc = objModelMaster[0].Chasis_PartDesc;
                    // Added by Ajay 07032019


                    obj_RS_Model_Master_Edit.Tyre_Make_Size_Front = null;//"NULL";
                    obj_RS_Model_Master_Edit.Tyre_Make_Size_Rear = null;//"NULL";

                    //obj_RS_Model_Master_Edit.Platform_Id = "1";
                    obj_RS_Model_Master_Edit.Platform_Id = objModelMaster[0].Platform_Id;

                    obj_RS_Model_Master_Edit.Plant_ID = plantID;
                    obj_RS_Model_Master_Edit.Updated_Date = DateTime.Now;
                    obj_RS_Model_Master_Edit.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //obj_RS_Model_Master_Edit.Shop_ID = shop.Shop_ID;
                    obj_RS_Model_Master_Edit.Color_Code = objModelMaster[0].Color_Code;
                    obj_RS_Model_Master_Edit.Colour_ID = objModelMaster[0].Colour_ID;
                    obj_RS_Model_Master_Edit.Is_Colour_Applicable = objModelMaster[0].Is_Colour_Applicable;

                    obj_RS_Model_Master_Edit.Is_Spare = objModelMaster[0].Is_Spare;

                    obj_RS_Model_Master_Edit.Paint_Line = objModelMaster[0].Paint_Line;
                    obj_RS_Model_Master_Edit.Assembly_Line = objModelMaster[0].Assembly_Line;
                    obj_RS_Model_Master_Edit.Body_Line = objModelMaster[0].Body_Line;
                    obj_RS_Model_Master_Edit.Shop_ID = objModelMaster[0].Shop_ID;

                    db.Entry(obj_RS_Model_Master_Edit).State = EntityState.Modified;
                    db.SaveChanges();

                    // Add records in 
                    RS_ModelCode_Country_Mapping mcObj = new RS_ModelCode_Country_Mapping();
                    var modalCountryData = (from country in db.RS_ModelCode_Country_Mapping
                                           where country.Model_ID == obj_RS_Model_Master_Edit.Model_ID
                                           select country).ToList();
                    foreach(var Country in modalCountryData)
                    {
                        db.RS_ModelCode_Country_Mapping.Remove(Country);
                        db.SaveChanges();
                    }

                    foreach(var item in objModelMaster[0].Countries)
                    {
                        mcObj.Country_ID = item;
                        mcObj.Inserted_Date = DateTime.Now;
                        mcObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        mcObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mcObj.Model_ID = obj_RS_Model_Master_Edit.Model_ID;
                        db.RS_ModelCode_Country_Mapping.Add(mcObj);
                        db.SaveChanges();
                    }

                    RS_Model_Child_Part_Mapping mmcobj = new RS_Model_Child_Part_Mapping();
                    var modalChildPartItem = from part in db.RS_Model_Child_Part_Mapping
                                        where (part.Model_ID == obj_RS_Model_Master_Edit.Model_ID)
                                        select part;
                    foreach (var items in modalChildPartItem.ToList())
                    {
                        db.RS_Model_Child_Part_Mapping.Remove(items);
                        db.SaveChanges();
                    }

                   // foreach (var id in objModelMaster[0].Sub_Assembly_ID)
                    {
                        mmcobj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmcobj.Inserted_Date = DateTime.Now;
                        mmcobj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        mmcobj.Model_ID = Convert.ToDecimal(obj_RS_Model_Master_Edit.Model_ID);
                        mmcobj.Sub_Assembly_ID = Convert.ToDecimal(objModelMaster[0].Sub_Assembly_ID);
                        var id = objModelMaster[0].Sub_Assembly_ID;
                        if (id == 1)
                        {
                            mmcobj.Part_No = objModelMaster[0].BIW_PartNo;
                            mmcobj.Part_Description = objModelMaster[0].BIW_Desription;
                        }
                        if (id == 2)
                        {
                            mmcobj.Part_No = objModelMaster[0].Engine_PartNo;
                            mmcobj.Part_Description = objModelMaster[0].Engine_Part_Desc;
                        }
                        if (id == 3)
                        {
                            mmcobj.Part_No = objModelMaster[0].TA_PartNo;
                            mmcobj.Part_Description = objModelMaster[0].TA_Part_Desc;
                        }
                        if (id == 4)
                        {
                            mmcobj.Part_No = objModelMaster[0].FA_PartNo;
                            mmcobj.Part_Description = objModelMaster[0].FA_Part_Desc;
                        }
                        if (id == 5)
                        {
                            mmcobj.Part_No = objModelMaster[0].RA_PartNo;
                            mmcobj.Part_Description = objModelMaster[0].RA_PartDesc;
                        }
                        if (id == 6)
                        {
                            mmcobj.Part_No = objModelMaster[0].Chasis_PartNo;
                            mmcobj.Part_Description = objModelMaster[0].Chasis_PartDesc;
                        }

                        db.RS_Model_Child_Part_Mapping.Add(mmcobj);
                        db.SaveChanges();
                    }



                    //globalData.isSuccessMessage = true;
                    //globalData.messageTitle = ResourceModules.Model_Master;
                    //globalData.messageDetail = ResourceModules.Model_Master + " " + ResourceMessages.Edit_Success;
                    //TempData["globalData"] = globalData;
                    objJSONData.Id = obj_RS_Model_Master_Edit.Model_ID;
                    objJSONData.message = "Model Master Edited successfully!...";
                    objJSONData.type = "Success";
                    objJSONData.status = true;
                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }

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
                objJSONData.message = "Error in saving Model Master!...";
                objJSONData.type = "fail";
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult ShowModelMasterData(string Model_Code)
        {
            JSONData objJSONData = new JSONData();
            try
            {
                //var data = db.RS_Model_Master.Where(m => m.Model_Code == Model_Code).FirstOrDefault();
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var data = db.RS_Model_Master.Where(m => m.Model_Code == Model_Code && m.Plant_ID == plantID).Select(m => new
                {
                    m.Model_Attribute_ID,
                    m.Attribution_Parameters,
                    m.Auto_Remarks,
                    m.BIW_Description,
                    m.BIW_Part_No,
                    m.Config_ID,
                    m.Model_Code,
                    m.Model_Description,
                    m.OMconfig_ID,
                    m.VIN_Code,
                    m.In_Use,
                    m.Is_Spare,
                    m.Color_Code,
                    m.Body_Line,
                    m.Paint_Line,
                    m.Assembly_Line,
                    m.Platform_Id,
                    m.Shop_ID,
                    m.Engine_PartNo,
                    m.Engine_Part_Desc,
                    m.TA_PartNo,
                    m.TA_Part_Desc,
                    m.FA_PartNo,
                    m.FA_Part_Desc,
                    m.RA_PartNo,
                    m.RA_PartDesc,
                    m.Chasis_PartNo,
                    m.Chasis_PartDesc
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
                objJSONData.message = "Erro in saving Model Master!...";
                objJSONData.type = "fail";
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult GetChildPartMasterData(int Shop_ID)
        {
            var ChildPart = (from s in db.RS_Shops
                             join
       m in db.RS_Major_Sub_Assembly on
       s.Sub_Assembly_ID equals m.Sub_Assembly_ID
                             where s.Shop_ID == Shop_ID
                             select new
                             {
                                 id = s.Sub_Assembly_ID,
                                 value = m.Sub_Assembly_Name
                             }).ToList();

            return Json(ChildPart, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlatformByLine(decimal LineID)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var Platform = (from platform in db.RS_OM_Platform
                            join line in db.RS_Lines
                       on platform.Line_ID equals line.Line_ID
                            where platform.Line_ID == LineID && platform.Plant_ID == plantID
                            select new
                            {
                                Platform_ID = platform.Platform_ID,
                                Platform_Name = platform.Platform_Name
                            });

            return Json(Platform, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlatformByPlatformID(decimal Platform_ID)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var Platform = (from platform in db.RS_OM_Platform
                            where platform.Platform_ID == Platform_ID && platform.Plant_ID == plantID
                            select new
                            {
                                Platform_ID = platform.Platform_ID,
                                Platform_Name = platform.Platform_Name
                            });

            return Json(Platform, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBIWDescription(string BIW_Part_No)
        {
            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            //var Platform = (from BIWPart in db.RS_BIW_Part_Master
            //                where BIWPart.Variant_Code == BIW_Part_No
            //                select new
            //                {
            //                    BIW_Description = BIWPart.VARIANT_DESC
            //                });
            var data = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == BIW_Part_No && m.Plant_ID == plantID).Select(m => new
            {
                m.Attribution_Parameters,
                m.Shop_ID,
                m.Platform_ID,
                m.Sub_Assembly_ID,
                m.Variant_Code,
                m.VARIANT_DESC,
                m.LONG_DESC
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEngineDescription(string Engine_Part_No)
        {
            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            //var Platform = (from EnginePart in db.RS_BIW_Part_Master
            //                where EnginePart.Variant_Code == Engine_Part_No
            //                select new
            //                {
            //                    Engine_Description = EnginePart.VARIANT_DESC
            //                });
            var data = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == Engine_Part_No && m.Plant_ID == plantID).Select(m => new
            {
                m.Attribution_Parameters,
                m.Shop_ID,
                m.Platform_ID,
                m.Sub_Assembly_ID,
                m.Variant_Code,
                m.VARIANT_DESC,
                m.LONG_DESC
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTADescription(string TA_Part_No)
        {
            //var Platform = (from TAPart in db.RS_BIW_Part_Master
            //                where TAPart.Variant_Code == TA_Part_No
            //                select new
            //                {
            //                    TA_Description = TAPart.VARIANT_DESC
            //                });
            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var data = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == TA_Part_No && m.Plant_ID == plantID).Select(m => new
            {
                m.Attribution_Parameters,
                m.Shop_ID,
                m.Platform_ID,
                m.Sub_Assembly_ID,
                m.Variant_Code,
                m.VARIANT_DESC,
                m.LONG_DESC
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFADescription(string FA_Part_No)
        {
            //var Platform = (from FAPart in db.RS_BIW_Part_Master
            //                where FAPart.Variant_Code == FA_Part_No
            //                select new
            //                {
            //                    FA_Description = FAPart.VARIANT_DESC
            //                });

            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var data = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == FA_Part_No && m.Plant_ID == plantID).Select(m => new
            {
                m.Attribution_Parameters,
                m.Shop_ID,
                m.Platform_ID,
                m.Sub_Assembly_ID,
                m.Variant_Code,
                m.VARIANT_DESC,
                m.LONG_DESC
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRADescription(string RA_Part_No)
        {
            //var Platform = (from RAPart in db.RS_BIW_Part_Master
            //                where RAPart.Variant_Code == RA_Part_No
            //                select new
            //                {
            //                    RA_Description = RAPart.VARIANT_DESC
            //                });

            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var data = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == RA_Part_No && m.Plant_ID == plantID).Select(m => new
            {
                m.Attribution_Parameters,
                m.Shop_ID,
                m.Platform_ID,
                m.Sub_Assembly_ID,
                m.Variant_Code,
                m.VARIANT_DESC,
                m.LONG_DESC
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChasisDescription(string Chasis_Part_No)
        {
            //var Platform = (from ChPart in db.RS_BIW_Part_Master
            //                where ChPart.Variant_Code == Chasis_Part_No
            //                select new
            //                {
            //                    Chasis_Description = ChPart.VARIANT_DESC
            //                });

            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var data = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == Chasis_Part_No && m.Plant_ID == plantID).Select(m => new
            {
                m.Attribution_Parameters,
                m.Shop_ID,
                m.Platform_ID,
                m.Sub_Assembly_ID,
                m.Variant_Code,
                m.VARIANT_DESC,
                m.LONG_DESC
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMachineDescription(string Machine_Part_No)
        {
            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var data = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == Machine_Part_No && m.Plant_ID == plantID).Select(m => new
            {
                m.Attribution_Parameters,
                m.Shop_ID,
                m.Platform_ID,
                m.Sub_Assembly_ID,
                m.Variant_Code,
                m.VARIANT_DESC,
                m.LONG_DESC
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAttributeData(int PlatformID,int ShopID, int LineID)
        {
            var attribute = (from attr in db.RS_Model_Attribute_Master
                             where attr.Platform_ID == PlatformID && attr.Shop_ID == ShopID && attr.Line_ID == LineID
                             select new
                             {
                                 Id = attr.Model_Attribute_ID,
                                 Value = attr.Attribution
                             }
                         ).ToList();
            return Json(attribute, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0)]
        public ActionResult fillCountriesDropdown(decimal ModelID)
        {
            var countryObj = from a in db.RS_Country
                          where !(from b in db.RS_ModelCode_Country_Mapping where b.Model_ID == ModelID select b.Country_ID).Contains(a.Country_ID)
                          select new
                          {
                              Country_ID = a.Country_ID,
                              Country_Name = a.Country_Name
                          };

            return Json(countryObj, JsonRequestBehavior.AllowGet);
        }

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
                    var modelcode = Convert.ToString(Request["modelcode"]);
                    var plantID = Convert.ToInt32(Request["plantID"]);
                    // var partNo = Request["partNo"];

                    userId = Convert.ToInt16(((FDSession)this.Session["FDSession"]).userId);
                    HttpPostedFileBase file = files[0];
                    var ID = Convert.ToInt32(Request["Id"]);
                    fileName = Path.GetFileName(file.FileName);
                    fileExtension = Path.GetExtension(file.FileName);
                    var fileContent = file.ContentLength;

                    if (file != null && file.ContentLength > 0)
                    {
                        if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == "jpeg")
                        {
                            using (var reader = new System.IO.BinaryReader(file.InputStream))
                            {
                                var result = db.RS_Model_Master_Image.Where(m => m.Model_ID == ID).Select(m => m.Model_ID).ToList();
                                if (result.Count > 0)
                                {
                                    var imageId = db.RS_Model_Master_Image.Where(m => m.Model_ID == ID).Select(m => m.Image_ID).FirstOrDefault();
                                    RS_Model_Master_Image imageObj = db.RS_Model_Master_Image.Find(imageId);
                                    imageObj.Model_ID = ID;
                                    imageObj.Image_Content = reader.ReadBytes(file.ContentLength);
                                    imageObj.Image_Name = fileName;
                                    imageObj.Image_Type = Path.GetExtension(file.FileName);
                                    imageObj.Content_Type = file.ContentType;
                                    imageObj.Updated_Date = DateTime.Now;
                                    imageObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    imageObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    imageObj.Is_Edited = true;
                                    db.Entry(imageObj).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    RS_Model_Master_Image obj = new RS_Model_Master_Image();

                                    obj.Model_ID = ID;
                                    obj.Image_Content = reader.ReadBytes(file.ContentLength);
                                    obj.Image_Name = fileName;
                                    obj.Image_Type = Path.GetExtension(file.FileName);
                                    obj.Content_Type = file.ContentType;
                                    obj.Inserted_Date = DateTime.Now;
                                    obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    db.RS_Model_Master_Image.Add(obj);
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            globalData.isErrorMessage = true;
                            globalData.messageTitle = "Model Master";

                            globalData.messageDetail = "Image is not uploaded. Valid Image Format(.png|.jpg|.jpeg)";
                            TempData["globalData"] = globalData;
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = "Model Master";//ResourceQualityImageGroups.Quality_ImageGroups;

                        globalData.messageDetail = "Model Master Save Successfully but image is not uploaded";
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Create");
                    }
                    // width will increase the height proportionally
                    //ImageUpload imageUpload = new ImageUpload { Width = 2500 };
                    // mmQualityImageGroupObj.addRecordsInQualityImages(plantId, shopId, groupId, userId, ((FDSession)this.Session["FDSession"]).userHost);

                    // Returns message that successfully uploaded  

                    //to save using above parameters
                    //if (fileName == null)
                    //{
                    //    globalData.isErrorMessage = true;
                    //    globalData.messageTitle = "Model Master";//ResourceQualityImageGroups.Quality_ImageGroups;

                    //    globalData.messageDetail = "Please select image to upload";
                    //    TempData["globalData"] = globalData;
                    //    return RedirectToAction("Create");//RedirectToAction("Upload", new { id = groupId });
                    //}

                    //MM_Quality_Image_Group mmQualityImageGroupObj = new MM_Quality_Image_Group();
                    //userId = ((FDSession)this.Session["FDSession"]).userId;

                    //imageId = mmQualityImageGroupObj.getImageId(Convert.ToInt16(imageId), userId, ((FDSession)this.Session["FDSession"]).userHost, true);

                    //String fileNameExtension = file.FileName.Split('.')[1];
                    //var fileName = DateTime.Now.Ticks + "." + fileNameExtension;

                    //var path = Path.Combine(Server.MapPath("~/Content/ModelMaster"), modelcode+"_"+ plantID+ fileExtension);
                    //if(path != null) System.IO.File.Delete(path);
                    //path = Path.Combine(Server.MapPath("~/Content/ModelMaster"), modelcode + "_" + plantID + fileExtension);
                    //file.SaveAs(path);

                    // process to add image in quality image table

                    //RS_Model_Master RS_Model_Master = db.RS_Model_Master.Where(m => m.Model_Code.ToLower() == modelcode.ToLower() && m.Plant_ID == plantID).FirstOrDefault();
                    //if (RS_Model_Master != null)
                    //{
                    //    RS_Model_Master.Image_Name = modelcode + "_" + plantID + fileExtension;
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

        public ActionResult GetColorCode(bool Color_Code)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var st = (from color in db.RS_Colour
                      where color.Plant_ID == plantId
                      select new
                      {
                          id = color.Row_ID,
                          value = color.Colour_Desc
                      }).ToList().Distinct();
            return Json(st, JsonRequestBehavior.AllowGet);
        }
    }
}
