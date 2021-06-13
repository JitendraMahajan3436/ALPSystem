//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using ZHB_AD.App_LocalResources;
//using ZHB_AD.Helper;
//using ZHB_AD.Models;

//namespace ZHB_AD.Controllers.ZHB_AD
//{
//    public class Email_AlertController : Controller
//    {
//        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

//        // GET: Email_Alert
//        GlobalData globalData = new GlobalData();
//        FDSession adSession = new FDSession();
//        General generalObj = new General();
//        public ActionResult Index()
//        {
//            try
//            {
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//                int userID = ((FDSession)this.Session["FDSession"]).userId;
//                if (TempData["globalData"] != null)
//                {
//                    globalData = (GlobalData)TempData["globalData"];
//                }
//                ViewBag.GlobalDataModel = globalData;
//                globalData.pageTitle = ResourceModules.Email_alert;
//                var mM_Email_Alert = (from u in db.MM_Email_Alert join
//                                      f in db.MM_MTTUW_Employee on
//                                      u.Emp_ID equals f.Employee_ID
//                                      where u.Plant_ID ==plantID
//                                      select new
//                                      {
//                                        Emp_ID =  u.Emp_ID,
//                                       Emp_Name =  f.Employee_Name + "(" + f.Employee_No + ")"
//                                      }).Distinct().ToList();



//                var Email_Alertdata = mM_Email_Alert.Select(c => new Email_Alert_data
//                {
                   
//                    Emp_ID = Convert.ToInt16(c.Emp_ID),
//                    Emp_Name = c.Emp_Name
//                }).ToList();
             
//                return View(Email_Alertdata);
//            }
//            catch(Exception ex)
//            {
//                generalObj.addControllerException(ex, "Email_Alert", "Index", ((FDSession)this.Session["FDSession"]).userId);
//                return RedirectToAction("Index", "user");
//            }
           
//        }

//        // GET: Email_Alert/Details/5
//        public ActionResult Details(decimal? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_Email_Alert mM_Email_Alert = db.MM_Email_Alert.Find(id);
//            if (mM_Email_Alert == null)
//            {
//                return HttpNotFound();
//            }
//            return View(mM_Email_Alert);
//        }

//        // GET: Email_Alert/Create
//        public ActionResult Create()
//        {
//            try
//            {
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//                globalData.pageTitle = ResourceModules.Email_alert;
//                ViewBag.GlobalDataModel = globalData;
//                ViewBag.GlobalDataModel = globalData;
//                int userID = ((FDSession)this.Session["FDSession"]).userId;
//                ViewBag.Emp_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
//                ViewBag.Escal_EmpId = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
//                ViewBag.Alerts = new SelectList(db.MM_Alert_Mail.ToList(), "alert_Id", "alert_Name");
             
//                return View();
//            }
//            catch(Exception ex)
//            {
//                generalObj.addControllerException(ex, "Email_Alert", "Index", ((FDSession)this.Session["FDSession"]).userId);
//                return View("Index");
//            }
            
//        }

//        [OutputCache(Duration = 0)]
//        public ActionResult fillShopsDropDown(decimal userID)
//        {
//            var shopObj = from a in db.MM_MTTUW_Shops
//                          where !(from b in db.MM_Email_Alert where b.Emp_ID == userID select b.Shop_ID).Contains(a.Shop_ID)
//                          select new
//                          {
//                              Shop_ID = a.Shop_ID,
//                              Shop_Name = a.Shop_Name   
//                          };

//            return Json(shopObj, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(MM_Email_Alert mM_Email_Alert)
//        {
//            try
//            {

           
//            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//            DateTime today = DateTime.Now;
//            decimal UserID = ((FDSession)this.Session["FDSession"]).userId;
//            string compName = ((FDSession)this.Session["FDSession"]).userHost;
//            if (ModelState.IsValid)
//            {
//                foreach (decimal item in mM_Email_Alert.Shops)
//                {
//                    foreach(decimal alert in mM_Email_Alert.Alerts)
//                    {
                            
//                            mM_Email_Alert.Shop_ID = item;
//                            mM_Email_Alert.Alert_ID = alert;
//                            if(mM_Email_Alert.Escalation !=true)
//                            {
//                                mM_Email_Alert.Escal_EmpId = null;
//                                mM_Email_Alert.Delay_Escal = null;
//                            }
//                            mM_Email_Alert.Inserted_User_ID = UserID;
//                            mM_Email_Alert.Inserted_Date = today;
//                            mM_Email_Alert.Inserted_Host = compName;
//                            mM_Email_Alert.Plant_ID = plantID;
//                            db.MM_Email_Alert.Add(mM_Email_Alert);
//                            db.SaveChanges();
//                        }
//                }
                    
//                return RedirectToAction("Index");
//            }
//                ViewBag.Emp_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_Email_Alert.Emp_ID);
//                return View(mM_Email_Alert);
//            }
//            catch (Exception ex)
//            {
//                generalObj.addControllerException(ex, "Email_Alert", "Create", ((FDSession)this.Session["FDSession"]).userId);
//                return View("Index");
//            }
         

//        }

//        // GET: Email_Alert/Edit/5
//        public ActionResult Edit(decimal? id)
//        {
//            try
//            {
//                int userID = ((FDSession)this.Session["FDSession"]).userId;
//                globalData.pageTitle = ResourceModules.Email_alert;
//                ViewBag.GlobalDataModel = globalData;
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//                ViewBag.Plant_ID = plantID;
//                if (id == null)
//                {
//                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//                }
//                //MM_Email_Alert mM_Email_Alert = db.MM_Email_Alert.Find(id);
//                //if (mM_Email_Alert == null)
//                //{
//                //    return HttpNotFound();
//                //}
//                var Alertlist = db.MM_Email_Alert.Where(x => x.Emp_ID == id &&  x.Plant_ID == plantID).Select(x => (x.Alert_ID)).Distinct().ToArray();
//                var Shoplist = db.MM_Email_Alert.Where(x => x.Emp_ID == id && x.Plant_ID == plantID).Select(x => (x.Shop_ID)).Distinct().ToArray();

//                var EscalEmp = (from e in db.MM_Email_Alert
//                                   where e.Emp_ID == id
//                                   select new
//                                   {
//                                       e.Escal_EmpId
//                                   }).FirstOrDefault();
//                var Escalcheck = (from e in db.MM_Email_Alert
//                                  where e.Emp_ID == id
//                                  select new
//                                  {
//                                      e.Escalation,
//                                      e.Delay_Escal
//                                  }).FirstOrDefault();
//                var Plantcheck = (from e in db.MM_Email_Alert
//                                  where e.Emp_ID == id
//                                  select new
//                                  {
//                                      e.Plant_Alert
//                                  }).FirstOrDefault();
//                ViewBag.Emp_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", id);
//                ViewBag.Escal_EmpId = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", EscalEmp.Escal_EmpId);
//                ViewBag.Alerts = new MultiSelectList(db.MM_Alert_Mail.ToList(), "alert_Id", "alert_Name",Alertlist);
//                ViewBag.Shops =new MultiSelectList(db.MM_MTTUW_Shops.ToList(), "Shop_ID", "Shop_Name", Shoplist);

//                ViewBag.Escalation = Escalcheck.Escalation;
//                ViewBag.Plant_Alert = Plantcheck.Plant_Alert;
//                ViewBag.Delay_Escal = Escalcheck.Delay_Escal;


//                return View();
//            }
//            catch(Exception ex)
//            {
//                generalObj.addControllerException(ex, "Email_Alert", "Create", ((FDSession)this.Session["FDSession"]).userId);
//                return View("Index");
//            }
           
//        }

//        // POST: Email_Alert/Edit/5
      
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(MM_Email_Alert mM_Email_Alert)
//        {
//            try
//            {
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                
//                if (ModelState.IsValid)
//                {
//                    DateTime today = DateTime.Now;
//                    decimal UserID = ((FDSession)this.Session["FDSession"]).userId;
//                    string compName = ((FDSession)this.Session["FDSession"]).userHost;

//                    var insertedDate = db.MM_Email_Alert.Where(s => s.Emp_ID == mM_Email_Alert.Emp_ID).Select(s => s.Inserted_Date).FirstOrDefault();
//                    var HostName = db.MM_Email_Alert.Where(s => s.Emp_ID == mM_Email_Alert.Emp_ID).Select(s => s.Inserted_Host).FirstOrDefault();
//                    var insertedUserID = db.MM_Email_Alert.Where(s => s.Emp_ID == mM_Email_Alert.Emp_ID).Select(s => s.Inserted_User_ID).FirstOrDefault();
//                    var Emp = db.MM_Email_Alert.Where(x => x.Emp_ID ==mM_Email_Alert.Emp_ID  && x.Plant_ID == plantID).ToList();
//                    foreach (var item in Emp)
//                    {
//                        db.MM_Email_Alert.Remove(item);
//                        db.SaveChanges();
//                    }
//                    foreach(decimal item in mM_Email_Alert.Shops)
//                {
//                        foreach (decimal alert in mM_Email_Alert.Alerts)
//                        {

//                            mM_Email_Alert.Shop_ID = item;
//                            mM_Email_Alert.Alert_ID = alert;
//                            if (mM_Email_Alert.Escalation != true)
//                            {
//                                mM_Email_Alert.Escal_EmpId = null;
//                                mM_Email_Alert.Delay_Escal = null;
//                            }
//                            mM_Email_Alert.Inserted_User_ID = insertedUserID;
//                            mM_Email_Alert.Inserted_Date = insertedDate;
//                            mM_Email_Alert.Inserted_Host = HostName;
//                            mM_Email_Alert.Updated_Date = today;
//                            mM_Email_Alert.Updated_Host = compName;
//                            mM_Email_Alert.Updated_User_ID = UserID;
//                            mM_Email_Alert.Plant_ID = plantID;
//                            db.MM_Email_Alert.Add(mM_Email_Alert);
//                            db.SaveChanges();
//                        }
//                    }



//                    globalData.isSuccessMessage = true;
//                    globalData.pageTitle = ResourceModules.Email_alert;
//                    globalData.messageDetail = ResourceGlobal.Edit;
//                    TempData["globalData"] = globalData;
//                    return RedirectToAction("Index");
//                }
//                ViewBag.Emp_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_Email_Alert.Emp_ID);
//                return View(mM_Email_Alert);
//            }
//            catch(Exception ex)
//            {
//                return View("Index");
//            }
           
//        }

//        // GET: Email_Alert/Delete/5
//        public ActionResult Delete(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_Email_Alert mM_Email_Alert = db.MM_Email_Alert.Find(id);
//            if (mM_Email_Alert == null)
//            {
//                return HttpNotFound();
//            }
//            return View(mM_Email_Alert);
//        }

//        // POST: Email_Alert/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(decimal id)
//        {
//            MM_Email_Alert mM_Email_Alert = db.MM_Email_Alert.Find(id);
//            db.MM_Email_Alert.Remove(mM_Email_Alert);
//            db.SaveChanges();
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
//    }
//}
