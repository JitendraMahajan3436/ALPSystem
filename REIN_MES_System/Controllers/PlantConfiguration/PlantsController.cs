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

namespace REIN_MES_System.Controllers.PlantConfiguration
{
    /* Controller Name            : PlantsController
    *  Description                : Plant controller is to add and manage the plants
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class PlantsController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        //private REIN_SOLUTION_MEntities db_1 = new REIN_SOLUTION_MEntities();

        GlobalData globalData = new GlobalData();

        RS_Plants mmPlantObj = new RS_Plants();
       

        General generalObj = new General();

        String plantName = "";
        int plantId = 0;
        //private object ResourceModules;

        /* Action Name                : Index
        *  Description                : Action used to show the list of plant
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Plants/
        public ActionResult Index()
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = ResourceModules.Plant_Config;
               //globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "Plants";
              //  globalData.actionName = ResourceGlobal.Lists;
               // globalData.contentTitle = ResourceModules.Plant + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceModules.Plant + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;
                var userId = ((FDSession)this.Session["FDSession"]).userId;
                var plants = (from p in db.RS_Plants
                             join e in db.RS_AM_UserPlant
                             on p.Plant_ID equals e.Plant_ID
                             where e.Employee_ID == userId select p);
                //var RS_Plants = db.RS_Plants.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m=>m.RS_AM_UserPlant);
                return View(plants.ToList());
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
            
        }

        /* Action Name                : Details
        *  Description                : Action used to show the detail of selected plant
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (plant id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Plants/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Plants RS_Plants = db.RS_Plants.Find(id);
            if (RS_Plants == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Plant_Config;
           // globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Plants";
            globalData.actionName = ResourceGlobal.Create;
          //  globalData.contentTitle = ResourceModules.Plant+" "+ResourceGlobal.Details;
          //  globalData.contentFooter = ResourceModules.Plant + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Plants);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Create
        // Input Parameter      : None
        // Return Type          : ActionResult
        // Author & Time Stamp  : Jitendra Mahajan
        // Description          : Action used to show the add plant form
        //////////////////////////////////////////////////////////////////////////////////////////////////

        // GET: /Plants/Create
        public ActionResult Create()
        {
            try {
                globalData.pageTitle = ResourceModules.Plant_Config;
                //globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Plants";
                globalData.actionName = ResourceGlobal.Create;
                //  globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
                //  globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
                return View();
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
            }

        /* Action Name                : Create
        *  Description                : Action used to add the plant information in database with validation of plant name
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Plants
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Plants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( RS_Plants RS_Plants)
        {
            if (ModelState.IsValid)
            {
                plantName = RS_Plants.Plant_Name;
                if (RS_Plants.isPlantExists(plantName, 0))
                {
                    ModelState.AddModelError("Plant_Name", ResourceValidation.Exist);
                }
                else
                    if (RS_Plants.isSAPCodeExists(RS_Plants.Plant_Code_SAP, 0))
                    {
                        ModelState.AddModelError("Plant_Code_SAP", ResourceValidation.Exist);
                    }
                else
                {
                    RS_Plants.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Plants.Inserted_Date = DateTime.Now;
                    RS_Plants.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Plants.Add(RS_Plants);
                    db.SaveChanges();

                    ////Insert records into MTTUW DB
                    //mttuwPlant.Address = RS_Plants.Address;
                    //mttuwPlant.City = RS_Plants.City;
                    //mttuwPlant.Country = RS_Plants.Country;
                    //mttuwPlant.Description = RS_Plants.Description;
                    //mttuwPlant.Inserted_Date = RS_Plants.Inserted_Date;
                    //mttuwPlant.Inserted_Host = RS_Plants.Inserted_Host;
                    //mttuwPlant.Inserted_User_ID = RS_Plants.Inserted_User_ID;
                    //mttuwPlant.Plant_Code_SAP = RS_Plants.Plant_Code_SAP;
                    //mttuwPlant.Plant_Name = RS_Plants.Plant_Name;
                    //mttuwPlant.State = RS_Plants.State;
                    //mttuwPlant.Plant_ID = RS_Plants.Plant_ID;
                    //db_1.MM_MTTUW_Plants.Add(mttuwPlant);
                    //db_1.SaveChanges();
                    ////End

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Plant;
                    globalData.messageDetail = ResourceModules.Plant+" "+ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Plant_Config;
            //globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Plants";
            globalData.actionName = ResourceGlobal.Create;
           // globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
           // globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Plants.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Plants.Updated_User_ID);
            return View(RS_Plants);
        }

        /* Action Name                : Edit
        *  Description                : Action used to show the plant edit form by plant id
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (plant id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Plants/Edit/5
        public ActionResult Edit(decimal id)
        {
            try {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RS_Plants RS_Plants = db.RS_Plants.Find(id);
                if (RS_Plants == null)
                {
                    return HttpNotFound();
                }

                globalData.pageTitle = ResourceModules.Plant_Config;
                //globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "Plants";
                globalData.actionName = ResourceGlobal.Edit;
                //  globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
                // globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Plants.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Plants.Updated_User_ID);
                return View(RS_Plants);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
            }

        /* Action Name                : Edit
        *  Description                : Action used to update the plant record
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Plants
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */        
        // POST: /Plants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Plants RS_Plants)
        {
            if (ModelState.IsValid)
            {
                plantName = RS_Plants.Plant_Name;
                plantId = Convert.ToInt16(RS_Plants.Plant_ID);

                if (RS_Plants.isPlantExists(plantName, plantId))
                {
                    ModelState.AddModelError("Plant_Name", ResourceValidation.Exist);
                }
                else
                    if (RS_Plants.isSAPCodeExists(RS_Plants.Plant_Code_SAP, plantId))
                {
                    ModelState.AddModelError("Plant_Code_SAP", ResourceValidation.Exist);
                }
                else
                {
                    //RS_Plants.Updated_Date = DateTime.Now;
                    //RS_Plants.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //db.Entry(RS_Plants).State = EntityState.Modified;
                    //db.SaveChanges();

                    mmPlantObj = db.RS_Plants.Find(plantId);
                    mmPlantObj.Plant_Name = RS_Plants.Plant_Name;
                    mmPlantObj.Plant_Code_SAP = RS_Plants.Plant_Code_SAP;
                    mmPlantObj.Description = RS_Plants.Description;
                    mmPlantObj.Country = RS_Plants.Country;
                    mmPlantObj.State = RS_Plants.State;
                    mmPlantObj.City = RS_Plants.City;
                    mmPlantObj.Address = RS_Plants.Address;

                    mmPlantObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mmPlantObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmPlantObj.Updated_Date = DateTime.Now;
                    mmPlantObj.Is_Edited = true;
                    db.Entry(mmPlantObj).State = EntityState.Modified;
                    db.SaveChanges();


                    ////Update MTTUW for plant
                    //mttuwPlant = db_1.MM_MTTUW_Plants.Find(mmPlantObj.Plant_ID);
                    //mttuwPlant.Plant_Name = mmPlantObj.Plant_Name;
                    //mttuwPlant.Plant_Code_SAP = mmPlantObj.Plant_Code_SAP;
                    //mttuwPlant.Description = mmPlantObj.Description;
                    //mttuwPlant.Country = mmPlantObj.Country;
                    //mttuwPlant.State = mmPlantObj.State;
                    //mttuwPlant.City = mmPlantObj.City;
                    //mttuwPlant.Address = mmPlantObj.Address;
                    //mttuwPlant.Updated_Host = mmPlantObj.Updated_Host;
                    //mttuwPlant.Updated_User_ID = mmPlantObj.Updated_User_ID;
                    //mttuwPlant.Updated_Date = mmPlantObj.Updated_Date;
                    //mttuwPlant.Is_Edited = true;
                    //db_1.Entry(mttuwPlant).State = EntityState.Modified;
                    //db_1.SaveChanges();
                    ////end

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Plant;
                    globalData.messageDetail = ResourceModules.Plant + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Plant_Config;
           // globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Plants";
            globalData.actionName = ResourceGlobal.Edit;
           // globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
          //  globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Plants.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Plants.Updated_User_ID);
            return View(RS_Plants);
        }

        /* Action Name                : Delete
        *  Description                : Action used to show the delete plant form for user confirmantion
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (plant id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Plants/Delete/5
        public ActionResult Delete(decimal id)
        {
            try {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RS_Plants RS_Plants = db.RS_Plants.Find(id);
                if (RS_Plants == null)
                {
                    return HttpNotFound();
                }

                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = ResourceModules.Plant_Config;
                //   globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "Plants";
                globalData.actionName = ResourceGlobal.Delete;
                //  globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
                //  globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Plant + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                return View(RS_Plants);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
            }

        /* Action Name                : DeleteConfirmed
        *  Description                : Action used to delete the plant
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (plant id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */        
        // POST: /Plants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                //int b = 0;
                //float a = 5 / b;
                RS_Plants RS_Plants = db.RS_Plants.Find(id);
                db.RS_Plants.Remove(RS_Plants);
                db.SaveChanges();

                ////Delete  records from MTTUW DB
                //mttuwPlant = db_1.MM_MTTUW_Plants.Find(id);
                //db_1.MM_MTTUW_Plants.Remove(mttuwPlant);
                //db_1.SaveChanges();
                ////End

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Plants", "Plant_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Plant;
                globalData.messageDetail = ResourceModules.Plant + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                generalObj.addControllerException(ex, "Plants", "DeleteConfirmed", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true ;
                globalData.messageTitle = ResourceModules.Plant;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete",id);
            }
           
        }

        /* Action Name                : Dispose
        *  Description                : Dispose the controller object
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (plant id)
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
    }
}
