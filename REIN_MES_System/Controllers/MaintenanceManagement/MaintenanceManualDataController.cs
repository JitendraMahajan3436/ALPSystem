using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
using ZHB_AD.App_LocalResources;
using System.IO;
using System.Data.OleDb;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class MaintenanceManualDataController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        int plantId = 0;
        // GET: MaintenanceManualData
        public ActionResult Index()
        {
            globalData.pageTitle = ResourceModules.BreakDown_Data;
            ViewBag.GlobalDataModel = globalData;
            var mM_MT_Maintenance_Manual_Data = db.MM_MT_Maintenance_Manual_Data.Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations);
            return View(mM_MT_Maintenance_Manual_Data.ToList());
        }

        // GET: MaintenanceManualData/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Maintenance_Manual_Data mM_MT_Maintenance_Manual_Data = db.MM_MT_Maintenance_Manual_Data.Find(id);
            if (mM_MT_Maintenance_Manual_Data == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.BreakDown_Data;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Maintenance_Manual_Data);
        }

        // GET: MaintenanceManualData/Create
        public ActionResult Create()
        {
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
            //ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
            globalData.pageTitle = ResourceModules.BreakDown_Data;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        // POST: MaintenanceManualData/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Row_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,BreakDown_Date,Machine_No,DownTime,RepairTime,Problem,Root_Cause,Correction,Corrective_Action,Preventive_Action,Repaired_Date,Owner,Remark,Is_Transferred,Is_Purgeable,Is_Edited,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Maintenance_Manual_Data mM_MT_Maintenance_Manual_Data)
        {
            if (ModelState.IsValid)
            {
                db.MM_MT_Maintenance_Manual_Data.Add(mM_MT_Maintenance_Manual_Data);
                mM_MT_Maintenance_Manual_Data.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                mM_MT_Maintenance_Manual_Data.Inserted_Host = HttpContext.Request.UserHostAddress;
                mM_MT_Maintenance_Manual_Data.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_MT_Maintenance_Manual_Data.Inserted_Date = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_MT_Maintenance_Manual_Data.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Maintenance_Manual_Data.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_MT_Maintenance_Manual_Data.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_MT_Maintenance_Manual_Data.Station_ID);
            globalData.pageTitle = ResourceModules.BreakDown_Data;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Maintenance_Manual_Data);
        }

        /*
        * Action Name          : GetShopByPlantID
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action used to Get all shop under plant
        */
        //Update shop with respect to plant
        public ActionResult GetShopByPlantID(int plantid)
        {
            var Shops = db.MM_Shops.Where(c => c.Plant_ID == plantid).Select(a => new { a.Shop_ID, a.Shop_Name });
            return Json(Shops, JsonRequestBehavior.AllowGet);
        }

        /*
        * Action Name          : GetLineByShopID
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Varaprasad
        * Description          : Action used to Get all line under Shop
        */
        //Update Line with respective shop
        public ActionResult GetLineByShopID(int shopid)
        {
            var Lines = db.MM_Lines.Where(c => c.Shop_ID == shopid).Select(a => new { a.Line_ID, a.Line_Name }).OrderBy(x => x.Line_Name);
            return Json(Lines, JsonRequestBehavior.AllowGet);
        }

        //Update Stations with respective line
        public ActionResult GetStationsByLineID(int lineid)
        {
            var Stations = db.MM_Stations.Where(c => c.Line_ID == lineid).Select(a => new { a.Station_ID, a.Station_Name }).OrderBy(x => x.Station_Name);
            return Json(Stations, JsonRequestBehavior.AllowGet);
        }

        // GET: MaintenanceManualData/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Maintenance_Manual_Data mM_MT_Maintenance_Manual_Data = db.MM_MT_Maintenance_Manual_Data.Find(id);
            if (mM_MT_Maintenance_Manual_Data == null)
            {
                return HttpNotFound();
            }
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(a => a.Line_ID == mM_MT_Maintenance_Manual_Data.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", mM_MT_Maintenance_Manual_Data.Station_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(a => a.Shop_ID == mM_MT_Maintenance_Manual_Data.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", mM_MT_Maintenance_Manual_Data.Line_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Maintenance_Manual_Data.Shop_ID);
            globalData.pageTitle = ResourceModules.BreakDown_Data;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Maintenance_Manual_Data);
        }

        // POST: MaintenanceManualData/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_MT_Maintenance_Manual_Data mM_MT_Maintenance_Manual_Data)
        {
            MM_MT_Maintenance_Manual_Data mmobj = new MM_MT_Maintenance_Manual_Data();
            if (ModelState.IsValid)
            {
                mmobj = db.MM_MT_Maintenance_Manual_Data.Find(mM_MT_Maintenance_Manual_Data.Row_ID);
                mmobj.Shop_ID = mM_MT_Maintenance_Manual_Data.Shop_ID;
                mmobj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                mmobj.Line_ID = mM_MT_Maintenance_Manual_Data.Line_ID;
                mmobj.Station_ID = mM_MT_Maintenance_Manual_Data.Station_ID;
                mmobj.BreakDown_Date = mM_MT_Maintenance_Manual_Data.BreakDown_Date;
                mmobj.Machine_No = mM_MT_Maintenance_Manual_Data.Machine_No;
                mmobj.DownTime = mM_MT_Maintenance_Manual_Data.DownTime;
                mmobj.RepairTime = mM_MT_Maintenance_Manual_Data.RepairTime;
                mmobj.Problem = mM_MT_Maintenance_Manual_Data.Problem;
                mmobj.Root_Cause = mM_MT_Maintenance_Manual_Data.Root_Cause;
                mmobj.Correction = mM_MT_Maintenance_Manual_Data.Correction;
                mmobj.Preventive_Action = mM_MT_Maintenance_Manual_Data.Preventive_Action;
                mmobj.Corrective_Action = mM_MT_Maintenance_Manual_Data.Corrective_Action;
                mmobj.Repaired_Date = mM_MT_Maintenance_Manual_Data.Repaired_Date;
                mmobj.Owner = mM_MT_Maintenance_Manual_Data.Owner;
                mmobj.Remark = mM_MT_Maintenance_Manual_Data.Remark;
                mmobj.Updated_Date = DateTime.Now;
                mmobj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                mmobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mmobj.Is_Edited = true;
                db.Entry(mmobj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            mM_MT_Maintenance_Manual_Data.Updated_Date = DateTime.Now;
            mM_MT_Maintenance_Manual_Data.Updated_Host = HttpContext.Request.UserHostAddress;
            mM_MT_Maintenance_Manual_Data.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(a => a.Line_ID == mM_MT_Maintenance_Manual_Data.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", mM_MT_Maintenance_Manual_Data.Station_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(a => a.Shop_ID == mM_MT_Maintenance_Manual_Data.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", mM_MT_Maintenance_Manual_Data.Line_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Maintenance_Manual_Data.Shop_ID);
            globalData.pageTitle = ResourceModules.BreakDown_Data;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Maintenance_Manual_Data);
        }

        // GET: MaintenanceManualData/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Maintenance_Manual_Data mM_MT_Maintenance_Manual_Data = db.MM_MT_Maintenance_Manual_Data.Find(id);
            if (mM_MT_Maintenance_Manual_Data == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.BreakDown_Data;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Maintenance_Manual_Data);
        }

        // POST: MaintenanceManualData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MM_MT_Maintenance_Manual_Data mM_MT_Maintenance_Manual_Data = db.MM_MT_Maintenance_Manual_Data.Find(id);
            db.MM_MT_Maintenance_Manual_Data.Remove(mM_MT_Maintenance_Manual_Data);
            db.SaveChanges();
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

        public FileResult Download(string fileName)
        {
            var FileVirtualPath = "~/App_Data/" + fileName;

            string file = @"~\App_Data\MM_MT_Maintenance_Manual_Data.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }

        public ActionResult ExcelUpload()
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(p => p.Shop_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(p => p.Line_ID == 0), "Station_ID", "Station_Name");
            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.Where(p => p.Station_ID == 0), "Machine_ID", "Machine_Name");
           

            if (TempData["MaintenanceManualDataRecords"] != null)
            {
                ViewBag.MaintenanceManualDataRecords = TempData["MaintenanceManualDataRecords"];
             }

            globalData.pageTitle = ResourceModules.BreakDown_Data;
            globalData.subTitle = ResourceGlobal.Upload;
            globalData.controllerName = "MaintenanceManualData";
            globalData.actionName = ResourceGlobal.Upload;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelUpload(ExcelMaintenanceManualData formData)
        {
            if (ModelState.IsValid)
            {
                GlobalOperations globalOperations = new GlobalOperations();
                string fileName = Path.GetFileName(formData.Excel_File.FileName);
                string fileExtension = Path.GetExtension(formData.Excel_File.FileName);
                string fileLocation = Server.MapPath("~/App_Data/" + fileName);
                System.IO.File.Delete(fileName);
                DataTable dt = ExcelToDataTable(formData.Excel_File, fileLocation, fileExtension);

                //plantId = Convert.ToInt32(formData.Plant_ID);
                if (dt.Rows.Count > 0)
                {

                    MaintenanceManualDataRecords[] maintenanceManualDataRecordsObj = new MaintenanceManualDataRecords[dt.Rows.Count];
                    int i = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        string plantName = row[0].ToString();
                        string shopName = row[1].ToString();
                        string lineName = row[2].ToString();
                        string stationName = row[3].ToString();
                        string machineNo = row[4].ToString();
                        string BreakDown_Date = row[5].ToString().Split(' ')[0];
                        string Down_Time = ((System.DateTime)(row[6])).TimeOfDay.ToString();
                        string Repair_Time = ((System.DateTime)(row[7])).TimeOfDay.ToString();
                        string Problem = row[8].ToString();
                        string Root_Cause = row[9].ToString();
                        string Correction = row[10].ToString();
                        string Corrective_Action = row[11].ToString();
                        string Preventive_Action = row[12].ToString();
                        string Repaired_Date = row[13].ToString().Split(' ')[0];
                        string Owner = row[14].ToString();
                        string Remark = row[15].ToString();

                        maintenanceManualDataRecordsObj[i] = new MaintenanceManualDataRecords();
                        // MM_MT_Maintenance_Part mmMaintenancePart = new MM_MT_Maintenance_Part();
                        MM_MT_Maintenance_Manual_Data obj = new MM_MT_Maintenance_Manual_Data();
                        MaintenanceManualDataRecords mmMaintenanceManualDataObj = new MaintenanceManualDataRecords();

                        mmMaintenanceManualDataObj.Plant_Name = plantName; 
                        mmMaintenanceManualDataObj.Shop_Name = shopName;
                        mmMaintenanceManualDataObj.Line_Name = lineName;
                        mmMaintenanceManualDataObj.Station_Name = stationName;
                        mmMaintenanceManualDataObj.Machine_No = machineNo;
                        mmMaintenanceManualDataObj.BreakDown_Date = BreakDown_Date;
                        mmMaintenanceManualDataObj.DownTime = Down_Time;
                        mmMaintenanceManualDataObj.RepairTime = Repair_Time;
                        mmMaintenanceManualDataObj.Problem = Problem;
                        mmMaintenanceManualDataObj.Root_Cause = Root_Cause;
                        mmMaintenanceManualDataObj.Correction = Correction;
                        mmMaintenanceManualDataObj.Corrective_Action = Corrective_Action;
                        mmMaintenanceManualDataObj.Preventive_Action = Preventive_Action;
                        mmMaintenanceManualDataObj.Repaired_Date = Repaired_Date;
                        mmMaintenanceManualDataObj.Owner = Owner;
                        mmMaintenanceManualDataObj.Remark = Remark;

                        var plant = db.MM_Plants.Where(m => m.Plant_Name == plantName).Select(m => m.Plant_ID).ToList();
                        var plantId = plant.Count != 0 ? plant[0] : 0;
                        var shop = db.MM_Shops.Where(m => m.Shop_Name == shopName && m.Plant_ID == plantId).Select(m => m.Shop_ID).ToList();
                        var shopId = shop.Count != 0 ? shop[0] : 0;
                        var line = db.MM_Lines.Where(m => m.Line_Name == lineName && m.Shop_ID == shopId).Select(m => m.Line_ID).ToList();
                        var lineId = line.Count != 0 ? line[0] : 0;
                        var station = db.MM_Stations.Where(m => m.Station_Name == stationName && m.Line_ID == lineId).Select(m => m.Station_ID).ToList();
                        var stationId = station.Count != 0 ? station[0] : 0;
                        if (plantId == 0)
                        {
                            mmMaintenanceManualDataObj.SS_Error_Sucess = "Plant Name does not exist";
                        }
                        else if(shopId == 0)
                        {
                            mmMaintenanceManualDataObj.SS_Error_Sucess = "Shop Name does not exist";
                        }
                        else if(lineId == 0)
                        {
                            mmMaintenanceManualDataObj.SS_Error_Sucess = "Line Name does not exist";
                        }
                        else if(stationId == 0)
                        {
                            mmMaintenanceManualDataObj.SS_Error_Sucess = "Station Name does not exist";
                        }

                        else
                        {
                            
                            string[] values = Down_Time.Split(':');

                            TimeSpan DownTime = new TimeSpan(Convert.ToInt32( values[0]), Convert.ToInt32(values[1]), Convert.ToInt32(values[2]));
                            values = Repair_Time.Split(':');
                            TimeSpan RepairTime = new TimeSpan(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), Convert.ToInt32(values[2]));
                            obj.Plant_ID = plantId;
                            obj.Shop_ID = shopId;
                            obj.Line_ID = lineId;
                            obj.Station_ID = stationId;
                            obj.Machine_No = machineNo;
                            obj.BreakDown_Date = Convert.ToDateTime(BreakDown_Date);
                            obj.DownTime = DownTime;
                            obj.RepairTime = RepairTime;
                            obj.Problem = Problem;
                            obj.Root_Cause = Root_Cause;
                            obj.Correction = Correction;
                            obj.Corrective_Action = Corrective_Action;
                            obj.Preventive_Action = Preventive_Action;
                            obj.Repaired_Date = Convert.ToDateTime(Repaired_Date);
                            obj.Owner = Owner;
                            obj.Remark = Remark;
                            obj.Is_Transferred = false;
                            obj.Is_Purgeable = false;
                            obj.Inserted_Date = DateTime.Now;
                            obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.MM_MT_Maintenance_Manual_Data.Add(obj);
                            db.SaveChanges();
                            mmMaintenanceManualDataObj.SS_Error_Sucess = "Record Added Successfully";
                        }

                        maintenanceManualDataRecordsObj[i] = mmMaintenanceManualDataObj;
                        i = i + 1;
                    }
                    TempData["MaintenanceManualDataRecords"] = maintenanceManualDataRecordsObj;
                    ViewBag.MaintenanceManualDataRecords = maintenanceManualDataRecordsObj;
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.BreakDown_Data;
                    globalData.messageDetail = ResourceGlobal.Excel + " " + ResourceMessages.Upload_Success;
                    globalData.pageTitle = ResourceModules.BreakDown_Data;
                    globalData.subTitle = ResourceGlobal.Upload;
                    globalData.controllerName = "MaintenanceManualData";
                    globalData.actionName = ResourceGlobal.Upload;
                    ViewBag.GlobalDataModel = globalData;
                }
            }


            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            
            globalData.pageTitle = ResourceModules.BreakDown_Data;
            globalData.subTitle = ResourceGlobal.Upload;
            globalData.controllerName = "MaintenanceManualData";
            globalData.actionName = ResourceGlobal.Upload;
            
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public DataTable ExcelToDataTable(HttpPostedFileBase uploadFile, string fileLocation, string fileExtension)
        {
            DataTable dtExcelRecords = new DataTable();
            string connectionString = "";
            if (uploadFile.ContentLength > 0)
            {

                uploadFile.SaveAs(fileLocation);

                //Check whether file extension is xls or xslx

                if (fileExtension == ".xls")
                {
                    connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                else if (fileExtension == ".xlsx")
                {
                    connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }

                //Create OleDB Connection and OleDb Command && Read data from excel and generate datatable 

                OleDbConnection con = new OleDbConnection(connectionString);
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;
                OleDbDataAdapter dAdapter = new OleDbDataAdapter(cmd);

                con.Open();
                DataTable dtExcelSheetName = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string getExcelSheetName = dtExcelSheetName.Rows[0]["Table_Name"].ToString();
                cmd.CommandText = "SELECT * FROM [" + getExcelSheetName + "]";
                dAdapter.SelectCommand = cmd;
                dAdapter.Fill(dtExcelRecords);
                con.Close();

            }
            return dtExcelRecords;
        }
    }
}
