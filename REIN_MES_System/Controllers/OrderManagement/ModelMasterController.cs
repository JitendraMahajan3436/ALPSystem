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
using System.Data.Entity.Validation;
using System.Data.SqlClient;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /*               Controller Name           : ModelMasterController
     *               Description               : Controller used to Crete,update,delete and details of all model. 
     *               Author, Timestamp         : Jitendra Mahajan
     */
    public class ModelMasterController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();

        int plantId = 0, shopId = 0, Tyre_Make_ID = 0; long? config_id = null;
        string model_code, model_desc, conf_desc, plat_form, colourid, Tyre_Make_Size_front, Tyre_Make_Size_rear;
        decimal? family, model_type, variant, series_code;


        General generalObj = new General();

        // GET: Model_Master
        /*               Action Name               : Index
         *               Description               : Action used to show the list of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
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

        /*               Action Name               : Details
         *               Description               : Action used to show the list of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Model_Master/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Model_Master RS_Model_Master = db.RS_Model_Master.Find(id);
            ViewBag.TyreMake = new SelectList(db.RS_Model_Tyre_Make, "Tyre_Make_ID", "Make_Name", Tyre_Make_ID);
            if (RS_Model_Master == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Model_Master;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Model Master";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;


            return View(RS_Model_Master);
        }

        /*               Action Name               : GetPlantID
         *               Description               : Action used to Get the shop Id of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Shop 
        public ActionResult GetPlantID(int Plant_Id)
        {
            //var Shop_Id = db.RS_Shops
            //                           .Where(c => c.Plant_ID == Plant_Id)
            //                           .Select(c => new {c.Shop_ID, c.Shop_Name });

            var Shop_Id = (from u in db.RS_Shops
                           where u.Plant_ID == Plant_Id
                           orderby u.Shop_Name ascending
                           select new { u.Shop_ID, u.Shop_Name }).Distinct();

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
            //var Platform_Id = db.RS_Platform
            //                           .Where(c => c.Plant_ID == Plant_Id)
            //                           .Select(c => new { c.Platform_Id, c.Platform_Description });

            var Platform_Id = (from u in db.RS_Platform
                               where u.Plant_ID == Plant_Id
                               orderby u.Platform_Description ascending
                               select new { u.Platform_Id, u.Platform_Description }).Distinct();

            return Json(Platform_Id, JsonRequestBehavior.AllowGet);
        }
        /*               Action Name               : getSerialNoConfigByShopID
        *               Description               : get Serial_number configuration depends upon shop id
        *               Author, Timestamp         : Jitendra Mahajan
        *               Input parameter           : shopid
        *               Return Type               : ActionResult
        *               Revision                  : 1
        */
        public ActionResult getSerialNoConfigByShopID(int ShopID)
        {
            var serialNo = (from u in db.RS_Serial_Number_Configuration
                            where u.Shop_ID == ShopID

                            select new { u.Config_ID, u.Display_Name }).Distinct();

            return Json(serialNo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getModelTypeByShopID(int ShopID)
        {
            var Shop_Id = (from modeltype in db.RS_Attribution_Parameters
                           where modeltype.Shop_ID == ShopID

                           select new { modeltype.Attribute_ID, modeltype.Attribute_Desc }).Distinct();

            return Json(Shop_Id, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getVarientByShopID(int ShopID)
        {
            var Shop_Id = (from v in db.RS_Attribution_Parameters
                           where v.Shop_ID == ShopID && v.Attribute_Type == "varient"

                           select new { v.Attribute_ID, v.Attribute_Desc }).Distinct();

            return Json(Shop_Id, JsonRequestBehavior.AllowGet);
        }


        /*               Action Name               : GetConfigueData
         *               Description               : Action used to Get the Configuartion Data of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Plant_id,Shop_id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Configuaration List
        public ActionResult GetConfigueData(int Plant_id, int Shop_id)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var configue_Data = db.RS_OM_Configuration
                .Where(c => c.Plant_ID == Plant_id && c.Shop_ID == Shop_id)
                .Select(c => new { c.OMconfig_ID, c.OMconfig_Desc })
                .Distinct()
                .OrderBy(c => c.OMconfig_Desc);
            return Json(configue_Data, JsonRequestBehavior.AllowGet);
        }

        /*               Action Name               : GetSeriesCode
         *               Description               : Action used to return the list of Series ID  for Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Plant_Id,shopid
         *               Return Type               : ActionResult
         *               Revision                  : 1
         */
        //Find Series
        public ActionResult GetSeriesCode(int Plant_Id, int Shop_Id)
        {
            var Series = db.RS_Series
                                          .Where(c => c.Plant_ID == Plant_Id && c.Shop_ID == Shop_Id)
                                          .Select(c => new { c.Series_Code, c.Series_Description });
            return Json(Series, JsonRequestBehavior.AllowGet);
        }

        /*               Action Name               : Create
         *               Description               : Action used to display Create of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Model_Master/Create
        public ActionResult Create()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Model_Master;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Model Master";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;


            var omconfig = db.RS_OM_Configuration
                           .Select(a => new { a.OMconfig_ID, a.OMconfig_Desc })
                           .Distinct()
                           .OrderBy(a => a.OMconfig_Desc);

            ViewBag.Configue = new SelectList(omconfig, "OMconfig_ID", "OMconfig_Desc");
            ViewBag.Serial_No_config = new SelectList(db.RS_Serial_Number_Configuration.OrderBy(x => x.Display_Name), "Config_ID", "Display_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", plantID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
            ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc");

            ViewBag.Model_type = new SelectList(db.RS_Attribution_Parameters.Where(p => p.Attribute_Type == "Model"), "Attribute_ID", "Attribute_Desc");
            ViewBag.Variant = new SelectList(db.RS_Attribution_Parameters.Where(p => p.Attribute_Type == "Varient"), "Attribute_ID", "Attribute_Desc");
            ViewBag.Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description");
            //ViewBag.Configue = new SelectList(db.RS_OM_Configuration.Where(p => p.Plant_ID == ), "", "");
            ViewBag.Platform_Id = new SelectList(db.RS_Platform, "Platform_Id", "Platform_Description");

            //Added by Amol Bodkhe 25 March
            ViewBag.TyreMake = new SelectList(db.RS_Model_Tyre_Make, "Tyre_Make_ID", "Make_Name");

            return View();
        }

        /*               Action Name               : Create
         *               Description               : Action used to display Create of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : RS_Model_Master
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // POST: Model_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       


        /*               Action Name               : Edit
         *               Description               : Action used to display Edit functionality of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Model_Master/Edit/5
        public ActionResult Edit(string id)
        {
            int plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            decimal edit_ID = Convert.ToDecimal(id);
            RS_Model_Master RS_Model_Master = db.RS_Model_Master.Find(edit_ID);
            if (RS_Model_Master == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Model_Master;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Model Master";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == RS_Model_Master.Shop_ID).SingleOrDefault();
            ViewBag.TyreMake = new SelectList(db.RS_Model_Tyre_Make, "Tyre_Make_ID", "Make_Name");
            plantId = Convert.ToInt16(shopObj.Plant_ID);
            RS_Model_Master.Plant_ID = plant_ID;

            #region Attribute
            RS_Model_Master.Varient1 = db.RS_Attribution_Parameters.Where(a => a.Attribute_ID == RS_Model_Master.Varient && a.Plant_ID == plant_ID).Select(a => a.Attribute_Desc).FirstOrDefault();
            RS_Model_Master.Family1 = db.RS_Attribution_Parameters.Where(a => a.Attribute_ID == RS_Model_Master.Family && a.Plant_ID == plant_ID).Select(a => a.Attribute_Desc).FirstOrDefault();
            RS_Model_Master.Model_Type1 = db.RS_Attribution_Parameters.Where(a => a.Attribute_ID == RS_Model_Master.Model_Type && a.Plant_ID == plant_ID).Select(a => a.Attribute_Desc).FirstOrDefault();
            RS_Model_Master.Series_Description = db.RS_Series.Where(s => s.Series_Code == RS_Model_Master.Series_Code && s.Plant_ID == plant_ID).Select(s => s.Series_Description).FirstOrDefault();
            #endregion

            ViewBag.OMconfig_ID = new SelectList(db.RS_OM_Configuration.Where(p => p.Plant_ID == plant_ID && p.Shop_ID == RS_Model_Master.Shop_ID).Select(a => new { a.OMconfig_ID, a.OMconfig_Desc })
                           .Distinct()
                           .OrderBy(a => a.OMconfig_Desc), "OMconfig_ID", "OMconfig_Desc", RS_Model_Master.OMconfig_ID);
            ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration.Where(snc => snc.Shop_ID == RS_Model_Master.Shop_ID), "Config_ID", "Display_Name", RS_Model_Master.Config_ID);

            ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc", RS_Model_Master.Colour_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Model_Master.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Model_Master.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plant_ID), "Plant_ID", "Plant_Name", RS_Model_Master.Plant_ID);

            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", RS_Model_Master.Shop_ID);
            ViewBag.Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description", RS_Model_Master.Series_Code);
            ViewBag.Platform_Id = new SelectList(db.RS_Platform.Where(p => p.Plant_ID == plant_ID), "Platform_Id", "Platform_Description", RS_Model_Master.Platform_Id);

            return View(RS_Model_Master);
        }

        /*               Action Name               : Edit
         *               Description               : Action used to display Edit of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : RS_Model_Master
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // POST: Model_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit(RS_Model_Master RS_Model_Master, string hSeries_Discription, string hFamily, string hVarient, string hModel_Type)
        //{
        //    int plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
        //    //if (ModelState.IsValid)
        //    //{
        //    try
        //    {
        //        shopId = Convert.ToInt16(RS_Model_Master.Shop_ID);
        //        plantId = Convert.ToInt16(RS_Model_Master.Plant_ID);
        //        model_desc = RS_Model_Master.Model_Description;
        //        conf_desc = RS_Model_Master.OMconfig_ID;
        //        family = Convert.ToDecimal(RS_Model_Master.Family);
        //        model_type = Convert.ToDecimal(RS_Model_Master.Model_Type);
        //        variant = Convert.ToDecimal(RS_Model_Master.Varient);
        //        series_code = RS_Model_Master.Series_Code;
        //        plat_form = RS_Model_Master.Platform_Id;
        //        model_code = RS_Model_Master.Model_Code;
        //        config_id = RS_Model_Master.Config_ID;
        //        colourid = RS_Model_Master.Colour_ID.ToString();
        //        //added by Jitendra Mahajan 29/03/2017

        //        Tyre_Make_Size_front = RS_Model_Master.Tyre_Make_Size_Front;
        //        Tyre_Make_Size_rear = RS_Model_Master.Tyre_Make_Size_Rear;
        //        Tyre_Make_ID = Convert.ToInt32(RS_Model_Master.Tyre_Make_ID);

        //        #region Series
        //        decimal Series_Code = RS_Model_Master.Series_Code.Value;
        //        if (hSeries_Discription == "" || hSeries_Discription == null)
        //        {
        //            RS_Model_Master.Series_Code = db.RS_Series.Where(s => s.Series_Description == RS_Model_Master.Series_Description && s.Plant_ID == plantId).Select(s => s.Series_Code).FirstOrDefault();
        //            if (RS_Model_Master.Series_Code.ToString() == "0" || RS_Model_Master.Series_Code.ToString() == null)
        //            {
        //                RS_Series objRS_Series = new RS_Series();
        //                //objRS_Series = db.RS_Series.Find(Series_Code);
        //                //objRS_Series.Series_Code = Series_Code;
        //                //objRS_Series.Series_Description = RS_Model_Master.Series_Discription;
        //                //objRS_Series.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
        //                //objRS_Series.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                //objRS_Series.Updated_Date = DateTime.Now;
        //                //db.Entry(objRS_Series).State = EntityState.Modified;
        //                //db.SaveChanges();
        //                objRS_Series.Shop_ID = RS_Model_Master.Shop_ID;
        //                objRS_Series.Series_Description = RS_Model_Master.Series_Description.ToUpper();
        //                objRS_Series.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
        //                objRS_Series.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                objRS_Series.Inserted_Date = DateTime.Now;
        //                objRS_Series.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                objRS_Series.Updated_Date = DateTime.Now;
        //                db.RS_Series.Add(objRS_Series);
        //                db.SaveChanges();
        //                RS_Model_Master.Series_Code = objRS_Series.Series_Code;
        //            }

        //        }
        //        else
        //        {
        //            RS_Model_Master.Series_Code = Convert.ToDecimal(hSeries_Discription);
        //        }

        //        #endregion

        //        #region Family
        //        if (hFamily == "" || hFamily == null)
        //        {
        //            decimal family = RS_Model_Master.Family.Value;
        //            RS_Model_Master.Family = db.RS_Attribution_Parameters.Where(s => s.Attribute_Desc == RS_Model_Master.Family1 && s.Plant_ID == plantId).Select(a => a.Attribute_ID).FirstOrDefault();
        //        }
        //        if (RS_Model_Master.Family.ToString() == "0")
        //        {
        //            RS_Attribution_Parameters objRS_Attribution_Parameters = new RS_Attribution_Parameters();
        //            //objRS_Attribution_Parameters = db.RS_Attribution_Parameters.Find(family);
        //            //objRS_Attribution_Parameters.Attribute_Type = "Family";
        //            //objRS_Attribution_Parameters.Shop_ID = RS_Model_Master.Shop_ID;
        //            //objRS_Attribution_Parameters.Attribute_Desc = RS_Model_Master.Family1.ToUpper();
        //            ////objRS_Attribution_Parameters.Inserted_Date = DateTime.Now;
        //            ////objRS_Attribution_Parameters.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            //objRS_Attribution_Parameters.Updated_Date = DateTime.Now;
        //            //objRS_Attribution_Parameters.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            //objRS_Attribution_Parameters.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
        //            //db.Entry(objRS_Attribution_Parameters).State = EntityState.Modified;
        //            //db.SaveChanges();
        //            objRS_Attribution_Parameters.Attribute_Type = "Family";
        //            objRS_Attribution_Parameters.Shop_ID = RS_Model_Master.Shop_ID;
        //            objRS_Attribution_Parameters.Attribute_Desc = RS_Model_Master.Family1.ToUpper();
        //            objRS_Attribution_Parameters.Inserted_Date = DateTime.Now;
        //            objRS_Attribution_Parameters.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            objRS_Attribution_Parameters.Updated_Date = DateTime.Now;
        //            objRS_Attribution_Parameters.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            objRS_Attribution_Parameters.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
        //            db.RS_Attribution_Parameters.Add(objRS_Attribution_Parameters);
        //            db.SaveChanges();
        //            RS_Model_Master.Family = objRS_Attribution_Parameters.Attribute_ID;
        //        }


        //        else
        //        {
        //            // RS_Model_Master.Family = Convert.ToDecimal(hFamily);
        //        }
        //        #endregion

        //        #region Varient
        //        decimal varient = RS_Model_Master.Varient.Value;
        //        if (hVarient == "" || hVarient == null)
        //        {
        //            RS_Model_Master.Varient = db.RS_Attribution_Parameters.Where(s => s.Attribute_Desc == RS_Model_Master.Varient1 && s.Plant_ID == plantId).Select(a => a.Attribute_ID).FirstOrDefault();
        //        }
        //        if (RS_Model_Master.Varient.ToString() == "0")
        //        {
        //            RS_Attribution_Parameters objRS_Attribution_Parameters = new RS_Attribution_Parameters();
        //            //objRS_Attribution_Parameters = db.RS_Attribution_Parameters.Find(varient);
        //            //objRS_Attribution_Parameters.Attribute_Type = "Varient";
        //            //objRS_Attribution_Parameters.Attribute_Desc = RS_Model_Master.Varient1.ToUpper();
        //            //objRS_Attribution_Parameters.Shop_ID = RS_Model_Master.Shop_ID;
        //            //objRS_Attribution_Parameters.Updated_Date = DateTime.Now;
        //            //objRS_Attribution_Parameters.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            //objRS_Attribution_Parameters.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
        //            //db.Entry(objRS_Attribution_Parameters).State = EntityState.Modified;
        //            //db.SaveChanges();
        //            objRS_Attribution_Parameters.Attribute_Type = "Varient";
        //            objRS_Attribution_Parameters.Attribute_Desc = RS_Model_Master.Varient1.ToUpper();
        //            objRS_Attribution_Parameters.Shop_ID = RS_Model_Master.Shop_ID;
        //            objRS_Attribution_Parameters.Inserted_Date = DateTime.Now;
        //            objRS_Attribution_Parameters.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            objRS_Attribution_Parameters.Updated_Date = DateTime.Now;
        //            objRS_Attribution_Parameters.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            objRS_Attribution_Parameters.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
        //            db.RS_Attribution_Parameters.Add(objRS_Attribution_Parameters);
        //            db.SaveChanges();
        //            RS_Model_Master.Varient = objRS_Attribution_Parameters.Attribute_ID;
        //        }

        //        else
        //        {
        //            // RS_Model_Master.Varient = Convert.ToDecimal(hVarient);
        //        }

        //        #endregion

        //        #region Model_Type
        //        decimal Model_Type = RS_Model_Master.Model_Type.Value;
        //        if (hModel_Type == "" || hModel_Type == null)
        //        {

        //            RS_Model_Master.Model_Type = db.RS_Attribution_Parameters.Where(s => s.Attribute_Desc == RS_Model_Master.Model_Type1 && s.Plant_ID == plantId).Select(a => a.Attribute_ID).FirstOrDefault();
        //        }
        //        if (RS_Model_Master.Model_Type.ToString() == "0")
        //        {
        //            RS_Attribution_Parameters objRS_Attribution_Parameters = new RS_Attribution_Parameters();
        //            //objRS_Attribution_Parameters = db.RS_Attribution_Parameters.Find(Model_Type);
        //            //objRS_Attribution_Parameters.Attribute_Type = "Model";
        //            //objRS_Attribution_Parameters.Shop_ID = RS_Model_Master.Shop_ID;
        //            //objRS_Attribution_Parameters.Attribute_Desc = RS_Model_Master.Model_Type1.ToUpper();
        //            //objRS_Attribution_Parameters.Updated_Date = DateTime.Now;
        //            //objRS_Attribution_Parameters.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            //objRS_Attribution_Parameters.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
        //            //db.Entry(objRS_Attribution_Parameters).State = EntityState.Modified;
        //            //db.SaveChanges();
        //            objRS_Attribution_Parameters.Attribute_Type = "Model";
        //            objRS_Attribution_Parameters.Shop_ID = RS_Model_Master.Shop_ID;
        //            objRS_Attribution_Parameters.Attribute_Desc = RS_Model_Master.Model_Type1.ToUpper();
        //            objRS_Attribution_Parameters.Inserted_Date = DateTime.Now;
        //            objRS_Attribution_Parameters.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            objRS_Attribution_Parameters.Updated_Date = DateTime.Now;
        //            objRS_Attribution_Parameters.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //            objRS_Attribution_Parameters.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
        //            db.RS_Attribution_Parameters.Add(objRS_Attribution_Parameters);
        //            db.SaveChanges();
        //            RS_Model_Master.Model_Type = objRS_Attribution_Parameters.Attribute_ID;
        //        }

        //        else
        //        {
        //            // RS_Model_Master.Model_Type = Convert.ToDecimal(hModel_Type);
        //        }

        //        #endregion



        //        RS_Model_Master.Platform_Id = "1";
        //        RS_Model_Master.Config_ID = config_id;
        //        if (colourid != "" && colourid != null)
        //        {
        //            RS_Model_Master.Colour_ID = Convert.ToUInt32(colourid);
        //        }
        //        RS_Model_Master.Inserted_Date = DateTime.Now;
        //        RS_Model_Master.Updated_Date = DateTime.Now;
        //        RS_Model_Master.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //        db.Entry(RS_Model_Master).State = EntityState.Modified;
        //        db.SaveChanges();
        //        globalData.isSuccessMessage = true;
        //        globalData.messageTitle = ResourceModules.Model_Master;
        //        globalData.messageDetail = ResourceModules.Model_Master + " " + ResourceMessages.Edit_Success;
        //        TempData["globalData"] = globalData;
        //        return RedirectToAction("Index");

        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        // Retrieve the error messages as a list of strings.
        //        var errorMessages = ex.EntityValidationErrors
        //                .SelectMany(x => x.ValidationErrors)
        //                .Select(x => x.ErrorMessage);

        //        // Join the list to a single string.
        //        var fullErrorMessage = string.Join("; ", errorMessages);
        //        string msg = ex.Message;
        //        globalData.messageTitle = ResourceModules.Model_Master;
        //        globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.Model_Master;

        //    }

        //    ViewBag.Config_ID = new SelectList(db.RS_Serial_Number_Configuration.OrderBy(x => x.Display_Name), "Config_ID", "Display_Name", RS_Model_Master.Config_ID);
        //    ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Model_Master.Plant_ID);
        //    ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", RS_Model_Master.Shop_ID);
        //    ViewBag.Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description", RS_Model_Master.Series_Code);
        //    ViewBag.Platform_Id = new SelectList(db.RS_Platform, "Platform_Id", "Platform_Description", RS_Model_Master.Platform_Id);
        //    //ViewBag.OMconfig_ID = new SelectList(db.RS_OM_Configuration.Select(a => new { a.OMconfig_ID, a.OMconfig_Desc })
        //    //                .Distinct()
        //    //                .OrderBy(a => a.OMconfig_Desc), "OMconfig_ID", "OMconfig_Desc", RS_Model_Master.OMconfig_ID);
        //    ViewBag.OMconfig_ID = new SelectList(db.RS_OM_Configuration.Where(p => p.Plant_ID == plant_ID).Select(a => new { a.OMconfig_ID, a.OMconfig_Desc })
        //                 .Distinct().OrderBy(a => a.OMconfig_Desc), "OMconfig_ID", "OMconfig_Desc", RS_Model_Master.OMconfig_ID);
        //    ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc", RS_Model_Master.Colour_ID);
        //    ViewBag.Family = new SelectList(db.RS_Attribution_Parameters.Where(p => p.Attribute_Type == "Family" && p.Shop_ID == RS_Model_Master.Shop_ID), "Attribute_ID", "Attribute_Desc", RS_Model_Master.Family);
        //    ViewBag.Model_type = new SelectList(db.RS_Attribution_Parameters.Where(p => p.Attribute_Type == "Model"), "Attribute_ID", "Attribute_Desc", RS_Model_Master.Model_Type);
        //    ViewBag.Varient = new SelectList(db.RS_Attribution_Parameters.Where(p => p.Attribute_Type == "Variant"), "Attribute_ID", "Attribute_Desc", RS_Model_Master.Varient);
        //    ViewBag.TyreMake = new SelectList(db.RS_Model_Tyre_Make.Where(xp => xp.Tyre_Make_ID == Tyre_Make_ID), "Tyre_Make_ID", "Make_Name", RS_Model_Master.Tyre_Make_ID);
        //    return View(RS_Model_Master);
        //}

        /*               Action Name               : Delete
         *               Description               : Action used to display Delete functionality of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Model_Master/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            globalData.pageTitle = ResourceModules.Model_Master;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Model Master";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Model_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            decimal modelID = Convert.ToDecimal(id);
            RS_Model_Master RS_Model_Master = db.RS_Model_Master.Find(modelID);
            if (RS_Model_Master == null)
            {
                return HttpNotFound();
            }
            return View(RS_Model_Master);
        }

        //-------- Jitendra Mahajan CODE ---------------------//
        public ActionResult GetFamilyCodes(decimal Plant_Id, decimal Shop_id)
        {
            try
            {
                var AttrObj = db.RS_Attribution_Parameters.Where(a => a.Shop_ID == Shop_id && a.Attribute_Type == "Family")
                    .Select(a => new { a.Attribute_ID, a.Attribute_Desc })
                    .ToList();
                return Json(AttrObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "ManifestController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Manifest;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceGlobal.Edit + " " + ResourceModules.Manifest;
                this.Session["globalData"] = globalData;
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        ///////////////////////////////////////////////////


        /*               Action Name               : Edit
         *               Description               : Action used to Delete of Model Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // POST: Model_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            decimal Model_ID = Convert.ToDecimal(id);
            RS_Model_Master RS_Model_Master = db.RS_Model_Master.Find(Model_ID);
            db.RS_Model_Master.Remove(RS_Model_Master);
            db.SaveChanges();
            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Model_Master", "Model_Code", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.Model_Master;
            globalData.messageDetail = ResourceModules.Model_Master + " " + ResourceMessages.Delete_Success;
            TempData["globalData"] = globalData;
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

        //Sandip 
        public ActionResult CheckPartNoExist(string Part_No, int shopID)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (!db.RS_Partmaster.Any(p => p.Part_No == Part_No && p.Plant_ID == plantId && p.Shop_ID == shopID))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckSeriesExist(string Series_Code, string model_code, int shopID)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (db.RS_Series.Any(series => series.Series_Description == Series_Code && series.Shop_ID == shopID))
            {
                decimal series_no = db.RS_Series.Where(series => series.Series_Description == Series_Code && series.Shop_ID == shopID).Select(s => s.Series_Code).FirstOrDefault();
                if (db.RS_Model_Master.Any(model => model.Model_Code == model_code && model.Series_Code == series_no && model.Shop_ID == shopID))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //    RS_Series RS_Series = new RS_Series();
                //    RS_Series.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                //    RS_Series.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                //    RS_Series.Inserted_Date = DateTime.Now;
                //    RS_Series.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                //    RS_Series.Updated_Date = DateTime.Now;               
                //    db.RS_Series.Add(RS_Series);
                //    db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetSeriesByShop(string prefix, int Shop_ID)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            //List<RS_Series> results = new List<RS_Series>();
            var seriesno = db.RS_Series.Where(series => series.Shop_ID == Shop_ID && series.Plant_ID == plantId).Select(s => s.Series_Description);
            var results = db.RS_Series.Where(se => se.Series_Description.Contains(prefix) && se.Shop_ID == Shop_ID).Select(s => new { label = s.Series_Description, val = s.Series_Code }).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        //public class 
        //{
        //    public decimal Series_Code { get; set; }
        //    public string Series_Description { get; set; }
        //}
        public ActionResult CheckModelCodeExist(string model_code, int Shop_ID)
        {
            RS_Model_Master RS_Model_Master = new RS_Model_Master();
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            plat_form = "1";
            if (RS_Model_Master.IsModelCodeExists(plantId, Shop_ID, 0, Convert.ToInt16(plat_form), model_code))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetModelType(string prefix, int Shop_ID)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            //List<string> result = new List<string>();
            var result = db.RS_Attribution_Parameters.Where(se => se.Attribute_Desc.Contains(prefix) && se.Shop_ID == Shop_ID && se.Plant_ID == plantId && se.Attribute_Type == "Model").Select(s => new { label = s.Attribute_Desc, val = s.Attribute_ID }).Distinct().ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetVarientByShop(string prefix, int Shop_ID)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            //List<string> varient = new List<string>();
            var varient = db.RS_Attribution_Parameters.Where(se => se.Attribute_Desc.Contains(prefix) && se.Shop_ID == Shop_ID && se.Attribute_Type == "varient" && se.Plant_ID == plantId).Select(s => new { label = s.Attribute_Desc, val = s.Attribute_ID }).Distinct().ToList();

            return Json(varient, JsonRequestBehavior.AllowGet);

            //var Shop_Id = (from v in db.RS_Attribution_Parameters
            //               where v.Shop_ID == ShopID && v.Attribute_Type == "varient"

            //               select new { v.Attribute_ID, v.Attribute_Desc }).Distinct();
        }

        public ActionResult GetFamilyCodesByShop(string prefix, decimal Shop_ID)
        {
            //try
            //{
            //    var AttrObj = db.RS_Attribution_Parameters.Where(a => a.Shop_ID == Shop_id && a.Attribute_Type == "Family")
            //        .Select(a => new { a.Attribute_ID, a.Attribute_Desc })
            //        .ToList();
            //    return Json(AttrObj, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception exp)
            //{
            //    generalHelper.addControllerException(exp, "ManifestController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
            //    globalData.isErrorMessage = true;
            //    globalData.messageTitle = ResourceModules.Manifest;
            //    globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceGlobal.Edit + " " + ResourceModules.Manifest;
            //    this.Session["globalData"] = globalData;
            //}
            //return Json(null, JsonRequestBehavior.AllowGet);

            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            //List<string> varient = new List<string>();
            var familys = db.RS_Attribution_Parameters.Where(se => se.Attribute_Desc.Contains(prefix) && se.Shop_ID == Shop_ID && se.Attribute_Type == "Family" && se.Plant_ID == plantId).Select(s => new { label = s.Attribute_Desc, val = s.Attribute_ID }).Distinct().ToList();

            return Json(familys, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckSeriesExists(string family)
        {
            RS_Model_Master RS_Model_Master = new RS_Model_Master();
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var seriesno = db.RS_Series.Where(series => series.Series_Description == family && series.Plant_ID == plantId).Select(s => s.Series_Code).FirstOrDefault();
            if (db.RS_Model_Master.Any(m => m.Series_Code == seriesno))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
