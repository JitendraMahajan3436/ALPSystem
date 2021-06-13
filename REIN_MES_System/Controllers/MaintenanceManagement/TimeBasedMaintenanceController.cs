//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using ZHB_AD.Models;
//using ZHB_AD.Helper;
//using ZHB_AD.App_LocalResources;
//using System.IO;
//using ZHB_AD.Controllers.BaseManagement;

//namespace ZHB_AD.Controllers
//{
//    /* Controller Name             : TimeBasedMaintenance
//     *  Description                : To create,edit,delete and show all Time Based Maintenance 
//     *  Author, Timestamp          : Vijaykuumar Kagne       
//     */
//    public class TimeBasedMaintenanceController : BaseController
//    {
//        #region Global Variables declaration
//        private MVML_MGMTEntities db = new MVML_MGMTEntities();
//        public GlobalData globalData = new GlobalData();
//        MM_MT_Time_Based_Maintenance mMMTTimeBasedMaintenance = new MM_MT_Time_Based_Maintenance();
//        #endregion
//        General generalObj = new General();

//        #region Show All Machines or single Machine with Time Based Maintenance
//        /*
//         * Action Name          : Index
//         * Input Parameter      : None
//         * Return Type          : ActionResult
//         * Author & Time Stamp  : Vijaykumar Kagne
//         * Description          : Get the list of Machine Time based maintenance
//         */

//        // GET: TimeBasedMaintenance
//        public ActionResult Index()
//        {
//            if (TempData["globalData"] != null)
//            {
//                globalData = (GlobalData)TempData["globalData"];
//            }

//            var mM_MT_Time_Based_Maintenance = db.MM_MT_Time_Based_Maintenance.Include(m => m.MM_Employee).Include(m => m.MM_MT_Machines).Include(m => m.MM_Plants);

//            globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//            globalData.subTitle = ResourceGlobal.Lists;
//            globalData.controllerName = "TimeBasedMaintenance";
//            globalData.actionName = ResourceGlobal.Lists;
//            globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Machine_Maintenance_Lists;
//            globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Machine_Maintenance_Lists;
//            ViewBag.GlobalDataModel = globalData;
//            //TempData["globalData"] = globalData;

//            return View(mM_MT_Time_Based_Maintenance.ToList());
//        }

//        /*
//         // Action Name          : Details
//         // Input Parameter      : id
//        //  Return Type          : ActionResult
//        //  Author & Time Stamp  : Vijaykumar Kagne
//        //  Description          : Get Details of Machine(Time based maintenance)
//        */

//        // GET: TimeBasedMaintenance/Details/5
//        public ActionResult Details(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_MT_Time_Based_Maintenance mM_MT_Time_Based_Maintenance = db.MM_MT_Time_Based_Maintenance.Find(id);
//            if (mM_MT_Time_Based_Maintenance == null)
//            {
//                return HttpNotFound();
//            }
//            globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//            globalData.subTitle = ResourceGlobal.Details;
//            globalData.controllerName = "TimeBasedMaintenance";
//            globalData.actionName = ResourceGlobal.Details;
//            globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Machine_Maintenance_Detail;
//            globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Machine_Maintenance_Detail;
//            ViewBag.GlobalDataModel = globalData;
//            //   TempData["globalData"] = globalData;

//            return View(mM_MT_Time_Based_Maintenance);
//        }
//        #endregion

//        #region Confirm Specific machine TBM Maintenance as completed
//        /*
//        * Action Name          : Confirm
//        * Input Parameter      : none
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Load Confirm machine maintenance form with details 
//        */
//        public ActionResult Confirm()
//        {
//            MM_MT_Time_Based_Maintenance_Log _TBM = new MM_MT_Time_Based_Maintenance_Log();
//            int a = ((FDSession)this.Session["FDSession"]).userId;
//            DateTime today = DateTime.Now;
//            var lst_TBM = (from t in db.MM_MT_Time_Based_Maintenance_Log.AsEnumerable() where t.Is_Maintenance_Done == false && (t.Scheduled_Date.Year == today.Year && t.Scheduled_Date.Month == today.Month && t.Scheduled_Date.Day == today.Day) select t).ToList();
//            globalData.pageTitle = ResourceTimeBasedMaitenanace.Time_Based_Maintenance;
//            globalData.subTitle = ResourceGlobal.Lists;
//            globalData.controllerName = "TimeBasedMaintenance";
//            globalData.actionName = ResourceGlobal.Lists;
//            globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Machine_Maintenance_Lists;
//            globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Machine_Maintenance_Lists;
//            ViewBag.GlobalDataModel = globalData;
//            TempData["globalData"] = globalData;
//            return View(lst_TBM);

//        }
//        public ActionResult SaveRemark(decimal id)
//        {
//            // ViewBag.id = id;
//            var obj = db.MM_MT_Time_Based_Maintenance_Log.Find(id);
//            return PartialView("PartialConfirm", obj);
//        }
//        /*
//        * Action Name          : Complete
//        * Input Parameter      : id
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Confirm machine maintenance  
//        */
//        public ActionResult Complete(FormCollection fc)
//        {

//            MM_MT_Time_Based_Maintenance_Log mM_MT_Time_Based_Maintenance = new MM_MT_Time_Based_Maintenance_Log();
//            // mMMTTimeBasedMaintenance = db.MM_MT_Time_Based_Maintenance.Where(x => x.TBM_ID == id).FirstOrDefault();
//            if (ModelState.IsValid)
//            {
//                string id = fc["TBM_Log_ID"].ToString().Trim();
//                MM_MT_Time_Based_Maintenance_Log _PMLog = new MM_MT_Time_Based_Maintenance_Log();
//                _PMLog = db.MM_MT_Time_Based_Maintenance_Log.Where(x => x.TBM_Log_ID.ToString() == id).FirstOrDefault();
//                _PMLog.Is_Maintenance_Done = true;
//                _PMLog.Remark = fc["Remark"].ToString();
//                _PMLog.Special_Observation = fc["Special_Observation"].ToString();
//                db.Entry(_PMLog).State = EntityState.Modified;
//                db.SaveChanges();
//                globalData.isSuccessMessage = true;
//                globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//                globalData.messageDetail = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Edit_Success;
//                TempData["globalData"] = globalData;
//                ViewBag.GlobalDataModel = globalData;
//                return RedirectToAction("confirm");
//            }

//            //globalData.pageTitle = ResourceTimeBasedMaitenanace.Machine;
//            //globalData.subTitle = ResourceGlobal.Edit;
//            //globalData.controllerName = "Time Based Maintenance";
//            //globalData.actionName = ResourceGlobal.Edit;
//            //globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            //globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            //globalData.isSuccessMessage = true;
//            //globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//            //globalData.messageDetail = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Edit_Success;
//            TempData["globalData"] = globalData;
//            ViewBag.GlobalDataModel = globalData;
//            //List<MM_MT_Time_Based_Maintenance> _PM = new List<MM_MT_Time_Based_Maintenance>();
//            //_PM.AddRange(db.MM_MT_Time_Based_Maintenance.ToList());
//            return RedirectToAction("Confirm");
//        }

//        /*
//       * Action Name          : EditMaintenanceDone
//       * Input Parameter      : MM_MT_Preventive_Maintenance
//       * Return Type          : ActionResult
//       * Author & Time Stamp  : Vijaykumar Kagne
//       * Description          : Update machine maintenance  
//       */
//        public ActionResult EditMaintenanceDone([Bind(Include = "PM_ID,Machine_ID,EQP_ID,Scheduled_Date,Last_Maintenance_Date,Maintenance_User_ID,Cycle_Period,Receipent_Email,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host")] MM_MT_Time_Based_Maintenance mM_MT_Time_Based_Maintenance)
//        {
//            mM_MT_Time_Based_Maintenance.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//            if (ModelState.IsValid)
//            {
//                mMMTTimeBasedMaintenance = db.MM_MT_Time_Based_Maintenance.FirstOrDefault(x => x.TBM_ID == mM_MT_Time_Based_Maintenance.TBM_ID);
//                mMMTTimeBasedMaintenance.Cycle_Period = mM_MT_Time_Based_Maintenance.Cycle_Period;
//                mMMTTimeBasedMaintenance.Maintenance_User_ID = mM_MT_Time_Based_Maintenance.Maintenance_User_ID;
//                mMMTTimeBasedMaintenance.Receipent_Email = mM_MT_Time_Based_Maintenance.Receipent_Email;
//                mMMTTimeBasedMaintenance.Last_Maintenance_Date = mM_MT_Time_Based_Maintenance.Last_Maintenance_Date;
//                mMMTTimeBasedMaintenance.Scheduled_Date = mM_MT_Time_Based_Maintenance.Scheduled_Date;

//                mMMTTimeBasedMaintenance.Machine_ID = mM_MT_Time_Based_Maintenance.Machine_ID;
//                mM_MT_Time_Based_Maintenance.Updated_Date = DateTime.Now;
//                mM_MT_Time_Based_Maintenance.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                mM_MT_Time_Based_Maintenance.Updated_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
//                db.Entry(mMMTTimeBasedMaintenance).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Confirm");
//            }


//            globalData.pageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//            globalData.subTitle = ResourceGlobal.Edit;
//            globalData.controllerName = "TimeBasedMaintenance";
//            globalData.actionName = ResourceGlobal.Edit;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;


//            globalData.isSuccessMessage = true;
//            globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//            globalData.messageDetail = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Edit_Success;

//            ViewBag.GlobalDataModel = globalData;
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Maintenance_User_ID);
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Inserted_User_ID);
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Updated_User_ID);
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Time_Based_Maintenance.Machine_ID);
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Time_Based_Maintenance.Machine_ID);
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment, "EQP_ID", "Equipment_Name", mM_MT_Time_Based_Maintenance.EQP_ID);
//            return View(mM_MT_Time_Based_Maintenance);
//        }
//        #endregion

//        #region Adding machine with Time Based Maintenance
//        /*
//        * Action Name          : Create
//        * Input Parameter      : None
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : used to load Time Based maintenance against Machine creation form
//        */
//        // GET: TimeBasedMaintenance/Create
//        public ActionResult Create()
//        {

//            globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//            globalData.subTitle = ResourceGlobal.Create;
//            globalData.controllerName = "TimeBasedMaintenance";
//            globalData.actionName = ResourceGlobal.Create;
//            globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//            globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//            // TempData["globalData"] = globalData;
//            ViewBag.GlobalDataModel = globalData;

//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number");
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name");
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment, "EQP_ID", "Equipment_Name");

//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.Where(x => x.Is_Status_Machine == true).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name");

//            return View();
//        }
//        /*
//        * Action Name          : Create
//        * Input Parameter      : MM_MT_Time_Based_Maintenance object
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : used to Add Time Based maintenance Machine 
//        */

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "TBM_ID,Machine_ID,Plant_ID,Scheduled_Date,Last_Maintenance_Date,Maintenance_User_ID,Cycle_Period,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,EQP_ID,users,mails")] MM_MT_Time_Based_Maintenance mM_MT_Time_Based_Maintenance)
//        {
//            mM_MT_Time_Based_Maintenance.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//            string mails = "";
//            foreach (var item in mM_MT_Time_Based_Maintenance.mails)
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

//            }

//            mM_MT_Time_Based_Maintenance.Receipent_Email = mails;

//            if (ModelState.IsValid)
//            {
//                //mMMTTimeBasedMaintenance = db.MM_MT_Time_Based_Maintenance.FirstOrDefault(x => x.TBM_ID == mM_MT_Time_Based_Maintenance.TBM_ID);
//                if (db.MM_MT_Time_Based_Maintenance.Where(x => x.Machine_ID == mM_MT_Time_Based_Maintenance.Machine_ID && x.EQP_ID == mM_MT_Time_Based_Maintenance.EQP_ID).Count() > 0)
//                {
//                    ModelState.AddModelError("Machine_ID", ResourceTimeBasedMaitenanace.Machine_Maintenance_Error_Machine_Name_Exists);
//                    globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//                    globalData.subTitle = ResourceGlobal.Create;
//                    globalData.controllerName = "TimeBasedMaintenance";
//                    globalData.actionName = ResourceGlobal.Create;
//                    globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//                    globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//                    ViewBag.GlobalDataModel = globalData;
//                    TempData["globalData"] = globalData;
//                }
//                else
//                {
//                    mM_MT_Time_Based_Maintenance.Inserted_Date = DateTime.Now;
//                    mM_MT_Time_Based_Maintenance.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
//                    mM_MT_Time_Based_Maintenance.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                    db.MM_MT_Time_Based_Maintenance.Add(mM_MT_Time_Based_Maintenance);
//                    db.SaveChanges();

//                    foreach (var item in mM_MT_Time_Based_Maintenance.users)
//                    {
//                        MM_MT_TBM_Users tbmuser = new MM_MT_TBM_Users();
//                        tbmuser.Machine_ID = mM_MT_Time_Based_Maintenance.Machine_ID;
//                        tbmuser.User_ID = item;
//                        tbmuser.TBM_ID = mM_MT_Time_Based_Maintenance.TBM_ID;
//                        db.MM_MT_TBM_Users.Add(tbmuser);
//                        db.SaveChanges();
//                    }


//                    globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//                    globalData.subTitle = ResourceGlobal.Create;
//                    globalData.controllerName = "TimeBasedMaintenance";
//                    globalData.actionName = ResourceGlobal.Create;
//                    globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//                    globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//                    globalData.isSuccessMessage = true;
//                    globalData.messageTitle = ResourceTimeBasedMaitenanace.Time_Based_Maintenance;
//                    globalData.messageDetail = ResourceTimeBasedMaitenanace.Machine_Maintenance_Success_Machine_Maintenance_Add_Success;
//                    ViewBag.GlobalDataModel = globalData;
//                    TempData["globalData"] = globalData;
//                    return RedirectToAction("Index");
//                }
//            }
//            ViewBag.mails = new SelectList(db.MM_Employee, "Employee_ID", "Email_Address", mM_MT_Time_Based_Maintenance.mails);
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.users);
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Maintenance_User_ID);
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Inserted_User_ID);
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Updated_User_ID);
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Time_Based_Maintenance.Machine_ID);
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Time_Based_Maintenance.Machine_ID);
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Time_Based_Maintenance.Plant_ID);
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment.Where(x => x.Machine_ID == mM_MT_Time_Based_Maintenance.Machine_ID), "EQP_ID", "Equipment_Name", mM_MT_Time_Based_Maintenance.EQP_ID);
//            return View(mM_MT_Time_Based_Maintenance);
//        }
//        #endregion

//        #region Edit Specific Machine details of time based maintenance
//        /*
//        * Action Name          : Edit
//        * Input Parameter      : id(TBM_ID)
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : used to load Time Based maintenance Edit form
//        */

//        // GET: TimeBasedMaintenance/Edit/5
//        public ActionResult Edit(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_MT_Time_Based_Maintenance mM_MT_Time_Based_Maintenance = db.MM_MT_Time_Based_Maintenance.Find(id);
//            if (mM_MT_Time_Based_Maintenance == null)
//            {
//                return HttpNotFound();
//            }
//            globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//            globalData.subTitle = ResourceGlobal.Edit;
//            globalData.controllerName = "TimeBasedMaintenance";
//            globalData.actionName = ResourceGlobal.Edit;
//            globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            // TempData["globalData"] = globalData;
//            ViewBag.GlobalDataModel = globalData;

//            List<string> emails = mM_MT_Time_Based_Maintenance.Receipent_Email.Split(';').ToList();
//            List<decimal> lst_Selected_EmailsID = new List<decimal>();
//            List<decimal> lst_usersids = new List<decimal>();

//            var lstusr = db.MM_MT_TBM_Users.Where(x => x.TBM_ID == id).ToList();
//            foreach (var usr in lstusr)
//            {
//                lst_usersids.Add(usr.User_ID);
//            }
//            foreach (string email in emails)
//            {
//                if (email != "" && email != " " && email != null)
//                {
//                    if (db.MM_Employee.Where(x => x.Email_Address.ToString().ToLower().Trim() == email.ToString().ToLower().Trim()).Count() > 0)
//                    {
//                        decimal j = db.MM_Employee.Where(x => x.Email_Address.ToLower().ToString().Trim() == email.ToLower().ToString().Trim()).FirstOrDefault().Employee_ID;
//                        lst_Selected_EmailsID.Add(j);
//                    }
//                }
//            }
//            ViewBag.mails = new MultiSelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address", lst_Selected_EmailsID);
//            ViewBag.users = new MultiSelectList(db.MM_Employee, "Employee_ID", "Employee_Name", lst_usersids);
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Inserted_User_ID);
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Updated_User_ID);
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Time_Based_Maintenance.Machine_ID);
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Time_Based_Maintenance.Machine_ID);
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Time_Based_Maintenance.Plant_ID);
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment.Where(x => x.Machine_ID == mM_MT_Time_Based_Maintenance.Machine_ID), "EQP_ID", "Equipment_Name", mM_MT_Time_Based_Maintenance.EQP_ID);
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Time_Based_Maintenance.Machine_ID);
//            return View(mM_MT_Time_Based_Maintenance);
//        }

//        /*
//        * Action Name          : Edit
//        * Input Parameter      : id(TBM_ID)
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : used to Edit Specified Time Based maintenance
//        */
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "TBM_ID,Machine_ID,EQP_ID,Plant_ID,Scheduled_Date,Last_Maintenance_Date,Maintenance_User_ID,Receipent_Email,Cycle_Period,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,mails,users")] MM_MT_Time_Based_Maintenance mM_MT_Time_Based_Maintenance)
//        {
//            mM_MT_Time_Based_Maintenance.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//            if (ModelState.IsValid)
//            {
//                string strmails = "";
//                foreach (var item in mM_MT_Time_Based_Maintenance.mails)
//                { //decimal j=Convert.ToDecimal(item);
//                    //  strmails += db.MM_Employee.Where(x => x.Employee_ID ==j ).FirstOrDefault().Email_Address + "; ";
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
//                }
//                mM_MT_Time_Based_Maintenance.Receipent_Email = strmails;
//                if (db.MM_MT_Time_Based_Maintenance.Where(x => x.Machine_ID == mM_MT_Time_Based_Maintenance.Machine_ID && x.EQP_ID == mM_MT_Time_Based_Maintenance.EQP_ID).Count() > 0)
//                {
//                    ModelState.AddModelError("EQP_ID", ResourceTimeBasedMaitenanace.Machine_Maintenance_Error_Machine_Name_Exists);
//                    globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//                    globalData.subTitle = ResourceGlobal.Create;
//                    globalData.controllerName = "TimeBasedMaintenance";
//                    globalData.actionName = ResourceGlobal.Create;
//                    globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//                    globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//                    ViewBag.GlobalDataModel = globalData;
//                    TempData["globalData"] = globalData;
//                }
//                else
//                {
//                    mMMTTimeBasedMaintenance = db.MM_MT_Time_Based_Maintenance.FirstOrDefault(x => x.TBM_ID == mM_MT_Time_Based_Maintenance.TBM_ID);
//                    mMMTTimeBasedMaintenance.Receipent_Email = mM_MT_Time_Based_Maintenance.Receipent_Email;
//                    mMMTTimeBasedMaintenance.Cycle_Period = mM_MT_Time_Based_Maintenance.Cycle_Period;
//                    mMMTTimeBasedMaintenance.EQP_ID = mM_MT_Time_Based_Maintenance.EQP_ID;
//                    mMMTTimeBasedMaintenance.Maintenance_User_ID = mM_MT_Time_Based_Maintenance.Maintenance_User_ID;
//                    mMMTTimeBasedMaintenance.Scheduled_Date = mM_MT_Time_Based_Maintenance.Scheduled_Date;
//                    mMMTTimeBasedMaintenance.Updated_Date = DateTime.Now;
//                    mMMTTimeBasedMaintenance.Updated_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
//                    mMMTTimeBasedMaintenance.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                    db.Entry(mMMTTimeBasedMaintenance).State = EntityState.Modified;
//                    db.SaveChanges();


//                    var record = db.MM_MT_TBM_Users.Where(x => x.TBM_ID == mM_MT_Time_Based_Maintenance.TBM_ID).ToList();
//                    db.MM_MT_TBM_Users.RemoveRange(record);

//                    foreach (var item in mM_MT_Time_Based_Maintenance.users)
//                    {
//                        MM_MT_TBM_Users pmuser = new MM_MT_TBM_Users();
//                        pmuser.TBM_ID = mMMTTimeBasedMaintenance.TBM_ID;
//                        pmuser.User_ID = item;
//                        pmuser.Machine_ID = mMMTTimeBasedMaintenance.Machine_ID;
//                        db.MM_MT_TBM_Users.Add(pmuser);
//                        db.SaveChanges();
//                    }
//                }


//                globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//                globalData.subTitle = ResourceGlobal.Edit;
//                globalData.controllerName = "TimeBasedMaintenance";
//                globalData.actionName = ResourceGlobal.Edit;
//                globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Success_Machine_Maintenance_Edit_Success;
//                globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Success_Machine_Maintenance_Edit_Success;
//                globalData.isSuccessMessage = true;
//                globalData.messageTitle = ResourceTimeBasedMaitenanace.Time_Based_Maintenance;
//                globalData.messageDetail = ResourceTimeBasedMaitenanace.Machine_Maintenance_Success_Machine_Maintenance_Edit_Success;
//                ViewBag.GlobalDataModel = globalData;
//                TempData["globalData"] = globalData;
//                return RedirectToAction("Index");
//            }

//            ViewBag.GlobalDataModel = globalData;
//            //ViewBag.Receipent_Email = new SelectList(db.MM_Employee, "Employee_ID", "Receipent_Email", mM_MT_Time_Based_Maintenance.Receipent_Email);
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Maintenance_User_ID);
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Inserted_User_ID);
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Updated_User_ID);
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Time_Based_Maintenance.Machine_ID);
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Time_Based_Maintenance.Machine_ID);
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Time_Based_Maintenance.Plant_ID);
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment.Where(x => x.Machine_ID == mM_MT_Time_Based_Maintenance.Machine_ID), "EQP_ID", "Equipment_Name", mM_MT_Time_Based_Maintenance.EQP_ID);
//            return View(mM_MT_Time_Based_Maintenance);
//        }
//        #endregion

//        #region Delete Specified Machine Details for time Based Maintenance
//        /*
//        * Action Name          : Delete
//        * Input Parameter      : id(TBM_ID)
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : used to load Time Based maintenance Delete form
//        */
//        // GET: TimeBasedMaintenance/Delete/5
//        public ActionResult Delete(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_MT_Time_Based_Maintenance mM_MT_Time_Based_Maintenance = db.MM_MT_Time_Based_Maintenance.Find(id);
//            if (mM_MT_Time_Based_Maintenance == null)
//            {
//                return HttpNotFound();
//            }

//            globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//            globalData.subTitle = ResourceGlobal.Delete;
//            globalData.controllerName = "TimeBasedMaintenance";
//            globalData.actionName = ResourceGlobal.Delete;
//            globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Delete_Machine_Maintenance;
//            globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Delete_Machine_Maintenance;
//            TempData["globalData"] = globalData;
//            ViewBag.GlobalDataModel = globalData;

//            return View(mM_MT_Time_Based_Maintenance);
//        }

//        /*
//       * Action Name          : Delete
//       * Input Parameter      : id(TBM_ID)
//       * Return Type          : ActionResult
//       * Author & Time Stamp  : Vijaykumar Kagne
//       * Description          : used to Delete Time Based maintenance
//       */
//        // POST: TimeBasedMaintenance/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(decimal id)
//        {


//            var tbmuser = db.MM_MT_TBM_Users.Where(x => x.TBM_ID == id).ToList();
//            db.MM_MT_TBM_Users.RemoveRange(tbmuser);
//            db.SaveChanges();

//            MM_MT_Time_Based_Maintenance mM_MT_Time_Based_Maintenance = db.MM_MT_Time_Based_Maintenance.Find(id);
//            db.MM_MT_Time_Based_Maintenance.Remove(mM_MT_Time_Based_Maintenance);
//            db.SaveChanges();

//            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_MT_Time_Based_Maintenance", "TBM_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

//            globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//            globalData.subTitle = ResourceGlobal.Delete;
//            globalData.controllerName = "TimeBasedMaintenance";
//            globalData.actionName = ResourceGlobal.Delete;
//            globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Delete_Machine_Maintenance;
//            globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Delete_Machine_Maintenance;


//            globalData.isSuccessMessage = true;
//            globalData.messageTitle = ResourceTimeBasedMaitenanace.Time_Based_Maintenance;
//            globalData.messageDetail = ResourceTimeBasedMaitenanace.Machine_Maintenance_Success_Machine_Maintenance_Delete_Success;
//            TempData["globalData"] = globalData;
//            ViewBag.GlobalDataModel = globalData;

//            return RedirectToAction("Index");
//        }
//        #endregion

//        #region update dropdown of machine with respect to plant
//        /*
//        * Action Name          : GetMachinesByPlantID
//        * Input Parameter      : id(TBM_ID)
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Get Machine list With respect to plant
//        */
//        public ActionResult GetMachinesByPlantID(int plantid)
//        {
//            var Machines = db.MM_MT_Machines.Where(x => x.Plant_ID == plantid).Select(x => new { x.Machine_ID, x.Machine_Number });
//            return Json(Machines, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetEquipmentByMachineID(int machineid)
//        {
//            var Machines = db.MM_MT_Preventive_Equipment.Where(x => x.Machine_ID == machineid).Select(x => new { x.EQP_ID, x.Equipment_Name });
//            return Json(Machines, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region Disposing objects(releasing Memory)
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//        #endregion

//        #region Upload Machine for time based maintenance
//        /*
//        * Action Name          : Upload
//        * Input Parameter      : None
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : used to upload Time Based maintenance
//        */
//        public ActionResult Upload()
//        {
//            if (TempData["TBMRecords"] != null)
//            {
//                ViewBag.tbmRecords = TempData["TBMRecords"];
//                ViewBag.tbmDataTable = TempData["TBMDataTable"];
//            }
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines, "Machine_ID", "Machine_Number");
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines, "Machine_ID", "Machine_Name");
//            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");

//            globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//            globalData.subTitle = ResourceGlobal.Upload;
//            globalData.controllerName = "TimeBasedMaintenance";
//            globalData.actionName = ResourceGlobal.Upload;
//            globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//            globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;

//            TempData["globalData"] = globalData;
//            ViewBag.GlobalDataModel = globalData;
//            return View();
//        }

//        /*
//        * Action Name          : Upload
//        * Input Parameter      : File object and MM_MT_Time_Based_Maintenance object
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne 
//        * Description          : used to upload Time Based maintenance
//        * Revision             :
//        */
//        //GET: GET The file from upload control 
//        [HttpPost]
//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Upload(HttpPostedFileBase files, [Bind(Include = "TBM_ID,Machine_ID,Plant_ID,Scheduled_Date,Last_Maintenance_Date,Maintenance_User_ID,Cycle_Period,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] MM_MT_Time_Based_Maintenance mM_MT_Time_Based_Maintenance)
//        {
//            try
//            {
//                mM_MT_Time_Based_Maintenance.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//                GlobalOperations globalOperations = new GlobalOperations();
//                string fileName = Path.GetFileName(files.FileName);
//                string fileExtension = Path.GetExtension(files.FileName);
//                string fileLocation = Server.MapPath("~/App_Data/" + fileName);
//                DataTable dt = globalOperations.ExcelToDataTable(files, fileLocation, fileExtension);
//                ExcelMTTBMRecords[] tbmRecords = new ExcelMTTBMRecords[dt.Rows.Count];

//                int j = 0;
//                if (dt.Rows.Count > 0)
//                {
//                    foreach (DataRow tbmlist in dt.Rows)
//                    {
//                        MM_MT_Time_Based_Maintenance _TBM = new MM_MT_Time_Based_Maintenance();
//                        ExcelMTTBMRecords pmsg = new ExcelMTTBMRecords();
//                        string machineName = tbmlist["Machine_Name"].ToString().Trim();
//                        string machinePart = tbmlist["Equipment_Name"].ToString().Trim();
//                        string scheduledDate = tbmlist["Scheduled_Date"].ToString().Trim();
//                        string cycle = tbmlist["Cycle_Period"].ToString().Trim();
//                        pmsg.MachineName = machineName;
//                        pmsg.EquipmentName = machinePart;

//                        decimal machine_id = db.MM_MT_Machines.Where(x => x.Machine_Name.ToLower() == machineName.ToLower()).FirstOrDefault().Machine_ID;
//                        decimal? eqp_id = null;
//                        if (db.MM_MT_Preventive_Equipment.Where(x => x.Equipment_Name.ToLower() == machinePart.ToLower() && x.Machine_ID == machine_id).Count() > 0)
//                        {
//                            eqp_id = db.MM_MT_Preventive_Equipment.Where(x => x.Equipment_Name.ToLower() == machinePart.ToLower() && x.Machine_ID == machine_id).FirstOrDefault().EQP_ID;
//                        }

//                        if (machineName == "" && machineName == null)
//                        {
//                            pmsg.TBM_Error = "Machine is not be null";
//                        }
//                        else if (db.MM_MT_Time_Based_Maintenance.Where(x => x.EQP_ID == eqp_id && x.Machine_ID == machine_id).Count() > 0)
//                        {
//                            pmsg.TBM_Error = "Machine is already Exists";
//                        }
//                        else if (eqp_id == null)
//                        {
//                            pmsg.TBM_Error = "No Machine Configured with this Part";
//                        }
//                        else if (eqp_id != null)
//                        {
//                            if (db.MM_MT_Preventive_Equipment.Where(x => x.Machine_ID == machine_id && x.Equipment_Name.ToLower() == machinePart.ToLower()).Count() > 0)
//                            {
//                                #region Save TBM
//                                _TBM.Machine_ID = machine_id;
//                                _TBM.EQP_ID = (decimal)eqp_id;
//                                _TBM.Cycle_Period = Convert.ToInt32(cycle);
//                                _TBM.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//                                _TBM.Scheduled_Date = Convert.ToDateTime(scheduledDate);

//                                string[] users = tbmlist["Users_Token"].ToString().Split(';').ToArray();
//                                string email = "";
//                                int[] user = new int[users.Count()];
//                                for (int i = 0; i < users.Count(); i++)
//                                {
//                                    string usr = users[i].ToString();
//                                    decimal id = db.MM_Employee.Where(x => x.Employee_No.ToString().ToLower() == usr.ToLower().Trim()).FirstOrDefault().Employee_ID;
//                                    user[i] = Convert.ToInt32(id);
//                                    email += db.MM_Employee.Where(x => x.Employee_No.ToString().ToLower() == usr.ToLower().Trim()).FirstOrDefault().Email_Address + ";";
//                                }
//                                _TBM.Receipent_Email = email;
//                                _TBM.Inserted_Date = DateTime.Now;
//                                _TBM.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
//                                _TBM.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                                db.MM_MT_Time_Based_Maintenance.Add(_TBM);
//                                db.SaveChanges();
//                                for (int i = 0; i < users.Count(); i++)
//                                {
//                                    if (users[i] != "" && users[i] != null && users[i]!=" ")
//                                    {
//                                        MM_MT_TBM_Users tbmuser = new MM_MT_TBM_Users();
//                                        tbmuser.Machine_ID = machine_id;
//                                        tbmuser.TBM_ID = _TBM.TBM_ID;
//                                        string usr = users[i].ToString();
//                                        decimal id = db.MM_Employee.Where(x => x.Employee_No.ToString().ToLower() == usr.ToLower().Trim()).FirstOrDefault().Employee_ID;
//                                        tbmuser.User_ID = id;
//                                    }
//                                }
//                                pmsg.TBM_Error = "Machine is added sucessfully";
//                                globalData.isSuccessMessage = true;
//                                globalData.messageTitle = ResourceTimeBasedMaitenanace.Time_Based_Maintenance;
//                                globalData.messageDetail = ResourceTimeBasedMaitenanace.Machine_Maintenance_Success_Machine_Maintenance_Add_Success;
//                                #endregion
//                            }
//                        }
//                        tbmRecords[j] = pmsg;
//                        j = j + 1;
//                    }

//                }
//                //   InsertIntoDataTable(dt, mM_MT_Time_Based_Maintenance);
//                ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Maintenance_User_ID);
//                ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Inserted_User_ID);
//                ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Time_Based_Maintenance.Updated_User_ID);
//                ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Time_Based_Maintenance.Machine_ID);
//                ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Time_Based_Maintenance.Machine_ID);
//                ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Time_Based_Maintenance.Plant_ID);

//                globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//                globalData.subTitle = ResourceGlobal.Upload;
//                globalData.controllerName = "Machines";
//                globalData.actionName = ResourceGlobal.Upload;
//                globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//                globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;

//                TempData["TBMRecords"] = tbmRecords;
//                TempData["TBMDataTable"] = dt;
//                ViewBag.tbmRecords = tbmRecords;
//                ViewBag.tbmDataTable = dt;
//                TempData["globalData"] = globalData;
//                ViewBag.GlobalDataModel = globalData;
//            }
//            catch (Exception ex)
//            {
//                globalData.pageTitle = ResourceTimeBasedMaitenanace.TBM_Configuration;
//                globalData.subTitle = ResourceGlobal.Upload;
//                globalData.controllerName = "Machines";
//                globalData.actionName = ResourceGlobal.Upload;
//                globalData.contentTitle = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;
//                globalData.contentFooter = ResourceTimeBasedMaitenanace.Machine_Maintenance_Title_Add_Machine_Maintenance;

//                globalData.isErrorMessage = true;
//                if (files == null)
//                {
//                    globalData.messageTitle = ResourceTimeBasedMaitenanace.Time_Based_Maintenance;
//                    globalData.messageDetail = "Please upload file, file can not be empty.";
//                }
//                else
//                {
//                    globalData.messageTitle = ResourceTimeBasedMaitenanace.Time_Based_Maintenance;
//                    globalData.messageDetail = ex.Message.ToString();
//                }

//                TempData["globalData"] = globalData;
//                ViewBag.GlobalDataModel = globalData;
//            }
//            return View();
//        }
//        #endregion

//        #region Insert all Data of DataTable into respective Database Table
//        /*
//        * Action Name          : InsertIntoDataTable
//        * Input Parameter      : DataTable object and MM_MT_Time_Based_Maintenance object
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne 
//        * Description          : used to insert datatable rows into database table
//        * Revision             :
//        */
//        private bool InsertIntoDataTable(DataTable dt, MM_MT_Time_Based_Maintenance mM_MT_Time_Based_Maintenance)
//        {
//            try
//            {
//                foreach (DataRow dr in dt.Rows)
//                {

//                    mM_MT_Time_Based_Maintenance.Inserted_Date = DateTime.Now;
//                    mM_MT_Time_Based_Maintenance.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                    mM_MT_Time_Based_Maintenance.Cycle_Period = Convert.ToInt32(dr["Cycle_Period"].ToString());
//                    mM_MT_Time_Based_Maintenance.Scheduled_Date = Convert.ToDateTime(dr["Scheduled_Date"].ToString());
//                    db.MM_MT_Time_Based_Maintenance.Add(mM_MT_Time_Based_Maintenance);
//                    db.SaveChanges();
//                }

//            }
//            catch (Exception ex)
//            {

//            }
//            return true;
//        }
//        #endregion
//    }
//}
