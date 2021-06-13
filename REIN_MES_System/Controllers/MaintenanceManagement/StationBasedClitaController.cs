//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using ZHB_AD.Models;
//using ZHB_AD.Controllers.BaseManagement;
//using ZHB_AD.App_LocalResources;
//using ZHB_AD.Helper;
//using System.IO;

//namespace ZHB_AD.Controllers.MaintenanceManagement
//{
//    public class StationBasedClitaController : BaseController
//    {
//        private MVML_MGMTEntities db = new MVML_MGMTEntities();
//        private GlobalData globalData = new GlobalData();
//        MM_Station_Based_Clita stationclita = new MM_Station_Based_Clita();

//        General generalObj = new General();

//        // GET: StationBasedClita
//        public ActionResult Index()
//        {
//            if (TempData["globalData"] != null)
//            {
//                globalData = (GlobalData)TempData["globalData"];
//            }
//            globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//            globalData.subTitle = ResourceGlobal.Lists;
//            globalData.controllerName = "StationBasedClita";
//            globalData.actionName = ResourceGlobal.Lists;
//            globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Clita_Item_Lists;
//            globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Clita_Item_Lists;


//            ViewBag.GlobalDataModel = globalData;

//            return View(db.MM_Station_Based_Clita.ToList());
//        }

//        // GET: StationBasedClita/Details/5
//        public ActionResult Details(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_Station_Based_Clita station_Based_Clita = db.MM_Station_Based_Clita.Find(id);
//            if (station_Based_Clita == null)
//            {
//                return HttpNotFound();
//            }
//            globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//            globalData.subTitle = ResourceGlobal.Details;
//            globalData.controllerName = "Station Clita Item";
//            globalData.actionName = ResourceGlobal.Details;
//            globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Clita_Item_Detail;
//            globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Clita_Item_Detail;


//            ViewBag.GlobalDataModel = globalData;
//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
//            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number");
//            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
//            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification");
//            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard");
//            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method");
//            ViewBag.GlobalDataModel = globalData;
//            return View(station_Based_Clita);
//        }

//        // GET: StationBasedClita/Create
//        public ActionResult Create()
//        {
//            globalData.stationIPAddress = GlobalOperations.GetIP();
//            globalData.hostName = GlobalOperations.GetHostName();
//            globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//            globalData.subTitle = ResourceGlobal.Create;
//            globalData.controllerName = "StationBasedClita";
//            globalData.actionName = ResourceGlobal.Create;
//            globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;
//            globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;
//            //   TempData["globalData"] = globalData;
//            ViewBag.GlobalDataModel = globalData;
//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
//            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
//            //  ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines, "Machine_ID", "Machine_Number");
//            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
//            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification");
//            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard");
//            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method");
//            return View();
//        }

//        // POST: StationBasedClita/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "Station_Clita_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Clita_Item,Clita_Classification_ID,Clita_Standard_ID,Clita_Tool_ID,Clita_Method_ID,Cycle,Maintenance_Time_TACT,Recipent_Email,Start_Date,End_Date,Maintenance_User_ID,Last_Maintenance_Date,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,users,mails")] MM_Station_Based_Clita station_Based_Clita)
//        {

//            station_Based_Clita.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//            string mails = "";
//            foreach (var item in station_Based_Clita.mails)
//            {
//                if (db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).Count() > 0)
//                {
//                    mails += db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).First().Email_Address + ";";
//                }
//                else
//                {
//                    mails += item.ToString() + "; ";
//                    //add email address which are manually entered here
//                    //not present in database
//                }
//                // mails += db.MM_Employee.Where(x => x.Employee_ID.ToString() == item.ToString()).First().Email_Address + ";";
//            }

//            station_Based_Clita.Recipent_Email = mails;
//            station_Based_Clita.End_Date = station_Based_Clita.Start_Date;

//            if (ModelState.IsValid)
//            {
//                if (db.MM_Station_Based_Clita.Where(x => x.Station_ID == station_Based_Clita.Station_ID && x.Clita_Item.ToLower() == station_Based_Clita.Clita_Item.Trim().ToLower() && x.Clita_Tool_ID == station_Based_Clita.Clita_Tool_ID && x.Clita_Method_ID == station_Based_Clita.Clita_Method_ID && x.Clita_Standard_ID == station_Based_Clita.Clita_Standard_ID && x.Clita_Classification_ID == station_Based_Clita.Clita_Classification_ID).Count() > 0)
//                {
//                    ModelState.AddModelError("Clita_Item", "Station with Specified clita item is already exists.");
//                    globalData.stationIPAddress = GlobalOperations.GetIP();
//                    globalData.hostName = GlobalOperations.GetHostName();
//                    globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//                    globalData.subTitle = ResourceGlobal.Create;
//                    globalData.controllerName = "StationBasedClita";
//                    globalData.actionName = ResourceGlobal.Create;
//                    globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;
//                    globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;
//                    ViewBag.GlobalDataModel = globalData;
//                    TempData["globalData"] = globalData;
//                }
//                else
//                {
//                    station_Based_Clita.Inserted_Date = DateTime.Now;
//                    station_Based_Clita.Inserted_Host = Dns.GetHostName();
//                    station_Based_Clita.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

//                    db.MM_Station_Based_Clita.Add(station_Based_Clita);

//                    db.SaveChanges();


//                    foreach (var item in station_Based_Clita.users)
//                    {
//                        MM_Station_Clita_Users clitauser = new MM_Station_Clita_Users();
//                        clitauser.Station_ID = station_Based_Clita.Station_ID;
//                        clitauser.User_ID = item;
//                        clitauser.Station_Clita_ID = station_Based_Clita.Station_Clita_ID;
//                        db.MM_Station_Clita_Users.Add(clitauser);
//                        db.SaveChanges();
//                    }
//                    globalData.stationIPAddress = GlobalOperations.GetIP();
//                    globalData.hostName = GlobalOperations.GetHostName();
//                    globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//                    globalData.subTitle = ResourceGlobal.Create;
//                    globalData.controllerName = "StationBasedClita";
//                    globalData.actionName = ResourceGlobal.Create;
//                    globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;
//                    globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;
//                    globalData.isSuccessMessage = true;
//                    globalData.messageTitle = ResourceStationClita.Station_Clita_Item;
//                    globalData.messageDetail = ResourceStationClita.Clita_Success_Clita_Item_Add_Success;
//                    ViewBag.GlobalDataModel = globalData;
//                    TempData["globalData"] = globalData;

//                    ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//                    ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//                    ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//                    ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//                    ViewBag.Clita_Tool_ID = new SelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name");
//                    ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//                    ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
//                    ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
//                    ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
//                    ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number");
//                    ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
//                    ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification");
//                    ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard");
//                    ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method");
//                    return RedirectToAction("Index");
//                }
//            }
//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
//            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number");
//            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
//            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification");
//            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard");
//            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method");
//            return View(station_Based_Clita);
//        }

//        // GET: StationBasedClita/Edit/5
//        public ActionResult Edit(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_Station_Based_Clita station_Based_Clita = db.MM_Station_Based_Clita.Find(id);
//            if (station_Based_Clita == null)
//            {
//                return HttpNotFound();
//            }

//            List<string> emails = station_Based_Clita.Recipent_Email.Split(';').ToList();
//            List<decimal> lst_Selected_EmailsID = new List<decimal>();
//            List<decimal> lst_usersids = new List<decimal>();

//            var lstusr = db.MM_Station_Clita_Users.Where(x => x.Station_Clita_ID == id).ToList();
//            foreach (var usr in lstusr)
//            {
//                lst_usersids.Add(usr.User_ID);
//            }
//            foreach (string email in emails)
//            {
//                if (email != "" && email != " " && email != null)
//                {
//                    if (db.MM_Employee.Where(x => x.Email_Address.ToLower().Trim() == email.Trim()).Count() > 0)
//                    {
//                        decimal j = db.MM_Employee.Where(x => x.Email_Address.ToLower().Trim() == email.Trim()).FirstOrDefault().Employee_ID;
//                        lst_Selected_EmailsID.Add(j);
//                    }
//                }
//            }
//            globalData.stationIPAddress = GlobalOperations.GetIP();
//            globalData.hostName = GlobalOperations.GetHostName();
//            globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//            globalData.subTitle = ResourceGlobal.Edit;
//            globalData.controllerName = "StationBasedClita";
//            globalData.actionName = ResourceGlobal.Edit;
//            globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Edit_Clita_Item;
//            globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Edit_Clita_Item;
//            ViewBag.GlobalDataModel = globalData;
//            ViewBag.mails = new MultiSelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address", lst_Selected_EmailsID);
//            ViewBag.users = new MultiSelectList(db.MM_Employee, "Employee_ID", "Employee_Name", lst_usersids);
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", station_Based_Clita.Maintenance_User_ID);
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", station_Based_Clita.Inserted_User_ID);
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", station_Based_Clita.Updated_User_ID);
//            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", station_Based_Clita.Line_ID);
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", station_Based_Clita.Plant_ID);
//            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", station_Based_Clita.Station_ID);
//            // ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines, "Machine_ID", "Machine_Number", mM_MT_Clita.Machine_ID);
//            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", station_Based_Clita.Shop_ID);
//            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification", station_Based_Clita.Clita_Classification_ID);
//            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard", station_Based_Clita.Clita_Standard_ID);
//            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method", station_Based_Clita.Clita_Method_ID);
//            return View(station_Based_Clita);
//        }

//        // POST: StationBasedClita/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "Station_Clita_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Clita_Item,Clita_Classification_ID,Clita_Standard_ID,Clita_Tool_ID,Clita_Method_ID,Cycle,Maintenance_Time_TACT,Recipent_Email,Start_Date,End_Date,Maintenance_User_ID,Last_Maintenance_Date,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,mails,users")] MM_Station_Based_Clita station_Based_Clita)
//        {

//            station_Based_Clita.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//            station_Based_Clita.End_Date = station_Based_Clita.Start_Date;
//            stationclita = db.MM_Station_Based_Clita.FirstOrDefault(x => x.Station_Clita_ID == station_Based_Clita.Station_Clita_ID);
//            if (ModelState.IsValid)
//            {
//                string strmails = "";
//                foreach (var item in station_Based_Clita.mails)
//                {
//                    if (db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).Count() > 0)
//                    {
//                        strmails += db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).First().Email_Address + ";";
//                    }
//                    else
//                    {
//                        strmails += item.ToString() + "; ";
//                        //add email address which are manually entered here
//                        //not present in database
//                    }
//                    //    decimal j = Convert.ToDecimal(item);
//                    //    strmails += db.MM_Employee.Where(x => x.Employee_ID == j).FirstOrDefault().Email_Address+"; "; 
//                }
//                station_Based_Clita.Recipent_Email = strmails;
//                stationclita.Maintenance_Time_TACT = station_Based_Clita.Maintenance_Time_TACT;
//                stationclita.Clita_Method_ID = station_Based_Clita.Clita_Method_ID;
//                stationclita.Clita_Item = station_Based_Clita.Clita_Item;
//                stationclita.Clita_Tool_ID = station_Based_Clita.Clita_Tool_ID;
//                stationclita.Clita_Classification_ID = station_Based_Clita.Clita_Classification_ID;
//                stationclita.Cycle = station_Based_Clita.Cycle;
//                stationclita.Recipent_Email = station_Based_Clita.Recipent_Email;
//                stationclita.Start_Date = station_Based_Clita.Start_Date;
//                stationclita.End_Date = station_Based_Clita.End_Date.AddMonths((int)station_Based_Clita.Cycle);
//                stationclita.Clita_Standard_ID = station_Based_Clita.Clita_Standard_ID;
//                stationclita.Updated_Date = DateTime.Now;
//                stationclita.Updated_Host = "192.168.1.123";
//                stationclita.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;


//                db.Entry(stationclita).State = EntityState.Modified;
//                db.SaveChanges();

//                var record = db.MM_Station_Clita_Users.Where(x => x.Station_Clita_ID == station_Based_Clita.Station_Clita_ID).ToList();
//                db.MM_Station_Clita_Users.RemoveRange(record);


//                foreach (var item in station_Based_Clita.users)
//                {
//                    MM_Station_Clita_Users clitauser = new MM_Station_Clita_Users();
//                    clitauser.Station_ID = stationclita.Station_ID;
//                    clitauser.User_ID = item;
//                    clitauser.Station_Clita_ID = stationclita.Station_Clita_ID;
//                    db.MM_Station_Clita_Users.Add(clitauser);
//                    db.SaveChanges();
//                }

//                globalData.stationIPAddress = GlobalOperations.GetIP();
//                globalData.hostName = GlobalOperations.GetHostName();
//                globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//                globalData.subTitle = ResourceGlobal.Edit;
//                globalData.controllerName = "StationBasedClita";
//                globalData.actionName = ResourceGlobal.Edit;
//                globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Edit_Clita_Item;
//                globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Edit_Clita_Item;
//                globalData.isSuccessMessage = true;
//                globalData.messageTitle = ResourceStationClita.Station_Clita_Item;
//                globalData.messageDetail = ResourceStationClita.Clita_Success_Clita_Item_Edit_Success;
//                ViewBag.GlobalDataModel = globalData;
//                TempData["globalData"] = globalData;
//                ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//                ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//                ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", station_Based_Clita.Maintenance_User_ID);
//                ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", station_Based_Clita.Inserted_User_ID);
//                ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", station_Based_Clita.Updated_User_ID);
//                ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", station_Based_Clita.Line_ID);
//                ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", station_Based_Clita.Plant_ID);
//                ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", station_Based_Clita.Station_ID);
//                //   ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines, "Machine_ID", "Machine_Number", mM_MT_Clita.Machine_ID);
//                ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", station_Based_Clita.Shop_ID);
//                ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification", station_Based_Clita.Clita_Classification_ID);
//                ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard", station_Based_Clita.Clita_Standard_ID);
//                ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method", station_Based_Clita.Clita_Method_ID);
//                return RedirectToAction("Index");
//            }
//            return View(station_Based_Clita);
//        }

//        // GET: StationBasedClita/Delete/5
//        public ActionResult Delete(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_Station_Based_Clita station_Based_Clita = db.MM_Station_Based_Clita.Find(id);
//            if (station_Based_Clita == null)
//            {
//                return HttpNotFound();
//            }


//            globalData.stationIPAddress = GlobalOperations.GetIP();
//            globalData.hostName = GlobalOperations.GetHostName();
//            globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//            globalData.subTitle = ResourceGlobal.Delete;
//            globalData.controllerName = "StationBasedClita";
//            globalData.actionName = ResourceGlobal.Delete;
//            globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Delete_Clita_Item;
//            globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Delete_Clita_Item;

//            ViewBag.GlobalDataModel = globalData;
//            TempData["globalData"] = globalData;
//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
//            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number");
//            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
//            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification");
//            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard");
//            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method");
//            return View(station_Based_Clita);
//        }

//        // POST: StationBasedClita/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(decimal id)
//        {
//            MM_Station_Based_Clita station_Based_Clita = db.MM_Station_Based_Clita.Find(id);
//            db.MM_Station_Based_Clita.Remove(station_Based_Clita);
//            db.SaveChanges();

//            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_Station_Based_Clita", "Station_Clita_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

//            globalData.stationIPAddress = GlobalOperations.GetIP();
//            globalData.hostName = GlobalOperations.GetHostName();
//            globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//            globalData.subTitle = ResourceGlobal.Delete;
//            globalData.controllerName = "StationBasedClita";
//            globalData.actionName = ResourceGlobal.Delete;
//            globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Delete_Clita_Item;
//            globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Delete_Clita_Item;

//            globalData.isSuccessMessage = true;
//            globalData.messageTitle = ResourceClitaItem.Clita_Item;
//            globalData.messageDetail = ResourceClitaItem.Clita_Success_Clita_Item_Delete_Success;
//            ViewBag.GlobalDataModel = globalData;
//            TempData["globalData"] = globalData;
//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
//            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number");
//            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
//            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification");
//            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard");
//            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method");
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        public ActionResult Confirm()
//        {

//            if (TempData["globalData"] != null)
//            {
//                globalData = (GlobalData)TempData["globalData"];
//            }
//            int station = ((FDSession)this.Session["FDSession"]).stationId;
//            DateTime scheduleObj = DateTime.Now;
//            var lst_Station_Clita_Log = (from t in db.MM_Station_Based_Clita_Log.AsEnumerable()
//                                         where t.Station_ID == station && (t.End_Date.Year == scheduleObj.Date.Year && t.End_Date.Month == scheduleObj.Date.Month && t.End_Date.Day == scheduleObj.Date.Day)
//                                         select t);



//            return View(lst_Station_Clita_Log);
//        }
//        public ActionResult SaveRemark(decimal id)
//        {
//            // ViewBag.id = id;
//            var obj = db.MM_Station_Based_Clita_Log.Find(id);
//            return PartialView("PartialConfirm", obj);
//        }
//        public ActionResult Complete(FormCollection fc)
//        {

//            string id = fc["Station_Clita_Log_ID"].ToString().Trim();
//            MM_Station_Based_Clita_Log stationBasdclita = new MM_Station_Based_Clita_Log();
//            stationBasdclita = db.MM_Station_Based_Clita_Log.Where(x => x.Station_Clita_Log_ID.ToString() == id).FirstOrDefault();
//            if (ModelState.IsValid)
//            {

//                MM_Station_Based_Clita_Log s_Clita_Log = new MM_Station_Based_Clita_Log();
//                s_Clita_Log = db.MM_Station_Based_Clita_Log.Where(x => x.Station_Clita_Log_ID.ToString() == id).FirstOrDefault();
//                s_Clita_Log.Is_Maintenance_Done = true;
//                s_Clita_Log.Remark = fc["Remark"].ToString();
//                s_Clita_Log.Special_Observation = fc["Special_Observation"].ToString();
//                s_Clita_Log.End_Date = s_Clita_Log.End_Date.AddDays(Convert.ToDouble(s_Clita_Log.Cycle));
//                db.Entry(s_Clita_Log).State = EntityState.Modified;
//                db.SaveChanges();

//                globalData.pageTitle = ResourceClitaItem.Clita_Item;
//                globalData.subTitle = ResourceGlobal.Lists;
//                globalData.controllerName = "Confirm Station Clita Item Maintenance";
//                globalData.actionName = ResourceGlobal.Lists;
//                globalData.contentTitle = ResourceClitaItem.Clita_Item_Title_Clita_Item_Detail;
//                globalData.contentFooter = ResourceClitaItem.Clita_Item_Title_Clita_Item_Detail;

//                globalData.isSuccessMessage = true;
//                globalData.messageTitle = ResourceClitaItem.Clita_Item;
//                globalData.messageDetail = ResourceClitaItem.Clita_Item;
//                TempData["globalData"] = globalData;
//                ViewBag.GlobalDataModel = globalData;
//                return RedirectToAction("Confirm");
//            }

//            return View(stationclita);
//        }

//        #region file uploading
//        public ActionResult Upload()
//        {

//            if (TempData["SClitaRecords"] != null)
//            {
//                ViewBag.sClitaRecords = TempData["SClitaRecords"];
//                ViewBag.sClitaDataTable = TempData["SClitaDataTable"];
//            }

//            ViewBag.GlobalDataModel = globalData;
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
//            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
//            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name");
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.Where(x => x.Is_Status_Machine == true).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name");
//            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification");
//            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard");
//            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//            globalData.subTitle = ResourceGlobal.Upload;
//            globalData.controllerName = "StationBasedClita";
//            globalData.actionName = ResourceGlobal.Upload;
//            globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;
//            globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;

//            TempData["globalData"] = globalData;
//            ViewBag.GlobalDataModel = globalData;
//            return View();
//            //return View();
//        }


//        [HttpPost]
//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Upload(HttpPostedFileBase files, [Bind(Include = "Station_Clita_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Clita_Item,Classification,Standards,Clita_Tool_ID,Method,Cycle,Maintenance_Time_TACT,Recipent_Email,Start_Date,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Maintenance_User_ID,users,mails,End_Date")] MM_Station_Based_Clita mM_MT_Clita)
//        {
//            try
//            {
              
//                GlobalOperations globalOperations = new GlobalOperations();
//                string fileName = Path.GetFileName(files.FileName);
//                string fileExtension = Path.GetExtension(files.FileName);
//                string fileLocation = Server.MapPath("~/App_Data/" + fileName);
//                DataTable dt = globalOperations.ExcelToDataTable(files, fileLocation, fileExtension);
//                int J = 0;
//                ExcelMTSClitaRecords[] sClitaRecords = new ExcelMTSClitaRecords[dt.Rows.Count];
//                if (dt.Rows.Count > 0)
//                {

//                    foreach (DataRow drStationClita in dt.Rows)
//                    {
//                        MM_Station_Based_Clita mM_station_Based_clita = new MM_Station_Based_Clita();
                        
//                        ExcelMTSClitaRecords sclitamsg = new ExcelMTSClitaRecords();
                       
//                        #region Machine Clita
//                        string shop = drStationClita["Shop_Name"].ToString();
//                        string line = drStationClita["Line_Name"].ToString();
//                        string stationname = drStationClita["Station_Name"].ToString();
//                        string clitaitem = drStationClita["Clita_Item"].ToString();
//                        string Clita_tool = drStationClita["Tool_Name"].ToString();
//                        string Clita_Method = drStationClita["Method"].ToString();
//                        string ClitaStandard = drStationClita["Standard"].ToString();
//                        string startDate = drStationClita["Start_Date"].ToString();
//                        string Frequency = drStationClita["Frequency"].ToString();
//                        string Classification = drStationClita["Classification"].ToString();
//                        string TactTime = drStationClita["Tact_Time"].ToString();
//                        string[] users = drStationClita["User"].ToString().Split(';').ToArray();
//                        int[] user = new int[users.Count()];
//                        string email = "";
//                        for (int i = 0; i < users.Count(); i++)
//                        {
//                            string usr = users[i].ToString();
//                            decimal id = db.MM_Employee.Where(x => x.Employee_No.ToString().ToLower() == usr.ToLower().Trim()).FirstOrDefault().Employee_ID;
//                            user[i] = Convert.ToInt32(id);
//                            email += db.MM_Employee.Where(x => x.Employee_No.ToString().ToLower() == usr.ToLower().Trim()).FirstOrDefault().Email_Address + ";";
//                        }
//                        decimal Classification_ID = db.MM_MT_Clita_Classification.Where(x => x.Classification.ToLower() == Classification.ToLower().Trim()).FirstOrDefault().Clita_Classification_ID;
//                        decimal shop_ID = db.MM_Shops.Where(x => x.Shop_Name.ToLower() == shop.ToLower().Trim()).FirstOrDefault().Shop_ID;
//                        decimal Station_ID = db.MM_Stations.Where(x => x.Station_Name.ToLower() == stationname.ToLower().Trim()).FirstOrDefault().Station_ID;
//                        decimal Line_ID = db.MM_Lines.Where(x => x.Line_Name.ToLower() == line.ToLower().Trim()).FirstOrDefault().Line_ID;
//                        decimal Tool_ID = db.MM_MT_Clita_Tool.Where(x => x.Tool_Name.ToLower() == Clita_tool.ToLower().Trim()).FirstOrDefault().Clita_Tool_ID;
//                        decimal Method_ID = db.MM_MT_Clita_Method.Where(x => x.Method.ToLower() == Clita_Method.ToLower().Trim()).FirstOrDefault().Clita_Method_ID;
//                        decimal standard_ID = db.MM_MT_Clita_Standard.Where(x => x.Standard.ToLower() == ClitaStandard.ToLower().Trim()).FirstOrDefault().Clita_Standard_ID;
//                        sclitamsg.Clita_Method = Clita_Method;
//                        sclitamsg.Clita_standard = ClitaStandard;
//                        sclitamsg.Clita_item = clitaitem;
//                        sclitamsg.Station_Name = stationname;
//                        sclitamsg.Classification = Classification;
//                        sclitamsg.Clita_Tool = Clita_tool;
//                        if((standard_ID==null && standard_ID==0) && (clitaitem==null && clitaitem==""))
//                        {
//                            sclitamsg.SClita_Error = "Value Can not be null aleardy Exists";
//                        }
//                        if(db.MM_Station_Based_Clita.Where(x=>x.Station_ID==Station_ID && x.Clita_Item.ToLower()==clitaitem.ToLower().Trim() && x.Clita_Standard_ID==standard_ID && x.Clita_Method_ID==Method_ID && x.Clita_Tool_ID==Tool_ID).Count()>0)
//                        {
//                            sclitamsg.SClita_Error = "Clita Item aleardy Exists";
//                        }
//                        else
//                        {
//                            mM_station_Based_clita.Station_ID = Station_ID;
//                            mM_station_Based_clita.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//                            mM_station_Based_clita.Recipent_Email = email;
//                            mM_station_Based_clita.Shop_ID = shop_ID;
//                            mM_station_Based_clita.Line_ID = Line_ID;
//                            mM_station_Based_clita.Clita_Item = clitaitem;
//                            mM_station_Based_clita.Clita_Method_ID = Method_ID;
//                            mM_station_Based_clita.Clita_Standard_ID = standard_ID;
//                            mM_station_Based_clita.Clita_Tool_ID = Tool_ID;
//                            mM_station_Based_clita.End_Date =Convert.ToDateTime(startDate);
//                            mM_station_Based_clita.Start_Date =Convert.ToDateTime(startDate);
//                            mM_station_Based_clita.Cycle = Convert.ToDecimal(Frequency);
//                            mM_station_Based_clita.Clita_Classification_ID = Classification_ID;
//                            mM_station_Based_clita.Maintenance_Time_TACT = Convert.ToDateTime(TactTime).TimeOfDay;
//                            mM_station_Based_clita.Inserted_Date = DateTime.Now;
//                            mM_station_Based_clita.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
//                            mM_station_Based_clita.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).plantId;
//                            db.MM_Station_Based_Clita.Add(mM_station_Based_clita);
//                            db.SaveChanges();

//                            for (int i = 0; i < users.Count(); i++)
//                            {
//                                if (users[i].ToString() != null && users[i] != "")
//                                {
//                                    string usr = users[i].ToString();
//                                    decimal id = db.MM_Employee.Where(x => x.Employee_No.ToString().ToLower() == usr.ToLower().Trim()).FirstOrDefault().Employee_ID;
//                                    MM_Station_Clita_Users StationClitauser = new MM_Station_Clita_Users();
//                                    StationClitauser.Station_Clita_ID = mM_station_Based_clita.Station_Clita_ID;
//                                    StationClitauser.Station_ID = Station_ID;
//                                    StationClitauser.User_ID = id;
//                                    db.MM_Station_Clita_Users.Add(StationClitauser);
//                                    db.SaveChanges();
//                                }
//                            }
//                            sclitamsg.SClita_Error = "Clita Item added Sucessfully";

//                            globalData.isSuccessMessage = true;
//                            globalData.messageTitle = ResourceClitaItem.Clita_Item_Title_Add_Clita_Item;
//                            globalData.messageDetail = ResourceClitaItem.Clita_Success_Clita_Item_Add_Success;

//                            TempData["globalData"] = globalData;
                           
//                            ViewBag.GlobalDataModel = globalData;

//                        }
//                        sClitaRecords[J] = sclitamsg;
//                        J = J + 1;
//                        #endregion
//                    }
//                    TempData["SClitaRecords"] = sClitaRecords;
//                    TempData["SClitaDataTable"] = dt;
//                    ViewBag.sClitaRecords = sClitaRecords;
//                    ViewBag.sClitaDataTable = dt;
//                    ViewBag.GlobalDataModel = globalData;
//                }
//            }
//            catch (Exception ex)
//            {
//                globalData.isErrorMessage = true;
//                globalData.messageTitle = ResourceClitaItem.Clita_Item_Title_Add_Clita_Item;
//                globalData.messageDetail = ex.Message.ToString();
//                TempData["globalData"] = globalData;
//                ViewBag.GlobalDataModel = globalData;
//            }
//            globalData.pageTitle = ResourceStationClita.Station_Clita_Item;
//            globalData.subTitle = ResourceGlobal.Upload;
//            globalData.controllerName = "StationBasedClita";
//            globalData.actionName = ResourceGlobal.Upload;
//            globalData.contentTitle = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;
//            globalData.contentFooter = ResourceStationClita.Clita_Item_Title_Add_Clita_Item;
//            TempData["globalData"] = globalData;
         
//            ViewBag.GlobalDataModel = globalData;
          
//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Maintenance_User_ID);
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Inserted_User_ID);
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Updated_User_ID);
//            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_MT_Clita.Line_ID);
//            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name", mM_MT_Clita.Clita_Tool_ID);
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Clita.Plant_ID);
//            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_MT_Clita.Station_ID);
//            // ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines, "Machine_ID", "Machine_Number", mM_MT_Clita.Machine_ID);
//            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_MT_Clita.Shop_ID);
//            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification, "Clita_Classification_ID", "Classification", mM_MT_Clita.Clita_Classification_ID);
//            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard, "Clita_Standard_ID", "Standard", mM_MT_Clita.Clita_Standard_ID);
//            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method", mM_MT_Clita.Clita_Method_ID);
//            return View();
//        }

//        public ActionResult GetStationsByLineID(decimal lineid)
//        {
//            var stationslist = db.MM_Stations.Where(c => c.Line_ID == lineid).Select(a => new { a.Station_ID, a.Station_Name });
//            return Json(stationslist, JsonRequestBehavior.AllowGet);
//        }

//        private void InsertIntoDataTable(DataTable dt, MM_Station_Based_Clita mM_MT_Clita)
//        {
//            try
//            {

//                foreach (DataRow dr in dt.Rows)
//                {
//                    string clitaitem = dr["Clita_Item"].ToString().Trim();
//                    //write a logic to sav clita items fr0m excel 

//                    if (db.MM_Station_Based_Clita.Where(x => x.Station_ID == mM_MT_Clita.Station_ID && x.Clita_Item.ToString() == clitaitem).Count() > 0)
//                    {

//                    }
//                    else
//                    {
//                        mM_MT_Clita.Clita_Item = Convert.ToString(dr["Clita_Item"]).ToString();
//                        mM_MT_Clita.Clita_Classification_ID = Convert.ToDecimal(Convert.ToString(dr["Classification"]).ToString());
//                        mM_MT_Clita.Cycle = Convert.ToDecimal(dr["Cycle"].ToString());
//                        mM_MT_Clita.End_Date = Convert.ToDateTime(dr["End_Date"].ToString());
//                        mM_MT_Clita.Clita_Standard_ID = Convert.ToDecimal(Convert.ToString(dr["Standards"].ToString()));
//                        mM_MT_Clita.Start_Date = Convert.ToDateTime(dr["Start_Date"].ToString());
//                        mM_MT_Clita.Clita_Standard_ID = Convert.ToDecimal(Convert.ToString(dr["Method"].ToString()));
//                        mM_MT_Clita.Maintenance_Time_TACT = TimeSpan.FromMinutes(Convert.ToDouble(dr["Tact_Time"]));
//                        //  mM_MT_Clita.Last_Maintenance_Date = Convert.ToDateTime(dr["Maintenance_Date"].ToString());
//                        mM_MT_Clita.Inserted_Date = DateTime.Now.Date;
//                        mM_MT_Clita.Inserted_Host = Dns.GetHostName();
//                        mM_MT_Clita.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                        //if (ModelState.IsValid)
//                        //{
//                        db.MM_Station_Based_Clita.Add(mM_MT_Clita);
//                        db.SaveChanges();
//                        //}

//                        foreach (var item in mM_MT_Clita.users)
//                        {
//                            MM_Station_Clita_Users clitauser = new MM_Station_Clita_Users();
//                            clitauser.Station_ID = mM_MT_Clita.Station_ID;
//                            clitauser.User_ID = item;
//                            clitauser.Station_Clita_ID = mM_MT_Clita.Station_Clita_ID;
//                            db.MM_Station_Clita_Users.Add(clitauser);
//                            db.SaveChanges();
//                        }

//                    }
//                    //  }
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//        }
//        #endregion
//    }
//}
