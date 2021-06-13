using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.MaintenanceManagement
{
    public class MaintenancePartController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_Maintenance_Part mmtobj = new RS_Maintenance_Part();

        int plantId = 0, shopId = 0, lineId = 0, stationId = 0;
        int partId = 0;
       

        // GET: MaintenancePart
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.MaintenancePart;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "MaintenancePart";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var RS_Maintenance_Part = db.RS_Maintenance_Part.Where(p => p.Plant_ID == PlantID);
            return View(RS_Maintenance_Part.ToList());
        }

        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Maintenance_Part RS_Maintenance_Part = db.RS_Maintenance_Part.Find(id);
            if (RS_Maintenance_Part == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.MaintenancePart;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "MaintenancePart";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Maintenance_Part);
        }

        public ActionResult Create()
        {
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.MaintenancePart;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "MaintenancePart";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Machine_ID = new SelectList(db.RS_MT_Machines, "Machine_ID", "Machine_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p=>p.Plant_ID==PlantID), "Plant_ID", "Plant_Name",PlantID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p=>p.Plant_ID==PlantID), "Shop_ID", "Shop_Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Maintenance_Part RS_Maintenance_Part)
        {
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            try
            {

                if (ModelState.IsValid)
                {
                    if(RS_Maintenance_Part.Part_Name == null)
                    {
                        ModelState.AddModelError("Part_Name", ResourceValidation.Required);
                        globalData.pageTitle = ResourceModules.MaintenancePart;
                        globalData.subTitle = ResourceGlobal.Create;
                        globalData.controllerName = "MaintenancePart";
                        globalData.actionName = ResourceGlobal.Create;
                        ViewBag.GlobalDataModel = globalData;
                        ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID), "Plant_ID", "Plant_Name", PlantID);
                        ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_Maintenance_Part.Inserted_User_ID);
                        ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_Maintenance_Part.Updated_User_ID);
                        ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name");
                        return View(RS_Maintenance_Part);
                    }
                    else
                    {
                        RS_Maintenance_Part.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        RS_Maintenance_Part.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_Maintenance_Part.Inserted_Date = DateTime.Now;
                        //RS_Maintenance_Part.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        db.RS_Maintenance_Part.Add(RS_Maintenance_Part);
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.MaintenancePart;
                        globalData.messageDetail = ResourceModules.MaintenancePart + " " + ResourceMessages.Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                   
                }
                
                globalData.isSuccessMessage = true;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMessages.SomethingWrong;
                globalData.messageDetail = ResourceMessages.CheckAllValue;
                TempData["globalData"] = globalData;
                globalData.pageTitle = ResourceModules.MaintenancePart;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "MaintenancePart";
                globalData.actionName = ResourceGlobal.Create;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID), "Plant_ID", "Plant_Name", PlantID);
                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_Maintenance_Part.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_Maintenance_Part.Updated_User_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name");

                return View(RS_Maintenance_Part);

            }
            catch (Exception ex)
            {
                globalData.isSuccessMessage = true;
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMessages.SomethingWrong;
                globalData.messageDetail = ResourceMessages.CheckAllValue;
                TempData["globalData"] = globalData;
                globalData.pageTitle = ResourceModules.MaintenancePart;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "MaintenancePart";
                globalData.actionName = ResourceGlobal.Create;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID), "Plant_ID", "Plant_Name", PlantID);
                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_Maintenance_Part.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_Maintenance_Part.Updated_User_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name");

                return View(RS_Maintenance_Part);
            }
           
        }

        public ActionResult Edit(decimal id)
        {
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Maintenance_Part RS_Maintenance_Part = db.RS_Maintenance_Part.Find(id);
            if (RS_Maintenance_Part == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.MaintenancePart;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "MaintenancePart";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Machine_ID = new SelectList(db.RS_MT_Machines, "Machine_ID", "Machine_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(x => x.Plant_Name), "Plant_ID", "Plant_Name", RS_Maintenance_Part.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name");
            return View(RS_Maintenance_Part);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Maintenance_Part RS_Maintenance_Part)
        {
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                if(RS_Maintenance_Part.Part_Name == null)
                {
                    ModelState.AddModelError("Part_Name", ResourceValidation.Required);
                    globalData.pageTitle = ResourceModules.MaintenancePart;
                    globalData.subTitle = ResourceGlobal.Create;
                    globalData.controllerName = "MaintenancePart";
                    globalData.actionName = ResourceGlobal.Create;
                    ViewBag.GlobalDataModel = globalData;
                    ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID), "Plant_ID", "Plant_Name", PlantID);
                    ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_Maintenance_Part.Inserted_User_ID);
                    ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_Maintenance_Part.Updated_User_ID);
                    ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name");
                    return View(RS_Maintenance_Part);
                }
                else
                {
                    partId = Convert.ToInt16(RS_Maintenance_Part.Maintenance_Part_ID);

                    mmtobj = db.RS_Maintenance_Part.Find(RS_Maintenance_Part.Maintenance_Part_ID);
                    mmtobj.Plant_ID = RS_Maintenance_Part.Plant_ID;
                    mmtobj.Part_Name = RS_Maintenance_Part.Part_Name;
                    mmtobj.Part_Desscription = RS_Maintenance_Part.Part_Desscription;
                    mmtobj.Qty = RS_Maintenance_Part.Qty;
                    mmtobj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mmtobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmtobj.Updated_Date = DateTime.Now;
                    mmtobj.Is_Edited = true;
                    db.Entry(mmtobj).State = EntityState.Modified;
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.MaintenancePart;
                    globalData.messageDetail = ResourceModules.MaintenancePart + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");

                }

            }
            globalData.pageTitle = ResourceModules.MaintenancePart;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "MaintenancePart";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(x => x.Plant_Name), "Plant_ID", "Plant_Name", RS_Maintenance_Part.Plant_ID);
            return View(RS_Maintenance_Part);
        }
        
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Maintenance_Part RS_Maintenance_Part = db.RS_Maintenance_Part.Find(id);
            if (RS_Maintenance_Part == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.MaintenancePart;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "MaintenancePart";
            globalData.actionName = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Maintenance_Part);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Maintenance_Part RS_Maintenance_Part = db.RS_Maintenance_Part.Find(id);
            if (RS_Maintenance_Part.RS_Maintenance_Machine_Part.Count > 0)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.MaintenancePart;
                globalData.messageDetail = ResourceMessages.CanNotDelete;
                TempData["globalData"] = globalData;
            }
            else
            {
                db.RS_Maintenance_Part.Remove(RS_Maintenance_Part);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.MaintenancePart;
                globalData.messageDetail = ResourceModules.MaintenancePart + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
            }
            return RedirectToAction("Index");
        }

        public ActionResult ExcelUpload()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name",plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Line_ID == 0), "Station_ID", "Station_Name");
            ViewBag.Machine_ID = new SelectList(db.RS_MT_Machines.Where(p => p.Station_ID == 0), "Machine_ID", "Machine_Name");
            //mmAttributionParameterObj = new MM_Quality_Checklist().getPlantFamily(plantId);
            //ViewBag.Attribute_ID = new SelectList(mmAttributionParameterObj, "Attribute_ID", "Attribute_Desc");


            if (TempData["MaintenancePartListRecords"] != null)
            {
                ViewBag.MaintenancePartListRecords = TempData["MaintenancePartListRecords"];
                //ViewBag.MaintenancePartDataTable = TempData["MaintenancePartDataTable"];
            }

            globalData.pageTitle = ResourceModules.MaintenancePart;
            globalData.subTitle = ResourceGlobal.Upload;
            globalData.controllerName = "MaintenancePart";
            globalData.actionName = ResourceGlobal.Upload;
            globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceGlobal.Upload + " " + ResourceModules.Quality_Checklist + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceGlobal.Upload + " " + ResourceModules.Quality_Checklist + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelUpload(ExcelMaintenancePart formData)
        {
            if (ModelState.IsValid)
            {
                GlobalOperations globalOperations = new GlobalOperations();
                string fileName = Path.GetFileName(formData.Excel_File.FileName);
                string fileExtension = Path.GetExtension(formData.Excel_File.FileName);
                string fileLocation = Server.MapPath("~/App_Data/" + fileName);
                System.IO.File.Delete(fileName);
                DataTable dt = ExcelToDataTable(formData.Excel_File, fileLocation, fileExtension);
                //attributeId = formData.Attribute_ID;

                plantId = Convert.ToInt32(formData.Plant_ID);
                if (dt.Rows.Count > 0)
                {

                    MaintenancePartListRecords[] maintenancePartListRecordsObj = new MaintenancePartListRecords[dt.Rows.Count];
                    int i = 0;
                    foreach (DataRow partRow in dt.Rows)
                    {
                        String partName = partRow[0].ToString();
                        String partDescription = partRow[1].ToString();
                        maintenancePartListRecordsObj[i] = new MaintenancePartListRecords();
                       // MM_MT_Maintenance_Part mmMaintenancePart = new MM_MT_Maintenance_Part();
                        MaintenancePartListRecords mmMaintenancePartObj = new MaintenancePartListRecords();

                        mmMaintenancePartObj.partName = partName;
                        mmMaintenancePartObj.partDescription = partDescription;

                        if (partName == "" || partName == null)
                        {
                            mmMaintenancePartObj.checkListError = "Item can not be empty";

                        }
                        //else
                        //    if (mmQualityChecklist.isChecklistExists(checkListName, attributeId, plantId, 0))
                        //{
                        //    mmQualityChecklistObj.checkListError = "Item already exists";
                        //}

                        //else
                        //{
                        //    // process to save the quality checklist
                        //    mmQualityChecklist = new MM_Quality_Checklist();
                        //    mmQualityChecklist.Checklist_Name = checkListName;
                        //    mmQualityChecklist.Checklist_Description = checkListDescription;
                        //    mmQualityChecklist.Plant_ID = plantId;
                        //    mmQualityChecklist.Attribute_ID = attributeId;

                        //    mmQualityChecklist.Is_Deleted = false;
                        //    mmQualityChecklist.Inserted_Date = DateTime.Now;
                        //    mmQualityChecklist.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        //    mmQualityChecklist.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

                        //    db.MM_Quality_Checklist.Add(mmQualityChecklist);
                        //    db.SaveChanges();
                        //    mmQualityChecklistObj.checkListError = "Item is added successfully";
                        //}

                        maintenancePartListRecordsObj[i] = mmMaintenancePartObj;
                        i = i + 1;
                    }
                }
            }


            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            //mmAttributionParameterObj = new MM_Quality_Checklist().getPlantFamily(plantId);
            //ViewBag.Attribute_ID = new SelectList(mmAttributionParameterObj, "Attribute_ID", "Attribute_Desc");

            globalData.pageTitle = ResourceModules.Quality_Checklist;
            globalData.subTitle = ResourceGlobal.Upload;
            globalData.controllerName = "MaintenancePart";
            globalData.actionName = ResourceGlobal.Upload;
            globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceGlobal.Upload + " " + ResourceModules.Quality_Checklist + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceGlobal.Upload + " " + ResourceModules.Quality_Checklist + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;


            return View();
        }
        public ActionResult GetMachineByStationID(int stationId)
        {
            try
            {
                var st = from machine in db.RS_MT_Machines
                         where machine.Station_ID == stationId
                         select new
                         {
                             Id = machine.Machine_ID,
                             Value = machine.Machine_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public DataTable ExcelToDataTable(HttpPostedFileBase uploadFile, string fileLocation, string fileExtension)
        {
            DataTable dtExcelRecords = new DataTable();
            string connectionString = "";
            if (uploadFile.ContentLength > 0)
            {

                uploadFile.SaveAs(fileLocation);

                //Check whether file extension is xls or xslx

                if (fileExtension == ".xls")
                {
                    connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                else if (fileExtension == ".xlsx")
                {
                    connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }

                //Create OleDB Connection and OleDb Command && Read data from excel and generate datatable 

                OleDbConnection con = new OleDbConnection(connectionString);
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;
                OleDbDataAdapter dAdapter = new OleDbDataAdapter(cmd);

                con.Open();
                DataTable dtExcelSheetName = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string getExcelSheetName = dtExcelSheetName.Rows[0]["Table_Name"].ToString();
                cmd.CommandText = "SELECT * FROM [" + getExcelSheetName + "]";
                dAdapter.SelectCommand = cmd;
                dAdapter.Fill(dtExcelRecords);
                con.Close();

            }
            return dtExcelRecords;
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