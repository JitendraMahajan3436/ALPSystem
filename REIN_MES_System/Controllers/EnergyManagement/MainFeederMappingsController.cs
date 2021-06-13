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

namespace ZHB_AD.Controllers.MasterManagement
{
    public class MainFeederMappingsController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();
        // GET: MainFeederMappings
        public ActionResult Index()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                //Session["PageTitle"] = "Main Feeder (Meter) Configuration";
                globalData.pageTitle = ResourceMainFeederMapping.MainFeeder_List;

                //globalData.controllerName = "StationRoles";
                //globalData.actionName = ResourceGlobal.Details;
                //globalData.contentTitle = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
                //globalData.contentFooter = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
                ViewBag.GlobalDataModel = globalData;

                var result = db.UtilityMainFeederMappings.Include(m => m.MM_MTTUW_Shops).Include(m => m.MM_Feeders).Take(100);
                
            
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Energy==true).ToList(), "Shop_ID", "Shop_Name");
                return View(result.Where(s=>s.Plant_ID ==plantID).ToList());

            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "user");
            }
            
        }

        // GET: MainFeederMappings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UtilityMainFeederMapping utilityMainFeederMapping = db.UtilityMainFeederMappings.Find(id);
            if (utilityMainFeederMapping == null)
            {
                return HttpNotFound();
            }
            return View(utilityMainFeederMapping);
        }

        // GET: MainFeederMappings/Create
        public ActionResult Create()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                ViewBag.Plant_ID = plantID;
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name");
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s=>s.Plant_ID ==plantID), "Shop_ID", "Shop_Name");
                var Result = (from f in db.MM_Category
                              select new
                              {
                                  Category_Id = f.Category_Id,
                                  Category_Name = f.Category_Name + "(" + f.Production +")",
                                 
                              }).ToList();
                //var userObj = from employee in db.MM_Employee
                //              where employee.Employee_ID == mm_user_roles.Employee_ID
                //              select new
                //              {
                //                  Employee_ID = employee.Employee_ID,
                //                  Employee_Name = employee.Employee_Name + "(" + employee.Employee_No + ")"
                //              };
                ViewBag.Category_ID = new SelectList(Result, "Category_Id", "Category_Name");
                //ViewBag.Category_ID = Result;
                ViewBag.Parameter_ID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name");
                ViewBag.Meter = new SelectList(db.MM_Meter, "MeterName", "MeterName");
                ViewBag.Meter_ID = new SelectList(db.MM_Meter, "MeterID", "MeterName");
                ViewBag.Unit_ID = new SelectList(db.MM_Unit_Measurement, "Unit_ID", "Data_Unit");
                ViewBag.Area_ID = new SelectList(db.MM_Area, "Area_ID","Area_Name");        
                ViewBag.Feeder_ID = new SelectList(db.MM_Feeders, "Feeder_Id", "FeederName");
                ViewBag.Load_ID = new SelectList(db.MM_Load, "Load_ID", "Load_Name");
                
                globalData.pageTitle = ResourceMainFeederMapping.Title_Add_MainFeeder;
                ViewBag.GlobalDataModel = globalData;
                return View();
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        
        //[HttpPost]
        //public ActionResult Create(UtilityMainFeederMapping utilityMainFeederMapping)
        //{
        //    string compName = ((FDSession)this.Session["FDSession"]).userHost;
        //    try
        //    {
        //        int plantID1 = ((FDSession)this.Session["FDSession"]).plantId;
        //        int userID1 = ((FDSession)this.Session["FDSession"]).userId;


        //        var feeder = utilityMainFeederMapping.FeederName;
        //        int shopId = Convert.ToInt16(utilityMainFeederMapping.Shop_ID);
        //        int CatergoryId = Convert.ToInt16(utilityMainFeederMapping.Category_ID);
        //        int TagIndex = Convert.ToInt16(utilityMainFeederMapping.TagIndex);
        //    }
        //    catch (Exception ex)
        //    {
        //        generalObj.addControllerException(ex, "utilityMainFeederMapping", "Create", ((FDSession)this.Session["FDSession"]).userId);

        //        var mes = ex;
        //        globalData.isErrorMessage = true;
        //        globalData.messageTitle = ResourceMainFeederMapping.MainFeeder_List;
        //        globalData.messageDetail = "child1 loop ";
        //        TempData["globalData"] = globalData;
        //        return RedirectToAction("Index");
        //    }
                      
        //                {
        //                            UtilityMainFeederMapping obj = new UtilityMainFeederMapping();
                
        //                      int userID = ((FDSession)this.Session["FDSession"]).userId;
        //                   //      utilityMainFeederMapping.Inserted_Date = DateTime.Now;
        //                   // utilityMainFeederMapping.Inserted_Host = Request.UserHostName;
        //                   //utilityMainFeederMapping.Inserted_User_ID = userID;
        //                    try          
        //                    {
        //                        obj.Active = utilityMainFeederMapping.Active;
        //                        obj.Category_ID = utilityMainFeederMapping.Category_ID;
        //            obj.Consider = utilityMainFeederMapping.Consider;
        //            obj.FeederName = utilityMainFeederMapping.FeederName;
        //            obj.Inserted_Date = DateTime.Now;
        //            obj.Inserted_Host = compName;
        //            obj.Inserted_User_ID = userID;
        //            obj.ManualMeter = utilityMainFeederMapping.ManualMeter;
        //            obj.Meter = utilityMainFeederMapping.Meter;
        //            obj.MeterType = utilityMainFeederMapping.MeterType;
        //            obj.ParameterDesc = utilityMainFeederMapping.ParameterDesc;
        //            obj.Parameter_ID = utilityMainFeederMapping.Parameter_ID;
        //            obj.Plant_ID = utilityMainFeederMapping.Plant_ID;
        //            obj.PortName = utilityMainFeederMapping.PortName;
        //            obj.ShopName = utilityMainFeederMapping.ShopName;
        //            obj.Shop_ID = utilityMainFeederMapping.Shop_ID;
        //            obj.TagIndex = utilityMainFeederMapping.TagIndex;
        //            obj.TagName = utilityMainFeederMapping.TagName;
        //            obj.Target = utilityMainFeederMapping.Target;
        //            obj.Unit = utilityMainFeederMapping.Unit;
                    
        //                        db.UtilityMainFeederMappings.Add(obj);
        //                        db.SaveChanges();
        //            globalData.isSuccessMessage = true;
        //            globalData.messageTitle = ResourceMainFeederMapping.MainFeeder_List;
        //            globalData.messageDetail = ResourceMainFeederMapping.MainFeederMapping_Success_MainFeederMapping_Add_Success;
        //            TempData["globalData"] = globalData;
        //            return RedirectToAction("Index");

        //        }
        //                    catch (Exception ex)
        //                    {
        //                        generalObj.addControllerException(ex, "utilityMainFeederMapping", "Create", ((FDSession)this.Session["FDSession"]).userId);

        //                        var mes = ex;
        //                        globalData.isErrorMessage = true;
        //                        globalData.messageTitle = ResourceMainFeederMapping.MainFeeder_List;
        //                        globalData.messageDetail = "child2 loop ";
        //                        TempData["globalData"] = globalData;
        //                        return RedirectToAction("Index");

        //                    }

                        
        //                }


        //    int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            
        //    ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
        //        ViewBag.Category_ID = new SelectList(db.MM_Category.Where(s => s.Plant_ID == plantID), "Category_Id", "Category_Name");
        //        ViewBag.Parameter_ID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name");
        //        ViewBag.Meter = new SelectList(db.MM_Meter, "MeterName", "MeterName");
        //        return View(utilityMainFeederMapping);
        //    }







            [HttpPost]
        public ActionResult Create(UtilityMainFeederMapping utilityMainFeederMapping)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                string compName = ((FDSession)this.Session["FDSession"]).userHost;
                try
                {
                    if (ModelState.IsValid)
                    {
                        int feeder = Convert.ToInt32(utilityMainFeederMapping.Feeder_ID);
                        int shopId = Convert.ToInt32(utilityMainFeederMapping.Shop_ID);
                        int CatergoryId = Convert.ToInt32(utilityMainFeederMapping.Category_ID);
                        int TagIndex = Convert.ToInt32(utilityMainFeederMapping.TagIndex);
                        int ParameterID = Convert.ToInt32(utilityMainFeederMapping.Parameter_ID);

                        if (utilityMainFeederMapping.IsTagIndexExists(TagIndex, plantID, 0))
                        {
                            ModelState.AddModelError("TagIndex", ResourceValidation.Exist);
                        }
                        else if (utilityMainFeederMapping.IsFeederExists(feeder, plantID, shopId, ParameterID, 0))
                        {
                            ModelState.AddModelError("FeederName", ResourceValidation.Exist);
                        }
                        else
                        {


                            UtilityMainFeederMapping obj = new UtilityMainFeederMapping();

                            obj.Active = utilityMainFeederMapping.Active;
                            obj.Category_ID = utilityMainFeederMapping.Category_ID;
                            obj.Consider = utilityMainFeederMapping.Consider;
                            obj.FeederName = utilityMainFeederMapping.FeederName;
                            obj.Inserted_Date = DateTime.Now;
                            obj.Inserted_Host = compName;
                            obj.Inserted_User_ID = userID;
                            obj.ManualMeter = utilityMainFeederMapping.ManualMeter;
                            obj.Meter_ID = utilityMainFeederMapping.Meter_ID;
                            obj.Unit_ID = utilityMainFeederMapping.Unit_ID;
                            obj.Area_ID = utilityMainFeederMapping.Area_ID;
                            obj.Meter = utilityMainFeederMapping.Meter;
                            //if(utilityMainFeederMapping.Meter == "Main Meter")
                            //{
                            //    obj.MeterType = true;
                            //}
                            //else
                            //{
                            //    obj.MeterType = false;
                            //}
                           
                            obj.ConsumptionType = utilityMainFeederMapping.ConsumptionType;
                            obj.ParameterDesc = utilityMainFeederMapping.ParameterDesc;
                            obj.Parameter_ID = utilityMainFeederMapping.Parameter_ID;
                            obj.Plant_ID = utilityMainFeederMapping.Plant_ID;
                            obj.PortName = utilityMainFeederMapping.PortName;
                            obj.ShopName = utilityMainFeederMapping.ShopName;
                            obj.Shop_ID = utilityMainFeederMapping.Shop_ID;
                            obj.TagIndex = utilityMainFeederMapping.TagIndex;
                            obj.TagName = utilityMainFeederMapping.TagName;
                            obj.Target = utilityMainFeederMapping.Target;
                            obj.Unit = utilityMainFeederMapping.Unit;
                            obj.Is_Incomer = utilityMainFeederMapping.Is_Incomer;
                            obj.Feeder_ID = utilityMainFeederMapping.Feeder_ID;
                            obj.Load_ID = utilityMainFeederMapping.Load_ID;
                            db.UtilityMainFeederMappings.Add(obj);
                            db.SaveChanges();



                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceMainFeederMapping.MainFeeder_List;
                            globalData.messageDetail = ResourceMainFeederMapping.MainFeederMapping_Success_MainFeederMapping_Add_Success;
                            TempData["globalData"] = globalData;
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceMainFeederMapping.MainFeeder_List;
                    globalData.messageDetail = "1st inner loop ";
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");

                }

                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                ViewBag.Category_ID = new SelectList(db.MM_Category.Where(s => s.Plant_ID == plantID), "Category_Id", "Category_Name");
                ViewBag.Parameter_ID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name");
                ViewBag.Meter = new SelectList(db.MM_Meter, "MeterName", "MeterName");
                ViewBag.Meter_ID = new SelectList(db.MM_Meter, "MeterID", "MeterName");
                ViewBag.Unit_ID = new SelectList(db.MM_Unit_Measurement, "Unit_ID", "Data_Unit");
                ViewBag.Area_ID = new SelectList(db.MM_Area, "Area_ID", "Area_Name");
                return View(utilityMainFeederMapping);
            }
            catch (Exception ex)
            {
                generalObj.addControllerException(ex, "utilityMainFeederMapping", "Create", ((FDSession)this.Session["FDSession"]).userId);

                var mes = ex;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMainFeederMapping.MainFeeder_List;
                globalData.messageDetail = "2nd inner loop ";
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");

            }

        }

        public ActionResult addFeeder(List<Feederlist> feeders)
        {
            string validatemsg;
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                string compName = ((FDSession)this.Session["FDSession"]).userHost;
                foreach (var id in feeders)
                {
                    UtilityMainFeederMapping obj = new UtilityMainFeederMapping();
                    obj.Shop_ID = id.Shop;
                    obj.Feeder_ID = id.feeder;
                    obj.Category_ID = id.Category;
                    if(id.Area!=0)
                    {
                        obj.Area_ID = id.Area;
                    }
                    
                    obj.Meter_ID = id.Meter;
                    obj.Active = id.sharedfeeder;
                    obj.ManualMeter = id.Manualfeeder;
                    obj.Parameter_ID = id.Prameter;
                    obj.Unit_ID = id.Unit;
                    obj.TagIndex = id.TagIndex;
                    obj.TagName = id.TagName;
                    obj.Consider = id.Active;
                    obj.Plant_ID = plantID;
                    obj.Inserted_User_ID = userID;
                    obj.Inserted_Host = compName;
                    obj.Load_ID = id.Load_ID;
                    obj.Inserted_Date = System.DateTime.Now;
                     if(id.Prameter==1)
                    {
                        obj.Ratedload = id.RatedLoad;
                        obj.ConsumptionType = true;
                    }
                    if (id.Prameter!=0)
                    {
                        db.UtilityMainFeederMappings.Add(obj);
                        db.SaveChanges();
                    }
                }
              
                validatemsg = "Feeder (Meter) Mapping is config added successfully .......!";
                return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                validatemsg = "Try agin.....!";
                return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
            }
          
        }


        public ActionResult Edit(int? id)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UtilityMainFeederMapping utilityMainFeederMapping = db.UtilityMainFeederMappings.Find(id);
                if (utilityMainFeederMapping == null)
                {
                    return HttpNotFound();
                }
                

                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name", utilityMainFeederMapping.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s=>s.Plant_ID ==plantID), "Shop_ID", "Shop_Name", utilityMainFeederMapping.Shop_ID);
                var Result = (from f in db.MM_Category
                              select new
                              {
                                  Category_Id = f.Category_Id,
                                  Category_Name = f.Category_Name + "(" + f.Production + ")",

                              }).ToList();
                //var userObj = from employee in db.MM_Employee
                //              where employee.Employee_ID == mm_user_roles.Employee_ID
                //              select new
                //              {
                //                  Employee_ID = employee.Employee_ID,
                //                  Employee_Name = employee.Employee_Name + "(" + employee.Employee_No + ")"
                //              };
                ViewBag.Category_ID = new SelectList(Result, "Category_Id", "Category_Name", utilityMainFeederMapping.Category_ID);
                //ViewBag.Category_ID = new SelectList(db.MM_Category.Where(s => s.Plant_ID == plantID), "Category_Id", "Category_Name", utilityMainFeederMapping.Category_ID);
                ViewBag.Parameter_ID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name", utilityMainFeederMapping.Parameter_ID);
                var unit = db.MM_Parameter.Where(s => s.Prameter_ID == utilityMainFeederMapping.Parameter_ID).Select(s => s.Unit).FirstOrDefault();
                ViewBag.Meter = new SelectList(db.MM_Meter, "MeterName", "MeterName",utilityMainFeederMapping.Meter);
                ViewBag.Feeder_ID = new SelectList(db.MM_Feeders.Where(s=>s.Shop_ID ==utilityMainFeederMapping.Shop_ID), "Feeder_Id", "FeederName",utilityMainFeederMapping.Feeder_ID);
                ViewBag.Meter_ID = new SelectList(db.MM_Meter, "MeterID", "MeterName",utilityMainFeederMapping.Meter_ID);
                ViewBag.Unit_ID = new SelectList(db.MM_Unit_Measurement.Where(s=>s.Display_unit == unit), "Unit_ID", "Data_Unit",utilityMainFeederMapping.Unit_ID);
                ViewBag.Area_ID = new SelectList(db.MM_Area.Where(s=>s.Shop_ID ==utilityMainFeederMapping.Shop_ID), "Area_ID", "Area_Name",utilityMainFeederMapping.Area_ID);
                ViewBag.Load_ID = new SelectList(db.MM_Load, "Load_ID", "Load_Name", utilityMainFeederMapping.Load_ID);
                globalData.pageTitle = ResourceMainFeederMapping.Title_Edit_MainFeeder;
                var Feeders = (from f in db.MM_Feeders
                               where f.Feeder_ID == utilityMainFeederMapping.Feeder_ID
                               select new { f.FeederDesc }).FirstOrDefault();
                ViewBag.FeederDesc = Feeders.FeederDesc;
                ViewBag.GlobalDataModel = globalData;
                return View(utilityMainFeederMapping);
            }
            catch
            {
                return RedirectToAction("Index");
            }
           
            
        }

        // POST: MainFeederMappings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( UtilityMainFeederMapping utilityMainFeederMapping)
        {
            try
            {
                string compName = ((FDSession)this.Session["FDSession"]).userHost;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (ModelState.IsValid)
                {
                    var feeder = utilityMainFeederMapping.Feeder_ID;
                    int shopId = Convert.ToInt32(utilityMainFeederMapping.Shop_ID);
                    int ParameterId = Convert.ToInt32(utilityMainFeederMapping.Parameter_ID);
                  
                    int TagIndex = Convert.ToInt32(utilityMainFeederMapping.TagIndex);
                    int plantId = Convert.ToInt32(utilityMainFeederMapping.Plant_ID);
                    int meterId = Convert.ToInt32(utilityMainFeederMapping.RowId);
                    if (utilityMainFeederMapping.IsTagIndexExists(TagIndex, plantID, meterId))
                    {
                        ModelState.AddModelError("TagIndex", ResourceValidation.Exist);
                    }
                    else if (utilityMainFeederMapping.IsFeederExists(Convert.ToInt32(feeder), plantID, shopId, ParameterId, meterId))
                    {
                        ModelState.AddModelError("FeederName", ResourceValidation.Exist);
                    }
                    else
                    {


                        utilityMainFeederMapping.Updated_Date = DateTime.Now;
                        utilityMainFeederMapping.Updated_Host = compName;
                        utilityMainFeederMapping.Updated_User_ID = userID;
                        //if (utilityMainFeederMapping.Active == null)
                        //{
                        //    utilityMainFeederMapping.Active = false;
                        //}
                        //else
                        //{
                        //    utilityMainFeederMapping.Active = true;
                        //}
                        if(utilityMainFeederMapping.Parameter_ID ==1)
                        {
                            utilityMainFeederMapping.ConsumptionType = true;
                        }
                        if (utilityMainFeederMapping.Meter == "Main Meter")
                        {
                            utilityMainFeederMapping.MeterType = true;
                        }
                        else
                        {
                            utilityMainFeederMapping.MeterType = false;
                        }
                        db.Entry(utilityMainFeederMapping).State = EntityState.Modified;
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageDetail = ResourceMainFeederMapping.MainFeederMapping_Success_MainFeederMapping_edit_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");

                    }

                }








                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name", utilityMainFeederMapping.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name", utilityMainFeederMapping.Shop_ID);
                ViewBag.Category_ID = new SelectList(db.MM_Category.Where(s => s.Plant_ID == plantID), "Category_Id", "Category_Name", utilityMainFeederMapping.Category_ID);
                ViewBag.Parameter_ID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name", utilityMainFeederMapping.Parameter_ID);
                ViewBag.Meter = new SelectList(db.MM_Meter, "MeterName", "MeterName", utilityMainFeederMapping.Meter);
                ViewBag.Meter_ID = new SelectList(db.MM_Meter, "MeterID", "MeterName", utilityMainFeederMapping.Meter_ID);
                ViewBag.Feeder_ID = new SelectList(db.MM_Feeders, "Feeder_ID", "FeederName", utilityMainFeederMapping.Feeder_ID);
                ViewBag.Area_ID = new SelectList(db.MM_Area, "Area_ID", "Area_Name", utilityMainFeederMapping.Area_ID);
                var unit = db.MM_Parameter.Where(s => s.Prameter_ID == utilityMainFeederMapping.Parameter_ID).Select(s => s.Unit).FirstOrDefault();
                ViewBag.Unit_ID = new SelectList(db.MM_Unit_Measurement.Where(s => s.Display_unit == unit), "Unit_ID", "Data_Unit", utilityMainFeederMapping.Unit_ID);
                return View(utilityMainFeederMapping);
            }
            catch 
            {
                return RedirectToAction("Index");
            }
           
        }

        // GET: MainFeederMappings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UtilityMainFeederMapping utilityMainFeederMapping = db.UtilityMainFeederMappings.Find(id);
            if (utilityMainFeederMapping == null)
            {
                return HttpNotFound();
            }
            return View(utilityMainFeederMapping);
        }

        // POST: MainFeederMappings/Delete/5
       
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                int Id = Convert.ToInt32(id);
                UtilityMainFeederMapping utilityMainFeederMapping = db.UtilityMainFeederMappings.Find(Id);
                db.UtilityMainFeederMappings.Remove(utilityMainFeederMapping);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceMainFeederMapping.MainFeeder_List;
                globalData.messageDetail = ResourceMainFeederMapping.MainFeederMapping_Success_MainFeederMapping_Delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                generalObj.addControllerException(ex, "MainFeederMappings", "delete", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMainFeederMapping.MainFeeder_List;
                globalData.messageDetail = ResourceMainFeederMapping.MainFeederMapping_Delete_Dependency_Failure;
                TempData["globalData"] = globalData;
                //ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Category.Plant_ID);
                return RedirectToAction("Index");

            }

        }
            
        public ActionResult displayfeederDetails(int shop_ID)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", plantID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(p => p.Plant_ID == plantID), "Shop_ID", "Shop_Name");
            //ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(p => p.Line_ID == 0), "Line_ID", "Line_Name");
            //ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(p => p.Shop_ID == 0), "Station_ID", "Station_Name");
            //ViewBag.Checklist_ID = new SelectList(db.UtilityMainFeederMappings.Where(p => p.Plant_ID == plantID && p.Shop_ID == shop_ID), "Checklist_ID", "Checklist_Name");

            //************************
            //nilesh
            db.Configuration.ProxyCreationEnabled = false;
            var res = from u in db.UtilityMainFeederMappings
                      join shop in db.MM_MTTUW_Shops on u.Shop_ID equals shop.Shop_ID
                      join feeder in db.MM_Feeders on  u.Feeder_ID equals feeder.Feeder_ID
                      join parameter in db.MM_Parameter on u.Parameter_ID equals parameter.Prameter_ID
                      where u.Plant_ID == plantID && u.Shop_ID == shop_ID && shop.Energy ==true
                      select new
                      {
                          shopName = shop.Shop_Name,
                          feederName = feeder.FeederName,
                          ParmeterName = parameter.Prameter_Name,
                          tagindex =u.TagIndex,
                          tagName =u.TagName,
                          MeterId = u.RowId
                          //Type= qualityCheckpoint.
                      };

            return Json(res, JsonRequestBehavior.AllowGet);

         
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
