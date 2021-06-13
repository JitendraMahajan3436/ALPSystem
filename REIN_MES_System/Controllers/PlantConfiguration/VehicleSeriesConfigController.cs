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

namespace REIN_MES_System.Controllers.PlantConfiguration
{
    /* Controller Name            : VehicleSeriesConfigController
    *  Description                : VehicleSeriesConfig controller is used to add/edit/delete/display the style code of model
    *  Author, Timestamp          : Mukesh Chaudhari       
    */
    public class VehicleSeriesConfigController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        RS_Model_Master mmLinesObj = new RS_Model_Master();
        int plantId = 0, lineId = 0, lineTypeId = 0, shopId = 0;


        General generalObj = new General();

        /* Action Name                : Index
        *  Description                : Get the list of lines added
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Line/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.VehicleSeriesConfig;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "VehicleSeriesConfig";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;


            //var RS_Lines = from l in db.RS_Lines join s in db.RS_Shops on l.Shop_ID equals s.Shop_ID where s.Plant_ID == plantID select l;
            //var vmVehicleSeriesObj = (from series in db.RS_Vehicle_Series
            //                      where series.Plant_ID == plantID && series.Attribute_Name != null
            //                      select new VMVehicleSeriesConfig()
            //                      {
            //                          Model_ID = series.Model_ID,
            //                          Shop_ID = series.Shop_ID,
            //                          Model_Code = series.Model_Code,
            //                          Style_Code = series.Style_Code,
            //                          Shop_Name = series.RS_Shops.Shop_Name
            //                      }).ToList();
            var vmVehicleSeriesObj = db.RS_Vehicle_Series.Where(m => m.Plant_ID == plantID).ToList();
            return View(vmVehicleSeriesObj);
        }

        /* Action Name                : Details
        *  Description                : Get the detail of lines by line id
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Line/Details/5
        public ActionResult Details(decimal id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var seriesObj = db.RS_Vehicle_Series.Find(id);
            if (seriesObj == null)
            {
                return HttpNotFound();
            }
        

       

            globalData.pageTitle = ResourceModules.VehicleSeriesConfig;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "VehicleSeriesConfig";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(seriesObj);
        }

        /* Action Name                : Create
        *  Description                : Action used to add new line under plant with shop and line type
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Line/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.VehicleSeriesConfig;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "VehicleSeriesConfig";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            //ViewBag.Shop_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name");
            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            var Attribute_Name = db.RS_Attribution_Parameters.Where(c => c.Attribute_Type.ToLower() == "Vehicle Series".ToLower());

            //ViewBag.Attribute_Name = new SelectList(Attribute_Name, "Attribute_ID", "Attribute_Desc");
            ViewBag.Attribute_Name = new SelectList(db.RS_Vehicle_Series.Where(p => p.Plant_ID == plantId), "Row_ID", "Attribute_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);

            return View();
        }

        // POST: /Line/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        /* Action Name                : Create
        *  Description                : Create the line. Validate the line is already added or not
        *  Author, Timestamp          : Mukesh Chaudhari
        *  Input parameter            : RS_Lines
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Vehicle_Series vehicleSeriesConfig)
        {
            if (ModelState.IsValid)
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                shopId = Convert.ToInt16(vehicleSeriesConfig.Shop_ID);
                
                try
                {

              
                var Attribute_Id = Convert.ToInt16(vehicleSeriesConfig.Attribute_Name);
                   
                //lineTypeId = Convert.ToInt16(RS_Lines.Line_Type_Id);

                var Attribute_Name = db.RS_Attribution_Parameters.Find(Attribute_Id).Attribute_Desc;
                if (Attribute_Name != null)
                {
                    var mmVehicleSeriesObj = new RS_Vehicle_Series();
                        var date = DateTime.Now;
                    mmVehicleSeriesObj.Attribute_Name = Attribute_Name;
                        mmVehicleSeriesObj.Inserted_Date = date;
                        mmVehicleSeriesObj.Plant_ID = vehicleSeriesConfig.Plant_ID;
                    mmVehicleSeriesObj.Shop_ID = vehicleSeriesConfig.Shop_ID;
                    mmVehicleSeriesObj.BOT = vehicleSeriesConfig.BOT;
                        mmVehicleSeriesObj.Blackout = vehicleSeriesConfig.Blackout;
                        mmVehicleSeriesObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.RS_Vehicle_Series.Add(mmVehicleSeriesObj);
                    db.SaveChanges();

                
                }



                }
                catch (Exception ex)
                {
                    return RedirectToAction("Create");

                }

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.VehicleSeriesConfig;
                globalData.messageDetail = ResourceModules.VehicleSeriesConfig + " " + ResourceMessages.Add_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");

            }
            globalData.pageTitle = ResourceModules.VehicleSeriesConfig;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "VehicleSeriesConfig";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = Convert.ToInt16(vehicleSeriesConfig.Plant_ID);


            //ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", modelObj.Inserted_User_ID);
            //ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", mo.Updated_User_ID);
            //ViewBag.Line_Type_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name", RS_Lines.Line_Type_Id);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants.DefaultIfEmpty(), "Plant_ID", "Plant_Name", RS_Lines.Plant_ID);
            //ViewBag.Is_Conveyor_Selected = RS_Lines.Is_Conveyor;
            //ViewBag.Selected_Plant_ID = RS_Lines.Plant_ID;

            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Model_ID = new SelectList(db.RS_Model_Master.Where(p => p.Plant_ID == plantId), "Model_ID", "Model_Code");
            ViewBag.Style_Code = new SelectList(db.RS_Style_Code.Where(p => p.Plant_ID == plantId), "Style_Code", "Style_Code");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            return View(vehicleSeriesConfig);
        }

        /* Action Name                : Edit
        *  Description                : Show the edit line form
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Line/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Vehicle_Series series = db.RS_Vehicle_Series.Find(id);
            if (series == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.VehicleSeriesConfig;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "VehicleSeriesConfig";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == series.Shop_ID).SingleOrDefault();

            plantId = Convert.ToInt16(shopObj.Plant_ID);

            //VMVehicleSeriesConfig vmVehicleSeries = new VMVehicleSeriesConfig();



            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", series.Shop_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            var Attribute_Name_Row_ID = db.RS_Attribution_Parameters.Where(c => c.Attribute_Desc.ToLower() == series.Attribute_Name.ToLower()).Select(m=>m.Attribute_ID).FirstOrDefault();

            //ViewBag.Attribute_Name = new SelectList(Attribute_Name, "Attribute_ID", "Attribute_Desc", series.Attribute_Name);

            ViewBag.Selected_Attribute = series.Attribute_Name;
            ViewBag.Attribute_Name = series.Attribute_Name;
            //ViewBag.Attribute_Name = new SelectList(db.RS_Vehicle_Series.Where(p => p.Plant_ID == plantId), "Row_ID", "Attribute_Name", series.Row_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);


            return View(series);
        }

        // POST: /Line/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        /* Action Name                : Edit
        *  Description                : Action is used to edit the line
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Lines
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Vehicle_Series vehicleSeriesConfig)
        {
            if (ModelState.IsValid)
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                shopId = Convert.ToInt16(vehicleSeriesConfig.Shop_ID);
                var rowID = Convert.ToInt16(vehicleSeriesConfig.Row_ID);
                try
                {


                    var mmVehicleSeriesObj = db.RS_Vehicle_Series.Find(rowID);

                   
                        
                        var date = DateTime.Now;
                        mmVehicleSeriesObj.Attribute_Name = mmVehicleSeriesObj.Attribute_Name;
                        mmVehicleSeriesObj.Inserted_Date = date;
                        mmVehicleSeriesObj.Plant_ID = mmVehicleSeriesObj.Plant_ID;
                        mmVehicleSeriesObj.Shop_ID = mmVehicleSeriesObj.Shop_ID;
                        mmVehicleSeriesObj.BOT = vehicleSeriesConfig.BOT;
                    mmVehicleSeriesObj.Blackout = vehicleSeriesConfig.Blackout;
                    mmVehicleSeriesObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmVehicleSeriesObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        mmVehicleSeriesObj.Updated_Date = DateTime.Now;
                        mmVehicleSeriesObj.Is_Edited = true;
                        db.Entry(mmVehicleSeriesObj).State = EntityState.Modified;
                        db.SaveChanges();




                }
                catch (Exception ex)
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.VehicleSeriesConfig;
                    globalData.messageDetail = ResourceModules.VehicleSeriesConfig + " " + ResourceMessages.Is_Error;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");

                }

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.VehicleSeriesConfig;
                globalData.messageDetail = ResourceModules.VehicleSeriesConfig + " " + ResourceMessages.Edit_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");

            }
            globalData.pageTitle = ResourceModules.VehicleSeriesConfig;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "VehicleSeriesConfig";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.VehicleSeriesConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = Convert.ToInt16(vehicleSeriesConfig.Plant_ID);


            //ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", modelObj.Inserted_User_ID);
            //ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", mo.Updated_User_ID);
            //ViewBag.Line_Type_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name", RS_Lines.Line_Type_Id);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants.DefaultIfEmpty(), "Plant_ID", "Plant_Name", RS_Lines.Plant_ID);
            //ViewBag.Is_Conveyor_Selected = RS_Lines.Is_Conveyor;
            //ViewBag.Selected_Plant_ID = RS_Lines.Plant_ID;

            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Model_ID = new SelectList(db.RS_Model_Master.Where(p => p.Plant_ID == plantId), "Model_ID", "Model_Code");
            ViewBag.Style_Code = new SelectList(db.RS_Style_Code.Where(p => p.Plant_ID == plantId), "Style_Code", "Style_Code");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            return View(vehicleSeriesConfig);
        }
        // GET: /Plants/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Vehicle_Series series = db.RS_Vehicle_Series.Find(id);
            if (series == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.VehicleSeriesConfig;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "VehicleSeriesConfig";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(series);
        }


        /* Action Name                : DeleteConfirmed
        *  Description                : Action is used to delete line
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Line/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var seriesId = Convert.ToInt16(id);
            var seriesObj = new RS_Vehicle_Series();

            try
            {


                seriesObj = db.RS_Vehicle_Series.Find(seriesId);
                db.RS_Vehicle_Series.Remove(seriesObj);
                db.SaveChanges();


                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.VehicleSeriesConfig;
                globalData.messageDetail = ResourceModules.VehicleSeriesConfig + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.VehicleSeriesConfig;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete", seriesObj);
            }
        }

        public ActionResult GetVehicleSeriesByShopID(int shopId)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            try
            {
                var vehicleSeriesObj = db.RS_Vehicle_Series.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopId).ToList();
                var st = (from series in db.RS_Attribution_Parameters
                         where series.Plant_ID == plantID && series.Shop_ID == shopId && series.Attribute_Type.ToLower() == "Vehicle Series"
                         select new
                         {
                             Attribute_Desc = series.Attribute_Desc,
                             Attribute_ID = series.Attribute_ID,
                         }).ToList();
          
        
                
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
       
    }
}
