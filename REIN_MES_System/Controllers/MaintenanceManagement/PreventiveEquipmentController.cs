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
using System.Data.OleDb;
using System.Globalization;
using ZHB_AD.Controllers.BaseManagement;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    /* Controller Name          : PreventiveEquipment
  *  Description                : To create,edit,delete and show all Equipment 
  *  Author, Timestamp          : Ajay Wagh       
  */
    public class PreventiveEquipmentController : BaseController
    {
        #region global variable decaration
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        MM_MT_Preventive_Equipment mmPreveniveEquipment = new MM_MT_Preventive_Equipment();
        MM_TBM_Image mm_img = new MM_TBM_Image();
        #endregion

        #region Show all or Specified Equipment Against Machine
        /*
        * Action Name          : Index
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Get the list of Equipment against machine
        */

        // GET: PreventiveEquipment
        public ActionResult Index()
        {
            var mM_MT_Preventive_Equipment = db.MM_MT_Preventive_Equipment.Include(m => m.MM_MTTUW_Employee).Include(m => m.MM_MT_MTTUW_Machines); //db.MM_MT_Preventive_Equipment.ToList();//db.MM_MT_Preventive_Equipment.Include(m => m.MM_Employee).Include(m => m.MM_MT_Machines).Include(m => m.MM_MT_Machines1);
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            // globalData.pageTitle = ResourceDisplayName.Machine_Part;
            globalData.pageTitle = " TBM Configuration ";
            globalData.subTitle = ResourceGlobal.Index;
            globalData.controllerName = "Preventive Machine Maintenance";
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceGlobal.Equipment + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceGlobal.Equipment + " " + ResourceGlobal.Lists;
            TempData["globalData"] = globalData;

            ViewBag.GlobalDataModel = globalData;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            return View(mM_MT_Preventive_Equipment.Where(m => m.Plant_ID == plantId).ToList());
        }
        /*
         * Action Name          : Index
         * Input Parameter      : id(EQP_ID)
         * Return Type          : ActionResult
         * Author & Time Stamp  : Ajay Wagh
         * Description          : Get one Equipment Details
         */
        // GET: PreventiveEquipment/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Preventive_Equipment mM_MT_Preventive_Equipment = db.MM_MT_Preventive_Equipment.Find(id);
            if (mM_MT_Preventive_Equipment == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceDisplayName.Machine_Part;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Equipment";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceGlobal.Equipment + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceGlobal.Equipment + " " + ResourceGlobal.Details;


            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Preventive_Equipment);
        }
        #endregion

        #region Creation of Equipment against machine

        /*
        * Action Name          : Create
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : used to load equipment creation form
        */

        // GET: PreventiveEquipment/Create
        public ActionResult Create()
        {
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name");
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new { Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")", Machine_ID = a.Machine_ID })
            //    , "Machine_ID", "Machine_Name");
            globalData.pageTitle = ResourceDisplayName.Machine_Part;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Equipment";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;


            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        /*
        * Action Name          : Create
        * Input Parameter      : object of MM_MT_PreventiveEquipment
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : used to Add equipment against machine
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EQP_ID,Equipment_Name,Is_PM_Equipment,Machine_ID,IsActive,Sequence_No,Designated_Life,Life_Per_Cycle,Warning_At,Stop_At,Remaining_Life,Reset_Consumelife,Action_taken,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host,SAP_Part_No,Data_Retention_Period")] MM_MT_Preventive_Equipment mM_MT_Preventive_Equipment, HttpPostedFileBase upload)
        {
            var isValid = true;
            if (mM_MT_Preventive_Equipment.Warning_At > mM_MT_Preventive_Equipment.Designated_Life)
            {
                isValid = false;
                ModelState.AddModelError("Warning_At", "Warning At value to be less than designated life");
            }
            if (mM_MT_Preventive_Equipment.Stop_At <= mM_MT_Preventive_Equipment.Warning_At || mM_MT_Preventive_Equipment.Stop_At > mM_MT_Preventive_Equipment.Designated_Life)
            {
                isValid = false;
                ModelState.AddModelError("Stop_At", "Stop At value to be less than or equal to designated life and it should be greater than Warning At");
            }
            if (ModelState.IsValid == true && isValid == true)
            {

                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                mM_MT_Preventive_Equipment.Plant_ID = plantId;
                mM_MT_Preventive_Equipment.Remaining_Life = mM_MT_Preventive_Equipment.Designated_Life;
                mM_MT_Preventive_Equipment.Inserted_Date = DateTime.Now;
                mM_MT_Preventive_Equipment.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_MT_Preventive_Equipment.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.MM_MT_Preventive_Equipment.Add(mM_MT_Preventive_Equipment);
                db.SaveChanges();


                MM_TBM_Image mm_image = new MM_TBM_Image();
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
                        mm_image.EQP_ID = mM_MT_Preventive_Equipment.EQP_ID;
                        db.MM_TBM_Image.Add(mm_image);
                        db.SaveChanges();
                    }
                }

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine_Part;
                globalData.messageDetail = ResourceGlobal.Equipment + " " + ResourceMessages.Add_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            globalData.pageTitle = ResourceDisplayName.Machine_Part;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Equipment";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;

            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Equipment.Updated_User_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", mM_MT_Preventive_Equipment.Machine_ID);

            return View(mM_MT_Preventive_Equipment);
        }
        #endregion

        #region Edit Specified Equipment Details
        /*
        * Action Name          : Edit
        * Input Parameter      : id(EQP_ID)
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : used to Load specified Equipment details in form
        */
        // GET: PreventiveEquipment/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Preventive_Equipment mM_MT_Preventive_Equipment = db.MM_MT_Preventive_Equipment.FirstOrDefault(x => x.EQP_ID == id);
            if (mM_MT_Preventive_Equipment == null)
            {
                return HttpNotFound();
            }
            var TBMimage = db.MM_TBM_Image.Where(m => m.EQP_ID == id).Select(m => m.Image_Name).FirstOrDefault();
            if (TBMimage != null)
            {
                ViewBag.FileNameImage = TBMimage;
            }
            else
            {
                ViewBag.FileNameImage = "Image not uploaded ";
            }


            globalData.pageTitle = ResourceDisplayName.Machine_Part;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Equipment";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;

            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Equipment.Updated_User_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", mM_MT_Preventive_Equipment.Machine_ID);
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new { Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")", Machine_ID = a.Machine_ID }),
            //    "Machine_ID", "Machine_Name", mM_MT_Preventive_Equipment.Machine_ID);
            return View(mM_MT_Preventive_Equipment);
        }
        /*
        * Action Name          : Edit
        * Input Parameter      : object of MM_MT_PreventiveEquipment
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : used to Edit equipment and save changes
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_MT_Preventive_Equipment mM_MT_Preventive_Equipment, string Savebtn, string Resetbtn, HttpPostedFileBase upload)
        {



            if (Savebtn != null)
            {
                var isValid = true;
                if (mM_MT_Preventive_Equipment.Warning_At > mM_MT_Preventive_Equipment.Designated_Life)
                {
                    isValid = false;
                    ModelState.AddModelError("Warning_At", "Warning At value to be less than designated life");
                }
                if (mM_MT_Preventive_Equipment.Stop_At <= mM_MT_Preventive_Equipment.Warning_At || mM_MT_Preventive_Equipment.Stop_At > mM_MT_Preventive_Equipment.Designated_Life)
                {
                    isValid = false;
                    ModelState.AddModelError("Stop_At", "Stop At value to be less than or equal to designated life and it should be greater than Warning At");
                }
                if (ModelState.IsValid == true && isValid == true)
                {
                    var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                    mM_MT_Preventive_Equipment.Plant_ID = plantId;
                    mmPreveniveEquipment = db.MM_MT_Preventive_Equipment.FirstOrDefault(x => x.EQP_ID == mM_MT_Preventive_Equipment.EQP_ID);
                    mmPreveniveEquipment.Equipment_Name = mM_MT_Preventive_Equipment.Equipment_Name;
                    mmPreveniveEquipment.Reset_Consumelife = mM_MT_Preventive_Equipment.Reset_Consumelife;
                    mmPreveniveEquipment.Action_taken = mM_MT_Preventive_Equipment.Action_taken;
                    mmPreveniveEquipment.Is_PM_Equipment = mM_MT_Preventive_Equipment.Is_PM_Equipment;
                    mmPreveniveEquipment.Remaining_Life = mM_MT_Preventive_Equipment.Remaining_Life;
                    mmPreveniveEquipment.Machine_ID = mM_MT_Preventive_Equipment.Machine_ID;
                    mmPreveniveEquipment.Designated_Life = mM_MT_Preventive_Equipment.Designated_Life;
                    mmPreveniveEquipment.Life_Per_Cycle = mM_MT_Preventive_Equipment.Life_Per_Cycle;
                    mmPreveniveEquipment.Warning_At = mM_MT_Preventive_Equipment.Warning_At;
                    mmPreveniveEquipment.Stop_At = mM_MT_Preventive_Equipment.Stop_At;
                    mmPreveniveEquipment.IsActive = mM_MT_Preventive_Equipment.IsActive;
                    mmPreveniveEquipment.Sequence_No = mM_MT_Preventive_Equipment.Sequence_No;
                    mmPreveniveEquipment.SAP_Part_No = mM_MT_Preventive_Equipment.SAP_Part_No;
                    mmPreveniveEquipment.Data_Retention_Period = mM_MT_Preventive_Equipment.Data_Retention_Period;
                    mmPreveniveEquipment.Updated_Date = DateTime.Now;
                    mmPreveniveEquipment.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmPreveniveEquipment.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mmPreveniveEquipment.Is_Edited = true;
                    db.Entry(mmPreveniveEquipment).State = EntityState.Modified;
                    db.SaveChanges();

                    var imagecount = db.MM_TBM_Image.Where(m => m.EQP_ID == mM_MT_Preventive_Equipment.EQP_ID).Select(m => m.Image_TBM_ID).FirstOrDefault();


                    MM_TBM_Image mm_image = new MM_TBM_Image();
                    if (imagecount > 0)

                    {
                        mm_img = db.MM_TBM_Image.Find(imagecount);
                        if (upload != null && upload.ContentLength > 0)
                        {
                            using (var reader = new System.IO.BinaryReader(upload.InputStream))
                            {


                                mm_img.Image_Name = System.IO.Path.GetFileName(upload.FileName);
                                mm_img.Image_Type = Path.GetExtension(upload.FileName);
                                mm_img.Content_Type = upload.ContentType;
                                mm_img.Image_Content = reader.ReadBytes(upload.ContentLength);
                                mm_img.Updated_Date = DateTime.Now;
                                mm_img.Updated_Host = HttpContext.Request.UserHostAddress;
                                mm_img.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                mm_img.EQP_ID = mM_MT_Preventive_Equipment.EQP_ID;
                                db.Entry(mm_img).State = EntityState.Modified;
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
                                mm_image.EQP_ID = mM_MT_Preventive_Equipment.EQP_ID;
                                db.MM_TBM_Image.Add(mm_image);
                                db.SaveChanges();
                            }
                        }

                    }





                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceDisplayName.Machine_Part;
                    globalData.messageDetail = ResourceGlobal.Equipment + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }
            else if (Resetbtn != null)
            {
                var plantId = ((FDSession)this.Session["FDSession"]).plantId;

                mmPreveniveEquipment = db.MM_MT_Preventive_Equipment.FirstOrDefault(x => x.EQP_ID == mM_MT_Preventive_Equipment.EQP_ID);
                MM_TBM_Reset_History obj = new MM_TBM_Reset_History();
                obj.TBM_ID = mmPreveniveEquipment.EQP_ID;
                obj.Machine_ID = mmPreveniveEquipment.Machine_ID;
                obj.Plant_ID = plantId;
                obj.Inserted_Date = DateTime.Now;
                obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                obj.Inserted_UserName = ((FDSession)this.Session["FDSession"]).userName;
                obj.Designated_Life = mmPreveniveEquipment.Designated_Life;
                obj.Consumed_Life = (mmPreveniveEquipment.Designated_Life - mmPreveniveEquipment.Remaining_Life);

                db.MM_TBM_Reset_History.Add(obj);
                db.SaveChanges();
                mmPreveniveEquipment.Remaining_Life = mmPreveniveEquipment.Designated_Life;
                db.Entry(mmPreveniveEquipment).State = EntityState.Modified;
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine_Part;
                globalData.messageDetail = ResourceGlobal.Equipment + " " + ResourceMessages.Edit_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }

            globalData.pageTitle = ResourceDisplayName.Machine_Part;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PreventiveEquipment";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;
            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Equipment.Updated_User_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", mM_MT_Preventive_Equipment.Machine_ID);

            return View(mM_MT_Preventive_Equipment);
        }
        #endregion

        public ActionResult ResetTBM(decimal TBMID, string SAPRNo, string remarks)
        {
            if (SAPRNo != "")
            {
                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                mmPreveniveEquipment = db.MM_MT_Preventive_Equipment.FirstOrDefault(x => x.EQP_ID == TBMID);
                MM_TBM_Reset_History obj = new MM_TBM_Reset_History();
                obj.TBM_ID = mmPreveniveEquipment.EQP_ID;
                obj.Machine_ID = mmPreveniveEquipment.Machine_ID;
                obj.Plant_ID = plantId;
                obj.Inserted_Date = DateTime.Now;
                obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                obj.Inserted_UserName = ((FDSession)this.Session["FDSession"]).userName;
                obj.Designated_Life = mmPreveniveEquipment.Designated_Life;
                obj.Consumed_Life = (mmPreveniveEquipment.Designated_Life - mmPreveniveEquipment.Remaining_Life);
                obj.SAP_Reservation_Number = Convert.ToDecimal(SAPRNo);
                obj.Remarks = remarks;
                db.MM_TBM_Reset_History.Add(obj);
                db.SaveChanges();

                mmPreveniveEquipment.Remaining_Life = mmPreveniveEquipment.Designated_Life;
                db.Entry(mmPreveniveEquipment).State = EntityState.Modified;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        #region Delete Specified Equipment

        /*
        * Action Name          : Delete
        * Input Parameter      : id(EQP_ID)
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : load Delete equipment from with details
        */
        // GET: PreventiveEquipment/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Preventive_Equipment mM_MT_Preventive_Equipment = db.MM_MT_Preventive_Equipment.Find(id);
            if (mM_MT_Preventive_Equipment == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceDisplayName.Machine_Part;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Equipment";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;

            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Preventive_Equipment);
        }

        /*
        * Action Name          : Delete
        * Input Parameter      : id(EQP_ID)
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : used to Delete equipment
        */
        // POST: PreventiveEquipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                MM_MT_Preventive_Equipment mM_MT_Preventive_Equipment = db.MM_MT_Preventive_Equipment.Find(id);
                db.MM_MT_Preventive_Equipment.Remove(mM_MT_Preventive_Equipment);

                //if (db.MM_MT_Preventive_Equipment.Where(x => x.EQP_ID == mM_MT_Preventive_Equipment.EQP_ID).Count() > 0)
                //{
                //    db.MM_MT_Preventive_Maintenance.RemoveRange(db.MM_MT_Preventive_Maintenance.Where(x => x.EQP_ID == mM_MT_Preventive_Equipment.EQP_ID));
                //}
                db.SaveChanges();

                globalData.pageTitle = ResourceDisplayName.Machine_Part;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "Equipment";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Equipment + " " + ResourceMessages.Delete_Success;
                globalData.contentFooter = ResourceGlobal.Equipment + " " + ResourceMessages.Delete_Success;

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine_Part;
                globalData.messageDetail = ResourceGlobal.Equipment + " " + ResourceMessages.Delete_Success;

                TempData["globalData"] = globalData;
                ViewBag.GlobalDataModel = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceDisplayName.Machine_Part;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Disposing and Releasing Objects
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        #endregion

        #region Upload File (create Euqipments Against Machine using Excel file upload)

        /*
        * Action Name          : Upload
        * Input Parameter      : None
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action used to show upload form
        */

        //GET: Upload file page load

        public ActionResult Upload()
        {

            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Number");

            globalData.pageTitle = ResourceDisplayName.Machine_Part;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Equipment";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceGlobal.Equipment + " " + ResourceGlobal.Form;

            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        /*
        * Action Name          : Upload
        * Input Parameter      : Files object and MM_MT_PreventiveEquipment obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action used get upload excel file and generationg datatable
        */
        //GET: GET The file from upload control 
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult uploadFile(HttpPostedFileBase files, [Bind(Include = "EQP_ID,Equipment_Name,Machine_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host")] MM_MT_Preventive_Equipment mM_MT_Preventive_Equipment)
        {
            GlobalOperations globalOperations = new GlobalOperations();
            string fileName = Path.GetFileName(files.FileName);
            string fileExtension = Path.GetExtension(files.FileName);
            string fileLocation = Server.MapPath("~/App_Data/" + fileName);
            DataTable dt = globalOperations.ExcelToDataTable(files, fileLocation, fileExtension);
            InsertIntoDataTable(dt, mM_MT_Preventive_Equipment);


            globalData.pageTitle = ResourceDisplayName.Machine_Part;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Equipment";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Equipment + " " + ResourceMessages.Add_Success;
            globalData.contentFooter = ResourceGlobal.Equipment + " " + ResourceMessages.Add_Success;

            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceDisplayName.Machine_Part;
            globalData.messageDetail = ResourceGlobal.Equipment + " " + ResourceMessages.Add_Success;

            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Equipment.Updated_User_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", mM_MT_Preventive_Equipment.Machine_ID);


            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            return View("Index", db.MM_MT_Preventive_Equipment.ToList());
        }
        #endregion

        #region Insert all Data of DataTable into respective Database Table
        /*
        * Action Name          : InsertIntoDataTable
        * Input Parameter      : Datatable and MM_MT_Machines obj
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Action used to add new Machines into database table which are present in datattable under plant with shop and line 
        */
        private bool InsertIntoDataTable(DataTable dt, MM_MT_Preventive_Equipment mM_MT_Preventive_Equipment)
        {
            try
            {

                foreach (DataRow dr in dt.Rows)
                {

                    // MM_MT_Preventive_Equipment mM_MT_Preventive_Equipment = new MM_MT_Preventive_Equipment();
                    mM_MT_Preventive_Equipment.Inserted_Date = DateTime.Now;
                    mM_MT_Preventive_Equipment.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mM_MT_Preventive_Equipment.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                    //mM_MT_Preventive_Equipment.Machine_ID = Convert.ToDecimal(dr["Machine_ID"].ToString().Trim());
                    mM_MT_Preventive_Equipment.Equipment_Name = Convert.ToString(dr["Equipment_Name"].ToString().Trim());
                    //mM_MT_Preventive_Equipment.Is_Deleted = Convert.ToBoolean(dr["Is_Deleted"]);
                    //mM_MT_Preventive_Equipment.Is_Purgeable = Convert.ToBoolean(dr["Is_Purgeable"]);
                    //mM_MT_Preventive_Equipment.Is_Transfered = Convert.ToBoolean(dr["Is_Transfered"]);


                    db.MM_MT_Preventive_Equipment.Add(mM_MT_Preventive_Equipment);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
            return true;
        }
        #endregion

        #region NEW Code
        public JsonResult isEquipmentValid(string Equipment_Name, decimal? Machine_ID, string InitialEquipmentName)
        {
            if (InitialEquipmentName.Equals(Equipment_Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            if (Machine_ID != null && Equipment_Name != null)
            {
                if (String.IsNullOrWhiteSpace(Equipment_Name))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(!(db.MM_MT_Preventive_Equipment.Any(x => x.Equipment_Name == Equipment_Name && x.Machine_ID == Machine_ID)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}

