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

namespace ZHB_AD.Controllers
{
    /* Class Name                : MachinesController
   *  Description                : To create,edit,delete and show all machines 
   *  Author, Timestamp          : Ajay Wagh       
   */

    public class MachinesController : BaseController
    {
        #region Varibales declaration
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        private REIN_SOLUTION_MEntities MTTUWdb = new REIN_SOLUTION_MEntities();
        MM_Machine_Shift_Data mMMachineob = new MM_Machine_Shift_Data();
        MM_Machine_Image mm_img1 = new MM_Machine_Image();
        MM_MT_MTTUW_Machines MTMachine = new MM_MT_MTTUW_Machines();
        GlobalData globalData = new GlobalData();
        MM_MT_Machines mMMachineobj = new MM_MT_Machines();
        int plantId = 0, lineId = 0, lineTypeId = 0, shopId = 0;
        int mid = 0, eqp = 0;
        #endregion

        General generalObj = new General();

        #region Show details of machine (all machine or Specified Machine)
        /*
        * Action Name          : Index
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Get the list of machines added
        */

        // GET: Machines
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var mM_MT_Machines = db.MM_MT_Machines.Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops);
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
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceDisplayName.Machine;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceDisplayName.Machine;
            ViewBag.GlobalDataModel = globalData;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            return View(mM_MT_Machines.Where(p => p.Plant_ID == plantId).ToList());
        }

        /*
        * Action Name          : Details
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Get the value of machine of specified machine id
        */
        // GET: Machines/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Machines mM_MT_Machines = db.MM_MT_Machines.Find(id);
            if (mM_MT_Machines == null)
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
            return View(mM_MT_Machines);
        }
        #endregion

        #region Create a Machine with respect to plant,shop,line,etc
        /*
        * Action Name          : Create
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action used to add new Machine under plant with shop and line 
        */
        // GET: Machines/Create
        public ActionResult Create()
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceDisplayName.Machine;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceDisplayName.Machine;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", plantId);

            ViewBag.M_Type_ID = new SelectList(db.MM_MT_Machine_Type, "M_Type_ID", "Machine_Type");
            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_Machine_Category, "Machine_Category_ID", "Category");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
            return View();
        }

        /*
        * Action Name          : Create
        * Input Parameter      : Object of MM_MT_Machines
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Create the Machine. Validate the Machine is already added or not with same configuration
        */
        //POST:(Machine with Model class) 
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_MT_Machines mM_MT_Machines, HttpPostedFileBase[] files, HttpPostedFileBase upload)
        {
            try
            {
                mM_MT_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                bool isvalid = true;
                if (ModelState.IsValid)
                {
                    if (mM_MT_Machines.Machine_Name == mM_MT_Machines.Machine_Description)
                    {
                        ModelState.AddModelError("Machine_Description", "Machine Description can not be same as Machine Name");
                        isvalid = false;
                    }
                    if (mM_MT_Machines.Machine_Model == mM_MT_Machines.Machine_Make)
                    {
                        ModelState.AddModelError("Machine_Model", "Machine Model can not be same as Machine Make");
                        isvalid = false;
                    }
                    if (isvalid == true)
                    {


                        mM_MT_Machines.Inserted_Date = DateTime.Now;
                        mM_MT_Machines.Inserted_Host = HttpContext.Request.UserHostAddress;
                        mM_MT_Machines.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.MM_MT_Machines.Add(mM_MT_Machines);
                        db.SaveChanges();

                        MM_Machine_Image mm_image = new MM_Machine_Image();
                        if (upload != null && upload.ContentLength > 0)
                        {
                            using (var reader = new System.IO.BinaryReader(upload.InputStream))
                            {

                                mm_image.Image_Name = System.IO.Path.GetFileName(upload.FileName);
                                mm_image.Image_Type = Path.GetExtension(upload.FileName);
                                mm_image.Content_Type = upload.ContentType;
                                mm_image.Image_Content = reader.ReadBytes(upload.ContentLength);
                                mm_image.Inserted_Date = DateTime.Now;
                                mm_image.Inserted_Host = HttpContext.Request.UserHostAddress;
                                mm_image.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                mm_image.Machine_ID = mM_MT_Machines.Machine_ID;
                                db.MM_Machine_Image.Add(mm_image);
                                db.SaveChanges();
                            }
                        }










                        //Insert records into MTTUW machines tables
                        MTMachine.Family = mM_MT_Machines.Family;
                        MTMachine.IsActive = mM_MT_Machines.IsActive;
                        MTMachine.Machine_Make = mM_MT_Machines.Machine_Make;
                        MTMachine.Machine_Model = mM_MT_Machines.Machine_Model;
                        MTMachine.M_Type_ID = mM_MT_Machines.M_Type_ID;
                        MTMachine.Mail_Trigger_In = mM_MT_Machines.Mail_Trigger_In;
                        MTMachine.FMEA_Document = mM_MT_Machines.Family;
                        MTMachine.Inserted_Date = mM_MT_Machines.Inserted_Date;
                        MTMachine.Inserted_Host = mM_MT_Machines.Inserted_Host;
                        MTMachine.Inserted_User_ID = mM_MT_Machines.Inserted_User_ID;
                        MTMachine.Is_Deleted = mM_MT_Machines.Is_Deleted;
                        MTMachine.Is_Edited = mM_MT_Machines.Is_Edited;
                        MTMachine.Is_Purgeable = mM_MT_Machines.Is_Purgeable;
                        MTMachine.Is_Status_Machine = mM_MT_Machines.Is_Status_Machine;
                        MTMachine.Is_Transfered = mM_MT_Machines.Is_Transfered;
                        MTMachine.Line_ID = mM_MT_Machines.Line_ID;
                        MTMachine.Machine_Category_ID = mM_MT_Machines.Machine_Category_ID;
                        MTMachine.Machine_Description = mM_MT_Machines.Machine_Description;
                        MTMachine.Machine_ID = mM_MT_Machines.Machine_ID;
                        MTMachine.Machine_Name = mM_MT_Machines.Machine_Name;
                        MTMachine.Machine_Number = mM_MT_Machines.Machine_Number;
                        MTMachine.Manufaturing_Year = mM_MT_Machines.Manufaturing_Year;
                        MTMachine.Plant_ID = mM_MT_Machines.Plant_ID;
                        MTMachine.Shop_ID = mM_MT_Machines.Shop_ID;
                        MTMachine.Station_ID = mM_MT_Machines.Station_ID;

                        MTMachine.IsCBM = mM_MT_Machines.IsCBM;
                        MTMachine.IsTBM = mM_MT_Machines.IsTBM;
                        MTMachine.IsMinorStoppage = mM_MT_Machines.IsMinorStoppage;
                        MTMachine.CBM_Matrix1 = mM_MT_Machines.CBM_Matrix1;
                        MTMachine.CBM_Matrix2 = mM_MT_Machines.CBM_Matrix2;
                        MTMachine.CBM_Matrix3 = mM_MT_Machines.CBM_Matrix3;
                        MTMachine.CBM_Email1 = mM_MT_Machines.CBM_Email1;
                        MTMachine.CBM_Email2 = mM_MT_Machines.CBM_Email2;
                        MTMachine.CBM_Email3 = mM_MT_Machines.CBM_Email3;
                        MTMachine.CBM_SMS1 = mM_MT_Machines.CBM_SMS1;
                        MTMachine.CBM_SMS2 = mM_MT_Machines.CBM_SMS2;
                        MTMachine.CBM_SMS3 = mM_MT_Machines.CBM_SMS3;
                        MTMachine.TBM_Matrix1 = mM_MT_Machines.TBM_Matrix1;
                        MTMachine.TBM_Matrix2 = mM_MT_Machines.TBM_Matrix2;
                        MTMachine.TBM_Matrix3 = mM_MT_Machines.TBM_Matrix3;
                        MTMachine.TBM_Email1 = mM_MT_Machines.TBM_Email1;
                        MTMachine.TBM_Email2 = mM_MT_Machines.TBM_Email2;
                        MTMachine.TBM_Email3 = mM_MT_Machines.TBM_Email3;
                        MTMachine.TBM_SMS1 = mM_MT_Machines.TBM_SMS1;
                        MTMachine.TBM_SMS2 = mM_MT_Machines.TBM_SMS2;
                        MTMachine.TBM_SMS3 = mM_MT_Machines.TBM_SMS3;
                        MTMachine.MS_Matrix1 = mM_MT_Machines.MS_Matrix1;
                        MTMachine.MS_Matrix2 = mM_MT_Machines.MS_Matrix2;
                        MTMachine.MS_Matrix3 = mM_MT_Machines.MS_Matrix3;
                        MTMachine.MS_Occurences1 = mM_MT_Machines.MS_Occurences1;
                        MTMachine.MS_Occurences2 = mM_MT_Machines.MS_Occurences2;
                        MTMachine.MS_Occurences3 = mM_MT_Machines.MS_Occurences3;
                        MTMachine.MS_Email1 = mM_MT_Machines.MS_Email1;
                        MTMachine.MS_Email2 = mM_MT_Machines.MS_Email2;
                        MTMachine.MS_Email3 = mM_MT_Machines.MS_Email3;
                        MTMachine.MS_SMS1 = mM_MT_Machines.MS_SMS1;
                        MTMachine.MS_SMS2 = mM_MT_Machines.MS_SMS2;
                        MTMachine.MS_SMS3 = mM_MT_Machines.MS_SMS3;

                        MTTUWdb.MM_MT_MTTUW_Machines.Add(MTMachine);
                        MTTUWdb.SaveChanges();
                        if (files[0] != null)
                        {
                            //if (Path.GetExtension(files.FileName).ToLower() != ".pdf")
                            //{
                            //    ModelState.AddModelError("FMEA_Document", "File Should be type of Pdf");
                            //}
                            //else if (Path.GetExtension(files.FileName).ToLower() == ".pdf")
                            //{
                            //    mM_MT_Machines.FMEA_Document = Path.Combine(@"Content\FMEA_Documents\", mM_MT_Machines.Machine_Number + "_" + files.FileName);
                            //    if (System.IO.File.Exists(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), mM_MT_Machines.Machine_Number + "_" + files.FileName)))
                            //    {
                            //        System.IO.File.Delete(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), mM_MT_Machines.Machine_Number + "_" + files.FileName));
                            //        files.SaveAs(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), mM_MT_Machines.Machine_Number + "_" + files.FileName));
                            //    }
                            //    else
                            //    {
                            //        files.SaveAs(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), mM_MT_Machines.Machine_Number + "_" + files.FileName));
                            //    }
                            //}
                            MM_MT_Machine_Document mM_MT_Machine_Document = new MM_MT_Machine_Document();
                            if (files.Length > 10)
                            {
                                ModelState.AddModelError("FMEA_Document", "Upto 10 files are uploaded for one machine");
                            }
                            else
                            {
                                foreach (var file in files)
                                {
                                    if (file != null && file.ContentLength > 0)
                                    {
                                        using (var reader = new System.IO.BinaryReader(file.InputStream))
                                        {
                                            mM_MT_Machine_Document.Document_Name = System.IO.Path.GetFileName(file.FileName);
                                            mM_MT_Machine_Document.Document_Type = Path.GetExtension(file.FileName);
                                            mM_MT_Machine_Document.Content_Type = file.ContentType;
                                            mM_MT_Machine_Document.Document_Content = reader.ReadBytes(file.ContentLength);
                                            mM_MT_Machine_Document.Machine_ID = mM_MT_Machines.Machine_ID;
                                            mM_MT_Machine_Document.Inserted_Date = DateTime.Now;
                                            mM_MT_Machine_Document.Inserted_Host = HttpContext.Request.UserHostAddress;
                                            mM_MT_Machine_Document.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                            db.MM_MT_Machine_Document.Add(mM_MT_Machine_Document);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        //End
                        globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                        globalData.subTitle = ResourceGlobal.Create;
                        globalData.controllerName = "Machines";
                        globalData.actionName = ResourceGlobal.Create;
                        globalData.contentTitle = ResourceGlobal.Add + " " + ResourceDisplayName.Machine;
                        globalData.contentFooter = ResourceGlobal.Add + " " + ResourceDisplayName.Machine;

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config; ;
                        globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Add_Success;

                        ViewBag.GlobalDataModel = globalData;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
            }

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Machines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Machines.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_MT_Machines.Line_ID);
            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_Machine_Category, "Machine_Category_ID", "Category", mM_MT_Machines.Machine_Category_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Machines.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(x => x.Plant_ID == mM_MT_Machines.Plant_ID).ToList().OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Machines.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_MT_Machines.Station_ID);
            ViewBag.M_Type_ID = new SelectList(db.MM_MT_Machine_Type, "M_Type_ID", "Machine_Type", mM_MT_Machines.M_Type_ID);
            return View(mM_MT_Machines);
        }
        #endregion



        #region Update Child DropDown with respect to plant and Shop

        /*
        * Action Name          : GetShopByPlantID
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
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
        * Author & Time Stamp  : Ajay Wagh
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
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Show the edit Machine form
        */
        // GET: Machines/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Machines mM_MT_Machines = db.MM_MT_Machines.FirstOrDefault(x => x.Machine_ID == id);
            if (mM_MT_Machines == null)
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

            var machineimage = db.MM_Machine_Image.Where(m => m.Machine_ID == id).Select(m => m.Image_Name).FirstOrDefault();
            if (machineimage != null)
            {
                ViewBag.FileNameImage = machineimage;
            }
            else
            {
                ViewBag.FileNameImage = "Image not uploaded ";
            }
            var ShopID = mM_MT_Machines.Shop_ID;
            List<MM_Shift> ShiftList = db.MM_Shift.Where(m => m.Shop_ID == ShopID).Select(m => m).ToList();
            ViewBag.Shifts = ShiftList;

            var filename = (from f in db.MM_MT_Machine_Document
                            where f.Machine_ID == id
                            select f.Document_Name).ToList();
            ViewBag.FileName = filename.Count + " files exists";
            ViewBag.FileCount = filename.Count;
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(a => a.Line_ID == mM_MT_Machines.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", mM_MT_Machines.Station_ID);
            //ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Machines.Inserted_User_ID);
            //ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Machines.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(a => a.Shop_ID == mM_MT_Machines.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", mM_MT_Machines.Line_ID);
            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_Machine_Category, "Machine_Category_ID", "Category", mM_MT_Machines.Machine_Category_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Machines.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Machines.Shop_ID);
            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.Select(c => new { Machine_Name = c.Machine_Name + "(" + c.Machine_Number + ")", Machine_Number = c.Machine_Number }), "Machine_Number", "Machine_Name", mM_MT_Machines.Machine_Number);
            ViewBag.M_Type_ID = new SelectList(db.MM_MT_Machine_Type, "M_Type_ID", "Machine_Type", mM_MT_Machines.M_Type_ID);

            ///            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.Where(x => x.Machine_Number == mM_MT_Machines.Machine_Number && x.Machine_Name == mM_MT_MachinesMachine_Name.== mM_MT_Machines.Machine_Name + "(" + mM_MT_Machines.Machine_Number + ")" + "," + "Machine_Number" + "," + "Machine_Name" + "," + mM_MT_Machines.Machine_Name));
            return View(mM_MT_Machines);
        }

        /*
        * Action Name          : Edit
        * Input Parameter      : MM_Lachines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action is used to edit the Machine
        */
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_MT_Machines mM_MT_Machines, HttpPostedFileBase[] files, HttpPostedFileBase upload)
        {
            try
            {
                mM_MT_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                bool isvalid = true;
                if (ModelState.IsValid)
                {
                    mMMachineobj = db.MM_MT_Machines.FirstOrDefault(x => x.Machine_ID == mM_MT_Machines.Machine_ID);
                    if (mM_MT_Machines.Machine_Name == mM_MT_Machines.Machine_Description)
                    {
                        ModelState.AddModelError("Machine_Description", "Machine Description can not be same as Machine Name");
                        isvalid = false;
                    }
                    if (mM_MT_Machines.Machine_Make == mM_MT_Machines.Machine_Model)
                    {
                        ModelState.AddModelError("Machine_Model", "Machine Model can not be same as Machine Make");
                        isvalid = false;
                    }
                    if (isvalid == true)
                    {
                        if (files[0] != null)
                        {

                            //mMMachineobj.FMEA_Document = Path.Combine(@"Content\FMEA_Documents\", mM_MT_Machines.Machine_Number + "_" + files.FileName);
                            //if (System.IO.File.Exists(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), mM_MT_Machines.Machine_Number + "_" + files.FileName)))
                            //{
                            //    System.IO.File.Delete(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), mM_MT_Machines.Machine_Number + "_" + files.FileName));
                            //    files.SaveAs(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), mM_MT_Machines.Machine_Number + "_" + files.FileName));
                            //}
                            //else
                            //{
                            //    files.SaveAs(Path.Combine(Server.MapPath(@"~\Content\FMEA_Documents\"), mM_MT_Machines.Machine_Number + "_" + files.FileName));
                            //}
                            mMMachineobj.Shop_ID = mM_MT_Machines.Shop_ID;
                            mMMachineobj.Line_ID = mM_MT_Machines.Line_ID;
                            mMMachineobj.Station_ID = mM_MT_Machines.Station_ID;
                            mMMachineobj.Machine_Name = mM_MT_Machines.Machine_Name;
                            mMMachineobj.Machine_Number = mM_MT_Machines.Machine_Number;
                            //mMMachineobj.Family = mM_MT_Machines.Family;
                            mMMachineobj.M_Type_ID = mM_MT_Machines.M_Type_ID;
                            mMMachineobj.Machine_Make = mM_MT_Machines.Machine_Make;
                            mMMachineobj.Machine_Model = mM_MT_Machines.Machine_Model;
                            mMMachineobj.IsActive = mM_MT_Machines.IsActive;
                            mMMachineobj.Mail_Trigger_In = mM_MT_Machines.Mail_Trigger_In;
                            mMMachineobj.Machine_Description = mM_MT_Machines.Machine_Description;
                            mMMachineobj.M_Asset_Number = mM_MT_Machines.M_Asset_Number;
                            mMMachineobj.IP_Address = mM_MT_Machines.IP_Address;

                            mMMachineobj.IsCBM = mM_MT_Machines.IsCBM;
                            mMMachineobj.IsTBM = mM_MT_Machines.IsTBM;
                            mMMachineobj.IsMinorStoppage = mM_MT_Machines.IsMinorStoppage;
                            mMMachineobj.CBM_Matrix1 = mM_MT_Machines.CBM_Matrix1;
                            mMMachineobj.CBM_Matrix2 = mM_MT_Machines.CBM_Matrix2;
                            mMMachineobj.CBM_Matrix3 = mM_MT_Machines.CBM_Matrix3;
                            mMMachineobj.CBM_Email1 = mM_MT_Machines.CBM_Email1;
                            mMMachineobj.CBM_Email2 = mM_MT_Machines.CBM_Email2;
                            mMMachineobj.CBM_Email3 = mM_MT_Machines.CBM_Email3;
                            mMMachineobj.CBM_SMS1 = mM_MT_Machines.CBM_SMS1;
                            mMMachineobj.CBM_SMS2 = mM_MT_Machines.CBM_SMS2;
                            mMMachineobj.CBM_SMS3 = mM_MT_Machines.CBM_SMS3;
                            mMMachineobj.TBM_Matrix1 = mM_MT_Machines.TBM_Matrix1;
                            mMMachineobj.TBM_Matrix2 = mM_MT_Machines.TBM_Matrix2;
                            mMMachineobj.TBM_Matrix3 = mM_MT_Machines.TBM_Matrix3;
                            mMMachineobj.TBM_Email1 = mM_MT_Machines.TBM_Email1;
                            mMMachineobj.TBM_Email2 = mM_MT_Machines.TBM_Email2;
                            mMMachineobj.TBM_Email3 = mM_MT_Machines.TBM_Email3;
                            mMMachineobj.TBM_SMS1 = mM_MT_Machines.TBM_SMS1;
                            mMMachineobj.TBM_SMS2 = mM_MT_Machines.TBM_SMS2;
                            mMMachineobj.TBM_SMS3 = mM_MT_Machines.TBM_SMS3;
                            mMMachineobj.MS_Matrix1 = mM_MT_Machines.MS_Matrix1;
                            mMMachineobj.MS_Matrix2 = mM_MT_Machines.MS_Matrix2;
                            mMMachineobj.MS_Matrix3 = mM_MT_Machines.MS_Matrix3;
                            mMMachineobj.MS_Occurences1 = mM_MT_Machines.MS_Occurences1;
                            mMMachineobj.MS_Occurences2 = mM_MT_Machines.MS_Occurences2;
                            mMMachineobj.MS_Occurences3 = mM_MT_Machines.MS_Occurences3;
                            mMMachineobj.MS_Email1 = mM_MT_Machines.MS_Email1;
                            mMMachineobj.MS_Email2 = mM_MT_Machines.MS_Email2;
                            mMMachineobj.MS_Email3 = mM_MT_Machines.MS_Email3;
                            mMMachineobj.MS_SMS1 = mM_MT_Machines.MS_SMS1;
                            mMMachineobj.MS_SMS2 = mM_MT_Machines.MS_SMS2;
                            mMMachineobj.MS_SMS3 = mM_MT_Machines.MS_SMS3;

                            mMMachineobj.Is_Edited = true;
                            mMMachineobj.Updated_Date = DateTime.Now;
                            mMMachineobj.Updated_Host = HttpContext.Request.UserHostAddress;
                            mMMachineobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.Entry(mMMachineobj).State = EntityState.Modified;
                            db.SaveChanges();

                            //Insert records into MTTUW machines tables
                            MTMachine = MTTUWdb.MM_MT_MTTUW_Machines.Find(mMMachineobj.Machine_ID);
                            MTMachine.Family = mMMachineobj.Family;
                            MTMachine.IsActive = mMMachineobj.IsActive;
                            MTMachine.Mail_Trigger_In = mMMachineobj.Mail_Trigger_In;
                            MTMachine.Machine_Make = mMMachineobj.Machine_Make;
                            MTMachine.Machine_Model = mMMachineobj.Machine_Model;
                            MTMachine.M_Type_ID = mMMachineobj.M_Type_ID;
                            MTMachine.FMEA_Document = mMMachineobj.FMEA_Document;
                            MTMachine.Inserted_Date = mMMachineobj.Inserted_Date;
                            MTMachine.Inserted_Host = mMMachineobj.Inserted_Host;
                            MTMachine.Inserted_User_ID = mMMachineobj.Inserted_User_ID;
                            MTMachine.Is_Deleted = mMMachineobj.Is_Deleted;
                            MTMachine.Is_Edited = mMMachineobj.Is_Edited;
                            MTMachine.Is_Purgeable = mMMachineobj.Is_Purgeable;
                            MTMachine.Is_Status_Machine = mMMachineobj.Is_Status_Machine;
                            MTMachine.Is_Transfered = mMMachineobj.Is_Transfered;
                            MTMachine.Line_ID = mMMachineobj.Line_ID;
                            MTMachine.Machine_Category_ID = mMMachineobj.Machine_Category_ID;
                            MTMachine.Machine_Description = mMMachineobj.Machine_Description;
                            MTMachine.Machine_Name = mMMachineobj.Machine_Name;
                            MTMachine.Machine_Number = mMMachineobj.Machine_Number;
                            MTMachine.Manufaturing_Year = mMMachineobj.Manufaturing_Year;
                            MTMachine.Plant_ID = mMMachineobj.Plant_ID;
                            MTMachine.Shop_ID = mMMachineobj.Shop_ID;
                            MTMachine.Station_ID = mMMachineobj.Station_ID;

                            MTMachine.IsCBM = mMMachineobj.IsCBM;
                            MTMachine.IsTBM = mMMachineobj.IsTBM;
                            MTMachine.IsMinorStoppage = mMMachineobj.IsMinorStoppage;
                            MTMachine.CBM_Matrix1 = mMMachineobj.CBM_Matrix1;
                            MTMachine.CBM_Matrix2 = mMMachineobj.CBM_Matrix2;
                            MTMachine.CBM_Matrix3 = mMMachineobj.CBM_Matrix3;
                            MTMachine.CBM_Email1 = mMMachineobj.CBM_Email1;
                            MTMachine.CBM_Email2 = mMMachineobj.CBM_Email2;
                            MTMachine.CBM_Email3 = mMMachineobj.CBM_Email3;
                            MTMachine.CBM_SMS1 = mMMachineobj.CBM_SMS1;
                            MTMachine.CBM_SMS2 = mMMachineobj.CBM_SMS2;
                            MTMachine.CBM_SMS3 = mMMachineobj.CBM_SMS3;
                            MTMachine.TBM_Matrix1 = mMMachineobj.TBM_Matrix1;
                            MTMachine.TBM_Matrix2 = mMMachineobj.TBM_Matrix2;
                            MTMachine.TBM_Matrix3 = mMMachineobj.TBM_Matrix3;
                            MTMachine.TBM_Email1 = mMMachineobj.TBM_Email1;
                            MTMachine.TBM_Email2 = mMMachineobj.TBM_Email2;
                            MTMachine.TBM_Email3 = mMMachineobj.TBM_Email3;
                            MTMachine.TBM_SMS1 = mMMachineobj.TBM_SMS1;
                            MTMachine.TBM_SMS2 = mMMachineobj.TBM_SMS2;
                            MTMachine.TBM_SMS3 = mMMachineobj.TBM_SMS3;
                            MTMachine.MS_Matrix1 = mMMachineobj.MS_Matrix1;
                            MTMachine.MS_Matrix2 = mMMachineobj.MS_Matrix2;
                            MTMachine.MS_Matrix3 = mMMachineobj.MS_Matrix3;
                            MTMachine.MS_Occurences1 = mMMachineobj.MS_Occurences1;
                            MTMachine.MS_Occurences2 = mMMachineobj.MS_Occurences2;
                            MTMachine.MS_Occurences3 = mMMachineobj.MS_Occurences3;
                            MTMachine.MS_Email1 = mMMachineobj.MS_Email1;
                            MTMachine.MS_Email2 = mMMachineobj.MS_Email2;
                            MTMachine.MS_Email3 = mMMachineobj.MS_Email3;
                            MTMachine.MS_SMS1 = mMMachineobj.MS_SMS1;
                            MTMachine.MS_SMS2 = mMMachineobj.MS_SMS2;
                            MTMachine.MS_SMS3 = mMMachineobj.MS_SMS3;

                            MTTUWdb.Entry(MTMachine).State = EntityState.Modified;
                            MTTUWdb.SaveChanges();
                            //End


                            MM_MT_Machine_Document mM_MT_Machine_Document = new MM_MT_Machine_Document();
                            var count = db.MM_MT_Machine_Document.Where(a => a.Machine_ID == mM_MT_Machines.Machine_ID).Select(m => m.M_Doc_ID).ToList().Count;
                            var AllDocumentCount = count + files.Length;
                            //mM_MT_Machine_Document = db.MM_MT_Machine_Document.Find(id);
                            if (AllDocumentCount > 10)
                            {
                                ModelState.AddModelError("FMEA_Document", "Upto 10 files are uploaded against one machine");
                            }
                            else
                            {
                                foreach (var file in files)
                                {
                                    if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                                    {
                                        ModelState.AddModelError("FMEA_Document", "File Should be type of Pdf");
                                    }
                                    else if (Path.GetExtension(file.FileName).ToLower() == ".pdf")
                                    {
                                        if (file != null && file.ContentLength > 0)
                                        {
                                            using (var reader = new System.IO.BinaryReader(file.InputStream))
                                            {
                                                mM_MT_Machine_Document.Document_Name = System.IO.Path.GetFileName(file.FileName);
                                                mM_MT_Machine_Document.Document_Type = Path.GetExtension(file.FileName);
                                                mM_MT_Machine_Document.Content_Type = file.ContentType;
                                                mM_MT_Machine_Document.Document_Content = reader.ReadBytes(file.ContentLength);
                                                mM_MT_Machine_Document.Machine_ID = mM_MT_Machines.Machine_ID;
                                                mM_MT_Machine_Document.Inserted_Date = DateTime.Now;
                                                mM_MT_Machine_Document.Inserted_Host = HttpContext.Request.UserHostAddress;
                                                mM_MT_Machine_Document.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                                db.MM_MT_Machine_Document.Add(mM_MT_Machine_Document);
                                                db.SaveChanges();
                                            }
                                        }


                                    }
                                }

                                var imagecount = db.MM_Machine_Image.Where(m => m.Machine_ID == mM_MT_Machines.Machine_ID).Select(m => m.Image_ID).FirstOrDefault();


                                MM_Machine_Image mm_image = new MM_Machine_Image();
                                if (imagecount > 0)

                                {
                                    if (upload != null && upload.ContentLength > 0)
                                    {
                                        mm_img1 = db.MM_Machine_Image.Find(imagecount);

                                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                                        {


                                            mm_img1.Image_Name = System.IO.Path.GetFileName(upload.FileName);
                                            mm_img1.Image_Type = Path.GetExtension(upload.FileName);
                                            mm_img1.Content_Type = upload.ContentType;
                                            mm_img1.Image_Content = reader.ReadBytes(upload.ContentLength);
                                            mm_img1.Updated_Date = DateTime.Now;
                                            mm_img1.Updated_Host = HttpContext.Request.UserHostAddress;
                                            mm_img1.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                            mm_img1.Machine_ID = mM_MT_Machines.Machine_ID;
                                            db.Entry(mm_img1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {

                                    if (upload != null && upload.ContentLength > 0)
                                    {
                                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                                        {

                                            mm_image.Image_Name = System.IO.Path.GetFileName(upload.FileName);
                                            mm_image.Image_Type = Path.GetExtension(upload.FileName);
                                            mm_image.Content_Type = upload.ContentType;
                                            mm_image.Image_Content = reader.ReadBytes(upload.ContentLength);
                                            mm_image.Inserted_Date = DateTime.Now;
                                            mm_image.Inserted_Host = HttpContext.Request.UserHostAddress;
                                            mm_image.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                            mm_image.Machine_ID = mM_MT_Machines.Machine_ID;
                                            db.MM_Machine_Image.Add(mm_image);
                                            db.SaveChanges();
                                        }
                                    }

                                }







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
                            mMMachineobj.Shop_ID = mM_MT_Machines.Shop_ID;
                            mMMachineobj.Line_ID = mM_MT_Machines.Line_ID;
                            mMMachineobj.Machine_Name = mM_MT_Machines.Machine_Name;
                            mMMachineobj.Machine_Number = mM_MT_Machines.Machine_Number;
                            //mMMachineobj.Family = mM_MT_Machines.Family;
                            mMMachineobj.Machine_Make = mM_MT_Machines.Machine_Make;
                            mMMachineobj.Machine_Model = mM_MT_Machines.Machine_Model;
                            mMMachineobj.M_Type_ID = mM_MT_Machines.M_Type_ID;
                            mMMachineobj.IsActive = mM_MT_Machines.IsActive;
                            mMMachineobj.Mail_Trigger_In = mM_MT_Machines.Mail_Trigger_In;
                            mMMachineobj.Station_ID = mM_MT_Machines.Station_ID;
                            mMMachineobj.Machine_Category_ID = mM_MT_Machines.Machine_Category_ID;
                            mMMachineobj.Manufaturing_Year = mM_MT_Machines.Manufaturing_Year;
                            mMMachineobj.Machine_Description = mM_MT_Machines.Machine_Description;
                            mMMachineobj.M_Asset_Number = mM_MT_Machines.M_Asset_Number;
                            mMMachineobj.IP_Address = mM_MT_Machines.IP_Address;

                            mMMachineobj.IsCBM = mM_MT_Machines.IsCBM;
                            mMMachineobj.IsTBM = mM_MT_Machines.IsTBM;
                            mMMachineobj.IsMinorStoppage = mM_MT_Machines.IsMinorStoppage;
                            mMMachineobj.CBM_Matrix1 = mM_MT_Machines.CBM_Matrix1;
                            mMMachineobj.CBM_Matrix2 = mM_MT_Machines.CBM_Matrix2;
                            mMMachineobj.CBM_Matrix3 = mM_MT_Machines.CBM_Matrix3;
                            mMMachineobj.CBM_Email1 = mM_MT_Machines.CBM_Email1;
                            mMMachineobj.CBM_Email2 = mM_MT_Machines.CBM_Email2;
                            mMMachineobj.CBM_Email3 = mM_MT_Machines.CBM_Email3;
                            mMMachineobj.CBM_SMS1 = mM_MT_Machines.CBM_SMS1;
                            mMMachineobj.CBM_SMS2 = mM_MT_Machines.CBM_SMS2;
                            mMMachineobj.CBM_SMS3 = mM_MT_Machines.CBM_SMS3;
                            mMMachineobj.TBM_Matrix1 = mM_MT_Machines.TBM_Matrix1;
                            mMMachineobj.TBM_Matrix2 = mM_MT_Machines.TBM_Matrix2;
                            mMMachineobj.TBM_Matrix3 = mM_MT_Machines.TBM_Matrix3;
                            mMMachineobj.TBM_Email1 = mM_MT_Machines.TBM_Email1;
                            mMMachineobj.TBM_Email2 = mM_MT_Machines.TBM_Email2;
                            mMMachineobj.TBM_Email3 = mM_MT_Machines.TBM_Email3;
                            mMMachineobj.TBM_SMS1 = mM_MT_Machines.TBM_SMS1;
                            mMMachineobj.TBM_SMS2 = mM_MT_Machines.TBM_SMS2;
                            mMMachineobj.TBM_SMS3 = mM_MT_Machines.TBM_SMS3;
                            mMMachineobj.MS_Matrix1 = mM_MT_Machines.MS_Matrix1;
                            mMMachineobj.MS_Matrix2 = mM_MT_Machines.MS_Matrix2;
                            mMMachineobj.MS_Matrix3 = mM_MT_Machines.MS_Matrix3;
                            mMMachineobj.MS_Occurences1 = mM_MT_Machines.MS_Occurences1;
                            mMMachineobj.MS_Occurences2 = mM_MT_Machines.MS_Occurences2;
                            mMMachineobj.MS_Occurences3 = mM_MT_Machines.MS_Occurences3;
                            mMMachineobj.MS_Email1 = mM_MT_Machines.MS_Email1;
                            mMMachineobj.MS_Email2 = mM_MT_Machines.MS_Email2;
                            mMMachineobj.MS_Email3 = mM_MT_Machines.MS_Email3;
                            mMMachineobj.MS_SMS1 = mM_MT_Machines.MS_SMS1;
                            mMMachineobj.MS_SMS2 = mM_MT_Machines.MS_SMS2;
                            mMMachineobj.MS_SMS3 = mM_MT_Machines.MS_SMS3;

                            mMMachineobj.Is_Edited = true;
                            mMMachineobj.Updated_Date = DateTime.Now;
                            mMMachineobj.Updated_Host = HttpContext.Request.UserHostAddress;
                            mMMachineobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.Entry(mMMachineobj).State = EntityState.Modified;
                            db.SaveChanges();


                            //Insert records into MTTUW machines tables
                            MTMachine = MTTUWdb.MM_MT_MTTUW_Machines.Find(mMMachineobj.Machine_ID);
                            //MTMachine.Family = mMMachineobj.Family;
                            MTMachine.Machine_Make = mMMachineobj.Machine_Make;
                            MTMachine.Machine_Model = mMMachineobj.Machine_Model;
                            MTMachine.M_Type_ID = mMMachineobj.M_Type_ID;
                            MTMachine.IsActive = mMMachineobj.IsActive;
                            MTMachine.Mail_Trigger_In = mMMachineobj.Mail_Trigger_In;
                            MTMachine.Inserted_Date = mMMachineobj.Inserted_Date;
                            MTMachine.Inserted_Host = mMMachineobj.Inserted_Host;
                            MTMachine.Inserted_User_ID = mMMachineobj.Inserted_User_ID;
                            MTMachine.Is_Deleted = mMMachineobj.Is_Deleted;
                            MTMachine.Is_Edited = mMMachineobj.Is_Edited;
                            MTMachine.Is_Purgeable = mMMachineobj.Is_Purgeable;
                            MTMachine.Is_Status_Machine = mMMachineobj.Is_Status_Machine;
                            MTMachine.Is_Transfered = mMMachineobj.Is_Transfered;
                            MTMachine.Line_ID = mMMachineobj.Line_ID;
                            MTMachine.Machine_Category_ID = mMMachineobj.Machine_Category_ID;
                            MTMachine.Machine_Description = mMMachineobj.Machine_Description;
                            MTMachine.Machine_Name = mMMachineobj.Machine_Name;
                            MTMachine.Machine_Number = mMMachineobj.Machine_Number;
                            MTMachine.Manufaturing_Year = mMMachineobj.Manufaturing_Year;
                            MTMachine.Plant_ID = mMMachineobj.Plant_ID;
                            MTMachine.Shop_ID = mMMachineobj.Shop_ID;
                            MTMachine.Station_ID = mMMachineobj.Station_ID;
                            MTMachine.IsCBM = mMMachineobj.IsCBM;
                            MTMachine.IsTBM = mMMachineobj.IsTBM;
                            MTMachine.IsMinorStoppage = mMMachineobj.IsMinorStoppage;
                            MTMachine.CBM_Matrix1 = mMMachineobj.CBM_Matrix1;
                            MTMachine.CBM_Matrix2 = mMMachineobj.CBM_Matrix2;
                            MTMachine.CBM_Matrix3 = mMMachineobj.CBM_Matrix3;
                            MTMachine.CBM_Email1 = mMMachineobj.CBM_Email1;
                            MTMachine.CBM_Email2 = mMMachineobj.CBM_Email2;
                            MTMachine.CBM_Email3 = mMMachineobj.CBM_Email3;
                            MTMachine.CBM_SMS1 = mMMachineobj.CBM_SMS1;
                            MTMachine.CBM_SMS2 = mMMachineobj.CBM_SMS2;
                            MTMachine.CBM_SMS3 = mMMachineobj.CBM_SMS3;
                            MTMachine.TBM_Matrix1 = mMMachineobj.TBM_Matrix1;
                            MTMachine.TBM_Matrix2 = mMMachineobj.TBM_Matrix2;
                            MTMachine.TBM_Matrix3 = mMMachineobj.TBM_Matrix3;
                            MTMachine.TBM_Email1 = mMMachineobj.TBM_Email1;
                            MTMachine.TBM_Email2 = mMMachineobj.TBM_Email2;
                            MTMachine.TBM_Email3 = mMMachineobj.TBM_Email3;
                            MTMachine.TBM_SMS1 = mMMachineobj.TBM_SMS1;
                            MTMachine.TBM_SMS2 = mMMachineobj.TBM_SMS2;
                            MTMachine.TBM_SMS3 = mMMachineobj.TBM_SMS3;
                            MTMachine.MS_Matrix1 = mMMachineobj.MS_Matrix1;
                            MTMachine.MS_Matrix2 = mMMachineobj.MS_Matrix2;
                            MTMachine.MS_Matrix3 = mMMachineobj.MS_Matrix3;
                            MTMachine.MS_Occurences1 = mMMachineobj.MS_Occurences1;
                            MTMachine.MS_Occurences2 = mMMachineobj.MS_Occurences2;
                            MTMachine.MS_Occurences3 = mMMachineobj.MS_Occurences3;
                            MTMachine.MS_Email1 = mMMachineobj.MS_Email1;
                            MTMachine.MS_Email2 = mMMachineobj.MS_Email2;
                            MTMachine.MS_Email3 = mMMachineobj.MS_Email3;
                            MTMachine.MS_SMS1 = mMMachineobj.MS_SMS1;
                            MTMachine.MS_SMS2 = mMMachineobj.MS_SMS2;
                            MTMachine.MS_SMS3 = mMMachineobj.MS_SMS3;
                            MTTUWdb.Entry(MTMachine).State = EntityState.Modified;
                            MTTUWdb.SaveChanges();
                            //End



                            var imagecount = db.MM_Machine_Image.Where(m => m.Machine_ID == mM_MT_Machines.Machine_ID).Select(m => m.Image_ID).FirstOrDefault();


                            MM_Machine_Image mm_image = new MM_Machine_Image();
                            if (imagecount > 0)

                            {
                                mm_img1 = db.MM_Machine_Image.Find(imagecount);

                                if (upload != null && upload.ContentLength > 0)
                                {
                                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                                    {


                                        mm_img1.Image_Name = System.IO.Path.GetFileName(upload.FileName);
                                        mm_img1.Image_Type = Path.GetExtension(upload.FileName);
                                        mm_img1.Content_Type = upload.ContentType;
                                        mm_img1.Image_Content = reader.ReadBytes(upload.ContentLength);
                                        mm_img1.Updated_Date = DateTime.Now;
                                        mm_img1.Updated_Host = HttpContext.Request.UserHostAddress;
                                        mm_img1.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        mm_img1.Machine_ID = mM_MT_Machines.Machine_ID;
                                        db.Entry(mm_img1).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                            else
                            {

                                if (upload != null && upload.ContentLength > 0)
                                {
                                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                                    {

                                        mm_image.Image_Name = System.IO.Path.GetFileName(upload.FileName);
                                        mm_image.Image_Type = Path.GetExtension(upload.FileName);
                                        mm_image.Content_Type = upload.ContentType;
                                        mm_image.Image_Content = reader.ReadBytes(upload.ContentLength);
                                        mm_image.Inserted_Date = DateTime.Now;
                                        mm_image.Inserted_Host = HttpContext.Request.UserHostAddress;
                                        mm_image.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        mm_image.Machine_ID = mM_MT_Machines.Machine_ID;
                                        db.MM_Machine_Image.Add(mm_image);
                                        db.SaveChanges();
                                    }
                                }

                            }









                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                            globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Edit_Success;
                            ViewBag.GlobalDataModel = globalData;
                            TempData["globalData"] = globalData;
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
            }

            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceDisplayName.Machine + " " + ResourceMessages.Edit_Success;
            globalData.contentFooter = ResourceDisplayName.Machine + " " + ResourceMessages.Edit_Success;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            //ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_MT_Machines.Station_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(a => a.Line_ID == mM_MT_Machines.Line_ID).OrderBy(a => a.Station_Name), "Station_ID", "Station_Name", mM_MT_Machines.Station_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Machines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Machines.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(a => a.Shop_ID == mM_MT_Machines.Shop_ID).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name", mM_MT_Machines.Line_ID);
            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_Machine_Category, "Machine_Category_ID", "Category", mM_MT_Machines.Machine_Category_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Machines.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.OrderBy(x => x.Shop_Name), "Shop_ID", "Shop_Name", mM_MT_Machines.Shop_ID);
            ViewBag.M_Type_ID = new SelectList(db.MM_MT_Machine_Type, "M_Type_ID", "Machine_Type", mM_MT_Machines.M_Type_ID);
            return View(mM_MT_Machines);
        }
        #endregion



        public ActionResult SaveShiftData(int MachineID,bool PMactivity,string ID)
        {
            db.MM_Machine_Shift_Data.RemoveRange(db.MM_Machine_Shift_Data.Where(a => a.Machine_ID == MachineID));
            db.SaveChanges();
            //mMMachineob = db.MM_Machine_Shift_Data.Find();
          
            if (ID != "")
            {
                var shiftID = ID.Split(',');
                foreach (var id in shiftID)
                {
                    var ShiftID = Convert.ToDecimal(id);
                    MM_Machine_Shift_Data obj = new MM_Machine_Shift_Data();
                    obj.Machine_ID = MachineID;
                    obj.Shift_ID = ShiftID;
                    obj.Is_PM_Activity = PMactivity;
                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    obj.Inserted_Date = DateTime.Now;
                    db.MM_Machine_Shift_Data.Add(obj);
                    db.SaveChanges();
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);

        }



        [HttpGet]
        public ActionResult GETShiftDetails(decimal Row_id)
        {
            var Machine_Shift_ID = db.MM_Machine_Shift_Data.Where(m => m.Machine_ID == Row_id).Select(m => m.Shift_ID).ToList();
            var isPM = db.MM_Machine_Shift_Data.Where(m => m.Machine_ID == Row_id).Select(m => m.Is_PM_Activity).FirstOrDefault();
            List<decimal> ShiftData = new List<decimal>();
            try
            {
               
                foreach (var id in Machine_Shift_ID)
                {
                    var shiftID = Convert.ToDecimal(id);
                    ShiftData.Add(shiftID);
                   
                }
                return Json(new { Result = ShiftData,IsPM = isPM }, JsonRequestBehavior.AllowGet); 
            }

            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }













        #region Delete Specified Machine



        /*
        * Action Name          : Delete
        * Input Parameter      : (id) Machine id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action is used to edit the machine
        */
        // GET: Machines/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Machines mM_MT_Machines = db.MM_MT_Machines.Find(id);
            if (mM_MT_Machines == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Machines";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(mM_MT_Machines);
        }

        /*
        * Action Name          : Delete
        * Input Parameter      : (id) Machine id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action is used to  Confirmed user to delete the machine
        */
        // POST: Machines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            //bool flag = false;
            try
            {
                MM_MT_Machines mM_MT_Machines = db.MM_MT_Machines.Where(x => x.Machine_ID == id).FirstOrDefault();
                db.MM_MT_Machines.Remove(mM_MT_Machines);
                db.SaveChanges();

                //delete records from MTTUW DB
                MTMachine = MTTUWdb.MM_MT_MTTUW_Machines.Find(id);
                MTTUWdb.MM_MT_MTTUW_Machines.Remove(MTMachine);
                MTTUWdb.SaveChanges();
                //End

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_MT_Machines", "Machine_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "Machines";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceDisplayName.Machine + " " + ResourceMessages.Delete_Success;
                globalData.contentFooter = ResourceDisplayName.Machine + " " + ResourceMessages.Delete_Success;

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Delete_Success;

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
                    globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "Machine Deletion Error";
                    globalData.messageDetail = "Can not remove Machine because machine has other references so please first remove reference of machine then try again.";
                    ViewBag.GlobalDataModel = globalData;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
                else
                {

                    globalData.pageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                    globalData.subTitle = ResourceGlobal.Delete;
                    globalData.controllerName = "Machines";
                    globalData.actionName = ResourceGlobal.Delete;
                    globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceDisplayName.Machine + " " + ResourceGlobal.Form;

                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "Machine Deletion Error";
                    globalData.messageDetail = ex.Message.ToString();
                    ViewBag.GlobalDataModel = globalData;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

            // return RedirectToAction("Index");
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
        * Input Parameter      : Datatable and MM_MT_Machines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action used to show upload form
        */
        //GET: Upload file page load
        public ActionResult Upload()
        {
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");

            ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_Machine_Category, "Machine_category_ID", "Category");
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
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceDisplayName.Machine;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceDisplayName.Machine;

            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }
        /*
        * Action Name          : Upload
        * Input Parameter      : Files(file object) and MM_MT_Machines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action used get upload excel file and generationg datatable
        */

        //GET: GET The file from upload control 
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(HttpPostedFileBase files, [Bind(Include = "Machine_ID,Machine_Number,Machine_Name,Machine_Description,Plant_ID,Shop_ID,Line_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Machines mM_MT_Machine)
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
                        MM_MT_Machines mM_MT_Machines = new MM_MT_Machines();
                        mM_MT_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        string machinename = mchineList["Machine_Name"].ToString().Trim();
                        string machineNumber = mchineList["Machine_Number"].ToString().Trim();
                        string machineDes = mchineList["Machine_Description"].ToString().Trim();
                        string Shop = mchineList["Shop_Name"].ToString().Trim();
                        string Line = mchineList["Line_Name"].ToString().Trim();
                        string manufacturing_yerar = mchineList["Manufacturing_Year"].ToString().Trim();
                        string machine_category = mchineList["Machine_Category"].ToString().Trim();
                        decimal shop_ID = db.MM_Shops.Where(x => x.Shop_Name.ToLower() == Shop.ToLower()).FirstOrDefault().Shop_ID;
                        decimal line_ID = db.MM_Lines.Where(x => x.Line_Name.ToLower() == Line.ToLower()).FirstOrDefault().Line_ID;
                        decimal category_ID = db.MM_MT_Machine_Category.Where(x => x.Category.ToLower() == machine_category.ToLower()).FirstOrDefault().Machine_Category_ID;
                        MachineRecords mmrecordobj = new MachineRecords();
                        mmrecordobj.machineName = machinename;
                        mmrecordobj.machineNumber = machineNumber;
                        mmrecordobj.machineDescription = machineDes;
                        if (machinename == "" && machinename != null)
                        {
                            mmrecordobj.MachineListError = "Machine can not be null";
                        }
                        else if (mM_MT_Machines.isMachineExists(machineNumber, 0, mM_MT_Machines.Plant_ID))
                        {
                            mmrecordobj.MachineListError = "Machine Name is already Exists";
                        }
                        else
                        {
                            mM_MT_Machines.Machine_Name = Convert.ToString(machinename);
                            mM_MT_Machines.Machine_Number = Convert.ToString(machineNumber);
                            mM_MT_Machines.Machine_Description = Convert.ToString(machineDes);
                            mM_MT_Machines.Manufaturing_Year = Convert.ToDateTime(manufacturing_yerar);
                            mM_MT_Machines.Machine_Category_ID = category_ID;
                            mM_MT_Machines.Shop_ID = shop_ID;
                            mM_MT_Machines.Line_ID = line_ID;
                            mM_MT_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                            mM_MT_Machines.Inserted_Date = DateTime.Now;
                            mM_MT_Machines.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                            mM_MT_Machines.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.MM_MT_Machines.Add(mM_MT_Machines);
                            db.SaveChanges();
                            mmrecordobj.MachineListError = "Machine is added successfully";
                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                            globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Add_Success;
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
                ViewBag.Machine_Category_ID = new SelectList(db.MM_MT_Machine_Category, "Machine_category_ID", "Category");
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
                    globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                    globalData.messageDetail = "Please upload file, file can not be empty.";
                }
                else
                {
                    globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
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
        //    MM_MT_Machines mM_Machines = db.MM_MT_Machines.Find(id);
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

        public FileResult DownloadFile(decimal Machine_Id, decimal Doc_Id)
        {
            MM_MT_Machine_Document mM_MT_Machine_Document = db.MM_MT_Machine_Document.Where(m => m.Machine_ID == Machine_Id && m.M_Doc_ID == Doc_Id).FirstOrDefault();
            byte[] fileBytes = mM_MT_Machine_Document.Document_Content;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(mM_MT_Machine_Document.Document_Name));
        }

        public FileResult Download(decimal id)
        {
            MM_MT_Machines mM_Machines = db.MM_MT_Machines.Find(id);
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath(@"~/" + mM_Machines.FMEA_Document)));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(mM_Machines.FMEA_Document));
        }
        #endregion

        #region Insert all Data of DataTable into respective Database Table
        /*
        * Action Name          : InsertIntoDataTable
        * Input Parameter      : Datatable and MM_MT_Machines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action used to insert datatable data into database table  
        */
        private bool InsertIntoDataTable(DataTable dt, MM_MT_Machines mM_MT_Machines)
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
                    decimal category_ID = db.MM_MT_Machine_Category.Where(x => x.Category.ToLower() == machine_category.ToLower()).FirstOrDefault().Machine_Category_ID;
                    if (db.MM_MT_Machines.Where(x => x.Machine_Number.ToLower() == machineNumber.ToLower()).Count() > 0)
                    {
                        ModelState.AddModelError("Machine_Number", ResourceValidation.Exist);
                        return isstatus;
                    }
                    else
                    {

                        mM_MT_Machines.Machine_Name = Convert.ToString(dr["Machine_Name"].ToString().Trim());
                        mM_MT_Machines.Machine_Number = Convert.ToString(dr["Machine_Number"].ToString().Trim());
                        mM_MT_Machines.Machine_Description = Convert.ToString(dr["Machine_Description"].ToString().Trim());
                        mM_MT_Machines.Manufaturing_Year = Convert.ToDateTime(manufacturing_yerar);
                        mM_MT_Machines.Machine_Category_ID = category_ID;
                        mM_MT_Machines.Shop_ID = shop_ID;
                        mM_MT_Machines.Line_ID = line_ID;
                        mM_MT_Machines.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        mM_MT_Machines.Inserted_Date = DateTime.Now;
                        mM_MT_Machines.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                        mM_MT_Machines.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.MM_MT_Machines.Add(mM_MT_Machines);
                        db.SaveChanges();
                        return isstatus = true;
                    }
                    // }
                }
            }
            catch (Exception ex)
            {
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine + " " + ResourceGlobal.Config;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
            }
            return isstatus;
        }
        #endregion
    }
}
