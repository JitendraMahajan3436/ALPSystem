using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System.Data.OleDb;
using REIN_MES_System.App_LocalResources;
using System.IO;
using REIN_MES_System.Controllers.BaseManagement;

namespace REIN_MES_System.Controllers.BOM
{
    public class BomController : BaseController
    {
        //private REIN_SOLUTION_MEntities db_1 = new REIN_SOLUTION_MEntities();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        string email = "";
        string empno = "";
        decimal empId = 0;
        decimal plantId = 0;
        decimal shopId = 0;
        decimal lineId = 0;
        int category = 0;

       
        General generalObj = new General();
        // GET: Bom
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.BOM;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = ResourceModules.User;
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceModules.BOM + " " + ResourceGlobal.Lists;
            //globalData.contentFooter = ResourceModules.BOM + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View(db.RS_BOM_Item.ToList());
        }
        public ActionResult BomUploadData()
        {
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                ViewBag.Setup_ID = new SelectList(db.RS_Setup.Where(p => p.Line_ID == lineId), "Setup_ID", "Setup_Name");
                List<SelectListItem> listModel = new List<SelectListItem>();
                ViewBag.Line_ID = new SelectList(listModel);

                if (TempData["OrderUploadRecords"] != null)
                {
                    ViewBag.OrderUploadRecords = TempData["OrderUploadRecords"];
                }

                globalData.pageTitle = "Bom Uplaod";
                globalData.subTitle = ResourceGlobal.Upload;
                globalData.controllerName = "Bom";
                globalData.actionName = ResourceGlobal.Upload;
                globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Add_User + " " + ResourceGlobal.Form;
                //globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Add_User + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Bom");
            }
        }
        public FileResult DownloadBomExcel(string fileName)
        {
            var FileVirtualPath = "~/App_Data/" + fileName;

            string file = @"~\App_Data\BOM_Formate\BOM_Formate.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult BomUploadData(RS_BOM_Item formData)
        {
            String createdOrders = "";
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if(formData.Excel_File==null)
            {
                ModelState.AddModelError("Excel_File", "This field is required.");
                RS_BOM_Item[] orderUploadRecordsObj1 = new RS_BOM_Item[1];
                TempData["OrderUploadRecords"] = orderUploadRecordsObj1;
                ViewBag.OrderUploadRecords = orderUploadRecordsObj1;
                ViewBag.dt = orderUploadRecordsObj1;
                globalData.isSuccessMessage = true;
                globalData.messageTitle = "Bom Upload";
                globalData.messageDetail = "Empty Excel";
                globalData.pageTitle = ResourceGlobal.Excel + " " + "Bom Upload" + " " + ResourceGlobal.Form;
                globalData.subTitle = ResourceGlobal.Upload;
                globalData.controllerName = "BOM";
                globalData.actionName = ResourceGlobal.Upload;
                ViewBag.GlobalDataModel = globalData;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    GlobalOperations globalOperations = new GlobalOperations();
                    string fileName = Path.GetFileName(formData.Excel_File.FileName);
                    string fileExtension = Path.GetExtension(formData.Excel_File.FileName);
                    string fileLocation = Server.MapPath("~/App_Data/" + fileName);
                    System.IO.File.Delete(fileName);
                    DataTable dt = ExcelToDataTable(formData.Excel_File, fileLocation, fileExtension);
                    //String attributeId = formData.Attribute_ID;



                    if (dt.Rows.Count > 0)
                    {
                        RS_BOM_Item[] orderUploadRecordsObj = new RS_BOM_Item[dt.Rows.Count];
                        //RS_Maintenance_Machine_Part mmOrderCreationObj = new RS_Maintenance_Machine_Part();

                        int i = 0;
                        string[] modellist= new string[dt.Rows.Count];
                        //int ExcelNumber = db.RS_All_Configuration.Max(m => m.Excel_No).Value;
                        foreach (DataRow checkListRow in dt.Rows)
                        {

                            String modelcode = checkListRow[0].ToString() != null ? checkListRow[0].ToString().Trim() : null;
                            String modeldesc = checkListRow[1].ToString() != null ? checkListRow[1].ToString().Trim() : null;
                            String FGeffectiveDateFrom = checkListRow[2].ToString() != null ? checkListRow[2].ToString().Trim() : null;
                            String FGeffectiveDateTo = checkListRow[3].ToString() != null ? checkListRow[3].ToString().Trim() : null;
                            String partnumber = checkListRow[4].ToString() != null ? checkListRow[4].ToString().Trim() : null;
                            String partdesc = checkListRow[5].ToString() != null ? checkListRow[5].ToString().Trim() : null;
                            String effectiveDateFrom = checkListRow[6].ToString() != null ? checkListRow[6].ToString().Trim() : null;
                            String effectiveDateTo = checkListRow[7].ToString() != null ? checkListRow[7].ToString().Trim() : null;

                            //modellist[i] = modelcode;

                            orderUploadRecordsObj[i] = new RS_BOM_Item();
                            RS_BOM_Item oJTUpload1 = new RS_BOM_Item();
                            oJTUpload1.Model_Code = modelcode;
                            oJTUpload1.Model_Desc = modeldesc;
                            oJTUpload1.Part_No = partnumber;
                            oJTUpload1.Part_Description = partdesc;

                            if (FGeffectiveDateFrom != "" && FGeffectiveDateFrom != null)
                            {
                                oJTUpload1.FGEffective_Date_From = Convert.ToDateTime(FGeffectiveDateFrom);
                            }
                            if (FGeffectiveDateTo != "" && FGeffectiveDateTo != null)
                            {
                                oJTUpload1.FGEffective_Date_To = Convert.ToDateTime(FGeffectiveDateTo);
                            }


                            if (modelcode != "" && modeldesc != "" && partdesc != "" && partnumber != "" && effectiveDateFrom != "" && effectiveDateTo != "")
                            {
                                oJTUpload1.Effective_Date_From = Convert.ToDateTime(effectiveDateFrom);
                                oJTUpload1.Effective_Date_To = Convert.ToDateTime(effectiveDateTo);
                                bool ifExist= modellist.Any(c=>c==modelcode);

                                modellist[i] = modelcode;
                                // add in bom
                                RS_BOM obj = new RS_BOM();
                                RS_BOM hist = new RS_BOM();
                                int bomversion = 0;
                                if (db.RS_BOM.Any(c => c.Model_Code == modelcode))
                                {
                                    //save in history
                                    hist = db.RS_BOM.Where(c => c.Model_Code == modelcode).FirstOrDefault();
                                    if (!ifExist)
                                    {
                                        RS_BOM_History histSave = new RS_BOM_History();
                                        histSave.Model_Code = hist.Model_Code;
                                        histSave.Model_Description = hist.Model_Description;
                                        histSave.Bom_Version = hist.Bom_Version;
                                        histSave.Bom_Revision = hist.Bom_Revision;
                                        histSave.Plant_ID = hist.Plant_ID;
                                        histSave.Is_Transfered = hist.Is_Transfered;
                                        histSave.Is_Purgeable = hist.Is_Purgeable;
                                        histSave.Is_Edited = hist.Is_Edited;
                                        histSave.Inserted_User_ID = hist.Inserted_User_ID;
                                        histSave.Inserted_Date = hist.Inserted_Date;
                                        histSave.Updated_User_ID = hist.Updated_User_ID;
                                        histSave.Updated_Date = hist.Updated_Date;
                                        histSave.Is_Active = hist.Is_Active;
                                        histSave.Inserted_Host = hist.Inserted_Host;
                                        histSave.Effective_Date_From = hist.Effective_Date_From;
                                        histSave.Effective_Date_To = hist.Effective_Date_To;
                                        bomversion = Convert.ToInt32(hist.Bom_Version);
                                        db.RS_BOM_History.Add(histSave);
                                        db.RS_BOM.Remove(hist);
                                        db.SaveChanges();

                                        if (FGeffectiveDateFrom != "" && FGeffectiveDateFrom != null)
                                        {
                                            obj.Effective_Date_From = Convert.ToDateTime(FGeffectiveDateFrom);
                                        }
                                        else
                                        {
                                            obj.Effective_Date_From = Convert.ToDateTime("2020-01-01");
                                        }
                                        if (FGeffectiveDateTo != "" && FGeffectiveDateTo != null)
                                        {
                                            obj.Effective_Date_To = Convert.ToDateTime(FGeffectiveDateTo);
                                        }
                                        else
                                        {
                                            obj.Effective_Date_To = Convert.ToDateTime("2999-01-01");
                                        }
                                        obj.Bom_Version = hist.Bom_Version + 1;
                                        obj.Model_Code = modelcode;
                                        obj.Model_Description = modeldesc;
                                        obj.Inserted_Date = DateTime.Now;
                                        obj.Plant_ID = plantId;
                                        obj.Inserted_Host = HttpContext.Request.UserHostAddress;
                                        obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        db.RS_BOM.Add(obj);
                                        db.SaveChanges();

                                    }

                                    else
                                    {
                                        bomversion = Convert.ToInt32(hist.Bom_Version);
                                    }
                                }
                                //model not exist in bom table
                                else
                                {
                                    bomversion = 1;

                                    if (FGeffectiveDateFrom != "" && FGeffectiveDateFrom != null)
                                    {
                                        obj.Effective_Date_From = Convert.ToDateTime(FGeffectiveDateFrom);
                                    }
                                    else
                                    {
                                        obj.Effective_Date_From = Convert.ToDateTime("2020-01-01");
                                    }
                                    if (FGeffectiveDateTo != "" && FGeffectiveDateTo != null)
                                    {
                                        obj.Effective_Date_To = Convert.ToDateTime(FGeffectiveDateTo);
                                    }
                                    else
                                    {
                                        obj.Effective_Date_To = Convert.ToDateTime("2999-01-01");
                                    }
                                    obj.Bom_Version = bomversion;
                                    obj.Model_Code = modelcode;
                                    obj.Model_Description = modeldesc;
                                    obj.Inserted_Date = DateTime.Now;
                                    obj.Plant_ID = plantId;
                                    obj.Inserted_Host = HttpContext.Request.UserHostAddress;
                                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    db.RS_BOM.Add(obj);
                                    db.SaveChanges();

                                }



                                //Bom Item capturing
                                RS_BOM_Item obj12 = new RS_BOM_Item();
                                RS_BOM_Item hist1 = new RS_BOM_Item();
                                var bomobj = db.RS_BOM.Where(c => c.Model_Code == modelcode).FirstOrDefault();
                                if (db.RS_BOM_Item.Any(c => c.Model_Code == modelcode && c.Part_No == partnumber))
                                {
                                    //save in history
                                    hist1 = db.RS_BOM_Item.Where(c => c.Model_Code == modelcode && c.Part_No == partnumber).FirstOrDefault();
                                    RS_BOM_Item_History histSave1 = new RS_BOM_Item_History();
                                    histSave1.Bom_ID = hist1.Bom_ID;
                                    histSave1.Model_Code = hist1.Model_Code;
                                    histSave1.Part_No = hist1.Part_No;
                                    histSave1.Part_Description = hist1.Part_Description;
                                    histSave1.Bom_Version = hist1.Bom_Version;
                                    histSave1.SAP_Station_ID = hist1.SAP_Station_ID;
                                    histSave1.Qty = hist1.Qty;
                                    histSave1.UOM = hist1.UOM;
                                    histSave1.Is_Transfered = hist1.Is_Transfered;
                                    histSave1.Is_Purgeable = hist1.Is_Purgeable;
                                    histSave1.Is_Edited = hist1.Is_Edited;
                                    histSave1.Inserted_User_ID = hist1.Inserted_User_ID;
                                    histSave1.Inserted_Date = hist1.Inserted_Date;
                                    histSave1.Updated_User_ID = hist1.Updated_User_ID;
                                    histSave1.Updated_Date = hist1.Updated_Date;
                                    histSave1.Updated_Host = hist1.Updated_Host;
                                    histSave1.Is_Active = hist1.Is_Active;
                                    histSave1.Effective_Date_From = hist1.Effective_Date_From;
                                    histSave1.Effective_Date_To = hist1.Effective_Date_To;
                                    histSave1.Inserted_Host = hist1.Inserted_Host;
                                    db.RS_BOM_Item_History.Add(histSave1);
                                    db.RS_BOM_Item.Remove(hist1);
                                    db.SaveChanges();
                                }

                                //add in bom item 
                                RS_BOM_Item obj1 = new RS_BOM_Item();
                                if (effectiveDateFrom != "" && effectiveDateFrom != null)
                                {
                                    obj1.Effective_Date_From = Convert.ToDateTime(effectiveDateFrom);
                                }
                                else
                                {
                                    obj1.Effective_Date_From = Convert.ToDateTime("2020-01-01");
                                }
                                if (effectiveDateTo != "" && effectiveDateTo != null)
                                {
                                    obj1.Effective_Date_To = Convert.ToDateTime(effectiveDateTo);
                                }
                                else
                                {
                                    obj1.Effective_Date_To = Convert.ToDateTime("2999-01-01");
                                }
                                obj1.Model_Code = modelcode;
                                obj1.Part_No = partnumber;
                                obj1.Part_Description = partdesc;
                                obj1.Effective_Date_From = Convert.ToDateTime(effectiveDateFrom);
                                obj1.Effective_Date_To = Convert.ToDateTime(effectiveDateTo);
                                obj1.Bom_ID = bomobj.Bom_ID;
                                obj1.Bom_Version = bomobj.Bom_Version;
                                obj1.Qty = "1";
                                obj1.UOM = "1";
                                obj1.Inserted_Date = DateTime.Now;
                                obj1.Inserted_Host = HttpContext.Request.UserHostAddress;
                                obj1.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                db.RS_BOM_Item.Add(obj1);
                                db.SaveChanges();

                                oJTUpload1.SS_Error_Sucess = "Record add successfully";
                                oJTUpload1.Is_Sucess = false;
                            }
                            else
                            {
                                oJTUpload1.SS_Error_Sucess = "Error";
                                oJTUpload1.Is_Sucess = false;
                            }

                            orderUploadRecordsObj[i] = oJTUpload1;
                            i = i + 1;

                        }
                        TempData["OrderUploadRecords"] = orderUploadRecordsObj;
                        ViewBag.OrderUploadRecords = orderUploadRecordsObj;
                        ViewBag.dt = orderUploadRecordsObj;
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = "Bom Upload";
                        globalData.messageDetail = "Bom Upload successfully";
                        globalData.pageTitle = ResourceGlobal.Excel + " " + "Bom Upload" + " " + ResourceGlobal.Form;
                        globalData.subTitle = ResourceGlobal.Upload;
                        globalData.controllerName = "BOM";
                        globalData.actionName = ResourceGlobal.Upload;
                        ViewBag.GlobalDataModel = globalData;
                        ViewBag.createdOrders = createdOrders;
                    }
                    else
                    {
                        RS_BOM_Item[] orderUploadRecordsObj1 = new RS_BOM_Item[1];
                        TempData["OrderUploadRecords"] = orderUploadRecordsObj1;
                        ViewBag.OrderUploadRecords = orderUploadRecordsObj1;
                        ViewBag.dt = orderUploadRecordsObj1;
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = "Bom Upload";
                        globalData.messageDetail = "Empty Excel";
                        globalData.pageTitle = ResourceGlobal.Excel + " " + "Bom Upload" + " " + ResourceGlobal.Form;
                        globalData.subTitle = ResourceGlobal.Upload;
                        globalData.controllerName = "BOM";
                        globalData.actionName = ResourceGlobal.Upload;
                        ViewBag.GlobalDataModel = globalData;
                    }
                }
            }
            

            return View();
        }

        public DataTable ExcelToDataTable(HttpPostedFileBase uploadFile, string fileLocation, string fileExtension)
        {
            DataTable dtExcelRecords = new DataTable();
            string connectionString = "";
            if (uploadFile.ContentLength > 0)
            {
                uploadFile.SaveAs(fileLocation);



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

        // GET: Bom/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_BOM_Item RS_BOM_Item = db.RS_BOM_Item.Find(id);
            if (RS_BOM_Item == null)
            {
                return HttpNotFound();
            }
            return View(RS_BOM_Item);
        }

        // GET: Bom/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.BOM;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Bom";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Create + " " + ResourceModules.BOM + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Create + " " + ResourceModules.BOM + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        // POST: Bom/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_BOM_Item RS_BOM_Item)
        {
            try
            {
                bool isvalid = true;
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int userid = ((FDSession)this.Session["FDSession"]).userId;
                string userhost = ((FDSession)this.Session["FDSession"]).userHost;


                if (RS_BOM_Item.Model_Code == null)
                {
                    ModelState.AddModelError("Model_Code", "This field is required");
                    isvalid = false;
                }
                if (RS_BOM_Item.Model_Desc == null)
                {
                    ModelState.AddModelError("Model_Desc", "This field is required");
                    isvalid = false;
                }
                if (RS_BOM_Item.Part_No == null)
                {
                    ModelState.AddModelError("Part_No", "This field is required");
                    isvalid = false;
                }
                if (RS_BOM_Item.Part_Description == null)
                {
                    ModelState.AddModelError("Part_Description", "This field is required");
                    isvalid = false;
                }
                if (RS_BOM_Item.Effective_Date_From == null)
                {
                    ModelState.AddModelError("Effective_Date_From", "This field is required");
                    isvalid = false;
                }
                if (RS_BOM_Item.Effective_Date_To == null)
                {
                    ModelState.AddModelError("Effective_Date_To", "This field is required");
                    isvalid = false;
                }

                //check part code exist
                if (db.RS_BOM_Item.Any(c => c.Model_Code == RS_BOM_Item.Model_Code && c.Part_No == RS_BOM_Item.Part_No))
                {
                    ModelState.AddModelError("Part_No", "Part number already exist against " + RS_BOM_Item.Model_Code);
                    isvalid = false;
                }


                if (RS_BOM_Item.FGEffective_Date_From == Convert.ToDateTime("01-01-0001") || RS_BOM_Item.FGEffective_Date_From == null)
                {
                    RS_BOM_Item.FGEffective_Date_From = Convert.ToDateTime("2020-01-01");
                    ModelState["FGEffective_Date_From"].Errors.Clear();
                }
                if (RS_BOM_Item.FGEffective_Date_To == Convert.ToDateTime("01-01-0001") || RS_BOM_Item.FGEffective_Date_To == null)
                {
                    RS_BOM_Item.FGEffective_Date_To = Convert.ToDateTime("2999-01-01");
                    ModelState["FGEffective_Date_To"].Errors.Clear();
                }


                if (ModelState.IsValid)
                {
                    if (isvalid)
                    {
                        //check bom exist
                        RS_BOM bom = new RS_BOM();
                        bom = db.RS_BOM.Where(c => c.Model_Code == RS_BOM_Item.Model_Code).FirstOrDefault();
                        int bomversion = 1;
                        int bomID = 0;

                        if (bom == null)
                        {
                            //add bom
                            RS_BOM obj = new RS_BOM();
                            if (RS_BOM_Item.FGEffective_Date_From != Convert.ToDateTime("01-01-0001"))
                            {
                                obj.Effective_Date_From = RS_BOM_Item.FGEffective_Date_From;
                            }
                            else
                            {
                                obj.Effective_Date_From = Convert.ToDateTime("2020-01-01");
                            }
                            if (RS_BOM_Item.FGEffective_Date_To != Convert.ToDateTime("01-01-0001"))
                            {
                                obj.Effective_Date_To = RS_BOM_Item.FGEffective_Date_To;
                            }
                            else
                            {
                                obj.Effective_Date_To = Convert.ToDateTime("2999-01-01");
                            }

                            obj.Bom_Version = bomversion;
                            obj.Model_Code = RS_BOM_Item.Model_Code;
                            obj.Model_Description = RS_BOM_Item.Model_Desc;
                            obj.Inserted_Date = DateTime.Now;
                            obj.Plant_ID = plantId;
                            obj.Inserted_Host = userhost;
                            obj.Inserted_User_ID = userid;
                            db.RS_BOM.Add(obj);
                            db.SaveChanges();
                            bomID = obj.Bom_ID;

                        }
                        else
                        {
                            bomversion = Convert.ToInt32(bom.Bom_Version);
                            bomID = bom.Bom_ID;
                        }


                        //add in bom item 

                        RS_BOM_Item.Bom_ID = bomID;
                        RS_BOM_Item.Bom_Version = bomversion;
                        RS_BOM_Item.Qty = "1";
                        RS_BOM_Item.UOM = "1";
                        RS_BOM_Item.Inserted_Date = DateTime.Now;
                        RS_BOM_Item.Inserted_Host = userhost;
                        RS_BOM_Item.Inserted_User_ID = userid;
                        db.RS_BOM_Item.Add(RS_BOM_Item);
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = "Bom Created";
                        globalData.messageDetail = "Bom Created successfully";
                        globalData.pageTitle = ResourceGlobal.Create + " " + "Bom Created" + " " + ResourceGlobal.Form;
                        globalData.subTitle = "BOM Create";
                        globalData.controllerName = "BOM";
                        globalData.actionName = ResourceGlobal.Create;
                        ViewBag.GlobalDataModel = globalData;
                        return RedirectToAction("Index");
                    }

                }
                globalData.pageTitle = ResourceModules.BOM;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Bom";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Create + " " + ResourceModules.BOM + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Create + " " + ResourceModules.BOM + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;
                return View(RS_BOM_Item);
            }
            catch (Exception ex)
            {
                globalData.isSuccessMessage = false;
                globalData.messageTitle = ResourceModules.BOM;
                globalData.messageDetail = "BOM " + " " + ResourceMessages.Is_Error;
                TempData["globalData"] = globalData;
                return View(RS_BOM_Item);
            }
     
        }

        // GET: Bom/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_BOM_Item RS_BOM_Item = db.RS_BOM_Item.Find(id);
           

            if (RS_BOM_Item == null)
            {
                return HttpNotFound();
            }
            RS_BOM RS_BOM = db.RS_BOM.Where(c => c.Model_Code == RS_BOM_Item.Model_Code).FirstOrDefault();
            if (RS_BOM_Item.Effective_Date_From != null)
            {
                ViewBag.Effective_Date_From = RS_BOM_Item.Effective_Date_From.Value.ToShortDateString();
            }
            if (RS_BOM_Item.Effective_Date_To != null)
            {
                ViewBag.Effective_Date_To = RS_BOM_Item.Effective_Date_To.Value.ToShortDateString();
            }
            if (RS_BOM.Effective_Date_From != null)
            {
                ViewBag.FGEffective_Date_From = RS_BOM.Effective_Date_From.Value.ToShortDateString();
            }
            if (RS_BOM.Effective_Date_To != null)
            {
                ViewBag.FGEffective_Date_To = RS_BOM.Effective_Date_To.Value.ToShortDateString();
            }
            RS_BOM_Item.Model_Desc = RS_BOM.Model_Description;
            globalData.pageTitle = ResourceModules.BOM;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Bom";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.BOM + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.BOM + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_BOM_Item);
        }

        // POST: Bom/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_BOM_Item RS_BOM_Item)
        {
            bool isvalid = true;
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int userid = ((FDSession)this.Session["FDSession"]).userId;
            string userhost = ((FDSession)this.Session["FDSession"]).userHost;


            if (RS_BOM_Item.Model_Code == null)
            {
                ModelState.AddModelError("Model_Code", "This field is required");
                isvalid = false;
            }
            if (RS_BOM_Item.Model_Desc == null)
            {
                ModelState.AddModelError("Model_Desc", "This field is required");
                isvalid = false;
            }
            if (RS_BOM_Item.Part_No == null)
            {
                ModelState.AddModelError("Part_No", "This field is required");
                isvalid = false;
            }
            if (RS_BOM_Item.Part_Description == null)
            {
                ModelState.AddModelError("Part_Description", "This field is required");
                isvalid = false;
            }
            if (RS_BOM_Item.Effective_Date_From == null)
            {
                ModelState.AddModelError("Effective_Date_From", "This field is required");
                isvalid = false;
            }
            if (RS_BOM_Item.Effective_Date_To == null)
            {
                ModelState.AddModelError("Effective_Date_To", "This field is required");
                isvalid = false;
            }
          
            //check part code exist
            if (db.RS_BOM_Item.Any(c => c.Model_Code == RS_BOM_Item.Model_Code && c.Part_No == RS_BOM_Item.Part_No && c.Row_ID!= RS_BOM_Item.Row_ID))
            {
                ModelState.AddModelError("Part_No", "Part number already exist against " + RS_BOM_Item.Model_Code);
                isvalid = false;
            }
            if (ModelState.IsValid)
            {
                if(isvalid)
                {
                    int bomversion = 0;
                    //remove from bom and capture history
                    RS_BOM hist = new RS_BOM();
                    hist = db.RS_BOM.Where(c => c.Model_Code == RS_BOM_Item.Model_Code).FirstOrDefault();
                    RS_BOM_History histSave = new RS_BOM_History();
                    histSave.Model_Code = hist.Model_Code;
                    histSave.Model_Description = hist.Model_Description;
                    histSave.Bom_Version = hist.Bom_Version;
                    histSave.Bom_Revision = hist.Bom_Revision;
                    histSave.Plant_ID = hist.Plant_ID;
                    histSave.Is_Transfered = hist.Is_Transfered;
                    histSave.Is_Purgeable = hist.Is_Purgeable;
                    histSave.Is_Edited = hist.Is_Edited;
                    histSave.Inserted_User_ID = hist.Inserted_User_ID;
                    histSave.Inserted_Date = hist.Inserted_Date;
                    histSave.Updated_User_ID = hist.Updated_User_ID;
                    histSave.Updated_Date = hist.Updated_Date;
                    histSave.Is_Active = hist.Is_Active;
                    histSave.Inserted_Host = hist.Inserted_Host;
                    histSave.Effective_Date_From = hist.Effective_Date_From;
                    histSave.Effective_Date_To = hist.Effective_Date_To;
                    bomversion = Convert.ToInt32(hist.Bom_Version);
                    db.RS_BOM_History.Add(histSave);
                    db.RS_BOM.Remove(hist);
                    //add bom
                    RS_BOM obj = new RS_BOM();
                    if (RS_BOM_Item.FGEffective_Date_From != Convert.ToDateTime("01-01-0001"))
                    {
                        obj.Effective_Date_From = RS_BOM_Item.FGEffective_Date_From;
                    }
                    else
                    {
                        obj.Effective_Date_From = Convert.ToDateTime("2020-01-01");
                    }
                    if (RS_BOM_Item.FGEffective_Date_To != Convert.ToDateTime("01-01-0001"))
                    {
                        obj.Effective_Date_To = RS_BOM_Item.FGEffective_Date_To;
                    }
                    else
                    {
                        obj.Effective_Date_To = Convert.ToDateTime("2999-01-01");
                    }
                 
                    obj.Bom_Version = bomversion+1;
                    obj.Model_Code = RS_BOM_Item.Model_Code;
                    obj.Model_Description = RS_BOM_Item.Model_Desc;
                    obj.Inserted_Date = DateTime.Now;
                    obj.Plant_ID = plantId;
                    obj.Inserted_Host = userhost;
                    obj.Inserted_User_ID = userid;
                    db.RS_BOM.Add(obj);
                    db.SaveChanges();

                    //remove from bom item and capture history
                    RS_BOM_Item hist1 = new RS_BOM_Item();
                    hist1 = db.RS_BOM_Item.Find(RS_BOM_Item.Row_ID); 
                    RS_BOM_Item_History histSave1 = new RS_BOM_Item_History();
                    histSave1.Bom_ID = hist1.Bom_ID;
                    histSave1.Model_Code = hist1.Model_Code;
                    histSave1.Part_No = hist1.Part_No;
                    histSave1.Part_Description = hist1.Part_Description;
                    histSave1.Bom_Version = hist1.Bom_Version;
                    histSave1.SAP_Station_ID = hist1.SAP_Station_ID;
                    histSave1.Qty = hist1.Qty;
                    histSave1.UOM = hist1.UOM;
                    histSave1.Is_Transfered = hist1.Is_Transfered;
                    histSave1.Is_Purgeable = hist1.Is_Purgeable;
                    histSave1.Is_Edited = hist1.Is_Edited;
                    histSave1.Inserted_User_ID = hist1.Inserted_User_ID;
                    histSave1.Inserted_Date = hist1.Inserted_Date;
                    histSave1.Updated_User_ID = hist1.Updated_User_ID;
                    histSave1.Updated_Date = hist1.Updated_Date;
                    histSave1.Updated_Host = hist1.Updated_Host;
                    histSave1.Is_Active = hist1.Is_Active;
                    histSave1.Effective_Date_From = hist1.Effective_Date_From;
                    histSave1.Effective_Date_To = hist1.Effective_Date_To;
                    histSave1.Inserted_Host = hist1.Inserted_Host;
                    db.RS_BOM_Item_History.Add(histSave1);
                    db.RS_BOM_Item.Remove(hist1);
                    db.SaveChanges();

                    //add in bom item
                    //add in bom item 
                    RS_BOM_Item obj1 = new RS_BOM_Item();
                    obj1.Effective_Date_From = RS_BOM_Item.Effective_Date_From;
                    obj1.Effective_Date_To = RS_BOM_Item.Effective_Date_To;
                    obj1.Model_Code = RS_BOM_Item.Model_Code;
                    obj1.Part_No = RS_BOM_Item.Part_No;
                    obj1.Part_Description = RS_BOM_Item.Part_Description;
                    obj1.Bom_ID = RS_BOM_Item.Bom_ID;
                    obj1.Bom_Version = bomversion+1;
                    obj1.Qty = "1";
                    obj1.UOM = "1";
                    obj1.Inserted_Date = DateTime.Now;
                    obj1.Inserted_Host =userhost;
                    obj1.Inserted_User_ID = userid;
                    db.RS_BOM_Item.Add(obj1);
                    db.SaveChanges();

                    var bomitem = db.RS_BOM_Item.Where(c=>c.Model_Code== RS_BOM_Item.Model_Code).ToList();
                    foreach (var item in bomitem)
                    {
                        RS_BOM_Item newobj =new RS_BOM_Item();
                        newobj = db.RS_BOM_Item.Find(item.Row_ID);
                        newobj.Updated_Date = System.DateTime.Now;
                        newobj.Updated_User_ID = userid;
                        newobj.Updated_Host = userhost;
                        newobj.Bom_Version=bomversion+1;
                        db.Entry(newobj).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "Bom Edited";
                    globalData.messageDetail = "Bom Edited successfully";
                    globalData.pageTitle = ResourceGlobal.Edit + " " + "Bom Edited" + " " + ResourceGlobal.Form;
                    globalData.subTitle = "BOM Edit";
                    globalData.controllerName = "BOM";
                    globalData.actionName = ResourceGlobal.Edit;
                    ViewBag.GlobalDataModel = globalData;
                    return RedirectToAction("Index");
                }
            }
            return View(RS_BOM_Item);
        }

        // GET: Bom/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_BOM_Item RS_BOM_Item = db.RS_BOM_Item.Find(id);
            if (RS_BOM_Item == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.BOM;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Bom";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.BOM + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.BOM + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_BOM_Item);
        }

        // POST: Bom/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RS_BOM_Item RS_BOM_Item = db.RS_BOM_Item.Find(id);
            db.RS_BOM_Item.Remove(RS_BOM_Item);
            db.SaveChanges();
            globalData.isSuccessMessage = true;
            globalData.messageTitle = "Bom Deleted";
            globalData.messageDetail = "Bom Deleted successfully";
            globalData.pageTitle = ResourceGlobal.Delete + " " + "Bom Deleted" + " " + ResourceGlobal.Form;
            globalData.subTitle = "BOM Delete";
            globalData.controllerName = "BOM";
            globalData.actionName = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            return RedirectToAction("Index");
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
