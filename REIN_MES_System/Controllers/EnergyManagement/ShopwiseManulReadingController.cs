
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.MasterManagement
{
    public class ShopwiseManulReadingController : Controller
    {
        // GET: ShopwiseManulReading
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();
       
        public ActionResult Index()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                int userId = ((FDSession)this.Session["FDSession"]).userId;
                globalData.pageTitle = ResourceShopwiseManualReading.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                DateTime consumptiondate = System.DateTime.Now;
                consumptiondate = consumptiondate.AddDays(-8);
                var ManualConsumtion = (from s in db.ShopwiseManualConsumptions
                                        
                                   
                                        select (s)).ToList();

                ManualConsumtion = ManualConsumtion.OrderByDescending(s => s.ConsumptionDate).ToList();

                //ManualConsumtion = ManualConsumtion.Where(s => Convert.ToDateTime(s.ConsumptionDate).Year == System.DateTime.Now.Year
                //                                           && Convert.ToDateTime(s.ConsumptionDate).Month == System.DateTime.Now.Month
                //                                           && Convert.ToDateTime(s.ConsumptionDate).Day == System.DateTime.Now.Day).ToList();

                ManualConsumtion = ManualConsumtion.Where(s => (s.ConsumptionDate) > consumptiondate).ToList();


                return View(ManualConsumtion);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "user");
            }
        }

        // GET: ShopwiseManulReading/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ShopwiseManulReading/Create
        public ActionResult Create()
        {
            try
            {
                int userId = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceShopwiseManualReading.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = plantID;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                ViewBag.FeederName = new SelectList(db.MM_Feeders.Where(s => s.Plant_ID == plantID), "Feeder_Id", "FeederName");
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "user");
            }

        }

        // POST: ShopwiseManulReading/Create
        //[HttpPost]
        //public ActionResult Create(ShopwiseManualConsumption obj)
        //{
        //    try
        //    {
        //        int userId = ((FDSession)this.Session["FDSession"]).userId;
        //        int plantID = ((FDSession)this.Session["FDSession"]).plantId;
               
        //        globalData.pageTitle = ResourceShopwiseManualReading.PageTitle;
        //        globalData.isSuccessMessage = true;
        //        globalData.messageDetail = ResourceShopwiseManualReading.Added_Manaul_Reading;
        //        //DateTime consumptionDate = System.DateTime.Now;
        //        //consumptionDate = consumptionDate.AddDays(-1);
        //        DateTime consumptionDate = Convert.ToDateTime(obj.ConsumptionDate);
        //        if (ModelState.IsValid)
        //        {

        //            if (obj.IsFeederExists(plantID, Convert.ToInt16(obj.Shop_ID), 0, consumptionDate.Date, Convert.ToInt16(obj.TagIndex)))
        //            {
        //                ModelState.AddModelError("TagIndex", ResourceShopwiseManualReading.Shop_Error_FeederName);
        //            }
        //            else
        //            {
        //                obj.Plant_ID = plantID;
        //                var feederName = (from f in db.UtilityMainFeederMappings
        //                                  where f.Plant_ID == plantID && f.Shop_ID == obj.Shop_ID && f.TagIndex == obj.TagIndex
        //                                  select new
        //                                  {
        //                                      f.Feeder_ID
        //                                  }).FirstOrDefault();
        //                obj.Feeder_ID = feederName.Feeder_ID;
        //                obj.ConsumptionDate = consumptionDate;
        //                obj.Inserted_User_ID = userId;
        //                obj.ConsumptionType = true;
        //                obj.Inserted_Date = System.DateTime.Now;
        //                db.ShopwiseManualConsumptions.Add(obj);
        //                db.SaveChanges();
        //                TempData["globalData"] = globalData;
        //                return RedirectToAction("Index");
        //            }
    
        //        }
        //        ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name", obj.Shop_ID);
        //        return View(obj);
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        // GET: ShopwiseManulReading/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {

                DateTime consumptionDate = System.DateTime.Now;
                consumptionDate = consumptionDate.AddDays(-1);
                DateTime date = DateTime.Now.Date;
                ShopwiseManualConsumption obj = db.ShopwiseManualConsumptions.Find(id);
                if (obj == null)
                {
                    return HttpNotFound();
                }
                //var time1 = TimeSpan.Parse("00:00:00.000");

                //consumptionDate = (date + time1);
                var ManualConsumtion = (from s in db.ShopwiseManualConsumptions
                                        where s.Shop_ID ==obj.Shop_ID

                                        select (s)).ToList();

                // ManualConsumtion = ManualConsumtion.OrderByDescending(s => s.ConsumptionDate).ToList();
                ManualConsumtion = ManualConsumtion.Where(s => Convert.ToDateTime(s.ConsumptionDate).Year == consumptionDate.Year
                                                           && Convert.ToDateTime(s.ConsumptionDate).Month == consumptionDate.Month
                                                           && Convert.ToDateTime(s.ConsumptionDate).Day == consumptionDate.Day).ToList();

                ViewBag.yesturdayConsumption = ManualConsumtion[0].Consumption;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name",obj.Shop_ID);
                ViewBag.TagIndex = new SelectList(db.UtilityMainFeederMappings.Where(s => s.Plant_ID == plantID && s.ManualMeter == true && s.Shop_ID == obj.Shop_ID), "TagIndex", "FeederName", obj.Shop_ID);
                globalData.pageTitle = ResourceParameterMaster.pageTitle;
                ViewBag.GlobalDataModel = globalData;
               
                return View(obj);

            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceParameterMaster.pageTitle;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");

            }
        }

        // POST: ShopwiseManulReading/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ShopwiseManualConsumption obj)
        {
            try
            {

                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                string compName = ((FDSession)this.Session["FDSession"]).userHost;
                ShopwiseManualConsumption obj1 = db.ShopwiseManualConsumptions.Find(id);

                obj1.Consumption = obj.Consumption;
                obj1.Consider = obj.Consider;
                obj1.ConsumptionType = true;
                obj1.Updated_Date = System.DateTime.Now;
                obj1.Updated_User_ID = userID;
                obj1.Updated_Host = compName;         
                db.SaveChanges();
                // TODO: Add update logic here

                globalData.isSuccessMessage = true;
                globalData.pageTitle = ResourceShopwiseManualReading.PageTitle;
                globalData.messageDetail = ResourceShopwiseManualReading.Edit_Manaul_Reading;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ShopwiseManulReading/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                ShopwiseManualConsumption mm_shops = db.ShopwiseManualConsumptions.Find(id);
                db.ShopwiseManualConsumptions.Remove(mm_shops);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceMM_Shops.pagetitle;
                globalData.messageDetail = ResourceMM_Shops.MM_Shops_Success_MM_Shops_Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                generalObj.addControllerException(ex, "ShopwiseManulReading", "Delete", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceShopwiseManualReading.PageTitle;
                globalData.messageDetail = ResourceShopwiseManualReading.Delete_Manaul_Reading;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
        }

        // POST: ShopwiseManulReading/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                ShopwiseManualConsumption mm_shops = db.ShopwiseManualConsumptions.Find(id);
                db.ShopwiseManualConsumptions.Remove(mm_shops);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceMM_Shops.pagetitle;
                globalData.messageDetail = ResourceMM_Shops.MM_Shops_Success_MM_Shops_Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                generalObj.addControllerException(ex, "ShopwiseManulReading", "Delete", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceShopwiseManualReading.PageTitle;
                globalData.messageDetail = ResourceShopwiseManualReading.Delete_Manaul_Reading;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
        }

        public ActionResult AllConfigurationExcelUpload()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
          
            //List<SelectListItem> listModel = new List<SelectListItem>();
            //ViewBag.Line_ID = new SelectList(listModel);

            if (TempData["OrderUploadRecords"] != null)
            {
                ViewBag.OrderUploadRecords = TempData["OrderUploadRecords"];
            }
            else
            {
                ViewBag.OrderUploadRecords = null;
            }
            globalData.pageTitle = ResourceShopwiseManualReading.PageTitle;
            ViewBag.GlobalDataModel = globalData;
            //globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.CBM_Dashboard + " " + ResourceGlobal.Form;
            //globalData.subTitle = ResourceGlobal.Upload;
            //globalData.controllerName = "EmployeeSkillSet";
            //globalData.actionName = ResourceGlobal.Upload;
            //globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Add_User + " " + ResourceGlobal.Form;
            //globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Add_User + " " + ResourceGlobal.Form;
            //ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult AllConfigurationExcelUpload(ExcelAllConfiguration formData)
        {
            String createdOrders = "";
            try
            {
                if (ModelState.IsValid)
                {
                    // GlobalOperations globalOperations = new GlobalOperations();
                    string fileName = Path.GetFileName(formData.Excel_File.FileName);
                    string fileExtension = Path.GetExtension(formData.Excel_File.FileName);
                    string fileLocation = Server.MapPath("~/App_Data/" + fileName);
                    System.IO.File.Delete(fileName);
                    DataTable dt = ExcelToDataTable(formData.Excel_File, fileLocation, fileExtension);
                    //String attributeId = formData.Attribute_ID;

                    int userId = ((FDSession)this.Session["FDSession"]).userId;
                    int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                    globalData.pageTitle = ResourceShopwiseManualReading.PageTitle;
                    DateTime consumptionDate = System.DateTime.Now;
                    //consumptionDate = consumptionDate.AddDays(-1);
                    consumptionDate = consumptionDate.Date;


                    if (dt.Rows.Count > 0)
                    {
                        AllConfigurationUploadRecords[] orderUploadRecordsObj = new AllConfigurationUploadRecords[dt.Rows.Count];
                        //MM_Maintenance_Machine_Part mmOrderCreationObj = new MM_Maintenance_Machine_Part();

                        int i = 0;
                        //int ExcelNumber = db.ShopwiseManualConsumptions.Max(m => m.Manual_ID).;
                        foreach (DataRow checkListRow in dt.Rows)
                        {


                            //String PlantName = checkListRow[0].ToString() != null ? checkListRow[0].ToString().Trim() : null;
                            var ShopId = checkListRow[0] != null ? checkListRow[0].ToString().Trim() : null;
                            String ShopName = checkListRow[1].ToString() != null ? checkListRow[1].ToString().Trim() : null;
                            String Meter_Name = checkListRow[2].ToString() != null ? checkListRow[2].ToString().Trim() : null;
                            var TagIndex = checkListRow[3].ToString() != null ? checkListRow[3].ToString().Trim() : null;
                            var Reading = checkListRow[4].ToString() != null ? checkListRow[4].ToString().Trim() : null;
                            var Opertion = checkListRow[5].ToString() != null ? checkListRow[5].ToString().Trim() : null;
                            var Date = checkListRow[6].ToString() != null ? checkListRow[6].ToString().Trim() : null;
                            //var feederID = checkListRow[6].ToString() != null ? checkListRow[7].ToString().Trim() : null;
                            //var feederID = checkListRow[6].ToString() != null ? checkListRow[7].ToString().Trim() : null;
                            var Type = checkListRow[7].ToString() != null ? checkListRow[7].ToString().Trim() : null;
                            var Is_Reading = checkListRow[8].ToString() != null ? checkListRow[8].ToString().Trim() : null;

                            orderUploadRecordsObj[i] = new AllConfigurationUploadRecords();

                            MM_MTTUW_Shops obj = new MM_MTTUW_Shops();
                            ShopwiseManualConsumption manual = new ShopwiseManualConsumption();

                            if (ShopName != "" && ShopName != "" && Meter_Name != "" && TagIndex != "" && Reading != "" && Opertion != "" && Date != "" && Type != "" && Is_Reading != "")
                            {

                                consumptionDate = Convert.ToDateTime(Date);


                                if (manual.IsFeederExists(plantID, Convert.ToInt16(ShopId), 0, consumptionDate.Date, Convert.ToInt16(TagIndex)))
                                {
                                    //ModelState.AddModelError("TagIndex", ResourceShopwiseManualReading.Shop_Error_FeederName);
                                    break;
                                }
                                else
                                {
                                    //obj.Plant_ID = plantID;

                                    // manual.Feeder_ID = Convert.ToInt32(feederID);
                                    manual.ConsumptionDate = consumptionDate;
                                    manual.Inserted_User_ID = userId;
                                    manual.Inserted_Date = System.DateTime.Now;
                                    manual.Consumption = Convert.ToDouble(Reading);
                                    manual.TagIndex = Convert.ToInt32(TagIndex);
                                    manual.Shop_ID = Convert.ToInt32(ShopId);
                                    var feeder = (from f in db.UtilityMainFeederMappings
                                                  where f.Plant_ID == plantID &&  f.TagIndex == manual.TagIndex
                                                  select new
                                                  {
                                                      f.Feeder_ID
                                                  }).FirstOrDefault();
                                    manual.Feeder_ID = feeder.Feeder_ID;
                                    manual.Plant_ID = plantID;



                                    if (Convert.ToInt32(Opertion) == 1)
                                    {
                                        manual.Consider = true;
                                    }
                                    else
                                    {
                                        manual.Consider = false;
                                    }
                                    if (Convert.ToInt32(Type) == 1)
                                    {
                                        manual.ConsumptionType = true;
                                    }
                                    else
                                    {
                                        manual.ConsumptionType = false;
                                    }
                                    if (Convert.ToInt32(Is_Reading) == 0)
                                    {
                                        manual.FinalConsumption = manual.Consumption;
                                        manual.Is_Reading = false;

                                    }
                                    else
                                    {
                                        manual.Is_Reading = true;
                                    }

                                    db.ShopwiseManualConsumptions.Add(manual);
                                    db.SaveChanges();
                                    ViewBag.GlobalDataModel = globalData;
                                    // return RedirectToAction("Index");
                                }
                            }



                            i = i + 1;
                        }

                        if (i < dt.Rows.Count)
                        {
                            globalData.isAlertMessage = true;
                            globalData.messageTitle = ResourceShopwiseManualReading.PageTitle;
                            globalData.messageDetail = ResourceGlobal.Excel + " " + ResourceShopwiseManualReading.Duplicate_Entry;
                            globalData.subTitle = ResourceGlobal.Upload;
                            ViewBag.GlobalDataModel = globalData;
                            TempData["globalData"] = globalData;
                        }
                        else
                        {
                            TempData["OrderUploadRecords"] = orderUploadRecordsObj;
                            //TempData["ChecklistDataTable"] = dt;
                            ViewBag.OrderUploadRecords = orderUploadRecordsObj;
                            ViewBag.dt = orderUploadRecordsObj;
                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceShopwiseManualReading.PageTitle;
                            globalData.messageDetail = ResourceGlobal.Excel + " " + ResourceShopwiseManualReading.Added_Manaul_Reading;
                            globalData.subTitle = ResourceGlobal.Upload;
                            ViewBag.GlobalDataModel = globalData;
                            TempData["globalData"] = globalData;

                            ViewBag.createdOrders = createdOrders;
                        }

                    }

                    //return PartialView("QualityChecklistDetails");

                }
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
           
            //return View();
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
        public ActionResult YesterdayMeterReading(int ShopID,int TagIndex)
        {
            var TodayDate = System.DateTime.Today;
            var YesterdayDate = TodayDate.AddDays(-1);
            var Reading = db.ShopwiseManualConsumptions.Where(s => s.Shop_ID == ShopID &&
            s.TagIndex == TagIndex &&
            s.ConsumptionDate == YesterdayDate).Select(s => s.Consumption).FirstOrDefault();
           
            return Json(Reading,JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExcelFormatData()
        {
            string filename = @"~\ExcelData\AllConsumption.xlsx";
            string contenttype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
          
            return File(filename,contenttype,Path.GetFileName(filename));
            //return View();
        }
    }
}
