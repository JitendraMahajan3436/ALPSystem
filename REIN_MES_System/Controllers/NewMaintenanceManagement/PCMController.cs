using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using System.Net;

namespace ZHB_AD.Controllers.NewMaintenanceManagement
{
    public class PCMController : BaseController
    {
        // GET: PCM
        #region Global Variable Declaration
        private MTTUWEntities db = new MTTUWEntities();
        GlobalData globalData = new GlobalData();
        MM_MTTUW_PCM_Clita mMMMClita = new MM_MTTUW_PCM_Clita();
        MM_MTTUW_PCM_Clita mmClitaLog = new MM_MTTUW_PCM_Clita();
        #endregion

        #region Get specified Details and List of Clita Machines
        /*
        * Action Name          : Index
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Get the Clita items of machines added
        */
        // GET: Clita
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var MM_MTTUW_PCM_Clita = db.MM_MTTUW_PCM_Clita.Include(m => m.MM_MTTUW_Employee).Where(m => m.Is_Deleted != true).Include(m => m.MM_MTTUW_Lines).Include(m => m.MM_MTTUW_Plants).Include(m => m.MM_MTTUW_Stations).Include(m => m.MM_MT_MTTUW_Machines).Include(m => m.MM_MTTUW_Shops);
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.CLITA_Item + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.CLITA_Item + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            return View(MM_MTTUW_PCM_Clita.Where(m => m.Plant_ID == plantId).ToList());
        }

        /*
       * Action Name          : Details
       * Input Parameter      : Id
       * Return Type          : ActionResult
       * Author & Time Stamp  : Vijaykumar Kagne
       * Description          : Get the Clita items Details of machines
       */
        // GET: Clita/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MTTUW_PCM_Clita MM_MTTUW_PCM_Clita = db.MM_MTTUW_PCM_Clita.Find(id);
            if (MM_MTTUW_PCM_Clita == null)
            {
                return HttpNotFound();
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.CLITA_Item + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.CLITA_Item + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(MM_MTTUW_PCM_Clita);
        }
        #endregion

        #region Create/ADD Clita item to Machine
        /*
        * Action Name          : Create
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Load Add the Clita items of machines
        */
        // GET: Clita/Create
        public ActionResult Create()
        {
            var plant_ID= ((FDSession)this.Session["FDSession"]).plantId;
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.mails = new SelectList(db.MM_MTTUW_Employee.Where(x => x.Email_Address != null && x.Email_Address != "" && x.Plant_ID == plant_ID), "Employee_ID", "Email_Address");
            ViewBag.users = new SelectList(db.MM_MTTUW_Employee.Where(c=>c.Plant_ID==plant_ID), "Employee_ID", "Employee_Name");
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name");
            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Tool.Where(c => c.Plant_ID == plant_ID), "Clita_Tool_ID", "Tool_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(c => c.Plant_ID == plant_ID), "Plant_ID", "Plant_Name");
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name");
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number");
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name");
            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Classification.Where(c => c.Plant_ID == plant_ID), "Classification", "Classification");
            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Standard.Where(c => c.Plant_ID == plant_ID), "Standard", "Standard");
            ViewBag.Clita_Method_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Method.Where(c => c.Plant_ID == plant_ID), "Clita_Method_ID", "Method");
            return View();
        }

        /*
      * Action Name          : Create
      * Input Parameter      : MM_MTTUW_PCM_Clita object
      * Return Type          : ActionResult
      * Author & Time Stamp  : Vijaykumar Kagne
      * Description          : Add the Clita items of machines
      */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Frequency,Is_Value_Based,Is_Critical,Clita_ID,Plant_ID,Shop_ID,Machine_ID,Clita_Item,Clita_Classification_ID,Clita_Standard_ID,Clita_Tool_ID,Clita_Method_ID,Frequency,Maintenance_Time_TACT,Recipent_Email,Start_Date,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Maintenance_User_ID,users,mails,Clita_Tools,Clita_Classification,Clita_Standard")] MM_MTTUW_PCM_Clita MM_MTTUW_PCM_Clita)
        {
            var plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            try
            {
               
                MM_MTTUW_PCM_Clita.Line_ID = db.MM_MT_MTTUW_Machines.Where(x => x.Machine_ID == MM_MTTUW_PCM_Clita.Machine_ID).Select(x => x.Line_ID).FirstOrDefault();
                var stationId = db.MM_MT_MTTUW_Machines.Where(x => x.Machine_ID == MM_MTTUW_PCM_Clita.Machine_ID).Select(x => x.Station_ID).FirstOrDefault();
                MM_MTTUW_PCM_Clita.Station_ID = Convert.ToDecimal(stationId);
                if (!String.IsNullOrWhiteSpace(MM_MTTUW_PCM_Clita.Clita_Standard))
                {
                    MM_MTTUW_PCM_Clita_Standard stdObj = db.MM_MTTUW_PCM_Clita_Standard.Where(a => a.Standard == MM_MTTUW_PCM_Clita.Clita_Standard).FirstOrDefault();

                    if (stdObj == null)
                    {
                        MM_MTTUW_PCM_Clita_Standard standardObj = new MM_MTTUW_PCM_Clita_Standard();
                        standardObj.Standard = MM_MTTUW_PCM_Clita.Clita_Standard;
                        standardObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        standardObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        standardObj.Inserted_Date = DateTime.Now;
                        standardObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        db.MM_MTTUW_PCM_Clita_Standard.Add(standardObj);
                        db.SaveChanges();
                        MM_MTTUW_PCM_Clita.Clita_Standard_ID = standardObj.Clita_Standard_ID;
                    }
                    else
                    {
                        MM_MTTUW_PCM_Clita.Clita_Standard_ID = stdObj.Clita_Standard_ID;
                    }
                }

                if (!String.IsNullOrWhiteSpace(MM_MTTUW_PCM_Clita.Clita_Classification))
                {
                    MM_MTTUW_PCM_Clita_Classification cltnObj = db.MM_MTTUW_PCM_Clita_Classification.Where(a => a.Classification == MM_MTTUW_PCM_Clita.Clita_Classification).FirstOrDefault();

                    if (cltnObj == null)
                    {
                        MM_MTTUW_PCM_Clita_Classification classificationObj = new MM_MTTUW_PCM_Clita_Classification();
                        classificationObj.Classification = MM_MTTUW_PCM_Clita.Clita_Classification;
                        classificationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        classificationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        classificationObj.Inserted_Date = DateTime.Now;
                        classificationObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        db.MM_MTTUW_PCM_Clita_Classification.Add(classificationObj);
                        db.SaveChanges();
                        MM_MTTUW_PCM_Clita.Clita_Classification_ID = classificationObj.Clita_Classification_ID;
                    }
                    else
                    {
                        MM_MTTUW_PCM_Clita.Clita_Classification_ID = cltnObj.Clita_Classification_ID;
                    }
                }
                MM_MTTUW_PCM_Clita.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                string mails = "";
                //foreach (var item in MM_MTTUW_PCM_Clita.mails)
                //{
                //    if (db.MM_MTTUW_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).Count() > 0)
                //    {
                //        mails += db.MM_MTTUW_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).First().Email_Address + ";";
                //    }
                //    else
                //    {
                //        mails += item.ToString() + "; ";
                //        //add email address which are manually entered here
                //        //not present in database
                //    }
                //    // mails += db.MM_MTTUW_Employee.Where(x => x.Employee_ID.ToString() == item.ToString()).First().Email_Address + ";";
                //}

                //MM_MTTUW_PCM_Clita.Recipent_Email = mails;

                if (ModelState.IsValid)
                {
                    if (db.MM_MTTUW_PCM_Clita.Where(x => x.Machine_ID == MM_MTTUW_PCM_Clita.Machine_ID && x.Clita_Item.ToLower() == MM_MTTUW_PCM_Clita.Clita_Item.Trim().ToLower() && x.Clita_Tool_ID == MM_MTTUW_PCM_Clita.Clita_Tool_ID && x.Clita_Method_ID == MM_MTTUW_PCM_Clita.Clita_Method_ID && x.Clita_Standard_ID == MM_MTTUW_PCM_Clita.Clita_Standard_ID && x.Clita_Classification_ID == MM_MTTUW_PCM_Clita.Clita_Classification_ID).Count() > 0)
                    {
                        ModelState.AddModelError("Clita_Item", "Machine with Specified clita item is already exists.");
                        globalData.hostName = GlobalOperations.GetHostName();
                        globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
                        globalData.subTitle = ResourceGlobal.Create;
                        globalData.controllerName = "Clita Items";
                        globalData.actionName = ResourceGlobal.Create;
                        globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                        globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                        //globalData.isErrorMessage = true;
                        //globalData.messageTitle = ResourceModules.CLITA_Item;
                        //globalData.messageDetail = "Machine with Specified clita item is already exists.";
                        ViewBag.GlobalDataModel = globalData;
                        TempData["globalData"] = globalData;
                    }
                    else
                    {
                        string expected_time;

                        if (MM_MTTUW_PCM_Clita.Maintenance_Time_TACT == null)
                        {
                            expected_time = Convert.ToString("00:00:00");
                        }
                        else
                        {
                            expected_time = Convert.ToString(MM_MTTUW_PCM_Clita.Maintenance_Time_TACT);
                        }

                        MM_MTTUW_PCM_Clita.Maintenance_Time_TACT = TimeSpan.Parse(expected_time);
                        MM_MTTUW_PCM_Clita.Start_Date = DateTime.Now;
                        MM_MTTUW_PCM_Clita.Inserted_Date = DateTime.Now;
                        MM_MTTUW_PCM_Clita.End_Date = DateTime.Now;
                        MM_MTTUW_PCM_Clita.Inserted_Host = HttpContext.Request.UserHostAddress;
                        MM_MTTUW_PCM_Clita.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        MM_MTTUW_PCM_Clita.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        MM_MTTUW_PCM_Clita.Is_Critical = MM_MTTUW_PCM_Clita.Is_Critical;
                        db.MM_MTTUW_PCM_Clita.Add(MM_MTTUW_PCM_Clita);
                        db.SaveChanges();

                        List<MM_MT_MTTUW_Machines> mmClitaToolsList = new List<MM_MT_MTTUW_Machines>();
                        foreach (int clitaToolID in MM_MTTUW_PCM_Clita.Clita_Tools)
                        {
                            MM_MTTUW_PCM_clitaItems_Tools_Relation cliteToolsObj = new MM_MTTUW_PCM_clitaItems_Tools_Relation();
                            cliteToolsObj.Clita_ID = MM_MTTUW_PCM_Clita.Clita_ID;
                            cliteToolsObj.Clita_Tool_ID = clitaToolID;
                            //MM_MTTUW_PCM_Clita.Clita_ID.Add(cliteToolsObj);
                            db.MM_MTTUW_PCM_clitaItems_Tools_Relation.Add(cliteToolsObj);
                            db.SaveChanges();
                        }

                        //foreach (var item in MM_MTTUW_PCM_Clita.users)
                        //{
                        //    MM_MT_PCM_Clita_Users clitauser = new MM_MT_PCM_Clita_Users();
                        //    clitauser.Machine_ID = MM_MTTUW_PCM_Clita.Machine_ID;
                        //    clitauser.User_ID = item;
                        //    clitauser.Clita_ID = MM_MTTUW_PCM_Clita.Clita_ID;
                        //    db.MM_MT_PCM_Clita_Users.Add(clitauser);
                        //    db.SaveChanges();
                        //}

                        globalData.stationIPAddress = GlobalOperations.GetIP();
                        globalData.hostName = GlobalOperations.GetHostName();
                        globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
                        globalData.subTitle = ResourceGlobal.Create;
                        globalData.controllerName = "Clita Items";
                        globalData.actionName = ResourceGlobal.Create;
                        globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                        globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.CLITA_Item;
                        globalData.messageDetail = ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                        ViewBag.GlobalDataModel = globalData;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Clita Items";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CLITA_Item;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
            }

            globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            //ViewBag.mails = new SelectList(db.MM_MTTUW_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
            //ViewBag.users = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Maintenance_User_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Updated_User_ID);
            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Tool.Where(c => c.Plant_ID == plant_ID), "Clita_Tool_ID", "Tool_Name");
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", MM_MTTUW_PCM_Clita.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", MM_MTTUW_PCM_Clita.Plant_ID);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", MM_MTTUW_PCM_Clita.Station_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", MM_MTTUW_PCM_Clita.Machine_ID);
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", MM_MTTUW_PCM_Clita.Machine_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", MM_MTTUW_PCM_Clita.Shop_ID);
            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Classification.Where(c => c.Plant_ID == plant_ID), "Classification", "Classification", MM_MTTUW_PCM_Clita.Clita_Classification);
            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Standard.Where(c => c.Plant_ID == plant_ID), "Standard", "Standard", MM_MTTUW_PCM_Clita.Clita_Standard);
            ViewBag.Clita_Method_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Method.Where(c => c.Plant_ID == plant_ID), "Clita_Method_ID", "Method", MM_MTTUW_PCM_Clita.Clita_Method_ID);
            return View(MM_MTTUW_PCM_Clita);
        }
        #endregion

        #region Edit Clita Machine
        /*
      * Action Name          : Edit
      * Input Parameter      : None
      * Return Type          : ActionResult
      * Author & Time Stamp  : Vijaykumar Kagne
      * Description          : Load Details Edit the Clita items on form
      */
        // GET: Clita/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MTTUW_PCM_Clita MM_MTTUW_PCM_Clita = db.MM_MTTUW_PCM_Clita.Find(id);
            if (MM_MTTUW_PCM_Clita == null)
            {
                return HttpNotFound();
            }

            //List<string> emails = MM_MTTUW_PCM_Clita.Recipent_Email.Split(';').ToList();
            //List<decimal> lst_Selected_EmailsID = new List<decimal>();
            //List<decimal> lst_usersids = new List<decimal>();

            //var lstusr = db.MM_MT_PCM_Clita_Users.Where(x => x.Clita_ID == id).ToList();
            //foreach (var usr in lstusr)
            //{
            //    lst_usersids.Add(usr.User_ID);
            //}
            //foreach (string email in emails)
            //{
            //    if (email != "" && email != " " && email != null)
            //    {
            //        decimal j = db.MM_MTTUW_Employee.Where(x => x.Email_Address.ToLower().Trim() == email.Trim()).FirstOrDefault().Employee_ID;
            //        lst_Selected_EmailsID.Add(j);
            //    }
            //}
            var plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = ((FDSession)this.Session["FDSession"]).userHost;
            globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;

            ViewBag.GlobalDataModel = globalData;
            //ViewBag.mails = new MultiSelectList(db.MM_MTTUW_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address", lst_Selected_EmailsID);
            //ViewBag.users = new MultiSelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", lst_usersids);
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Maintenance_User_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", MM_MTTUW_PCM_Clita.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", MM_MTTUW_PCM_Clita.Plant_ID);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", MM_MTTUW_PCM_Clita.Station_ID);
            ViewBag.Clita_Tools = new MultiSelectList(db.MM_MTTUW_PCM_Clita_Tool.Where(c => c.Plant_ID == plant_ID), "Clita_Tool_ID", "Tool_Name",db.MM_MTTUW_PCM_clitaItems_Tools_Relation.Where(c=>c.Clita_ID== MM_MTTUW_PCM_Clita.Clita_ID).Select(c=>c.Clita_Tool_ID).ToArray());
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", MM_MTTUW_PCM_Clita.Machine_ID);
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", MM_MTTUW_PCM_Clita.Machine_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", MM_MTTUW_PCM_Clita.Shop_ID);
            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Classification.Where(c => c.Plant_ID == plant_ID), "Classification", "Classification", MM_MTTUW_PCM_Clita.Clita_Classification);
            ViewBag.Clita_Classification = new SelectList(db.MM_MTTUW_PCM_Clita_Classification.Where(c => c.Plant_ID == plant_ID), "Classification", "Classification", MM_MTTUW_PCM_Clita.Clita_Classification);
            ViewBag.Clita_Standard = new SelectList(db.MM_MTTUW_PCM_Clita_Standard.Where(c => c.Plant_ID == plant_ID), "Standard", "Standard", MM_MTTUW_PCM_Clita.Clita_Standard);
            ViewBag.Clita_Method_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Method.Where(c => c.Plant_ID == plant_ID), "Clita_Method_ID", "Method", MM_MTTUW_PCM_Clita.Clita_Method_ID);
            return View(MM_MTTUW_PCM_Clita);
        }

        /*
     * Action Name          : Edit
     * Input Parameter      : MM_MTTUW_PCM_Clita
     * Return Type          : ActionResult
     * Author & Time Stamp  : Vijaykumar Kagne
     * Description          : Edit the Clita items of machines
     */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Frequency,Is_Value_Based,Is_Critical,Clita_Tools,Clita_ID,Plant_ID,Shop_ID,Machine_ID,Clita_Item,Clita_Classification_ID,Clita_Standard_ID,Clita_Tool_ID,Clita_Method_ID,Frequency,Maintenance_Time_TACT,Recipent_Email,Start_Date,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,End_Date,mails,users,Clita_Classification,Clita_Standard")] MM_MTTUW_PCM_Clita MM_MTTUW_PCM_Clita)
        {
            try
            {
                var plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                mMMMClita = db.MM_MTTUW_PCM_Clita.FirstOrDefault(x => x.Clita_ID == MM_MTTUW_PCM_Clita.Clita_ID);
                mMMMClita.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrWhiteSpace(MM_MTTUW_PCM_Clita.Clita_Standard))
                    {
                        MM_MTTUW_PCM_Clita_Standard stdObj = db.MM_MTTUW_PCM_Clita_Standard.Where(a => a.Standard == MM_MTTUW_PCM_Clita.Clita_Standard).FirstOrDefault();

                        if (stdObj == null)
                        {
                            MM_MTTUW_PCM_Clita_Standard standardObj = new MM_MTTUW_PCM_Clita_Standard();
                            standardObj.Standard = MM_MTTUW_PCM_Clita.Clita_Standard;
                            standardObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            standardObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            standardObj.Inserted_Date = DateTime.Now;
                            standardObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                            db.MM_MTTUW_PCM_Clita_Standard.Add(standardObj);
                            db.SaveChanges();
                            mMMMClita.Clita_Standard_ID = standardObj.Clita_Standard_ID;
                        }
                        else
                        {
                            mMMMClita.Clita_Standard_ID = stdObj.Clita_Standard_ID;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(MM_MTTUW_PCM_Clita.Clita_Classification))
                    {
                        MM_MTTUW_PCM_Clita_Classification cltnObj = db.MM_MTTUW_PCM_Clita_Classification.Where(a => a.Classification == MM_MTTUW_PCM_Clita.Clita_Classification).FirstOrDefault();

                        if (cltnObj == null)
                        {
                            MM_MTTUW_PCM_Clita_Classification classificationObj = new MM_MTTUW_PCM_Clita_Classification();
                            classificationObj.Classification = MM_MTTUW_PCM_Clita.Clita_Classification;
                            classificationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            classificationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            classificationObj.Inserted_Date = DateTime.Now;
                            classificationObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                            db.MM_MTTUW_PCM_Clita_Classification.Add(classificationObj);
                            db.SaveChanges();
                            mMMMClita.Clita_Classification_ID = classificationObj.Clita_Classification_ID;
                        }
                        else
                        {
                            mMMMClita.Clita_Classification_ID = cltnObj.Clita_Classification_ID;
                        }
                    }

                    //string strmails = "";
                    //foreach (var item in MM_MTTUW_PCM_Clita.mails)
                    //{
                    //    if (db.MM_MTTUW_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).Count() > 0)
                    //    {
                    //        strmails += db.MM_MTTUW_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).First().Email_Address + ";";
                    //    }
                    //    else
                    //    {
                    //        strmails += item.ToString() + "; ";
                    //        //add email address which are manually entered here
                    //        //not present in database
                    //    }
                    //    //    decimal j = Convert.ToDecimal(item);
                    //    //    strmails += db.MM_MTTUW_Employee.Where(x => x.Employee_ID == j).FirstOrDefault().Email_Address+"; "; 
                    //}
                    //MM_MTTUW_PCM_Clita.Recipent_Email = strmails;

                    string expected_time;

                    if (MM_MTTUW_PCM_Clita.Maintenance_Time_TACT == null)
                    {
                        expected_time = Convert.ToString("00:00:00");
                    }
                    else
                    {
                        expected_time = Convert.ToString(MM_MTTUW_PCM_Clita.Maintenance_Time_TACT);
                    }

                    //mMMMClita.Shop_ID = MM_MTTUW_PCM_Clita.Shop_ID;
                    // mMMMClita.Line_ID = MM_MTTUW_PCM_Clita.Shop_ID;
                    // mMMMClita.Station_ID = MM_MTTUW_PCM_Clita.Station_ID;

                    mMMMClita.Maintenance_Time_TACT = TimeSpan.Parse(expected_time);
                    mMMMClita.Maintenance_Time_TACT = MM_MTTUW_PCM_Clita.Maintenance_Time_TACT;
                    mMMMClita.Clita_Method_ID = MM_MTTUW_PCM_Clita.Clita_Method_ID;
                    mMMMClita.Clita_Item = MM_MTTUW_PCM_Clita.Clita_Item;
                    mMMMClita.Clita_Tool_ID = MM_MTTUW_PCM_Clita.Clita_Tool_ID;
                    //mMMMClita.Clita_Classification = mMMMClita.Clita_Classification;
                    // mMMMClita.Clita_Classification_ID = MM_MTTUW_PCM_Clita.Clita_Classification_ID;
                    mMMMClita.Frequency = MM_MTTUW_PCM_Clita.Frequency;
                    mMMMClita.Recipent_Email = MM_MTTUW_PCM_Clita.Recipent_Email;
                    mMMMClita.Start_Date = MM_MTTUW_PCM_Clita.Start_Date;
                    mMMMClita.End_Date = MM_MTTUW_PCM_Clita.End_Date;
                    // mMMMClita.Clita_Standard_ID = MM_MTTUW_PCM_Clita.Clita_Standard_ID;
                    // mMMMClita.Clita_Standard = mMMMClita.Clita_Standard;
                    mMMMClita.Updated_Date = DateTime.Now;
                    mMMMClita.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mMMMClita.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mMMMClita.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mMMMClita.Start_Date = DateTime.Now;
                    mMMMClita.Inserted_Date = DateTime.Now;
                    mMMMClita.End_Date = DateTime.Now;
                    mMMMClita.Is_Critical = MM_MTTUW_PCM_Clita.Is_Critical;
                    mMMMClita.Is_Edited = true;
                    db.Entry(mMMMClita).State = EntityState.Modified;
                    db.SaveChanges();

                    MM_MTTUW_PCM_clitaItems_Tools_Relation[] ToolArrayIDArray = db.MM_MTTUW_PCM_clitaItems_Tools_Relation.Where(x => x.Clita_ID== mMMMClita.Clita_ID).ToArray();
                    db.MM_MTTUW_PCM_clitaItems_Tools_Relation.RemoveRange(ToolArrayIDArray);

                    foreach (int clitaToolID in MM_MTTUW_PCM_Clita.Clita_Tools)
                    {
                        MM_MTTUW_PCM_clitaItems_Tools_Relation cliteToolsObj = new MM_MTTUW_PCM_clitaItems_Tools_Relation();
                        cliteToolsObj.Clita_ID = MM_MTTUW_PCM_Clita.Clita_ID;
                        cliteToolsObj.Clita_Tool_ID = clitaToolID;
                        //MM_MTTUW_PCM_Clita.Clita_ID.Add(cliteToolsObj);
                        db.MM_MTTUW_PCM_clitaItems_Tools_Relation.Add(cliteToolsObj);
                        db.SaveChanges();
                    }
                    //var record = db.MM_MT_PCM_Clita_Users.Where(x => x.Clita_ID == MM_MTTUW_PCM_Clita.Clita_ID).ToList();
                    //db.MM_MT_PCM_Clita_Users.RemoveRange(record);

                    //foreach (var item in MM_MTTUW_PCM_Clita.users)
                    //{
                    //    MM_MT_PCM_Clita_Users clitauser = new MM_MT_PCM_Clita_Users();
                    //    clitauser.Clita_ID = mMMMClita.Clita_ID;
                    //    clitauser.User_ID = item;
                    //    clitauser.Machine_ID = mMMMClita.Machine_ID;
                    //    db.MM_MT_PCM_Clita_Users.Add(clitauser);
                    //    db.SaveChanges();
                    //}

                    globalData.stationIPAddress = GlobalOperations.GetIP();
                    globalData.hostName = ((FDSession)this.Session["FDSession"]).userHost;
                    globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
                    globalData.subTitle = ResourceGlobal.Edit;
                    globalData.controllerName = "Clita Items";
                    globalData.actionName = ResourceGlobal.Edit;
                    globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                    ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Maintenance_User_ID);
                    ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Inserted_User_ID);
                    ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Updated_User_ID);
                    ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", MM_MTTUW_PCM_Clita.Line_ID);
                    ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", MM_MTTUW_PCM_Clita.Plant_ID);
                    ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", MM_MTTUW_PCM_Clita.Station_ID);
                    ViewBag.Clita_Tools = new MultiSelectList(db.MM_MTTUW_PCM_Clita_Tool.Where(c => c.Plant_ID == plant_ID), "Clita_Tool_ID", "Tool_Name", db.MM_MTTUW_PCM_clitaItems_Tools_Relation.Where(c => c.Clita_ID == MM_MTTUW_PCM_Clita.Clita_ID).Select(c => c.Clita_Tool_ID).ToArray());
                    ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", MM_MTTUW_PCM_Clita.Machine_ID);
                    ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", MM_MTTUW_PCM_Clita.Machine_ID);
                    ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == plant_ID).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", MM_MTTUW_PCM_Clita.Shop_ID);
                    ViewBag.Clita_Classification = new SelectList(db.MM_MTTUW_PCM_Clita_Classification.Where(c => c.Plant_ID == plant_ID), "Classification", "Classification", MM_MTTUW_PCM_Clita.Clita_Classification);
                    ViewBag.Clita_Standard = new SelectList(db.MM_MTTUW_PCM_Clita_Standard.Where(c => c.Plant_ID == plant_ID), "Standard", "Standard", MM_MTTUW_PCM_Clita.Clita_Standard);
                    ViewBag.Clita_Method_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Method.Where(c => c.Plant_ID == plant_ID), "Clita_Method_ID", "Method", MM_MTTUW_PCM_Clita.Clita_Method_ID);

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.CLITA_Item;
                    globalData.messageDetail = ResourceModules.CLITA_Item + " " + ResourceMessages.Edit_Success;
                    ViewBag.GlobalDataModel = globalData;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CLITA_Item;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;

            }

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            //ViewBag.mails = new SelectList(db.MM_MTTUW_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
            //ViewBag.users = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Maintenance_User_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", MM_MTTUW_PCM_Clita.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", MM_MTTUW_PCM_Clita.Line_ID);
            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Tool, "Clita_Tool_ID", "Tool_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", MM_MTTUW_PCM_Clita.Plant_ID);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", MM_MTTUW_PCM_Clita.Station_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", MM_MTTUW_PCM_Clita.Machine_ID);
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", MM_MTTUW_PCM_Clita.Machine_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name", MM_MTTUW_PCM_Clita.Shop_ID);
            ViewBag.Clita_Classification = new SelectList(db.MM_MTTUW_PCM_Clita_Classification, "Classification", "Classification", MM_MTTUW_PCM_Clita.Clita_Classification);
            ViewBag.Clita_Standard = new SelectList(db.MM_MTTUW_PCM_Clita_Standard, "Standard", "Standard", MM_MTTUW_PCM_Clita.Clita_Standard);
            ViewBag.Clita_Method_ID = new SelectList(db.MM_MTTUW_PCM_Clita_Method, "Clita_Method_ID", "Method", MM_MTTUW_PCM_Clita.Clita_Method_ID);

            return View(mMMMClita);
        }
        #endregion

        #region Delete Clita Machine
        /*
      * Action Name          : Delete
      * Input Parameter      : None
      * Return Type          : ActionResult
      * Author & Time Stamp  : Vijaykumar Kagne
      * Description          : Delete the Clita items of machines (load details on form)
      */
        // GET: Clita/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MTTUW_PCM_Clita MM_MTTUW_PCM_Clita = db.MM_MTTUW_PCM_Clita.Find(id);
            if (MM_MTTUW_PCM_Clita == null)
            {
                return HttpNotFound();
            }


            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;

            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(MM_MTTUW_PCM_Clita);
        }

        /*
     * Action Name          : Delete
     * Input Parameter      : ID
     * Return Type          : ActionResult
     * Author & Time Stamp  : Vijaykumar Kagne
     * Description          : Delete the Clita items of machines
     */
        // POST: Clita/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_MTTUW_PCM_Clita MM_MTTUW_PCM_Clita = db.MM_MTTUW_PCM_Clita.Find(id);
            try
            {
                MM_MTTUW_PCM_Clita.Is_Deleted = true;
                MM_MTTUW_PCM_Clita.Is_Edited = true;
                db.Entry(MM_MTTUW_PCM_Clita).State = EntityState.Modified;
                db.SaveChanges();

                //db.MM_MTTUW_PCM_Clita_Standard.Remove(db.MM_MTTUW_PCM_Clita_Standard.Where(x => x.Clita_Standard_ID == MM_MTTUW_PCM_Clita.Clita_Standard_ID).FirstOrDefault());
                //db.MM_MTTUW_PCM_Clita_Method.Remove(db.MM_MTTUW_PCM_Clita_Method.Where(x => x.Clita_Method_ID == MM_MTTUW_PCM_Clita.Clita_Method_ID).FirstOrDefault());
                //db.MM_MTTUW_PCM_Clita_Classification.RemoveRange(db.MM_MTTUW_PCM_Clita_Classification.Where(x => x.Clita_Classification_ID == MM_MTTUW_PCM_Clita.Clita_Classification_ID).ToList());
                //db.MM_MTTUW_PCM_clitaItems_Tools_Relation.RemoveRange(db.MM_MTTUW_PCM_clitaItems_Tools_Relation.Where(x => x.Clita_ID == MM_MTTUW_PCM_Clita.Clita_ID).ToList());
                //db.MM_MTTUW_PCM_Clita.Remove(MM_MTTUW_PCM_Clita);
                //db.SaveChanges();

                //globalData.stationIPAddress = GlobalOperations.GetIP();
                //globalData.hostName = GlobalOperations.GetHostName();
                //globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
                //globalData.subTitle = ResourceGlobal.Delete;
                //globalData.controllerName = "Clita Items";
                //globalData.actionName = ResourceGlobal.Delete;
                //globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                //globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;

                //globalData.isSuccessMessage = true;
                //globalData.messageTitle = ResourceModules.CLITA_Item;
                //globalData.messageDetail = ResourceClitaItem.Clita_Success_Clita_Item_Delete_Success;
                //ViewBag.GlobalDataModel = globalData;
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CLITA_Item;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceDisplayName.Machine_Clita_Item;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return RedirectToAction("Index");
        }
        #endregion

        #region Releasing Objects
        /*
      * Action Name          : Dispose
      * Input Parameter      : None
      * Return Type          : ActionResult
      * Author & Time Stamp  : Vijaykumar Kagne
      * Description          : To Released un-used objects from memory
      */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        //   #region Filling child DropDown Dependant Elements on parent
        /*
      * Action Name          : GetShopByPlantID
      * Input Parameter      : id(plantID)
      * Return Type          : ActionResult
      * Author & Time Stamp  : Vijaykumar Kagne
      * Description          : Get all Shop list By plant
      */
        //update Shop with respect plant
        public ActionResult GetShopByPlantID(int plantid)
        {
            var Shops = db.MM_MTTUW_Shops.Where(c => c.Plant_ID == plantid).Select(a => new { a.Shop_ID, a.Shop_Name });
            return Json(Shops, JsonRequestBehavior.AllowGet);
        }

        /*
     * Action Name          : GetLineByShopID
     * Input Parameter      : id(shopid)
     * Return Type          : ActionResult
     * Author & Time Stamp  : Vijaykumar Kagne
     * Description          : Get all Line list By Shop
     */

        //Update Line with respective shop
        public ActionResult GetLineByShopID(int shopid)
        {
            var Lines = db.MM_MTTUW_Lines.Where(c => c.Shop_ID == shopid).Select(a => new { a.Line_ID, a.Line_Name }).OrderBy(x => x.Line_Name);
            return Json(Lines, JsonRequestBehavior.AllowGet);
        }


        //Update Clita Items with respective Machines
        public ActionResult GetClitaItemsByMachineID(int machineid)
        {
            var ClitaItems = db.MM_MTTUW_PCM_Clita.Where(c => c.Machine_ID == machineid).Select(a => new { a.Clita_ID, a.Clita_Item }).OrderBy(x => x.Clita_Item);
            return Json(ClitaItems, JsonRequestBehavior.AllowGet);
        }

        /*
        * Action Name          : GetStationByLineID
        * Input Parameter      : id(lineid)
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Get all Station list By Line
        */
        //Update station with respective Line
        public ActionResult GetStationByLineID(int lineid)
        {
            var stationslist = db.MM_MTTUW_Stations.Where(c => c.Line_ID == lineid).Select(a => new { a.Station_ID, a.Station_Name });
            var machineslist = db.MM_MT_MTTUW_Machines.Where(d => d.Line_ID == lineid).Select(a => new { a.Machine_ID, a.Machine_Name }).OrderBy(x => x.Machine_Name);
            var data = new { stations = stationslist, machines = machineslist };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /*
         * Action Name          : GetMachineByShopID
         * Input Parameter      : id(lineid)
         * Return Type          : ActionResult
         * Author & Time Stamp  : Vijaykumar Kagne
         * Description          : Get all Machine list By Shop
         */
        //Update station with respective Station
        public ActionResult GetMachineByShopID(int shopid)
        {
            var Machine = db.MM_MT_MTTUW_Machines.Where(x => x.Shop_ID == shopid).Select(a => a.Machine_ID).ToList();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            foreach (var item in Machine)
            {
                obj.Add(item.ToString(), db.MM_MT_MTTUW_Machines.Where(x => x.Machine_ID == item).Select(x => x.Machine_Name).First().ToString());
            }
            ViewData["MyList"] = new SelectList(obj.OrderBy(x => x.Value), "Key", "Value");
            return Json(ViewData["MyList"], JsonRequestBehavior.AllowGet);
        }

    }
}