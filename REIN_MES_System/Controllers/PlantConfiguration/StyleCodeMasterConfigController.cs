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
    /* Controller Name            : StyleCodeMasterConfigController
    *  Description                : StyleCodeMasterConfig controller is used to add/edit/delete/display the style code of model
    *  Author, Timestamp          : Mukesh Chaudhari       
    */
    public class StyleCodeMasterConfigController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        RS_Model_Master mmLinesObj = new RS_Model_Master();
        int plantId = 0, lineId = 0, lineTypeId = 0, shopId = 0;


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
            globalData.pageTitle = ResourceModules.StyleCodeMasterConfig;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "StyleCodeMasterConfig";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;


            var RS_Lines = from l in db.RS_Lines join s in db.RS_Shops on l.Shop_ID equals s.Shop_ID where s.Plant_ID == plantID select l;
            var vmStyleCodeObj = (from master in db.RS_BIW_Part_Master
                                  where master.Plant_ID == plantID && master.StyleCode_ID != null
                                  select new VMStyleCodeMasterConfig()
                                  {
                                      Row_ID = master.Row_ID,
                                      Shop_ID = master.Shop_ID,
                                      Variant_Code = master.Variant_Code,
                                      Style_Code = master.RS_Style_Code.Style_Code,
                                      Shop_Name = master.RS_Shops.Shop_Name
                                  }).ToList();
            return View(vmStyleCodeObj);
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
            RS_BIW_Part_Master mmBiwPartMaster = db.RS_BIW_Part_Master.Find(id);
            if (mmBiwPartMaster == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == mmBiwPartMaster.Shop_ID).SingleOrDefault();

            plantId = Convert.ToInt16(shopObj.Plant_ID);

            VMStyleCodeMasterConfig vmstylecode = new VMStyleCodeMasterConfig();
            vmstylecode.Style_Code = mmBiwPartMaster.RS_Style_Code.Style_Code;
            vmstylecode.Variant_Code = mmBiwPartMaster.Variant_Code;
            vmstylecode.Plant_ID = mmBiwPartMaster.Plant_ID;
            vmstylecode.Shop_ID = mmBiwPartMaster.Shop_ID;
            vmstylecode.Shop_Name = mmBiwPartMaster.RS_Shops.Shop_Name;
            vmstylecode.Row_ID = mmBiwPartMaster.Row_ID;


            globalData.pageTitle = ResourceModules.StyleCodeMasterConfig;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "StyleCodeMasterConfig";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(vmstylecode);
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

            globalData.pageTitle = ResourceModules.StyleCodeMasterConfig;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StyleCodeMasterConfig";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            //ViewBag.Shop_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name");
            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Row_ID = new SelectList(db.RS_BIW_Part_Master.Where(p => p.Plant_ID == plantId), "Row_ID", "Variant_Code");
            ViewBag.Style_Code = new SelectList(db.RS_Style_Code.Where(p => p.Plant_ID == plantId), "StyleCode_ID", "Style_Code");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);

            return View();
        }

        // POST: /Line/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        /* Action Name                : Create
        *  Description                : Create the line. Validate the line is already added or not
        *  Author, Timestamp          : Mukesh Chaudhari
        *  Input parameter            : RS_Lines
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VMStyleCodeMasterConfig vMStyleCodeMasterConfig)
        {
            if (ModelState.IsValid)
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                shopId = Convert.ToInt16(vMStyleCodeMasterConfig.Shop_ID);
                shopId = Convert.ToInt16(vMStyleCodeMasterConfig.Shop_ID);
                var biwPartNoId = Convert.ToInt16(vMStyleCodeMasterConfig.Row_ID);
                //lineTypeId = Convert.ToInt16(RS_Lines.Line_Type_Id);
                //creating style code master config
                var biwobj = db.RS_BIW_Part_Master.Find(biwPartNoId);
                if (biwobj != null)
                {
                    var StyleCode_ID = Convert.ToInt16(vMStyleCodeMasterConfig.Style_Code);
                    biwobj.Updated_Date = DateTime.Now;
                    biwobj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    biwobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    biwobj.StyleCode_ID = StyleCode_ID;
                    db.Entry(biwobj).State = EntityState.Modified;
                    db.SaveChanges();


                    //updating stylecode in model master
                    var modelObj = db.RS_Model_Master.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.BIW_Part_No.ToLower() == biwobj.Variant_Code.Trim().ToLower()).ToList();
                    if (modelObj != null)
                    {
                        foreach (var model in modelObj)
                        {
                            model.Style_Code = biwobj.RS_Style_Code.Style_Code;//vMStyleCodeMasterConfig.Style_Code;
                            model.Is_Edited = true;
                            model.Updated_Date = DateTime.Now;
                            model.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                            db.Entry(model).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }


                




                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.StyleCodeMasterConfig;
                globalData.messageDetail = ResourceModules.StyleCodeMasterConfig + " " + ResourceMessages.Add_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");

            }
            globalData.pageTitle = ResourceModules.StyleCodeMasterConfig;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StyleCodeMasterConfig";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = Convert.ToInt16(vMStyleCodeMasterConfig.Plant_ID);


            //ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", modelObj.Inserted_User_ID);
            //ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", mo.Updated_User_ID);
            //ViewBag.Line_Type_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name", RS_Lines.Line_Type_Id);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants.DefaultIfEmpty(), "Plant_ID", "Plant_Name", RS_Lines.Plant_ID);
            //ViewBag.Is_Conveyor_Selected = RS_Lines.Is_Conveyor;
            //ViewBag.Selected_Plant_ID = RS_Lines.Plant_ID;

            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Row_ID = new SelectList(db.RS_BIW_Part_Master.Where(p => p.Plant_ID == plantId), "Row_ID", "Variant_Code");
            ViewBag.Style_Code = new SelectList(db.RS_Style_Code.Where(p => p.Plant_ID == plantId), "Style_Code", "Style_Code");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            return View(vMStyleCodeMasterConfig);
        }

        /* Action Name                : Edit
        *  Description                : Show the edit line form
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        //GET: /Line/Edit/5
        public ActionResult Edit(decimal id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            RS_BIW_Part_Master mmBiwPartMaster = db.RS_BIW_Part_Master.Find(id);
            if (mmBiwPartMaster == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.StyleCodeMasterConfig;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "StyleCodeMasterConfig";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == mmBiwPartMaster.Shop_ID).SingleOrDefault();
       
            plantId = Convert.ToInt16(shopObj.Plant_ID);

            VMStyleCodeMasterConfig vmstylecode = new VMStyleCodeMasterConfig();
            vmstylecode.Style_Code = mmBiwPartMaster.RS_Style_Code.Style_Code;
            vmstylecode.Variant_Code = mmBiwPartMaster.Variant_Code;
            vmstylecode.Plant_ID = mmBiwPartMaster.Plant_ID;
            vmstylecode.Shop_ID = mmBiwPartMaster.Shop_ID;
            vmstylecode.Shop_Name = mmBiwPartMaster.RS_Shops.Shop_Name;
            vmstylecode.Row_ID = mmBiwPartMaster.Row_ID;

            var stylecodeid = Convert.ToInt16(mmBiwPartMaster.StyleCode_ID);

            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", mmBiwPartMaster.Shop_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Row_ID = new SelectList(db.RS_BIW_Part_Master.Where(p => p.Plant_ID == plantId), "Row_ID", "Variant_Code", id);
            ViewBag.Style_Code = new SelectList(db.RS_Style_Code.Where(p => p.Plant_ID == plantId), "StyleCode_ID", "Style_Code", stylecodeid);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            return View(vmstylecode);
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
        public ActionResult Edit(VMStyleCodeMasterConfig vMStyleCodeMasterConfig)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            if (ModelState.IsValid)
            {

             
                shopId = Convert.ToInt16(vMStyleCodeMasterConfig.Shop_ID);
                var biwPartNoId = Convert.ToInt16(vMStyleCodeMasterConfig.Row_ID);
                //lineTypeId = Convert.ToInt16(RS_Lines.Line_Type_Id);
                //creating style code master config
                var biwobj = db.RS_BIW_Part_Master.Find(biwPartNoId);
                if (biwobj != null)
                {
                    var StyleCode_ID = Convert.ToInt16(vMStyleCodeMasterConfig.Style_Code);
                    biwobj.Updated_Date = DateTime.Now;
                    biwobj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    biwobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    biwobj.StyleCode_ID = StyleCode_ID;
                    db.Entry(biwobj).State = EntityState.Modified;
                    db.SaveChanges();


                    //updating stylecode in model master
                    var modelObj = db.RS_Model_Master.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.BIW_Part_No.ToLower() == biwobj.Variant_Code.Trim().ToLower()).ToList();
                    if (modelObj != null)
                    {
                        foreach (var model in modelObj)
                        {
                            model.Style_Code = biwobj.RS_Style_Code.Style_Code; //vMStyleCodeMasterConfig.Style_Code;
                            model.Is_Edited = true;
                            model.Updated_Date = DateTime.Now;
                            model.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                            db.Entry(model).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.StyleCodeMasterConfig;
                globalData.messageDetail = ResourceModules.StyleCodeMasterConfig + " " + ResourceMessages.Add_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");

            }
            globalData.pageTitle = ResourceModules.StyleCodeMasterConfig;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "StyleCodeMasterConfig";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = Convert.ToInt16(vMStyleCodeMasterConfig.Plant_ID);

            ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Row_ID = new SelectList(db.RS_BIW_Part_Master.Where(p => p.Plant_ID == plantId), "Row_ID", "Variant_Code");
            ViewBag.Style_Code = new SelectList(db.RS_Style_Code.Where(p => p.Plant_ID == plantId), "Style_Code", "Style_Code");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);


            //ViewBag.Shop_Id = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            //ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            //ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            //ViewBag.Model_ID = new SelectList(db.RS_Model_Master.Where(p => p.Plant_ID == plantId), "Model_ID", "Model_Code");
            //ViewBag.Style_Code = new SelectList(db.RS_Style_Code.Where(p => p.Plant_ID == plantId), "Style_Code", "Style_Code");
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            return View(vMStyleCodeMasterConfig);
        }

        ///* Action Name                : Delete
        //*  Description                : Action is used to show the delete line page for user confirmation
        //*  Author, Timestamp          : Jitendra Mahajan
        //*  Input parameter            : id (line id)
        //*  Return Type                : ActionResult
        //*  Revision                   : 1.0
        //*/
        //// GET: /Line/Delete/5
        public ActionResult Delete(decimal id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_BIW_Part_Master mmBiwPartMaster = db.RS_BIW_Part_Master.Find(id);
            if (mmBiwPartMaster == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == mmBiwPartMaster.Shop_ID).SingleOrDefault();

            plantId = Convert.ToInt16(shopObj.Plant_ID);

            VMStyleCodeMasterConfig vmstylecode = new VMStyleCodeMasterConfig();
            vmstylecode.Style_Code = mmBiwPartMaster.RS_Style_Code.Style_Code;
            vmstylecode.Variant_Code = mmBiwPartMaster.Variant_Code;
            vmstylecode.Plant_ID = mmBiwPartMaster.Plant_ID;
            vmstylecode.Shop_ID = mmBiwPartMaster.Shop_ID;
            vmstylecode.Shop_Name = mmBiwPartMaster.RS_Shops.Shop_Name;
            vmstylecode.Row_ID = mmBiwPartMaster.Row_ID;


            globalData.pageTitle = ResourceModules.StyleCodeMasterConfig;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "StyleCodeMasterConfig";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.StyleCodeMasterConfig + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(vmstylecode);
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
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var biwPartNoId = Convert.ToInt16(id);
            VMStyleCodeMasterConfig vmstylecode = new VMStyleCodeMasterConfig();
            try
            {
                var biwobj = db.RS_BIW_Part_Master.Find(biwPartNoId);
                shopId = Convert.ToInt16(biwobj.Shop_ID);
           
           
                if (biwobj.StyleCode_ID != null)
                {
                    
                    biwobj.Updated_Date = DateTime.Now;
                    biwobj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    biwobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    biwobj.StyleCode_ID = null;
                    db.Entry(biwobj).State = EntityState.Modified;
                    db.SaveChanges();


                    //updating stylecode in model master
                    var modelObj = db.RS_Model_Master.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.BIW_Part_No.ToLower() == biwobj.Variant_Code.Trim().ToLower()).ToList();
                    if (modelObj != null)
                    {
                        foreach (var model in modelObj)
                        {
                            model.Style_Code = null;
                            model.Is_Edited = true;
                            model.Updated_Date = DateTime.Now;
                            model.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                            db.Entry(model).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }



                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.StyleCodeMasterConfig;
                globalData.messageDetail = ResourceModules.StyleCodeMasterConfig + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.StyleCodeMasterConfig;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete", vmstylecode);
            }
        }

        ///* Action Name                : Dispose
        //*  Description                : Disponse the Line controller object
        //*  Author, Timestamp          : Jitendra Mahajan
        //*  Input parameter            : disposing
        //*  Return Type                : ActionResult
        //*  Revision                   : 1.0
        //*/
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



       
        ///* Action Name                : GetLineByShopID
        //*  Description                : Action is used to return the list of lines in json format
        //*  Author, Timestamp          : Jitendra Mahajan
        //*  Input parameter            : shopId (shop id)
        //*  Return Type                : ActionResult
        //*  Revision                   : 1.0
        //*/
        public ActionResult GetBiwPartNoByShopID(int shopId)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            try
            {
                var st = from model in db.RS_BIW_Part_Master
                         where model.Shop_ID == shopId && model.Sub_Assembly_ID == 1 &&
                         model.Plant_ID == plantID && model.StyleCode_ID == null
                         select new
                         {
                             Row_ID = model.Row_ID,
                             Variant_Code = model.Variant_Code,
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
