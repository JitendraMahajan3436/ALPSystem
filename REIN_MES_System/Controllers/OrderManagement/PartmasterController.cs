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
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class PartmasterController : BaseController
    {
        int plantId = 0, shopId = 0, pgroupId = 0, lineId = 0, stationId = 0, Month_Identifier, Year_Identifier,PartID=0;
        string partno, part_desc, stage, Std_Char;
        Int16 Start_Position, Year_Start_Position, Month_Start_Position;
        bool kit, seq, tra, err, sob, is_Non_RS_Barcode,isTraceability;
        decimal? part_grp;
        decimal? series_code;
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        General generalObj = new General();

        /*               Action Name               : Index
         *               Description               : Action used to show the list of Part Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */

        // GET: Partmaster
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Part Master";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Part_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Part_Master + " " + ResourceGlobal.Form;

            ViewBag.GlobalDataModel = globalData;

            var RS_Partmaster = db.RS_Partmaster.Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Partgroup).Include(m => m.RS_Series).Include(m => m.RS_Lines).Include(m => m.RS_Stations).Where(p => p.Plant_ID == plantID);
            return View(RS_Partmaster.ToList());
        }



        /*               Action Name               : Details
         *               Description               : Action used to show the detail of Part Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Partmaster/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RS_Partmaster RS_Partmaster = db.RS_Partmaster.Find(Convert.ToDecimal(id));
            if (RS_Partmaster == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Part_Master + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Part Master";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Part_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Part_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Partmaster);
        }

        /*               Action Name               : GetPlantID
         *               Description               : Action used to return the list of Shop ID for Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Plant_Id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Shop 
        public ActionResult GetPlantID(int Plant_Id)
        {
            var Shop_Id = db.RS_Shops
                                       .Where(c => c.Plant_ID == Plant_Id)
                                       .Select(c => new { c.Shop_ID, c.Shop_Name });
            return Json(Shop_Id, JsonRequestBehavior.AllowGet);
        }

        /*               Action Name               : GetLineID
         *               Description               : Action used to return the list of Line ID for Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Shopid
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Line
        public ActionResult GetLineID(int Shopid)
        {
            var lineDetail = (from line in db.RS_Lines
                              where line.Shop_ID == Shopid
                              orderby line.Line_Name ascending
                              select new
                              {
                                  line.Line_Name,
                                  line.Line_ID
                              }).ToList();
            return Json(lineDetail, JsonRequestBehavior.AllowGet);
        }

        /*               Action Name               : GetStationID
         *               Description               : Action used to return the list of Station ID for Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : plantid,shopid
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Station
        public ActionResult GetStationID(int plantid, int shopid, int LineId)
        {
            var Station_ID = (from u in db.RS_Route_Configurations
                              join b in db.RS_Stations
                              on u.Station_ID equals b.Station_ID
                              where (u.Plant_ID == plantid && u.Shop_ID == shopid && u.Line_ID == LineId)
                              orderby b.Station_Name ascending
                              select new { b.Station_ID, b.Station_Name }).Distinct();
            return Json(Station_ID, JsonRequestBehavior.AllowGet);
        }


        /*               Action Name               : GetPartGroup
         *               Description               : Action used to return the list of PartGroup ID  for Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           :  Plant_Id,shopid
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // Find PartGroup
        public ActionResult GetPartGroup(int Plant_Id, int Shop_Id)
        {
            //var PartFroup = db.RS_Partgroup
            //                               .Where(c => c.Plant_ID == Plant_Id && c.Shop_ID == Shop_Id)
            //                               .Select(c => new { c.Partgroup_ID, c.Partgrup_Desc });
            bool isMain = db.RS_Shops.Where(shop => shop.Shop_ID == Shop_Id).Select(sh => sh.Is_Main).FirstOrDefault();
            if (isMain)
            {
                var PartFroup = (from u in db.RS_Partgroup
                                 where u.Plant_ID == Plant_Id && u.Order_Create == true
                                 orderby u.Partgrup_Desc ascending
                                 select new { u.Partgroup_ID, u.Partgrup_Desc }).Distinct();
                return Json(PartFroup, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var PartFroup = (from u in db.RS_Partgroup
                                 where u.Plant_ID == Plant_Id && u.Shop_ID == Shop_Id
                                 orderby u.Partgrup_Desc ascending
                                 select new { u.Partgroup_ID, u.Partgrup_Desc }).Distinct();
                return Json(PartFroup, JsonRequestBehavior.AllowGet);
            }


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
            //var Series = db.RS_Series
            //                              .Where(c => c.Plant_ID == Plant_Id && c.Shop_ID==Shop_Id)
            //                              .Select(c => new { c.Series_Code, c.Series_Description });

            var Series = (from u in db.RS_Series
                          where u.Plant_ID == Plant_Id && u.Shop_ID == Shop_Id
                          orderby u.Series_Description ascending
                          select new { u.Series_Code, u.Series_Description });
            return Json(Series, JsonRequestBehavior.AllowGet);
        }

        /*               Action Name               : Create
         *               Description               : Action used to show the add Part Master Form
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Partmaster/Create
        public ActionResult Create()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Part Master";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Part_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Part_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;


            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantID), "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantID), "User_ID", "User_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantID), "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Plant_ID == plantID), "Line_ID", "Line_Description");
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Description");
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup.Where(p => p.Plant_ID == plantID), "Partgroup_ID", "Partgrup_Desc");
            ViewBag.PartID = new SelectList(db.RS_PartID.Select(m=> new { RowID = m.RowID, PartIDDescription = m.PartID + "(" + m.PartIDDescription + ")" }), "RowID", "PartIDDescription");
            ViewBag.Series = new SelectList(db.RS_Series.Where(p => p.Plant_ID == plantID), "Series_Code", "Series_Description");
            return View();
        }

        /*               Action Name               : Create
         *               Description               : Action used to add the Part Master for the plant
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : RS_Partmaster
         *               Return Type               : ActionResult
         *               Revision                  : 1
         */
        // POST: Partmaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Partmaster RS_Partmaster, string hSeries_Discription)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    shopId = Convert.ToInt16(RS_Partmaster.Shop_ID);
                    plantId = Convert.ToInt16(RS_Partmaster.Plant_ID);
                    part_grp = RS_Partmaster.Partgroup_ID;
                    partno = RS_Partmaster.Part_No.ToUpper();
                    RS_Partmaster.Part_No = partno;
                    part_desc = RS_Partmaster.Part_Description;
                    stage = RS_Partmaster.Stage;
                    series_code = RS_Partmaster.Series_Code;
                    lineId = Convert.ToInt16(RS_Partmaster.Line_ID);
                    stationId = Convert.ToInt16(RS_Partmaster.Station_ID);
                   
                    is_Non_RS_Barcode = Convert.ToBoolean(RS_Partmaster.is_Non_RS_Barcode);
                    if (is_Non_RS_Barcode)
                    {
                        Std_Char = RS_Partmaster.Std_Char;
                        Year_Identifier = Convert.ToInt32(RS_Partmaster.Year_Identifier);
                        Year_Start_Position = Convert.ToInt16(RS_Partmaster.Year_Start_Position);
                        Month_Identifier = Convert.ToInt32(RS_Partmaster.Month_Identifier);
                        Month_Start_Position = Convert.ToInt16(RS_Partmaster.Month_Start_Position);
                        Start_Position = Convert.ToInt16(RS_Partmaster.Start_Position);
                    }

                    if (RS_Partmaster.IsPartNoExists(plantId, shopId, stationId, lineId, part_grp, partno))
                    {
                        ModelState.AddModelError("Part_No", ResourceValidation.Exist);
                    }
                    else
                    {
                        #region Series

                        //if (hSeries_Discription == "" || hSeries_Discription == null)
                        //{
                        //    if (db.RS_Series.Any(s => s.Series_Description == RS_Partmaster.Series_Discription && s.Plant_ID == plantId))
                        //    {
                        //        RS_Partmaster.Series_Code = db.RS_Series.Where(s => s.Series_Description == RS_Partmaster.Series_Discription && s.Plant_ID == plantId).Select(s => s.Series_Code).FirstOrDefault();
                        //    }
                        //    else
                        //    {
                        //        RS_Series objRS_Series = new RS_Series();
                        //        objRS_Series.Shop_ID = RS_Partmaster.Shop_ID;
                        //        objRS_Series.Series_Description = RS_Partmaster.Series_Discription.ToUpper();
                        //        objRS_Series.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        //        objRS_Series.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        //        objRS_Series.Inserted_Date = DateTime.Now;
                        //        objRS_Series.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        //        objRS_Series.Updated_Date = DateTime.Now;
                        //        db.RS_Series.Add(objRS_Series);
                        //        db.SaveChanges();
                        //        RS_Partmaster.Series_Code = objRS_Series.Series_Code;
                        //    }

                        //}
                        //else
                        //{
                        //    RS_Partmaster.Series_Code = Convert.ToDecimal(hSeries_Discription);
                        //}

                        #endregion
                        //Part Master
                        


                        RS_Partmaster.Inserted_Date = DateTime.Now;
                        RS_Partmaster.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_Partmaster.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_Partmaster.Updated_Date = DateTime.Now;
                        db.RS_Partmaster.Add(RS_Partmaster);
                        db.SaveChanges();

                        //Partgroup Item
                        RS_PartgroupItem RS_PartgroupItem = new RS_PartgroupItem();

                        if (RS_Partmaster.Is_Part_Create_Order == true)
                        {
                            RS_PartgroupItem.Is_Order_Create = true;
                        }
                        else
                        {
                            RS_PartgroupItem.Is_Order_Create = false;
                        }


                        string partgroupitem_id = RS_PartgroupItem.GetLastPartGroupItemNumber();
                        RS_PartgroupItem.PartgroupItem_ID = Convert.ToInt16(partgroupitem_id);
                        RS_PartgroupItem.Part_No = partno;
                        RS_PartgroupItem.Partgroup_ID = Convert.ToDecimal(part_grp);
                        RS_PartgroupItem.Series_Code = series_code;
                        RS_PartgroupItem.Inserted_Date = DateTime.Now;
                        RS_PartgroupItem.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_PartgroupItem.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_PartgroupItem.Updated_Date = DateTime.Now;
                        int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                        RS_PartgroupItem.Plant_ID = plantID;
                        db.RS_PartgroupItem.Add(RS_PartgroupItem);
                        db.SaveChanges();


                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
                        globalData.messageDetail = ResourceModules.Part_Master + " " + ResourceMessages.Save_Success;
                        TempData["globalData"] = globalData;

                        return RedirectToAction("Index");
                    }


                }
                catch (Exception ex)
                {
                    globalData.messageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
                    globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceGlobal.Form;
                }

            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_Partmaster.Plant_ID);
            var Shop_ID = db.RS_Shops.Where(p => p.Plant_ID == plantId).Select(s => s.Shop_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(l => Shop_ID.Contains(l.Shop_ID)), "Line_ID", "Line_Description");
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Description");
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup.Where(p => p.Plant_ID == plantId), "Partgroup_ID", "Partgrup_Desc");
            ViewBag.PartID = new SelectList(db.RS_PartID.Select(m => new { RowID = m.RowID, PartIDDescription = m.PartID + "(" + m.PartIDDescription + ")" }), "RowID", "PartIDDescription",RS_Partmaster.PartID);
            //ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description");
            // ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Partmaster.Shop_ID);
            //ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup, "Partgroup_ID", "Partgrup_Desc", RS_Partmaster.Partgroup_ID);
            return View(RS_Partmaster);
        }


        /*               Action Name               : Edit
         *               Description               : Action used to show edit Part Master form to allow user to update Part Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
         */
        // GET: Partmaster/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Partmaster RS_Partmaster = db.RS_Partmaster.Find(Convert.ToDecimal(id));
            if (RS_Partmaster == null)
            {
                return HttpNotFound();
            }
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Part Master";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Part_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Part_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            RS_Shops Shop_ID = db.RS_Shops.Where(p => p.Shop_ID == RS_Partmaster.Shop_ID).SingleOrDefault();
            RS_Lines LineID = db.RS_Lines.Where(p => p.Line_ID == RS_Partmaster.Line_ID).SingleOrDefault();
            RS_Stations StationID = db.RS_Stations.Where(p => p.Station_ID == RS_Partmaster.Station_ID).SingleOrDefault();


            plantId = Convert.ToInt16(Shop_ID.Plant_ID);
            RS_Partmaster.Plant_ID = plantId;
            RS_Partmaster.Shop_ID = Shop_ID.Shop_ID;
            RS_Partmaster.Line_ID = LineID.Line_ID;
            RS_Partmaster.Station_ID = StationID.Station_ID;


            RS_Series series_data = db.RS_Series.Where(p => p.Series_Code == RS_Partmaster.Series_Code).SingleOrDefault();
            if (series_code != null)
            {
                series_code = series_data.Series_Code;
                RS_Partmaster.Series_Code = series_code;
            }


            ViewBag.Series_Code = new SelectList(db.RS_Series.Where(p => p.Plant_ID == plantID), "Series_Code", "Series_Description", RS_Partmaster.Series_Code);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantID), "User_ID", "User_Name", RS_Partmaster.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantID), "User_ID", "User_Name", RS_Partmaster.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", RS_Partmaster.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantID), "Shop_ID", "Shop_Name", RS_Partmaster.Shop_ID);
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup.Where(p => p.Plant_ID == plantID && p.Shop_ID == RS_Partmaster.Shop_ID), "Partgroup_ID", "Partgrup_Desc", RS_Partmaster.Partgroup_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Partmaster.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Partmaster.Station_ID);
            ViewBag.PartID = new SelectList(db.RS_PartID.Select(m => new { RowID = m.RowID, PartIDDescription = m.PartID + "(" + m.PartIDDescription + ")" }), "RowID", "PartIDDescription", RS_Partmaster.PartID);
            return View(RS_Partmaster);
        }


        /*               Action Name               : Edit
         *               Description               : Action used to update Part Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : RS_Partmaster
         *               Return Type               : ActionResult
         *               Revision                  : 1
         */
        // POST: Partmaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Partmaster RS_Partmaster, bool is_Non_RS_Barcode)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            try
            {
                if (ModelState.IsValid)
                {
                    shopId = Convert.ToInt16(RS_Partmaster.Shop_ID);
                    plantId = Convert.ToInt16(RS_Partmaster.Plant_ID);
                    stationId = Convert.ToInt16(RS_Partmaster.Station_ID);
                    lineId = Convert.ToInt16(RS_Partmaster.Line_ID);
                    part_grp = RS_Partmaster.Partgroup_ID;
                    partno = RS_Partmaster.Part_No;
                    series_code = RS_Partmaster.Series_Code;
                    is_Non_RS_Barcode = Convert.ToBoolean(RS_Partmaster.is_Non_RS_Barcode);
                    Std_Char = RS_Partmaster.Std_Char;
                    Year_Identifier = Convert.ToInt32(RS_Partmaster.Year_Identifier);
                    Year_Start_Position = Convert.ToInt16(RS_Partmaster.Year_Start_Position);
                    Month_Identifier = Convert.ToInt32(RS_Partmaster.Month_Identifier);
                    Month_Start_Position = Convert.ToInt16(RS_Partmaster.Month_Start_Position);
                    Start_Position = Convert.ToInt16(RS_Partmaster.Start_Position);


                    RS_Partmaster mmPartMastrObj = new RS_Partmaster();
                    mmPartMastrObj = db.RS_Partmaster.Find(RS_Partmaster.PartMaster_ID);



                    mmPartMastrObj.Plant_ID = plantId;
                    mmPartMastrObj.Shop_ID = shopId;
                    mmPartMastrObj.Station_ID = stationId;
                    mmPartMastrObj.Line_ID = lineId;
                    mmPartMastrObj.Partgroup_ID = part_grp;
                    //string oldSeries = series_code;
                    //if (mmPartMastrObj.Series_Code != series_code)
                    //{
                    //    oldSeries = mmPartMastrObj.Series_Code;
                    //}
                    mmPartMastrObj.Series_Code = series_code;
                    // mmPartMastrObj.Old_Series_Code = oldSeries;
                    mmPartMastrObj.Part_Description = RS_Partmaster.Part_Description;
                    mmPartMastrObj.Stage = RS_Partmaster.Stage;
                    mmPartMastrObj.Sequence = RS_Partmaster.Sequence;
                    mmPartMastrObj.Kitting = RS_Partmaster.Kitting;
                    mmPartMastrObj.Traceability = RS_Partmaster.Traceability;
                    mmPartMastrObj.Error_Proofing = RS_Partmaster.Error_Proofing;
                    mmPartMastrObj.Genealogy = RS_Partmaster.Genealogy;
                    mmPartMastrObj.SOB = RS_Partmaster.SOB;
                    mmPartMastrObj.isFinished = RS_Partmaster.isFinished;
                    mmPartMastrObj.Traceability = RS_Partmaster.Traceability;
                    mmPartMastrObj.PartID = RS_Partmaster.PartID;
                    mmPartMastrObj.is_Non_RS_Barcode = is_Non_RS_Barcode;
                    if (is_Non_RS_Barcode == false)
                    {
                        mmPartMastrObj.Std_Char = null;
                        mmPartMastrObj.Year_Identifier = null;
                        mmPartMastrObj.Year_Start_Position = null;
                        mmPartMastrObj.Month_Identifier = null;
                        mmPartMastrObj.Month_Start_Position = null;
                        mmPartMastrObj.Start_Position = null;
                    }
                    else
                    {
                        mmPartMastrObj.Std_Char = Std_Char;
                        mmPartMastrObj.Year_Identifier = Year_Identifier;
                        mmPartMastrObj.Year_Start_Position = Year_Start_Position;
                        mmPartMastrObj.Month_Identifier = Month_Identifier;
                        mmPartMastrObj.Month_Start_Position = Month_Start_Position;
                        mmPartMastrObj.Start_Position = Start_Position;

                    }
                    // mmPartMastrObj.Inserted_User_ID = RS_Partmaster.Inserted_User_ID;
                    // mmPartMastrObj.Inserted_Date = RS_Partmaster.Inserted_Date;
                    mmPartMastrObj.Updated_Date = DateTime.Now;
                    mmPartMastrObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                    mmPartMastrObj.Is_Edited = true;
                    db.Entry(mmPartMastrObj).State = EntityState.Modified;
                    db.SaveChanges();

                    //  MM_Part_Master mmModelMaster = db.MM_Part_Master.Find(mmPartMastrObj.Part_No);
                    //if (mmModelMaster != null)
                    //{
                    //    mmModelMaster.Series_Code = series_code;
                    //    mmModelMaster.Old_Series_Code = oldSeries;
                    //    db.Entry(mmModelMaster).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}


                    globalData.isSuccessMessage = true;
                    globalData.pageTitle = ResourceModules.Part_Master;
                    globalData.subTitle = ResourceGlobal.Edit;
                    globalData.controllerName = "Part_Master";
                    globalData.actionName = ResourceGlobal.Edit;
                    globalData.contentTitle = ResourceModules.Part_Master + " " + ResourceMessages.Edit_Success;
                    globalData.contentFooter = ResourceModules.Part_Master + " " + ResourceMessages.Edit_Success;
                    globalData.messageDetail = ResourceModules.Part_Master + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");

                    RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == RS_Partmaster.Shop_ID).SingleOrDefault();
                    plantId = Convert.ToInt16(shopObj.Plant_ID);
                    RS_Partmaster.Plant_ID = plantId;




                    //}
                }
            }
            catch (Exception ex)
            {
                globalData.messageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.Part_Master;
            }

            globalData.pageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Part Master";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantID), "User_ID", "User_Name", RS_Partmaster.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantID), "User_ID", "User_Name", RS_Partmaster.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", RS_Partmaster.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantID), "Shop_ID", "Shop_Name", RS_Partmaster.Shop_ID);
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup.Where(p => p.Plant_ID == plantID), "Partgroup_ID", "Partgrup_Desc", RS_Partmaster.Partgroup_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Partmaster.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Partmaster.Station_ID);
            ViewBag.Series_Code = new SelectList(db.RS_Series.Where(p => p.Plant_ID == plantID), "Series_Code", "Series_Description", RS_Partmaster.Series_Code);
            ViewBag.PartID = new SelectList(db.RS_PartID.Select(m => new { RowID = m.RowID, PartIDDescription = m.PartID + "(" + m.PartIDDescription + ")" }), "RowID", "PartIDDescription", RS_Partmaster.PartID);
            return View(RS_Partmaster);

        }


        /*               Action Name               : Delete
         *               Description               : Action used to show Part Master delete form for confirnmation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
         */
        // GET: Partmaster/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Partmaster RS_Partmaster = db.RS_Partmaster.Find(Convert.ToDecimal(id));
            if (RS_Partmaster == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Partmaster";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceModules.Part_Master + " " + ResourceMessages.Delete_Success;
            globalData.contentFooter = ResourceModules.Part_Master + " " + ResourceMessages.Delete_Success;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Partmaster);
        }

        /*               Action Name               : DeleteConfirmed
         *               Description               : Action used to delete Part Master
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
         */
        // POST: Partmaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            RS_Partmaster RS_Partmaster = db.RS_Partmaster.Find(Convert.ToDecimal(id));
            try
            {
                db.RS_Partmaster.Remove(RS_Partmaster);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Partmaster", "Part_No", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
                globalData.messageDetail = ResourceModules.Part_Master + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

            }
            catch (DbUpdateException ex)
            {
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Partmaster", "Part_No", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Part_Master + " " + ResourceGlobal.Config;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete", RS_Partmaster);
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

        //Check Partno exist
        public ActionResult CheckPartNoExist(string Part_No, int shopID, int LineId, int Station_ID, int Partgroup_ID)
        {
            RS_Partmaster RS_Partmaster = new RS_Partmaster();
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (RS_Partmaster.IsPartNoExists(plantId, shopID, Station_ID, LineId, Partgroup_ID, Part_No))
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
