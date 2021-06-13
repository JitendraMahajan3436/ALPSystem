using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using REIN_MES_System.Controllers.BaseManagement;
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.PlantConfiguration
{
    /* Controller Name            : LineController
    *  Description                : Line controller is used to add/edit/delete/display the line of shop
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class LineController : BaseController
    {
        //private REIN_SOLUTION_MEntities MTdb = new REIN_SOLUTION_MEntities();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        RS_Lines mmLinesObj = new RS_Lines();
        int plantId = 0, lineId = 0, lineTypeId = 0, shopId = 0;
        String lineName = "";

        General generalObj = new General();

        /* Action Name                : Index
        *  Description                : Get the list of lines added
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Line/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Line_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Line";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            //var RS_Lines = db.RS_Lines.Include(m => m.RS_Line_Types).Include(m => m.RS_Shops).Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Line_Types1);
            //var RS_Lines = db.RS_Lines.Include(m => m.RS_Shops.Plant_ID == plantID);
            var RS_Lines = from l in db.RS_Lines join s in db.RS_Shops on l.Shop_ID equals s.Shop_ID where s.Plant_ID == plantID select l;
            return View(RS_Lines.ToList());
        }

        /* Action Name                : Details
        *  Description                : Get the detail of lines by line id
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Line/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Lines RS_Lines = db.RS_Lines.Find(id);
            if (RS_Lines == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Line_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Line";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Line + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Line + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Lines);
        }

        /* Action Name                : Create
        *  Description                : Action used to add new line under plant with shop and line type
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Line/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Line_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Line";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Shop_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name");
            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Line_Type_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name");
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Plant_ID =  plantId;
            return View();
        }

        // POST: /Line/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        /* Action Name                : Create
        *  Description                : Create the line. Validate the line is already added or not
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Lines
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Lines RS_Lines)
        {
            if (ModelState.IsValid)
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                shopId = Convert.ToInt16(RS_Lines.Shop_ID);
                //lineTypeId = Convert.ToInt16(RS_Lines.Line_Type_Id);
                lineName = RS_Lines.Line_Name;
                if (RS_Lines.isLineExists(lineName, shopId, 0, plantId))
                {
                    ModelState.AddModelError("Line_Name", ResourceValidation.Exist);
                }

                else
                {
                    if (RS_Lines.isPLC == null)
                    {
                        RS_Lines.isPLC = false;
                    }

                    if (RS_Lines.Is_Conveyor == true)
                    {
                        // calculate TACT time
                        RS_Lines.Tact_Time_Sec = Convert.ToInt32(RS_Lines.TACT_Time.Value.TotalSeconds);
                    }

                    RS_Lines.Inserted_Date = DateTime.Now;
                    RS_Lines.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Lines.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Lines.Add(RS_Lines);
                    db.SaveChanges();

                   // //Save line records into MTTUW lines DB
                   // MTLines.Line_ID = RS_Lines.Line_ID;
                   //// MTLines.Allow_NoScan_LineStop = RS_Lines.Allow_NoScan_LineStop;
                   // MTLines.Current_Stoppage_Seconds = RS_Lines.Current_Stoppage_Seconds;
                   // MTLines.Inserted_Date = RS_Lines.Inserted_Date;
                   // MTLines.Inserted_Host = RS_Lines.Inserted_Host;
                   // MTLines.Inserted_User_ID = RS_Lines.Inserted_User_ID;
                   // MTLines.isLineStop = RS_Lines.isLineStop;
                   // MTLines.isPLC = RS_Lines.isPLC;
                   // MTLines.Is_Conveyor = RS_Lines.Is_Conveyor;
                   // MTLines.Is_Shop_Line_End = RS_Lines.Is_Shop_Line_End;
                   // MTLines.Is_Shop_Line_Start = RS_Lines.Is_Shop_Line_Start;
                   // MTLines.Is_Sub_Assembly = RS_Lines.Is_Sub_Assembly;
                   // MTLines.Is_Transferred = RS_Lines.Is_Transferred;
                   // MTLines.LineMove_Time = RS_Lines.LineMove_Time;
                   // MTLines.LineStart_Time = RS_Lines.LineStart_Time;
                   // MTLines.LineStopStation_ID = RS_Lines.LineStopStation_ID;
                   // MTLines.Line_Code = RS_Lines.Line_Code;
                   // MTLines.Line_Description = RS_Lines.Line_Description;
                   // MTLines.Line_Name = RS_Lines.Line_Name;
                   // MTLines.Line_Type_Id = RS_Lines.Line_Type_Id;
                   // MTLines.Plant_ID = RS_Lines.Plant_ID;
                   // MTLines.Shop_ID = RS_Lines.Shop_ID;
                   // MTLines.TACT_Time = RS_Lines.TACT_Time;
                   // MTLines.Tact_Time_Sec = RS_Lines.Tact_Time_Sec;
                   // MTLines.Is_Edited = RS_Lines.Is_Edited;
                   // MTLines.Is_Purgeable = RS_Lines.Is_Purgeable;
                   // MTdb.MM_MTTUW_Lines.Add(MTLines);
                   // MTdb.SaveChanges();
                   // //End

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Line;
                    globalData.messageDetail = ResourceModules.Line + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }
            globalData.pageTitle = ResourceModules.Line_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Line";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = Convert.ToInt16(RS_Lines.Plant_ID);

            ViewBag.Shop_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name", RS_Lines.Shop_ID);
            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Lines.Shop_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Lines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Lines.Updated_User_ID);
            ViewBag.Line_Type_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name", RS_Lines.Line_Type_Id);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.DefaultIfEmpty(), "Plant_ID", "Plant_Name", RS_Lines.Plant_ID);
            ViewBag.Is_Conveyor_Selected = RS_Lines.Is_Conveyor;
            ViewBag.Selected_Plant_ID = RS_Lines.Plant_ID;
            return View(RS_Lines);
        }

        /* Action Name                : Edit
        *  Description                : Show the edit line form
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Line/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Lines RS_Lines = db.RS_Lines.Find(id);
            if (RS_Lines == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Line_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Line";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == RS_Lines.Shop_ID).SingleOrDefault();
            //var obj = from shop in db.RS_Shops
            //          where shop.Shop_ID == RS_Lines.Shop_Id
            //          select shop.Plant_ID;

            //var student = obj.SingleOrDefault<RS_Shops>();

            plantId = Convert.ToInt16(shopObj.Plant_ID);
            RS_Lines.Plant_ID = plantId;

            ViewBag.Shop_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name", RS_Lines.Shop_ID);
            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Lines.Shop_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Lines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Lines.Updated_User_ID);
            ViewBag.Line_Type_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name", RS_Lines.Line_Type_Id);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Is_Conveyor_Selected = RS_Lines.Is_Conveyor;
            return View(RS_Lines);
        }

        // POST: /Line/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        /* Action Name                : Edit
        *  Description                : Action is used to edit the line
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Lines
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Lines RS_Lines)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                shopId = Convert.ToInt16(RS_Lines.Shop_ID);
                //lineTypeId = Convert.ToInt16(RS_Lines.Line_Type_Id);
                lineName = RS_Lines.Line_Name;
                lineId = Convert.ToInt16(RS_Lines.Line_ID);
                
                if (RS_Lines.Shop_ID == null || RS_Lines.Shop_ID == 0)
                {
                    ModelState.AddModelError("Shop_Name", ResourceValidation.Required);
                }
                else if (RS_Lines.isLineExists(lineName, shopId, lineId, plantId))
                {
                    ModelState.AddModelError("Line_Name", ResourceValidation.Exist);
                }

                //else
                //    //if ((RS_Lines.Is_Shop_Line_Start == true) && (RS_Lines.isShopLineStartExists(shopId, lineId)))
                //    //{
                //    //    ModelState.AddModelError("Is_Shop_Line_Start", ResourceLine.Line_Error_Shop_Line_Start_Exists);
                //    //}
                //    //else
                //        if ((RS_Lines.Is_Shop_Line_End == true) && (RS_Lines.isShopLineEndExists(shopId, lineId)))
                //        {
                //            ModelState.AddModelError("Is_Shop_Line_End", ResourceLine.Line_Error_Shop_Line_End_Exists);
                //        }
                else
                {
                    if (RS_Lines.isLineCodeExists(RS_Lines.Line_Code, shopId, lineId, plantId))
                    {
                        ModelState.AddModelError("Line_Code", ResourceValidation.Exist);
                    }
                    else
                    {
                        mmLinesObj = db.RS_Lines.Find(RS_Lines.Line_ID);
                        mmLinesObj.Line_Name = RS_Lines.Line_Name;
                        mmLinesObj.Line_Description = RS_Lines.Line_Description;
                        mmLinesObj.Shop_ID = RS_Lines.Shop_ID;
                        mmLinesObj.Line_Code = RS_Lines.Line_Code;
                        //mmLinesObj.Line_Type_Id = RS_Lines.Line_Type_Id;
                        mmLinesObj.Is_Conveyor = RS_Lines.Is_Conveyor;
                        mmLinesObj.TACT_Time = RS_Lines.TACT_Time;
                        mmLinesObj.Is_Shop_Line_Start = RS_Lines.Is_Shop_Line_Start;
                        mmLinesObj.Is_Shop_Line_End = RS_Lines.Is_Shop_Line_End;
                        mmLinesObj.Is_Buildsheet = RS_Lines.Is_Buildsheet;
                        mmLinesObj.Is_PRN = RS_Lines.Is_PRN;
                        if (RS_Lines.isPLC == null || RS_Lines.isPLC == false)
                        {
                            mmLinesObj.isPLC = false;
                        }
                        else
                        {
                            mmLinesObj.isPLC = true;
                        }

                        if (RS_Lines.Is_Conveyor == true)
                        {
                            mmLinesObj.Tact_Time_Sec = Convert.ToInt32(RS_Lines.TACT_Time.Value.TotalSeconds);
                        }
                        else
                        {
                            mmLinesObj.isPLC = false;
                            mmLinesObj.Tact_Time_Sec = 0;

                        }
                        mmLinesObj.Is_Edited = true;
                        mmLinesObj.Updated_Date = DateTime.Now;
                        mmLinesObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmLinesObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.Entry(mmLinesObj).State = EntityState.Modified;
                        db.SaveChanges();

                        ////Save line records into MTTUW lines DB
                        //MTLines = MTdb.MM_MTTUW_Lines.Find(mmLinesObj.Line_ID);
                        //MTLines.Allow_NoScan_LineStop = mmLinesObj.Allow_NoScan_LineStop;
                        //MTLines.Current_Stoppage_Seconds = mmLinesObj.Current_Stoppage_Seconds;
                        //MTLines.Inserted_Date = mmLinesObj.Inserted_Date;
                        //MTLines.Inserted_Host = mmLinesObj.Inserted_Host;
                        //MTLines.Inserted_User_ID = mmLinesObj.Inserted_User_ID;
                        //MTLines.isLineStop = mmLinesObj.isLineStop;
                        //MTLines.isPLC = mmLinesObj.isPLC;
                        //MTLines.Is_Conveyor = mmLinesObj.Is_Conveyor;
                        //MTLines.Is_Shop_Line_End = mmLinesObj.Is_Shop_Line_End;
                        //MTLines.Is_Shop_Line_Start = mmLinesObj.Is_Shop_Line_Start;
                        //MTLines.Is_Sub_Assembly = mmLinesObj.Is_Sub_Assembly;
                        //MTLines.Is_Transferred = mmLinesObj.Is_Transferred;
                        //MTLines.LineMove_Time = mmLinesObj.LineMove_Time;
                        //MTLines.LineStart_Time = mmLinesObj.LineStart_Time;
                        //MTLines.LineStopStation_ID = mmLinesObj.LineStopStation_ID;
                        //MTLines.Line_Code = mmLinesObj.Line_Code;
                        //MTLines.Line_Description = mmLinesObj.Line_Description;
                        //MTLines.Line_Name = mmLinesObj.Line_Name;
                        //MTLines.Line_Type_Id = mmLinesObj.Line_Type_Id;
                        //MTLines.Shop_ID = mmLinesObj.Shop_ID;
                        //MTLines.TACT_Time = mmLinesObj.TACT_Time;
                        //MTLines.Tact_Time_Sec = mmLinesObj.Tact_Time_Sec;
                        //MTLines.Is_Edited = mmLinesObj.Is_Edited;
                        //MTLines.Is_Purgeable = mmLinesObj.Is_Purgeable;
                        //MTdb.Entry(MTLines).State = EntityState.Modified;
                        //MTdb.SaveChanges();
                        ////End

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Line;
                        globalData.messageDetail = ResourceModules.Line + " " + ResourceMessages.Edit_Success;
                        TempData["globalData"] = globalData;

                        return RedirectToAction("Index");
                    }
                }
            }

            //RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == RS_Lines.Shop_ID).SingleOrDefault();
            //plantId = Convert.ToInt16(shopObj.Plant_ID);

            RS_Lines.Plant_ID = plantId;

            globalData.pageTitle = ResourceModules.Line_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Line";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Shop_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name", RS_Lines.Shop_ID);
            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Lines.Shop_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Lines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Lines.Updated_User_ID);
            ViewBag.Line_Type_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name", RS_Lines.Line_Type_Id);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p=>p.Plant_ID==plantId), "Plant_ID", "Plant_Name", RS_Lines.Plant_ID);
            ViewBag.Is_Conveyor_Selected = RS_Lines.Is_Conveyor;
            return View(RS_Lines);
        }

        /* Action Name                : Delete
        *  Description                : Action is used to show the delete line page for user confirmation
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Line/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Lines RS_Lines = db.RS_Lines.Find(id);
            if (RS_Lines == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Line_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Line";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Lines);
        }

        /* Action Name                : DeleteConfirmed
        *  Description                : Action is used to delete line
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Line/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Lines RS_Lines = db.RS_Lines.Find(id);
            try
            {
                db.RS_Lines.Remove(RS_Lines);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Lines", "Line_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Line;
                globalData.messageDetail = ResourceModules.Line + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Lines", "Line_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                //globalData.dbUpdateExceptionDetail = ex.InnerException.InnerException.Message.ToString();

                globalData.isAlertMessage = true;
                globalData.messageTitle = ResourceModules.Line;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Line;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete", RS_Lines);
            }
        }

        /* Action Name                : Dispose
        *  Description                : Disponse the Line controller object
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : disposing
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /* Action Name                : GetLineByShopID
        *  Description                : Action is used to return the list of lines in json format
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : shopId (shop id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetLineByShopID(int shopId, Boolean? online = null, Boolean? offline = null)
        {
            try
            {
                var st = from line in db.RS_Lines
                         where line.Shop_ID == shopId
                         orderby line.Line_Name
                         select new
                         {
                             Id = line.Line_ID,
                             Value = line.Line_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* Action Name                : GetMarriageLineByShopID
        *  Description                : Action is used to return the list of lines. If lineId is provided it returns all line 
        including end line, excluding provided line. If line id is not provided, it returns all line except end line.
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : shopId, lineId
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetMarriageLineByShopID(int shopId, int lineId)
        {
            try
            {
                if (lineId == 0)
                {
                    var st = from line in db.RS_Lines
                             where line.Shop_ID == shopId && (line.Is_Shop_Line_End == false || line.Is_Shop_Line_End == null)
                             select new
                             {
                                 Id = line.Line_ID,
                                 Value = line.Line_Name,
                             };
                    return Json(st, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var st = from line in db.RS_Lines
                    //         where line.Shop_ID == shopId && line.Line_ID != lineId && (line.Is_Shop_Line_Start == false || line.Is_Shop_Line_Start == null)
                    //         select new
                    //         {
                    //             Id = line.Line_ID,
                    //             Value = line.Line_Name,
                    //         };

                    var st = from line in db.RS_Lines
                             where line.Shop_ID == shopId && line.Line_ID != lineId
                             select new
                             {
                                 Id = line.Line_ID,
                                 Value = line.Line_Name,
                             };
                    return Json(st, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* Action Name                : GetLineStationForMarriageByLineID
        *  Description                : Action is used to return the start or end station for route marriage configuration.
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : shopId, isStartStation
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetLineStationForMarriageByLineID(int lineId, int isStartStation)
        {
            try
            {
                if (isStartStation == 0)
                {
                    // require end station of line
                    var st = from station in db.RS_Stations
                             where (from routeConfiguration in db.RS_Route_Configurations where routeConfiguration.Is_End_Station == true && routeConfiguration.Line_ID == lineId select routeConfiguration.Station_ID).Contains(station.Station_ID)
                             select new
                             {
                                 Id = station.Station_ID,
                                 Value = station.Station_Name,
                             };
                    return Json(st, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    // require end station
                    var st = from station in db.RS_Stations
                             where (from routeConfiguration in db.RS_Route_Configurations where routeConfiguration.Is_Start_Station == true && routeConfiguration.Line_ID == lineId select routeConfiguration.Station_ID).Contains(station.Station_ID)
                             select new
                             {
                                 Id = station.Station_ID,
                                 Value = station.Station_Name,
                             };
                    return Json(st, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /* Action Name                : GetEndLineByShopID
        *  Description                : Action is used to return the end line and last station of end line.
        including end line, excluding provided line. If line id is not provided, it returns all line except end line.
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : shopId
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetEndLineByShopID(int shopId)
        {
            try
            {
                var st = from line in db.RS_Lines
                         where line.Shop_ID == shopId && (line.Is_Shop_Line_End == true)
                         select new
                         {
                             Id = line.Line_ID,
                             Value = line.Line_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
