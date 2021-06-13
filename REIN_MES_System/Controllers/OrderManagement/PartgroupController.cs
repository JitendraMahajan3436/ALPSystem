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
using System.IO;
using System.Data.OleDb;
using System.Globalization;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using REIN_MES_System.Controllers.BaseManagement;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /*               Controller Name           : PartgroupController
     *               Description               : Part Group Controller used to add,update,delete new part group .
     *               Author, Timestamp         : Jitendra Mahajan
    */
    public class PartgroupController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();


        int plantId = 0, shopId = 0, lineId = 0, Serial_Config = 0, consumption_LineId, consumption_StationId, Qty;
        String Part_Desc = "", Partgroup_Code;
        Boolean order_create, Is_Kitting;


        General generalObj = new General();

        /*               Action Name               : Index
         *               Description               : Action used to show the list of Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Partgroup
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.PartGroup_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Part Group";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            var RS_Partgroup = db.RS_Partgroup.Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Lines).Where(p => p.Plant_ID == plantID);

            return View(RS_Partgroup.ToList());
        }


        /*               Action Name               : Details
         *               Description               : Action used to show the detail of Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */

        // GET: Partgroup/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RS_Partgroup RS_Partgroup = db.RS_Partgroup.Find(Convert.ToDecimal(id));
            if (RS_Partgroup == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.PartGroup;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Part Group";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Partgroup);
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


       

        public ActionResult GetStationID(int Lineid)
        {
            var stationDetail = (from station in db.RS_Stations
                     where station.Line_ID == Lineid && (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == Lineid select routeConfig.Station_ID).Contains(station.Station_ID)
                     select new
                     {
                         station.Station_ID,
                         station.Station_Name
                     }).ToList();
            
            return Json(stationDetail, JsonRequestBehavior.AllowGet);
        }

        /*               Action Name             : GetSerial_ID
         *               Description               : Action used to return the list of Serial Number Config. for Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Boolean
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult GetSerial_ID(int Shop_ID)
        {
            var SerialDetail = (from line in db.RS_Serial_Number_Configuration
                                where line.Shop_ID == Shop_ID
                                orderby line.Serial_Logic ascending
                                select new
                                {
                                    line.Config_ID,
                                    line.Serial_Logic
                                }).ToList();
            return Json(SerialDetail, JsonRequestBehavior.AllowGet);
        }



        /*               Action Name              : CheckSerialConfig
        *               Description               : Action used to return the Selected of Serial Number Config. for Part Group
        *               Author, Timestamp         : Jitendra Mahajan
        *               Input parameter           : Boolean
        *               Return Type               : ActionResult
        *               Revision                  : 1
       */

        public ActionResult CheckSerialConfig(int? Order_Create)
        {
            if (Order_Create != 0 && Order_Create != null)
            {

                var CheckSerialDetail = (from partGroup in db.RS_Serial_Number_Configuration
                                         where partGroup.Config_ID == Order_Create
                                         orderby partGroup.Serial_Logic ascending
                                         select new
                                         {
                                             partGroup.Config_ID,
                                             partGroup.Serial_Logic
                                         }).ToList();
                return Json(CheckSerialDetail, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }


        //public ActionResult UpdateSerial_ID(int Shop_Id, int Line_Id)
        //{
        //    RS_Partgroup RS_Partgroup = new RS_Partgroup();
        //    try
        //    {

        //        RS_Partgroup = db.RS_Partgroup.Where(p => p.Shop_ID == Shop_Id && p.Line_ID == Line_Id).Single();
        //        RS_Partgroup.Serial_Config_ID = null;
        //        RS_Partgroup.Is_Edited = true;
        //        RS_Partgroup.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //        db.Entry(RS_Partgroup).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return View(RS_Partgroup);
        //    }
        //    catch(Exception ex)
        //    {
        //        globalData.isErrorMessage = true;
        //        globalData.messageTitle = ResourceModules.PartGroup;
        //        globalData.messageDetail = ex.ToString();
        //        TempData["globalData"] = globalData;

        //        return RedirectToAction("Edit", RS_Partgroup);

        //    }
        //}
        ////code for is kitting filling
        //public ActionResult UpdateKitting(int Shop_Id, int Line_Id)
        //{
        //    RS_Partgroup RS_Partgroup = new RS_Partgroup();
        //    try
        //    {

        //        RS_Partgroup = db.RS_Partgroup.Where(p => p.Shop_ID == Shop_Id && p.Line_ID == Line_Id).Single();
        //        RS_Partgroup.Serial_Config_ID = null;
        //        RS_Partgroup.Is_Edited = true;
        //        RS_Partgroup.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //        db.Entry(RS_Partgroup).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return View(RS_Partgroup);
        //    }
        //    catch (Exception ex)
        //    {
        //        globalData.isErrorMessage = true;
        //        globalData.messageTitle = ResourceModules.PartGroup;
        //        globalData.messageDetail = ex.ToString();
        //        TempData["globalData"] = globalData;

        //        return RedirectToAction("Edit", RS_Partgroup);

        //    }
        //}




        public ActionResult GetConsumption_Station(int Consumption_LineId)
        {
            var ConsumptionStation = (from consumption in db.RS_Stations
                                      where consumption.Line_ID == Consumption_LineId
                                      orderby consumption.Station_Name ascending
                                      select new
                                      {
                                          consumption.Station_ID,
                                          consumption.Station_Name
                                      }).ToList();

            return Json(ConsumptionStation, JsonRequestBehavior.AllowGet);
        }


        /*               Action Name               : Create
         *               Description               : Action used to show the add Part Group Form
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Partgroup/Create
        // GET: Partgroup/Create
        public ActionResult Create()
        {
            try
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceModules.PartGroup;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Part Group";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
                ViewBag.Line_Type_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name");
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                //ViewBag.LineId = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");

                return View();
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
           
        }



        /*               Action Name               : Create
         *               Description               : Action used to add the Part Group for the plant
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : RS_Partgroup
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // POST: Partgroup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Partgroup RS_Partgroup)
        {
            if (ModelState.IsValid)
            {
                // string partId = "select Partgroup_ID from RS_Partgroup where Partgroup_ID='"+ RS_Partgroup.Partgroup_ID +"'";
                // var validParts = db.Database.SqlQuery<DbValidParts>(sqlqry, partgrouppart_mapping.PartGroupId, partgrouppart_mapping.PartItem);

                shopId = Convert.ToInt16(RS_Partgroup.Shop_ID);
                plantId = Convert.ToInt16(RS_Partgroup.Plant_ID);
                lineId = Convert.ToInt16(RS_Partgroup.Line_ID);
                Serial_Config = Convert.ToInt16(RS_Partgroup.Serial_Config_ID);

                Part_Desc = RS_Partgroup.Partgrup_Desc;
                order_create = RS_Partgroup.Order_Create;
                // String partNo = RS_Partgroup.GetLastPartGroupNumber(plantId, shopId);
                if (RS_Partgroup.IsPartGroupExists(plantId, shopId, Part_Desc, 0))
                {
                    ModelState.AddModelError("Partgrup_Desc", ResourceValidation.Exist);
                    //return RedirectToAction("Create");
                }
                else
                {
                    RS_Partgroup.Inserted_Date = DateTime.Now;
                    RS_Partgroup.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Partgroup.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Partgroup.Updated_Date = DateTime.Now;
                    // RS_Partgroup.Partgroup_ID = partNo;
                    RS_Partgroup.Partgrup_Desc = Part_Desc;
                    RS_Partgroup.Order_Create = order_create;
                    RS_Partgroup.Serial_Config_ID = Serial_Config;
                    //if (consumption_StationId == 0)
                    //{
                    //    RS_Partgroup.Consumption_Station_ID = null;
                    //}
                    //else
                    //{
                    //    RS_Partgroup.Consumption_Station_ID = consumption_StationId;
                    //}
                    //if (consumption_LineId == 0)
                    //{
                    //    RS_Partgroup.Consumption_Line_ID = null;
                    //}
                    //else
                    //{
                    //    RS_Partgroup.Consumption_Line_ID = consumption_LineId;
                    //}
                    if (order_create == false)
                    {
                        RS_Partgroup.Serial_Config_ID = null;

                    }
                    else
                    {
                        
                        RS_Partgroup.Consumption_Line_ID = null;
                        RS_Partgroup.Consumption_Station_ID = null;
                        RS_Partgroup.Partgroup_Code = "BI";
                       
                    }
                    RS_Partgroup.Shop_ID = shopId;
                    RS_Partgroup.Plant_ID = plantId;
                    RS_Partgroup.Line_ID = lineId;
                    
                    db.RS_Partgroup.Add(RS_Partgroup);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.PartGroup;
                    globalData.messageDetail = ResourceModules.PartGroup + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }


            globalData.pageTitle = ResourceModules.PartGroup;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Part Group";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Partgroup.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Partgroup.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m=>m.Plant_ID==plantId), "Plant_ID", "Plant_Name", RS_Partgroup.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Partgroup.Shop_ID);
            return View(RS_Partgroup);
        }


        /*               Action Name               : Edit
         *               Description               : Action used to show edit Part Group form to allow user to update Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Partgroup/Edit/5
        // GET: Partgroup/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Partgroup RS_Partgroup = db.RS_Partgroup.Find(id);
            if (RS_Partgroup == null)
            {
                return HttpNotFound();
            }

            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.PartGroup;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Part Group";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.PartGroup + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == RS_Partgroup.Shop_ID).SingleOrDefault();

            plantId = Convert.ToInt16(shopObj.Plant_ID);
            RS_Partgroup.Plant_ID = plantId;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Partgroup.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Partgroup.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Partgroup.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Partgroup.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Partgroup.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Partgroup.Station_ID);
            ViewBag.Serial_Config_ID = new SelectList(db.RS_Serial_Number_Configuration, "Config_ID", "Serial_Logic", RS_Partgroup.Serial_Config_ID);
            ViewBag.Consumption_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Partgroup.Consumption_Line_ID);
            ViewBag.Consumption_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Partgroup.Consumption_Station_ID);

            //added by ketan
            //if (RS_Partgroup.Is_Kitting == true)
            //{
            var lineDetail = (from line in db.RS_Lines
                              where line.Shop_ID == RS_Partgroup.Shop_ID
                              orderby line.Line_Name ascending
                              select new
                              {
                                  line.Line_Name,
                                  line.Line_ID
                              }).ToList();
            ViewBag.Kitting_Release_Line_ID = new SelectList(lineDetail, "Line_ID", "Line_Name", RS_Partgroup.Kitting_Release_Line_ID);
            var StationDetail = (from line in db.RS_Stations
                                 where line.Line_ID == RS_Partgroup.Kitting_Release_Line_ID
                                 orderby line.Station_Name ascending
                                 select new
                                 {
                                     line.Station_Name,
                                     line.Station_ID
                                 }).ToList();
            ViewBag.Kitting_Release_Station_ID = new SelectList(StationDetail, "Station_ID", "Station_Name", RS_Partgroup.Kitting_Release_Line_ID);

            // }
            return View(RS_Partgroup);
        }


        /*               Action Name               : Edit
         *               Description               : Action used to update Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // POST: Partgroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Partgroup RS_Partgroup)
        {
            if (ModelState.IsValid)
            {

                shopId = Convert.ToInt16(RS_Partgroup.Shop_ID);
                plantId = Convert.ToInt16(RS_Partgroup.Plant_ID);
                lineId = Convert.ToInt16(RS_Partgroup.Line_ID);
                Part_Desc = RS_Partgroup.Partgrup_Desc;
                order_create = RS_Partgroup.Order_Create;
                consumption_LineId = Convert.ToInt16(RS_Partgroup.Consumption_Line_ID);
                consumption_StationId = Convert.ToInt16(RS_Partgroup.Consumption_Station_ID);
                Is_Kitting = Convert.ToBoolean(RS_Partgroup.Is_Kitting);
                RS_Partgroup mmPartGroupObj = new RS_Partgroup();
                mmPartGroupObj = db.RS_Partgroup.Find(RS_Partgroup.Partgroup_ID);
                if (order_create == true)
                {
                    mmPartGroupObj.Consumption_Line_ID = null;
                    mmPartGroupObj.Consumption_Station_ID = null;
                    mmPartGroupObj.Partgroup_Code = "BI";
                    mmPartGroupObj.Is_Kitting = false;
                    mmPartGroupObj.Kitting_Release_Line_ID = null;
                    mmPartGroupObj.Kitting_Release_Station_ID = null;
                    mmPartGroupObj.Serial_Config_ID = RS_Partgroup.Serial_Config_ID;

                }
                else
                {

                    mmPartGroupObj.Serial_Config_ID = null;
                    mmPartGroupObj.Consumption_Line_ID = RS_Partgroup.Consumption_Line_ID;
                    mmPartGroupObj.Consumption_Station_ID = RS_Partgroup.Consumption_Station_ID;
                    mmPartGroupObj.Partgroup_Code = RS_Partgroup.Partgroup_Code;
                    if (Is_Kitting == true)
                    {
                        mmPartGroupObj.Kitting_Release_Line_ID = RS_Partgroup.Kitting_Release_Line_ID;
                        mmPartGroupObj.Kitting_Release_Station_ID = RS_Partgroup.Kitting_Release_Station_ID;
                    }
                    else
                    {
                        mmPartGroupObj.Kitting_Release_Line_ID = null;
                        mmPartGroupObj.Kitting_Release_Station_ID = null;
                    }
                }
                mmPartGroupObj.Plant_ID = plantId;
                mmPartGroupObj.Shop_ID = shopId;
                mmPartGroupObj.Line_ID = lineId;
                mmPartGroupObj.Partgrup_Desc = Part_Desc;
                mmPartGroupObj.Order_Create = order_create;
                mmPartGroupObj.Is_Edited = true;

                mmPartGroupObj.Is_Kitting = Is_Kitting;
                mmPartGroupObj.Partgrup_Desc = RS_Partgroup.Partgrup_Desc;
                mmPartGroupObj.Updated_Date = DateTime.Now;
                mmPartGroupObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                mmPartGroupObj.Qty = RS_Partgroup.Qty;
                mmPartGroupObj.Is_Print = RS_Partgroup.Is_Print;
                db.Entry(mmPartGroupObj).State = EntityState.Modified;
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.PartGroup;
                globalData.messageDetail = ResourceModules.PartGroup + " " + ResourceMessages.Edit_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
                //}
            }

            RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == RS_Partgroup.Shop_ID).SingleOrDefault();
            plantId = Convert.ToInt16(shopObj.Plant_ID);
            RS_Partgroup.Plant_ID = plantId;

            globalData.pageTitle = ResourceModules.PartGroup;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Part Group";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceModules.PartGroup + " " + ResourceMessages.Edit_Success;
            globalData.contentFooter = ResourceModules.PartGroup + " " + ResourceMessages.Edit_Success;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Partgroup.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Partgroup.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Partgroup.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Partgroup.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Partgroup.Shop_ID), "Line_ID", "Line_Name", RS_Partgroup.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p=>p.Line_ID == RS_Partgroup.Line_ID), "Station_ID", "Station_Name", RS_Partgroup.Station_ID);
            ViewBag.Serial_Config_ID = new SelectList(db.RS_Serial_Number_Configuration, "Config_ID", "Serial_Logic", RS_Partgroup.Serial_Config_ID);
            ViewBag.Consumption_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Partgroup.Consumption_Line_ID);
            ViewBag.Consumption_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Partgroup.Consumption_Station_ID);
            var lineDetail = (from line in db.RS_Lines
                              where line.Shop_ID == RS_Partgroup.Shop_ID
                              orderby line.Line_Name ascending
                              select new
                              {
                                  line.Line_Name,
                                  line.Line_ID
                              }).ToList();
            ViewBag.Kitting_Release_Line_ID = new SelectList(lineDetail, "Line_ID", "Line_Name", RS_Partgroup.Kitting_Release_Line_ID);
            var StationDetail = (from line in db.RS_Stations
                                 where line.Line_ID == RS_Partgroup.Kitting_Release_Line_ID
                                 orderby line.Station_Name ascending
                                 select new
                                 {
                                     line.Station_Name,
                                     line.Station_ID
                                 }).ToList();
            ViewBag.Kitting_Release_Station_ID = new SelectList(StationDetail, "Station_ID", "Station_Name", RS_Partgroup.Kitting_Release_Line_ID);
            return View(RS_Partgroup);
        }


        /*               Action Name               : Delete
         *               Description               : Action used to show Part Group delete form for confirnmation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Partgroup/Delete/5
        // GET: Partgroup/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Partgroup RS_Partgroup = db.RS_Partgroup.Find(id);
            if (RS_Partgroup == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.PartGroup;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Part Group";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceModules.PartGroup + " " + ResourceMessages.Delete_Success;
            globalData.contentFooter = ResourceModules.PartGroup + " " + ResourceMessages.Delete_Success;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Partgroup);
        }


        /*               Action Name               : DeleteConfirmed
         *               Description               : Action used to delete Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // POST: Partgroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            RS_Partgroup RS_Partgroup = db.RS_Partgroup.Find(Convert.ToDecimal(id));
            try
            {
                db.RS_Partgroup.Remove(RS_Partgroup);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Partgroup", "Partgroup_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.PartGroup;
                globalData.messageDetail = ResourceModules.PartGroup + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
            }
            catch (DbUpdateException ex)
            {


                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Partgroup", "Partgroup_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.PartGroup;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;

            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.PartGroup;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete", RS_Partgroup);
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
    }
}
