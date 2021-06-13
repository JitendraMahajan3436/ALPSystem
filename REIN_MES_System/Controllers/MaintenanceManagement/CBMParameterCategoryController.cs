using ZHB_AD.App_LocalResources;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class CBMParameterCategoryController : BaseController
    {
        #region Variables declaration
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        public GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        MM_CBM_Parameter_Category cbmpc = new MM_CBM_Parameter_Category();
        #endregion

        /*
         * Action Name          : Index
         * Input Parameter      : None
         * Return Type          : ActionResult
         * Author & Time Stamp  : Ajay Wagh
         * Description          : Get the list of Machines Parameter Category
         */
        // GET: CBMParameterCategory
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var mM_CBM_Parameter_Category = db.MM_CBM_Parameter_Category;

            globalData.pageTitle = ResourceModules.CBMPC_Config;
            globalData.controllerName = "CBMParameterCategory";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            // TempData["globalData"] = globalData;

            var plantId = ((FDSession)this.Session["FDSession"]).plantId;

            return View(mM_CBM_Parameter_Category.ToList());
        }

        /*
        * Action Name          : Details
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Get Details of Machines(Conditional Based Maintenance) Parameter Category
        */
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_CBM_Parameter_Category mM_CBM_Parameter_Category = db.MM_CBM_Parameter_Category.Find(id);
            if (mM_CBM_Parameter_Category == null)
            {
                return HttpNotFound();
            }
            
            globalData.pageTitle = ResourceModules.CBMPC_Config;
            globalData.controllerName = "CBMParameterCategory";
            globalData.actionName = ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_CBM_Parameter_Category);
        }

        /*
        * Action Name          : Create
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Add Machines (Conditional Based Maintenance) Parameter Category
        */
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            
            globalData.pageTitle = ResourceModules.CBMPC_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "CBMParameterCategory";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.C_ID =  new SelectList(db.MM_Parameter_Category, "C_ID", "C_Name");
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        /*
      * Action Name          : Create
      * Input Parameter      : MM_Conditional_Based_Maintenance object
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Add Machines (Conditional Based Maintenance) Parameter Category
      */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Category_ID,Category_Name,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,C_ID,Image_ID")] MM_CBM_Parameter_Category mM_CBM_Parameter_Category,HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                    if (file != null && file.ContentLength > 0)
                    {
                        using (var reader = new System.IO.BinaryReader(file.InputStream))
                        {
                            MM_CBM_Category_Image obj = new MM_CBM_Category_Image();
                            obj.Image_Name = System.IO.Path.GetFileName(file.FileName);
                            obj.Image_Type = Path.GetExtension(file.FileName);
                            obj.Content_Type = file.ContentType;
                            obj.Image_Content = reader.ReadBytes(file.ContentLength);
                            obj.Inserted_Date = DateTime.Now;
                            obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.MM_CBM_Category_Image.Add(obj);
                            db.SaveChanges();

                            mM_CBM_Parameter_Category.Image_ID = obj.Image_ID;
                            mM_CBM_Parameter_Category.Image_Name = System.IO.Path.GetFileName(file.FileName);
                            mM_CBM_Parameter_Category.Image_Type = Path.GetExtension(file.FileName);
                            mM_CBM_Parameter_Category.Content_Type = file.ContentType;
                            mM_CBM_Parameter_Category.Image_Content = reader.ReadBytes(file.ContentLength);
                        }
                    }
                     
                    mM_CBM_Parameter_Category.Inserted_Date = DateTime.Now;
                    mM_CBM_Parameter_Category.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mM_CBM_Parameter_Category.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.MM_CBM_Parameter_Category.Add(mM_CBM_Parameter_Category);
                    db.SaveChanges();
                    ViewBag.C_ID = new SelectList(db.MM_Parameter_Category, "C_ID", "C_Name",mM_CBM_Parameter_Category.C_ID);
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.CBM;
                    globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CBM;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
            }


            globalData.pageTitle = ResourceModules.CBMPC_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "CBMParameterCategory";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.C_ID = new SelectList(db.MM_Parameter_Category, "C_ID", "C_Name", mM_CBM_Parameter_Category.C_ID);

            return View(mM_CBM_Parameter_Category);
        }

        /*
      * Action Name          : Edit
      * Input Parameter      : id
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Load Conditional Based Maintenance Parameter Category edit form
      */
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_CBM_Parameter_Category mM_CBM_Parameter_Category = db.MM_CBM_Parameter_Category.Find(id);
            if (mM_CBM_Parameter_Category == null)
            {
                return HttpNotFound();
            }
            
            globalData.pageTitle = ResourceModules.CBMPC_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "CBMParameterCategory";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.FileName = mM_CBM_Parameter_Category.Image_Name;
            ViewBag.C_ID = new SelectList(db.MM_Parameter_Category, "C_ID", "C_Name", mM_CBM_Parameter_Category.C_ID);
            return View(mM_CBM_Parameter_Category);
        }

        /*
        * Action Name          : Edit
        * Input Parameter      : MM_Conditional_Based_Maintenance 
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Edit Conditional Based Maintenance Parameter Category of Machine
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Category_ID,Category_Name,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,C_ID,Image_ID")] MM_CBM_Parameter_Category mM_CBM_Parameter_Category, HttpPostedFileBase files)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    cbmpc = db.MM_CBM_Parameter_Category.Find(mM_CBM_Parameter_Category.Category_ID);
                    MM_CBM_Category_Image obj1 = new MM_CBM_Category_Image();
                    if (files != null && files.ContentLength > 0)
                    {
                        using (var reader = new System.IO.BinaryReader(files.InputStream))
                        {

                            MM_CBM_Category_Image obj = db.MM_CBM_Category_Image.Find(cbmpc.Image_ID);
                            
                            if (obj == null)
                            {
                                obj1.Image_Name = System.IO.Path.GetFileName(files.FileName);
                                obj1.Image_Type = Path.GetExtension(files.FileName);
                                obj1.Content_Type = files.ContentType;
                                obj1.Image_Content = reader.ReadBytes(files.ContentLength);
                                obj1.Inserted_Date = DateTime.Now;
                                obj1.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                obj1.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                db.MM_CBM_Category_Image.Add(obj1);
                                db.SaveChanges();
                            }
                            else
                            {
                                obj.Image_Name = System.IO.Path.GetFileName(files.FileName);
                                obj.Image_Type = Path.GetExtension(files.FileName);
                                obj.Content_Type = files.ContentType;
                                obj.Image_Content = reader.ReadBytes(files.ContentLength);
                                obj.Updated_Date = DateTime.Now;
                                obj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                obj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                obj.Is_Edited = true;
                                db.Entry(obj).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            cbmpc.Image_Name = System.IO.Path.GetFileName(files.FileName);
                            cbmpc.Image_Type = Path.GetExtension(files.FileName);
                            cbmpc.Content_Type = files.ContentType;
                            cbmpc.Image_Content = reader.ReadBytes(files.ContentLength);
                        }
                    }
                    cbmpc.Category_Name = mM_CBM_Parameter_Category.Category_Name;
                    cbmpc.C_ID = mM_CBM_Parameter_Category.C_ID;
                    cbmpc.Image_ID = cbmpc.Image_ID > 0 ? cbmpc.Image_ID : obj1.Image_ID;
                    cbmpc.Updated_Date = DateTime.Now;
                    cbmpc.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    cbmpc.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    cbmpc.Is_Purgeable = mM_CBM_Parameter_Category.Is_Purgeable;
                    cbmpc.Is_Transfered = mM_CBM_Parameter_Category.Is_Transfered;
                    cbmpc.Is_Edited = true;
                    db.Entry(cbmpc).State = EntityState.Modified;
                    db.SaveChanges();

                    
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.CBM;
                    globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;
                    ViewBag.GlobalDataModel = globalData;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exp)
            {

                generalHelper.addControllerException(exp, "CBMParameterCategoryController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CBMPC_Config;
                globalData.messageDetail = exp.Message;
                this.Session["globalData"] = globalData;
            }
            
            globalData.pageTitle = ResourceModules.CBMPC_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "CBMParameterCategory";
            globalData.actionName = ResourceGlobal.Edit;
            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.C_ID = new SelectList(db.MM_Parameter_Category, "C_ID", "C_Name", mM_CBM_Parameter_Category.C_ID);
            return View(mM_CBM_Parameter_Category);
        }

        /*
      * Action Name          : Delete
      * Input Parameter      : id
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : load specified machine parameter category for deletion 
      */

        // GET: CBMParameterCategory/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_CBM_Parameter_Category mM_CBM_Parameter_Category = db.MM_CBM_Parameter_Category.Find(id);
            if (mM_CBM_Parameter_Category == null)
            {
                return HttpNotFound();
            }
            
            globalData.pageTitle = ResourceModules.CBMPC_Config;
            globalData.controllerName = "CBMParameterCategory";
            globalData.actionName = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(mM_CBM_Parameter_Category);
        }

        /*
     * Action Name          : Delete
     * Input Parameter      : id
     * Return Type          : ActionResult
     * Author & Time Stamp  : Ajay Wagh
     * Description          : Delete machine parameter category 
     */
        // POST: CBMParameterCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                MM_CBM_Parameter_Category mM_CBM_Parameter_Category = db.MM_CBM_Parameter_Category.Find(id);
                db.MM_CBM_Parameter_Category.Remove(mM_CBM_Parameter_Category);
                db.SaveChanges();


                globalData.pageTitle = ResourceModules.CBMPC_Config;
                globalData.controllerName = "CBMParameterCategory";
                globalData.actionName = ResourceGlobal.Delete;

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.CBMPC;
                globalData.messageDetail = ResourceModules.CBMPC + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
                ViewBag.GlobalDataModel = globalData;
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.CBMPC;
                globalData.messageDetail = ex.Message.ToString();
                TempData["globalData"] = globalData;
                ViewBag.GlobalDataModel = globalData;
            }
            globalData.pageTitle = ResourceModules.CBMPC_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "CBMParameterCategory";
            globalData.actionName = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return RedirectToAction("Index");
        }

        /*
         * Action Name          : Dispose
         * Input Parameter      : true/false
         * Return Type          : Void
         * Author & Time Stamp  : Ajay Wagh
         */
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