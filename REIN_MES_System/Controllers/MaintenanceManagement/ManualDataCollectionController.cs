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
using ZHB_AD.Helper;
using ZHB_AD.Controllers.BaseManagement;

namespace ZHB_AD.Controllers
{
    /* Class Name               : ManualDataCollection
  *  Description                : To create,edit,delete,show all data and Collect 
  *  Author, Timestamp          : Vijaykuumar Kagne       
  */
    public class ManualDataCollectionController : BaseController
    {
        #region Variable Declaration
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public GlobalData globalData = new GlobalData();
        MM_Manual_Data_Collection mM_ManualDataCollection = new MM_Manual_Data_Collection();
        static bool isflag = false;
        #endregion

        General generalObj = new General();

        #region Show all manual data collected or specific data
        /*
         * Action Name          : Index
         * Input Parameter      : None
         * Return Type          : ActionResult
         * Author & Time Stamp  : Vijaykumar Kagne
         * Description          : Get the list of All Collected Data will be showed to user(Manual Data Collection)
         */
        // GET: ManualDataCollection
        public ActionResult getShiftbyShop(int ShopID)
        {
            int PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            var res = db.MM_Shift.Where(s => s.Shop_ID == ShopID && s.Plant_ID == PlantID).Select(s => new { ID = s.Shift_ID, Value = s.Shift_Name }).ToList();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {

            globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
            globalData.controllerName = "Manual Data Collection";
            ViewBag.GlobalDataModel = globalData;
            var mM_Manual_Data_Collection = db.MM_Manual_Data_Collection.Include(m => m.MM_Employee).Include(m => m.MM_Employee1).Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shift).Include(m => m.MM_Shops).Include(m => m.MM_Stations);
            return View(mM_Manual_Data_Collection.ToList());
        }

        /*
         * Action Name          : Details
         * Input Parameter      : id
         * Return Type          : ActionResult
         * Author & Time Stamp  : Vijaykumar Kagne
         * Description          : Get the details specified Collected Data will be showed to user(Manual Data Collection)
         */
        // GET: ManualDataCollection/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Manual_Data_Collection mM_Manual_Data_Collection = db.MM_Manual_Data_Collection.Find(id);
            if (mM_Manual_Data_Collection == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
            globalData.controllerName = "Manual Data Collection";
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Manual_Data_Collection);
        }
        #endregion

        #region Inserting Manual data by user like Pressure,Concentration,Tempreture
        // GET: ManualDataCollection/Create
        /*
         * Action Name          : Create
         * Input Parameter      : none
         * Return Type          : ActionResult
         * Author & Time Stamp  : Vijaykumar Kagne
         * Description          : Add Manually data with respect to machine (Load form)
         */
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
            globalData.controllerName = "Manual Data Collection";
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shift_ID = new SelectList(db.MM_Shift, "Shift_ID", "Shift_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
            ViewBag.Parameter_ID = new SelectList(db.MM_MT_Parameter, "Parameter_ID", "Parameter_Name");
            return View();
        }


        /*
        * Action Name          : Create
        * Input Parameter      : MM_MT_MAnaul_Data_Collection object
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Add Manually data with respect to machine (Manual Data Collection)
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MDC_ID,Frequency,Plant_ID,Shop_ID,Line_ID,Parameter_ID,Station_ID,Shift_ID,Minimum_Value,Maximum_Value,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host")] MM_Manual_Data_Collection mM_Manual_Data_Collection)
        {
            mM_Manual_Data_Collection.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                mM_Manual_Data_Collection.Inserted_Date = DateTime.Now;
                mM_Manual_Data_Collection.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                mM_Manual_Data_Collection.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.MM_Manual_Data_Collection.Add(mM_Manual_Data_Collection);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = "Manual Data Collection";
                globalData.messageDetail = "Data Saved Successfully";
                ViewBag.GlobalDataModel = globalData;
                return RedirectToAction("Index");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Manual_Data_Collection.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Manual_Data_Collection.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_Manual_Data_Collection.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Manual_Data_Collection.Plant_ID);
            ViewBag.Shift_ID = new SelectList(db.MM_Shift, "Shift_ID", "Shift_Name", mM_Manual_Data_Collection.Shift_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_Manual_Data_Collection.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_Manual_Data_Collection.Station_ID);
            globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
            globalData.controllerName = "Manual Data Collection";
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Manual_Data_Collection);
        }

        #endregion

        #region Edit specified manual Data
        /*
        * Action Name          : Edit
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Edit Manually data with respect to machine (Load form with specified Manual Data Collection)
        */
        // GET: ManualDataCollection/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Manual_Data_Collection mM_Manual_Data_Collection = db.MM_Manual_Data_Collection.Find(id);
            if (mM_Manual_Data_Collection == null)
            {
                return HttpNotFound();
            }

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Manual_Data_Collection.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Manual_Data_Collection.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_Manual_Data_Collection.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Manual_Data_Collection.Plant_ID);
            ViewBag.Shift_ID = new SelectList(db.MM_Shift, "Shift_ID", "Shift_Name", mM_Manual_Data_Collection.Shift_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_Manual_Data_Collection.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_Manual_Data_Collection.Station_ID);
            ViewBag.Parameter_ID = new SelectList(db.MM_MT_Parameter, "Parameter_ID", "Parameter_Name", mM_Manual_Data_Collection.Parameter_ID);
            globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
            globalData.controllerName = "Manual Data Collection";
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Manual_Data_Collection);
        }

        /*
      * Action Name          : Create
      * Input Parameter      : MM_MT_MAnaul_Data_Collection object
      * Return Type          : ActionResult
      * Author & Time Stamp  : Vijaykumar Kagne
      * Description          : Edit Manually data with respect to machine (Manual Data Collection)
      */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MDC_ID,Frequency,Plant_ID,Shop_ID,Parameter_ID,Line_ID,Station_ID,Shift_ID,Minimum_Value,Maximum_Value,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host")] MM_Manual_Data_Collection mM_Manual_Data_Collection)
        {
            mM_ManualDataCollection = db.MM_Manual_Data_Collection.Find(mM_Manual_Data_Collection.MDC_ID);
            if (ModelState.IsValid)
            {
                mM_ManualDataCollection.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                mM_ManualDataCollection.Shop_ID = mM_Manual_Data_Collection.Shop_ID;
                mM_ManualDataCollection.Shift_ID = mM_Manual_Data_Collection.Shift_ID;
                mM_ManualDataCollection.Line_ID = mM_Manual_Data_Collection.Line_ID;
                mM_ManualDataCollection.Station_ID = mM_Manual_Data_Collection.Station_ID;

                mM_ManualDataCollection.Parameter_ID = mM_Manual_Data_Collection.Parameter_ID;
                mM_ManualDataCollection.Minimum_Value = mM_Manual_Data_Collection.Minimum_Value;
                mM_ManualDataCollection.Maximum_Value = mM_Manual_Data_Collection.Maximum_Value;
                mM_ManualDataCollection.Frequency = mM_Manual_Data_Collection.Frequency;

                mM_ManualDataCollection.Updated_Date = DateTime.Now;
                mM_ManualDataCollection.Updated_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                mM_ManualDataCollection.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_ManualDataCollection.Is_Edited = true;
                db.Entry(mM_ManualDataCollection).State = EntityState.Modified;
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = "Manual Data Collection";
                globalData.messageDetail = "Data Edited Successfully";
                ViewBag.GlobalDataModel = globalData;
                return RedirectToAction("Index");
            }
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Manual_Data_Collection.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Manual_Data_Collection.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_Manual_Data_Collection.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Manual_Data_Collection.Plant_ID);
            ViewBag.Shift_ID = new SelectList(db.MM_Shift, "Shift_ID", "Shift_Name", mM_Manual_Data_Collection.Shift_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_Manual_Data_Collection.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_Manual_Data_Collection.Station_ID);
            ViewBag.Parameter_ID = new SelectList(db.MM_MT_Parameter, "Parameter_ID", "Parameter_Name", mM_Manual_Data_Collection.Parameter_ID);
            globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
            globalData.controllerName = "Manual Data Collection";
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Manual_Data_Collection);
        }
        #endregion

        #region Delete Specified Manual Data
        /*
        * Action Name          : Delete
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Delete Manually data with respect to machine (Load form with specified Manual Data Collection)
        */
        // GET: ManualDataCollection/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Manual_Data_Collection mM_Manual_Data_Collection = db.MM_Manual_Data_Collection.Find(id);
            if (mM_Manual_Data_Collection == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
            globalData.controllerName = "Manual Data Collection";
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Manual_Data_Collection);
        }

        /*
        * Action Name          : Delete
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Delete Manually data with respect to machine (Load form with specified Manual Data Collection)
        */
        // POST: ManualDataCollection/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_Manual_Data_Collection mM_Manual_Data_Collection = db.MM_Manual_Data_Collection.Find(id);
            db.MM_Manual_Data_Collection.Remove(mM_Manual_Data_Collection);
            db.SaveChanges();
            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_Manual_Data_Collection", "MDC_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

            globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
            globalData.controllerName = "Manual Data Collection";
            ViewBag.GlobalDataModel = globalData;
            return RedirectToAction("Index");
        }
        #endregion

        #region Disposing objects (releasing memory)
        /*
        * Action Name          : Dispose
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Dispose memory 
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Filling child Dropdown with respect to parent
        /*
        * Action Name          : GetShopByPlantID
        * Input Parameter      : plantid
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Get List of All Shop within Plant
        */
        public ActionResult GetShopByPlantID(int plantid)
        {
            var Shops = db.MM_Shops.Where(c => c.Plant_ID == plantid).Select(a => new { a.Shop_ID, a.Shop_Name });
            return Json(Shops, JsonRequestBehavior.AllowGet);
        }

        /*
        * Action Name          : GetLineByShopID
        * Input Parameter      : shopid
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Get List of All Line within Shop
        */
        public ActionResult GetLineByShopID(int shopid)
        {
            var Lines = db.MM_Lines.Where(c => c.Shop_ID == shopid).Select(a => new { a.Line_ID, a.Line_Name });
            return Json(Lines, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShiftByShopID(int shopid)
        {
            var Shifts = db.MM_Shift.Where(c => c.Shop_ID == shopid).Select(a => new { a.Shift_ID, a.Shift_Name });
            return Json(Shifts, JsonRequestBehavior.AllowGet);
        }

        /*
        * Action Name          : GetStationByLineID
        * Input Parameter      : lineid
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Get List of All Station within Line
        */
        public ActionResult GetStationByLineID(int lineid)
        {
            var stations = db.MM_Stations.Where(c => c.Line_ID == lineid).Select(a => new { a.Station_ID, a.Station_Name });
            return Json(stations, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region manualy take data from shop user of temp,pressure,concentration

        /*
        * Action Name          : Collect
        * Input Parameter      : none
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Get details and load form
        */
        public ActionResult Collect()

        {
            decimal station_id = ((FDSession)this.Session["FDSession"]).stationId;
            decimal shop_id = ((FDSession)this.Session["FDSession"]).shopId;
            List<MM_Manual_Data_Collection> _shopManualDataCollection = new List<MM_Manual_Data_Collection>();
            List<MM_MT_Shop_Manual_Data_Collection> _lstShopMDC = new List<MM_MT_Shop_Manual_Data_Collection>();
            TimeSpan currDate = DateTime.Now.AddHours(1).TimeOfDay;
            double hh = currDate.TotalHours;
            var shiftObj = (from shift in db.MM_Shift

                            where
                            shift.Shop_ID == shop_id &&
                            TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                            && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                            select new
                            {
                                Id = shift.Shift_ID,
                                value = shift.Shift_Start_Time
                            }).FirstOrDefault();
            double Hours = shiftObj.value.TotalHours;
            //if (hh > Hours)
            //{
            _shopManualDataCollection = db.MM_Manual_Data_Collection.Where(x => x.Station_ID == station_id && x.Shift_ID == shiftObj.Id).ToList();
            //foreach (var item in _shopManualDataCollection.Select(x=>x.MDC_ID).ToList())
            //{
            TempData["MDC"] = _shopManualDataCollection;
            var freq = 0;
            if (_shopManualDataCollection.Count > 0)
            {
                foreach (var item in _shopManualDataCollection)
                {
                    freq = Convert.ToInt32(item.Frequency);
                }
            }
            if (db.MM_MT_Shop_Manual_Data_Collection.Where(x => x.Station_ID == station_id && x.Shift_ID == shiftObj.Id && (x.Inserted_Date.Year == DateTime.Now.Year && x.Inserted_Date.Month == DateTime.Now.Month && x.Inserted_Date.Day == DateTime.Now.Day)).Count() > 0)
            {
                // _shopManualDataCollection = new List<MM_Manual_Data_Collection>();
                _lstShopMDC.AddRange(db.MM_MT_Shop_Manual_Data_Collection.Where(x => x.Station_ID == station_id && x.Inserted_Date.Year == DateTime.Now.Year && x.Inserted_Date.Month == DateTime.Now.Month && x.Inserted_Date.Day == DateTime.Now.Day).ToList());
            }
            var count1 = _lstShopMDC.Count;
            var count = 0;
            MM_MT_Shop_Manual_Data_Collection _mdc = new MM_MT_Shop_Manual_Data_Collection();
            List<MM_Manual_Data_Collection> MDC = (List<MM_Manual_Data_Collection>)TempData["MDC"];
            List<MM_MT_Shop_Manual_Data_Collection> _lstShopMDC1 = new List<MM_MT_Shop_Manual_Data_Collection>();
            if (count1 > 0 && freq > count1)
            {
                for (int i = 0; count < freq - count1;)
                {
                    _mdc.MDC_ID = MDC[i].MDC_ID;
                    _mdc.Inserted_Date = DateTime.Now;
                    _mdc.Inserted_Host = Request.UserHostAddress;
                    _mdc.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    _mdc.MM_Lines = MDC[i].MM_Lines;
                    _mdc.MM_Shops = MDC[i].MM_Shops;
                    //_mdc.MM_Shift = MDC[i].MM_Shift;
                    _mdc.MM_Stations = MDC[i].MM_Stations;
                    //_mdc.MM_Plants = MDC[i].MM_Plants;
                    _mdc.MM_MT_Parameter = MDC[i].MM_MT_Parameter;
                    _mdc.Minimum_Value = MDC[i].Minimum_Value;
                    _mdc.Maximum_Value = MDC[i].Maximum_Value;
                    _lstShopMDC1.Add(_mdc);
                    count++;
                }
                TempData["NullData"] = _lstShopMDC1;
            }
            //}
            //}
            //else
            //{
            //    //_shopManualDataCollection = db.MM_Manual_Data_Collection.Where(x => x.Station_ID == station_id).ToList();
            //    //if (db.MM_MT_Shop_Manual_Data_Collection.Where(x => x.Station_ID == station_id && (x.Inserted_Date.Year == DateTime.Now.Year && x.Inserted_Date.Month == DateTime.Now.Month && x.Inserted_Date.Day == DateTime.Now.Day)).Count() > 0)
            //    //{
            //    //    _shopManualDataCollection = new List<MM_Manual_Data_Collection>();
            //    //    _lstShopMDC.AddRange(db.MM_MT_Shop_Manual_Data_Collection.Where(x => x.Inserted_Date.Year == DateTime.Now.Year && x.Inserted_Date.Month == DateTime.Now.Month && x.Inserted_Date.Day == DateTime.Now.Day).ToList());
            //    //}


            //    //ticket generation code appears here for Manual Data Collection

            //    #region Open Ticket
            //    MM_MT_Ticket mticket = new MM_MT_Ticket();

            //    mticket.Shop_ID = ((FDSession)this.Session["FDSession"]).shopId;
            //    mticket.Station_ID = ((FDSession)this.Session["FDSession"]).stationId;
            //    mticket.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            //    mticket.Line_ID = ((FDSession)this.Session["FDSession"]).lineId;
            //    mticket.Ticket_Number = "MDCT" + DateTime.Now.ToString();
            //    mticket.Ticket_Details = "Data is not collected from  station yet";
            //    mticket.Is_Mail_Sent = false;
            //    mticket.Is_Ticket_Closed = false;
            //    mticket.Maintenance_Type = 7;

            //    mticket.Inserted_Date = DateTime.Now;
            //    mticket.Inserted_Host = Request.UserHostAddress;
            //    mticket.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

            //    db.MM_MT_Ticket.Add(mticket);
            //    db.SaveChanges();

            //    string param = "";

            //    foreach (var item in _shopManualDataCollection.Select(x => x.Parameter_ID).ToList())
            //    {
            //        param = db.MM_MT_Parameter.Where(x => x.Parameter_ID == item).FirstOrDefault().Parameter_Name + ", ";

            //    }
            //    string bdy = "Dear All, <br/> We would like to inform you that Today there was no data colleted for any parameter, System is going to open ticket so kindly assist system and help it to close ticket. The following are details about stations with its parameters.<br/> Station Name :" + ((FDSession)this.Session["FDSession"]).stationName + "<br/> Parameters :" + param + " <br/> The given link is to close ticket please click on link to close ticket. <br/>Link : " + Request.Url + "_" + mticket.T_ID + " Reagrds,<br/>MES Team";

            //    string subject = "Manual Data Collection ALERT " + DateTime.Now.ToString();
            //    GlobalOperations.sendMail("vijaykumar.kagne@gmail.com", "vijaykumar.kagne@mestechservices.com", subject, bdy);
            //    TempData["globalData"] = globalData;
            //    ViewBag.GlobalDataModel = globalData;
            //    #endregion

            //}

            globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
            globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
            globalData.subTitle = "Collect";
            globalData.controllerName = "Manual Data Collection";
            ViewBag.GlobalDataModel = globalData;

            ViewData["Data"] = _lstShopMDC;
            ViewBag.Plant_ID = db.MM_Plants.Where(x => x.Plant_ID == db.MM_Route_Configurations.FirstOrDefault().Plant_ID).First().Plant_Name.ToString();
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
            ViewBag.Parameter_ID = new SelectList(db.MM_MT_Parameter, "Parameter_ID", "Parameter_Name");
            TempData["MDC"] = _shopManualDataCollection;
            return View(_shopManualDataCollection);
        }

        /*
       * Action Name          : Collect
       * Input Parameter      : FormCollection(Form all controls)
       * Return Type          : ActionResult
       * Author & Time Stamp  : Vijaykumar Kagne
       * Description          : Collect Data  from user and save into database
       */
        [HttpPost]
        public ActionResult Collect(FormCollection fc)
        {
            try
            {
                string[] dataValues = fc["Current_Value"].ToString().Split(new char[] { ',' });
                string[] Remark = fc["Remark"].ToString().Split(new char[] { ',' });
                decimal station_id = ((FDSession)this.Session["FDSession"]).stationId;
                decimal shop_id = ((FDSession)this.Session["FDSession"]).shopId;
                List<MM_Manual_Data_Collection> _shopManualDataCollection = new List<MM_Manual_Data_Collection>();
                List<MM_MT_Shop_Manual_Data_Collection> _lstShopMDC = new List<MM_MT_Shop_Manual_Data_Collection>();
                TimeSpan currDate = DateTime.Now.AddHours(1).TimeOfDay;
                var shiftObj = (from shift in db.MM_Shift

                                where
                                shift.Shop_ID == shop_id &&
                                TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                                && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                                select new
                                {
                                    Id = shift.Shift_ID,
                                    value = shift.Shift_Start_Time
                                }).FirstOrDefault();
                _shopManualDataCollection = db.MM_Manual_Data_Collection.Where(x => x.Station_ID == station_id && x.Shift_ID == shiftObj.Id).ToList();
                TempData["MDC"] = _shopManualDataCollection;
                List<MM_Manual_Data_Collection> MDC = (List<MM_Manual_Data_Collection>)TempData["MDC"];
                List<MM_MT_Shop_Manual_Data_Collection> shopMDc = new List<MM_MT_Shop_Manual_Data_Collection>();
                List<MM_MT_Shop_Manual_Data_Collection> _lstShopMDC1 = new List<MM_MT_Shop_Manual_Data_Collection>();

                for (int i = 0; i < MDC.Count; i++)
                {
                    int frequency = Convert.ToInt32(MDC[i].Frequency);
                    frequency = dataValues.Count();
                    for (int j = 0; j < frequency; j++)
                    {
                        MM_MT_Shop_Manual_Data_Collection _mdc = new MM_MT_Shop_Manual_Data_Collection();
                        _mdc.Current_Value = dataValues[j] != "" ? Convert.ToDecimal(dataValues[j]) : 0;

                        if (_mdc.Current_Value != 0)
                        {
                            _mdc.MDC_ID = MDC[i].MDC_ID;
                            _mdc.Inserted_Date = DateTime.Now;
                            _mdc.Inserted_Host = Request.UserHostAddress;
                            _mdc.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            _mdc.Line_ID = MDC[i].Line_ID;
                            _mdc.Shop_ID = MDC[i].Shop_ID;
                            _mdc.Shift_ID = MDC[i].Shift_ID;
                            _mdc.Station_ID = MDC[i].Station_ID;
                            //  _mdc.Frequency = frequency;
                            _mdc.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                            _mdc.Parameter_ID = MDC[i].Parameter_ID;
                            _mdc.Minimum_Value = MDC[i].Minimum_Value;
                            _mdc.Maximum_Value = MDC[i].Maximum_Value;
                            _mdc.Remark = Remark[j];
                            shopMDc.Add(_mdc);
                            db.MM_MT_Shop_Manual_Data_Collection.Add(_mdc);
                            db.SaveChanges();
                        }
                        else
                        {

                            _mdc.MDC_ID = MDC[i].MDC_ID;
                            _mdc.Inserted_Date = DateTime.Now;
                            _mdc.Inserted_Host = Request.UserHostAddress;
                            _mdc.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            _mdc.MM_Lines = MDC[i].MM_Lines;
                            _mdc.MM_Shops = MDC[i].MM_Shops;
                            //_mdc.MM_Shift = MDC[i].MM_Shift;
                            _mdc.MM_Stations = MDC[i].MM_Stations;
                            //_mdc.MM_Plants = MDC[i].MM_Plants;
                            _mdc.MM_MT_Parameter = MDC[i].MM_MT_Parameter;
                            _mdc.Minimum_Value = MDC[i].Minimum_Value;
                            _mdc.Maximum_Value = MDC[i].Maximum_Value;
                            _lstShopMDC1.Add(_mdc);
                        }
                    }
                }

                TempData["NullData"] = _lstShopMDC1;
                globalData.pageTitle = ResourceModules.Mannual_Data_Collection;
                globalData.subTitle = "Collect";
                globalData.contentTitle = ResourceModules.Mannual_Data_Collection;
                globalData.controllerName = "Manual Data Collection";

                globalData.isSuccessMessage = true;
                globalData.messageDetail = ResourceGlobal.Data + " " + ResourceMessages.Save_Success;
                globalData.messageTitle = ResourceModules.Mannual_Data_Collection;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
                ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
                ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
                ViewBag.Shift_ID = new SelectList(db.MM_Shift, "Shift_ID", "Shift_Name");
                ViewBag.Parameter_ID = new SelectList(db.MM_MT_Parameter, "Parameter_ID", "Parameter_Name");

                return RedirectToAction("Collect");
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public ActionResult Collected(MM_MT_Shop_Manual_Data_Collection shopMDc)
        {
            isflag = true;

            return View(shopMDc);
        }

        #endregion
     
    }
}
