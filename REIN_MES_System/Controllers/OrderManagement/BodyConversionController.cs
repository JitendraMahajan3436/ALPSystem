using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class BodyConversionController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        string stationhost, validatemsg, userIpAddress;

        public ActionResult NoPermission()
        {
            return View();
        }

        // GET: BodyConversion
        public ActionResult Index()
        {
            try
            {
                int userId = ((FDSession)this.Session["FDSession"]).userId;
                if(userId == 4 || userId == 319 || userId == 111 || userId == 368 || userId == 199 || userId == 369)
                {

                }
                else
                {
                    return View("NoPermission");
                }
                //int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                globalData.pageTitle = "Body Conversion";
                globalData.controllerName = "BodyConversion";
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Model_Code = new SelectList(db.RS_Model_Master.OrderBy(c => c.Model_Code), "Model_ID", "Model_Code");
                ViewBag.Color_Code = new SelectList(db.RS_Colour.OrderBy(c => c.Colour_ID), "Row_ID", "Colour_ID");
                ViewBag.Varient_Code = new SelectList(db.RS_Model_Master.OrderBy(c => c.Varient), "Model_ID", "Varient");
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult getinfo(string BodySrNo)
        {
            var Shop_ID = db.RS_OM_Order_List.Where(m => m.Serial_No == BodySrNo).Select(m => m.Shop_ID).FirstOrDefault();
            var Line_ID = db.RS_OM_Order_List.Where(m => m.Serial_No == BodySrNo).Select(m => m.Line_ID).FirstOrDefault();

            var St = (from data in db.RS_OM_Order_List
                      join models in db.RS_OM_OrderRelease on data.Order_No equals models.Order_No
                      join models1 in db.RS_Model_Master on data.Model_Code equals models1.Model_Code
                      where data.Serial_No == BodySrNo
                      select new
                      {
                          ModelCode = data.Model_Code,
                          Color_Code = models.Model_Color,
                          Remarks = models1.Auto_Remarks,
                          Varient = models1.BIW_Part_No
                      }).FirstOrDefault();
            var ModelS = (from mod in db.RS_Model_Master
                          where mod.Shop_ID == Shop_ID
                          select new
                          {
                              Id = mod.Model_ID,
                              Value = mod.Model_Code
                          }).ToList();
            var Colors = (from mod in db.RS_Colour
                          where mod.Plant_ID == 3
                          select new
                          {
                              Id = mod.Row_ID,
                              Value = mod.Colour_ID
                          }).ToList();
            var Varient = (from mod in db.RS_Model_Master
                           where mod.Shop_ID == Shop_ID
                           select new
                           {
                               //Id = mod.Model_ID,
                               Value = mod.BIW_Part_No
                           }).Distinct().ToList();
            bool ispaint = false;

            var IsPaintout = db.RS_OM_Order_Status_Live.Where(m => m.Serial_No == BodySrNo && m.Action_Status == "Paint Out").Select(m => m.Order_Status_ID).FirstOrDefault();

            if (IsPaintout > 0)
            {
                ispaint = true;
            }


            return Json(new { St = St, Colors = Colors, ModelS = ModelS, Varient = Varient, ispaint = ispaint }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorCodeByModelCode(string modelCode)
        {
            var plantid = ((FDSession)this.Session["FDSession"]).plantId;
            var obj_MM_ModelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode && m.Plant_ID == plantid).FirstOrDefault();
         
            if (obj_MM_ModelMaster.Is_Colour_Applicable == true)
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

        public ActionResult GetAutoRemarkByModelCode(string modelCode)
        {   
            var obj_MM_ModelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode).FirstOrDefault();

            return Json(obj_MM_ModelMaster.Auto_Remarks, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveBodyConversionModelData(string bodyno, string O_Model_Code, string N_Model_Code,string O_Color_Code, string N_Color_Code, string O_Remark, string N_Remarks, string Reason, bool IsModel, string C_Varient_Code, bool paintout)
        {
            bool success = false;
            if (IsModel == true)
            {
                var NewBodySrNo = bodyno;
                if(N_Color_Code != "Select Color")
                {
                    NewBodySrNo = bodyno.Substring(0, 8);
                    NewBodySrNo += N_Color_Code;
                }
                
                RS_Body_Conversion mM_Body = new RS_Body_Conversion();
                mM_Body.Body_Sr_No = bodyno.ToUpper();
                mM_Body.o_Body_Sr_No = bodyno.ToUpper();
                mM_Body.n_Body_Sr_No = NewBodySrNo.ToUpper();
                mM_Body.o_Model_Code = O_Model_Code;
                mM_Body.n_Model_Code = N_Model_Code;

                mM_Body.o_Color = O_Color_Code;
                if(N_Color_Code != "Select Color")
                {
                    mM_Body.n_Color = N_Color_Code;
                }
                else
                {
                    N_Color_Code = O_Color_Code;
                    mM_Body.n_Color = O_Color_Code;
                }
                mM_Body.Is_Model_Code = IsModel;
                mM_Body.o_Remarks = O_Remark;
                mM_Body.n_Remarks = N_Remarks;
                mM_Body.Reason = Reason;
                mM_Body.Is_Proceed = false;
                mM_Body.Is_Purgeable = false;
                mM_Body.Is_Transferred = false;
                mM_Body.o_Biw_Part_No = C_Varient_Code;
                mM_Body.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_Body.Inserted_Date = DateTime.Now;
                mM_Body.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.RS_Body_Conversion.Add(mM_Body);
                db.SaveChanges();


                //Update RS_OM_Order_List table
                var Row_ID = db.RS_OM_Order_List.Where(m => m.Serial_No == bodyno).Select(m => m.Order_ID).FirstOrDefault();
                RS_OM_Order_List RS_OM_Order_List = db.RS_OM_Order_List.Find(Row_ID);
                RS_OM_Order_List.Model_Code = N_Model_Code;
                RS_OM_Order_List.partno = N_Model_Code;
                RS_OM_Order_List.Is_Edited = true;
                RS_OM_Order_List.Serial_No = NewBodySrNo.ToUpper();
                db.Entry(RS_OM_Order_List).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //Update RS_OM_OrderRelease
                var RowID = db.RS_OM_OrderRelease.Where(m => m.Order_No == RS_OM_Order_List.Order_No).Select(m => m.Row_ID).FirstOrDefault();
                RS_OM_OrderRelease RS_OM_OrderRelease = db.RS_OM_OrderRelease.Find(RowID);
                RS_OM_OrderRelease.Model_Code = N_Model_Code;
                RS_OM_OrderRelease.Model_Color = N_Color_Code;
                RS_OM_OrderRelease.Is_Edited = true;
                db.Entry(RS_OM_OrderRelease).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //Update RS_OM_Order_Status_Live
                var OrderStatusID = db.RS_OM_Order_Status_Live.Where(m => m.Serial_No == bodyno).Select(m => m.Order_Status_ID).FirstOrDefault();
                if (OrderStatusID > 0)
                {
                    RS_OM_Order_Status_Live RS_OM_Order_Status_Live = db.RS_OM_Order_Status_Live.Find(OrderStatusID);
                    RS_OM_Order_Status_Live.Serial_No = NewBodySrNo.ToUpper();
                    RS_OM_Order_Status_Live.Updated_Date = DateTime.Now;
                    RS_OM_Order_Status_Live.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    RS_OM_Order_Status_Live.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_OM_Order_Status_Live.Is_Edited = true;
                    db.Entry(RS_OM_Order_Status_Live).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                validatemsg = "Body Conversion Done Successfully ";
                success = true;
            }
            else
            {

            }
            return Json(new { Message = validatemsg, Success = success }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveBodyConversionVariantData(string bodyno, string O_Model_Code, string O_Color_Code, string O_Remark, string Reason, bool IsModel, string C_Varient_Code, string N_Varient_Code)
        {
            bool success = false;
            if (IsModel == false)
            {
                RS_Body_Conversion mM_Body = new RS_Body_Conversion();
                mM_Body.Body_Sr_No = bodyno;
                mM_Body.o_Body_Sr_No = bodyno;
                mM_Body.n_Body_Sr_No = bodyno;
                mM_Body.o_Model_Code = O_Model_Code;
                //mM_Body.n_Model_Code = N_Model_Code;

                mM_Body.o_Color = O_Color_Code;

                //mM_Body.n_Color = N_Color_Code;
                mM_Body.Is_Model_Code = IsModel;
                mM_Body.o_Remarks = O_Remark;
                //mM_Body.n_Remarks = N_Remarks;
                mM_Body.Reason = Reason;
                mM_Body.Is_Proceed = false;
                mM_Body.Is_Purgeable = false;
                mM_Body.Is_Transferred = false;
                mM_Body.o_Biw_Part_No = C_Varient_Code;
                mM_Body.n_Biw_Part_No = N_Varient_Code;
                mM_Body.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_Body.Inserted_Date = DateTime.Now;
                mM_Body.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.RS_Body_Conversion.Add(mM_Body);

                db.SaveChanges();

                validatemsg = "Body Sr No Convert Varient Code Successfully ";
                success = true;

            }
            else
            {

            }
            return Json(new { Message = validatemsg, Success = success }, JsonRequestBehavior.AllowGet);
        }
       
    }
}