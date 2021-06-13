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
    /* Class Name                 : AddCriticalStationController
    *  Description                : This class is used to add/save critical stations against particular station selected
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    public class AddCriticalStationController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0;


        /*	    Action Name		    : Index
        *		Description		    : To Display the users information in grid
        *		Author, Timestamp	: Ajay Wagh
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AddCriticalStation
        public ActionResult Index()
        {
            try
            {
                var RS_Route_Configurations = db.RS_Route_Configurations.Include(m => m. RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations).Include(m => m.RS_Stations1).Include(m => m.RS_Stations2);
                return View(RS_Route_Configurations.ToList());
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_Session;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }


        /*	    Action Name		    : Details
         *		Description		    : To show the Critical Station detailed information
         *		Author, Timestamp	: Ajay Wagh
         *		Input parameter	    : id  
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // GET: AddCriticalStation/Details/5
        public ActionResult Details(decimal id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RS_Route_Configurations RS_Route_Configurations = db.RS_Route_Configurations.Find(id);
                if (RS_Route_Configurations == null)
                {
                    return HttpNotFound();
                }
                return View(RS_Route_Configurations);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_Session;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }


        /*	    Action Name		    : Create
        *		Description		    : To read the critical station info which is to be saved
        *		Author, Timestamp	: Ajay Wagh
        *		Input parameter	    : 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AddCriticalStation/Create
        public ActionResult Create()
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                    ViewBag.Plant_ID = new SelectList("");
                    ViewBag.Shop_ID = new SelectList("");
                    ViewBag.Line_ID = new SelectList("");
                    ViewBag.Station_ID = new SelectList("");
                    ViewBag.myListBox1 = new SelectList("");
                    //plantId = Convert.ToInt32(TempData["plantId"].ToString());
                    //shopId = Convert.ToInt32(TempData["shopId"].ToString());
                    //lineId = Convert.ToInt32(TempData["lineId"].ToString());
                    //stationId = Convert.ToInt32(TempData["stationId"].ToString());
                }
                else
                {

                }
                globalData.pageTitle = ResourceModules.Critical_Station;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = ResourceModules.Critical_Station;
                globalData.actionName = ResourceGlobal.Index;
                globalData.contentTitle = ResourceModules.Critical_Station + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceModules.Critical_Station + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where((p => p.Line_ID == 0)), "Line_ID", "Line_Name");
                // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 0);
                decimal plant_Id = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_Id).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
                ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
                ViewBag.myListBox1 = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
                ViewBag.Next_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
                ViewBag.Prev_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
                return View();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_Session;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }

            }

        }



        /*	    Action Name		    : Create
        *		Description		    : To save the critical station information
        *		Author, Timestamp	: Ajay Wagh
        *		Input parameter	    : RS_Route_Configurations object 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // POST: AddCriticalStation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Route_Configuration_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Prev_Station_ID,Next_Station_ID,Is_Start_Station,Is_End_Station,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,myListBox1,selectedStation")] RS_Route_Configurations RS_Route_Configurations)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    plantId = 1;//Convert.ToInt32(RS_Route_Configurations.Plant_ID);
                    shopId = Convert.ToInt32(RS_Route_Configurations.Shop_ID);
                    lineId = Convert.ToInt32(RS_Route_Configurations.Line_ID);

                    stationId = Convert.ToInt16(RS_Route_Configurations.Station_ID);

                    try
                    {
                        var stationItem = from station in db.RS_Stations
                                          where (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                                          select station;

                        RS_Stations stationObj = new RS_Stations();
                        foreach (var item in stationItem.ToList())
                        {
                            stationObj = db.RS_Stations.Find(item.Station_ID);
                            stationObj.Is_Critical_Station = false;
                            stationObj.Is_Edited = true;
                            db.Entry(stationObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex1)
                    {

                    }


                    for (int i = 0; i < RS_Route_Configurations.myListBox1.Count(); i++)
                    {
                        try
                        {
                            int selectedStationId = Convert.ToInt16(RS_Route_Configurations.myListBox1[i]);

                            if (selectedStationId == 0)
                                continue;

                            RS_Stations stationObj = new RS_Stations();
                            stationObj = db.RS_Stations.Find(RS_Route_Configurations.myListBox1[i]);
                            stationObj.Is_Critical_Station = true;
                            stationObj.Is_Edited = true;
                            db.Entry(stationObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            
                        }

                        //if (db.RS_Stations.Where(x => x.Station_ID.ToString() == RS_Route_Configurations.myListBox1[i].ToString()).Count()>0)
                        //{
                        //    db.RS_Stations.Where(x => x.Station_ID.ToString() == RS_Route_Configurations.myListBox1[i].ToString()).FirstOrDefault().Is_Critical_Station = true;
                        //    db.SaveChanges();
                        //}
                        //criticalid=db.RS_Stations.Is_Critical_Station

                    }
                    // db.RS_Route_Configurations.Add(RS_Route_Configurations);
                    //db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Critical_Station;
                    globalData.messageDetail = ResourceModules.Critical_Station + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    //return View();
                    // return RedirectToAction("Index");
                }

                globalData.pageTitle = ResourceModules.Critical_Station;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "CtiticalStations";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Critical_Station + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Critical_Station + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Updated_User_ID);
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where((p => p.Line_ID == 0)), "Line_ID", "Line_Name", RS_Route_Configurations.Line_ID);
                // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 0);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name");
                ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Station_ID == 0), "Station_ID", "Station_Name", RS_Route_Configurations.Station_ID);
                ViewBag.Next_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Next_Station_ID);
                ViewBag.Prev_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Prev_Station_ID);
                //return View();
                return RedirectToAction("Create");
                //return View(RS_Route_Configurations);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_Session;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Create");
                }
            }

        }


        /*	    Action Name		    : Edit
        *		Description		    : To read the critical station information which is to be edited
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AddCriticalStation/Edit/5
        public ActionResult Edit(decimal id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RS_Route_Configurations RS_Route_Configurations = db.RS_Route_Configurations.Find(id);
                if (RS_Route_Configurations == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Updated_User_ID);
                ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Configurations.Line_ID);
                //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Configurations.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Configurations.Shop_ID);
                ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Station_ID);
                ViewBag.Next_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Next_Station_ID);
                ViewBag.Prev_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Prev_Station_ID);
                return View(RS_Route_Configurations);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_Session;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }


        /*	    Action Name		    : Edit
       *		Description		    : To edit the critical station information
       *		Author, Timestamp	: Jitendra Mahajan
       *		Input parameter	    : RS_Route_Configurations object 
       *		Return Type		    : ActionResult
       *		Revision		    :
       */
        // POST: AddCriticalStation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Route_Configuration_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Prev_Station_ID,Next_Station_ID,Is_Start_Station,Is_End_Station,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Route_Configurations RS_Route_Configurations)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RS_Route_Configurations.Is_Edited = true;
                    db.Entry(RS_Route_Configurations).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Updated_User_ID);
                ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Configurations.Line_ID);
                // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Configurations.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Configurations.Shop_ID);
                ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Station_ID);
                ViewBag.Next_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Next_Station_ID);
                ViewBag.Prev_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Prev_Station_ID);
                return View(RS_Route_Configurations);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_Session;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }


        /*	    Action Name		    : Delete
        *		Description		    : To Display the users information which is to be deleted
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id to be deleted row  
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AddCriticalStation/Delete/5
        public ActionResult Delete(decimal id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RS_Route_Configurations RS_Route_Configurations = db.RS_Route_Configurations.Find(id);
                if (RS_Route_Configurations == null)
                {
                    return HttpNotFound();
                }
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                return View(RS_Route_Configurations);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_Session;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }


        /*	    Action Name		    : DeleteConfirmed
        *		Description		    : To delete the users record
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id of user whose record is to be deleted
        *		Return Type		    : redirected to the index page
        *		Revision		    :
        */
        // POST: AddCriticalStation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_Route_Configurations RS_Route_Configurations = db.RS_Route_Configurations.Find(id);
                db.RS_Route_Configurations.Remove(RS_Route_Configurations);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_Session;
                    globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Delete", id);
                }
            }

        }


        /*	    Action Name		    : Dispose
        *		Description		    : To clear the memory allocated by objects
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : disposing bool value 
        *		Return Type		    :
        *		Revision		    :
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult SaveCriticalStations(string stations, int lineId)
        {
            try
            {
                
                if (stations != null && stations != "")
                {

                    RS_Stations stationObj = new RS_Stations();
                    var stationItem = from station in db.RS_Stations
                                      where (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                                      select station;

                    foreach (var item in stationItem.ToList())
                    {
                        stationObj = db.RS_Stations.Find(item.Station_ID);
                        stationObj.Is_Edited = true;
                        stationObj.Is_Critical_Station = false;
                        db.Entry(stationObj).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    string[] words;
                    words = stations.Split(',');

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
                        stationObj = db.RS_Stations.Find(i);
                        stationObj.Is_Edited = true;
                        stationObj.Is_Critical_Station = true;
                        db.Entry(stationObj).State = EntityState.Modified;
                        db.SaveChanges();
                        i = 0;
                    }

                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
               
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
    }
}
