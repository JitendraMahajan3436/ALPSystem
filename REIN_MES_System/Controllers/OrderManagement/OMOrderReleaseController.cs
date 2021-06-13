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
using System.IO;
using System.Data.OleDb;
using System.Globalization;
using System.Data.Entity.Infrastructure;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Helpers;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /*               Controller Name           : OMOrderReleaseController
     *               Description               : Controller used to releasing the order . 
     *               Author, Timestamp         : Jitendra Mahajan
     */
    public class OMOrderReleaseController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        General generalObj = new General();

        /*               Action Name               : Order_Creation
         *               Description               : Action used to find the Model Code of Order Release
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Model code
        public ActionResult Order_Creation()
        {
            var st = from order_create in db.RS_OM_Creation
                     select order_create;
            return Json(st, JsonRequestBehavior.AllowGet);

        }


        /*               Action Name               : ShowCreatedOrders
         *               Description               : Action used to show the Created Order for Order Release
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Plant_ID,Shop_ID
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowCreatedOrders(int Plant_ID, int Shop_ID)
        {
            DateTime time = DateTime.Now.Date;             // Use current time.
            string format = "MM-dd-yy";   // Use this format.
            string dtObj = time.ToString(format);

            // String query = "select RS_OM_Creation.* from RS_OM_Creation where Plant_ID =" + Plant_ID + " and Shop_ID = " + Shop_ID + " and (CONVERT(VARCHAR(10),Inserted_Date,10)='" + dtObj + "' OR CONVERT(VARCHAR(10),Updated_Date,10)='" + dtObj + "')";
            // String query = "select c.Row_ID,c.Plant_ID,c.Shop_ID,c.Plant_OrderNo,c.Model_Code,c.Colour_ID,c.Order_Type,c.qty,s.Series_Description from RS_OM_Creation as c,RS_Series s where c.Series_code=s.Series_Code and c.Plant_ID =" + Plant_ID + " and c.Shop_ID = " + Shop_ID + " and (CONVERT(VARCHAR(10),c.Inserted_Date,10)='" + dtObj + "' OR CONVERT(VARCHAR(10),c.Updated_Date,10)='" + dtObj + "')"; 
            // IEnumerable<RS_OM_Creation> data = db.Database.SqlQuery<RS_OM_Creation>(query);

            var data = db.RS_OM_Creation.AsEnumerable()
                .Where(a => a.Plant_ID == Plant_ID && a.Shop_ID == Shop_ID && (a.Inserted_Date.Value.ToString("dd.MM.yy") == time.ToString("dd.MM.yy") || a.Updated_Date.Value.ToString("dd.MM.yy") == time.ToString("dd.MM.yy")))
                .ToList();

            return PartialView(data);

        }

        /*               Action Name               : ShowOrderReadyToRelease
         *               Description               : Action used to show Order which is Ready for Order Release
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : hdnRowId,hdnQuantity
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowOrderReadyToRelease(int hdnRowId, int hdnQuantity)
        {
            DateTime time = DateTime.Now;             // Use current time.
            string format = "MM-dd-yy";   // Use this format.
            string dtObj = time.ToString(format);

            String query = "select RS_OM_Creation.* from RS_OM_Creation where Row_Id=" + hdnRowId;
            //String query = "select c.Row_ID,c.Plant_ID,c.Shop_ID,c.Plant_OrderNo,c.Model_Code,c.Order_Type,c.qty,s.Series_Description from RS_OM_Creation as c,RS_Series s where c.Series_code=s.Series_Code and c.Row_Id=" + hdnRowId; 

            IEnumerable<RS_OM_Creation> data = db.Database.SqlQuery<RS_OM_Creation>(query).ToList();

            RS_OM_Creation mmOmCreation = (RS_OM_Creation)data.ToList()[0];
            ViewBag.Quantity = hdnQuantity;
            ViewBag.ModelCode = mmOmCreation.Model_Code;
            ViewBag.ColorCode = mmOmCreation.Colour_ID;
            ViewBag.Shop_Id = mmOmCreation.Shop_ID;

            ////RS_Series series = db.RS_Series.Where(p => p.Series_Code == mmOmCreation.Series_code).Single();
            ////ViewBag.Series_Code = series.Series_Description;

            RS_Model_Master mmModelMasterObj;
            mmModelMasterObj = (from modelMaster in db.RS_Model_Master
                                where (from orderCreate in db.RS_OM_Creation where orderCreate.Row_ID == hdnRowId select orderCreate.Model_Code).Contains(modelMaster.Model_Code)
                                select modelMaster).Single();

            String configId = mmModelMasterObj.OMconfig_ID;

            RS_OM_Configuration[] omConfiguration;
            omConfiguration = (from configuration in db.RS_OM_Configuration
                               where configuration.OMconfig_ID == configId
                               select configuration).ToArray();

            ViewBag.omConfiguration = omConfiguration;


            // process to date the total released order for the day
            try
            {
                query = "select * from RS_OM_OrderRelease where CONVERT(VARCHAR(10),Inserted_Date,10)='" + dtObj + "'";
                IEnumerable<RS_OM_OrderRelease> totalRelease = db.Database.SqlQuery<RS_OM_OrderRelease>(query);
            }
            catch (Exception ex)
            {
                ViewBag.TotalReleasedItem = 0;
            }

            return PartialView(data.ToList());

        }

        /*               Action Name               : GetShopMode
         *               Description               : Action used to find out the order type for Order Release
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : order_type
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find GetShopMode
        public ActionResult GetShopMode(String order_type)
        {
            try
            {
                if (order_type == "S")
                {
                    var st = from shop in db.RS_Shops
                             where shop.Is_Main != true
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
                             where shop.Is_Main == true
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
        public ActionResult Index()
        {
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceModules.OM_Release;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "OMOrderRelease";
                globalData.actionName = ResourceGlobal.Lists;
                globalData.contentTitle = ResourceModules.OM_Release;
                globalData.contentFooter = ResourceModules.OM_Release;

                ViewBag.GlobalDataModel = globalData;

                //Stattic Order List for Order Type
                var order_List =
                //    new SelectList(new[]
                //{
                //    new { ID = "P", Name = "Parent" },
                //    new { ID = "S", Name = "Spare" },
                //},
                //        "ID", "Name", 1);


                ViewBag.Order_Type = new SelectList(db.RS_OM_Order_Type.Where(m => m.Plant_ID == 0), "Order_Type_Name", "Order_Type_Name");

                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
                //decimal plantid = Convert.ToInt32(1);
                ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(p => p.Plant_ID == 0), "Platform_ID", "Platform_Name");
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Plant_ID == 0), "Line_ID", "Line_Name");

                // ViewBag.Plant_ID = db.RS_Plants.Where(c => c.Plant_ID == plantid).Select(x=>x.Plant_Name).FirstOrDefault(); 

                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }
        class JSONModelWiseData
        {
            public int Qty { get; set; }
            public decimal Plant_OrderNo { get; set; }
            public Nullable<int> Release_Qty { get; set; }
            public int Release_Qty_Create { get; set; }

            public string Model_Code { get; set; }
            public string Order_Type { get; set; }
            public string Attribute { get; set; }
            public bool is_added { get; set; }
            public string msg { get; set; }
            public decimal Model_Attribute_ID { get; set; }


        }
        class JSONAttributeWiseData
        {
            public int Qty { get; set; }
            public Nullable<int> Release_Qty { get; set; }
            public Nullable<int> Release_Qty_Create { get; set; }

            public string Order_Type { get; set; }
            public string Attribute { get; set; }
            public bool is_added { get; set; }
            public string msg { get; set; }


        }
        public ActionResult getOrderByModel(int Shop_ID, int Line_Id, int Platform_ID, int OrderType)
        {

            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            string ordertype = db.RS_OM_Order_Type.Find(OrderType).Order_Type_Name;

            var data = (from order in db.RS_OM_Creation
                        join model in db.RS_Model_Master on order.Model_Code.ToLower() equals model.Model_Code.ToLower()
                        where order.Plant_ID == plantId && order.Shop_ID == Shop_ID && order.Line_ID == Line_Id && (order.Release_Qty < order.Qty || order.Release_Qty == null) && order.Platform_Id == Platform_ID && order.Order_Type.ToLower() == ordertype.ToLower()
                        orderby order.Plant_OrderNo
                        select new JSONModelWiseData
                        {
                            Plant_OrderNo = order.Plant_OrderNo,
                            Qty = order.Qty,
                            Release_Qty = order.Release_Qty ?? 0,
                            Model_Code = order.Model_Code,
                            Order_Type = order.Order_Type,
                            Attribute = model.RS_Model_Attribute_Master.Attribution,
                            Model_Attribute_ID = (decimal)model.Model_Attribute_ID
                        }).ToList();
            //foreach (var or in data)
            //{

            //    var model = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantId && m.Shop_ID == Shop_ID).FirstOrDefault();
            //    var Attribute = "";
            //    List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
            //    for (int i = 0; i < attributionParameters.Count; i++)
            //    {
            //        AttributionParameters attributionParameter = attributionParameters[i];
            //        try
            //        {
            //            Convert.ToInt32(attributionParameter.Value);
            //        }
            //        catch (Exception)
            //        {

            //            continue;
            //        }
            //        if (attributionParameter.label.Equals("Attribute", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            int attrId = Convert.ToInt32(attributionParameter.Value);
            //            Attribute = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
            //            break;
            //            //       attributionParameter.Value;
            //        }

            //    }
            //    or.Attribute = Attribute;
            //    if (or.Release_Qty == null)
            //    {
            //        or.Release_Qty = 0;
            //    }
            //}





            return Json(data, JsonRequestBehavior.AllowGet);

        }
        //public ActionResult getOrderByModel(int Shop_ID, int Platform_ID, int OrderType)
        //{

        //    int plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //    string ordertype = db.RS_OM_Order_Type.Find(OrderType).Order_Type_Name;

        //    var data = (from order in db.RS_OM_Creation
        //                join model in db.RS_Model_Master on order.Model_Code.ToLower() equals model.Model_Code.ToLower()
        //                where order.Plant_ID == plantId && order.Shop_ID == Shop_ID && (order.Release_Qty < order.Qty || order.Release_Qty == null) && order.Platform_Id == Platform_ID && order.Order_Type.ToLower() == ordertype.ToLower()
        //                orderby order.Plant_OrderNo
        //                select new JSONModelWiseData
        //                {
        //                    Plant_OrderNo = order.Plant_OrderNo,
        //                    Qty = order.Qty,
        //                    Release_Qty = order.Release_Qty,
        //                    Model_Code = order.Model_Code,
        //                    Order_Type = order.Order_Type,
        //                }).ToList();
        //    foreach (var or in data)
        //    {

        //        var model = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantId && m.Shop_ID == Shop_ID).FirstOrDefault();
        //        var Attribute = "";
        //        List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
        //        for (int i = 0; i < attributionParameters.Count; i++)
        //        {
        //            AttributionParameters attributionParameter = attributionParameters[i];
        //            try
        //            {
        //                Convert.ToInt32(attributionParameter.Value);
        //            }
        //            catch (Exception)
        //            {

        //                continue;
        //            }
        //            if (attributionParameter.label.Equals("Attribute", StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                int attrId = Convert.ToInt32(attributionParameter.Value);
        //                Attribute = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
        //                break;
        //                //       attributionParameter.Value;
        //            }

        //        }
        //        or.Attribute = Attribute;
        //        if (or.Release_Qty == null)
        //        {
        //            or.Release_Qty = 0;
        //        }
        //    }





        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}
        public ActionResult getOrderByAttribute(int Shop_ID, int Line_id, int Platform_ID, int OrderType)
        {

            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            string ordertype = db.RS_OM_Order_Type.Find(OrderType).Order_Type_Name;

            var data = (from order in db.RS_OM_Creation
                        join model in db.RS_Model_Master on order.Model_Code.ToLower() equals model.Model_Code.ToLower()
                        where order.Plant_ID == plantId && order.Shop_ID == Shop_ID && order.Line_ID == Line_id && (order.Release_Qty < order.Qty || order.Release_Qty == null) && order.Platform_Id == Platform_ID && order.Order_Type.ToLower() == ordertype.ToLower()
                        orderby order.Plant_OrderNo
                        select new JSONModelWiseData
                        {
                            Plant_OrderNo = order.Plant_OrderNo,
                            Qty = order.Qty,
                            Release_Qty = order.Release_Qty,
                            Model_Code = order.Model_Code,
                            Order_Type = order.Order_Type,
                            Attribute = model.RS_Model_Attribute_Master.Attribution,
                            Model_Attribute_ID = model.RS_Model_Attribute_Master.Model_Attribute_ID

                        }).ToList();
            //foreach (var or in data)
            //{

            //    var model = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantId && m.Shop_ID == Shop_ID).FirstOrDefault();
            //    var Attribute = "";
            //    List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
            //    for (int i = 0; i < attributionParameters.Count; i++)
            //    {
            //        AttributionParameters attributionParameter = attributionParameters[i];
            //        try
            //        {
            //            Convert.ToInt32(attributionParameter.Value);
            //        }
            //        catch (Exception)
            //        {

            //            continue;
            //        }
            //        if (attributionParameter.label.Equals("Attribute", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            int attrId = Convert.ToInt32(attributionParameter.Value);
            //            Attribute = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
            //            break;
            //            //       attributionParameter.Value;
            //        }

            //    }
            //    or.Attribute = Attribute;
            //    if (or.Release_Qty == null)
            //    {
            //        or.Release_Qty = 0;
            //    }
            //}

            //grouping by attribute
            var dataObj = data.GroupBy(d => d.Attribute)
                                .Select(
                                    g => new JSONAttributeWiseData
                                    {
                                        Attribute = g.Key,
                                        Qty = g.Sum(s => s.Qty),
                                        Release_Qty = g.Sum(s => s.Release_Qty),
                                        Order_Type = g.First().Order_Type
                                    });

            ////end of grouping




            return Json(dataObj, JsonRequestBehavior.AllowGet);

        }
        //public ActionResult getOrderByAttribute(int Shop_ID, int Platform_ID, int OrderType)
        //{

        //    int plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //    string ordertype = db.RS_OM_Order_Type.Find(OrderType).Order_Type_Name;

        //    var data = (from order in db.RS_OM_Creation
        //                join model in db.RS_Model_Master on order.Model_Code.ToLower() equals model.Model_Code.ToLower()
        //                where order.Plant_ID == plantId && order.Shop_ID == Shop_ID && (order.Release_Qty < order.Qty || order.Release_Qty == null) && order.Platform_Id == Platform_ID && order.Order_Type.ToLower() == ordertype.ToLower()
        //                orderby order.Plant_OrderNo
        //                select new JSONModelWiseData
        //                {
        //                    Plant_OrderNo = order.Plant_OrderNo,
        //                    Qty = order.Qty,
        //                    Release_Qty = order.Release_Qty,
        //                    Model_Code = order.Model_Code,
        //                    Order_Type = order.Order_Type

        //                }).ToList();
        //    foreach (var or in data)
        //    {

        //        var model = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantId && m.Shop_ID == Shop_ID).FirstOrDefault();
        //        var Attribute = "";
        //        List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
        //        for (int i = 0; i < attributionParameters.Count; i++)
        //        {
        //            AttributionParameters attributionParameter = attributionParameters[i];
        //            try
        //            {
        //                Convert.ToInt32(attributionParameter.Value);
        //            }
        //            catch (Exception)
        //            {

        //                continue;
        //            }
        //            if (attributionParameter.label.Equals("Attribute", StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                int attrId = Convert.ToInt32(attributionParameter.Value);
        //                Attribute = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
        //                break;
        //                //       attributionParameter.Value;
        //            }

        //        }
        //        or.Attribute = Attribute;
        //        if (or.Release_Qty == null)
        //        {
        //            or.Release_Qty = 0;
        //        }
        //    }

        //    //grouping by attribute
        //    var dataObj = data.GroupBy(d => d.Attribute)
        //                        .Select(
        //                            g => new JSONAttributeWiseData
        //                            {
        //                                Attribute = g.Key,
        //                                Qty = g.Sum(s => s.Qty),
        //                                Release_Qty = g.Sum(s => s.Release_Qty),
        //                                Order_Type = g.First().Order_Type
        //                            });

        //    ////end of grouping




        //    return Json(dataObj, JsonRequestBehavior.AllowGet);

        //}

        /*               Action Name               : Create
         *               Description               : Action used to get data for Create the Order.
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Create
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: OM_OrderRelease/Create
        public ActionResult Create()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.OM_Release;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "OMOrderRelease";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.OM_Release;
            globalData.contentFooter = ResourceModules.OM_Release;

            ViewBag.GlobalDataModel = globalData;

            //Stattic Order List for Order Type



            ViewBag.Order_Type = new SelectList(db.RS_OM_Order_Type.Where(m => m.Plant_ID == plantId), "Order_Type_Name", "Order_Type_Name");

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name");
            decimal plantid = Convert.ToInt32(1);
            // ViewBag.Plant_ID = db.RS_Plants.Where(c => c.Plant_ID == plantid).Select(x=>x.Plant_Name).FirstOrDefault(); 

            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            return View();
        }

        /*               Action Name               : Create
         *               Description               : Action used to Create the Order.
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : RS_OM_OrderRelease
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // POST: OM_OrderRelease/Create
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_OM_OrderRelease RS_OM_OrderRelease)
        {
            if (ModelState.IsValid)
            {
                db.RS_OM_OrderRelease.Add(RS_OM_OrderRelease);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_OM_OrderRelease.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_OM_OrderRelease.Shop_ID);
            return View(RS_OM_OrderRelease);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        /*               Action Name               : GetCreatedOrderDetails
         *               Description               : Action used to get details for created Order details.
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : rowId
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult GetCreatedOrderDetails(int rowId)
        {
            try
            {

                var st = from orderDetail in db.RS_OM_Creation
                         join plant in db.RS_Plants on orderDetail.Plant_ID equals plant.Plant_ID
                         join shop in db.RS_Shops on orderDetail.Shop_ID equals shop.Shop_ID
                         where orderDetail.Row_ID == rowId
                         select new
                         {
                             Plant_OrderNo = orderDetail.Plant_OrderNo,
                             Model_Code = orderDetail.Model_Code,
                             Order_Type = orderDetail.Order_Type,
                             ///Series_Code = orderDetail.RS_Series.Series_Description,
                             Quantity_Min_To_Release = (orderDetail.Qty - orderDetail.Release_Qty),
                             Release_Qty = orderDetail.Release_Qty,
                             Qty = orderDetail.Qty

                         };

                RS_OM_Creation creation = db.RS_OM_Creation.Where(p => p.Row_ID == rowId).Single();
                ViewBag.plant = creation.Plant_ID;

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /*               Action Name               : CreateOrders
         *               Description               : Action used created Order.
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : rowId,priority,remark,quantity
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        [HttpPost]
        public ActionResult CreateReleaseModelOrders(int Shop_id, int Line_Id, int Platform_ID, string OrderType, List<string> orderData)
        {

            DateTime nowTime = DateTime.Now;
            int plantId, shopId = 0;
            int? lineId = null;
            List<JSONModelWiseData> CreatedModelWiseData = new List<JSONModelWiseData>();
            JSONModelWiseData CreatedModelWiseDataObj = new JSONModelWiseData();

            List<JSONModelWiseData> JSONModelWiseData =
                (List<JSONModelWiseData>)JsonConvert.DeserializeObject(orderData[0], typeof(List<JSONModelWiseData>));
            if (!(JSONModelWiseData.Count() > 0))
            {

            }
            try
            {
                foreach (var or in JSONModelWiseData)
                {
                    var Plant_OrderNo = or.Plant_OrderNo;
                    var Model_Code = or.Model_Code;
                    var Order_Type = or.Order_Type;
                    var Release_Qty = or.Release_Qty;
                    var Qty = or.Qty;
                    CreatedModelWiseDataObj = or;
                    //db.Configuration.AutoDetectChangesEnabled = false;
                    //db.Configuration.ValidateOnSaveEnabled = false;

                    //string part_number;
                    decimal? series_code;
                    int Line_Code;
                    //string sub_partno, sub_series;
                    RS_Model_Master mmModelMasterObj;
                    RS_OM_Creation mmOmCreationObj;

                    RS_OM_Configuration[] omConfiguration;
                    RS_Partgroup partGroupObj;
                    plantId = ((FDSession)this.Session["FDSession"]).plantId;

                    mmModelMasterObj = (from modelMaster in db.RS_Model_Master
                                        where (from orderCreate in db.RS_OM_Creation where orderCreate.Plant_OrderNo == Plant_OrderNo && orderCreate.Model_Code.ToLower() == Model_Code.ToLower() && orderCreate.Order_Type.ToLower() == Order_Type.ToLower() && orderCreate.Line_ID == Line_Id && orderCreate.Plant_ID == plantId select orderCreate.Model_Code).Contains(modelMaster.Model_Code) && modelMaster.Plant_ID == plantId && modelMaster.Shop_ID == Shop_id
                                        select modelMaster).FirstOrDefault();


                    mmOmCreationObj = (from orderCreate in db.RS_OM_Creation
                                       where orderCreate.Plant_OrderNo == Plant_OrderNo && orderCreate.Plant_ID == plantId && orderCreate.Shop_ID == Shop_id && orderCreate.Line_ID == Line_Id
                                       select orderCreate).FirstOrDefault();

                    //Enter quantity check 
                    int totqty;
                    totqty = (Convert.ToInt32(mmOmCreationObj.Qty) - Convert.ToInt32(mmOmCreationObj.Release_Qty));

                    if (Release_Qty > totqty)
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
                                           where configuration.OMconfig_ID == configId && partgroup_id.Order_Create == true && configuration.Plant_ID == plantId && configuration.Shop_ID == Shop_id
                                           select configuration).ToArray();


                        int totalOrder = 0;
                        RS_OM_OrderRelease omOrderRelease = new RS_OM_OrderRelease();
                        omOrderRelease.Plant_ID = mmOmCreationObj.Plant_ID;
                        int Plant_ID = Convert.ToInt32(omOrderRelease.Plant_ID);
                        omOrderRelease.Model_Code = mmModelMasterObj.Model_Code;
                        string model_code = omOrderRelease.Model_Code;
                        omOrderRelease.Order_Type = mmOmCreationObj.Order_Type.Trim();
                        omOrderRelease.Order_Status = "Release";
                        omOrderRelease.Remarks = "";
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
                                for (int j = 0; j < Release_Qty; j++)
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

                                    omOrderRelease.Country_ID = Convert.ToInt32(mmOmCreationObj.Country_ID != null ? mmOmCreationObj.Country_ID : 1);
                                    //omOrderRelease.Country_ID = 1;
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
                                            omOrderRelease.Country_ID = Convert.ToInt32(mmOmCreationObj.Country_ID != null ? mmOmCreationObj.Country_ID : 1);
                                            //omOrderRelease.Country_ID = 1;
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
                                                   where orderCreation.Plant_OrderNo == Plant_OrderNo
                                                   select orderCreation).SingleOrDefault();

                                int quantityRelease = Convert.ToInt32(mmOmCreationObj.Release_Qty);
                                mmOmCreationObj.Release_Qty = quantityRelease + Release_Qty;

                                db.Entry(mmOmCreationObj).State = EntityState.Modified;
                                db.SaveChanges();

                                or.is_added = true;
                                or.msg = "Added Successfully";

                                CreatedModelWiseData.Add(or);

                            }

                        }
                        else
                        {
                            or.is_added = false;
                            or.msg = "bom not available";


                            CreatedModelWiseData.Add(or);
                        }
                    }

                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                var msg = "";
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        msg += Message;
                        raise = new InvalidOperationException(Message, raise);
                    }
                }
                CreatedModelWiseDataObj.msg = msg;

                CreatedModelWiseData.Add(CreatedModelWiseDataObj);

            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "PPC Module:Order Creation", "Create Order::Post", Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                CreatedModelWiseDataObj.msg = ex.Message;
                CreatedModelWiseData.Add(CreatedModelWiseDataObj);
            }
            finally
            {
                generalHelper.logUserActivity(shopId, lineId, "PPC Module", "Order Release", nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
            }
            return Json(CreatedModelWiseData.ToList(), JsonRequestBehavior.AllowGet);

        }

        //[HttpPost]
        //public ActionResult CreateReleaseModelOrders(int Shop_id, int Platform_ID, string OrderType, List<string> orderData)
        //{

        //    DateTime nowTime = DateTime.Now;
        //    int plantId, shopId = 0;
        //    int? lineId = null;
        //    List<JSONModelWiseData> CreatedModelWiseData = new List<JSONModelWiseData>();
        //    JSONModelWiseData CreatedModelWiseDataObj = new JSONModelWiseData();

        //    List<JSONModelWiseData> JSONModelWiseData =
        //        (List<JSONModelWiseData>)JsonConvert.DeserializeObject(orderData[0], typeof(List<JSONModelWiseData>));
        //    if (!(JSONModelWiseData.Count() > 0))
        //    {

        //    }
        //    try
        //    {
        //        foreach (var or in JSONModelWiseData)
        //        {
        //            var Plant_OrderNo = or.Plant_OrderNo;
        //            var Model_Code = or.Model_Code;
        //            var Order_Type = or.Order_Type;
        //            var Release_Qty = or.Release_Qty;
        //            var Qty = or.Qty;
        //            CreatedModelWiseDataObj = or;
        //            //db.Configuration.AutoDetectChangesEnabled = false;
        //            //db.Configuration.ValidateOnSaveEnabled = false;

        //            //string part_number;
        //            decimal? series_code;
        //            int Line_Code;
        //            //string sub_partno, sub_series;
        //            RS_Model_Master mmModelMasterObj;
        //            RS_OM_Creation mmOmCreationObj;

        //            RS_OM_Configuration[] omConfiguration;
        //            RS_Partgroup partGroupObj;
        //            plantId = ((FDSession)this.Session["FDSession"]).plantId;

        //            mmModelMasterObj = (from modelMaster in db.RS_Model_Master
        //                                where (from orderCreate in db.RS_OM_Creation where orderCreate.Plant_OrderNo == Plant_OrderNo && orderCreate.Model_Code.ToLower() == Model_Code.ToLower() && orderCreate.Order_Type.ToLower() == Order_Type.ToLower() && orderCreate.Plant_ID == plantId select orderCreate.Model_Code).Contains(modelMaster.Model_Code) && modelMaster.Plant_ID == plantId
        //                                select modelMaster).FirstOrDefault();


        //            mmOmCreationObj = (from orderCreate in db.RS_OM_Creation
        //                               where orderCreate.Plant_OrderNo == Plant_OrderNo && orderCreate.Plant_ID == plantId
        //                               select orderCreate).FirstOrDefault();

        //            //Enter quantity check 
        //            int totqty;
        //            totqty = (Convert.ToInt32(mmOmCreationObj.Qty) - Convert.ToInt32(mmOmCreationObj.Release_Qty));

        //            if (Release_Qty > totqty)
        //            {
        //                ModelState.AddModelError("Quantity", ResourceValidation.Qty_Is_Greater);
        //            }
        //            else
        //            {
        //                String configId = mmModelMasterObj.OMconfig_ID;
        //                decimal? partGroupId = null;
        //                shopId = 0;

        //                omConfiguration = (from configuration in db.RS_OM_Configuration
        //                                   join partgroup_id in db.RS_Partgroup on configuration.Partgroup_ID equals partgroup_id.Partgroup_ID
        //                                   where configuration.OMconfig_ID == configId && partgroup_id.Order_Create == true && configuration.Plant_ID == plantId
        //                                   select configuration).ToArray();


        //                int totalOrder = 0;
        //                RS_OM_OrderRelease omOrderRelease = new RS_OM_OrderRelease();
        //                omOrderRelease.Plant_ID = mmOmCreationObj.Plant_ID;
        //                int Plant_ID = Convert.ToInt32(omOrderRelease.Plant_ID);
        //                omOrderRelease.Model_Code = mmModelMasterObj.Model_Code;
        //                string model_code = omOrderRelease.Model_Code;
        //                omOrderRelease.Order_Type = mmOmCreationObj.Order_Type.Trim();
        //                omOrderRelease.Order_Status = "Release";
        //                omOrderRelease.Remarks = "";
        //                omOrderRelease.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                omOrderRelease.Inserted_Date = DateTime.Now;
        //                omOrderRelease.Plant_OrderNo = mmOmCreationObj.Plant_OrderNo;
        //                omOrderRelease.Planned_Date = mmOmCreationObj.Planned_Date;
        //                if (omOrderRelease.isBOMavailableOrNot(model_code) == true)
        //                {
        //                    if (omConfiguration != null)
        //                    {
        //                        bool ok = true;
        //                        int groupId = omOrderRelease.getGroupNo();
        //                        for (int j = 0; j < Release_Qty; j++)
        //                        {

        //                            String uToken = new Random().Next(10000, 99999).ToString() + DateTime.Now.Ticks;
        //                            //RSN Number
        //                            partGroupId = db.RS_PartgroupItem.Where(a => a.Part_No == model_code && a.Plant_ID == Plant_ID).FirstOrDefault().Partgroup_ID;
        //                            omOrderRelease.partno = model_code;
        //                            omOrderRelease.Series_Code = mmModelMasterObj.Series_Code;

        //                            // partGroupObj = db.RS_Partgroup.Find(partGroupId);
        //                            partGroupObj = db.RS_Partgroup.Where(p => p.Plant_ID == Plant_ID && p.Partgroup_ID == partGroupId).Select(p => p).FirstOrDefault();
        //                            shopId = Convert.ToInt32(partGroupObj.Shop_ID);
        //                            omOrderRelease.Shop_ID = shopId;

        //                            DateTime Inserted_Date = omOrderRelease.Inserted_Date;
        //                            omOrderRelease.ORN = Convert.ToInt32(omOrderRelease.generateORNNumber(Plant_ID, Inserted_Date));
        //                            omOrderRelease.RSN = Convert.ToInt32(omOrderRelease.generateRSNNumber(Plant_ID, shopId, mmOmCreationObj.Planned_Date));
        //                            omOrderRelease.CUMN = omOrderRelease.ORN;

        //                            series_code = mmModelMasterObj.Series_Code;
        //                            lineId = Convert.ToInt32(partGroupObj.Line_ID);
        //                            //Commented by ketan 19-08-2017
        //                            //Line_Code = Convert.ToInt32(partGroupObj.Line_ID);

        //                            decimal PlantOrderNo = db.RS_OM_Creation.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Model_Code == model_code).Max(q => (decimal?)(q.Plant_OrderNo)) ?? 0;
        //                            decimal Line_ID = db.RS_OM_Creation.Where(m => m.Plant_OrderNo == PlantOrderNo).Select(m => m.Line_ID).FirstOrDefault();
        //                            omOrderRelease.Line_ID = Line_ID;

        //                            totalOrder = omOrderRelease.getTotalOrderReleasedByDate(shopId, Convert.ToInt16(Line_ID));
        //                            omOrderRelease.Order_No = omOrderRelease.generateOrderNumber(totalOrder + 1, model_code);

        //                            omOrderRelease.Inserted_Date = DateTime.Now;
        //                            omOrderRelease.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                            omOrderRelease.Updated_Date = DateTime.Now;
        //                            omOrderRelease.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                            //omOrderRelease.Model_Color = "GR"
        //                            //added by ketan Date 19-08-17
        //                            bool IsColorCheck = Convert.ToBoolean(db.RS_Model_Master.Where(m => m.Model_Code == model_code).Select(m => m.Color_Code).FirstOrDefault());
        //                            if (IsColorCheck)
        //                            {
        //                                var color_code = model_code.Substring(model_code.Length - 2, 2);
        //                                omOrderRelease.Model_Color = color_code;
        //                                //var color_Name = db.MM_Color.Where(m => m.COLOUR_ID == color_code).Select(c => c.COLOUR_DESC );
        //                            }
        //                            else
        //                            {
        //                                omOrderRelease.Model_Color = mmOmCreationObj.Colour_ID;
        //                            }
        //                            omOrderRelease.Country_ID = 1;
        //                            omOrderRelease.Order_Start = false;
        //                            omOrderRelease.Is_Active = true;
        //                            omOrderRelease.Is_Deleted = false;
        //                            omOrderRelease.UToken = uToken;
        //                            db.RS_OM_OrderRelease.Add(omOrderRelease);
        //                            db.SaveChanges();


        //                            RS_OM_OrderRelease orderReleaseObj = new RS_OM_OrderRelease();
        //                            //orderReleaseObj.addRecordToPlannedOrders(omOrderRelease, groupId);

        //                            String modelCode = omOrderRelease.Model_Code;
        //                            decimal? seriesCode = omOrderRelease.Series_Code;
        //                            //mmOmPlannedOrdersObj = new RS_OM_Planned_Orders();
        //                            //Child Order Release
        //                            for (int i = 0; i < omConfiguration.Count(); i++)
        //                            {
        //                                partGroupId = omConfiguration[i].Partgroup_ID;

        //                                partGroupObj = db.RS_Partgroup.Where(p => p.Partgroup_ID == partGroupId).SingleOrDefault();
        //                                shopId = Convert.ToInt32(partGroupObj.Shop_ID);
        //                                omOrderRelease.Shop_ID = shopId;

        //                                RS_PartgroupItem partgroupItemObj = omOrderRelease.getPartGroupItemByPartGroupAndModelCode(partGroupId, model_code);
        //                                if (partgroupItemObj == null)
        //                                {

        //                                }
        //                                else
        //                                {
        //                                    omOrderRelease.partno = partgroupItemObj.Part_No;
        //                                    RS_Partmaster mmPartMasterObj = omOrderRelease.getPartmasterByPartNumber(partgroupItemObj.Part_No);

        //                                    if (partgroupItemObj.Series_Code == null || partgroupItemObj.Series_Code == null)
        //                                    {

        //                                        omOrderRelease.Series_Code = mmModelMasterObj.Series_Code;
        //                                    }
        //                                    else
        //                                    {
        //                                        omOrderRelease.Series_Code = partgroupItemObj.Series_Code;
        //                                    }

        //                                    //series_code = mmModelMasterObj.Series_Code;
        //                                    Line_Code = Convert.ToInt32(partGroupObj.Line_ID);
        //                                    omOrderRelease.Line_ID = Line_Code;

        //                                    totalOrder = omOrderRelease.getTotalOrderReleasedByDate(shopId, Line_Code);
        //                                    omOrderRelease.Order_No = omOrderRelease.generateOrderNumber(totalOrder + 1, model_code);

        //                                    omOrderRelease.RSN = Convert.ToInt32(omOrderRelease.generateRSNNumber(Plant_ID, shopId, mmOmCreationObj.Planned_Date));
        //                                    omOrderRelease.Inserted_Date = DateTime.Now;
        //                                    omOrderRelease.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                                    omOrderRelease.Updated_Date = DateTime.Now;
        //                                    omOrderRelease.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                                    //omOrderRelease.Model_Color = "GR";
        //                                    IsColorCheck = Convert.ToBoolean(db.RS_Model_Master.Where(m => m.Model_Code == model_code).Select(m => m.Color_Code).FirstOrDefault());
        //                                    if (IsColorCheck)
        //                                    {
        //                                        var color_code = model_code.Substring(model_code.Length - 2, 2);
        //                                        omOrderRelease.Model_Color = color_code;
        //                                        //var color_Name = db.MM_Color.Where(m => m.COLOUR_ID == color_code).Select(c => c.COLOUR_DESC );
        //                                    }
        //                                    else
        //                                    {
        //                                        omOrderRelease.Model_Color = mmOmCreationObj.Colour_ID;
        //                                    }
        //                                    omOrderRelease.Country_ID = 1;
        //                                    omOrderRelease.Order_Start = false;

        //                                    omOrderRelease.Is_Active = true;
        //                                    omOrderRelease.Is_Deleted = false;
        //                                    omOrderRelease.UToken = uToken;
        //                                    db.RS_OM_OrderRelease.Add(omOrderRelease);
        //                                    db.SaveChanges();
        //                                }
        //                            }
        //                        }
        //                        mmOmCreationObj = (from orderCreation in db.RS_OM_Creation
        //                                           where orderCreation.Plant_OrderNo == Plant_OrderNo
        //                                           select orderCreation).SingleOrDefault();

        //                        int quantityRelease = Convert.ToInt32(mmOmCreationObj.Release_Qty);
        //                        mmOmCreationObj.Release_Qty = quantityRelease + Release_Qty;

        //                        db.Entry(mmOmCreationObj).State = EntityState.Modified;
        //                        db.SaveChanges();

        //                        or.is_added = true;
        //                        or.msg = "Added Successfully";

        //                        CreatedModelWiseData.Add(or);

        //                    }

        //                }
        //                else
        //                {
        //                    or.is_added = false;
        //                    or.msg = "bom not available";


        //                    CreatedModelWiseData.Add(or);
        //                }
        //            }

        //        }
        //    }
        //    catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
        //    {
        //        Exception raise = dbex;
        //        var msg = "";
        //        foreach (var ValidationErrors in dbex.EntityValidationErrors)
        //        {
        //            foreach (var validationError in ValidationErrors.ValidationErrors)
        //            {
        //                string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
        //                msg += Message;
        //                raise = new InvalidOperationException(Message, raise);
        //            }
        //        }
        //        CreatedModelWiseDataObj.msg = msg;

        //        CreatedModelWiseData.Add(CreatedModelWiseDataObj);

        //    }
        //    catch (Exception ex)
        //    {
        //        generalHelper.addControllerException(ex, "PPC Module:Order Creation", "Create Order::Post", Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
        //        while (ex.InnerException != null)
        //        {
        //            ex = ex.InnerException;
        //        }
        //        CreatedModelWiseDataObj.msg = ex.Message;
        //        CreatedModelWiseData.Add(CreatedModelWiseDataObj);
        //    }
        //    finally
        //    {
        //        generalHelper.logUserActivity(shopId, lineId, "PPC Module", "Order Release", nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
        //    }
        //    return Json(CreatedModelWiseData.ToList(), JsonRequestBehavior.AllowGet);

        //}


        //Create class
        public class order_release_data
        {
            public string Plant_OrderNo { get; set; }
            public string Release_Qty { get; set; }
            public string Model_Code { get; set; }
            public string Order_Type { get; set; }
            public string Attribute { get; set; }

        }

        [HttpPost]
        public ActionResult CreateReleaseAttributeWiseOrders(int Shop_id, int Line_Id, int Platform_ID, string OrderType, List<string> orderData)
        {
            DateTime nowTime = DateTime.Now;
            int plantId, shopId = 0;
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            int? lineId = null;
            List<JSONAttributeWiseData> CreatedAttributeWiseData = new List<JSONAttributeWiseData>();
            List<JSONModelWiseData> CreatedModelWiseData = new List<JSONModelWiseData>();
            JSONModelWiseData CreatedModelWiseDataObj = new JSONModelWiseData();

            List<JSONAttributeWiseData> JSONAttributeWiseData = (List<JSONAttributeWiseData>)JsonConvert.DeserializeObject(orderData[0], typeof(List<JSONAttributeWiseData>));

            List<JSONModelWiseData> jSONModelWiseData = new List<JSONModelWiseData>();
            if (!(JSONAttributeWiseData.Count() > 0))
            {
                CreatedModelWiseDataObj.is_added = false;
                CreatedModelWiseDataObj.msg = "Please add order before releasing";
            }
            try
            {
                var ordType = JSONAttributeWiseData[0].Order_Type;
                var data = (from order in db.RS_OM_Creation
                            join model in db.RS_Model_Master on order.Model_Code.ToLower() equals model.Model_Code.ToLower()
                            where order.Plant_ID == plantId && order.Shop_ID == Shop_id && (order.Release_Qty < order.Qty || order.Release_Qty == null) && order.Platform_Id == Platform_ID && order.Line_ID == Line_Id && order.Order_Type.ToLower() == ordType.ToLower()
                            orderby order.Plant_OrderNo
                            select new JSONModelWiseData
                            {
                                Plant_OrderNo = order.Plant_OrderNo,
                                Qty = order.Qty,
                                Release_Qty = order.Release_Qty ?? 0,
                                Model_Code = order.Model_Code,
                                Order_Type = order.Order_Type,
                                Release_Qty_Create = 0,
                                Attribute = model.RS_Model_Attribute_Master.Attribution,
                                Model_Attribute_ID = model.RS_Model_Attribute_Master.Model_Attribute_ID

                            }).ToList();
                //foreach (var or in data)
                //{
                //    var model = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantId && m.Shop_ID == Shop_id).FirstOrDefault();
                //    var Attribute = "";
                //    List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
                //    for (int i = 0; i < attributionParameters.Count; i++)
                //    {
                //        AttributionParameters attributionParameter = attributionParameters[i];
                //        try
                //        {
                //            Convert.ToInt32(attributionParameter.Value);
                //        }
                //        catch (Exception)
                //        {

                //            continue;
                //        }
                //        if (attributionParameter.label.Equals("Attribute", StringComparison.InvariantCultureIgnoreCase))
                //        {
                //            int attrId = Convert.ToInt32(attributionParameter.Value);
                //            Attribute = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                //            break;
                //            //       attributionParameter.Value;
                //        }

                //    }
                //    or.Attribute = Attribute;
                //    if (or.Release_Qty == null)
                //    {
                //        or.Release_Qty = 0;
                //    }
                //}
                //var orderData = from modelorders in data 
                //                join attr in JSONAttributeWiseData on 
                //var iCount = 0;
                foreach (var attr in JSONAttributeWiseData)
                {
                    var releaseqty = 0;
                    releaseqty = Convert.ToInt32(attr.Release_Qty);

                    for (int i = 0; i < data.Count(); i++)
                    //foreach (var modelorder in data)
                    {
                        if (attr.Attribute.Equals(data[i].Attribute, StringComparison.CurrentCultureIgnoreCase) && attr.Order_Type.Equals(data[i].Order_Type, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var modelRelease = Convert.ToInt32(data[i].Qty - data[i].Release_Qty);
                            if (releaseqty > modelRelease)
                            {
                                data[i].Release_Qty += modelRelease;
                                releaseqty = releaseqty - modelRelease;
                                //iCount = i + 1;
                                JSONModelWiseData dataModel = new JSONModelWiseData();
                                dataModel.Release_Qty_Create = modelRelease;
                                dataModel.Attribute = data[i].Attribute;
                                dataModel.Model_Code = data[i].Model_Code;
                                dataModel.Order_Type = data[i].Order_Type;
                                dataModel.Plant_OrderNo = data[i].Plant_OrderNo;
                                dataModel.Qty = data[i].Qty;
                                dataModel.Release_Qty = data[i].Release_Qty;


                                jSONModelWiseData.Add(dataModel);
                            }
                            else if (releaseqty <= modelRelease)
                            {
                                //iCount = i + 1;
                                data[i].Release_Qty += releaseqty;

                                JSONModelWiseData dataModel = new JSONModelWiseData();
                                dataModel.Release_Qty_Create = releaseqty;

                                dataModel.Attribute = data[i].Attribute;
                                dataModel.Model_Code = data[i].Model_Code;
                                dataModel.Order_Type = data[i].Order_Type;
                                dataModel.Plant_OrderNo = data[i].Plant_OrderNo;
                                dataModel.Qty = data[i].Qty;
                                dataModel.Release_Qty = data[i].Release_Qty;



                                //data[i].Release_Qty = releaseqty;
                                releaseqty = 0;
                                jSONModelWiseData.Add(dataModel);
                                break;
                            }

                        }
                    }

                }


                //////end of grouping


                //end of finding available models against attributes
                foreach (var or in jSONModelWiseData)
                {
                    var Plant_OrderNo = or.Plant_OrderNo;
                    var Model_Code = or.Model_Code;
                    var Order_Type = or.Order_Type;
                    var Release_Qty = or.Release_Qty_Create;
                    var Qty = or.Qty;
                    CreatedModelWiseDataObj = or;
                    var ordercounter = 0;


                    //db.Configuration.AutoDetectChangesEnabled = false;
                    //db.Configuration.ValidateOnSaveEnabled = false;

                    //string part_number;
                    decimal? series_code;
                    int Line_Code;
                    //string sub_partno, sub_series;
                    RS_Model_Master mmModelMasterObj;
                    RS_OM_Creation mmOmCreationObj;

                    RS_OM_Configuration[] omConfiguration;
                    RS_Partgroup partGroupObj;

                    mmModelMasterObj = (from modelMaster in db.RS_Model_Master
                                        where (from orderCreate in db.RS_OM_Creation where orderCreate.Plant_OrderNo == Plant_OrderNo && orderCreate.Model_Code.ToLower() == Model_Code.ToLower() && orderCreate.Order_Type.ToLower() == Order_Type.ToLower() && orderCreate.Plant_ID == plantId select orderCreate.Model_Code).Contains(modelMaster.Model_Code) && modelMaster.Plant_ID == plantId
                                        select modelMaster).FirstOrDefault();


                    mmOmCreationObj = (from orderCreate in db.RS_OM_Creation
                                       where orderCreate.Plant_OrderNo == Plant_OrderNo && orderCreate.Plant_ID == plantId
                                       select orderCreate).FirstOrDefault();

                    //Enter quantity check 
                    int totqty;
                    totqty = (Convert.ToInt32(mmOmCreationObj.Qty) - Convert.ToInt32(mmOmCreationObj.Release_Qty));

                    if (Release_Qty > totqty)
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
                        omOrderRelease.Remarks = "";
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
                                for (int j = 0; j < Release_Qty; j++)
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
                                    omOrderRelease.Country_ID = Convert.ToInt32(mmOmCreationObj.Country_ID != null ? mmOmCreationObj.Country_ID : 1);
                                    //omOrderRelease.Country_ID = 1;
                                    omOrderRelease.Order_Start = false;
                                    omOrderRelease.Is_Active = true;
                                    omOrderRelease.Is_Deleted = false;
                                    omOrderRelease.UToken = uToken;
                                    db.RS_OM_OrderRelease.Add(omOrderRelease);
                                    db.SaveChanges();
                                    ordercounter++;

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
                                            omOrderRelease.Country_ID = Convert.ToInt32(mmOmCreationObj.Country_ID != null ? mmOmCreationObj.Country_ID : 1);
                                            //omOrderRelease.Country_ID = 1;
                                            omOrderRelease.Order_Start = false;

                                            omOrderRelease.Is_Active = true;
                                            omOrderRelease.Is_Deleted = false;
                                            omOrderRelease.UToken = uToken;
                                            db.RS_OM_OrderRelease.Add(omOrderRelease);
                                            db.SaveChanges();
                                            ordercounter++;
                                        }
                                    }
                                }
                                mmOmCreationObj = (from orderCreation in db.RS_OM_Creation
                                                   where orderCreation.Plant_OrderNo == Plant_OrderNo
                                                   select orderCreation).SingleOrDefault();

                                int quantityRelease = Convert.ToInt32(mmOmCreationObj.Release_Qty);
                                mmOmCreationObj.Release_Qty = quantityRelease + Release_Qty;

                                db.Entry(mmOmCreationObj).State = EntityState.Modified;
                                db.SaveChanges();
                                or.Release_Qty = ordercounter;
                                or.is_added = true;
                                or.msg = "Added Successfully";

                                CreatedModelWiseData.Add(or);
                            }

                        }
                        else
                        {
                            or.is_added = false;
                            or.msg = "bom not available";
                            or.Release_Qty = 0;
                            CreatedModelWiseData.Add(or);
                        }
                    }

                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                var msg = "";
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        msg += Message;
                        raise = new InvalidOperationException(Message, raise);
                    }
                }
                CreatedModelWiseDataObj.msg = msg;

                CreatedModelWiseData.Add(CreatedModelWiseDataObj);
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "PPC Module:Order Creation", "Create Order::Post", Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                CreatedModelWiseDataObj.msg = ex.Message;
                CreatedModelWiseData.Add(CreatedModelWiseDataObj);
            }
            finally
            {
                generalHelper.logUserActivity(shopId, lineId, "PPC Module", "Order Release", nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
            }
            //for grouping data by attribute
            //foreach (var or in CreatedModelWiseData)
            //{

            //    var model = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantId && m.Shop_ID == Shop_id).FirstOrDefault();
            //    var Attribute = "";
            //    List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
            //    for (int i = 0; i < attributionParameters.Count; i++)
            //    {
            //        AttributionParameters attributionParameter = attributionParameters[i];
            //        try
            //        {
            //            Convert.ToInt32(attributionParameter.Value);
            //        }
            //        catch (Exception)
            //        {

            //            continue;
            //        }
            //        if (attributionParameter.label.Equals("Attribute", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            int attrId = Convert.ToInt32(attributionParameter.Value);
            //            Attribute = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
            //            break;
            //            //       attributionParameter.Value;
            //        }

            //    }
            //    or.Attribute = Attribute;
            //    if (or.Release_Qty == null)
            //    {
            //        or.Release_Qty = 0;
            //    }
            //}

            //grouping by attribute
            var dataObj = (CreatedModelWiseData.GroupBy(d => d.Attribute)
                                .Select(
                                    g => new JSONAttributeWiseData
                                    {
                                        is_added = g.First().is_added,
                                        msg = g.First().msg,
                                        Attribute = g.Key,
                                        Qty = g.Sum(s => s.Qty),
                                        Release_Qty = g.Sum(s => s.Release_Qty),
                                        Order_Type = g.First().Order_Type
                                    })).ToList();
            List<JSONAttributeWiseData> objList = new List<JSONAttributeWiseData>();

            ////end of grouping
            foreach (var or in JSONAttributeWiseData)
            {
                foreach (var res in dataObj)
                {
                    if (res.Attribute == or.Attribute)
                    {
                        if (or.Release_Qty > res.Release_Qty)
                        {
                            JSONAttributeWiseData obj = new JSONAttributeWiseData();
                            obj.is_added = true;
                            obj.Attribute = or.Attribute;
                            obj.Order_Type = or.Order_Type;
                            obj.Release_Qty = or.Release_Qty;
                            obj.Release_Qty_Create = or.Release_Qty;

                            or.is_added = true;
                            objList.Add(obj);

                            res.Release_Qty -= or.Release_Qty;
                            if (res.Release_Qty <= 0)
                            {
                                break;
                            }

                        }
                        else if (or.Release_Qty <= res.Release_Qty)
                        {
                            //or.is_added = true;

                            JSONAttributeWiseData obj = new JSONAttributeWiseData();
                            obj.is_added = true;
                            obj.Attribute = or.Attribute;
                            obj.Order_Type = or.Order_Type;
                            obj.Release_Qty = or.Release_Qty;
                            obj.Release_Qty_Create = or.Release_Qty;
                            objList.Add(obj);

                            res.Release_Qty -= or.Release_Qty;
                            if (res.Release_Qty <= 0)
                            {
                                break;
                            }

                        }
                    }
                }
            }

            return Json(objList.ToList(), JsonRequestBehavior.AllowGet);

        }



        public ActionResult CreateOrders(int rowId, String remark, int quantity, bool isCreationRequest = false)
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
                                omOrderRelease.Country_ID = Convert.ToInt32(mmOmCreationObj.Country_ID != null ? mmOmCreationObj.Country_ID : 1);
                                //omOrderRelease.Country_ID = 1;
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
                                        omOrderRelease.Country_ID = Convert.ToInt32(mmOmCreationObj.Country_ID != null ? mmOmCreationObj.Country_ID : 1);
                                        //omOrderRelease.Country_ID = 1;
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
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }

                if (isCreationRequest)
                {
                    // process to redirect to order creation
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.OM_Creation;
                    globalData.messageDetail = ResourceModules.OM_Creation + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Create", "OrderCreation");
                }
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
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLineID(int Shop_id)
        {

            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var lineDetail = (from line in db.RS_Lines
                              join partgroup in db.RS_Partgroup on line.Line_ID equals partgroup.Line_ID
                              where line.Shop_ID == Shop_id && partgroup.Order_Create == true && line.Plant_ID == plantId
                              select new
                              {
                                  line.Line_Name,
                                  line.Line_ID
                              }).Distinct().ToList();
            return Json(lineDetail, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetPlatformID(int Shop_id, int Line_id)
        public ActionResult GetPlatformID(int Shop_id, int Line_id)
        {

            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var platformDetail = (from platform in db.RS_OM_Platform
                                  join partgroup in db.RS_Partgroup on platform.Line_ID equals partgroup.Line_ID
                                  where platform.Shop_ID == Shop_id && partgroup.Line_ID == Line_id && partgroup.Order_Create == true && platform.Plant_ID == plantId
                                  select new
                                  {
                                      platform.Platform_Name,
                                      platform.Platform_ID
                                  }).Distinct().ToList();
            return Json(platformDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getOrderType(int Shop_id)
        {

            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var OrderTypeDetail = (from type in db.RS_OM_Order_Type
                                   where type.Shop_ID == Shop_id && type.Plant_ID == plantId
                                   select new
                                   {
                                       type.Order_Type_Name,
                                       type.Order_Type_ID
                                   }).Distinct().ToList();
            return Json(OrderTypeDetail, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Reprint(string serial_no) 
        //{
        //    bool test=true;

        //    return Json(test, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult getRealeasedOrders(int Shop_ID, int Line_id, int Platform_ID, int OrderType)
        {

            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            string ordertype = db.RS_OM_Order_Type.Find(OrderType).Order_Type_Name;

            //get all order in released state
            var orderdata = (from or in db.RS_OM_OrderRelease.Where(m => m.Order_Type.ToLower().Equals(ordertype, StringComparison.CurrentCultureIgnoreCase) && m.Order_Status.Equals("Release", StringComparison.CurrentCultureIgnoreCase) && m.Plant_ID == plantId && m.Shop_ID == Shop_ID && m.Line_ID == Line_id)
                                 //&& !(db.RS_OM_U321_Tactsheet_Orders.Any(p=>p.Order_No==m.Order_No)|| db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(p => p.Order_No == m.Order_No) || db.RS_OM_S201_Tactsheet_Orders.Any(p => p.Order_No == m.Order_No) ))
                             join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantId && m.Shop_ID == Shop_ID && m.Platform_Id == Platform_ID && m.Body_Line == Line_id)
                             on or.Model_Code equals mm.Model_Code into ormm
                             from om in ormm.DefaultIfEmpty()
                             orderby or.Inserted_Date ascending
                             select new CummulativeFields()
                             {
                                 isNOVin = or.is_NoVIN,
                                 Row_ID = or.Row_ID,
                                 Plant_OrderNo = or.Plant_OrderNo,
                                 Order_No = or.Order_No,
                                 Model_Code = or.Model_Code.Trim(),
                                 Model_Description = om.Model_Description.Trim() ?? "",
                                 Auto_Remarks = om.Auto_Remarks ?? "",
                                 Attribution = om.RS_Model_Attribute_Master.Attribution,
                                 Color_code = or.Model_Color ?? "",
                                 BIW_Part_No = om.BIW_Part_No ?? "",
                                 Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
                                 IPMS_Color_Code = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color).Select(m => m.IPMS_Color_Code).FirstOrDefault() ?? "",


                             }).ToList();


            //foreach (var or in orderdata)
            //{
            //    var Attribute = "";

            //    if (or.Attribution_Parameters != null && or.Attribution_Parameters != "")
            //    {
            //        List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(or.Attribution_Parameters, typeof(List<AttributionParameters>));
            //        for (int i = 0; i < attributionParameters.Count; i++)
            //        {
            //            AttributionParameters attributionParameter = attributionParameters[i];
            //            try
            //            {
            //                Convert.ToInt32(attributionParameter.Value);
            //            }
            //            catch (Exception)
            //            {

            //                continue;
            //            }
            //            if (attributionParameter.label.Equals("Attribute", StringComparison.InvariantCultureIgnoreCase))
            //            {
            //                int attrId = Convert.ToInt32(attributionParameter.Value);
            //                Attribute = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
            //                break;
            //                //       attributionParameter.Value;
            //            }

            //        }
            //    }
            //    or.Attribution_Parameters = Attribute;

            //}





            return Json(orderdata, JsonRequestBehavior.AllowGet);

        }
        public class JsonOrderDeleteData
        {
            public string order_No { get; set; }
            public bool is_Deleted { get; set; }
            public string msg { get; set; }


        }
        public class JsonRemoveReleaseOrders
        {
            public string orderno { get; set; }
            public int rowid { get; set; }
        }
        public ActionResult RemoveReleaseOrders(int Shop_id, int Line_Id, int Platform_ID, string OrderType, List<string> orderData)
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            List<JsonOrderDeleteData> jsonOrderDeleteDataobj = new List<JsonOrderDeleteData>();
            JsonOrderDeleteData JsonOrderDeleteObj = new JsonOrderDeleteData();
            List<JsonRemoveReleaseOrders> OrderrowIdsList = new List<JsonRemoveReleaseOrders>();
            try
            {
                OrderrowIdsList = (List<JsonRemoveReleaseOrders>)JsonConvert.DeserializeObject(orderData[0], typeof(List<JsonRemoveReleaseOrders>));

            }
            catch (Exception exe)
            {
                while (exe.InnerException != null)
                {
                    exe = exe.InnerException;
                }
                JsonOrderDeleteObj.is_Deleted = false;
                JsonOrderDeleteObj.msg = "Error: " + exe.Message;
                JsonOrderDeleteObj.order_No = "ERROR";
                jsonOrderDeleteDataobj.Add(JsonOrderDeleteObj);

                return Json(JsonOrderDeleteObj, JsonRequestBehavior.AllowGet);


            }
            //var lockedordersu321 = (from or in OrderrowIdsList
            //                    join tact in db.RS_OM_U321_Tactsheet_Orders on or.orderno.ToUpper() equals tact.Order_No
            //                    where tact.Plant_ID == plantId && tact.Shop_ID == Shop_id && tact.Is_Locked == true
            //                    select tact).ToList();

            //var lockedorderss201 = (from or in OrderrowIdsList
            //                    join tact in db.RS_OM_S201_Tactsheet_Orders on or.orderno.ToUpper() equals tact.Order_No
            //                    where tact.Plant_ID == plantId && tact.Shop_ID == Shop_id && tact.Is_Locked == true
            //                    select tact).ToList();
            //string loackedOrdersNo = "";
            //if (lockedordersu321.Count()>0 || lockedorderss201.Count()>0)
            //{

            //    foreach (var or in lockedordersu321)
            //    {
            //        loackedOrdersNo += or.Order_No + ", ";
            //    }
            //    JsonOrderDeleteObj.is_Deleted = false;
            //    JsonOrderDeleteObj.msg = "Error while Deleting, Some Orders are locked";
            //    JsonOrderDeleteObj.order_No = loackedOrdersNo;
            //    //jsonOrderDeleteDataobj.Add(JsonOrderDeleteObj);
            //    return Json(JsonOrderDeleteObj, JsonRequestBehavior.AllowGet);

            //}
            //if (lockedorderss201.Count() > 0)
            //{

            //    foreach (var or in lockedorderss201)
            //    {
            //        loackedOrdersNo += or.Order_No + ", ";
            //    }
            //    JsonOrderDeleteObj.is_Deleted = false;
            //    JsonOrderDeleteObj.msg = "Error while Deleting, Some Orders are locked";
            //    JsonOrderDeleteObj.order_No = loackedOrdersNo;
            //    //jsonOrderDeleteDataobj.Add(JsonOrderDeleteObj);
            //    return Json(JsonOrderDeleteObj, JsonRequestBehavior.AllowGet);

            //}
            //foreach(var or in OrderrowIdsList)
            //{
            //   var Order_No =  or.orderno;
            //    if (!(db.RS_OM_U321_Tactsheet_Orders.Any(p => p.Order_No == Order_No) || db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(p => p.Order_No == Order_No) || db.RS_OM_S201_Tactsheet_Orders.Any(p => p.Order_No == Order_No))){

            //    }
            //}
            foreach (var orders in OrderrowIdsList)
            {
                var orNumber = orders.orderno;
                var rowId = orders.rowid;
                int orderReleaserowId = Convert.ToInt32(rowId);
                var deleteQty = 1;
                var plantOrderNo = db.RS_OM_OrderRelease.Find(orderReleaserowId).Plant_OrderNo;

                ////for sckiping tactsheet order
                //if ((db.RS_OM_U321_Tactsheet_Orders.Any(p => p.Order_No == orNumber) || db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(p => p.Order_No == orNumber) || db.RS_OM_S201_Tactsheet_Orders.Any(p => p.Order_No == orNumber)))
                //{
                //    continue;
                //}

                RS_OM_Creation mmOmCreation = db.RS_OM_Creation.Where(p => p.Plant_OrderNo == plantOrderNo).Single();
                try
                {
                    ////partial deleting orders according to provided qty
                    DateTime date = DateTime.Today;
                    var todaydate = date.ToString("yyyy-MM-dd");
                    var releaseCount = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Plant_OrderNo == plantOrderNo && m.Order_Status.Equals("Release", StringComparison.CurrentCultureIgnoreCase)).Count();

                    if (deleteQty > releaseCount)
                    {
                        var orderno = db.RS_OM_OrderRelease.Find(orderReleaserowId).Order_No;
                        JsonOrderDeleteObj.is_Deleted = false;
                        JsonOrderDeleteObj.msg = "Order Not available for Delete, orders may be on Hold or Started";
                        JsonOrderDeleteObj.order_No = orderno;


                    }
                    else
                    {
                        if (releaseCount == deleteQty)
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
                            mmOmCreation.Qty -= deleteQty;
                            mmOmCreation.Release_Qty -= deleteQty;
                            mmOmCreation.Updated_Date = DateTime.Now;
                            mmOmCreation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mmOmCreation.Is_Edited = true;
                            db.Entry(mmOmCreation).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        var mmOrderRelease = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Row_ID == orderReleaserowId && m.Order_Status.Equals("Release", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                        mmOrderRelease.Updated_Date = DateTime.Now;
                        mmOrderRelease.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmOrderRelease.Is_Deleted = true;
                        db.Entry(mmOrderRelease).State = EntityState.Modified;
                        db.SaveChanges();


                        //Added by Ajay
                        RS_OM_Deleted_Orders mM_OM_Deleted = new RS_OM_Deleted_Orders();
                        mM_OM_Deleted.Country_ID = mmOrderRelease.Country_ID;
                        mM_OM_Deleted.CUMN = mmOrderRelease.CUMN;
                        mM_OM_Deleted.Inserted_Date = mmOrderRelease.Inserted_Date;
                        mM_OM_Deleted.Inserted_User_ID = mmOrderRelease.Inserted_User_ID;
                        mM_OM_Deleted.Line_ID = mmOrderRelease.Line_ID;
                        mM_OM_Deleted.Model_Code = mmOrderRelease.Model_Code;
                        mM_OM_Deleted.Model_Color = mmOrderRelease.Model_Color != null ? mmOrderRelease.Model_Color : "";
                        mM_OM_Deleted.Order_No = mmOrderRelease.Order_No;
                        mM_OM_Deleted.Order_Start = mmOrderRelease.Order_Start;
                        mM_OM_Deleted.Order_Status = mmOrderRelease.Order_Status;
                        mM_OM_Deleted.Order_Type = mmOrderRelease.Order_Type.Substring(0, 1);
                        mM_OM_Deleted.ORN = mmOrderRelease.ORN;
                        mM_OM_Deleted.partno = mmOrderRelease.partno;
                        mM_OM_Deleted.Plant_ID = mmOrderRelease.Plant_ID;
                        mM_OM_Deleted.Plant_OrderNo = mmOrderRelease.Plant_OrderNo;
                        mM_OM_Deleted.Priority = mmOrderRelease.Priority;
                        mM_OM_Deleted.Remarks = mmOrderRelease.Remarks;
                        mM_OM_Deleted.Order_ID = mmOrderRelease.Row_ID;
                        mM_OM_Deleted.RSN = mmOrderRelease.RSN;
                        mM_OM_Deleted.Series_Code = mmOrderRelease.Series_Code;
                        mM_OM_Deleted.Shop_ID = mmOrderRelease.Shop_ID;
                        mM_OM_Deleted.Updated_Date = mmOrderRelease.Updated_Date;
                        mM_OM_Deleted.Updated_User_ID = mmOrderRelease.Updated_User_ID;
                        mM_OM_Deleted.Is_Active = mmOrderRelease.Is_Active;
                        mM_OM_Deleted.is_Blocked = mmOrderRelease.Is_Blocked;
                        mM_OM_Deleted.Is_Deleted = mmOrderRelease.Is_Deleted;
                        mM_OM_Deleted.Country_ID = mmOrderRelease.Country_ID;
                        db.RS_OM_Deleted_Orders.Add(mM_OM_Deleted);
                        db.SaveChanges();
                        //
                        db.RS_OM_OrderRelease.Remove(mmOrderRelease);
                        db.SaveChanges();

                        generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_OrderRelease", "Row_ID", mmOrderRelease.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                        //var orderno = db.RS_OM_OrderRelease.Find(orderReleaserowId).Order_No;
                        JsonOrderDeleteObj.is_Deleted = true;
                        JsonOrderDeleteObj.msg = "Order Deleted Successfully";
                        JsonOrderDeleteObj.order_No = orNumber;
                    }

                    //return Json(JsonOrderDeleteObj, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }
                    //var orderno = db.RS_OM_OrderRelease.Find(orderReleaserowId).Order_No;
                    JsonOrderDeleteObj.is_Deleted = true;
                    JsonOrderDeleteObj.msg = "Error: " + ex.Message;
                    JsonOrderDeleteObj.order_No = orNumber;


                }
                jsonOrderDeleteDataobj.Add(JsonOrderDeleteObj);
            }

            return Json(jsonOrderDeleteDataobj, JsonRequestBehavior.AllowGet);

        }

        public class JsonNoVinReleaseOrders
        {
            public string orderno { get; set; }
            public int rowid { get; set; }
            public string msg { get; set; }
            public bool is_updated { get; set; }


        }

        public ActionResult updateNoVINStatus(int Shop_id, int Line_id, int Platform_ID, string OrderType, List<string> orderData)
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;

            string ordertype = "";
            List<JsonNoVinReleaseOrders> jsonNoVinReleaseOrdersobj = new List<JsonNoVinReleaseOrders>();
            JsonNoVinReleaseOrders jsonNoVinReleaseObj = new JsonNoVinReleaseOrders();
            List<JsonNoVinReleaseOrders> OrderrowIdsList = new List<JsonNoVinReleaseOrders>();
            List<RS_OM_OrderRelease> orderdata = new List<RS_OM_OrderRelease>();
            List<int> orderdatalist = new List<int>();

            try
            {
                int orderId = Convert.ToInt32(OrderType);
                ordertype = db.RS_OM_Order_Type.Find(orderId).Order_Type_Name;
                OrderrowIdsList = (List<JsonNoVinReleaseOrders>)JsonConvert.DeserializeObject(orderData[0], typeof(List<JsonNoVinReleaseOrders>));

                //orderdata = db.RS_OM_OrderRelease.Where(m => m.Order_Type.ToLower().Equals(ordertype, StringComparison.CurrentCultureIgnoreCase) && m.Order_Status.Equals("release", StringComparison.CurrentCultureIgnoreCase) && m.Plant_ID == plantId && m.Shop_ID == Shop_id && m.is_NoVIN == true
                //  && (db.RS_Model_Master.Any(o => o.Model_Code.Equals(m.Model_Code, StringComparison.CurrentCultureIgnoreCase) && o.Plant_ID == plantId && m.Shop_ID == Shop_id && o.Platform_Id == Platform_ID))).ToList();


                orderdatalist = (from or in db.RS_OM_OrderRelease.Where(m => m.Order_Type.ToLower().Equals(ordertype, StringComparison.CurrentCultureIgnoreCase) && m.Order_Status.Equals("Release", StringComparison.CurrentCultureIgnoreCase) && m.Plant_ID == plantId && m.Shop_ID == Shop_id && m.Line_ID == Line_id && m.is_NoVIN == true)
                                 join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantId && m.Shop_ID == Shop_id && m.Body_Line == Line_id && m.Platform_Id == Platform_ID)
                                 on or.Model_Code equals mm.Model_Code into ormm
                                 from om in ormm.DefaultIfEmpty()
                                 orderby or.Inserted_Date ascending
                                 select or.Row_ID).ToList();




                foreach (var or in orderdatalist)
                {
                    var omOrderObj = db.RS_OM_OrderRelease.Find(or);
                    omOrderObj.is_NoVIN = false;
                    db.Entry(omOrderObj).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch (Exception exe)
            {
                while (exe.InnerException != null)
                {
                    exe = exe.InnerException;
                }
                jsonNoVinReleaseObj.is_updated = false;
                jsonNoVinReleaseObj.msg = "Error: " + exe.Message;
                jsonNoVinReleaseObj.orderno = "ERROR";
                jsonNoVinReleaseOrdersobj.Add(jsonNoVinReleaseObj);

                return Json(jsonNoVinReleaseOrdersobj, JsonRequestBehavior.AllowGet);


            }

            foreach (var orders in OrderrowIdsList)
            {
                var orNumber = orders.orderno;
                var rowId = orders.rowid;
                int orderReleaserowId = Convert.ToInt32(rowId);

                try
                {
                    var orderreleaseObj = db.RS_OM_OrderRelease.Find(orderReleaserowId);
                    orderreleaseObj.is_NoVIN = true;

                    db.Entry(orderreleaseObj).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }
                    jsonNoVinReleaseObj.is_updated = false;
                    jsonNoVinReleaseObj.msg = "Error" + ex.Message;
                    jsonNoVinReleaseObj.orderno = orNumber;


                }
                jsonNoVinReleaseOrdersobj.Add(jsonNoVinReleaseObj);
            }

            return Json(jsonNoVinReleaseOrdersobj, JsonRequestBehavior.AllowGet);

        }
        //public ActionResult GetLineID(int Shop_id)
        //{

        //    var plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //    var lineDetail = (from line in db.RS_Lines
        //                      join partgroup in db.RS_Partgroup on line.Line_ID equals partgroup.Line_ID
        //                      where line.Shop_ID == Shop_id && partgroup.Order_Create == true && line.Plant_ID == plantId
        //                      select new
        //                      {
        //                          line.Line_Name,
        //                          line.Line_ID
        //                      }).Distinct().ToList();
        //    return Json(lineDetail, JsonRequestBehavior.AllowGet);
        //}
    }
}
