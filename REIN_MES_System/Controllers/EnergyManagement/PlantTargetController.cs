using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.ZHB_AD
{
    public class PlantTargetController : Controller
    {
        // GET: PlantTarget
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();
        public ActionResult Index()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                //Session["PageTitle"] = "Main Feeder (Meter) Configuration";
                globalData.pageTitle = ResourceModules.PlantTarget;

                //globalData.controllerName = "StationRoles";
                //globalData.actionName = ResourceGlobal.Details;
                //globalData.contentTitle = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
                //globalData.contentFooter = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
                ViewBag.GlobalDataModel = globalData;
                return View((from u in db.MM_PlantTarget
                              where u.Plant_ID == plantID
                             select (u)).ToList());
                // return View(db.MM_PowerTarget.ToList());
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");
            }
        }

        public ActionResult Create()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;

              

                List<TargetPlants_Result> shops = new List<TargetPlants_Result>();
                ViewBag.Plant_ID = plantID;
             
                var months = DateTimeFormatInfo
  .InvariantInfo
  .MonthNames
  .TakeWhile(monthName => monthName != String.Empty)
  .Select((monthName, index) => new SelectListItem
  {
      Value = string.Format("{0} ", monthName),
      Text = string.Format("{0} ", monthName)
  });

                ViewBag.Month = months;

                DateTime date = System.DateTime.Now;
                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string FinYear = null;

                if (DateTime.Today.Month > 3)
                {

                    FinYear = CurYear + "-" + NexYear;
                }
                else
                {
                    CurYear = CurrentYear.ToString();
                    FinYear = PreYear + "-" + CurYear;
                }
                ViewBag.Year = FinYear;
                globalData.pageTitle = ResourceTargetConsumption.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                return View();
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_PlantTarget mM_PowerTarget)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                var host = ((FDSession)this.Session["FDSession"]).userHost;
                string Month = mM_PowerTarget.Month;
                string Year = mM_PowerTarget.Year;
                int Plant = Convert.ToInt16(mM_PowerTarget.Plant_ID);
                if (ModelState.IsValid)
                {
                    if (mM_PowerTarget.IsMonthExists(Plant, Year, Month, 0))
                    {
                        ModelState.AddModelError("Month", ResourceValidation.Exist);
                    }
                    else
                    {
                        mM_PowerTarget.Inserted_User_ID = userID;
                        mM_PowerTarget.Inserted_Date = System.DateTime.Now;
                        mM_PowerTarget.Inserted_Host = host;
                        db.MM_PlantTarget.Add(mM_PowerTarget);
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;

                        globalData.messageTitle = ResourceModules.PlantTarget;
                        globalData.messageDetail = ResourceGlobal.Save;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }

                }
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name");
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                var months = DateTimeFormatInfo
  .InvariantInfo
  .MonthNames
  .TakeWhile(monthName => monthName != String.Empty)
  .Select((monthName, index) => new SelectListItem
  {
      Value = string.Format("{0} ", monthName),
      Text = string.Format("{0} ", monthName)
  });

                ViewBag.Month = months;

                DateTime date = System.DateTime.Now;
                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string FinYear = null;

                if (DateTime.Today.Month > 3)
                {

                    FinYear = CurYear + "-" + NexYear;
                }
                else
                {
                    CurYear = CurrentYear.ToString("yy");
                    FinYear = PreYear + "-" + CurYear;
                }
                ViewBag.Year = FinYear;
                return View(mM_PowerTarget);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

        }


        public ActionResult Edit(decimal? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                MM_PlantTarget mM_PowerTarget = db.MM_PlantTarget.Find(id);
                if (mM_PowerTarget == null)
                {
                    return HttpNotFound();
                }
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                int PlantId = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceModules.PlantTarget;
                ViewBag.GlobalDataModel = globalData;
                return View(mM_PowerTarget);
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_PlantTarget mM_PowerTarget)
        {
            int PlantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                if (mM_PowerTarget.IsMonthExists(PlantId, mM_PowerTarget.Year, mM_PowerTarget.Month, 0))
                {
                    ModelState.AddModelError("Month", ResourceValidation.Exist);
                }
                else
                {


                    db.Entry(mM_PowerTarget).State = EntityState.Modified;
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.PlantTarget;
                    globalData.messageDetail = ResourceGlobal.Save;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }
            return View(mM_PowerTarget);
        }


        public ActionResult AllConfigurationExcelUpload()
        {


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


            if (ModelState.IsValid)
            {
                // GlobalOperations globalOperations = new GlobalOperations();
                string fileName = Path.GetFileName(formData.Excel_File.FileName);
                string fileExtension = Path.GetExtension(formData.Excel_File.FileName);
                string fileLocation = Server.MapPath("~/App_Data/" + fileName);
                System.IO.File.Delete(fileName);
                DataTable dt = ExcelToDataTable(formData.Excel_File, fileLocation, fileExtension);
                //String attributeId = formData.Attribute_ID;
                var host = ((FDSession)this.Session["FDSession"]).userHost;
                int userId = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceModules.PlantTarget;
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
                        //var ShopId = checkListRow[0] != null ? checkListRow[0].ToString().Trim() : null;
                        //String ShopName = checkListRow[1].ToString() != null ? checkListRow[1].ToString().Trim() : null;
                        String Year = checkListRow[2].ToString() != null ? checkListRow[2].ToString().Trim() : null;
                        String Month = checkListRow[2].ToString() != null ? checkListRow[3].ToString().Trim() : null;
                        var Target = checkListRow[3].ToString() != null ? checkListRow[4].ToString().Trim() : null;

                        //int shop = Convert.ToInt32(ShopId);

                        orderUploadRecordsObj[i] = new AllConfigurationUploadRecords();

                        MM_Shops obj = new MM_Shops();
                        MM_PlantTarget mM_PowerTarget = new MM_PlantTarget();

                        if ( Year != "" && Month != "" && Target != "")
                        {

                            if (mM_PowerTarget.IsMonthExists(plantID, Year, Month, 0))
                            {
                                break;
                            }
                            else
                            {
                                //obj.Plant_ID = plantID;
                                //var feederName = (from f in db.UtilityMainFeederMappings
                                //                  where f.Plant_ID == plantID && f.Shop_ID == obj.Shop_ID && f.TagIndex == obj.TagIndex
                                //                  select new
                                //                  {
                                //                      f.FeederName
                                //                  }).FirstOrDefault();
                                mM_PowerTarget.Inserted_User_ID = userId;
                                mM_PowerTarget.Inserted_Date = System.DateTime.Now;
                                mM_PowerTarget.Inserted_Host = host;                          
                                mM_PowerTarget.Year = Year;
                                mM_PowerTarget.Month = Month;
                                mM_PowerTarget.Plant_ID = plantID;
                                mM_PowerTarget.Target = Convert.ToDouble(Target);
                                db.MM_PlantTarget.Add(mM_PowerTarget);
                                db.SaveChanges();
                                //db.SaveChanges();
                                ViewBag.GlobalDataModel = globalData;
                                // return RedirectToAction("Index");
                            }
                        }



                        i = i + 1;
                    }

                    if (i < dt.Rows.Count)
                    {
                        globalData.isAlertMessage = true;
                        globalData.messageTitle = ResourceTargetConsumption.pageTitle;
                        globalData.messageDetail = ResourceGlobal.Excel + " " + ResourceTargetConsumption.Duplicate_Entry;
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
                        globalData.messageTitle = ResourceTargetConsumption.pageTitle;
                        globalData.messageDetail = ResourceGlobal.Excel + " " + ResourceGlobal.Save;
                        globalData.subTitle = ResourceGlobal.Upload;
                        ViewBag.GlobalDataModel = globalData;
                        TempData["globalData"] = globalData;

                        ViewBag.createdOrders = createdOrders;
                    }

                }

                //return PartialView("QualityChecklistDetails");

            }
            return RedirectToAction("Index");
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

        // GET: TargetConsumption/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_PowerTarget mM_PowerTarget = db.MM_PowerTarget.Find(id);
            if (mM_PowerTarget == null)
            {
                return HttpNotFound();
            }
            return View(mM_PowerTarget);
        }

        // POST: TargetConsumption/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                MM_PlantTarget mM_PowerTarget = db.MM_PlantTarget.Find(id);
                db.MM_PlantTarget.Remove(mM_PowerTarget);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.PlantTarget;
                globalData.messageDetail = ResourceGlobal.Delete;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                generalObj.addControllerException(ex, "PlantTarget", "delete", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.PlantTarget;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                //ViewBag.Plant_ID = new SelectList(db.M_Plants, "Plant_ID", "Plant_Name", mM_Category.Plant_ID);
                return RedirectToAction("Index");
            }

        }

        public ActionResult ExcelFormatData()
        {
            string filename = @"~\ExcelData\Traget.xlsx";
            string contenttype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(filename, contenttype, Path.GetFileName(filename));
            //return View();
        }
        public partial class TargetPlants_Result
        {
            public decimal Shop_ID { get; set; }
            public string Shop_Name { get; set; }
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

