using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Helper;
using REIN_MES_System.App_LocalResources;
using System.Linq;
using REIN_MES_System.Controllers.BaseManagement;
using System.Configuration;
using System.Data.SqlClient;
namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class EmployeeChangeRequestController : BaseController
    {
        // GET: EmployeeChangeRequest
        REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        string connectionString = ConfigurationManager.ConnectionStrings["REIN_SOLUTIONEntities_SP"].ToString();
        DataTable dt = new DataTable();
        int plantId = 0;
        public ActionResult Index()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            if (TempData["employeeNumber"] != null)
            {
                TempData.Keep("employeeNumber");
            }
            //string employeeno = "0125478";
            globalData.pageTitle = ResourceModules.User;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = ResourceModules.User;
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceGlobal.User + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceGlobal.User + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }
        public ActionResult EmployeeList(string employeeNumber)
        {

            TempData["employeeNumber"] = employeeNumber;
            decimal employee_ID = db.RS_Employee.Where(emp => emp.Employee_No == employeeNumber).Select(emp => emp.Employee_ID).FirstOrDefault();
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            //var mm_Operator_Allocation = db.RS_AM_Operator_Station_Allocation_History.Include(m => m.RS_Employee).Include(p => p.RS_Plants).Include(s => s.RS_Shops).Include(l => l.RS_Lines).Include(st => st.RS_Stations).Include(cr => cr.RS_AM_Operator_Change_Request).Where(m => m.Employee_ID == employee_ID && m.Plant_ID == plantId && DbFunctions.TruncateTime(m.Allocation_Date) > DbFunctions.TruncateTime(DateTime.Now));
            dt = null;
            dt = GetAllocationList(employee_ID, plantId);
            List<RS_AM_Operator_Station_Allocation_History> employeeList = new List<RS_AM_Operator_Station_Allocation_History>();
            RS_AM_Operator_Station_Allocation_History obj = new RS_AM_Operator_Station_Allocation_History();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                obj = new RS_AM_Operator_Station_Allocation_History();
                obj.Row_ID = Convert.ToInt32(dt.Rows[i]["Row_ID"]);
                obj.Plant_ID = Convert.ToDecimal(dt.Rows[i]["Plant_ID"]);
                obj.Shop_ID = Convert.ToDecimal(dt.Rows[i]["Shop_ID"]);
                obj.Line_ID = Convert.ToDecimal(dt.Rows[i]["Line_ID"]);
                obj.Station_ID = Convert.ToDecimal(dt.Rows[i]["Station_ID"]);
                obj.Shift_ID = Convert.ToDecimal(dt.Rows[i]["Shift_ID"]);
                obj.Employee_ID = Convert.ToDecimal(dt.Rows[i]["Employee_ID"]);
                obj.Allocation_Date = Convert.ToDateTime(dt.Rows[i]["Allocation_Date"]);
                obj.Week_Day = Convert.ToString(dt.Rows[i]["Week_Day"]);
                obj.Request_Status = Convert.ToString(dt.Rows[i]["rStatus"]);
                obj.Employee_No = Convert.ToString(dt.Rows[i]["Employee_No"]);
                obj.Shift_Name = Convert.ToString(dt.Rows[i]["Shift_Name"]);
                obj.Employee_Name = Convert.ToString(dt.Rows[i]["Employee_Name"]);
                obj.NewShift = Convert.ToString(dt.Rows[i]["New Shift"]);
                employeeList.Add(obj);
               
            }

            return PartialView("EmployeeList", employeeList);
        }

        public ActionResult ChangeRequest(decimal? id)
        {
            DateTime allocation_date = Convert.ToDateTime(Request["allocation_date"]);
            RS_AM_Operator_Station_Allocation_History objOperator_Allocation_History = new RS_AM_Operator_Station_Allocation_History();
            objOperator_Allocation_History = db.RS_AM_Operator_Station_Allocation_History.Where(hist => hist.Employee_ID == id && DbFunctions.TruncateTime(hist.Allocation_Date) == DbFunctions.TruncateTime(allocation_date)).FirstOrDefault();
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(c => c.Plant_ID == plantId).OrderBy(s => s.Shop_Name), "Shop_ID", "Shop_Name", objOperator_Allocation_History.Shop_ID);
            ViewBag.Line_ID = new SelectList((db.RS_Lines.Where(S => S.Shop_ID == objOperator_Allocation_History.Shop_ID)).OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", objOperator_Allocation_History.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(sta => sta.Line_ID == objOperator_Allocation_History.Line_ID), "Station_ID", "Station_Name", objOperator_Allocation_History.Station_ID);
            ViewBag.Shift = new SelectList(db.RS_Shift.Where(shift => shift.Plant_ID == plantId), "Shift_ID", "Shift_Name", objOperator_Allocation_History.Shift_ID);
            TempData["shift_ID"] = objOperator_Allocation_History.Shift_ID;
            // TempData["employeeNumber"]
            TempData.Keep("employeeNumber");

            return View(objOperator_Allocation_History);
        }
        [HttpPost]
        public JsonResult SaveChangeRequest(decimal Row_ID, decimal? Shift_ID, decimal? Shop_ID, decimal? Line_ID, decimal? Station_ID, decimal? Shift, decimal? Employee_ID, DateTime Allocation_Date)
        {
            try
            {
                RS_AM_Operator_Change_Request objOperator_Change_Request = new RS_AM_Operator_Change_Request();
                // DateTime allocation_date = Allocation_Date;
                List<RS_AM_Operator_Change_Request> objOperator_Change_Requests = (from cr in db.RS_AM_Operator_Change_Request
                                                                                   join his in db.RS_AM_Operator_Station_Allocation_History
                                                                                   on cr.Row_ID equals his.Row_ID
                                                                                   where his.Employee_ID == Employee_ID &&
                                                                                   DbFunctions.TruncateTime(his.Allocation_Date) == DbFunctions.TruncateTime(Allocation_Date)
                                                                                   select (cr)).ToList();
                if (objOperator_Change_Requests.Count > 0)
                {
                    objOperator_Change_Request = (from cr in db.RS_AM_Operator_Change_Request
                                                  join his in db.RS_AM_Operator_Station_Allocation_History
                                                  on cr.Row_ID equals his.Row_ID
                                                  where his.Employee_ID == Employee_ID &&
                                                  DbFunctions.TruncateTime(his.Allocation_Date) == DbFunctions.TruncateTime(Allocation_Date)
                                                  select (cr)).FirstOrDefault();
                    RS_AM_Operator_Change_Request objOperator_Change = new RS_AM_Operator_Change_Request();
                    objOperator_Change = db.RS_AM_Operator_Change_Request.Find(objOperator_Change_Request.ID);
                    objOperator_Change.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    objOperator_Change.Row_ID = Row_ID;
                    objOperator_Change.Shop_ID = Shop_ID.Value;
                    objOperator_Change.Line_ID = Line_ID.Value;
                    objOperator_Change.Station_ID = Station_ID.Value;
                    objOperator_Change.Current_Shift_ID = Shift_ID.Value;
                    objOperator_Change.Updated_Date = DateTime.Now;
                    objOperator_Change.New_Shift_ID = Shift.Value;
                    objOperator_Change.Request_Status = 0;
                    objOperator_Change.Updated_User_ID = Employee_ID;
                    objOperator_Change.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    objOperator_Change.Is_Edited = true;
                    db.Entry(objOperator_Change).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { msg = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (Shift_ID == Shift)
                    {
                        return Json(new { msg = "ChangeShift" }, JsonRequestBehavior.AllowGet);
                    }
                    int shiftID = Convert.ToInt32(TempData["shift_ID"]);
                    objOperator_Change_Request = new RS_AM_Operator_Change_Request();
                    objOperator_Change_Request.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    objOperator_Change_Request.Row_ID = Row_ID;
                    objOperator_Change_Request.Shop_ID = Shop_ID.Value;
                    objOperator_Change_Request.Line_ID = Line_ID.Value;
                    objOperator_Change_Request.Station_ID = Station_ID.Value;
                    objOperator_Change_Request.Current_Shift_ID = Shift_ID.Value;
                    objOperator_Change_Request.Inserted_Date = DateTime.Now;
                    objOperator_Change_Request.New_Shift_ID = Shift.Value;
                    objOperator_Change_Request.Request_Status = 0;
                    objOperator_Change_Request.Employe_ID = Employee_ID;
                    objOperator_Change_Request.Inserted_User_ID = Employee_ID;
                    objOperator_Change_Request.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    objOperator_Change_Request.Allocation_Date = Allocation_Date;
                    db.RS_AM_Operator_Change_Request.Add(objOperator_Change_Request);
                    db.SaveChanges();
                    return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { msg = "Error" }, JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult isValidToken(string employeeNumber)
        {
            var employee_ID = db.RS_Employee.Where(emp => emp.Employee_No == employeeNumber).Select(emp => emp.Employee_ID).ToList();
            if (employee_ID.Count > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ChangeRequestNotification()
        {
            return View();
        }

        public ActionResult getChangeRequest()
        {
            decimal supervisorID = ((FDSession)this.Session["FDSession"]).userId;
            var objChangeRequest = (from cr in db.RS_AM_Operator_Change_Request
                                    join his in db.RS_AM_Operator_Station_Allocation_History
                                    on cr.Row_ID equals his.Row_ID
                                    where (db.RS_Assign_OperatorToSupervisor.Where(op => op.Supervisor_ID == supervisorID).Select(op => op.AssignedOperator_ID)
                                    ).Contains(cr.Employe_ID.Value) &&
                                   DbFunctions.TruncateTime(his.Allocation_Date) >= DbFunctions.TruncateTime(DateTime.Now)
                                   && cr.Request_Status == 0
                                    select cr).ToList();
            return PartialView("getChangeRequest", objChangeRequest);
        }

        public ActionResult getRejectRequest()
        {
            decimal supervisorID = ((FDSession)this.Session["FDSession"]).userId;
            var objChangeRequest = (from cr in db.RS_AM_Operator_Change_Request
                                    join his in db.RS_AM_Operator_Station_Allocation_History
                                    on cr.Row_ID equals his.Row_ID
                                    where (db.RS_Assign_OperatorToSupervisor.Where(op => op.Supervisor_ID == supervisorID).Select(op => op.AssignedOperator_ID)
                                    ).Contains(cr.Employe_ID.Value) &&
                                   DbFunctions.TruncateTime(his.Allocation_Date) >= DbFunctions.TruncateTime(DateTime.Now)
                                   && cr.Request_Status == 2
                                    select (cr)).ToList();
            return PartialView("getRejectRequest", objChangeRequest);
        }
        public ActionResult getAcceptRequest()
        {
            decimal supervisorID = ((FDSession)this.Session["FDSession"]).userId;
            var objChangeRequest = (from cr in db.RS_AM_Operator_Change_Request
                                    join his in db.RS_AM_Operator_Station_Allocation_History
                                    on cr.Row_ID equals his.Row_ID
                                    where (db.RS_Assign_OperatorToSupervisor.Where(op => op.Supervisor_ID == supervisorID).Select(op => op.AssignedOperator_ID)
                                    ).Contains(cr.Employe_ID.Value) &&
                                   DbFunctions.TruncateTime(his.Allocation_Date) >= DbFunctions.TruncateTime(DateTime.Now)
                                   && cr.Request_Status == 1
                                    select (cr)).ToList();
            return PartialView("getAcceptRequest", objChangeRequest);
        }

        public ActionResult changeRequestStatusByLineOfficer(int id, DateTime allocation_date)
        {
            RS_AM_Operator_Change_Request obj = new RS_AM_Operator_Change_Request();
            obj = db.RS_AM_Operator_Change_Request.Find(id);
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(c => c.Plant_ID == plantId).OrderBy(s => s.Shop_Name), "Shop_ID", "Shop_Name", obj.Shop_ID);
            ViewBag.Line_ID = new SelectList((db.RS_Lines.Where(S => S.Shop_ID == obj.Shop_ID)).OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", obj.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(sta => sta.Line_ID == obj.Line_ID), "Station_ID", "Station_Name", obj.Station_ID);
            ViewBag.Shift = new SelectList(db.RS_Shift.Where(shift => shift.Plant_ID == plantId), "Shift_ID", "Shift_Name", obj.New_Shift_ID);
            return View(obj);
        }

        [HttpPost]
        public JsonResult UpdateChangeRequestStatus(int Row_ID, int Status)
        {
            try
            {
                RS_AM_Operator_Change_Request obj = db.RS_AM_Operator_Change_Request.Find(Row_ID);
                obj.Request_Status = Status;
                obj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                obj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                obj.Is_Edited = true;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }


        }

        public DataTable GetAllocationList(decimal empID, int plantID)
        {
            try
            {
                DataTable dtt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllocationList", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@empID", empID);
                        cmd.Parameters.AddWithValue("@plantID", plantID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dtt);
                        con.Close();
                    }
                }
                return dtt;
            }
            catch (Exception ex)
            {
                return dt;
            }
        }

    }
}