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
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class SerialNumberDataController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_Serial_Number_Data serialNO = new RS_Serial_Number_Data();
        GlobalData globalData = new GlobalData();

        General generalObj = new General();

        // GET: SerialNumberData
        public ActionResult Index()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.actionName = ResourceGlobal.Lists;

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Lists;

            ViewBag.GlobalDataModel = globalData;
            return View(db.RS_Serial_Number_Data.Where(p => p.Plant_ID == plantID).ToList());
        }

        // GET: SerialNumberData/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Serial_Number_Data RS_Serial_Number_Data = db.RS_Serial_Number_Data.Find(id);
            if (RS_Serial_Number_Data == null)
            {
                return HttpNotFound();
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Details;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Serial_Number_Data);
        }

        // GET: SerialNumberData/Create
        public ActionResult Create()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Part_No = new SelectList(db.RS_Model_Master.Where(p => p.Plant_ID == plantID), "Model_Code", "Model_Code");

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Create;

            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        // POST: SerialNumberData/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Row_ID,Plant_ID,Part_No,Plant_Code,Year_Code,Month_Code,Model,Varient_Group,Common,Tractor_Family,Brand,Engine_Family,Tractor_Model,Tractor_Feture_A,Tractor_Feture_B,Model_Year,Assembly_Plant,Model_Month,Trans_Series,Trans_Year,Trans_Month,Hyd_series,Hyd_year,Hyd_month,Prefix,Suffix,Running_Serial")] RS_Serial_Number_Data RS_Serial_Number_Data)
        {

            RS_Serial_Number_Data.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

            if (ModelState.IsValid)
            {
                db.RS_Serial_Number_Data.Add(RS_Serial_Number_Data);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageDetail = "Serial Number Configured Sucessfully";
                globalData.messageTitle = "Serial Number Data";
                ViewBag.GlobalDataModel = globalData;
                return RedirectToAction("Index");
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Create;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Serial_Number_Data);
        }

        // GET: SerialNumberData/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Serial_Number_Data RS_Serial_Number_Data = db.RS_Serial_Number_Data.Find(id);
            if (RS_Serial_Number_Data == null)
            {
                return HttpNotFound();
            }
            ViewBag.Part_No = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Code", RS_Serial_Number_Data.Part_No);
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Edit;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Serial_Number_Data);
        }

        // POST: SerialNumberData/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Row_ID,Plant_ID,Part_No,Plant_Code,Year_Code,Month_Code,Model,Varient_Group,Common,Tractor_Family,Brand,Engine_Family,Tractor_Model,Tractor_Feture_A,Tractor_Feture_B,Model_Year,Assembly_Plant,Model_Month,Trans_Series,Trans_Year,Trans_Month,Hyd_series,Hyd_year,Hyd_month,Prefix,Suffix,Running_Serial")] RS_Serial_Number_Data RS_Serial_Number_Data)
        {
            if (ModelState.IsValid)
            {
                RS_Serial_Number_Data.Is_Edited = true;
                db.Entry(RS_Serial_Number_Data).State = EntityState.Modified;
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageDetail = "Serial Number Edited Sucessfully";
                globalData.messageTitle = "Serial Number Data";
                ViewBag.GlobalDataModel = globalData;
                return RedirectToAction("Index");
            }
            ViewBag.Part_No = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Code", RS_Serial_Number_Data.Part_No);
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Edit;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Serial_Number_Data);
        }

        // GET: SerialNumberData/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Serial_Number_Data RS_Serial_Number_Data = db.RS_Serial_Number_Data.Find(id);
            if (RS_Serial_Number_Data == null)
            {
                return HttpNotFound();
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Delete;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Serial_Number_Data);
        }

        // POST: SerialNumberData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            RS_Serial_Number_Data RS_Serial_Number_Data = db.RS_Serial_Number_Data.Find(id);
            db.RS_Serial_Number_Data.Remove(RS_Serial_Number_Data);
            db.SaveChanges();
            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Serial_Number_Data", "Row_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
            return RedirectToAction("Index");
        }

        public ActionResult GetSerialNoDetails(string partno)
        {
            var GetSerialNoDetails = db.RS_Model_Master.Where(x => x.Model_Code.ToLower() == partno.ToLower()).FirstOrDefault().Config_ID;
            var obj = db.RS_Serial_Number_Configuration.Where(x => x.Config_ID == GetSerialNoDetails).Select(x => new { x.Config_ID, x.Serial_Logic });
            List<Tuple<string, string>> columnName = new List<Tuple<string, string>>();
            foreach (var item in obj)
            {
                var obj2 = item.Serial_Logic.Split(',').ToList();
                foreach (var itm in obj2)
                {
                    columnName.Add(new Tuple<string, string>(db.RS_Serial_Number_Columns.Where(x => x.Column_Name.ToLower() == itm.ToLower()).FirstOrDefault().Column_Name, db.RS_Serial_Number_Columns.Where(x => x.Column_Name.ToLower() == itm.ToLower()).FirstOrDefault().Column_Desc));
                }
            }
            var resultnew = new { Result = obj, Column = columnName };
            return Json(resultnew, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPartNoDetails(string PartNo)
        {
            var obj3 = db.RS_Serial_Number_Data.Where(x => x.Part_No.ToLower() == PartNo.ToLower()).LastOrDefault();
            return View(obj3);
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
