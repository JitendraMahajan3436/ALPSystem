using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Models;
using REIN_MES_System.Helper;
using System.IO;
using System.Data.OleDb;
using System.Globalization;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using REIN_MES_System.Controllers.BaseManagement;
using Newtonsoft.Json;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /*               Controller Name           : OrderCreationController
     *               Description               : Controller used to creating the order . 
     *               Author, Timestamp         : Jitendra Mahajan
     */
    public class OrderCreationController : BaseController
    {
        int shopid = 0;
        int plantid = 0;
        string order_no = "";
        string order_type = "";
        int country;
        string model_code = "";
        int qty = 0;
        string colour;
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_OM_Creation mmOmCreation = new RS_OM_Creation();
        General generalHelper = new General();

        /*               Action Name               : Index
         *               Description               : Action used to show the list of Order Creation Data and populating the all data related to order creation.
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Order_Creation
        public ActionResult Index()
        {
            plantid = ((FDSession)this.Session["FDSession"]).plantId;
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            // var Started_Qty = (from Order_Start in db.RS_OM_OrderRelease where Order_Start= select Order_Start).Count();


            globalData.pageTitle = ResourceModules.OM_Creation;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Order Creation";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.OM_Creation;
            globalData.contentFooter = ResourceModules.OM_Creation;

            ViewBag.GlobalDataModel = globalData;

            //ViewBag.Platform = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name");
            //var Platform = (from platform in db.RS_OM_Platform
            //                where platform.Platform_ID == db.RS_OM_Creation.Find(Platform_Id) && platform.Plant_ID == plantid
            //                select new
            //                {
            //                    Platform_ID = platform.Platform_ID,
            //                    Platform_Name = platform.Platform_Name
            //                });

            ///modified by mukesh
            //DateTime time = DateTime.Now.Date;
            //var RS_OM_Creation = db.RS_OM_Creation.Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Country).Where(c => c.Inserted_Date.Value.ToString("dd.MM.yy") == time.ToString("dd.MM.yy"));
            var RS_OM_Creation = db.RS_OM_Creation.Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Country);


            var data = db.RS_OM_Creation
               .Where(c =>  c.Plant_ID == plantid).OrderByDescending(m=>m.Planned_Date).ThenBy(m=>m.Plant_OrderNo)
               .ToList();

            //current
            //var data = db.RS_OM_Creation.AsEnumerable()
            //   .Where(c => c.Inserted_Date.Value.ToString("dd.MM.yy") == time.ToString("dd.MM.yy") && c.Plant_ID == plantid)
            //   .ToList();


            return View(data);
        }

        /*               Action Name               : GetPlantID
         *               Description               : Action used to find the shop Id for new order creation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : plant_Id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Shop 
        public ActionResult GetPlantID(int Plant_Id)
        {
            var Shop_Id = db.RS_Shops
                                       .Where(c => c.Plant_ID == Plant_Id)
                                       .Select(c => new { c.Shop_ID, c.Shop_Name });

            return Json(Shop_Id, JsonRequestBehavior.AllowGet);
        }

        /*               Action Name               : GetShopMode
         *               Description               : Action used to find the order type for new order creation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Order_type
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find GetShopMode
        //public ActionResult GetShopMode(String order_type)
        //{
        //    try
        //    {
        //        plantid = ((FDSession)this.Session["FDSession"]).plantId;
        //        if (order_type == "S")
        //        {
        //            var st = from shop in db.RS_Shops
        //                     where shop.Shop_Name != "Tractor" && shop.Plant_ID == plantid
        //                     select new
        //                     {
        //                         Id = shop.Shop_ID,
        //                         Value = shop.Shop_Name
        //                     };
        //            return Json(st, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var st = from shop in db.RS_Shops
        //                     where shop.Shop_Name.Contains("Tractor") && shop.Plant_ID == plantid
        //                     select new
        //                     {
        //                         Id = shop.Shop_ID,
        //                         Value = shop.Shop_Name
        //                     };
        //            return Json(st, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult GetShopMode(String order_type)
        {
            try
            {
                plantid = ((FDSession)this.Session["FDSession"]).plantId;
                if (order_type == "S")
                {
                    var st = from shop in db.RS_Shops
                             where shop.Is_Main == false && shop.Plant_ID == plantid
                             select new
                             {
                                 Id = shop.Shop_ID,
                                 Value = shop.Shop_Name
                             };
                    return Json(st, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var st = from shop in db.RS_Shops
                             where shop.Is_Main == true && shop.Plant_ID == plantid
                             select new
                             {
                                 Id = shop.Shop_ID,
                                 Value = shop.Shop_Name
                             };
                    return Json(st, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /*               Action Name               : GetPartNumber
         *               Description               : Action used to find the Part Number for new order creation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : shop_Id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Part number
        public ActionResult GetPartNumber(int Shop_Id, decimal? series_code = null)
        {
            try
            {
                if (series_code == null)
                {
                    var model = (from modelMaster in db.RS_Model_Master
                                 where (modelMaster.Shop_ID == Shop_Id)
                                 orderby modelMaster.Model_Code ascending
                                 select new { Id = modelMaster.Model_Code, Value = modelMaster.Model_Code });
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    var model = (from modelMaster in db.RS_Model_Master
                                 where (modelMaster.Shop_ID == Shop_Id && modelMaster.Series_Code == series_code)
                                 orderby modelMaster.Model_Code ascending
                                 select new { Id = modelMaster.Model_Code, Value = modelMaster.Model_Code });
                    return Json(model, JsonRequestBehavior.AllowGet);
                }

                //return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /*               Action Name               : GetPartSeries
         *               Description               : Action used to find the part Series for new order creation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : model_code
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Part Series
        public ActionResult GetPartSeries(int Shop_id)
        {
            try
            {
                var seriesCode = from series in db.RS_Series
                                 where series.Shop_ID == Shop_id
                                 orderby series.Series_Description
                                 select new { Series_Code = series.Series_Code, Series_Description = series.Series_Description };
                return Json(seriesCode, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /*               Action Name               : Details
         *               Description               : Action used to get Details of Specific Order in Details
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Order_Creation/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Creation RS_OM_Creation = db.RS_OM_Creation.Find(id);
            if (RS_OM_Creation == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_Creation);
        }

        /*               Action Name               : Create
         *               Description               : Action used to get the data which used to create for new order creation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : plant_Id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Order_Creation/Create
        public ActionResult Create()
        {
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;


                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                //Global Data
                globalData.pageTitle = ResourceModules.OM_Creation;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Order Creation";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;


                ////Stattic Order List for Order Type
                //var ordet_List = new SelectList(new[]
                //{
                //     new { ID = "P", Name = "Parent" },
                //    new { ID = "S", Name = "Spare" },
                //},
                //        "ID", "Name", 0);

                //static list for Priority

                var priority = new SelectList(new[]
                    {
                 new { ID = "1", Name = "Normal" },
                 new { ID = "99", Name = "High" },
                },
                 "ID", "Name", 1);


                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Plant_ID == plantId), "Line_ID", "Line_Name");
                ViewBag.Colour_ID = new SelectList(db.RS_Colour.Where(p => p.Plant_ID == plantId), "Colour_ID", "Colour_Desc");
                ViewBag.Country_ID = new SelectList(db.RS_Country, "Country_ID", "Country_Name");
                // ViewBag.Series_code = new SelectList(db.RS_Series.Where(p => p.Plant_ID == plantId), "Series_Code", "Series_Description");
                //ViewBag.Order_Type = ordet_List;
                ViewBag.Order_Type = new SelectList(db.RS_OM_Order_Type.Where(p => p.Shop_ID == 0), "Order_Type_Name", "Order_Type_Name");
                ViewBag.Priority = priority;
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }

        /*               Action Name               : Create
         *               Description               : Action Create for new order creation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : RS_OM_Creation
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // POST: Order_Creation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_OM_Creation RS_OM_Creation)
        {
            if (ModelState.IsValid)
            {
                int plantId, shopId = 0;
                int? lineId = null;
                DateTime nowTime = DateTime.Now;
                //shopID = Convert.ToInt16(frmFieldsArr["Shop_ID"]);
                //lineID = Convert.ToInt16(frmFieldsArr["Line_ID"]);

                try
                {
                    //checking for valid model code against order type
                    if (db.RS_Model_Master.Any(m => m.Model_Code == RS_OM_Creation.Model_Code && m.Is_Spare == true))
                    {
                        string OrderType = "Spare";
                        if (RS_OM_Creation.Order_Type.Trim().ToLower() != OrderType.Trim().ToLower())
                        {
                            ModelState.AddModelError("Model_Code", ResourceValidation.Spare_Order_Valid);

                            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_Creation.Plant_ID);
                            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == RS_OM_Creation.Plant_ID), "Shop_ID", "Shop_Name", RS_OM_Creation.Shop_ID);
                            ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc", RS_OM_Creation.Colour_ID);
                            ViewBag.shopId = RS_OM_Creation.Shop_ID;
                            ViewBag.modelCode = RS_OM_Creation.Model_Code;
                            ViewBag.Order_Type = new SelectList(db.RS_OM_Order_Type, "Order_Type_Name", "Order_Type_Name", RS_OM_Creation.Order_Type);
                            ViewBag.Country_ID = new SelectList(db.RS_Country, "Country_ID","Country_Name");
                            //ViewBag.Priority = priority;

                            //return View(RS_OM_Creation);
                            return View("Create", RS_OM_Creation);
                        }
                    }
                    plantId = ((FDSession)this.Session["FDSession"]).plantId;
                    shopId = Convert.ToInt32(RS_OM_Creation.Shop_ID);
                    lineId = null;//need to change
                    int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

                    globalData.pageTitle = ResourceModules.OM_Creation;
                    globalData.subTitle = ResourceGlobal.Create;
                    globalData.controllerName = "Order Creation";
                    globalData.actionName = ResourceGlobal.Create;
                    globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
                    ViewBag.GlobalDataModel = globalData;


                    var currentDate = new DateTime();
                    DateTime dt = currentDate.Date;
                    int qty = 0;
                    int or_qty = 0;
                    int rel_qty = 0;

                    shopid = Convert.ToInt32(shopId);
                    plantid = Convert.ToInt32(plantId);
                    country = Convert.ToInt32(RS_OM_Creation.Country_ID);
                    model_code = RS_OM_Creation.Model_Code;
                    or_qty = RS_OM_Creation.Qty;
                    rel_qty = Convert.ToInt32(RS_OM_Creation.Release_Qty);

                    //if (RS_OM_Creation.GetValidateOrderNumber(plantId, shopId, model_code, 0))
                    {
                        RS_OM_OrderRelease mmOrderReleaseObj = new RS_OM_OrderRelease();
                        String res = mmOrderReleaseObj.isOrderValidToCreate(RS_OM_Creation.Model_Code, RS_OM_Creation.Series_code);
                        if (!String.IsNullOrEmpty(res))
                        {
                            ModelState.AddModelError("Series_Code", res);

                            globalData.isErrorMessage = true;
                            globalData.messageTitle = "Error";
                            globalData.messageDetail = res;
                            ViewBag.GlobalDataModel = globalData;
                        }
                        else
                        {
                            //Update Quantity

                            //RS_OM_Creation orCreaton = (from ocreation in db.RS_OM_Creation
                            //                            where ocreation.Shop_ID == shopid && ocreation.Plant_ID == plantid && ocreation.Model_Code == model_code
                            //                          select ocreation).Take(1).FirstOrDefault();


                            //qty = orCreaton.Qty + or_qty;

                            //RS_OM_Creation mmOMCreationObj = new RS_OM_Creation();
                            //mmOMCreationObj = db.RS_OM_Creation.Find(orCreaton.Plant_OrderNo);
                            //mmOMCreationObj.Qty = qty;
                            //mmOMCreationObj.Inserted_Date = DateTime.Now;
                            //mmOMCreationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            //mmOMCreationObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            //mmOMCreationObj.Updated_Date = DateTime.Now;

                            //db.Entry(mmOMCreationObj).State = EntityState.Modified;
                            //db.SaveChanges();

                            //Insert 

                            decimal orderNo = RS_OM_Creation.GetLastOrderNumber(plantid, shopid);
                            RS_OM_Creation.Plant_OrderNo = orderNo;
                            RS_OM_Creation.Inserted_Date = DateTime.Now;
                            RS_OM_Creation.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            RS_OM_Creation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            RS_OM_Creation.Updated_Date = DateTime.Now;
                            //added by ketan Date 19-08-17
                            bool IsColorCheck = Convert.ToBoolean(db.RS_Model_Master.Where(m => m.Model_Code == model_code).Select(m => m.Color_Code).FirstOrDefault());
                            if (IsColorCheck)
                            {
                                var color_code = model_code.Substring(model_code.Length - 2, 2);
                                RS_OM_Creation.Colour_ID = color_code;
                                //var color_Name = db.MM_Color.Where(m => m.COLOUR_ID == color_code).Select(c => c.COLOUR_DESC );
                            }
                            else
                            {
                                RS_OM_Creation.Colour_ID = RS_OM_Creation.Colour_ID;
                            }
                           
                            //RS_OM_Creation.Colour_ID = "GR";
                            //RS_OM_Creation.Colour_ID = db.RS_Model_Master.Find(model_code).Colour_ID;                           
                            db.RS_OM_Creation.Add(RS_OM_Creation);
                            db.SaveChanges();
                            
                            //return RedirectToAction("CreateOrders", "OMOrderRelease", new { rowId = RS_OM_Creation.Row_ID, remark = "", quantity = RS_OM_Creation.Qty, isCreationRequest = true });

                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceModules.OM_Creation;
                            globalData.messageDetail = ResourceGlobal.Order + " " + ResourceMessages.Create_Success;
                            TempData["globalData"] = globalData;
                            return RedirectToAction("Create");
                        }

                    }
                    //else
                    //{
                    //    decimal orderNo = RS_OM_Creation.GetLastOrderNumber(plantid, shopid);
                    //    RS_OM_Creation.Plant_OrderNo = orderNo;
                    //    RS_OM_Creation.Inserted_Date = DateTime.Now;
                    //    RS_OM_Creation.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //    RS_OM_Creation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //    RS_OM_Creation.Updated_Date = DateTime.Now;
                    //    RS_OM_Creation.Colour_ID = "GR";

                    //    db.RS_OM_Creation.Add(RS_OM_Creation);
                    //    db.SaveChanges();

                    //    return RedirectToAction("CreateOrders", "OMOrderRelease", new { rowId = RS_OM_Creation.Row_ID, remark = "", quantity = RS_OM_Creation.Qty, isCreationRequest = true });

                    //    globalData.isSuccessMessage = true;
                    //    globalData.messageTitle = ResourceOrder_Creation.Order_Creation;
                    //    globalData.messageDetail = ResourceOrder_Creation.OrderCreation_Success_Add_Success;
                    //    TempData["globalData"] = globalData;
                    //    return RedirectToAction("Index");

                    //}
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
                {
                    Exception raise = dbex;
                    foreach (var ValidationErrors in dbex.EntityValidationErrors)
                    {
                        foreach (var validationError in ValidationErrors.ValidationErrors)
                        {
                            string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                            raise = new InvalidOperationException(Message, raise);
                        }
                    }
                    throw raise;
                }
                catch (Exception ex)
                {
                    //globalData.messageTitle = ResourceOrder_Creation.Order_Creation;
                    //globalData.messageDetail = ResourceOrder_Creation.Error_Message;
                    globalData.pageTitle = ResourceModules.OM_Creation;
                    globalData.subTitle = ResourceGlobal.Create;
                    globalData.controllerName = "Order Creation";
                    globalData.actionName = ResourceGlobal.Create;
                    globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;

                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.OM_Creation;
                    globalData.messageDetail = ex.Message.ToString();
                    ViewBag.GlobalDataModel = globalData;
                    TempData["globalData"] = globalData;
                    generalHelper.addControllerException(ex, "OrderCreationController", "create::Post", ((FDSession)this.Session["FDSession"]).userId);

                }

                finally
                {
                    generalHelper.logUserActivity(shopId, lineId, "PPC Module", "Order Create", nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
                }
            }

            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_Creation.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == RS_OM_Creation.Plant_ID), "Shop_ID", "Shop_Name", RS_OM_Creation.Shop_ID);
            ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc", RS_OM_Creation.Colour_ID);
            ViewBag.shopId = RS_OM_Creation.Shop_ID;
            ViewBag.Color = new SelectList(db.RS_Colour, "COLOUR_ID", "COLOUR_DESC");
            ViewBag.modelCode = RS_OM_Creation.Model_Code;
            ViewBag.Order_Type = new SelectList(db.RS_OM_Order_Type, "Order_Type_Name", "Order_Type_Name");
            ViewBag.Country_ID = new SelectList(db.RS_Country, "Country_ID", "Country_Name");
            //ViewBag.Priority = priority;

            //return View(RS_OM_Creation);
            return View("Create", RS_OM_Creation);
        }

        /*               Action Name               : Edit
         *               Description               : Action used to get the data which used to Edit for new order creation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Order_Creation/Edit/5
        //public ActionResult Edit(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RS_OM_Creation RS_OM_Creation = db.RS_OM_Creation.Where(p => p.Row_ID == id).Single();
        //    plantid = ((FDSession)this.Session["FDSession"]).plantId;
        //    if (RS_OM_Creation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    if (RS_OM_Creation.Release_Qty == null)
        //    {
        //        RS_OM_Creation.Release_Qty = 0;
        //    }

        //    if (RS_OM_Creation.Order_Type == "P")
        //    {
        //        RS_OM_Creation.Order_Type = "Parent";
        //    }
        //    else if (RS_OM_Creation.Order_Type == "S")
        //    {
        //        RS_OM_Creation.Order_Type = "Spare";
        //    }

        //    //Series description
        //    //RS_Series series = db.RS_Series.Where(p => p.Series_Code == RS_OM_Creation.Series_code).Single();


        //    globalData.pageTitle = ResourceModules.OM_Creation;
        //    globalData.subTitle = ResourceGlobal.Edit;
        //    globalData.controllerName = "Order Creation";
        //    globalData.actionName = ResourceGlobal.Edit;
        //    globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
        //    globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
        //    ViewBag.GlobalDataModel = globalData;

        //    //  RS_OM_Creation.Series_code = series.Series_Description;
        //    //
        //    //RS_OM_Creation.Series_code = series.Series_Code;

        //    string date = RS_OM_Creation.Planned_Date.Value.ToShortDateString();
        //    // RS_OM_Creation.Planned_Date = date;

        //    ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantid), "Plant_ID", "Plant_Name", RS_OM_Creation.Plant_ID);
        //    ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantid), "Shop_ID", "Shop_Name", RS_OM_Creation.Shop_ID);
        //    ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc", RS_OM_Creation.Colour_ID);
        //    //added by mukesh for line and plantform name
        //    //var line_id = ;
            
        //    //ViewBag.PlatForm_Name = db.RS_OM_Platform.Where(p => p.Platform_ID == db.RS_OM_Creation.Find(id).Platform_Id).Select(m => m.Platform_Name);
        //    // ViewBag.Series_Code = new SelectList(db.RS_Series, "Series_Code", "Series_Description", RS_OM_Creation.Series_code);
        //    return View(RS_OM_Creation);
        //}

        ///*               Action Name               : Edit
        // *               Description               : Action Edit for new order creation
        // *               Author, Timestamp         : Jitendra Mahajan
        // *               Input parameter           : RS_OM_Creation
        // *               Return Type               : ActionResult
        // *               Revision                  : 1
        //*/
        //// POST: Order_Creation/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(RS_OM_Creation RS_OM_Creation)
        //{
        //    plantid = ((FDSession)this.Session["FDSession"]).plantId;
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                shopid = Convert.ToInt16(RS_OM_Creation.Shop_ID);
        //                plantid = Convert.ToInt16(RS_OM_Creation.Plant_ID);
        //                country = Convert.ToInt16(RS_OM_Creation.Country_ID);
        //                model_code = RS_OM_Creation.Model_Code;
        //                //added by mukesh
        //                decimal? partGroupId = null;
        //                ////

        //                RS_OM_Creation mmOMCreationObj = new RS_OM_Creation();
        //                mmOMCreationObj = db.RS_OM_Creation.Find(RS_OM_Creation.Row_ID);

        //                mmOMCreationObj.Shop_ID = shopid;
        //                mmOMCreationObj.Plant_ID = plantid;
        //                // mmOmCreation.Plant_OrderNo = RS_OM_Creation.Plant_OrderNo;
        //                mmOMCreationObj.Model_Code = model_code;

        //                var qtyChange = 0;
        //                {//added by mukesh

        //                    qtyChange = RS_OM_Creation.Qty - mmOMCreationObj.Qty;
        //                }////



        //                //if (RS_OM_Creation.Order_Type == "Parent")
        //                //{
        //                //    mmOMCreationObj.Order_Type = "P";
        //                //}
        //                //else if (RS_OM_Creation.Order_Type == "Spare")
        //                //{
        //                //    mmOMCreationObj.Order_Type = "S";
        //                //}

        //                //mmOMCreationObj.Colour_ID = "GR";
        //                //   RS_OM_Creation.Colour_ID = db.RS_Model_Master.Find(model_code).Colour_ID;
        //                mmOMCreationObj.Qty = RS_OM_Creation.Qty;
        //                mmOMCreationObj.Release_Qty = RS_OM_Creation.Release_Qty;
        //                //mmOMCreationObj.Series_code = RS_OM_Creation.Series_code;
        //                mmOMCreationObj.Planned_Date = RS_OM_Creation.Planned_Date;
        //                mmOMCreationObj.Updated_Date = DateTime.Now;
        //                mmOMCreationObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                mmOMCreationObj.Is_Edited = true;
        //                db.Entry(mmOMCreationObj).State = EntityState.Modified;
        //                db.SaveChanges();

        //                ////added by mukesh//// on increasing or decreasing orders qty in RS_OM_Creation orders added or or deleted from RS_OM_OrderRelease
        //                {                          
        //                    if (qtyChange > 0)
        //                    {
        //                        int maxRowId = db.RS_OM_OrderRelease.Where(x => x.Plant_OrderNo == mmOMCreationObj.Plant_OrderNo).Max(m => m.Row_ID);
        //                        RS_OM_OrderRelease mmOrderRelease = db.RS_OM_OrderRelease.Find(maxRowId);
        //                        if (mmOrderRelease != null)
        //                        {
        //                            for (int i = 0; i < qtyChange; i++)
        //                            {
        //                                RS_OM_OrderRelease omOrderReleaseCreate = new RS_OM_OrderRelease();
        //                                omOrderReleaseCreate = mmOrderRelease;
        //                                omOrderReleaseCreate.Inserted_Date = DateTime.Now;
        //                                omOrderReleaseCreate.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                                int totalOrder = omOrderReleaseCreate.getTotalOrderReleasedByDate(shopid, Convert.ToInt16(mmOrderRelease.Line_ID));
        //                                omOrderReleaseCreate.Order_No = mmOrderRelease.generateOrderNumber(totalOrder + 1, model_code);
        //                                String uToken = new Random().Next(10000, 99999).ToString() + DateTime.Now.Ticks;
        //                                omOrderReleaseCreate.Order_Start = false;
        //                                omOrderReleaseCreate.Is_Edited = false;
        //                                omOrderReleaseCreate.Is_Active = true;
        //                                omOrderReleaseCreate.Is_Deleted = false;
        //                                omOrderReleaseCreate.UToken = uToken;
        //                                omOrderReleaseCreate.Order_Status = "Release";
        //                                omOrderReleaseCreate.Remarks = null;
        //                                omOrderReleaseCreate.Planned_Shift_ID= null;
        //                                DateTime Inserted_Date = omOrderReleaseCreate.Inserted_Date;
        //                                omOrderReleaseCreate.ORN = Convert.ToInt32(omOrderReleaseCreate.generateORNNumber(plantid, Inserted_Date));
        //                                omOrderReleaseCreate.RSN = Convert.ToInt32(omOrderReleaseCreate.generateRSNNumber(plantid, shopid, mmOMCreationObj.Planned_Date));
        //                                omOrderReleaseCreate.CUMN = omOrderReleaseCreate.ORN;

        //                                db.RS_OM_OrderRelease.Add(omOrderReleaseCreate);
        //                                db.SaveChanges();

        //                            }
        //                        }
        //                    }
        //                    else if (qtyChange < 0)
        //                    {

        //                        for (int i = 0; i > qtyChange; i--)
        //                        {
        //                            int maxRowIdDelete = db.RS_OM_OrderRelease.Where(x => x.Plant_OrderNo == mmOMCreationObj.Plant_OrderNo &&x.Order_Status=="Release").Max(m => m.Row_ID);
        //                            RS_OM_OrderRelease mmOrderReleaseDelete = db.RS_OM_OrderRelease.Find(maxRowIdDelete);
        //                            if (mmOrderReleaseDelete != null)
        //                            {
        //                                mmOrderReleaseDelete.Updated_Date = DateTime.Now;
        //                                mmOrderReleaseDelete.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                                mmOrderReleaseDelete.Is_Deleted = true;
        //                               // db.RS_OM_OrderRelease.(mmOrderReleaseDelete);
        //                                db.Entry(mmOrderReleaseDelete).State = EntityState.Deleted;
        //                                db.SaveChanges();
        //                            }
        //                        }

        //                    }                      
        //                }/////
        //                globalData.isSuccessMessage = true;

        //                globalData.pageTitle = ResourceModules.OM_Creation;
        //                globalData.subTitle = ResourceGlobal.Create;
        //                globalData.controllerName = "Order Creation";
        //                globalData.messageTitle = ResourceModules.OM_Creation;
        //                globalData.messageDetail = ResourceModules.OM_Creation + " " + ResourceMessages.Edit_Success;
        //                TempData["globalData"] = globalData;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            globalData.messageTitle = ResourceModules.OM_Creation;
        //            globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceModules.OM_Creation;
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantid), "Plant_ID", "Plant_Name", RS_OM_Creation.Plant_ID);
        //    ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantid), "Shop_ID", "Shop_Name", RS_OM_Creation.Shop_ID);
        //    ViewBag.Colour_ID = new SelectList(db.RS_Colour, "Colour_ID", "Colour_Desc", RS_OM_Creation.Colour_ID);
        //    return View(RS_OM_Creation);
        //}

        /*               Action Name               : Delete
         *               Description               : Action Delete for new order creation
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: Order_Creation/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int ID = Convert.ToInt32(id);
            RS_OM_Creation RS_OM_Creation = db.RS_OM_Creation.Where(p => p.Plant_OrderNo == ID).Single();
            if (RS_OM_Creation == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.OM_Creation;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Order Creation";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_OM_Creation);

        }


        //public ActionResult DeleteConfirmed(FormCollection frm, decimal id)
        //{

        //    var deleteQty =Convert.ToInt32(frm["deleteQty"]);
        //    var plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //    int ID = Convert.ToInt32(id);
        //    RS_OM_Creation mmOmCreation = db.RS_OM_Creation.Where(p => p.Plant_OrderNo == ID).Single();
        //    try
        //    {
        //        ////partial deleting orders according to provided qty
        //        DateTime date = DateTime.Today;
        //        var todaydate = date.ToString("yyyy-MM-dd");
        //        var releaseCount = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Plant_OrderNo == id && m.Order_Status.Equals("Release", StringComparison.CurrentCultureIgnoreCase)
        //         && !(db.RS_OM_U321_Tactsheet_Orders.Any(p => p.Order_No == m.Order_No) || db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(p => p.Order_No == m.Order_No) || db.RS_OM_S201_Tactsheet_Orders.Any(p => p.Order_No == m.Order_No))).Count();

        //        if (deleteQty > releaseCount)
        //        {

        //            globalData.isErrorMessage = true;
        //            globalData.messageTitle = ResourceModules.OM_Creation; ;
        //            globalData.messageDetail = "Invalid Delete Quantity, Some orders are on Hold or Started";
        //            TempData["globalData"] = globalData;
        //            return RedirectToAction("Delete", id);

        //        }
        //        else
        //        {
        //            if (releaseCount == deleteQty)
        //            {
        //                mmOmCreation.Updated_Date = DateTime.Now;
        //                mmOmCreation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                mmOmCreation.Is_Deleted = true;
        //                db.RS_OM_Creation.Remove(mmOmCreation);
        //                db.SaveChanges();

        //                generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_Creation", "Row_ID", mmOmCreation.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
        //            }
        //            else
        //            {
        //                mmOmCreation.Qty -= deleteQty;
        //                mmOmCreation.Release_Qty -= deleteQty;
        //                mmOmCreation.Updated_Date = DateTime.Now;
        //                mmOmCreation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                mmOmCreation.Is_Edited = true;
        //                db.Entry(mmOmCreation).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }



        //            var mmOrderRelease = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId  && m.Plant_OrderNo == id && m.Order_Status.Equals("Release", StringComparison.CurrentCultureIgnoreCase)
        //            && !(db.RS_OM_U321_Tactsheet_Orders.Any(p => p.Order_No == m.Order_No) || db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(p => p.Order_No == m.Order_No) || db.RS_OM_S201_Tactsheet_Orders.Any(p => p.Order_No == m.Order_No))).OrderByDescending(m=>m.RSN).ToArray();
        //            for(int i = 0; i < deleteQty; i++)
        //            {

        //                RS_OM_OrderRelease  orderReleaseObj = db.RS_OM_OrderRelease.Find(mmOrderRelease[i].Row_ID);
        //                orderReleaseObj.Updated_Date = DateTime.Now;
        //                orderReleaseObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                orderReleaseObj.Is_Deleted = true;
        //                db.RS_OM_OrderRelease.Remove(orderReleaseObj);
        //                db.SaveChanges();

        //                generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_OrderRelease", "Row_ID", orderReleaseObj.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
        //            }
        //            globalData.isSuccessMessage = true;
        //            globalData.messageTitle = ResourceModules.OM_Creation;
        //            globalData.messageDetail = ResourceGlobal.Order + " " + ResourceMessages.Delete_Success;
        //            TempData["globalData"] = globalData;
        //        }



        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        globalData.isErrorMessage = true;
        //        globalData.messageTitle = ResourceModules.OM_Creation; ;
        //        globalData.messageDetail = ex.ToString();
        //        TempData["globalData"] = globalData;
        //        return RedirectToAction("Delete", id);

        //    }
        //}

        // POST: Order_Creation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(FormCollection frm, decimal id)
        {
            int fqty = 0;
            var deleteQty = Convert.ToInt32(frm["deleteQty"]);
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int ID = Convert.ToInt32(id);
            RS_OM_Creation mmOmCreation = db.RS_OM_Creation.Where(p => p.Plant_OrderNo == ID).Single();

            try
            {
                ////partial deleting orders according to provided qty
                DateTime date = DateTime.Today;
                var todaydate = date.ToString("yyyy-MM-dd");

                var release = db.RS_OM_Creation.Where(m => m.Plant_ID == plantId && m.Plant_OrderNo == mmOmCreation.Plant_OrderNo);

                RS_OM_Creation creation = db.RS_OM_Creation.Where(m => m.Plant_ID == plantId && m.Plant_OrderNo == mmOmCreation.Plant_OrderNo).FirstOrDefault();

                int qty = Convert.ToInt32(creation.Qty);
                int release_qty = 0;
                if (creation.Release_Qty == null)
                {
                    release_qty = 0;
                }
                else
                {
                    release_qty = Convert.ToInt32(creation.Release_Qty);
                }

                int availableCount = 0;
                availableCount = (qty - release_qty);

                if (availableCount < 0)
                {

                    if (deleteQty > qty)
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = ResourceModules.OM_Creation; ;
                        globalData.messageDetail = "Invalid Delete Quantity";
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Delete", id);
                    }
                    else if (deleteQty < qty)
                    {
                        if (deleteQty > availableCount)
                        {
                            globalData.isErrorMessage = true;
                            globalData.messageTitle = ResourceModules.OM_Creation; ;
                            globalData.messageDetail = "Invalid Delete Quantity";
                            TempData["globalData"] = globalData;
                            return RedirectToAction("Delete", id);
                        }
                    }
                    else if (deleteQty == qty)
                    {
                        mmOmCreation.Updated_Date = DateTime.Now;
                        mmOmCreation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmOmCreation.Is_Deleted = true;
                        db.RS_OM_Creation.Remove(mmOmCreation);
                        db.SaveChanges();

                        generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_Creation", "Row_ID", mmOmCreation.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                    }
                }
                else if (deleteQty < qty)
                {
                    if (deleteQty > availableCount)
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = ResourceModules.OM_Creation; ;
                        globalData.messageDetail = "Invalid Delete Quantity";
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Delete", id);
                    }
                    else
                    {
                       
                        mmOmCreation.Qty -= deleteQty;
                        mmOmCreation.Updated_Date = DateTime.Now;
                        mmOmCreation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmOmCreation.Is_Edited = true;
                        db.Entry(mmOmCreation).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (deleteQty > qty)
                {
                    if (deleteQty == qty)
                    {
                        mmOmCreation.Updated_Date = DateTime.Now;
                        mmOmCreation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmOmCreation.Is_Deleted = true;
                        db.RS_OM_Creation.Remove(mmOmCreation);
                        db.SaveChanges();

                        generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_Creation", "Row_ID", mmOmCreation.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                    }
                    else
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = ResourceModules.OM_Creation; ;
                        globalData.messageDetail = "Invalid Delete Quantity";
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Delete", id);
                    }
                }
                else
                {
                    if (availableCount == deleteQty)
                    {
                        mmOmCreation.Updated_Date = DateTime.Now;
                        mmOmCreation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmOmCreation.Is_Deleted = true;
                        db.RS_OM_Creation.Remove(mmOmCreation);
                        db.SaveChanges();

                        generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_Creation", "Row_ID", mmOmCreation.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                    }
                    else if(deleteQty > availableCount)
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = ResourceModules.OM_Creation; ;
                        globalData.messageDetail = "Invalid Delete Quantity";
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Delete", id);
                    }
                    else
                    {
                        fqty = (deleteQty + release_qty);
                        mmOmCreation.Qty -= fqty;
                        // mmOmCreation.Release_Qty -= deleteQty;
                        mmOmCreation.Updated_Date = DateTime.Now;
                        mmOmCreation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmOmCreation.Is_Edited = true;
                        db.Entry(mmOmCreation).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.OM_Creation;
                    globalData.messageDetail = ResourceGlobal.Order + " " + ResourceMessages.Delete_Success;
                    TempData["globalData"] = globalData;
                }



                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.OM_Creation; ;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", id);

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

        public ActionResult getPlannedVsActual()
        {

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.OM_Creation;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Order Creation";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.OM_Creation;
            globalData.contentFooter = ResourceModules.OM_Creation;

            ViewBag.GlobalDataModel = globalData;

            DateTime time = DateTime.Now.Date;
            int shopId = 1;//((FDSession)this.Session["FDSession"]).shopId;
            String SQLQuery = "select count(*) as Planned_Qty,sum(CASE WHEN RS_OM_OrderRelease.Order_Status = 'Started' THEN 1 ELSE 0 END) as Actual_Qty,sum(CASE WHEN RS_OM_OrderRelease.Order_Status = 'Hold' THEN 1 ELSE 0 END) as Hold_Qty ";
            SQLQuery += "from RS_OM_OrderRelease ";
            SQLQuery += "where CONVERT(DATE,RS_OM_OrderRelease.Inserted_Date)= CONVERT(DATE,'" + time + "') and Shop_ID=" + shopId;

            var objectContext = ((IObjectContextAdapter)db).ObjectContext;
            List<object> listobj = new List<object>();
            List<PlannedVsActualModel> data = objectContext.ExecuteStoreQuery<PlannedVsActualModel>(SQLQuery).AsQueryable().ToList();

            ViewBag.PlannedData = data;
            return PartialView("getPlannedVsActual");
            // return View(st);
        }


        public ActionResult ExcelUpload()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");

            if (TempData["OrderUploadRecords"] != null)
            {
                ViewBag.OrderUploadRecords = TempData["OrderUploadRecords"];
            }

            globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
            globalData.subTitle = ResourceGlobal.Upload;
            globalData.controllerName = "OrderCreation";
            globalData.actionName = ResourceGlobal.Upload;
            globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelUpload(ExcelOrderUpload formData)
        {
            int plantId = 0, shopId = 0;
            String createdOrders = "";
            if (ModelState.IsValid)
            {
                GlobalOperations globalOperations = new GlobalOperations();
                string fileName = Path.GetFileName(formData.Excel_File.FileName);
                string fileExtension = Path.GetExtension(formData.Excel_File.FileName);
                string fileLocation = Server.MapPath("~/App_Data/" + fileName);
                DataTable dt = ExcelToDataTable(formData.Excel_File, fileLocation, fileExtension);
                //String attributeId = formData.Attribute_ID;

                plantId = Convert.ToInt32(formData.Plant_ID);

                shopId = Convert.ToInt32(formData.Shop_ID);
                if (dt.Rows.Count > 0)
                {
                    OrderUploadRecords[] orderUploadRecordsObj = new OrderUploadRecords[dt.Rows.Count];
                    RS_OM_Creation mmOrderCreationObj = new RS_OM_Creation();
                    int i = 0;
                    foreach (DataRow checkListRow in dt.Rows)
                    {
                        String modelCode = checkListRow[0].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(modelCode))
                        {
                            //String series = checkListRow[1].ToString().Trim();
                            string ColorName = checkListRow[1].ToString().Trim();
                            int qty = 0;
                            orderUploadRecordsObj[i] = new OrderUploadRecords();
                            OrderUploadRecords orderUploadObj = new OrderUploadRecords();
                            orderUploadObj.modelCode = modelCode;

                            //added by Ajay regarding Changes for Country
                            int? Country_ID = null;
                            string Country_Name = checkListRow[5].ToString().Trim();
                            if(!string.IsNullOrWhiteSpace(Country_Name))
                            {
                                var CountryID = db.RS_Country.Where(m => m.Country_Name == Country_Name).Select(m => m.Country_ID).FirstOrDefault();
                                if(CountryID != 0)
                                {
                                    var ModelID = db.RS_Model_Master.Where(m => m.Model_Code == modelCode).Select(m => m.Model_ID).FirstOrDefault();
                                    var IsExistagainstModelCode = db.RS_ModelCode_Country_Mapping.Where(m => m.Country_ID == CountryID && m.Model_ID == ModelID).Select(m => m.Country_ID).FirstOrDefault();
                                    if(IsExistagainstModelCode == 0)
                                    {
                                        orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Country is not exist against Model Code" + Country_Name;
                                    }
                                    else
                                    {
                                        Country_ID = CountryID;
                                    }
                                }
                                else
                                {
                                    orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Country is not exist. Check Country Configuration" + Country_Name;
                                }
                            }
                            else
                            {

                            }
                            //added by ketan Date 29/08/2017 
                            // regarding changes in color
                            orderUploadObj.Country = Country_Name;
                            bool IsColorApplicable = Convert.ToBoolean(db.RS_Model_Master.Where(m => m.Model_Code == modelCode).Select(m => m.Is_Colour_Applicable).FirstOrDefault());
                            bool IsColorCheck = Convert.ToBoolean(db.RS_Model_Master.Where(m => m.Model_Code == modelCode).Select(m => m.Color_Code).FirstOrDefault());
                            if(IsColorApplicable)
                            {
                                if (IsColorCheck)
                                {
                                    var color_code = modelCode.Substring(modelCode.Length - 2, 2);
                                    var color_Name = db.RS_Colour.Where(m => m.Colour_ID == color_code).Select(c => c.Colour_Desc).FirstOrDefault();
                                    if (color_Name == null)
                                    {
                                        orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Color code is not configured for this Model";
                                    }
                                    else
                                    {
                                        orderUploadObj.ColorCode = color_code;
                                    }
                                }
                                else
                                {
                                    var color_ID = db.RS_Colour.Where(m => m.Colour_Desc == ColorName).Select(c => c.Colour_ID).FirstOrDefault();
                                    if (color_ID == null)
                                    {
                                        orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Color code is not exist against color Name " + ColorName;
                                    }
                                    else
                                    {
                                        orderUploadObj.ColorCode = color_ID;
                                    }

                                }
                            }
                            else
                            {
                                orderUploadObj.ColorCode = null;
                            }

                            //orderUploadObj.ColorCode = ColorName;
                            orderUploadObj.qty = checkListRow[2].ToString();
                            //Added by sandip
                            string date = checkListRow[3].ToString();
                            DateTime pDate = DateTime.ParseExact(date, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                            string ConvertedPlannedDate = pDate.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
                            orderUploadObj.Planned_Date = Convert.ToDateTime(ConvertedPlannedDate);
                            try
                            {
                                qty = Convert.ToInt16(checkListRow[2].ToString().Trim());
                            }
                            catch (Exception ex1)
                            {
                                orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Order quantity can not be null or empty.";
                            }

                            if (qty == 0)
                            {
                                orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Order quantity can not be null or empty.";
                            }
                            else
                            {
                                // process to check the model code is empty or not
                                if (String.IsNullOrEmpty(modelCode))
                                {
                                    orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Model code can not be null or empty";
                                }
                                else
                                {
                                    // process to check the model is correct for the selected shop
                                    if (!mmOrderCreationObj.isModelCodeAvailableInShop(shopId, modelCode))
                                    {
                                        orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Model code is not configured in this shop";
                                    }
                                }

                                //// process to check the series is null or empty
                                //if (String.IsNullOrEmpty(series))
                                //{

                                //    orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Series can not be null or empty";
                                //}
                                //else
                                //{


                                //    // process to check the series is of correct model
                                //    //if (!mmOrderCreationObj.isSeriesAddedForModel(shopId, modelCode, series))
                                //    ////if (!mmOrderCreationObj.isSeriesAddedForShop(shopId, series))
                                //    //{
                                //    //    orderUploadObj.orderCreationError = "Order not created." + System.Environment.NewLine + "Series is not correct as per the model code in this shop";
                                //    //}

                                //    //modelCode = mmOrderCreationObj.getModelCode(series);
                                //    //if (String.IsNullOrEmpty(modelCode))
                                //    //{
                                //    //    orderUploadObj.orderCreationError = "Model code not found for this series";
                                //    //    orderUploadObj.modelCode = "";
                                //    //}
                                //    //else
                                //    //{
                                //    //    orderUploadObj.modelCode = modelCode;
                                //    //}
                                //}

                                if (String.IsNullOrEmpty(orderUploadObj.orderCreationError))
                                {
                                    // RS_Series[] mmSeriesObj = db.RS_Series.Where(p => p.Series_Description == series).ToArray();

                                    decimal series = 0;
                                    RS_OM_OrderRelease mmOrderReleaseObj = new RS_OM_OrderRelease();
                                    String res = mmOrderReleaseObj.isOrderValidToCreate(modelCode, series);
                                    if (!String.IsNullOrEmpty(res))
                                    {
                                        orderUploadObj.orderCreationError = res;
                                    }
                                }

                                // process to add the record in database is there is not error
                                if (String.IsNullOrEmpty(orderUploadObj.orderCreationError))
                                {
                                    mmOrderCreationObj = new RS_OM_Creation();
                                    mmOrderCreationObj.Plant_ID = plantId;
                                    mmOrderCreationObj.Shop_ID = shopId;
                                    var platformIDNo = db.RS_Model_Master.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Model_Code == modelCode).Select(p => p.Platform_Id).FirstOrDefault();
                                    mmOrderCreationObj.Platform_Id = platformIDNo;
                                    var Line_ID = db.RS_OM_Platform.Where(m => m.Platform_ID == platformIDNo).Select(m => m.Line_ID).FirstOrDefault();
                                    mmOrderCreationObj.Line_ID = Convert.ToDecimal(Line_ID);
                                        //db.RS_Model_Master.Where(m=>m.Plant_ID==plantId && m.Shop_ID == shopId && m.Model_Code == modelCode).Select(p=>p.Platform_Id).FirstOrDefault();
                                    mmOrderCreationObj.Plant_OrderNo = mmOrderCreationObj.GetLastOrderNumber(plantId, shopId);
                                    mmOrderCreationObj.Model_Code = modelCode;
                                    mmOrderCreationObj.Qty = qty;
                                    mmOrderCreationObj.Planned_Date = Convert.ToDateTime(ConvertedPlannedDate);
                                    //RS_Series[] mmSeriesObj = db.RS_Series.Where(p => p.Series_Description == series).ToArray();
                                    //mmOrderCreationObj.Series_code = mmSeriesObj[0].Series_Code;
                                    mmOrderCreationObj.Country_ID = Country_ID;
                                    mmOrderCreationObj.Inserted_Date = DateTime.Now;
                                    mmOrderCreationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    mmOrderCreationObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    bool isMain = db.RS_Shops.Where(sh => sh.Shop_ID == shopId).Select(sh => sh.Is_Main).FirstOrDefault();
                                    if (isMain)
                                    {
                                        mmOrderCreationObj.Order_Type = "P";

                                    }
                                    else
                                    {
                                        mmOrderCreationObj.Order_Type = "Spare";
                                    }

                                    mmOrderCreationObj.Updated_Date = DateTime.Now;
                                    //mmOrderCreationObj.Colour_ID = "GR";
                                    //added by ketan Date 29/08/2017
                                    mmOrderCreationObj.Colour_ID = orderUploadObj.ColorCode;
                                    //added as per changes in AD parent and spare order.
                                    string OrderType = checkListRow["Order_type"].ToString().Trim();
                                    mmOrderCreationObj.Order_Type = OrderType;
                                    //if (OrderType == "TearDown" || OrderType == "Trial" || OrderType == "Regular production")
                                    //{
                                    //    mmOrderCreationObj.Order_Type = OrderType;
                                    //    //mmOrderCreationObj.Child_Order_Type = Child_Order_Type;
                                    //}



                                    db.RS_OM_Creation.Add(mmOrderCreationObj);
                                    db.SaveChanges();

                                    //for creating orders in resequencing
                                    //for only creating dummy orders
                                    ////////var res = CreateOrders(mmOrderCreationObj.Row_ID, "", mmOrderCreationObj.Qty, true);
                                    //return RedirectToAction("CreateOrders", "OMOrderRelease", new { rowId = mmOrderCreationObj.Row_ID, remark = "", quantity = mmOrderCreationObj.Qty, isCreationRequest = true });
                                    //
                                    if (String.IsNullOrEmpty(createdOrders))
                                    {
                                        createdOrders = mmOrderCreationObj.Row_ID.ToString();
                                    }
                                    else
                                    {
                                        createdOrders = createdOrders + "," + mmOrderCreationObj.Row_ID.ToString();
                                    }

                                    orderUploadObj.orderCreationError = "Order is created successfully";
                                    orderUploadObj.isCreated = true;
                                    orderUploadObj.rowId = mmOrderCreationObj.Row_ID.ToString();
                                }

                            }

                            orderUploadRecordsObj[i] = orderUploadObj;
                            i = i + 1;
                        }





                    }


                    TempData["OrderUploadRecords"] = orderUploadRecordsObj;
                    //TempData["ChecklistDataTable"] = dt;
                    ViewBag.OrderUploadRecords = orderUploadRecordsObj;
                    //ViewBag.dt = qualityChecklistDt;
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.OM_Creation;
                    globalData.messageDetail = ResourceGlobal.Order + " " + ResourceMessages.Create_Success;
                    globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
                    globalData.subTitle = ResourceGlobal.Upload;
                    globalData.controllerName = "OrderCreation";
                    globalData.actionName = ResourceGlobal.Upload;
                    globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.OM_Creation + " " + ResourceGlobal.Form;
                    ViewBag.GlobalDataModel = globalData;

                    ViewBag.createdOrders = createdOrders;
                }
            }
            //return PartialView("QualityChecklistDetails");

            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", shopId);
            return View();
        }

        public ActionResult GetSeriesCodeByModelCode(String modelCode)
        {
            try
            {
                var res = from modelObj in db.RS_Model_Master

                          where modelObj.Model_Code == modelCode
                          select new
                          {
                              Series_Code = modelObj.Series_Code
                          };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /*
     * Function Name        : ExcelToDataTable
     * Input Parameter      : Uploaded File,saved file location,extension of file
     * Return Type          : DataTable
     * Author & Time Stamp  : Jitendra Mahajan
     * Description          : Convert Excel file data to DataTable Format
     */

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
                    //connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", fileLocation);
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
        public ActionResult GetLineName(int Shop_Id)
        {
            //var Line = db.RS_Lines
            //                              .Where(c => c.Shop_ID == Shop_Id)
            //                              .Select(c => new { c.Line_ID, c.Line_Name });
            var Line= (from line in db.RS_Lines
             join partgroup in db.RS_Partgroup on line.Line_ID equals partgroup.Line_ID
             where line.Shop_ID == Shop_Id && partgroup.Order_Create == true
             select new
             {
                 line.Line_Name,
                 line.Line_ID
             }).Distinct().ToList();
            return Json(Line, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderType(int Shop_Id)
        {
            var Line = (from type in db.RS_OM_Order_Type
                        where type.Shop_ID == Shop_Id
                        select new
                        {
                           type.Order_Type_ID,
                           type.Order_Type_Name
                        }).Distinct().ToList();
            return Json(Line, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult GetColorCodeByModelCode(string modelCode)
        {
            //try
            //{
            plantid = ((FDSession)this.Session["FDSession"]).plantId;
            var obj_MM_ModelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode && m.Plant_ID == plantid).FirstOrDefault();
            //var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            //var userInfoJs'on = jss.Serialize(obj_MM_ModelMaster.Attribution_Parameters);

            //var objJSONData = JsonConvert.D{ eserializeObject(obj_MM_ModelMaster.Attribution_Parameters);
            if(obj_MM_ModelMaster.Is_Colour_Applicable == true)
            {
                if (obj_MM_ModelMaster.Color_Code == true)
                {
                    var model_code = obj_MM_ModelMaster.Model_Code;
                    var color_code = model_code.Substring(modelCode.Length - 2, 2);
                    var color_Name = db.RS_Colour.Where(m => m.Colour_ID == color_code).Select(c => new { c.Colour_ID, c.Colour_Desc });
                    return Json(color_Name, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var color_Name = db.RS_Colour.Select(c => new { c.Colour_ID, c.Colour_Desc });
                    return Json(color_Name, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            

        }

        public ActionResult GetCountryByModelCode(string modelCode)
        {
            bool result = false;

            if (modelCode != null)
            {
                var SubID = db.RS_Model_Master.Where(m => m.Model_Code == modelCode).Select(m => m.Sub_Assembly_ID).FirstOrDefault();
                if (SubID > 0)
                {
                    if (SubID == 1)
                        result = true;
                    else
                        result = false;
                }
                else if (SubID == null)
                {
                    result = false;
                }
            }
            //result = db.RS_Model_Master
            var ModelID = db.RS_Model_Master.Where(m => m.Model_Code == modelCode).Select(m => m.Model_ID).FirstOrDefault();
            var Data = db.RS_ModelCode_Country_Mapping
                    .Where(c => c.Model_ID == ModelID)
                    .Select(c => new { Id = c.Country_ID, Value = c.RS_Country.Country_Name });

            return Json(new { result,Data}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModelCodeValidForStyleCode(string modelCode)
        {
            bool result = false;
            if (modelCode != null)
            {
                var SubID = db.RS_Model_Master.Where(m => m.Model_Code == modelCode).Select(m => m.Sub_Assembly_ID).FirstOrDefault();
                if (SubID > 0)
                {
                    RS_Major_Sub_Assembly obj = db.RS_Major_Sub_Assembly.Find(SubID);
                    if (SubID == 1)
                        result = (db.RS_Model_Master.Any(sname => sname.Model_Code == modelCode && sname.Style_Code != null));
                    else
                        result = (db.RS_Model_Master.Any(sname => sname.Model_Code == modelCode));
                }
                else if(SubID == null)
                {
                    result = (db.RS_Model_Master.Any(sname => sname.Model_Code == modelCode));
                }
            }
            //result = db.RS_Model_Master

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetModelCodeByLineAndPlatform(int PlatformID,string OrderType)
        {
            plantid = ((FDSession)this.Session["FDSession"]).plantId;

            var model = db.RS_Model_Master
                                          .Where(c => c.Platform_Id == PlatformID && c.Plant_ID == plantid && c.Sub_Assembly_ID == null)
                                          .Select(c => new { Id = c.Model_Code, Value = c.Model_Code });
            
                model = db.RS_Model_Master
                    .Where(c=> c.Platform_Id == PlatformID && c.Plant_ID == plantid && c.Sub_Assembly_ID > 0)
                    .Select(c => new { Id = c.Model_Code, Value = c.Model_Code });
                        
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ModelCodeValidForOrderType(string modelCode)
        {
            bool result = false;
            if (modelCode != null)
            {
                result = (db.RS_Model_Master.Any(sname => sname.Model_Code == modelCode && sname.Is_Spare == true));
            }
            //result = db.RS_Model_Master

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private class JsonData
        {
            public Boolean result { get; set; }
            public string message { get; set; }
            public int validDeltionQty { get; set; }
        }
        //public JsonResult validateDetetionQty(int deleteQty, int plantOrderId)
        //{

        //    JsonData jsondataObj = new JsonData();
        //    DateTime date = DateTime.Today;
        //    jsondataObj.validDeltionQty = 0;
        //    var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            
        //    try
        //    {
        //        var OrderCreation_Row_ID = Convert.ToInt32(plantOrderId);
        //        var todaydate = date.ToString("yyyy-MM-dd");
        //        var releaseCount = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Plant_OrderNo == plantOrderId && m.Order_Status.Equals("Release", StringComparison.CurrentCultureIgnoreCase)
        //        && !(db.RS_OM_U321_Tactsheet_Orders.Any(p => p.Order_No == m.Order_No) || db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(p => p.Order_No == m.Order_No) || db.RS_OM_S201_Tactsheet_Orders.Any(p => p.Order_No == m.Order_No))).Count();
        //        jsondataObj.validDeltionQty = releaseCount;
        //        if (deleteQty > releaseCount)
        //        {
        //            jsondataObj.message = "Invalid Delete Quantity, Some orders are on Hold or Started";
        //            jsondataObj.result = false;
        //        }
        //        else
        //        {
        //            jsondataObj.message = "Valid Delete Quantity";
        //            jsondataObj.result = true;
        //        }

        //    }
        //    catch(Exception ex){
        //        jsondataObj.message = "Please enter number only";
        //        jsondataObj.result = false;
        //        return Json(jsondataObj, JsonRequestBehavior.AllowGet);
        //    }


        //    return Json(jsondataObj, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult validateDetetionQty(int deleteQty, int plantOrderId)
        {

            JsonData jsondataObj = new JsonData();
            DateTime date = DateTime.Today;
            jsondataObj.validDeltionQty = 0;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;

            try
            {
                var OrderCreation_Row_ID = Convert.ToInt32(plantOrderId);
                var todaydate = date.ToString("yyyy-MM-dd");

                var release = db.RS_OM_Creation.Where(m => m.Plant_ID == plantId && m.Plant_OrderNo == plantOrderId);

                RS_OM_Creation creation = db.RS_OM_Creation.Where(m => m.Plant_ID == plantId && m.Plant_OrderNo == plantOrderId).FirstOrDefault();

                int qty = Convert.ToInt32(creation.Qty);
                int release_qty = 0;
                if (creation.Release_Qty == null)
                {
                    release_qty = 0;
                }
                else
                {
                    release_qty = Convert.ToInt32(creation.Release_Qty);
                }

                int releaseCount = 0;
                releaseCount = (qty - release_qty);
                jsondataObj.validDeltionQty = releaseCount;

                if (releaseCount < 0)
                {

                    if (deleteQty > qty)
                    {
                        jsondataObj.message = "Invalid Delete Quantity";
                        jsondataObj.result = false;
                    }
                }
                else
                {
                    if (deleteQty > releaseCount)
                    {
                        jsondataObj.message = "Invalid Delete Quantity";
                        jsondataObj.result = false;
                    }
                }



            }
            catch (Exception ex)
            {
                jsondataObj.message = "Please enter number only";
                jsondataObj.result = false;
                return Json(jsondataObj, JsonRequestBehavior.AllowGet);
            }


            return Json(jsondataObj, JsonRequestBehavior.AllowGet);
        }

        public bool CreateOrders(int rowId, String remark, int quantity, bool isCreationRequest = false)
        {
            DateTime nowTime = DateTime.Now;
            int plantId, shopId = 0;
            int? lineId = null;
            try
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;

                string part_number;
                decimal? series_code;
                int Line_Code;
                string sub_partno, sub_series;
                RS_Model_Master mmModelMasterObj;
                RS_OM_Creation mmOmCreationObj;

                RS_OM_Configuration[] omConfiguration;
                RS_Partgroup partGroupObj;
                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                mmModelMasterObj = (from modelMaster in db.RS_Model_Master
                                    where (from orderCreate in db.RS_OM_Creation where orderCreate.Row_ID == rowId && orderCreate.Plant_ID == plantId select orderCreate.Model_Code).Contains(modelMaster.Model_Code) && modelMaster.Plant_ID == plantId
                                    select modelMaster).FirstOrDefault();


                mmOmCreationObj = (from orderCreate in db.RS_OM_Creation
                                   where orderCreate.Row_ID == rowId && orderCreate.Plant_ID == plantId
                                   select orderCreate).FirstOrDefault();

                //Enter quantity check 
                int totqty;
                totqty = (Convert.ToInt32(mmOmCreationObj.Qty) - Convert.ToInt32(mmOmCreationObj.Release_Qty));

                if (quantity > totqty)
                {
                    ModelState.AddModelError("Quantity", ResourceValidation.Qty_Is_Greater);
                }
                else
                {
                    String configId = mmModelMasterObj.OMconfig_ID;
                    decimal? partGroupId = null;
                    shopId = 0;

                    omConfiguration = (from configuration in db.RS_OM_Configuration
                                       join partgroup_id in db.RS_Partgroup on configuration.Partgroup_ID equals partgroup_id.Partgroup_ID
                                       where configuration.OMconfig_ID == configId && partgroup_id.Order_Create == true && configuration.Plant_ID == plantId
                                       select configuration).ToArray();


                    int totalOrder = 0;
                    RS_OM_OrderRelease omOrderRelease = new RS_OM_OrderRelease();
                    omOrderRelease.Plant_ID = mmOmCreationObj.Plant_ID;
                    int Plant_ID = Convert.ToInt32(omOrderRelease.Plant_ID);
                    omOrderRelease.Model_Code = mmModelMasterObj.Model_Code;
                    string model_code = omOrderRelease.Model_Code;
                    omOrderRelease.Order_Type = mmOmCreationObj.Order_Type.Trim();
                    omOrderRelease.Order_Status = "Release";
                    omOrderRelease.Remarks = remark;
                    omOrderRelease.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    omOrderRelease.Inserted_Date = DateTime.Now;
                    omOrderRelease.Plant_OrderNo = mmOmCreationObj.Plant_OrderNo;
                    omOrderRelease.Planned_Date = mmOmCreationObj.Planned_Date;
                    if (omOrderRelease.isBOMavailableOrNot(model_code) == true)
                    {
                        if (omConfiguration != null)
                        {
                            bool ok = true;
                            int groupId = omOrderRelease.getGroupNo();
                            for (int j = 0; j < quantity; j++)
                            {

                                String uToken = new Random().Next(10000, 99999).ToString() + DateTime.Now.Ticks;
                                //RSN Number
                                partGroupId = db.RS_PartgroupItem.Where(a => a.Part_No == model_code && a.Plant_ID == Plant_ID).FirstOrDefault().Partgroup_ID;
                                omOrderRelease.partno = model_code;
                                omOrderRelease.Series_Code = mmModelMasterObj.Series_Code;

                                // partGroupObj = db.RS_Partgroup.Find(partGroupId);
                                partGroupObj = db.RS_Partgroup.Where(p => p.Plant_ID == Plant_ID && p.Partgroup_ID == partGroupId).Select(p => p).FirstOrDefault();
                                shopId = Convert.ToInt32(partGroupObj.Shop_ID);
                                omOrderRelease.Shop_ID = shopId;

                                DateTime Inserted_Date = omOrderRelease.Inserted_Date;
                                omOrderRelease.ORN = Convert.ToInt32(omOrderRelease.generateORNNumber(Plant_ID, Inserted_Date));
                                omOrderRelease.RSN = Convert.ToInt32(omOrderRelease.generateRSNNumber(Plant_ID, shopId, mmOmCreationObj.Planned_Date));
                                omOrderRelease.CUMN = omOrderRelease.ORN;

                                series_code = mmModelMasterObj.Series_Code;
                                lineId = Convert.ToInt32(partGroupObj.Line_ID);
                                //Commented by ketan 19-08-2017
                                //Line_Code = Convert.ToInt32(partGroupObj.Line_ID);

                                decimal PlantOrderNo = db.RS_OM_Creation.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Model_Code == model_code).Max(q => (decimal?)(q.Plant_OrderNo)) ?? 0;
                                decimal Line_ID = db.RS_OM_Creation.Where(m => m.Plant_OrderNo == PlantOrderNo).Select(m => m.Line_ID).FirstOrDefault();
                                omOrderRelease.Line_ID = Line_ID;

                                totalOrder = omOrderRelease.getTotalOrderReleasedByDate(shopId, Convert.ToInt16(Line_ID));
                                omOrderRelease.Order_No = omOrderRelease.generateOrderNumber(totalOrder + 1, model_code);

                                omOrderRelease.Inserted_Date = DateTime.Now;
                                omOrderRelease.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                omOrderRelease.Updated_Date = DateTime.Now;
                                omOrderRelease.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                //omOrderRelease.Model_Color = "GR"
                                //added by ketan Date 19-08-17
                                bool IsColorCheck = Convert.ToBoolean(db.RS_Model_Master.Where(m => m.Model_Code == model_code).Select(m => m.Color_Code).FirstOrDefault());
                                if (IsColorCheck)
                                {
                                    var color_code = model_code.Substring(model_code.Length - 2, 2);
                                    omOrderRelease.Model_Color = color_code;
                                    //var color_Name = db.MM_Color.Where(m => m.COLOUR_ID == color_code).Select(c => c.COLOUR_DESC );
                                }
                                else
                                {
                                    omOrderRelease.Model_Color = mmOmCreationObj.Colour_ID;
                                }
                                omOrderRelease.Country_ID = 1;
                                omOrderRelease.Order_Start = false;
                                omOrderRelease.Is_Active = true;
                                omOrderRelease.Is_Deleted = false;
                                omOrderRelease.UToken = uToken;
                                db.RS_OM_OrderRelease.Add(omOrderRelease);
                                db.SaveChanges();


                                RS_OM_OrderRelease orderReleaseObj = new RS_OM_OrderRelease();
                                //orderReleaseObj.addRecordToPlannedOrders(omOrderRelease, groupId);

                                String modelCode = omOrderRelease.Model_Code;
                                decimal? seriesCode = omOrderRelease.Series_Code;
                                //mmOmPlannedOrdersObj = new RS_OM_Planned_Orders();
                                //Child Order Release
                                for (int i = 0; i < omConfiguration.Count(); i++)
                                {
                                    partGroupId = omConfiguration[i].Partgroup_ID;

                                    partGroupObj = db.RS_Partgroup.Where(p => p.Partgroup_ID == partGroupId).SingleOrDefault();
                                    shopId = Convert.ToInt32(partGroupObj.Shop_ID);
                                    omOrderRelease.Shop_ID = shopId;

                                    RS_PartgroupItem partgroupItemObj = omOrderRelease.getPartGroupItemByPartGroupAndModelCode(partGroupId, model_code);
                                    if (partgroupItemObj == null)
                                    {

                                    }
                                    else
                                    {
                                        omOrderRelease.partno = partgroupItemObj.Part_No;
                                        RS_Partmaster mmPartMasterObj = omOrderRelease.getPartmasterByPartNumber(partgroupItemObj.Part_No);

                                        if (partgroupItemObj.Series_Code == null || partgroupItemObj.Series_Code == null)
                                        {

                                            omOrderRelease.Series_Code = mmModelMasterObj.Series_Code;
                                        }
                                        else
                                        {
                                            omOrderRelease.Series_Code = partgroupItemObj.Series_Code;
                                        }

                                        //series_code = mmModelMasterObj.Series_Code;
                                        Line_Code = Convert.ToInt32(partGroupObj.Line_ID);
                                        omOrderRelease.Line_ID = Line_Code;

                                        totalOrder = omOrderRelease.getTotalOrderReleasedByDate(shopId, Line_Code);
                                        omOrderRelease.Order_No = omOrderRelease.generateOrderNumber(totalOrder + 1, model_code);

                                        omOrderRelease.RSN = Convert.ToInt32(omOrderRelease.generateRSNNumber(Plant_ID, shopId, mmOmCreationObj.Planned_Date));
                                        omOrderRelease.Inserted_Date = DateTime.Now;
                                        omOrderRelease.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        omOrderRelease.Updated_Date = DateTime.Now;
                                        omOrderRelease.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        //omOrderRelease.Model_Color = "GR";
                                        IsColorCheck = Convert.ToBoolean(db.RS_Model_Master.Where(m => m.Model_Code == model_code).Select(m => m.Color_Code).FirstOrDefault());
                                        if (IsColorCheck)
                                        {
                                            var color_code = model_code.Substring(model_code.Length - 2, 2);
                                            omOrderRelease.Model_Color = color_code;
                                            //var color_Name = db.MM_Color.Where(m => m.COLOUR_ID == color_code).Select(c => c.COLOUR_DESC );
                                        }
                                        else
                                        {
                                            omOrderRelease.Model_Color = mmOmCreationObj.Colour_ID;
                                        }
                                        omOrderRelease.Country_ID = 1;
                                        omOrderRelease.Order_Start = false;

                                        omOrderRelease.Is_Active = true;
                                        omOrderRelease.Is_Deleted = false;
                                        omOrderRelease.UToken = uToken;
                                        db.RS_OM_OrderRelease.Add(omOrderRelease);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            mmOmCreationObj = (from orderCreation in db.RS_OM_Creation
                                               where orderCreation.Row_ID == rowId
                                               select orderCreation).SingleOrDefault();

                            int quantityRelease = Convert.ToInt32(mmOmCreationObj.Release_Qty);
                            mmOmCreationObj.Release_Qty = quantityRelease + quantity;

                            db.Entry(mmOmCreationObj).State = EntityState.Modified;
                            db.SaveChanges();

                        }

                    }
                    else
                    {
                        return false;/*Json(false, JsonRequestBehavior.AllowGet);*/
                    }
                }

                //if (isCreationRequest)
                //{
                //    // process to redirect to order creation
                //    globalData.isSuccessMessage = true;
                //    globalData.messageTitle = ResourceModules.OM_Creation;
                //    globalData.messageDetail = ResourceModules.OM_Creation + " " + ResourceMessages.Add_Success;
                //    TempData["globalData"] = globalData;
                //    return RedirectToAction("Create", "OrderCreation");
                //}
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                    }
                }
                throw raise;
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "PPC Module:Order Creation", "Create Order::Post", Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
            }
            finally
            {
                generalHelper.logUserActivity(shopId, lineId, "PPC Module", "Order Release", nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
            }
            return true;/*Json(true, JsonRequestBehavior.AllowGet);*/
        }
        //catch (Exception ex)
        //{


        //}

        //return Json(color_Name, JsonRequestBehavior.AllowGet);
        //}


    }
}
