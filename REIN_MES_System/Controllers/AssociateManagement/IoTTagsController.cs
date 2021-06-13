using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;

namespace ZHB_AD.Controllers.OrderManagement
{
    public class IoTTagsController : BaseController
    {
        private ZHB_ADEntities db = new ZHB_ADEntities();
        MM_IoT_Tags mmIotTagObj = new MM_IoT_Tags();
        GlobalData globalData = new GlobalData();

        int plantId = 0, shopId = 0;

        General generalObj = new General();

        // GET: IoTTags
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.IOT_Tags;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "IoTTags";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.IOT_Tags + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.IOT_Tags + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var mM_IoT_Tags = db.MM_IoT_Tags.Include(m => m.MM_Employee).Include(m => m.MM_Employee1).Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations);
            return View(mM_IoT_Tags.ToList());
        }

        // GET: IoTTags/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_IoT_Tags mM_IoT_Tags = db.MM_IoT_Tags.Find(id);
            if (mM_IoT_Tags == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.IOT_Tags + " " + ResourceGlobal.Details;
            globalData.subTitle = ResourceModules.IOT_Tags + " " + ResourceGlobal.Details;
            globalData.controllerName = "IoTTags";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.IOT_Tags + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.IOT_Tags + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_IoT_Tags);
        }

        // GET: IoTTags/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceGlobal.Add + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.subTitle = ResourceGlobal.Add + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.controllerName = "IoTTags";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(S => S.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", 0);
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(S => S.Station_ID == 0), "Station_ID", "Station_Name");
            return View();
        }

        // POST: IoTTags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Tag_ID,Tag_Name,Plant_ID,Shop_ID,Line_ID,Station_ID,Is_Line_Stop,Is_Line_Resume,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Tag_Type")] MM_IoT_Tags mM_IoT_Tags)
        {
            if (ModelState.IsValid)
            {
                string tagName = mM_IoT_Tags.Tag_Name;

                if (mM_IoT_Tags.IsTagNameExists(tagName))
                {
                    ModelState.AddModelError("Tag_Name", ResourceValidation.Exist);
                }
                else
                {
                    mM_IoT_Tags.Inserted_Date = DateTime.Now;
                    mM_IoT_Tags.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    // mM_IoT_Tags.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mM_IoT_Tags.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    // mM_IoT_Tags.Shop_ID = ((FDSession)this.Session["FDSession"]).shopId;               
                    int tagType = mM_IoT_Tags.Tag_Type;
                    if (tagType == 1)
                    {
                        mM_IoT_Tags.Is_Line_Stop = true;
                    }
                    else
                    {
                        mM_IoT_Tags.Is_Line_Stop = false;
                    }
                    if (tagType == 2)
                    {
                        mM_IoT_Tags.Is_Line_Resume = true;
                    }
                    else
                    {
                        mM_IoT_Tags.Is_Line_Resume = false;
                    }
                    db.MM_IoT_Tags.Add(mM_IoT_Tags);
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.IOT_Tags;
                    globalData.messageDetail = ResourceModules.IOT_Tags + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceGlobal.Add + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.subTitle = ResourceGlobal.Add + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.controllerName = "IoTTag";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_IoT_Tags.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_IoT_Tags.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_IoT_Tags.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_IoT_Tags.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_IoT_Tags.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_IoT_Tags.Station_ID);
            return View(mM_IoT_Tags);
        }

        // GET: IoTTags/Edit/5
        public ActionResult Edit(decimal id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_IoT_Tags mM_IoT_Tags = db.MM_IoT_Tags.Find(id);
            if (mM_IoT_Tags == null)
            {
                return HttpNotFound();
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            //var lineStop = from b in MM_IoT_Tags
            //               where b.Tag_ID == id
            //            select b; 
            // int lineStop=

            String tagSelection = "0";
            if (mM_IoT_Tags.Is_Line_Stop == true)
            {
                tagSelection = "1";
            }
            else
                if (mM_IoT_Tags.Is_Line_Resume == true)
                {
                    tagSelection = "2";
                }

            globalData.pageTitle = ResourceGlobal.Edit + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.subTitle = ResourceGlobal.Add + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.controllerName = "IoTTag";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Tag_Type = new SelectList(new List<object> { new { value = "0", text = "-Select Tag Type-" }, new { value = "1", text = "Is Line Stop" }, new { value = "2", text = "Is Line Resume" } }, "value", "text", tagSelection);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_IoT_Tags.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_IoT_Tags.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_IoT_Tags.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_IoT_Tags.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_IoT_Tags.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_IoT_Tags.Station_ID);
            return View(mM_IoT_Tags);
        }

        // POST: IoTTags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Tag_ID,Tag_Name,Plant_ID,Shop_ID,Line_ID,Station_ID,Is_Line_Stop,Is_Line_Resume,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Tag_Type")] MM_IoT_Tags mM_IoT_Tags)
        {
            try
            {
                mmIotTagObj = new MM_IoT_Tags();
                if (ModelState.IsValid)
                {
                    string tagName = mM_IoT_Tags.Tag_Name;
                    int tagId =Convert.ToInt32(mM_IoT_Tags.Tag_ID);
                    if (mM_IoT_Tags.IsTagNameExistEdit(tagName, tagId))
                    {
                        ModelState.AddModelError("Tag_Name", ResourceValidation.Exist);
                    }
                    else
                    {
                        mmIotTagObj = db.MM_IoT_Tags.Find(mM_IoT_Tags.Tag_ID);
                        mmIotTagObj.Shop_ID = mM_IoT_Tags.Shop_ID;
                        mmIotTagObj.Line_ID = mM_IoT_Tags.Line_ID;
                        mmIotTagObj.Station_ID = mM_IoT_Tags.Station_ID;
                        //mmIotTagObj.Tag_Type = mM_IoT_Tags.Tag_Type;
                        mmIotTagObj.Tag_Name = mM_IoT_Tags.Tag_Name;
                        mmIotTagObj.Is_Edited = true;
                        plantId = ((FDSession)this.Session["FDSession"]).plantId;

                        mmIotTagObj.Inserted_Date = db.MM_IoT_Tags.Find(mM_IoT_Tags.Tag_ID).Inserted_Date;
                        mmIotTagObj.Inserted_User_ID = db.MM_IoT_Tags.Find(mM_IoT_Tags.Tag_ID).Inserted_User_ID;
                        //mmIoTTagsObj.Inserted_Host = db.MM_DispatchDetails.Find(mM_DispatchDetails.Plan_ID).Inserted_Host;


                        mmIotTagObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmIotTagObj.Updated_Date = DateTime.Now;
                        // mmIoTTagsObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        int tagType = mM_IoT_Tags.Tag_Type;
                        if (tagType == 1)
                        {
                            mmIotTagObj.Is_Line_Stop = true;
                        }
                        else
                        {
                            mmIotTagObj.Is_Line_Stop = false;
                        }
                        if (tagType == 2)
                        {
                            mmIotTagObj.Is_Line_Resume = true;
                        }
                        else
                        {
                            mmIotTagObj.Is_Line_Resume = false;
                        }
                        mmIotTagObj.Is_Edited = true;
                        db.Entry(mmIotTagObj).State = EntityState.Modified;
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.IOT_Tags;
                        globalData.messageDetail = ResourceModules.IOT_Tags + " " + ResourceMessages.Edit_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }

            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.IOT_Tags;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");

            }
            globalData.pageTitle = ResourceGlobal.Edit + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.subTitle = ResourceGlobal.Edit + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.controllerName = "IoTTag";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_IoT_Tags.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_IoT_Tags.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_IoT_Tags.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_IoT_Tags.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_IoT_Tags.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_IoT_Tags.Station_ID);
            return View(mM_IoT_Tags);
        }

        // GET: IoTTags/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_IoT_Tags mM_IoT_Tags = db.MM_IoT_Tags.Find(id);
            if (mM_IoT_Tags == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.IOT_Tags;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "IoTTag";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.IOT_Tags + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_IoT_Tags);
        }

        // POST: IoTTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_IoT_Tags mM_IoT_Tags = db.MM_IoT_Tags.Find(id);
            try
            {
                db.MM_IoT_Tags.Remove(mM_IoT_Tags);
                db.SaveChanges();
                
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_IoT_Tags", "Tag_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.IOT_Tags;
                globalData.messageDetail = ResourceModules.IOT_Tags + " " + ResourceMessages.Delete_Success;

                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.IOT_Tags;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", mM_IoT_Tags);

            }

        }

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
