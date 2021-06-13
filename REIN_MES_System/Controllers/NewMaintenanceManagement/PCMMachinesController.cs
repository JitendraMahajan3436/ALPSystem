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
using System.IO;
using ZHB_AD.Controllers.BaseManagement;
using System.Data.Entity.Infrastructure;

namespace ZHB_AD.Controllers.NewMaintenanceManagement
{
    public class PCMMachinesController:BaseController
    {
          #region Varibales declaration
        private MVML_MGMTEntities db = new MVML_MGMTEntities();
        GlobalData globalData = new GlobalData();
        MM_MT_PCM_Machines mMMachineobj = new MM_MT_PCM_Machines();
        int plantId = 0, lineId = 0, lineTypeId = 0, shopId = 0;
        int mid = 0, eqp = 0;
        #endregion

        General generalObj = new General();

        #region Show details of machine (all machine or Specified Machine)
        /*
        * Action Name          : Index
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Get the list of machines added
        */

        // GET: Machines
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var MM_MT_PCM_Machines = db.MM_MT_PCM_Machines.Include(m => m.MM_Employee).Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops);
            var listScheduleMachine = (from t in db.MM_MT_Time_Based_Maintenance select t);

            foreach (var item in listScheduleMachine.ToList())
            {
                TimeSpan p = item.Scheduled_Date.Subtract(DateTime.Now);
                int diff = Convert.ToInt32(p.TotalDays);
            }
            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add+" "+ResourceDisplayName.Machine;
            globalData.contentFooter = ResourceGlobal.Add+" "+ResourceDisplayName.Machine;
            ViewBag.GlobalDataModel = globalData;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            return View(MM_MT_PCM_Machines.Where(p => p.Plant_ID == plantId).ToList());
        }

        /*
        * Action Name          : Details
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Get the value of machine of specified machine id
        */
        // GET: Machines/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_PCM_Machines MM_MT_PCM_Machines = db.MM_MT_PCM_Machines.Find(id);
            if (MM_MT_PCM_Machines == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceDisplayName.Machine + " " + ResourceGlobal.Config + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            //TempData["globalData"] = globalData;
            return View(MM_MT_PCM_Machines);
        }
        #endregion

        #region Create a Machine with respect to plant,shop,line,etc
        /*
        * Action Name          : Create
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action used to add new Machine under plant with shop and line 
        */
        // GET: Machines/Create
        public ActionResult Create()
        {
            var Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add+" "+ResourceDisplayName.Machine;
            globalData.contentFooter = ResourceGlobal.Add+" "+ResourceDisplayName.Machine;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", Plant_ID);

            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_PCM_Machine_Category, "Machine_Category_ID", "Category");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
            return View();
        }

        /*
        * Action Name          : Create
        * Input Parameter      : Object of MM_MT_PCM_Machines
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Create the Machine. Validate the Machine is already added or not with same configuration
        */
        //POST:(Machine with Model class) 
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Machine_ID,Machine_Number,Family,Machine_Name,Machine_Description,Plant_ID,Shop_ID,Line_ID,Station_ID,Machine_Category_ID,Manufaturing_Year,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_PCM_Machines MM_MT_PCM_Machines, HttpPostedFileBase files)
        {
            try
            {
                MM_MT_PCM_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

                if (ModelState.IsValid)
                {
                        if (files != null)
                        {
                            if (Path.GetExtension(files.FileName).ToLower() != ".pdf")
                            {
                                ModelState.AddModelError("FMEA_Document", "File Should be type of Pdf");
                            }
                            else if (Path.GetExtension(files.FileName).ToLower() == ".pdf")
                            {
                                MM_MT_PCM_Machines.FMEA_Document = Path.Combine(@"Content\FMEA_Documents\", MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName);
                                if (System.IO.File.Exists(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName)))
                                {
                                    System.IO.File.Delete(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName));
                                    files.SaveAs(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName));
                                }
                                else
                                {
                                    files.SaveAs(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName));
                                }
                            }
                        }

                        MM_MT_PCM_Machines.Inserted_Date = DateTime.Now;
                        MM_MT_PCM_Machines.Inserted_Host = HttpContext.Request.UserHostAddress;
                        MM_MT_PCM_Machines.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.MM_MT_PCM_Machines.Add(MM_MT_PCM_Machines);
                        db.SaveChanges();

                        globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                        globalData.subTitle = ResourceGlobal.Create;
                        globalData.controllerName = "Machines";
                        globalData.actionName = ResourceGlobal.Create;
                        globalData.contentTitle = ResourceGlobal.Add+" "+ResourceDisplayName.Machine;
                        globalData.contentFooter = ResourceGlobal.Add+" "+ResourceDisplayName.Machine;

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config; ;
                        globalData.messageDetail = ResourceDisplayName.Machine+" "+ResourceMessages.Add_Success;

                        ViewBag.GlobalDataModel = globalData;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine+" "+ResourceGlobal.Config;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
            }

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", MM_MT_PCM_Machines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", MM_MT_PCM_Machines.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", MM_MT_PCM_Machines.Line_ID);
            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_PCM_Machine_Category, "Machine_Category_ID", "Category", MM_MT_PCM_Machines.Machine_Category_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(x => x.Plant_ID == MM_MT_PCM_Machines.Plant_ID).ToList().OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", MM_MT_PCM_Machines.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", MM_MT_PCM_Machines.Station_ID);
            return View(MM_MT_PCM_Machines);
        }
        #endregion

        #region Update Child DropDown with respect to plant and Shop

        /*
        * Action Name          : GetShopByPlantID
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action used to Get all shop under plant
        */
        //Update shop with respect to plant
        public ActionResult GetShopByPlantID(int plantid)
        {
            var Shops = db.MM_Shops.Where(c => c.Plant_ID == plantid).Select(a => new { a.Shop_ID, a.Shop_Name });
            return Json(Shops, JsonRequestBehavior.AllowGet);
        }

        /*
        * Action Name          : GetLineByShopID
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action used to Get all line under Shop
        */
        //Update Line with respective shop
        public ActionResult GetLineByShopID(int shopid)
        {
            var Lines = db.MM_Lines.Where(c => c.Shop_ID == shopid).Select(a => new { a.Line_ID, a.Line_Name }).OrderBy(x => x.Line_Name);
            return Json(Lines, JsonRequestBehavior.AllowGet);
        }

        //Update Stations with respective line
        public ActionResult GetStationsByLineID(int lineid)
        {
            var Stations = db.MM_Stations.Where(c => c.Line_ID == lineid).Select(a => new { a.Station_ID, a.Station_Name }).OrderBy(x => x.Station_Name);
            return Json(Stations, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit Details with respect specified machine
        /*
        * Action Name          : Edit
        * Input Parameter      : Machine id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Show the edit Machine form
        */
        // GET: Machines/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_PCM_Machines MM_MT_PCM_Machines = db.MM_MT_PCM_Machines.FirstOrDefault(x => x.Machine_ID == id);
            if (MM_MT_PCM_Machines == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;

            ViewBag.GlobalDataModel = globalData;
            //   TempData["globalData"] = globalData;

            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(a => a.Line_ID == MM_MT_PCM_Machines.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", MM_MT_PCM_Machines.Station_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", MM_MT_PCM_Machines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", MM_MT_PCM_Machines.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(a => a.Shop_ID == MM_MT_PCM_Machines.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", MM_MT_PCM_Machines.Line_ID);
            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_PCM_Machine_Category, "Machine_Category_ID", "Category", MM_MT_PCM_Machines.Machine_Category_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", MM_MT_PCM_Machines.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", MM_MT_PCM_Machines.Shop_ID);
            return View(MM_MT_PCM_Machines);
        }

        /*
        * Action Name          : Edit
        * Input Parameter      : MM_Lachines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action is used to edit the Machine
        */
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Machine_ID,Machine_Number,Machine_Name,Family,Machine_Description,Plant_ID,Shop_ID,Line_ID,Station_ID,Machine_Category_ID,Manufaturing_Year,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_PCM_Machines MM_MT_PCM_Machines, HttpPostedFileBase files)
        {
            try
            {
                MM_MT_PCM_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                if (ModelState.IsValid)
                {
                        mMMachineobj = db.MM_MT_PCM_Machines.FirstOrDefault(x => x.Machine_ID == MM_MT_PCM_Machines.Machine_ID);
                    if (files != null)
                    {
                        if (Path.GetExtension(files.FileName).ToLower() != ".pdf")
                        {
                            ModelState.AddModelError("FMEA_Document", "File Should be type of Pdf");
                        }
                        else if (Path.GetExtension(files.FileName).ToLower() == ".pdf")
                        {
                            mMMachineobj.FMEA_Document = Path.Combine(@"Content\FMEA_Documents\", MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName);
                            if (System.IO.File.Exists(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName)))
                            {
                                System.IO.File.Delete(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName));
                                files.SaveAs(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName));
                            }
                            else
                            {
                                files.SaveAs(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), MM_MT_PCM_Machines.Machine_Number + "_" + files.FileName));
                            }

                            mMMachineobj.Machine_Name = MM_MT_PCM_Machines.Machine_Name;
                            mMMachineobj.Machine_Number = MM_MT_PCM_Machines.Machine_Number;
                            mMMachineobj.Family = MM_MT_PCM_Machines.Family;
                            mMMachineobj.Machine_Description = MM_MT_PCM_Machines.Machine_Description;
                            mMMachineobj.Is_Edited = true;
                            mMMachineobj.Updated_Date = DateTime.Now;
                            mMMachineobj.Updated_Host = HttpContext.Request.UserHostAddress;
                            mMMachineobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.Entry(mMMachineobj).State = EntityState.Modified;
                            db.SaveChanges();
                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                            globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Edit_Success;
                            ViewBag.GlobalDataModel = globalData;
                            TempData["globalData"] = globalData;
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        mMMachineobj.Machine_Name = MM_MT_PCM_Machines.Machine_Name;
                        mMMachineobj.Machine_Number = MM_MT_PCM_Machines.Machine_Number;
                        mMMachineobj.Family = MM_MT_PCM_Machines.Family;
                        mMMachineobj.Station_ID = MM_MT_PCM_Machines.Station_ID;
                        mMMachineobj.Machine_Category_ID = MM_MT_PCM_Machines.Machine_Category_ID;
                        mMMachineobj.Manufaturing_Year = MM_MT_PCM_Machines.Manufaturing_Year;
                        mMMachineobj.Machine_Description = MM_MT_PCM_Machines.Machine_Description;
                        mMMachineobj.Is_Edited = true;
                        mMMachineobj.Updated_Date = DateTime.Now;
                        mMMachineobj.Updated_Host = HttpContext.Request.UserHostAddress;
                        mMMachineobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                        db.Entry(mMMachineobj).State = EntityState.Modified;
                        db.SaveChanges();

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                        globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Edit_Success;
                        ViewBag.GlobalDataModel = globalData;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine+" "+ResourceGlobal.Config;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
            }

            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceDisplayName.Machine+" "+ResourceMessages.Edit_Success;
            globalData.contentFooter = ResourceDisplayName.Machine+" "+ResourceMessages.Edit_Success;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            //ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", MM_MT_PCM_Machines.Station_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(a => a.Line_ID == MM_MT_PCM_Machines.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", MM_MT_PCM_Machines.Station_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", MM_MT_PCM_Machines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", MM_MT_PCM_Machines.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(a => a.Shop_ID == MM_MT_PCM_Machines.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", MM_MT_PCM_Machines.Line_ID);
            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_PCM_Machine_Category, "Machine_Category_ID", "Category", MM_MT_PCM_Machines.Machine_Category_ID);
            // ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", MM_MT_PCM_Machines.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", MM_MT_PCM_Machines.Shop_ID);
            return View(MM_MT_PCM_Machines);
        }
        #endregion

        #region Delete Specified Machine
        /*
        * Action Name          : Delete
        * Input Parameter      : (id) Machine id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action is used to edit the machine
        */
        // GET: Machines/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_PCM_Machines MM_MT_PCM_Machines = db.MM_MT_PCM_Machines.Find(id);
            if (MM_MT_PCM_Machines == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete+" "+ResourceDisplayName.Machine+" "+ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete+" "+ResourceDisplayName.Machine+" "+ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(MM_MT_PCM_Machines);
        }

        /*
        * Action Name          : Delete
        * Input Parameter      : (id) Machine id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action is used to  Confirmed user to delete the machine
        */
        // POST: Machines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            //bool flag = false;
            MM_MT_PCM_Machines MM_MT_PCM_Machines = db.MM_MT_PCM_Machines.Where(x => x.Machine_ID == id).FirstOrDefault();
            try
            {

                db.MM_MT_PCM_Machines.Remove(MM_MT_PCM_Machines);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_MT_PCM_Machines", "Machine_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "Machines";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceDisplayName.Machine + " " + ResourceMessages.Delete_Success;
                globalData.contentFooter =  ResourceDisplayName.Machine + " " + ResourceMessages.Delete_Success;

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine+" "+ResourceGlobal.Config;
                globalData.messageDetail =  ResourceDisplayName.Machine + " " + ResourceMessages.Delete_Success;

                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(DbUpdateException))
                {

                    globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                    globalData.subTitle = ResourceGlobal.Delete;
                    globalData.controllerName = "Machines";
                    globalData.actionName = ResourceGlobal.Delete;
                    globalData.contentTitle = ResourceGlobal.Delete+" "+ResourceDisplayName.Machine+" "+ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Delete+" "+ResourceDisplayName.Machine+" "+ResourceGlobal.Form;
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "Machine Deletion Error";
                    globalData.messageDetail = "Can not remove Machine because machine has other references so please first remove reference of machine then try again.";
                    ViewBag.GlobalDataModel = globalData;
                    TempData["globalData"] = globalData;
                    return View(MM_MT_PCM_Machines);
                }
                else
                {

                    globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                    globalData.subTitle = ResourceGlobal.Delete;
                    globalData.controllerName = "Machines";
                    globalData.actionName = ResourceGlobal.Delete;
                    globalData.contentTitle = ResourceGlobal.Delete+" "+ResourceDisplayName.Machine+" "+ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Delete+" "+ResourceDisplayName.Machine+" "+ResourceGlobal.Form;

                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "Machine Deletion Error";
                    globalData.messageDetail = ex.Message.ToString();
                    ViewBag.GlobalDataModel = globalData;
                    TempData["globalData"] = globalData;
                    return View(MM_MT_PCM_Machines);
                }
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Disposing Objects
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Upload Machine from Excel File
        /*
        * Action Name          : Upload
        * Input Parameter      : Datatable and MM_MT_PCM_Machines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action used to show upload form
        */
        //GET: Upload file page load
        public ActionResult Upload()
        {
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");

            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_PCM_Machine_Category, "Machine_category_ID", "Category");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");

            if (TempData["MachineRecords"] != null)
            {
                ViewBag.MachineRecords = TempData["MachineRecords"];
                ViewBag.MachineDataTable = TempData["MachineDataTable"];
            }

            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Upload;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Upload;
            globalData.contentTitle = ResourceGlobal.Add+" "+ResourceDisplayName.Machine;
            globalData.contentFooter = ResourceGlobal.Add+" "+ResourceDisplayName.Machine;

            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }
        /*
        * Action Name          : Upload
        * Input Parameter      : Files(file object) and MM_MT_PCM_Machines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action used get upload excel file and generationg datatable
        */

        //GET: GET The file from upload control 
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(HttpPostedFileBase files, [Bind(Include = "Machine_ID,Machine_Number,Machine_Name,Machine_Description,Plant_ID,Shop_ID,Line_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_PCM_Machines mM_MT_Machine)
        {
            try
            {
                GlobalOperations globalOperations = new GlobalOperations();
                string fileName = Path.GetFileName(files.FileName);
                string fileExtension = Path.GetExtension(files.FileName);
                string fileLocation = Server.MapPath("~/App_Data/" + fileName);

                DataTable dt = globalOperations.ExcelToDataTable(files, fileLocation, fileExtension);

                if (dt.Rows.Count > 0)
                {

                    MachineRecords[] machineRecords = new MachineRecords[dt.Rows.Count];
                    int i = 0;
                    foreach (DataRow mchineList in dt.Rows)
                    {
                        MM_MT_PCM_Machines MM_MT_PCM_Machines = new MM_MT_PCM_Machines();
                        MM_MT_PCM_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        string machinename = mchineList["Machine_Name"].ToString().Trim();
                        string machineNumber = mchineList["Machine_Number"].ToString().Trim();
                        string machineDes = mchineList["Machine_Description"].ToString().Trim();
                        string Shop = mchineList["Shop_Name"].ToString().Trim();
                        string Line = mchineList["Line_Name"].ToString().Trim();
                        string manufacturing_yerar = mchineList["Manufacturing_Year"].ToString().Trim();
                        string machine_category = mchineList["Machine_Category"].ToString().Trim();
                        decimal shop_ID = db.MM_Shops.Where(x => x.Shop_Name.ToLower() == Shop.ToLower()).FirstOrDefault().Shop_ID;
                        decimal line_ID = db.MM_Lines.Where(x => x.Line_Name.ToLower() == Line.ToLower()).FirstOrDefault().Line_ID;
                        decimal category_ID = db.MM_MT_PCM_Machine_Category.Where(x => x.Category.ToLower() == machine_category.ToLower()).FirstOrDefault().Machine_Category_ID;
                        MachineRecords mmrecordobj = new MachineRecords();
                        mmrecordobj.machineName = machinename;
                        mmrecordobj.machineNumber = machineNumber;
                        mmrecordobj.machineDescription = machineDes;
                        if (machinename == "" && machinename != null)
                        {
                            mmrecordobj.MachineListError = "Machine can not be null";
                        }
                        else if (MM_MT_PCM_Machines.isMachineExists(machineNumber, 0, MM_MT_PCM_Machines.Plant_ID))
                        {
                            mmrecordobj.MachineListError = "Machine Name is already Exists";
                        }
                        else
                        {
                            MM_MT_PCM_Machines.Machine_Name = Convert.ToString(machinename);
                            MM_MT_PCM_Machines.Machine_Number = Convert.ToString(machineNumber);
                            MM_MT_PCM_Machines.Machine_Description = Convert.ToString(machineDes);
                            MM_MT_PCM_Machines.Manufaturing_Year = Convert.ToDateTime(manufacturing_yerar);
                            MM_MT_PCM_Machines.Machine_Category_ID = category_ID;
                            MM_MT_PCM_Machines.Shop_ID = shop_ID;
                            MM_MT_PCM_Machines.Line_ID = line_ID;
                            MM_MT_PCM_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                            MM_MT_PCM_Machines.Inserted_Date = DateTime.Now;
                            MM_MT_PCM_Machines.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                            MM_MT_PCM_Machines.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.MM_MT_PCM_Machines.Add(MM_MT_PCM_Machines);
                            db.SaveChanges();
                            mmrecordobj.MachineListError = "Machine is added successfully";
                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceDisplayName.Machine+" "+ResourceGlobal.Config;
                            globalData.messageDetail = ResourceDisplayName.Machine+" "+ResourceMessages.Add_Success;
                            ViewBag.GlobalDataModel = globalData;

                        }
                        machineRecords[i] = mmrecordobj;
                        i = i + 1;
                    }
                    globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                    globalData.subTitle = ResourceGlobal.Create;
                    globalData.controllerName = "Machines";
                    globalData.actionName = ResourceGlobal.Create;
                    globalData.contentTitle = ResourceDisplayName.Machine + " " + ResourceMessages.Add_Success;
                    globalData.contentFooter = ResourceDisplayName.Machine + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    TempData["MachineRecords"] = machineRecords;
                    TempData["MachineDataTable"] = dt;
                    ViewBag.machineRecords = machineRecords;
                    ViewBag.GlobalDataModel = globalData;
                }
                ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
                ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_PCM_Machine_Category, "Machine_category_ID", "Category");
                ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
                ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
                ViewBag.GlobalDataModel = globalData;
            }
            catch (Exception ex)
            {

                globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Machines";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceDisplayName.Machine + " " + ResourceMessages.Add_Success;
                globalData.contentFooter = ResourceDisplayName.Machine + " " + ResourceMessages.Add_Success;

                globalData.isErrorMessage = true;
                if (files == null)
                {
                    globalData.messageTitle = ResourceDisplayName.Machine+" "+ResourceGlobal.Config;
                    globalData.messageDetail = "Please upload file, file can not be empty.";
                }
                else
                {
                    globalData.messageTitle = ResourceDisplayName.Machine+" "+ResourceGlobal.Config;
                    globalData.messageDetail = ex.Message.ToString();
                }
                ViewBag.GlobalDataModel = globalData;
            }
            return View();
        }
        #endregion

        #region Download File
        //public ActionResult Download(decimal id)
        //{
        //    MM_MT_PCM_Machines mM_Machines = db.MM_MT_PCM_Machines.Find(id);
        //    WebClient wbclient = new WebClient();
        //    if (mM_Machines.FMEA_Document != null)
        //    {
        //        wbclient.DownloadFile(mM_Machines.FMEA_Document, Path.GetFileName(mM_Machines.FMEA_Document));
        //    }
        //    else
        //    {
        //        globalData.isErrorMessage = true;
        //        globalData.messageTitle = "Error in Dwonload";
        //        globalData.messageDetail = "FMEA File is not present for this machine";
        //        ViewBag.GlobalDataModel = globalData;
        //    }
        //    return RedirectToAction("Index");
        //}

        public FileResult Download(decimal id)
        {
            MM_MT_PCM_Machines mM_Machines = db.MM_MT_PCM_Machines.Find(id);
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath(@"~/" + mM_Machines.FMEA_Document)));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(mM_Machines.FMEA_Document));
        }
        #endregion

        #region Insert all Data of DataTable into respective Database Table
        /*
        * Action Name          : InsertIntoDataTable
        * Input Parameter      : Datatable and MM_MT_PCM_Machines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Vijaykumar Kagne
        * Description          : Action used to insert datatable data into database table  
        */
        private bool InsertIntoDataTable(DataTable dt, MM_MT_PCM_Machines MM_MT_PCM_Machines)
        {
            bool isstatus = false;
            try
            {
                //if (ModelState.IsValid)
                //{

                foreach (DataRow dr in dt.Rows)
                {
                    string machinename = dr["Machine_Name"].ToString().Trim();
                    string machineNumber = dr["Machine_Number"].ToString().Trim();
                    string machineDes = dr["Machine_Description"].ToString().Trim();
                    string Shop = dr["Shop_Name"].ToString().Trim();
                    string Line = dr["Line_Name"].ToString().Trim();
                    string manufacturing_yerar = dr["Manufacturing_Year"].ToString().Trim();
                    string machine_category = dr["Machine_Category"].ToString().Trim();

                    decimal shop_ID = db.MM_Shops.Where(x => x.Shop_Name.ToLower() == Shop.ToLower()).FirstOrDefault().Shop_ID;
                    decimal line_ID = db.MM_Lines.Where(x => x.Line_Name.ToLower() == Line.ToLower()).FirstOrDefault().Line_ID;
                    decimal category_ID = db.MM_MT_PCM_Machine_Category.Where(x => x.Category.ToLower() == machine_category.ToLower()).FirstOrDefault().Machine_Category_ID;
                    if (db.MM_MT_PCM_Machines.Where(x => x.Machine_Number.ToLower() == machineNumber.ToLower()).Count() > 0)
                    {
                        ModelState.AddModelError("Machine_Number", ResourceValidation.Exist);
                        return isstatus;
                    }
                    else
                    {

                        MM_MT_PCM_Machines.Machine_Name = Convert.ToString(dr["Machine_Name"].ToString().Trim());
                        MM_MT_PCM_Machines.Machine_Number = Convert.ToString(dr["Machine_Number"].ToString().Trim());
                        MM_MT_PCM_Machines.Machine_Description = Convert.ToString(dr["Machine_Description"].ToString().Trim());
                        MM_MT_PCM_Machines.Manufaturing_Year = Convert.ToDateTime(manufacturing_yerar);
                        MM_MT_PCM_Machines.Machine_Category_ID = category_ID;
                        MM_MT_PCM_Machines.Shop_ID = shop_ID;
                        MM_MT_PCM_Machines.Line_ID = line_ID;
                        MM_MT_PCM_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        MM_MT_PCM_Machines.Inserted_Date = DateTime.Now;
                        MM_MT_PCM_Machines.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                        MM_MT_PCM_Machines.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.MM_MT_PCM_Machines.Add(MM_MT_PCM_Machines);
                        db.SaveChanges();
                        return isstatus = true;
                    }
                    // }
                }
            }
            catch (Exception ex)
            {
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine+" "+ResourceGlobal.Config;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
            }
            return isstatus;
        }
        #endregion
    }
}
