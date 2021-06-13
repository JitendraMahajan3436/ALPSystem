using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
//using ZHB_AD.Helper.IoT;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.OrderManagement
{

    public class CriticalStationLoginController : Controller
    {
        private ZHB_ADEntities db = new ZHB_ADEntities();
        GlobalData globalData = new GlobalData();
        FDSession fdSession = new FDSession();
        General general = new General();
        //Kepware kepware = new Kepware();

        MM_Employee users = new MM_Employee();
        // GET: CriticalStationLogin
        public ActionResult Index()
        {
            return View();
        }

        // GET: CriticalStationLogin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CriticalStationLogin/Create
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
                if (globalData.isErrorMessage)
                {
                    ModelState.AddModelError("Token_ID", globalData.messageDetail);
                }
            }
            return View();
        }

        // POST: CriticalStationLogin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (TempData["globalData"]!=null)
                {
                    globalData =(GlobalData)TempData["globalData"];
                    if(globalData.isErrorMessage)
                    {
                        ModelState.AddModelError("Token_ID", globalData.messageDetail);
                    }
                }
                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }
        }

        // GET: CriticalStationLogin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CriticalStationLogin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CriticalStationLogin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CriticalStationLogin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /* Action Name                : GetoperatorsBySupervisorID
   *  Description                : Action used to get the operator list by supervisor Id
   *  Author, Timestamp          : Nilesh Gadhave
   *  Input parameter            : supervisorId 
   *  Return Type                : ActionResult
   *  Revision                   : 1.0
   */
        public bool IsExistToken(int tokenId)
        {
            try
            {
                IQueryable<MM_Employee> result;

                result = db.MM_Employee.Where(p => p.Employee_ID == tokenId);


                if (result.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult checkUserAuth(String Token_ID)
        {
            int stationID =  ((FDSession)this.Session["FDSession"]).stationId;//2
            int lineID = ((FDSession)this.Session["FDSession"]).lineId;
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;//2
            int shopID = ((FDSession)this.Session["FDSession"]).shopId;
            MM_Ctrl_LineStopEmergency obj1 = new MM_Ctrl_LineStopEmergency();
            if (IsUserExist(Token_ID))
            {
                var quer = db.MM_Employee.Where(p => p.Employee_No == Token_ID);
                users = quer.FirstOrDefault<MM_Employee>();
                string Token = Token_ID;
                var empno = (int)users.Employee_ID;
                if (IsLineSupervisor(empno) && IsLineStop(lineID))
                {
                    // Line resume tag logic
                    //kepware.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                    //kepware.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
                    MM_History_LineStop originalLSData = db.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == "Invalid Critical login" && a.Line_ID == lineID && a.Station_ID == stationID).OrderByDescending(a => a.Inserted_Date).FirstOrDefault();
                    if (originalLSData != null)
                    {
                        DateTime dateTimeNow;
                        TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                        if (dateObj != null)
                        {
                            dateTimeNow = dateObj.todayDate;
                        }
                        else
                        {
                            dateTimeNow = DateTime.Now;
                        }
                        UpdateModel(originalLSData);
                        originalLSData.Is_Edited = true;
                        originalLSData.Resume_Time = dateTimeNow;
                        db.Entry(originalLSData).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "Sucess";
                    globalData.messageDetail = "Line Resumed Successfully.";
                    ViewBag.GlobalDataModel = globalData;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Create", "CriticalStationLogin");
                }
                else
                {

                    if (IsEmployeeAllocated(empno, stationID)) //, stationID
                    {
                        if (IsUserPrescent(empno))
                        {
                            FDSession fdSessionObj = (FDSession)this.Session["FDSession"];
                            var query = db.MM_Employee.Where(p => p.Employee_ID == empno);
                            users = query.FirstOrDefault<MM_Employee>();
                            fdSessionObj.userId = (int)users.Employee_ID;
                            fdSessionObj.userName = users.Employee_Name;
                            fdSessionObj.insertedDate = (DateTime)users.Inserted_Date;

                            // process to add user details
                            MM_AM_User_Sessions mmAMUserSessionObj = new MM_AM_User_Sessions();
                            mmAMUserSessionObj.Plant_ID = fdSessionObj.plantId;
                            mmAMUserSessionObj.Shop_ID = fdSessionObj.shopId;
                            mmAMUserSessionObj.Line_ID = fdSessionObj.lineId;
                            mmAMUserSessionObj.Station_ID = fdSessionObj.stationId;
                            mmAMUserSessionObj.Employee_ID = fdSessionObj.userId;
                            mmAMUserSessionObj.Session_ID = HttpContext.Session.SessionID;
                            mmAMUserSessionObj.Login_Date = DateTime.Now;
                            db.MM_AM_User_Sessions.Add(mmAMUserSessionObj);
                            db.SaveChanges();

                            this.Session["FDSession"] = fdSessionObj;
                            return RedirectToAction("ShopScreen", "Manifest");
                        }
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = "Error";
                        globalData.messageDetail = "Employee is not Present.The Line has been Stopped!";
                        ViewBag.GlobalDataModel = globalData;

                        //Call line stop tag                       
                        general.logLineStopData(plantID, shopID, lineID, stationID, "Invalid Critical login", "Token id :" + Token_ID + ". User is absent");

                        TempData["globalData"] = globalData;
                        return RedirectToAction("Create", "CriticalStationLogin");
                    }
                    else
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = "Error";
                        globalData.messageDetail = "Employee is not allocated to station. The Line has been Stopped!";
                        ViewBag.GlobalDataModel = globalData;
                        //To do : call line stop tag
                        general.logLineStopData(plantID, shopID, lineID, stationID, "Invalid Critical login", "Token id :" + Token_ID + ".Employee is not allocated to station");

                        //obj1.Plant_ID = 1;
                        //obj1.Shop_ID =  ((FDSession)this.Session["FDSession"]).shopId;
                        //obj1.Line_ID =  ((FDSession)this.Session["FDSession"]).lineId;
                        //obj1.Station_ID = ((FDSession)this.Session["FDSession"]).stationId;
                        //obj1.Inserted_Date = DateTime.Now;
                        //obj1.Line_Stop = true;
                        //obj1.Emergency_Call = false;
                        //db.MM_Ctrl_LineStopEmergency.Add(obj1);
                        //db.SaveChanges();
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Create", "CriticalStationLogin");

                    }
                }
            }
            else
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Error";
                globalData.messageDetail = "Login User does not exist in plant.The Line has been Stopped!";
                ViewBag.GlobalDataModel = globalData;

                //Call line stop tag
                general.logLineStopData(plantID, shopID, lineID, stationID, "Invalid Critical login", "Token id :" + Token_ID + ".User not exist in plant");

                TempData["globalData"] = globalData;
                return RedirectToAction("Create", "CriticalStationLogin");
                
                //return View("Create", Token_ID);
            }
        }

        public bool IsEmployeeAllocated(int userId, int stationId)  //, int stationId
        {
            try
            {
                bool result = db.MM_AM_Operator_Station_Allocation.Any(p => p.Employee_ID == userId && p.Station_ID == stationId);
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public bool IsLineStop(int lineID)
        {
            try
            {
                string query = "SELECT * FROM MM_Ctrl_LineStopEmergency WHERE Line_ID = @p0 AND Line_Stop = 1 AND " +
                               " Inserted_Date = (SELECT MAX(Inserted_Date) FROM MM_Ctrl_LineStopEmergency WHERE Line_ID = @p0 AND Line_Stop = 1)";

                var dataObj = db.Database.SqlQuery<MM_Ctrl_LineStopEmergency>(query, lineID).ToList();
                return (dataObj.Count > 0);

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsUserExist(String userId)
        {
            try
            {
                bool result = db.MM_Employee.Any(p => p.Employee_No == userId);
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool IsLineSupervisor(int empno)
        {
            int lineID = ((FDSession)this.Session["FDSession"]).lineId;
            try
            {

                bool result = db.MM_AM_Line_Supervisor_Mapping.Any(p => p.Employee_ID == empno && p.Line_ID == lineID);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public bool IsOperatorToStationAllocated(String operatorTokenId, decimal stationId, decimal shiftId)
        {
            MM_AM_Operator_Station_Allocation obj = new MM_AM_Operator_Station_Allocation();
            bool Res = obj.IsOperatorToStationAllocated(operatorTokenId, stationId, shiftId);
            return Res;

            //return obj.IsOperatorToStationAllocated(operatorTokenId, stationId, shiftId);
        }

        /* Action Name                : GetoperatorsBySupervisorID
  *  Description                : Action used to get the operator list by supervisor Id
  *  Author, Timestamp          : Nilesh Gadhave
  *  Input parameter            : supervisorId 
  *  Return Type                : ActionResult
  *  Revision                   : 1.0
  */
        public bool IsCriticalStation(int stationId)
        {
            try
            {
                IQueryable<MM_Stations> result1;

                result1 = db.MM_Stations.Where(S => S.Station_ID == stationId && S.Is_Critical_Station == true);


                if (result1.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public bool IsUserPrescent(int empno)
        {
            try
            {
                IQueryable<MM_User_Attendance_Sheet> result1;

                result1 = db.MM_User_Attendance_Sheet.Where(S => S.Employee_ID == empno && S.Is_Present == true
                                    && S.Entry_Date.Value.Year == DateTime.Now.Year
                                    && S.Entry_Date.Value.Month == DateTime.Now.Month
                                    && S.Entry_Date.Value.Day == DateTime.Now.Day);


                if (result1.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
