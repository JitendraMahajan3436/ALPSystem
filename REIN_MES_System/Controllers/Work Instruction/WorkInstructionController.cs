using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.WorkInstruction
{
    public class WorkInstructionController : BaseController
    {
        //private REIN_SOLUTION_MEntities db_1 = new REIN_SOLUTION_MEntities();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        MetaShift objShift = new MetaShift();
        // GET: WorkInstruction
        public ActionResult Index()
        {
            var rS_WorkInstruction_Master = db.RS_WorkInstruction_Master.Include(r => r.RS_Stations);
            return View(rS_WorkInstruction_Master.ToList());
        }

        //Shopfloor screen
        public ActionResult WorkInstructionSubmission()
        {
            try
            {
                
                DateTime today = DateTime.Today;
                var model = TempData["ModelCode"];
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                
                int shiftID = objShift.getShiftID();
                //for 3rd shift bypass after 12 
                decimal sID = Convert.ToDecimal(shiftID);
                string shiftName = db.RS_Shift.Where(shift => shift.Shift_ID == sID).Select(shift => shift.Shift_Name).FirstOrDefault();
                if (shiftName == "C")
                {
                    TimeSpan dt = DateTime.Now.TimeOfDay;
                    int hours = dt.Hours;
                    if (hours == 0 || (hours > 1 && hours < 7))
                    {
                        ViewBag.flag = 1;
                    }
                }
                ViewBag.Plant_ID_User = plantId;
                ViewBag.Station_ID = stationId;
              

                ViewBag.Shop_ID_User = shopId;
                ViewBag.Line_ID_User = lineId;
                ViewBag.Station_ID_User = stationId;
                var rS_WorkInstruction_Master = db.RS_WorkInstruction_Master.Include(r => r.RS_Stations);
                ViewBag.stationName = ((FDSession)this.Session["FDSession"]).stationName;
                return View(rS_WorkInstruction_Master.ToList());
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "User");
            }
         
        }
        private bool SendToPrint()
        {
            try
            {
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult CheckCurationTime(string orderno)
        {
            try
            {
                int StationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                bool order = db.RS_OM_Order_List.Any(c => c.Order_No == orderno);
                if (!order)
                {
                    return Json(new { status = false, message = "Order No not available" }, JsonRequestBehavior.AllowGet);
                }
                //checking curation time


                var CurrentCuration = db.RS_Station_Curation_Master.Where(c => c.Current_Station_ID == StationId).ToList();
                int cnt = CurrentCuration.Count();
                int actualcnt = 0;
                foreach (var item in CurrentCuration)
                {
                    actualcnt++;
                    if (item != null)
                    {

                        int curationtime = Convert.ToInt32(item.Curation_Time);
                        decimal lineId = db.RS_Stations.Find(item.Previous_Station_ID).Line_ID;
                        bool isSubass = db.RS_Lines.Any(c => c.Line_ID == lineId && c.Is_Sub_Assembly == true);
                        if(!isSubass)
                        {
                            var previousStation = db.RS_Order_Queue.Where(c => c.Order_No == orderno && c.Station_ID == item.Previous_Station_ID && c.Status == true && c.Is_All_Submitted==true).FirstOrDefault();
                            if (previousStation != null)
                            {

                                TimeSpan ts = System.DateTime.Now - Convert.ToDateTime(previousStation.Updated_Date);
                                if (ts.TotalMinutes >= curationtime)
                                {
                                    return Json(new { status = true, message = "Order Number Scan Successfully.." }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new { status = false, message = "Curation Time not completed.Wait for " + Convert.ToDouble(String.Format("{0:0.00}", (curationtime - (ts.TotalMinutes)))) + " Minutes." }, JsonRequestBehavior.AllowGet);
                                }

                            }
                            else
                            {
                                if (cnt == actualcnt)
                                {
                                    return Json(new { status = false, message = "Order not completed at previous station" }, JsonRequestBehavior.AllowGet);

                                }
                            }
                        }
                      
                    }
                }


             
                return Json(new { status = true, message = "Order No Scan Successfully.." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = true, message = "Error in curation.." }, JsonRequestBehavior.AllowGet);
            }
          
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult CheckBOMPart(string MainOrder_No, string orderno, string partNo,int currentWIID)
        {
            try
            {
                
               

                if (partNo == null || orderno == null || MainOrder_No==null)
                {
                    return Json(new { status = false, message = "Part number or Order number is Empty." }, JsonRequestBehavior.AllowGet);
                }
                var workInstr = db.RS_WorkInstruction_Master.Find(currentWIID);
                //order started and all workinstruction completed ok checking

                RS_OM_Order_List ordrlist = db.RS_OM_Order_List.Where(c => c.Order_No.ToUpper() == MainOrder_No.Trim().ToUpper()).FirstOrDefault();
                if (ordrlist == null)
                {
                    return Json(new { status = false, message = "Final Assembly Order not Started yet..." + MainOrder_No }, JsonRequestBehavior.AllowGet);
                }

                bool isbomvalid = db.RS_BOM_Item.Any(c => c.Model_Code == ordrlist.Model_Code && c.Part_No.ToUpper() == partNo.ToUpper().Trim());
                if (isbomvalid && partNo.ToUpper() == workInstr.Part_No.ToUpper())
                {
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Wrong Error Proofing(BOM or Workinstruction Part No)..!" }, JsonRequestBehavior.AllowGet);
                }

                bool isExist = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno.ToUpper() && c.Part_No.ToUpper() == partNo.ToUpper() && c.Order_Status != "Closed");
                if (isExist)
                {
                    bool isOkOrder = db.RS_Order_Queue.Any(c => c.Order_No.ToUpper() == orderno.ToUpper() && c.Model_Code.ToUpper() == partNo.ToUpper() && c.Status == true && c.Is_All_Submitted == true);
                    if (!isOkOrder)
                    {
                        return Json(new { status = false, message = "Clear the All workinstructions against order- " + orderno }, JsonRequestBehavior.AllowGet);

                    }
                    //else
                    //{
                    //    //return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);

                    //}
                    //RS_OM_SubAssembly_Order_List obj = db.RS_OM_SubAssembly_Order_List.Where(c => c.Order_No.ToUpper() == orderno.ToUpper() && c.Part_No.ToUpper() == partNo.ToUpper()).FirstOrDefault();
                    //return Json(new { status = false, SubAssemly = obj, message = "Already" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    bool isExistwithClosed = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno.ToUpper() && c.Part_No.ToUpper() == partNo.ToUpper() && c.Order_Status == "Closed");
                    if (isExistwithClosed)
                    {
                        return Json(new { status = false, message = "Sub Assembly Order Already attached to another Main Assembly..." }, JsonRequestBehavior.AllowGet);

                    }

                    return Json(new { status = false, message = "Sub Assembly Order not Started yet..." }, JsonRequestBehavior.AllowGet);
                }
                //else
                

            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Exception" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult CheckAssemblyPartNo(string partNo)
        {
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            var result = db.RS_Line_Part_Assembly.Where(c=>c.Part_No.ToUpper()== partNo.Trim().ToUpper() && c.Line_ID==lineId).FirstOrDefault();
            if(result==null)
            {
                return Json(new { status = false, message = "Please configured part number against the Line" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = true,data= result, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult CheckAlredayAllSubmitted(string orderNo,string partNo)
        {
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            bool result = db.RS_Order_Queue.Any(c => c.Model_Code.ToUpper() == partNo.Trim().ToUpper() &&
                            c.Order_No.ToUpper() == orderNo.Trim().ToUpper() && c.Station_ID == stationId && c.Is_All_Submitted == true);
   
            return Json(new { status = result, message = "Workinstruction already Submitted against Order No-"+orderNo }, JsonRequestBehavior.AllowGet);
       
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult CheckSubAssemblyCurationTime(string orderno,string partNo)
        {
            try
            {
                int StationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                bool isOrderStartStation = db.RS_Stations.Any(c=>c.Station_ID==StationId && c.Is_Sub_Assembly_OS==true);
                bool order = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno.ToUpper().Trim() && c.Part_No.ToUpper()== partNo.Trim().ToUpper());
                if(!isOrderStartStation)
                {
                    if (!order)
                    {
                        return Json(new { status = false, message = "Order No not available" }, JsonRequestBehavior.AllowGet);
                    }
                }
               
                //checking curation time
                var CurrentCuration = db.RS_Station_Curation_Master.Where(c => c.Current_Station_ID == StationId).ToList();
                int cnt = CurrentCuration.Count();
                int actualcnt = 0;
                foreach (var item in CurrentCuration)
                {
                    actualcnt++;
                    if (item != null)
                    {

                        int curationtime = Convert.ToInt32(item.Curation_Time);
                        var previousStation = db.RS_Order_Queue.Where(c => c.Order_No == orderno && c.Model_Code.ToUpper() == partNo.Trim().ToUpper() && c.Station_ID == item.Previous_Station_ID && c.Status == true && c.Is_All_Submitted == true).FirstOrDefault();
                        if (previousStation != null)
                        {
                            
                            TimeSpan ts = System.DateTime.Now - Convert.ToDateTime(previousStation.Updated_Date);
                            if (ts.TotalMinutes >= curationtime)
                            {
                                return Json(new { status = true, message = "Order Number Scan Successfully.." }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { status = false, message = "Curation Time not completed.Wait for " + Convert.ToDouble(String.Format("{0:0.00}", (curationtime - (ts.TotalMinutes)))) + " Minutes." }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else
                        {
                            if (cnt == actualcnt)
                            {
                                return Json(new { status = false, message = "Order not completed at previous station" }, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                }



                return Json(new { status = true, message = "Order No Scan Successfully.." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = true, message = "Error in curation.." }, JsonRequestBehavior.AllowGet);
            }

        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult UpdateAllSubmitted(string orderno, string partNo,int StationId)
        {
            try
            {
                RS_Order_Queue result = db.RS_Order_Queue.Where(c => c.Order_No.ToUpper() == orderno.Trim().ToUpper()
                  && c.Model_Code.ToUpper() == partNo.Trim().ToUpper() && c.Station_ID == StationId).FirstOrDefault();
                if(result!=null)
                {
                    int userId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId);
                    int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                    string userHost = Convert.ToString(((FDSession)this.Session["FDSession"]).userId);
                    result.Is_All_Submitted = true;
                    result.Updated_Date = System.DateTime.Now;
                    result.Updated_Host = userHost;
                    result.Updated_User_ID = userId;
                    db.Entry(result).State = EntityState.Modified;
                    db.SaveChanges();

                    bool isDivorceStation = db.RS_Stations.Any(c=>c.Station_ID==stationId && c.Is_Carier_Divorced==true);
                    if(isDivorceStation)
                    {
                        DivorceCarrier(orderno, partNo);
                    }

                }
                return Json(new { status = true, message = "All Workinstruction Submitted Successfully..." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { status = false,  message = "Exception during update data" }, JsonRequestBehavior.AllowGet);
            }
        }
            [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getOrderDetail(string orderno)
        {
            RS_OM_Order_List isValidorderWork = db.RS_OM_Order_List.Where(c=>c.Order_No.ToUpper() == orderno.Trim().ToUpper()).FirstOrDefault();
            if (isValidorderWork != null)
            {
                return Json(new { status = true,OrderDetail= isValidorderWork, message = "Already" }, JsonRequestBehavior.AllowGet);
            }
            else 
            {
                return Json(new { status = false, message = "Order Number not available - "+ orderno }, JsonRequestBehavior.AllowGet);
            }
        }
        public decimal getShiftId()
        {

            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            RS_Quality_Captures shift = new RS_Quality_Captures();
            var shiftdata = shift.getCurrentRunningShiftByShopID(shopId);
            if(shiftdata==null)
            {
                return 0;
            }
            return shiftdata.Shift_ID;
        }

        //for main/sub assmbly isValidOrderno
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult isValidOrderno(string orderno,string partNumber, int StationId,bool IsSubAssmbly,bool IsMainAssmbly,int currentWIID)
        {
            try
            {
                orderno = orderno.Trim().ToUpper();
                partNumber = partNumber.Trim().ToUpper();
                if (IsSubAssmbly)
                {
                    bool isValidorder = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper()== partNumber);
                   
              
                         RS_WorkInstruction_Submitted isValidorderWork = db.RS_WorkInstruction_Submitted.Find(currentWIID);
                            if (isValidorderWork!=null && isValidorder==true)
                            {
                                return Json(new { status = false, message = "Already" }, JsonRequestBehavior.AllowGet);
                            }
                            else if (isValidorder)
                            {
                                return Json(new { status = true, message = "Order Number Scan Successfully.." }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { status = false, message = "Order Number not available-" + orderno }, JsonRequestBehavior.AllowGet);
                            }

                }
                else if(IsMainAssmbly)
                {
                    //bool isValidcarrier = db.RS_Carrier_Orderlist.Any(c => c.Order_No == orderno);
                    bool isValidorder = db.RS_OM_Order_List.Any(c => c.Order_No == orderno && c.Model_Code.ToUpper()==partNumber);

                    RS_WorkInstruction_Submitted isValidorderWork = db.RS_WorkInstruction_Submitted.Find(currentWIID);
                    if (isValidorderWork != null && isValidorder == true)
                    {
                        return Json(new { status = false, message = "Already" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (isValidorder)
                    {
                        return Json(new { status = true, message = "Order Number Scan Successfully.." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, message = "Order Number not available" + orderno }, JsonRequestBehavior.AllowGet);
                    }

                }
                return Json(new { status = false, message = "Order Number not available" + orderno }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = false, message = "Exception" }, JsonRequestBehavior.AllowGet);
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult captureworkInstructionForOrderScan(string orderno, string partNumber, int StationId, bool IsSubAssmbly, bool IsMainAssmbly, int currentWIID) //for capture Order scan(OL) workinstruction and load data
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                int userId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId);
                string userHost = Convert.ToString(((FDSession)this.Session["FDSession"]).userId);
                orderno = orderno.Trim().ToUpper();
                partNumber = partNumber.Trim().ToUpper();
                decimal shiftId = getShiftId();
                if (IsSubAssmbly)
                {
                    var orderlist = db.RS_OM_SubAssembly_Order_List.Where(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper()==partNumber).FirstOrDefault();
                    if (orderlist == null)
                    {
                        return Json(new { status = false, message = "Carrier Number not available." }, JsonRequestBehavior.AllowGet);
                    }
                    if (db.RS_WorkInstruction_Master.Where(c => c.Station_ID == StationId && c.Model_Code == orderlist.Part_No).Count() == 0)
                    {
                        return Json(new { status = false, message = "Workinstruction list not available" }, JsonRequestBehavior.AllowGet);
                    }
                    var workInstr = db.RS_WorkInstruction_Master.Find(currentWIID);
                    if (workInstr == null)
                    {
                        return Json(new { status = false, message = "Workinstruction not available " }, JsonRequestBehavior.AllowGet);
                    }
                    RS_WorkInstruction_Submitted obj = new RS_WorkInstruction_Submitted();
                    obj.Model_Code = orderlist.Part_No;
                    obj.Station_ID = StationId;
                    obj.Sequence_No = workInstr.Sequence_No;
                    obj.WI_Type_Name = workInstr.WI_Type_Name;
                    obj.WI_Description = workInstr.WI_Description;
                    obj.Order_No = orderlist.Order_No;
                    obj.WI_Number = workInstr.WI_Number;
                    obj.Status = true;
                    obj.Shift_ID = shiftId;
                    obj.Inserted_Date = System.DateTime.Now;
                    obj.Inserted_Host = userHost;
                    obj.Inserted_User_ID = userId;
                    db.RS_WorkInstruction_Submitted.Add(obj);
                    db.SaveChanges();
                    CheckToAddinQueue(obj);
                    //int worklistcount = db.RS_WorkInstruction_Master.Where(c=>c.Model_Code== obj.Model_Code && c.Station_ID== StationId).Count();
                    //int scrollX = (worklistcount/(worklistcount- Convert.ToInt32( obj.Sequence_No)))*100;
                    return Json(new { status = true, message = "Sequence No " + obj.Sequence_No + " Workinstruction Completed..!" }, JsonRequestBehavior.AllowGet);

                }
                else if(IsMainAssmbly)
                {
                    var orderList = db.RS_OM_Order_List.Where(c => c.Order_No.ToUpper() == orderno && c.Model_Code.ToUpper()==partNumber ).FirstOrDefault();
                    if (orderList == null)
                    {
                        return Json(new { status = false, message = "Order Number not available-" + orderno }, JsonRequestBehavior.AllowGet);
                    }
                    if (db.RS_WorkInstruction_Master.Where(c => c.Station_ID == StationId && c.Model_Code == orderList.Model_Code).Count() == 0)
                    {
                        return Json(new { status = false, message = "Workinstruction list not available" }, JsonRequestBehavior.AllowGet);
                    }
                    var workInstr = db.RS_WorkInstruction_Master.Find(currentWIID);
                    if (workInstr == null)
                    {
                        return Json(new { status = false, message = "Order Scan Workinstruction not available on Top 1st" }, JsonRequestBehavior.AllowGet);
                    }
                    RS_WorkInstruction_Submitted obj = new RS_WorkInstruction_Submitted();
                    obj.Model_Code = orderList.Model_Code;
                    obj.Station_ID = StationId;
                    obj.Sequence_No = workInstr.Sequence_No;
                    obj.WI_Type_Name = workInstr.WI_Type_Name;
                    obj.WI_Description = workInstr.WI_Description;
                    obj.Shift_ID = shiftId;
                    obj.Order_No = orderList.Order_No;
                    obj.WI_Number = workInstr.WI_Number;
                    obj.Status = true;

                    obj.Inserted_Date = System.DateTime.Now;
                    obj.Inserted_Host = userHost;
                    obj.Inserted_User_ID = userId;
                    db.RS_WorkInstruction_Submitted.Add(obj);
                    db.SaveChanges();
                    CheckToAddinQueue(obj);
                    //int worklistcount = db.RS_WorkInstruction_Master.Where(c=>c.Model_Code== obj.Model_Code && c.Station_ID== StationId).Count();
                    //int scrollX = (worklistcount/(worklistcount- Convert.ToInt32( obj.Sequence_No)))*100;
                    return Json(new { status = true, message = "Sequence No " + obj.Sequence_No + " Workinstruction Completed..!" }, JsonRequestBehavior.AllowGet);

                }

                return Json(false, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

         
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult resubmitWorkinstruction(string orderno,bool IsSubAssmbly,bool IsMainAssmbly, int seqNo,string supervisorname, string partno=null)
        {
            try
            {
          
            List<RS_WorkInstruction_Submitted> sub = new List<RS_WorkInstruction_Submitted>();
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                int userid = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId);
                string userHost = (((FDSession)this.Session["FDSession"]).userHost);
                decimal shiftId = getShiftId();
                var user = db.RS_AM_Line_Supervisor_Mapping.Where(c => c.RS_Employee.Employee_No.Trim().ToUpper() == supervisorname.Trim().ToUpper() && c.Line_ID == lineId).FirstOrDefault();
            if (user == null)
            {
                return Json(new { status = false, message = "Invalid Supervisor." }, JsonRequestBehavior.AllowGet);
            }
            if (IsSubAssmbly)
            {
                sub = db.RS_WorkInstruction_Submitted.Where(c => c.Order_No.ToUpper() == orderno.Trim().ToUpper() && c.Model_Code.ToUpper()==partno.Trim().ToUpper() && c.Sequence_No>=seqNo).ToList();
            }
            else if(IsMainAssmbly)
            {
                sub = db.RS_WorkInstruction_Submitted.Where(c => c.Order_No.ToUpper() == orderno.Trim().ToUpper() && c.Sequence_No >= seqNo).ToList();
            }
            // maintain history
            foreach(var item in sub)
            {
                RS_WorkInstruction_Submitted_History hist = new RS_WorkInstruction_Submitted_History();
                hist.Model_Code = item.Model_Code;
                hist.Station_ID = item.Station_ID;
                hist.Sequence_No = item.Sequence_No;
                hist.WI_Type_Name = item.WI_Type_Name;
                hist.WI_Description = item.WI_Description;
                hist.Part_No = item.Part_No;
                hist.Carrier_Number = item.Carrier_Number;
                hist.Order_No = item.Order_No;
                hist.Status = item.Status;
                    hist.Shift_ID = item.Shift_ID;

                    hist.WI_Number = item.WI_Number;
                hist.Inserted_User_ID = item.Inserted_User_ID;
                hist.Inserted_Date = item.Inserted_Date;
                hist.Inserted_Host = item.Inserted_Host;
                hist.Updated_User_ID = item.Updated_User_ID;
                hist.Updated_Date = item.Updated_Date;
                hist.Updated_Host = item.Updated_Host;
                hist.Is_Active = item.Is_Active;
                hist.Supervisor_ID = user.Employee_ID;
                 
                db.RS_WorkInstruction_Submitted_History.Add(hist);
                db.SaveChanges();

                    if (hist.WI_Type_Name== "SUBEP")
                    {
                        RS_OM_SubAssembly_Order_List_History subHist = new RS_OM_SubAssembly_Order_List_History();
                        RS_OM_SubAssembly_Order_List subassmbly = db.RS_OM_SubAssembly_Order_List.Where(c => c.Main_Order_No == orderno.Trim().ToUpper() && c.Part_No== hist.Part_No).FirstOrDefault();
                        if(subassmbly!=null)
                        {
                            subHist.Order_No = subassmbly.Order_No;
                            subHist.Serial_No = subassmbly.Serial_No;
                            subHist.Part_No = subassmbly.Part_No;
                            subHist.Order_Status = subassmbly.Order_Status;
                            subHist.Main_Order_No = subassmbly.Main_Order_No;
                            subHist.Scan_Value = subassmbly.Scan_Value;
                            subHist.Plant_ID = subassmbly.Plant_ID;
                            subHist.Shop_ID = subassmbly.Shop_ID;
                            subHist.Line_ID = subassmbly.Line_ID;
                            subHist.Station_ID = subassmbly.Station_ID;
                            subHist.Inserted_Host = subassmbly.Inserted_Host;
                            subHist.Inserted_User_ID = subassmbly.Inserted_User_ID;
                            subHist.Inserted_Date = subassmbly.Updated_Date;
                            subHist.Updated_Host = subassmbly.Updated_Host;
                            subHist.Updated_User_ID = subassmbly.Updated_User_ID;
                            subHist.Updated_Date = subassmbly.Updated_Date;
                            subHist.Supervisor_ID = user.Employee_ID;
                            subHist.Shift_ID = subassmbly.Shift_ID;

                            db.RS_OM_SubAssembly_Order_List_History.Add(subHist);
                            db.SaveChanges();

                            subassmbly.Order_Status = "Started";
                            subassmbly.Main_Order_No = null;
                            subassmbly.Updated_Date = System.DateTime.Now;
                            subassmbly.Updated_Host = userHost;
                            subassmbly.Updated_User_ID =userid;
                            db.Entry(subassmbly).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            db.RS_WorkInstruction_Submitted.RemoveRange(sub);
            db.SaveChanges();

            return Json(new { status = true, message = "Successfully Reset Workinstruction to Sequence number-"+ seqNo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = false, message = "Exception during saving data" }, JsonRequestBehavior.AllowGet);
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult CheckValidSupervisor(string supervisorname)
        {
            try
            {
                int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                var user = db.RS_AM_Line_Supervisor_Mapping.Any(c=>c.RS_Employee.Employee_No.Trim().ToUpper()==supervisorname.Trim().ToUpper() && c.Line_ID== lineId);
                 if(user)
                {
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Invalid Supervisor" }, JsonRequestBehavior.AllowGet);
                }
   
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Invalid Supervisor." }, JsonRequestBehavior.AllowGet);
            }
        }
            [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult captureworkInstruction(string orderno, int StationId,int currentWIID,string partNumber, bool IsSubAssmbly, bool IsMainAssmbly,string Partno =null,string okNok=null)
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                int userId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId);
                string userHost = Convert.ToString(((FDSession)this.Session["FDSession"]).userId);
                orderno = orderno.Trim().ToUpper();
                partNumber = partNumber.Trim().ToUpper();
                bool isOrderStartStn = db.RS_Stations.Any(c=>c.Station_ID==StationId && c.Is_Order_Start==true);
                decimal shiftId = getShiftId();
                if(isOrderStartStn)
                {
                    //order start
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (IsSubAssmbly)
                    {
                        var carrierlist = db.RS_OM_SubAssembly_Order_List.Where(c => c.Order_No.ToUpper() == orderno && c.Part_No== partNumber).FirstOrDefault();
                        if (carrierlist == null)
                        {
                            return Json(new { status = false, message = "Order Number not available-" +orderno }, JsonRequestBehavior.AllowGet);
                        }
                        if (db.RS_WorkInstruction_Master.Where(c => c.Station_ID == StationId && c.Model_Code == carrierlist.Part_No).Count() == 0)
                        {
                            return Json(new { status = false, message = "Workinstruction list not available" }, JsonRequestBehavior.AllowGet);
                        }
                        var workInstr = db.RS_WorkInstruction_Master.Find(currentWIID);
                        if (workInstr == null)
                        {
                            return Json(new { status = false, message = "Workinstruction not available" }, JsonRequestBehavior.AllowGet);
                        }
                        RS_WorkInstruction_Submitted obj = new RS_WorkInstruction_Submitted();
                        obj.Model_Code = carrierlist.Part_No;
                        obj.Station_ID = StationId;
                        obj.Sequence_No = workInstr.Sequence_No;
                        obj.WI_Type_Name = workInstr.WI_Type_Name;
                        obj.WI_Description = workInstr.WI_Description;
                        obj.Shift_ID = shiftId;
                        obj.Order_No = carrierlist.Order_No;
                        obj.WI_Number = workInstr.WI_Number;
                        if (workInstr.WI_Type_Name == "VI")
                        {
                            obj.Status = okNok == "OK" ? true : false;
                        }
                        else if (workInstr.WI_Type_Name == "Scan")
                        {
                            obj.Status = true;
                        }
                        else if (workInstr.WI_Type_Name == "EP")
                        {
                            if (Partno == null)
                            {
                                return Json(new { status = false, message = "Part No is Empty." }, JsonRequestBehavior.AllowGet);
                            }
                            if (workInstr.Part_No == null)
                            {
                                return Json(new { status = false, message = "Assign Part No in Workinstruction Master." }, JsonRequestBehavior.AllowGet);
                            }
                            //else
                            bool isbomvalid = db.RS_BOM_Item.Any(c => c.Model_Code == carrierlist.Part_No && c.Part_No.ToUpper() == Partno.ToUpper().Trim());
                            if (isbomvalid && Partno.ToUpper() == workInstr.Part_No.ToUpper())
                            {
                                obj.Part_No = Partno;
                                obj.Status = true;
                            }
                            else
                            {
                                return Json(new { status = false, message = "Wrong Error Proofing..!" }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else if (workInstr.WI_Type_Name == "LP")
                        {
                            //print method call
                            obj.Status = SendToPrint();
                        }
                        else if (workInstr.WI_Type_Name == "CT")
                        {
                            obj.Status = true;
                        }
                        obj.Inserted_Date = System.DateTime.Now;
                        obj.Inserted_Host = userHost;
                        obj.Inserted_User_ID = userId;
                        db.RS_WorkInstruction_Submitted.Add(obj);
                        db.SaveChanges();
                        if(obj.Status==false)
                        {
                            carrierlist.Q_Status = false;
                            carrierlist.Updated_Date = System.DateTime.Now;
                            carrierlist.Updated_Host = userHost;
                            carrierlist.Updated_User_ID = userId;
                            db.Entry(carrierlist).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        CheckToAddinQueue(obj);
                        //int worklistcount = db.RS_WorkInstruction_Master.Where(c=>c.Model_Code== obj.Model_Code && c.Station_ID== StationId).Count();
                        //int scrollX = (worklistcount/(worklistcount- Convert.ToInt32( obj.Sequence_No)))*100;
                        return Json(new { status = true, message = "Sequence No " + obj.Sequence_No + " Workinstruction Completed..!" }, JsonRequestBehavior.AllowGet);
                    }
                    else if(IsMainAssmbly)
                    {
                        var carrierlist = db.RS_Carrier_Orderlist.Where(c => c.Order_No == orderno).FirstOrDefault();
                        var orderlist = db.RS_OM_Order_List.Where(c => c.Order_No == orderno).FirstOrDefault();

                        if (carrierlist == null)
                        {
                            return Json(new { status = false, message = "Carrier Number not available." }, JsonRequestBehavior.AllowGet);
                        }
                        if (db.RS_WorkInstruction_Master.Where(c => c.Station_ID == StationId && c.Model_Code == carrierlist.Model_Code).Count() == 0)
                        {
                            return Json(new { status = false, message = "Workinstruction list not available" }, JsonRequestBehavior.AllowGet);
                        }
                        var workInstr = db.RS_WorkInstruction_Master.Find(currentWIID);
                        if (workInstr == null)
                        {
                            return Json(new { status = false, message = "Workinstruction not available" }, JsonRequestBehavior.AllowGet);
                        }
                        RS_WorkInstruction_Submitted obj = new RS_WorkInstruction_Submitted();
                        obj.Model_Code = carrierlist.Model_Code;
                        obj.Station_ID = StationId;
                        obj.Sequence_No = workInstr.Sequence_No;
                        obj.WI_Type_Name = workInstr.WI_Type_Name;
                        obj.WI_Description = workInstr.WI_Description;
                        obj.Carrier_Number = carrierlist.Carrier_Number;
                        obj.Shift_ID = shiftId;
                        obj.Order_No = carrierlist.Order_No;
                        obj.WI_Number = workInstr.WI_Number;
                        if (workInstr.WI_Type_Name == "VI")
                        {
                            obj.Status = okNok == "OK" ? true : false;
                        }
                        else if (workInstr.WI_Type_Name == "Scan")
                        {
                            obj.Status = true;
                        }
                        else if (workInstr.WI_Type_Name == "EP")
                        {
                            if (Partno == null)
                            {
                                return Json(new { status = false, message = "Part No is Empty." }, JsonRequestBehavior.AllowGet);
                            }
                            if (workInstr.Part_No == null)
                            {
                                return Json(new { status = false, message = "Assign Part No to Workinstruction Master." }, JsonRequestBehavior.AllowGet);
                            }
                            //else
                            bool isbomvalid = db.RS_BOM_Item.Any(c => c.Model_Code == carrierlist.Model_Code && c.Part_No.ToUpper() == Partno.ToUpper().Trim());
                            if (isbomvalid && Partno.ToUpper() == workInstr.Part_No.ToUpper())
                            {
                                obj.Part_No = Partno;
                                obj.Status = true;
                            }
                            else
                            {
                                return Json(new { status = false, message = "Wrong Error Proofing..!" }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else if (workInstr.WI_Type_Name == "SUBEP")
                        {
                            var order_part = Partno.Split(':');
                            if(order_part.Length!=2)
                            {
                                return Json(new { status = false, message = "Part number or Order number is Empty." }, JsonRequestBehavior.AllowGet);
                            }
                            Partno = order_part[0];
                            string subOrderno = order_part[1];

                            if (Partno == null || subOrderno==null)
                            {
                                return Json(new { status = false, message = "Part number or Order number is Empty." }, JsonRequestBehavior.AllowGet);
                            }
                            if (workInstr.Part_No == null)
                            {
                                return Json(new { status = false, message = "Assign Part number to Workinstruction Master." }, JsonRequestBehavior.AllowGet);
                            }
                            //order started and all workinstruction completed ok checking



                            bool isExist = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == subOrderno.ToUpper() && c.Part_No.ToUpper() == Partno.ToUpper() && c.Order_Status!="Closed");
                            if (isExist)
                            {
                                bool isOkOrder = db.RS_Order_Queue.Any(c => c.Order_No.ToUpper() == subOrderno.ToUpper() && c.Model_Code.ToUpper() == Partno.ToUpper() && c.Status == true && c.Is_All_Submitted==true);
                                if (!isOkOrder)
                                {
                                    return Json(new { status = false, message = "Clear the All workinstructions against order- " + subOrderno }, JsonRequestBehavior.AllowGet);

                                }
                                //else
                                //{
                                //    //return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);

                                //}
                                //RS_OM_SubAssembly_Order_List obj = db.RS_OM_SubAssembly_Order_List.Where(c => c.Order_No.ToUpper() == orderno.ToUpper() && c.Part_No.ToUpper() == partNo.ToUpper()).FirstOrDefault();
                                //return Json(new { status = false, SubAssemly = obj, message = "Already" }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                bool isExistwithClosed = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == subOrderno.ToUpper() && c.Part_No.ToUpper() == Partno.ToUpper() && c.Order_Status == "Closed");
                                if(isExistwithClosed)
                                {
                                    return Json(new { status = false, message = "Sub Assembly Order Already attached to another Main Assembly..." }, JsonRequestBehavior.AllowGet);

                                }

                                return Json(new { status = false, message = "Sub Assembly Order not Started yet..." }, JsonRequestBehavior.AllowGet);
                            }
                            //else
                            bool isbomvalid = db.RS_BOM_Item.Any(c => c.Model_Code == carrierlist.Model_Code && c.Part_No.ToUpper() == Partno.ToUpper().Trim());
                            if (isbomvalid && Partno.ToUpper() == workInstr.Part_No.ToUpper())
                            {
                                obj.Part_No = Partno;
                                obj.Status = true;
                               var subOrderobj= db.RS_OM_SubAssembly_Order_List.Where(c => c.Order_No.ToUpper() == subOrderno.ToUpper() && c.Part_No.ToUpper() == Partno.ToUpper()).FirstOrDefault();
                                subOrderobj.Order_Status = "Closed";
                                subOrderobj.Updated_Date = System.DateTime.Now;
                                subOrderobj.Updated_Host = userHost;
                                subOrderobj.Updated_User_ID =userId;
                                subOrderobj.Main_Order_No = obj.Order_No;
                                db.Entry(subOrderobj).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                return Json(new { status = false, message = "Wrong Error Proofing(BOM)..!" }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        else if (workInstr.WI_Type_Name == "LP")
                        {
                            //print method call
                            obj.Status = SendToPrint();
                        }
                        else if (workInstr.WI_Type_Name == "CT")
                        {
                            //print method call
                            obj.Status = true;
                        }
                        
                        obj.Inserted_Date = System.DateTime.Now;
                        obj.Inserted_Host = userHost;
                        obj.Inserted_User_ID = userId;
                        db.RS_WorkInstruction_Submitted.Add(obj);
                        db.SaveChanges();

                        if (obj.Status == false)
                        {
                            orderlist.Q_Status = false;
                            orderlist.Updated_Date = System.DateTime.Now;
                            orderlist.Updated_Host = userHost;
                            orderlist.Updated_User_ID = userId;
                            db.Entry(orderlist).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        CheckToAddinQueue(obj);
                        //int worklistcount = db.RS_WorkInstruction_Master.Where(c=>c.Model_Code== obj.Model_Code && c.Station_ID== StationId).Count();
                        //int scrollX = (worklistcount/(worklistcount- Convert.ToInt32( obj.Sequence_No)))*100;
                        return Json(new { status = true, message = "Sequence No " + obj.Sequence_No + " Workinstruction Completed..!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch(Exception )
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public bool CheckToAddinQueue(RS_WorkInstruction_Submitted obj)
        {
            int totalworkinstructionCount = db.RS_WorkInstruction_Master.Where(c => c.Model_Code == obj.Model_Code && c.Station_ID == obj.Station_ID).Count();
            int totalSubmittedworkinstructionCount = db.RS_WorkInstruction_Submitted.Where(c => c.Model_Code == obj.Model_Code && c.Order_No==obj.Order_No && c.Station_ID == obj.Station_ID).Count();
            int plant_id = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
            var station = db.RS_Stations.Find(obj.Station_ID);
            decimal shiftId = getShiftId();
            //add in queue
            if (totalworkinstructionCount== totalSubmittedworkinstructionCount)
            {
             
                int userId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId);
                string userHost = Convert.ToString(((FDSession)this.Session["FDSession"]).userId);
                int IsNOtokCount = db.RS_WorkInstruction_Submitted.Where(c => c.Model_Code == obj.Model_Code && c.Station_ID == obj.Station_ID && c.Status==false  && c.Order_No==obj.Order_No).Count();
                // insert into queue list
                bool iscarrierNoAlreadyAB = db.RS_Order_Queue.Any(c => c.Order_No == obj.Order_No && c.Model_Code.ToUpper()==obj.Model_Code.ToUpper()  && c.Station_ID == obj.Station_ID);
                if (!iscarrierNoAlreadyAB)
                {
                    RS_Order_Queue obj1 = new RS_Order_Queue();
                    obj1.Carrier_Number = obj.Carrier_Number;
                    obj1.Order_No = obj.Order_No;
                    obj1.Model_Code = obj.Model_Code;
                    obj1.Station_ID = obj.Station_ID;
                    obj1.Line_ID = station.Line_ID;
                    obj1.Shop_ID = station.Shop_ID;
                    obj1.Plant_ID = plant_id;
                    obj1.Inserted_User_ID = userId;
                    obj1.Inserted_Date = System.DateTime.Now;
                    obj1.Inserted_Host = userHost;
                    obj1.Shift_ID = shiftId;
                    obj1.Updated_User_ID = userId;
                    obj1.Updated_Date = System.DateTime.Now;
                    obj1.Updated_Host = userHost;

                    obj1.Status = IsNOtokCount > 0 ? false : true;
                    db.RS_Order_Queue.Add(obj1);
                    db.SaveChanges();
                }
                else
                {
                    RS_Order_Queue objUpdate = db.RS_Order_Queue.Where(c =>  c.Order_No == obj.Order_No && c.Station_ID == obj.Station_ID && c.Model_Code.ToUpper() == obj.Model_Code.ToUpper()).FirstOrDefault();
                    objUpdate.Updated_Date = System.DateTime.Now;
                    objUpdate.Updated_Host = userHost;
                    objUpdate.Updated_User_ID = userId;
                    objUpdate.Status = IsNOtokCount > 0 ? false : true;
                    db.Entry(objUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if(station.Is_Punching_Station)
                {

                    GenerateXML(obj.Order_No, obj.Model_Code, "", "pass");
                }
            }
            return true;
        }
        public bool DivorceCarrier(string OrderNo, string partNo)
        {
            try
            {
                RS_Carrier_Orderlist obj = db.RS_Carrier_Orderlist.Where(c => c.Order_No == OrderNo && c.Model_Code == partNo).FirstOrDefault();
                if(obj!=null)
                {
                    RS_Carrier_Orderlist_History hist = new RS_Carrier_Orderlist_History();
                    //hist.Carrier_ID = obj. ;
                    hist.Carrier_Number = obj.Carrier_Number ;
                    hist.Order_No = obj.Order_No ;
                    hist.Serial_No = obj.Serial_No ;
                    hist.Model_Code = obj.Model_Code ;
                    hist.Station_ID = obj.Station_ID ;
                    hist.Line_ID = obj.Line_ID ;
                    hist.Shop_ID = obj.Shop_ID ;
                    hist.Plant_ID = obj.Plant_ID ;
                    hist.Inserted_User_ID = obj.Inserted_User_ID ;
                    hist.Inserted_Date = obj.Inserted_Date ;
                    hist.Inserted_Host = obj.Inserted_Host ;
                    hist.Updated_User_ID = obj.Updated_User_ID ;
                    hist.Updated_Date = obj.Updated_Date ;
                    hist.Updated_Host = obj.Updated_Host ;
                    hist.Shift_ID = obj.Shift_ID ;
                    db.RS_Carrier_Orderlist_History.Add(hist);
                    db.SaveChanges();

                    db.RS_Carrier_Orderlist.Remove(obj);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool GenerateXML(string OrderNo, string Modelcode, string serialNo, string typename)
        {
            try
            {
                int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);

                RS_Stations station = db.RS_Stations.Find(stationId);
                RS_Model_Master model = db.RS_Model_Master.Where(c => c.Model_Code == Modelcode).FirstOrDefault();
                string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                path = path + "\\App_Data\\XMLFiles\\" + OrderNo + "_" + typename+ ".xml";

                RS_Quality_Captures shift = new RS_Quality_Captures();
                var shiftdata = shift.getCurrentRunningShiftByShopID(shopId);
                string shiftname = "";
                if (shiftdata != null)
                {
                    shiftname = shiftdata.Shift_Name;
                }
                using (XmlWriter writer = XmlWriter.Create(path))
                {
                    if (typename == "build")
                    {
                        // start output  buildinfo
                        writer.WriteStartElement("output");
                        writer.WriteAttributeString("type", typename);


                        writer.WriteStartElement("buildinfo");  // start job buildinfo
                        writer.WriteAttributeString("workorder", OrderNo);
                        writer.WriteAttributeString("status", "Started");
                        writer.WriteAttributeString("timestamp", System.DateTime.Now.ToString());
                        writer.WriteAttributeString("stationname", station.Station_Name.ToString());


                        writer.WriteStartElement("jobinfo"); // start job info
                        writer.WriteAttributeString("model", "---");
                        writer.WriteEndElement(); // end job info

                        writer.WriteStartElement("partinfo"); // start part info
                        writer.WriteAttributeString("partnumber", model.Model_Code);
                        writer.WriteAttributeString("partdesc", model.Model_Description);
                        writer.WriteEndElement(); // end part info

                        writer.WriteEndElement();// end job buildinfo


                        writer.WriteEndElement();// end output  buildinfo
                        writer.Flush();
                    }
                    else if (typename == "pass")
                    {
                        RS_OM_Order_List list = new RS_OM_Order_List();
                        list = db.RS_OM_Order_List.Where(c => c.Order_No == OrderNo).FirstOrDefault();
                        writer.WriteStartElement("output");     // start output 
                        writer.WriteAttributeString("type", typename);


                        writer.WriteStartElement("buildinfo");  // start job buildinfo  Order Information
                        writer.WriteAttributeString("workorder", OrderNo);
                        //writer.WriteAttributeString("status", "Started");
                        writer.WriteAttributeString("Starttimestamp", list.Inserted_Date.ToString());
                        writer.WriteAttributeString("Endtimestamp", System.DateTime.Now.ToString());
                        writer.WriteAttributeString("stationname", station.Station_Name.ToString());
                        writer.WriteAttributeString("shift", shiftname);

                        writer.WriteStartElement("jobinfo"); // start job info attribute information
                        //writer.WriteAttributeString("partid", "");
                        //writer.WriteAttributeString("vendorid", "");
                        //writer.WriteAttributeString("timestamp", System.DateTime.Now.ToString());                      
                        writer.WriteEndElement(); // end job info

                        writer.WriteStartElement("partinfo"); // start part info
                        writer.WriteAttributeString("partnumber", model.Model_Code);
                        writer.WriteAttributeString("partdesc", model.Model_Description);
                        writer.WriteEndElement(); // end part info

                        writer.WriteEndElement();// end job buildinfo


                        writer.WriteEndElement();// end output  
                        writer.Flush();
                    }

                }
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }
        public ActionResult GetData(string OrderNo, string Modelcode, string serialNo)
        {
            TempData["ModelCode"] = "";
            TempData["OrderNo"] = "";
            TempData["SerialNo"] = "";

            TempData["ModelCode"] = Modelcode;
            TempData["OrderNo"] = OrderNo;
            TempData["SerialNo"] = serialNo;
            return RedirectToAction("WorkInstructionSubmission", "WorkInstruction");
        }
   
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult marriageCarrier(string carrierNo,string OrderNo )
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                carrierNo = carrierNo.Trim().ToUpper();
                OrderNo = OrderNo.Trim().ToUpper();
                int userId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId);
                int StationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                int Plant_Id = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
                decimal shiftId = getShiftId();

                string userHost = Convert.ToString(((FDSession)this.Session["FDSession"]).userId);
                var station = db.RS_Stations.Find(StationId);
                var isOrderstarted = db.RS_OM_Order_List.Where(c=>c.Order_No.ToUpper()==OrderNo).FirstOrDefault();
                bool iscarrierNoAlready = db.RS_Carrier_Orderlist.Any(c => c.Carrier_Number.ToUpper() == carrierNo);
                if (iscarrierNoAlready)
                {
                    bool iscarrierNoAlreadyIscurrentStn = db.RS_Carrier_Orderlist.Any(c => c.Carrier_Number.ToUpper() == carrierNo && c.Station_ID==StationId);
                    if(iscarrierNoAlreadyIscurrentStn)
                    {
                        return Json(new { status = true, message = "Order No and Carrier Marriage Successfully.." }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { status = false, message = "Carrier number already on line" }, JsonRequestBehavior.AllowGet);
                }

                if (isOrderstarted == null)
                {
                    return Json(new { status = false, message = "OrderNo not available" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // insert into carrier orderlist
                    bool iscarrierNoAlreadyA = db.RS_Carrier_Orderlist.Any(c => c.Carrier_Number.ToUpper() == carrierNo.Trim() && c.Order_No.ToUpper() == OrderNo);
                    if(!iscarrierNoAlreadyA)
                    {
                        RS_Carrier_Orderlist obj = new RS_Carrier_Orderlist();
                        obj.Carrier_Number = carrierNo;
                        obj.Order_No = OrderNo;
                        obj.Serial_No = isOrderstarted.Serial_No;
                        obj.Model_Code = isOrderstarted.Model_Code;
                        obj.Station_ID = station.Station_ID;
                        obj.Line_ID = station.Line_ID;
                        obj.Shop_ID = station.Shop_ID;
                        obj.Plant_ID = Plant_Id;
                        obj.Inserted_User_ID = userId;
                        obj.Inserted_Date = System.DateTime.Now;
                        obj.Inserted_Host = userHost;
                        obj.Shift_ID = shiftId;
                        db.RS_Carrier_Orderlist.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        RS_Carrier_Orderlist objUpdate = db.RS_Carrier_Orderlist.Where(c => c.Carrier_Number.ToUpper() == carrierNo && c.Order_No.ToUpper() == OrderNo).FirstOrDefault();
                        objUpdate.Updated_Date = System.DateTime.Now;
                        objUpdate.Updated_Host = userHost;
                        objUpdate.Updated_User_ID = userId;
                        db.Entry(objUpdate).State = EntityState.Modified;
                        db.SaveChanges();
                    }



                    //// insert into queue list
                    //bool iscarrierNoAlreadyAB = db.RS_Order_Queue.Any(c => c.Carrier_Number == carrierNo && c.Order_No == OrderNo && c.Station_ID == StationId);
                    //if (!iscarrierNoAlreadyAB)
                    //{
                    //    RS_Order_Queue obj1 = new RS_Order_Queue();
                    //    obj1.Carrier_Number = carrierNo;
                    //    obj1.Order_No = OrderNo;
                    //    obj1.Model_Code = isOrderstarted.Model_Code;
                    //    obj1.Station_ID = station.Station_ID;
                    //    obj1.Line_ID = station.Line_ID;
                    //    obj1.Shop_ID = station.Shop_ID;
                    //    obj1.Plant_ID = station.Plant_ID;
                    //    obj1.Inserted_User_ID = userId;
                    //    obj1.Inserted_Date = System.DateTime.Now;
                    //    obj1.Inserted_Host = userHost;
                    //    db.RS_Order_Queue.Add(obj1);
                    //    db.SaveChanges();
                    //}
                    //else
                    //{
                    //    RS_Order_Queue objUpdate = db.RS_Order_Queue.Where(c => c.Carrier_Number == carrierNo && c.Order_No == OrderNo && c.Station_ID == StationId).FirstOrDefault();
                    //    objUpdate.Updated_Date = System.DateTime.Now;
                    //    objUpdate.Updated_Host = userHost;
                    //    objUpdate.Updated_User_ID = userId;
                    //    db.Entry(objUpdate).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult WorkInstructionShow(string Order_No, int StationId,string  partNumber,bool IsSubAssmbly,bool IsMainAssmbly)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                bool isSubassm = db.RS_Stations.Any(c => c.Station_ID == StationId && c.Is_Sub_Assembly_OS == true);
                Order_No = Order_No.Trim().ToUpper();
                partNumber = partNumber.Trim().ToUpper();
                if (IsSubAssmbly)
                {
                    List<RS_WorkInstruction_Master> list = new List<RS_WorkInstruction_Master>();
                    RS_OM_SubAssembly_Order_List subass = db.RS_OM_SubAssembly_Order_List.Where(c => c.Order_No.ToUpper() == Order_No && c.Part_No.ToUpper() == partNumber).FirstOrDefault();
                    //if (subass == null)
                    //{
                    //    return Json(new { status = false, message = "Sub Assembly Order not created." }, JsonRequestBehavior.AllowGet);
                    //}
                    var rS_WorkInstruction_Master = db.RS_WorkInstruction_Master.Where(c => c.Model_Code.ToUpper() == partNumber && c.Station_ID == StationId).ToList().OrderBy(c => c.Sequence_No);
                    if (rS_WorkInstruction_Master.Count() == 0)
                    {
                        return Json(new { status = false, message = "Workinstruction list not available" }, JsonRequestBehavior.AllowGet);
                    }
                    //var carrierlist = db.RS_Carrier_Orderlist.Where(c => c.Order_No == Order_No).FirstOrDefault();
                    string modeldesc = db.RS_BOM_Item.Where(c => c.Part_No.ToUpper() == partNumber).Select(c => c.Part_Description).FirstOrDefault();
                    foreach (var item in rS_WorkInstruction_Master)
                    {
                        RS_WorkInstruction_Master obj = new RS_WorkInstruction_Master();
                        var rS_WorkInstruction_submitted = db.RS_WorkInstruction_Submitted.Where(c => c.Model_Code.ToUpper() == partNumber && c.Station_ID == StationId && c.Order_No == Order_No.ToUpper() && c.WI_Number == item.WI_Number).FirstOrDefault();
                        if (rS_WorkInstruction_submitted == null)
                        {
                            obj.Is_Submitted = false;
                            obj.Status = "Pending";
                        }
                        else
                        {
                            obj.Is_Submitted = true;
                            obj.Status = rS_WorkInstruction_submitted.Status == true ? "OK" : "NOK";
                        }

                        if (item.WI_Type_Name == "EP" && rS_WorkInstruction_submitted != null)
                        {
                            obj.Part_No = rS_WorkInstruction_submitted.Part_No;
                        }
                        else
                        {
                            obj.Part_No = "";
                        }
                        obj.WI_Number = item.WI_Number;
                        obj.Sequence_No = item.Sequence_No;
                        obj.WI_Type_Name = item.WI_Type_Name;
                        obj.WI_Description = item.WI_Description;

                        list.Add(obj);
                    }
                    return Json(new { WorkInstruction = list.OrderBy(c => c.Sequence_No), CarrierOrderlist = subass, Model_Desc = modeldesc }, JsonRequestBehavior.AllowGet);

                }
                else if(IsMainAssmbly)
                {
                    string modelcode = db.RS_OM_Order_List.Where(c => c.Order_No == Order_No).Select(c => c.Model_Code).FirstOrDefault();
                    if (modelcode == null)
                    {
                        return Json(new { status = false, message = "Order No not Marriage with Carrier" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        List<RS_WorkInstruction_Master> list = new List<RS_WorkInstruction_Master>();
                        var rS_WorkInstruction_Master = db.RS_WorkInstruction_Master.Where(c => c.Model_Code == modelcode && c.Station_ID == StationId).ToList().OrderBy(c => c.Sequence_No);
                        if (rS_WorkInstruction_Master.Count() == 0)
                        {
                            return Json(new { status = false, message = "Workinstruction list not available" }, JsonRequestBehavior.AllowGet);
                        }
                        var carrierlist = db.RS_Carrier_Orderlist.Where(c => c.Order_No == Order_No).FirstOrDefault();
                      
                        string modeldesc = db.RS_Model_Master.Where(c => c.Model_Code == modelcode).Select(c => c.Model_Description).FirstOrDefault();
                        foreach (var item in rS_WorkInstruction_Master)
                        {
                            RS_WorkInstruction_Master obj = new RS_WorkInstruction_Master();
                            var rS_WorkInstruction_submitted = db.RS_WorkInstruction_Submitted.Where(c => c.Model_Code == modelcode && c.Station_ID == StationId && c.Order_No == Order_No && c.WI_Number == item.WI_Number).FirstOrDefault();
                            if (rS_WorkInstruction_submitted == null)
                            {
                                obj.Is_Submitted = false;
                                obj.Status = "Pending";
                            }
                            else
                            {
                                obj.Is_Submitted = true;
                                obj.Status = rS_WorkInstruction_submitted.Status == true ? "OK" : "NOK";
                            }

                            if (item.WI_Type_Name == "EP" && rS_WorkInstruction_submitted != null)
                            {
                                obj.Part_No = rS_WorkInstruction_submitted.Part_No;
                            }
                            else
                            {
                                obj.Part_No = "";
                            }
                            obj.WI_Number = item.WI_Number;
                            obj.Sequence_No = item.Sequence_No;
                            obj.WI_Type_Name = item.WI_Type_Name;
                            obj.WI_Description = item.WI_Description;

                            list.Add(obj);
                        }

                        return Json(new { WorkInstruction = list.OrderBy(c => c.Sequence_No), CarrierOrderlist = carrierlist, Model_Desc = modeldesc }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, message = "Invalid Assembly" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(new { status = false, message = "Workinstruction list not available" }, JsonRequestBehavior.AllowGet);
            }

        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult orderChecking(string orderno, string partNo, string vendorBarcodevalue)
        {
            try
            {
                bool isValidOrder = db.RS_OM_SubAssembly_Order_List.Any(c=>c.Order_No.ToUpper()== orderno.Trim().ToUpper() && c.Part_No.ToUpper()==partNo.Trim().ToUpper());
                if(isValidOrder)
                {
                    RS_OM_SubAssembly_Order_List obj = db.RS_OM_SubAssembly_Order_List.Where(c => c.Order_No.ToUpper() == orderno.Trim().ToUpper() && c.Part_No.ToUpper() == partNo.Trim().ToUpper()).FirstOrDefault();
                    bool isExist = db.RS_WorkInstruction_Submitted.Any(c => c.Order_No.ToUpper() == orderno.ToUpper() && c.Part_No.ToUpper() == partNo.ToUpper());
                    if (isExist)
                    {
                             return Json(new { status = false, SubAssemly = obj, message = "Already" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { status = true, SubAssemly = obj, message = "Order Scanned Successfully.." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                   
                    return Json(new { status = false, message = "Sub Assembly order not available..!" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Exception!" }, JsonRequestBehavior.AllowGet);
            }
        }

            [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult AddingSubOrderList(string orderno, string partNo, string vendorBarcodevalue,string Status,int currentWI)
        {
            try
            {
                int userId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId);
                int StationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                int Plant_Id = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
                string userHost = Convert.ToString(((FDSession)this.Session["FDSession"]).userId);
                RS_Stations station = db.RS_Stations.Find(StationId);
                partNo = partNo.Trim().ToUpper();
                orderno = orderno.Trim().ToUpper();
                decimal shiftId = getShiftId();
                RS_WorkInstruction_Master workinstruction = new RS_WorkInstruction_Master();
                workinstruction = db.RS_WorkInstruction_Master.Find(currentWI);
                if (workinstruction == null)
                {
                    return Json(new { status = false, message = "Workinstruction not found" }, JsonRequestBehavior.AllowGet);
                }

                if (!db.RS_SubAssembly_Station_PartMaster.Any(c => c.Station_ID ==StationId && c.Part_No.ToUpper() == partNo))
                {
                    return Json(new { status = false, message = "Part number -" + partNo + " not available in Station Part master..."}, JsonRequestBehavior.AllowGet);
                }

                bool isExist = false;
                if (Status == "Created")
                {
                    isExist = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper() == partNo && c.Order_Status == "Created");
                    if(isExist)
                    {
                        RS_OM_SubAssembly_Order_List obj = db.RS_OM_SubAssembly_Order_List.Where(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper() == partNo).FirstOrDefault();
                        RS_WorkInstruction_Submitted submittedExist1 = db.RS_WorkInstruction_Submitted.Where(c => c.Order_No.ToUpper() == orderno && c.Model_Code.ToUpper() == partNo
                             && c.WI_Number == currentWI).FirstOrDefault();
                        if (submittedExist1 == null)
                        {
                            //adding in submitted workinstruction
                            RS_WorkInstruction_Submitted submitted = new RS_WorkInstruction_Submitted();
                            submitted.Model_Code = partNo;
                            submitted.Sequence_No = workinstruction.Sequence_No;
                            submitted.WI_Type_Name = workinstruction.WI_Type_Name;
                            submitted.WI_Description = workinstruction.WI_Description;
                            submitted.Order_No = orderno;
                            submitted.WI_Number = workinstruction.WI_Number;
                            submitted.Status = true;
                            submitted.Shift_ID = shiftId;
                            submitted.Station_ID = station.Station_ID;
                            submitted.Inserted_Host = userHost;
                            submitted.Inserted_User_ID = userId;
                            submitted.Inserted_Date = System.DateTime.Now;
                            db.RS_WorkInstruction_Submitted.Add(submitted);
                            db.SaveChanges();
                        }
                      
                        return Json(new { status = false, SubAssemly = obj, message = "Order Already" + Status }, JsonRequestBehavior.AllowGet);
                    }
                    isExist = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper() == partNo && c.Order_Status == "Started");
                    if (isExist)
                    {
                        return Json(new { status = false, message = "Order already Started.."+ orderno }, JsonRequestBehavior.AllowGet);
                    }
                    isExist = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper() == partNo && c.Order_Status == "Closed");
                    if (isExist)
                    {
                        return Json(new { status = false, message = "Order already Closed.." + orderno }, JsonRequestBehavior.AllowGet);
                    }
                    //checking in BOM
                    var effectveDateTo = db.RS_BOM_Item.Where(c => c.Part_No == partNo).FirstOrDefault();
                    if (effectveDateTo == null)
                    {
                        return Json(new { status = false, message = "Please Check BOM detail against Part number- " + partNo }, JsonRequestBehavior.AllowGet);
                    }
                    bool validinBom = DateTime.Compare(System.DateTime.Now, Convert.ToDateTime(effectveDateTo.Effective_Date_To)) <= 0 ? true : false;
                    if (!validinBom)
                    {
                        return Json(new { status = false, message = "Please Check BOM detail against Part number- " + partNo }, JsonRequestBehavior.AllowGet);

                    }

                    RS_OM_SubAssembly_Order_List obj2 = new RS_OM_SubAssembly_Order_List();
                    if (Status == "Created")
                    {

                        obj2.Part_No = partNo;
                        obj2.Order_No = orderno;
                        obj2.Order_Status = Status;
                        obj2.Scan_Value = vendorBarcodevalue;
                        obj2.Plant_ID = Plant_Id;
                        obj2.Shop_ID = station.Shop_ID;
                        obj2.Line_ID = station.Line_ID;
                        obj2.Station_ID = station.Station_ID;
                        obj2.Shift_ID = shiftId;
                        obj2.Inserted_Host = userHost;
                        obj2.Inserted_User_ID = userId;
                        obj2.Inserted_Date = System.DateTime.Now;
                        //obj.Updated_Host = userHost;
                        //obj.Updated_User_ID = userId;
                        //obj.Updated_Date = System.DateTime.Now;
                        db.RS_OM_SubAssembly_Order_List.Add(obj2);
                        db.SaveChanges();
                    }
                    RS_WorkInstruction_Submitted submittedExist = db.RS_WorkInstruction_Submitted.Where(c => c.Order_No.ToUpper() == orderno && c.Model_Code.ToUpper() == partNo
                             && c.WI_Number == currentWI).FirstOrDefault();
                    if (submittedExist == null)
                    {
                        //adding in submitted workinstruction
                        RS_WorkInstruction_Submitted submitted = new RS_WorkInstruction_Submitted();
                        submitted.Model_Code = partNo;
                        submitted.Sequence_No = workinstruction.Sequence_No;
                        submitted.WI_Type_Name = workinstruction.WI_Type_Name;
                        submitted.WI_Description = workinstruction.WI_Description;
                        submitted.Order_No = orderno;
                        submitted.WI_Number = workinstruction.WI_Number;
                        submitted.Status = true;
                        submitted.Shift_ID = shiftId;
                        submitted.Station_ID = station.Station_ID;
                        submitted.Inserted_Host = userHost;
                        submitted.Inserted_User_ID = userId;
                        submitted.Inserted_Date = System.DateTime.Now;
                        db.RS_WorkInstruction_Submitted.Add(submitted);
                        db.SaveChanges();
                    }

                    return Json(new { status = true, SubAssemly = obj2, message = "Order " + Status+ "...!"}, JsonRequestBehavior.AllowGet);
                }
                else if(Status == "Started")
                {
                    isExist = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper() == partNo && c.Order_Status == "Created");
                    if (isExist)
                    {
                        //checking in BOM
                        var effectveDateTo = db.RS_BOM_Item.Where(c => c.Part_No == partNo).FirstOrDefault();
                        if (effectveDateTo == null)
                        {
                            return Json(new { status = false, message = "Please Check BOM detail against Part number- " + partNo }, JsonRequestBehavior.AllowGet);
                        }
                        bool validinBom = DateTime.Compare(System.DateTime.Now, Convert.ToDateTime(effectveDateTo.Effective_Date_To)) <= 0 ? true : false;
                        if (!validinBom)
                        {
                            return Json(new { status = false, message = "Please Check BOM detail against Part number- " + partNo }, JsonRequestBehavior.AllowGet);

                        }

                            RS_OM_SubAssembly_Order_List obj1 = db.RS_OM_SubAssembly_Order_List.Where(c => c.Part_No.ToUpper() == partNo && c.Order_No == orderno && c.Order_Status == "Created").FirstOrDefault();
                            if (obj1 == null)
                            {
                                return Json(new { status = false, message = "Order not Created yet..." }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                obj1.Order_Status = Status;
                                obj1.Updated_Host = userHost;
                                obj1.Updated_User_ID = userId;
                                obj1.Updated_Date = System.DateTime.Now;
                                db.Entry(obj1).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        RS_OM_SubAssembly_Order_List obj = db.RS_OM_SubAssembly_Order_List.Where(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper() == partNo).FirstOrDefault();
                        RS_WorkInstruction_Submitted submittedExist = db.RS_WorkInstruction_Submitted.Where(c => c.Order_No.ToUpper() == orderno && c.Model_Code.ToUpper() == partNo
                             && c.WI_Number == currentWI).FirstOrDefault();
                        if (submittedExist == null)
                        {
                            //adding in submitted workinstruction
                            RS_WorkInstruction_Submitted submitted = new RS_WorkInstruction_Submitted();
                            submitted.Model_Code = partNo;
                            submitted.Sequence_No = workinstruction.Sequence_No;
                            submitted.WI_Type_Name = workinstruction.WI_Type_Name;
                            submitted.WI_Description = workinstruction.WI_Description;
                            submitted.Order_No = orderno;
                            submitted.WI_Number = workinstruction.WI_Number;
                            submitted.Status = true;
                            submitted.Shift_ID = shiftId;
                            submitted.Station_ID = station.Station_ID;
                            submitted.Inserted_Host = userHost;
                            submitted.Inserted_User_ID = userId;
                            submitted.Inserted_Date = System.DateTime.Now;
                            db.RS_WorkInstruction_Submitted.Add(submitted);
                            db.SaveChanges();
                        }

                        return Json(new { status = true, SubAssemly = obj, message = "Order " + Status+"...!" }, JsonRequestBehavior.AllowGet);

                    }

                    isExist = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper() == partNo && c.Order_Status == "Closed");
                    if (isExist)
                    {
                        return Json(new { status = false, message = "Order already Closed.." }, JsonRequestBehavior.AllowGet);
                    }
                    isExist = db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper() == partNo && c.Order_Status == "Started");
                    if (isExist)
                    {
                        RS_WorkInstruction_Submitted submittedExist = db.RS_WorkInstruction_Submitted.Where(c => c.Order_No.ToUpper() == orderno && c.Model_Code.ToUpper() == partNo
                             && c.WI_Number == currentWI).FirstOrDefault();
                        if (submittedExist == null)
                        {
                            //adding in submitted workinstruction
                            RS_WorkInstruction_Submitted submitted = new RS_WorkInstruction_Submitted();
                            submitted.Model_Code = partNo;
                            submitted.Sequence_No = workinstruction.Sequence_No;
                            submitted.WI_Type_Name = workinstruction.WI_Type_Name;
                            submitted.WI_Description = workinstruction.WI_Description;
                            submitted.Order_No = orderno;
                            submitted.WI_Number = workinstruction.WI_Number;
                            submitted.Status = true;
                            submitted.Shift_ID = shiftId;
                            submitted.Station_ID = station.Station_ID;
                            submitted.Inserted_Host = userHost;
                            submitted.Inserted_User_ID = userId;
                            submitted.Inserted_Date = System.DateTime.Now;
                            db.RS_WorkInstruction_Submitted.Add(submitted);
                            db.SaveChanges();
                        }
                        var a=db.RS_OM_SubAssembly_Order_List.Any(c => c.Order_No.ToUpper() == orderno && c.Part_No.ToUpper() == partNo && c.Order_Status == "Started");
                        return Json(new { status = false, SubAssemly = a, message = "Order Already Started.." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, message = "Order not Created yet.." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, message = "Status not found" }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception Ex)
            {
                return Json(new { status = false, message = "Workinstruction list not available" }, JsonRequestBehavior.AllowGet);
            }
        }
        
       // GET: WorkInstruction/Details/5
       public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_WorkInstruction_Master rS_WorkInstruction_Master = db.RS_WorkInstruction_Master.Find(id);
            if (rS_WorkInstruction_Master == null)
            {
                return HttpNotFound();
            }
            return View(rS_WorkInstruction_Master);
        }

        // GET: WorkInstruction/Create
        public ActionResult Create()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            return View();
        }

        // POST: WorkInstruction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WI_Number,Model_Code,Station_ID,Sequence_No,WI_Type_Name,WI_Description,Part_No,Inserted_User_ID,Inserted_Date,Inserted_Host,Updated_User_ID,Updated_Date,Updated_Host,Is_Active")] RS_WorkInstruction_Master rS_WorkInstruction_Master)
        {
            if (ModelState.IsValid)
            {
                db.RS_WorkInstruction_Master.Add(rS_WorkInstruction_Master);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", rS_WorkInstruction_Master.Station_ID);
            return View(rS_WorkInstruction_Master);
        }

        // GET: WorkInstruction/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_WorkInstruction_Master rS_WorkInstruction_Master = db.RS_WorkInstruction_Master.Find(id);
            if (rS_WorkInstruction_Master == null)
            {
                return HttpNotFound();
            }
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", rS_WorkInstruction_Master.Station_ID);
            return View(rS_WorkInstruction_Master);
        }

        // POST: WorkInstruction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WI_Number,Model_Code,Station_ID,Sequence_No,WI_Type_Name,WI_Description,Part_No,Inserted_User_ID,Inserted_Date,Inserted_Host,Updated_User_ID,Updated_Date,Updated_Host,Is_Active")] RS_WorkInstruction_Master rS_WorkInstruction_Master)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rS_WorkInstruction_Master).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", rS_WorkInstruction_Master.Station_ID);
            return View(rS_WorkInstruction_Master);
        }

        // GET: WorkInstruction/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_WorkInstruction_Master rS_WorkInstruction_Master = db.RS_WorkInstruction_Master.Find(id);
            if (rS_WorkInstruction_Master == null)
            {
                return HttpNotFound();
            }
            return View(rS_WorkInstruction_Master);
        }

        // POST: WorkInstruction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_WorkInstruction_Master rS_WorkInstruction_Master = db.RS_WorkInstruction_Master.Find(id);
            db.RS_WorkInstruction_Master.Remove(rS_WorkInstruction_Master);
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
    }
}
