using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Helper;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Controllers.BaseManagement;
using System.IO;

namespace ZHB_AD.Controllers
{
    /* Controller Name         : Clita
*  Description                : To create,edit,delete and show all clita items 
*  Author, Timestamp          : Ajay Wagh       
*/

    public class ClitaController : BaseController
    {
        #region Global Variable Declaration
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        MM_MT_Clita mMMMClita = new MM_MT_Clita();
        General generalHelper = new General();
        //MM_MT_Clita_Log mmClitaLog = new MM_MT_Clita_Log();
        #endregion

        #region Get specified Details and List of Clita Machines
        /*
        * Action Name          : Index
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Get the Clita items of machines added
        */
        // GET: Clita
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var mM_MT_Clita = db.MM_MT_Clita.Include(m => m.MM_MTTUW_Employee).Where(m => m.Is_Deleted != true).Include(m => m.MM_MTTUW_Lines).Include(m => m.MM_MTTUW_Plants).Include(m => m.MM_MTTUW_Stations).Include(m => m.MM_MT_MTTUW_Machines).Include(m => m.MM_MTTUW_Shops);
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Clita_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.CLITA_Item + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.CLITA_Item + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            return View(mM_MT_Clita.Where(m => m.Plant_ID == plantId).ToList());
        }

        /*
       * Action Name          : Details
       * Input Parameter      : Id
       * Return Type          : ActionResult
       * Author & Time Stamp  : Ajay Wagh
       * Description          : Get the Clita items Details of machines
       */
        // GET: Clita/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Clita mM_MT_Clita = db.MM_MT_Clita.Find(id);
            if (mM_MT_Clita == null)
            {
                return HttpNotFound();
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Clita_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.CLITA_Item + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.CLITA_Item + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(mM_MT_Clita);
        }
        #endregion

        #region Create/ADD Clita item to Machine
        /*
        * Action Name          : Create
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Load Add the Clita items of machines
        */
        // GET: Clita/Create
        public ActionResult Create()
        {
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Clita_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            //   TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.mails = new SelectList(db.MM_MTTUW_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
            ViewBag.users = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Clita_Tool_ID = new SelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(c => c.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name");
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Shop_ID == 0).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name");
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Shop_ID == 0).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(m => m.Plant_ID == plantId).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name");
            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification.Where(c => c.Plant_ID == plantId), "Classification", "Classification");
            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard.Where(c => c.Plant_ID == plantId), "Standard", "Standard");
            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method.Where(c => c.Plant_ID == plantId), "Clita_Method_ID", "Method");
            return View();
        }

        /*
      * Action Name          : Create
      * Input Parameter      : MM_MT_Clita object
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Add the Clita items of machines
      */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_MT_Clita mM_MT_Clita)
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                int count = 0;
                if (mM_MT_Clita.Is_Value_Based == true && (mM_MT_Clita.Lower_Limit == null))
                {
                    ModelState.AddModelError("Lower_Limit", "Please enter the lower limit value");
                    count = 1;

                }
                if (mM_MT_Clita.Lower_Limit > mM_MT_Clita.Upper_Limit)
                {
                    ModelState.AddModelError("Upper_Limit", "Upper limit should be greater than Lower limit");
                    count = 1;

                }
                if (count == 1)
                {
                    ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Maintenance_User_ID);
                    ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Inserted_User_ID);
                    ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Updated_User_ID);
                    ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", mM_MT_Clita.Line_ID);
                    ViewBag.Clita_Tool_ID = new MultiSelectList(db.MM_MT_Clita_Tool.Where(c => c.Plant_ID == plantId), "Clita_Tool_ID", "Tool_Name", mM_MT_Clita.MM_MT_ClitaItems_Tools_Relation.Select(a => a.Clita_Tool_ID));
                    ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mM_MT_Clita.Plant_ID);
                    ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", mM_MT_Clita.Station_ID);
                    ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Shop_ID == mM_MT_Clita.Shop_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
                    ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plantId && c.Shop_ID == mM_MT_Clita.Shop_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
                    ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == plantId).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Clita.Shop_ID);
                    ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification.Where(c => c.Plant_ID == plantId), "Classification", "Classification", mM_MT_Clita.Clita_Classification);
                    ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard.Where(c => c.Plant_ID == plantId), "Standard", "Standard", mM_MT_Clita.Clita_Standard);
                    ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method.Where(c => c.Plant_ID == plantId), "Clita_Method_ID", "Method", mM_MT_Clita.Clita_Method_ID);
                    return View(mM_MT_Clita);
                }

                try
                {
                    mM_MT_Clita.Line_ID = db.MM_MT_MTTUW_Machines.Where(x => x.Machine_ID == mM_MT_Clita.Machine_ID).Select(x => x.Line_ID).FirstOrDefault();
                    mM_MT_Clita.Station_ID = (decimal)db.MM_MT_MTTUW_Machines.Where(x => x.Machine_ID == mM_MT_Clita.Machine_ID).Select(x => x.Station_ID).FirstOrDefault();
                    if (!String.IsNullOrWhiteSpace(mM_MT_Clita.Clita_Standard))
                    {
                        MM_MT_Clita_Standard stdObj = db.MM_MT_Clita_Standard.Where(a => a.Standard == mM_MT_Clita.Clita_Standard).FirstOrDefault();

                        if (stdObj == null)
                        {
                            MM_MT_Clita_Standard standardObj = new MM_MT_Clita_Standard();
                            standardObj.Standard = mM_MT_Clita.Clita_Standard;
                            standardObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            standardObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            standardObj.Inserted_Date = DateTime.Now;
                            standardObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                            db.MM_MT_Clita_Standard.Add(standardObj);
                            db.SaveChanges();
                            mM_MT_Clita.Clita_Standard_ID = standardObj.Clita_Standard_ID;
                        }
                        else
                        {
                            mM_MT_Clita.Clita_Standard_ID = stdObj.Clita_Standard_ID;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(mM_MT_Clita.Clita_Classification))
                    {
                        MM_MT_Clita_Classification cltnObj = db.MM_MT_Clita_Classification.Where(a => a.Classification == mM_MT_Clita.Clita_Classification).FirstOrDefault();

                        if (cltnObj == null)
                        {
                            MM_MT_Clita_Classification classificationObj = new MM_MT_Clita_Classification();
                            classificationObj.Classification = mM_MT_Clita.Clita_Classification;
                            classificationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            classificationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            classificationObj.Inserted_Date = DateTime.Now;
                            classificationObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                            db.MM_MT_Clita_Classification.Add(classificationObj);
                            db.SaveChanges();
                            mM_MT_Clita.Clita_Classification_ID = classificationObj.Clita_Classification_ID;
                        }
                        else
                        {
                            mM_MT_Clita.Clita_Classification_ID = cltnObj.Clita_Classification_ID;
                        }
                    }
                    mM_MT_Clita.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    string mails = "";

                    mM_MT_Clita.End_Date = DateTime.Now;
                    if (ModelState.IsValid)
                    {
                        if (db.MM_MT_Clita.Where(x => x.Machine_ID == mM_MT_Clita.Machine_ID && x.Clita_Item.ToLower() == mM_MT_Clita.Clita_Item.Trim().ToLower() && x.Clita_Tool_ID == mM_MT_Clita.Clita_Tool_ID && x.Clita_Method_ID == mM_MT_Clita.Clita_Method_ID && x.Clita_Standard_ID == mM_MT_Clita.Clita_Standard_ID && x.Clita_Classification_ID == mM_MT_Clita.Clita_Classification_ID).Count() > 0)
                        {
                            ModelState.AddModelError("Clita_Item", "Machine with Specified clita item is already exists.");
                            globalData.hostName = GlobalOperations.GetHostName();
                            globalData.pageTitle = ResourceModules.Clita_Config;
                            globalData.subTitle = ResourceGlobal.Create;
                            globalData.controllerName = "Clita Items";
                            globalData.actionName = ResourceGlobal.Create;
                            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
                            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;

                            ViewBag.GlobalDataModel = globalData;
                            TempData["globalData"] = globalData;
                        }
                        else
                        {
                            string expected_time;

                            if (mM_MT_Clita.Maintenance_Time_TACT == null)
                            {
                                expected_time = Convert.ToString("00:00:00");
                            }
                            else
                            {
                                expected_time = Convert.ToString(mM_MT_Clita.Maintenance_Time_TACT);
                            }

                            List<MM_MT_Clita_Tool> mmClitaToolsList = new List<MM_MT_Clita_Tool>();
                            foreach (int clitaToolID in mM_MT_Clita.Clita_Tools)
                            {
                                MM_MT_ClitaItems_Tools_Relation cliteToolsObj = new MM_MT_ClitaItems_Tools_Relation();
                                cliteToolsObj.Clita_Tool_ID = clitaToolID;
                                mM_MT_Clita.MM_MT_ClitaItems_Tools_Relation.Add(cliteToolsObj);
                            }

                            mM_MT_Clita.Maintenance_Time_TACT = TimeSpan.Parse(expected_time);
                            mM_MT_Clita.Start_Date = DateTime.Now;
                            mM_MT_Clita.Inserted_Date = DateTime.Now;
                            mM_MT_Clita.End_Date = DateTime.Now;
                            mM_MT_Clita.Inserted_Host = HttpContext.Request.UserHostAddress;
                            mM_MT_Clita.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.MM_MT_Clita.Add(mM_MT_Clita);
                            db.SaveChanges();

                            globalData.stationIPAddress = GlobalOperations.GetIP();
                            globalData.hostName = GlobalOperations.GetHostName();
                            globalData.pageTitle = ResourceModules.Clita_Config;
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
                    globalData.pageTitle = ResourceModules.Clita_Config;
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
            }
            globalData.pageTitle = ResourceModules.Clita_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            //var plantId = ((FDSession)this.Session["FDSession"]).plantId;


            ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Maintenance_User_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", mM_MT_Clita.Line_ID);
            ViewBag.Clita_Tool_ID = new MultiSelectList(db.MM_MT_Clita_Tool.Where(c => c.Plant_ID == plantId), "Clita_Tool_ID", "Tool_Name", mM_MT_Clita.MM_MT_ClitaItems_Tools_Relation.Select(a => a.Clita_Tool_ID));
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mM_MT_Clita.Plant_ID);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", mM_MT_Clita.Station_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Shop_ID == mM_MT_Clita.Shop_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Shop_ID == mM_MT_Clita.Shop_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Clita.Shop_ID);
            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification.Where(c => c.Plant_ID == plantId), "Classification", "Classification", mM_MT_Clita.Clita_Classification);
            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard.Where(c => c.Plant_ID == plantId), "Standard", "Standard", mM_MT_Clita.Clita_Standard);
            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method.Where(c => c.Plant_ID == plantId), "Clita_Method_ID", "Method", mM_MT_Clita.Clita_Method_ID);
            return View(mM_MT_Clita);
        }
        #endregion

        #region Edit Clita Machine
        /*
      * Action Name          : Edit
      * Input Parameter      : None
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Load Details Edit the Clita items on form
      */
        // GET: Clita/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Clita mM_MT_Clita = db.MM_MT_Clita.Find(id);
            if (mM_MT_Clita == null)
            {
                return HttpNotFound();
            }
            MM_MT_Clita_Tool[] mmClitaObj = null;
            mmClitaObj = (from clitatool in db.MM_MT_Clita_Tool
                          join cr in db.MM_MT_ClitaItems_Tools_Relation
                          on clitatool.Clita_Tool_ID equals cr.Clita_Tool_ID
                          join clita in db.MM_MT_Clita
                          on cr.Clita_ID equals clita.Clita_ID
                          where clita.Clita_ID == id
                          select clitatool).ToArray();


            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = ((FDSession)this.Session["FDSession"]).userHost;
            globalData.pageTitle = ResourceModules.Clita_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;

            ViewBag.GlobalDataModel = globalData;
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Maintenance_User_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", mM_MT_Clita.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Plant_ID", "Plant_Name", mM_MT_Clita.Plant_ID);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", mM_MT_Clita.Station_ID);
            ViewBag.Clita_Tools = new MultiSelectList(db.MM_MT_Clita_Tool.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Clita_Tool_ID", "Tool_Name", mmClitaObj.Select(x => x.Clita_Tool_ID).ToArray());
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Clita.Shop_ID);
            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Classification", "Classification", mM_MT_Clita.Clita_Classification_ID);
            ViewBag.Clita_Classification = new SelectList(db.MM_MT_Clita_Classification.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Classification", "Classification", mM_MT_Clita.MM_MT_Clita_Classification.Classification);
            ViewBag.Clita_Standard = new SelectList(db.MM_MT_Clita_Standard.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Standard", "Standard", mM_MT_Clita.MM_MT_Clita_Standard.Standard);
            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Clita_Method_ID", "Method", mM_MT_Clita.Clita_Method_ID);
            return View(mM_MT_Clita);
        }

        /*
     * Action Name          : Edit
     * Input Parameter      : MM_MT_Clita
     * Return Type          : ActionResult
     * Author & Time Stamp  : Ajay Wagh
     * Description          : Edit the Clita items of machines
     */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_MT_Clita mM_MT_Clita)
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var ID = mM_MT_Clita.Clita_ID;
            MM_MT_Clita_Tool[] mmClitaObj = null;
            mmClitaObj = (from clitatool in db.MM_MT_Clita_Tool
                          join cr in db.MM_MT_ClitaItems_Tools_Relation
                          on clitatool.Clita_Tool_ID equals cr.Clita_Tool_ID
                          join clita in db.MM_MT_Clita
                          on cr.Clita_ID equals clita.Clita_ID
                          where clita.Clita_ID == ID
                          select clitatool).ToArray();
            if (ModelState.IsValid)
            {


                int count = 0;
                if (mM_MT_Clita.Is_Value_Based == true && (mM_MT_Clita.Lower_Limit == null))
                {
                    ModelState.AddModelError("Lower_Limit", "Please enter the lower limit value");
                    count = 1;

                }
                if (mM_MT_Clita.Lower_Limit > mM_MT_Clita.Upper_Limit)
                {
                    ModelState.AddModelError("Upper_Limit", "Upper limit should be greater than Lower limit");
                    count = 1;

                }
                if (count == 1)
                {
                    ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Maintenance_User_ID);
                    ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Inserted_User_ID);
                    ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Updated_User_ID);
                    ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", mM_MT_Clita.Line_ID);
                    ViewBag.Clita_Tool_ID = new MultiSelectList(db.MM_MT_Clita_Tool.Where(c => c.Plant_ID == plantId), "Clita_Tool_ID", "Tool_Name", mM_MT_Clita.MM_MT_ClitaItems_Tools_Relation.Select(a => a.Clita_Tool_ID));
                    ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mM_MT_Clita.Plant_ID);
                    ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", mM_MT_Clita.Station_ID);
                    ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Shop_ID == mM_MT_Clita.Shop_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
                    ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plantId && c.Shop_ID == mM_MT_Clita.Shop_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
                    ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == plantId).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Clita.Shop_ID);
                    ViewBag.Clita_Tools = new MultiSelectList(db.MM_MT_Clita_Tool.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Clita_Tool_ID", "Tool_Name", mmClitaObj.Select(x => x.Clita_Tool_ID).ToArray());
                    ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification.Where(c => c.Plant_ID == plantId), "Classification", "Classification", mM_MT_Clita.Clita_Classification);
                    ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard.Where(c => c.Plant_ID == plantId), "Standard", "Standard", mM_MT_Clita.Clita_Standard);
                    ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method.Where(c => c.Plant_ID == plantId), "Clita_Method_ID", "Method", mM_MT_Clita.Clita_Method_ID);
                    return View(mM_MT_Clita);
                }
                try
                {

                    mMMMClita = db.MM_MT_Clita.FirstOrDefault(x => x.Clita_ID == mM_MT_Clita.Clita_ID);
                    mMMMClita.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    if (ModelState.IsValid)
                    {
                        if (!String.IsNullOrWhiteSpace(mM_MT_Clita.Clita_Standard))
                        {
                            MM_MT_Clita_Standard stdObj = db.MM_MT_Clita_Standard.Where(a => a.Standard == mM_MT_Clita.Clita_Standard).FirstOrDefault();

                            if (stdObj == null)
                            {
                                MM_MT_Clita_Standard standardObj = new MM_MT_Clita_Standard();
                                standardObj.Standard = mM_MT_Clita.Clita_Standard;
                                standardObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                standardObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                standardObj.Inserted_Date = DateTime.Now;
                                standardObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                                db.MM_MT_Clita_Standard.Add(standardObj);
                                db.SaveChanges();
                                mMMMClita.Clita_Standard_ID = standardObj.Clita_Standard_ID;
                            }
                            else
                            {
                                mMMMClita.Clita_Standard_ID = stdObj.Clita_Standard_ID;
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(mM_MT_Clita.Clita_Classification))
                        {
                            MM_MT_Clita_Classification cltnObj = db.MM_MT_Clita_Classification.Where(a => a.Classification == mM_MT_Clita.Clita_Classification).FirstOrDefault();

                            if (cltnObj == null)
                            {
                                MM_MT_Clita_Classification classificationObj = new MM_MT_Clita_Classification();
                                classificationObj.Classification = mM_MT_Clita.Clita_Classification;
                                classificationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                classificationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                classificationObj.Inserted_Date = DateTime.Now;
                                classificationObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                                db.MM_MT_Clita_Classification.Add(classificationObj);
                                db.SaveChanges();
                                mMMMClita.Clita_Classification_ID = classificationObj.Clita_Classification_ID;
                            }
                            else
                            {
                                mMMMClita.Clita_Classification_ID = cltnObj.Clita_Classification_ID;
                            }
                        }

                        
                        //string strmails = "";
                        //foreach (var item in mM_MT_Clita.mails)
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
                        //mM_MT_Clita.Recipent_Email = strmails;

                        string expected_time;

                        if (mM_MT_Clita.Maintenance_Time_TACT == null)
                        {
                            expected_time = Convert.ToString("00:00:00");
                        }
                        else
                        {
                            expected_time = Convert.ToString(mM_MT_Clita.Maintenance_Time_TACT);
                        }

                        //mMMMClita.Shop_ID = mM_MT_Clita.Shop_ID;
                        // mMMMClita.Line_ID = mM_MT_Clita.Shop_ID;
                        // mMMMClita.Station_ID = mM_MT_Clita.Station_ID;

                        mMMMClita.Maintenance_Time_TACT = TimeSpan.Parse(expected_time);
                        mMMMClita.Maintenance_Time_TACT = mM_MT_Clita.Maintenance_Time_TACT;
                        mMMMClita.Clita_Method_ID = mM_MT_Clita.Clita_Method_ID;
                        mMMMClita.Clita_Item = mM_MT_Clita.Clita_Item;
                        mMMMClita.Clita_Tool_ID = mM_MT_Clita.Clita_Tool_ID;
                        mMMMClita.Clita_Classification = mM_MT_Clita.Clita_Classification;
                        //mMMMClita.Clita_Classification_ID = mM_MT_Clita.Clita_Classification_ID;
                        mMMMClita.Cycle = mM_MT_Clita.Cycle;
                        mMMMClita.Recipent_Email = mM_MT_Clita.Recipent_Email;
                        mMMMClita.Start_Date = mM_MT_Clita.Start_Date;
                        mMMMClita.End_Date = mM_MT_Clita.End_Date;
                        mMMMClita.Clita_Tools = mM_MT_Clita.Clita_Tools;
                        // mMMMClita.Clita_Standard_ID = mM_MT_Clita.Clita_Standard_ID;
                        mMMMClita.Clita_Standard = mM_MT_Clita.Clita_Standard;
                        mMMMClita.Data_Retention_Period = mM_MT_Clita.Data_Retention_Period;
                        mMMMClita.Updated_Date = DateTime.Now;
                        mMMMClita.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        mMMMClita.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        mMMMClita.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mMMMClita.Start_Date = DateTime.Now;
                        mMMMClita.Inserted_Date = DateTime.Now;
                        mMMMClita.End_Date = DateTime.Now;
                        mMMMClita.Is_Edited = true;
                        mMMMClita.Is_Value_Based = mM_MT_Clita.Is_Value_Based;
                        mMMMClita.Upper_Limit = mM_MT_Clita.Upper_Limit;
                        mMMMClita.Lower_Limit = mM_MT_Clita.Lower_Limit;
                        db.Entry(mMMMClita).State = EntityState.Modified;
                        db.SaveChanges();


                        if (mM_MT_Clita.Clita_Tools.Count() > 0)
                        {
                            MM_MT_ClitaItems_Tools_Relation clitarelObj = new MM_MT_ClitaItems_Tools_Relation();
                            foreach (var clita in mM_MT_Clita.Clita_Tools)
                            {
                                MM_MT_ClitaItems_Tools_Relation obj = db.MM_MT_ClitaItems_Tools_Relation.Where(m => m.Clita_ID == ID && m.Clita_Tool_ID == clita).FirstOrDefault();
                                if (obj == null)
                                {
                                    clitarelObj.Clita_ID = ID;
                                    clitarelObj.Clita_Tool_ID = clita;
                                    //clitarelObj.MM_MT_Clita.Clita_Classification_ID = mMMMClita.Clita_Classification_ID;
                                    db.MM_MT_ClitaItems_Tools_Relation.Add(clitarelObj);
                                    db.SaveChanges();
                                }
                            }
                        }
                        //var record = db.MM_MT_Clita_Users.Where(x => x.Clita_ID == mM_MT_Clita.Clita_ID).ToList();
                        //db.MM_MT_Clita_Users.RemoveRange(record);

                        //foreach (var item in mM_MT_Clita.users)
                        //{
                        //    MM_MT_Clita_Users clitauser = new MM_MT_Clita_Users();
                        //    clitauser.Clita_ID = mMMMClita.Clita_ID;
                        //    clitauser.User_ID = item;
                        //    clitauser.Machine_ID = mMMMClita.Machine_ID;
                        //    db.MM_MT_Clita_Users.Add(clitauser);
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
                        ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Maintenance_User_ID);
                        ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Inserted_User_ID);
                        ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Updated_User_ID);
                        ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", mM_MT_Clita.Line_ID);
                        ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mM_MT_Clita.Plant_ID);
                        ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", mM_MT_Clita.Station_ID);
                        ViewBag.Clita_Tool_ID = new MultiSelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name", mM_MT_Clita.MM_MT_ClitaItems_Tools_Relation.Select(a => a.Clita_Tool_ID));
                        ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Shop_ID == mM_MT_Clita.Shop_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Clita.Machine_ID);
                        ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
                        ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == mMMMClita.Plant_ID).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Clita.Shop_ID);
                        ViewBag.Clita_Classification = new SelectList(db.MM_MT_Clita_Classification, "Classification", "Classification", mM_MT_Clita.Clita_Classification);
                        ViewBag.Clita_Standard = new SelectList(db.MM_MT_Clita_Standard, "Standard", "Standard", mM_MT_Clita.Clita_Standard);
                        ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method, "Clita_Method_ID", "Method", mM_MT_Clita.Clita_Method_ID);
                        ViewBag.Clita_Tools = new MultiSelectList(db.MM_MT_Clita_Tool.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Clita_Tool_ID", "Tool_Name", mmClitaObj.Select(x => x.Clita_Tool_ID).ToArray());
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
                ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Employee_ID", "Employee_Name", mM_MT_Clita.Maintenance_User_ID);
                ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Clita.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Employee_ID", "Employee_Name", mM_MT_Clita.Updated_User_ID);
                ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", mM_MT_Clita.Line_ID);
                ViewBag.Clita_Tool_ID = new MultiSelectList(db.MM_MT_Clita_Tool, "Clita_Tool_ID", "Tool_Name", mM_MT_Clita.MM_MT_ClitaItems_Tools_Relation.Select(a => a.Clita_Tool_ID));
                ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Plant_ID", "Plant_Name", mM_MT_Clita.Plant_ID);
                ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", mM_MT_Clita.Station_ID);
                ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
                ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Shop_ID", "Shop_Name", mM_MT_Clita.Shop_ID);
                ViewBag.Clita_Classification = new SelectList(db.MM_MT_Clita_Classification.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Classification", "Classification", mM_MT_Clita.Clita_Classification);
                ViewBag.Clita_Standard = new SelectList(db.MM_MT_Clita_Standard.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Standard", "Standard", mM_MT_Clita.Clita_Standard);
                ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Clita_Method_ID", "Method", mM_MT_Clita.Clita_Method_ID);
                ViewBag.Clita_Tools = new MultiSelectList(db.MM_MT_Clita_Tool.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Clita_Tool_ID", "Tool_Name", mmClitaObj.Select(x => x.Clita_Tool_ID).ToArray());
                return View(mMMMClita);
            }
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Maintenance_User_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee.Where(c => c.Plant_ID == plantId), "Employee_ID", "Employee_Name", mM_MT_Clita.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", mM_MT_Clita.Line_ID);
            ViewBag.Clita_Tool_ID = new MultiSelectList(db.MM_MT_Clita_Tool.Where(c => c.Plant_ID == plantId), "Clita_Tool_ID", "Tool_Name", mM_MT_Clita.MM_MT_ClitaItems_Tools_Relation.Select(a => a.Clita_Tool_ID));
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mM_MT_Clita.Plant_ID);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", mM_MT_Clita.Station_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Shop_ID == mM_MT_Clita.Shop_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
            ViewBag.Machine_Name = new SelectList(db.MM_MT_MTTUW_Machines.Where(c => c.Plant_ID == plantId && c.Shop_ID == mM_MT_Clita.Shop_ID).OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Clita.Machine_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(c => c.Plant_ID == plantId).OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Clita.Shop_ID);
            ViewBag.Clita_Tools = new MultiSelectList(db.MM_MT_Clita_Tool.Where(c => c.Plant_ID == mM_MT_Clita.Plant_ID), "Clita_Tool_ID", "Tool_Name", mmClitaObj.Select(x => x.Clita_Tool_ID).ToArray());
            ViewBag.Clita_Classification_ID = new SelectList(db.MM_MT_Clita_Classification.Where(c => c.Plant_ID == plantId), "Classification", "Classification", mM_MT_Clita.Clita_Classification);
            ViewBag.Clita_Standard_ID = new SelectList(db.MM_MT_Clita_Standard.Where(c => c.Plant_ID == plantId), "Standard", "Standard", mM_MT_Clita.Clita_Standard);
            ViewBag.Clita_Method_ID = new SelectList(db.MM_MT_Clita_Method.Where(c => c.Plant_ID == plantId), "Clita_Method_ID", "Method", mM_MT_Clita.Clita_Method_ID);
            return View(mM_MT_Clita);


        }
        #endregion

        #region Delete Clita Machine
        /*
      * Action Name          : Delete
      * Input Parameter      : None
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Delete the Clita items of machines (load details on form)
      */
        // GET: Clita/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Clita mM_MT_Clita = db.MM_MT_Clita.Find(id);
            if (mM_MT_Clita == null)
            {
                return HttpNotFound();
            }


            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Clita_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Clita Items";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.CLITA_Item + " " + ResourceGlobal.Form;

            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(mM_MT_Clita);
        }

        /*
     * Action Name          : Delete
     * Input Parameter      : ID
     * Return Type          : ActionResult
     * Author & Time Stamp  : Ajay Wagh
     * Description          : Delete the Clita items of machines
     */
        // POST: Clita/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_MT_Clita mM_MT_Clita = db.MM_MT_Clita.Find(id);
            try
            {
                //mM_MT_Clita.Is_Deleted = true;
                //mM_MT_Clita.Is_Edited = true;
                //db.Entry(mM_MT_Clita).State = EntityState.Modified;
                db.MM_MT_Clita.Remove(mM_MT_Clita);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CLITA_Item;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                ViewBag.GlobalDataModel = globalData;
                return RedirectToAction("Index");
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Clita_Config;
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
      * Author & Time Stamp  : Ajay Wagh
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
      * Author & Time Stamp  : Ajay Wagh
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
     * Author & Time Stamp  : Ajay Wagh
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
            var ClitaItems = db.MM_MT_Clita.Where(c => c.Machine_ID == machineid).Select(a => new { a.Clita_ID, a.Clita_Item }).OrderBy(x => x.Clita_Item);
            return Json(ClitaItems, JsonRequestBehavior.AllowGet);
        }

        /*
        * Action Name          : GetStationByLineID
        * Input Parameter      : id(lineid)
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
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
         * Author & Time Stamp  : Ajay Wagh
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

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getMachineList(decimal shopId)
        {
            try
            {
                var machineList = (from machine in db.MM_MT_MTTUW_Machines
                                   where machine.Shop_ID == shopId /*&& machine.IsActive == true*/
                                   select new
                                   {
                                       Id = machine.Machine_ID,
                                       Value = machine.Machine_Name
                                   }).Distinct();
                return Json(machineList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "ClitaController", "getMachineList(shopID : " + shopId + " )", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }
    }

}

