using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class AbsOperatorTransferController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        int plantId = 0, shopId = 0, lineId = 0, employeeId = 0, shiftId = 0;
        // GET: AbsOperatorTransfer
        public ActionResult Index()
        {
            var obj = db.RS_Abs_Operator_Transfer_Allocation.ToList();
            return View(obj);
        }

        public ActionResult Create()
        {
            try
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                    ViewBag.Old_Shop_ID = new SelectList("");
                    ViewBag.Old_Line_ID = new SelectList("");
                    ViewBag.New_Shop_ID = new SelectList("");
                    ViewBag.New_Line_ID = new SelectList("");
                    ViewBag.Shift_ID = new SelectList("");
                    ViewBag.Employee_ID = new SelectList("");
                    ViewBag.ListofSupervisor = new SelectList("");
                    ViewBag.selectedSupervisors = new SelectList("");
                }
                else
                {
                    ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                    ViewBag.ListofSupervisor = new SelectList(db.RS_Abs_Operator_Transfer_Allocation, "Employee_ID", "Employee_Name");
                    ViewBag.AssignedSupervisor_ID = new SelectList(db.RS_Abs_Operator_Transfer_Allocation, "Employee_ID", "Employee_Name");

                }

                globalData.pageTitle = "Abs Oprator Transformation";
                globalData.controllerName = "AbsOpratorTransfer";


                ViewBag.GlobalDataModel = globalData;

                ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
                ViewBag.Old_Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name");
                ViewBag.Old_Line_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name");
                ViewBag.New_Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name");
                ViewBag.New_Line_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name");
                ViewBag.Shift_ID = new SelectList(db.RS_Shift.OrderBy(c => c.Shift_Name), "Shift_ID", "Shift_Name");
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Abs_Operator_Transfer_Allocation RS_Abs_Operator_Transfer_Allocation)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    plantId = Convert.ToInt32(RS_Abs_Operator_Transfer_Allocation.Plant_ID);
                    shopId = Convert.ToInt32(RS_Abs_Operator_Transfer_Allocation.New_Shop_ID);
                    lineId = Convert.ToInt32(RS_Abs_Operator_Transfer_Allocation.New_Line_ID);
                    shiftId = Convert.ToInt32(RS_Abs_Operator_Transfer_Allocation.Shift_ID);



                    RS_Abs_Operator_Transfer_Allocation.deleteOperator(plantId, shopId, lineId, shiftId);//plantId,


                    for (int i = 0; i < RS_Abs_Operator_Transfer_Allocation.selectedOperators.Count(); i++)
                    {
                        var operatorId = Convert.ToInt16(RS_Abs_Operator_Transfer_Allocation.selectedOperators[i]);

                        if (operatorId == 0)
                            continue;

                        else
                        {

                            RS_Abs_Operator_Transfer_Allocation.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

                            RS_Abs_Operator_Transfer_Allocation.Inserted_Date = DateTime.Now;
                            RS_Abs_Operator_Transfer_Allocation.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                            RS_Abs_Operator_Transfer_Allocation.Employee_ID = RS_Abs_Operator_Transfer_Allocation.selectedOperators[i];

                            RS_Abs_Operator_Transfer_Allocation mmAssignSupervisorObj = new RS_Abs_Operator_Transfer_Allocation();
                            mmAssignSupervisorObj = RS_Abs_Operator_Transfer_Allocation;

                            db.Entry(mmAssignSupervisorObj).State = EntityState.Detached;

                            db.RS_Abs_Operator_Transfer_Allocation.Add(mmAssignSupervisorObj);

                            db.SaveChanges();
                        }

                    }
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "Abs Operator Transformation";
                    globalData.messageDetail = "Record Added Successfully";
                    TempData["globalData"] = globalData;

                }


                globalData.pageTitle = "Abs Operator Transformation";
                globalData.controllerName = "Abs Operator Transfer";
                ViewBag.GlobalDataModel = globalData;


                ViewBag.Shift_ID = new SelectList(db.RS_Shift, "Shift_ID", "Shift_Name", RS_Abs_Operator_Transfer_Allocation.Shift_ID);
                ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Abs_Operator_Transfer_Allocation.Plant_ID);
                ViewBag.Old_Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Abs_Operator_Transfer_Allocation.Old_Shop_ID);
                ViewBag.Old_Line_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Abs_Operator_Transfer_Allocation.Old_Line_ID);
                ViewBag.New_Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Abs_Operator_Transfer_Allocation.New_Shop_ID);
                ViewBag.New_Line_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Abs_Operator_Transfer_Allocation.New_Line_ID);
                ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Abs_Operator_Transfer_Allocation.Employee_ID);
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }

        public ActionResult GetShopListByPlantId(int plantId)
        {
            try
            {

                var st = from shop in db.RS_Shops
                         where shop.Plant_ID == plantId
                         orderby shop.Shop_Name
                         select new
                         {
                             Id = shop.Shop_ID,
                             Value = shop.Shop_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else if (ex.Message != null)
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLineListByShopId(int shopId)
        {
            try
            {
                decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                int supervisorID = ((FDSession)this.Session["FDSession"]).userId;

                var lineID = from sup in db.RS_AM_Line_Supervisor_Mapping
                             where sup.Employee_ID == supervisorID && sup.Plant_ID == plant_ID
                             select sup.Line_ID;

                var st = from line in db.RS_Lines
                         where line.Shop_ID == shopId && (lineID).Contains(line.Line_ID)
                         orderby line.Line_Name
                         select new
                         {
                             Id = line.Line_ID,
                             Value = line.Line_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetNewLineListByNewShopId(int shopId)
        {
            try
            {
                decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from line in db.RS_Lines
                         where line.Shop_ID == shopId
                         orderby line.Line_Name
                         select new
                         {
                             Id = line.Line_ID,
                             Value = line.Line_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetShiftListByShopID(int shopId)
        {
            try
            {
                //decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                TimeSpan currDate = DateTime.Now.TimeOfDay;
                var st = from shift in db.RS_Shift
                         where shift.Shop_ID == shopId
                          && TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                                && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                         orderby shift.Shift_Name
                         select new
                         {
                             Id = shift.Shift_ID,
                             Value = shift.Shift_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAbsOperatorByShiftID(int shiftId, int? shopId, int? lineId)//
        {
            try
            { 
                var st = (from a in db.RS_Employee
                                //join b in db.RS_Assign_OperatorToSupervisor on a.Employee_ID equals b.AssignedOperator_ID
                            join c in db.RS_User_Attendance_Sheet on a.Employee_No equals c.Employee_No
                            join d in db.RS_AM_Operator_Station_Allocation on a.Employee_ID equals d.Employee_ID
                            join e in db.RS_Stations on d.Station_ID equals e.Station_ID
                            where !(from sm in db.RS_Abs_Operator_Transfer_Allocation
                                    where sm.Allocation_Date == DateTime.Today
                                    select sm.Employee_ID ).Contains(a.Employee_ID)
                            where d.Line_ID == lineId && d.Shift_ID == shiftId && d.Is_Buffer_Station == true && d.Allocation_Date == DateTime.Today
                                    && c.Is_Present == true 
                                    && (c.Entry_Date.Value.Year == DateTime.Now.Year
                                          && c.Entry_Date.Value.Month == DateTime.Now.Month
                                            && c.Entry_Date.Value.Day == DateTime.Now.Day)

                            
                            select new
                            {
                               Id = a.Employee_ID,
                               Value = a.Employee_Name
                            }).Distinct().ToList();

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTransferOperatorByShiftID(int shiftId, int shopId, int lineId)
        {

            try
            {
                var allocationDate = DateTime.Today;
                var st = from emp in db.RS_Employee
                         where (from abstrans in db.RS_Abs_Operator_Transfer_Allocation
                                where abstrans.Shift_ID == shiftId && abstrans.Old_Shop_ID == shopId &&
                                abstrans.Old_Line_ID == lineId  && abstrans.Allocation_Date == allocationDate
                                select abstrans.Employee_ID).Contains(emp.Employee_ID)
                         orderby emp.Employee_Name
                         select new
                         {
                             Id = emp.Employee_ID,
                             Value = emp.Employee_Name
                         };


                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SaveTranserAbsOperators(string Operators, int shiftId, int shopId, int lineId, int nshopId, int nlineId)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            try
            {
                RS_Abs_Operator_Transfer_Allocation RS_Abs_Operator_Transfer_Allocation = new RS_Abs_Operator_Transfer_Allocation();

                RS_Abs_Operator_Transfer_Allocation.deleteOperator(plantId, shopId, lineId, shiftId);  //plantId, 


                string[] words;
                words = Operators.Split(',');

                foreach (string value in words)
                {
                    int i = 0;
                    if (value == "")
                    {
                        i = 0;
                    }
                    else
                    {
                        i = Convert.ToInt32(value);
                    }
                    if (i == 0)
                        continue;



                    var allocationDate = DateTime.Today;
                    var prevstationId = db.RS_Stations.Where(m => m.Line_ID == lineId && m.Is_Buffer == true).Select(m => m.Station_ID).FirstOrDefault();
                    var newstationId = db.RS_Stations.Where(m => m.Line_ID == nlineId && m.Is_Buffer == true).Select(m => m.Station_ID).FirstOrDefault();


                    var allocationId = db.RS_AM_Operator_Station_Allocation.Where(m => m.Employee_ID == i && m.Station_ID == prevstationId && m.Allocation_Date == allocationDate).Select(m => m.OSM_ID).FirstOrDefault();
                    if (allocationId > 0)
                    {
                        RS_AM_Operator_Station_Allocation obj = db.RS_AM_Operator_Station_Allocation.Find(allocationId);
                        obj.Plant_ID = plantId;
                        obj.Shop_ID = nshopId;
                        obj.Line_ID = nlineId;
                        obj.Station_ID = newstationId;
                        obj.Updated_Date = DateTime.Now;
                        obj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        obj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                        RS_AM_Operator_Station_Allocation_History obj1 = new RS_AM_Operator_Station_Allocation_History();
                        obj1.Allocation_Date = allocationDate;
                        obj1.Employee_ID = i;
                        obj1.Inserted_Date = DateTime.Now;
                        obj1.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        obj1.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        obj1.Is_Buffer_Station = true;
                        obj1.Is_Transferred = false;
                        obj1.Is_Purgeable = false;
                        obj1.Is_Transfer_Operator = true;
                        obj1.Line_ID = nlineId;
                        obj1.Shop_ID = nshopId;
                        obj1.Prev_Line_ID = lineId;
                        obj1.Prev_Shop_ID = shopId;
                        obj1.Station_ID = newstationId;
                        obj1.Plant_ID = plantId;
                        obj1.Shift_ID = shiftId;
                        db.RS_AM_Operator_Station_Allocation_History.Add(obj1);
                        db.SaveChanges();
                    }

                    RS_Abs_Operator_Transfer_Allocation.Plant_ID = plantId;
                    RS_Abs_Operator_Transfer_Allocation.Old_Shop_ID = shopId;
                    RS_Abs_Operator_Transfer_Allocation.Old_Line_ID = lineId;
                    RS_Abs_Operator_Transfer_Allocation.New_Shop_ID = nshopId;
                    RS_Abs_Operator_Transfer_Allocation.New_Line_ID = nlineId;
                    RS_Abs_Operator_Transfer_Allocation.Inserted_Date = DateTime.Now;
                    RS_Abs_Operator_Transfer_Allocation.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Abs_Operator_Transfer_Allocation.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    RS_Abs_Operator_Transfer_Allocation.Shift_ID = shiftId;
                    RS_Abs_Operator_Transfer_Allocation.Employee_ID = i;
                    RS_Abs_Operator_Transfer_Allocation.Allocation_Date = allocationDate;

                    db.Entry(RS_Abs_Operator_Transfer_Allocation).State = EntityState.Detached;
                    db.RS_Abs_Operator_Transfer_Allocation.Add(RS_Abs_Operator_Transfer_Allocation);
                    db.SaveChanges();


                    i = 0;
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}