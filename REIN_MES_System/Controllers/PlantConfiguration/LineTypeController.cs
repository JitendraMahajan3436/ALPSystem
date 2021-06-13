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
using Newtonsoft.Json;
using REIN_MES_System.Controllers.BaseManagement;

namespace REIN_MES_System.Controllers.PlantConfiguration
{
    /* Controller Name            : LineTypeController
    *  Description                : Line type controller is used to define the line type which work under shop
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class LineTypeController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        RS_Line_Types mmLineTypeObj = new RS_Line_Types();

        int lineTypeId = 0, plantId = 0;
        String lineType = "";

        /* Action Name                : Index
        *  Description                : Action is used to show the list of line types of plant
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /LineType/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle ="Line Type Configuration";
            globalData.subTitle = ResourceGlobal.Index;
            globalData.controllerName = "Line_Type";
            globalData.actionName = ResourceGlobal.Index;
            //globalData.contentTitle = ResourceLineType.Line_Type_Title_Lists;
            //globalData.contentFooter = ResourceLineType.Line_Type_Title_Lists;
            ViewBag.GlobalDataModel = globalData;

            var RS_Line_Types = db.RS_Line_Types.Include(m => m.RS_Plants).Include(m => m.RS_Employee).Include(m => m.RS_Employee1);
            return View(RS_Line_Types.ToList());
        }

        /* Action Name                : Details
        *  Description                : Action is used to show the detail of line type
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (linetype id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */        
        // GET: /LineType/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Line_Types RS_Line_Types = db.RS_Line_Types.Find(id);
            if (RS_Line_Types == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = "Line Type Configuration";
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Line_Type";
            globalData.actionName = ResourceGlobal.Details;
            //globalData.contentTitle = ResourceLineType.Line_Type_Title_Line_Type_Detail;
            //globalData.contentFooter = ResourceLineType.Line_Type_Title_Line_Type_Detail;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Line_Types);
        }

        /* Action Name                : Create
        *  Description                : Action show the create line type form
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */        
        // GET: /LineType/Create
        public ActionResult Create()
        {
            globalData.pageTitle = "Line Type Configuration";
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Line_Type";
            globalData.actionName = ResourceGlobal.Create;
           // globalData.contentTitle = ResourceLineType.Line_Type_Title_Add_Line_Type;
            //globalData.contentFooter = ResourceLineType.Line_Type_Title_Add_Line_Type;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_Id = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            return View();
        }

        /* Action Name                : Create
        *  Description                : Action show the save the line type information
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : MM_Line_Type
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */        
        // POST: /LineType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Line_Type_ID,Type_Name,Type_Description,Plant_Id,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Line_Types RS_Line_Types)
        {
            if (ModelState.IsValid)
            {
                lineType = RS_Line_Types.Type_Name;
                plantId = Convert.ToInt16(RS_Line_Types.Plant_ID);

                if(RS_Line_Types.isLineTypeExists(lineType,plantId,0))
                {
                    ModelState.AddModelError("Type_Name", "Line Type already Exist");
                }
                else
                {
                    RS_Line_Types.Type_Name = RS_Line_Types.Type_Name.Trim();
                    RS_Line_Types.Inserted_Date = DateTime.Now;
                    RS_Line_Types.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Line_Types.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Line_Types.Add(RS_Line_Types);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "Line Type";
                    globalData.messageDetail = "Line Type is Added Successfully";
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = "Line Type Configuration";
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Line_Type";
            globalData.actionName = ResourceGlobal.Create;
            //globalData.contentTitle = ResourceLineType.Line_Type_Title_Add_Line_Type;
            //globalData.contentFooter = ResourceLineType.Line_Type_Title_Add_Line_Type;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_Id = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Line_Types.Plant_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Line_Types.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Line_Types.Updated_User_ID);
            return View(RS_Line_Types);
        }

        /* Action Name                : Edit
        *  Description                : Action show the show the edit line type form for the requested line type id
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (linetype id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */        
        // GET: /LineType/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Line_Types RS_Line_Types = db.RS_Line_Types.Find(id);
            if (RS_Line_Types == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = "Line Type Configuration";
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Line_Type";
            globalData.actionName = ResourceGlobal.Edit;
            //globalData.contentTitle = ResourceLineType.Line_Type_Title_Edit_Line_Type;
           // globalData.contentFooter = ResourceLineType.Line_Type_Title_Edit_Line_Type;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_Id = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Line_Types.Plant_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Line_Types.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Line_Types.Updated_User_ID);
            return View(RS_Line_Types);
        }

        /* Action Name                : Edit
        *  Description                : Action is used to update the line type
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Line_Types
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */        
        // POST: /LineType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Line_Type_ID,Type_Name,Type_Description,Plant_Id,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Line_Types RS_Line_Types)
        {
            if (ModelState.IsValid)
            {
                lineType = RS_Line_Types.Type_Name;
                plantId = Convert.ToInt16(RS_Line_Types.Plant_ID);
                lineTypeId = Convert.ToInt16(RS_Line_Types.Line_Type_ID);

                if (RS_Line_Types.isLineTypeExists(lineType, plantId, lineTypeId))
                {
                    ModelState.AddModelError("Type_Name", ResourceMessages.Allready_Exit);
                }
                else
                {
                    mmLineTypeObj = db.RS_Line_Types.Find(lineTypeId);
                    mmLineTypeObj.Type_Name = RS_Line_Types.Type_Name;
                    mmLineTypeObj.Type_Description = RS_Line_Types.Type_Description;
                    mmLineTypeObj.Plant_ID = RS_Line_Types.Plant_ID;
                    mmLineTypeObj.Updated_Date = DateTime.Now;
                    mmLineTypeObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmLineTypeObj.Is_Edited = true;
                    db.Entry(mmLineTypeObj).State = EntityState.Modified;
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "Line Type";
                    globalData.messageDetail = ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = "Line Type Configuration";
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Line_Type";
            globalData.actionName = ResourceGlobal.Edit;
            //globalData.contentTitle = ResourceLineType.Line_Type_Title_Edit_Line_Type;
            //globalData.contentFooter = ResourceLineType.Line_Type_Title_Edit_Line_Type;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_Id = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Line_Types.Plant_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Line_Types.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Line_Types.Updated_User_ID);
            return View(RS_Line_Types);
        }

        /* Action Name                : Delete
        *  Description                : Action used to show the delete line type for user confirmation
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (linetype id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /LineType/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Line_Types RS_Line_Types = db.RS_Line_Types.Find(id);
            if (RS_Line_Types == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = "Line Type Configuration";
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Line_Type";
            globalData.actionName = ResourceGlobal.Delete;
            //globalData.contentTitle = ResourceLineType.Line_Type_Title_Delete_Line_Type;
           // globalData.contentFooter = ResourceLineType.Line_Type_Title_Delete_Line_Type;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Line_Types);
        }

        /* Action Name                : DeleteConfirmed
        *  Description                : Action used to delete line type
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (linetype id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /LineType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Line_Types RS_Line_Types = db.RS_Line_Types.Find(id);
            try
            {                
                db.RS_Line_Types.Remove(RS_Line_Types);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = "Line Type";
                globalData.messageDetail = ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Line Type";
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete",RS_Line_Types);
            }
           
        }

        /* Action Name                : Dispose
        *  Description                : Action used to dispose the line type controller object
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : disposing
        *  Return Type                : void
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

        /* Action Name                : GetLineTypeByPlantID
        *  Description                : Action used to return the list of line added for plant
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : plantId (plant id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetLineTypeByPlantID(int plantId)
        {
            try
            {
                //return Json(db.RS_Line_Types.Where(p => p.Plant_Id == plantId), JsonRequestBehavior.AllowGet);
                //return Json("7");

                String product = "hello";
                string json = JsonConvert.SerializeObject(product);
                var st = from lineType in db.RS_Line_Types
                         where lineType.Plant_ID == plantId
                         select new
                         {
                             Id = lineType.Line_Type_ID,
                             Value = lineType.Type_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
