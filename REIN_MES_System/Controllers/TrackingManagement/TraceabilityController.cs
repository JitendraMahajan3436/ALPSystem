using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.TrackingManagement
{
    public class TraceabilityController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        GlobalData globalData = new GlobalData();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0;
        string str, mPartID, mPartNo, mVendor, mDate, mShift, mSerialNo, bodysrno, mModel, mStncode, validatemsg, stationhost, status, mdate2, userIpAddress;
        bool stationcnt;
        // GET: Traceability
        public ActionResult Index()
        {
            var machineID = 0;
            try
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];

                }
                globalData.pageTitle = ResourceModules.Traceability;
                globalData.controllerName = "Traceability";
                globalData.actionName = ResourceGlobal.Create;


                ViewBag.GlobalDataModel = globalData;
                stationcnt = false;
                stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
                //stationhost = System.Environment.MachineName;
                ViewBag.HostName = stationhost.Split('.')[0];
                string userhost = stationhost.Split('.')[0];

                var userId = ((FDSession)this.Session["FDSession"]).userId;
                var userName = ((FDSession)this.Session["FDSession"]).userName;
                var station = (from s in db.RS_Stations
                               where s.Station_Host_Name == userhost
                               select new
                               {
                                   s.Linemode,
                                   s.Station_Name
                               }).FirstOrDefault();
                List<Tracebilityview> tracbility = new List<Tracebilityview>();

                ViewData["trecebility"] = tracbility;
                if (station != null)
                {
                    if (station.Linemode == true)
                    {
                        ViewBag.Linemode = station.Linemode;
                    }
                    else
                    {
                        ViewBag.ReworkMode = true;
                    }
                    ViewBag.Station = station.Station_Name;
                }
                else
                {
                    ViewBag.Linemode = true;
                }
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.ToList(), "Plant_ID", "Plant_Name", plantId);
                return View(tracbility);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult OnLoad()
        {
            if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                userIpAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (Request.UserHostAddress.Length != 0)
            {
                userIpAddress = Request.UserHostAddress;
            }
            stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
            //stationhost = System.Environment.MachineName;
            String[] strArr = stationhost.Split(new char[] { '.' });

            //stationhost = System.Environment.MachineName;


            string userhost = strArr[0];
            stationcnt = Validated(userhost);

            if (stationcnt == true)
            {
                return Json(new { stationcnt, userhost, userIpAddress }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { stationcnt, userhost, userIpAddress }, JsonRequestBehavior.AllowGet);
            }
        }
        public bool Validated(string StationHost)
        {

            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var station = db.RS_Stations.Where(m => m.Station_Host_Name == StationHost).ToList();
            //foreach (var item in station)
            //{
            //    if (item.Station_Host_Name == StationHost)
            //    {
            //        stationcnt = true;
            //        break;
            //    }
            //}
            if (station.Count > 0)
                stationcnt = true;

            //ViewBag.Plant_ID = new SelectList(db.RS_Plants.ToList(), "Plant_ID", "Plant_Name", plantId);
            return stationcnt;
        }

        public ActionResult ScanData(string Scanvalue)
        {
            object BOMData;
            string str, mPartID, mPartNo, mVendor, mDate, mShift, mSerialNo, bodysrno, mModel, mStncode, validatemsg, stationhost, datadate, mdate2;
            int cnt, Tcnt = 0;
            int count;
            bool dbcheck;

            stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
            String[] strArr1 = stationhost.Split(new char[] { '.' });
            string userhost = strArr1[0];

           
            String[] strArr = Scanvalue.Split(new char[] { ':' });

          
           // strArr = Scanvalue.Split(new char[] { ':' });
            //mModel = strArr[1].ToString();
            //mSerialNo = strArr[0].ToString();

            if(Scanvalue.Contains(' ') ==true)
            {
                strArr = Scanvalue.Split(new char[] { ' ' });
            }
            else if(Scanvalue.Contains(':') == true)
            {
                strArr = Scanvalue.Split(new char[] { ':' });
            }

            if (Scanvalue == "" || strArr.Length<2)
            {
                validatemsg = "Please Scan Correct Engine Serial No. & Model Barcode first !";
                return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (strArr.Length == 6)
                {
                    TempData["scandata"] = Scanvalue;
                    return Json(strArr, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    var str1 = (from s in db.RS_Stations
                                where s.Station_Host_Name == userhost
                                select new
                                {
                                    s.Station_ID,
                                    s.IsEngine
                                }).FirstOrDefault();

                    if (str1 != null)
                    {
                        if (str1.IsEngine == true)
                        {
                            dbcheck = true;
                            if (strArr.Length == 2)
                            {

                                mSerialNo = strArr[0];
                                mModel = strArr[1];

                                if (mSerialNo.Length > 10 || mSerialNo.Length < 10)
                                {
                                    validatemsg = "Vehicle  Engine srno not match.....!";
                                    return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                                }
                                var stationID = str1.Station_ID;
                                var checkIfValueinTbl = (from t in db.RS_Traceability
                                                         where t.EngineSrNo == mSerialNo && t.Station_ID == stationID
                                                         select new
                                                         {
                                                             t.EngineSrNo
                                                         }).ToList();
                                if (checkIfValueinTbl.Count > 0)
                                {

                                    validatemsg = "Engine Serial Number Already Scanned.....!";
                                    return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);

                                 
                                    //if (mSerialNo.Length > 10 || mSerialNo.Length < 10)
                                    //{
                                    //    validatemsg = "Vehicle  Engine srno not match.....!";
                                    //    return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                                    //}
                                  
                                    //else if (Scanvalue.Length == 17)
                                    //{
                                    //    validatemsg = "Please Scan EngineSr No Now !";
                                    //    return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                                    //}
                                    //else if (mSerialNo.Length == 10)
                                    //{
                                    //    BOMData = PopulateBOM(mModel, userhost);
                                    //    return Json(new { strArr, BOMData }, JsonRequestBehavior.AllowGet);
                                    //}
                                }
                                else
                                {
                                    mModel = strArr[1];
                                    var MODEL = (from m in db.RS_Model_Master
                                                 where m.Model_Code == mModel
                                                 select
                                                 new
                                                 {
                                                     m.Model_Code
                                                 }).FirstOrDefault();
                                    if (MODEL != null)
                                    {
                                        if (mSerialNo.Length == 10)
                                        {
                                            BOMData = PopulateBOM(mModel, userhost);
                                            return Json(new { strArr, BOMData }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            validatemsg = "Please Scan EngineSr No Now !";
                                            return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                                        }
                                    }

                                }



                            }
                            else
                            {
                                mSerialNo = strArr[0];
                                var checkIfValueinTbl = (from t in db.RS_Traceability
                                                         where t.EngineSrNo == mSerialNo
                                                         select new
                                                         {
                                                             t.EngineSrNo,
                                                             t.BOMPARTNO,
                                                             t.ModelCode
                                                         }).FirstOrDefault();
                                if (checkIfValueinTbl != null)
                                {

                                    if (Scanvalue.Substring(0, 3) == "MA1" && Scanvalue.Length == 17)
                                    {
                                        BOMData = PopulateBOM(checkIfValueinTbl.ModelCode, userhost);
                                        return Json(new { strArr, BOMData }, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        validatemsg = "Scanned Value not matching with VIN No Criteria....Please Scan VIN No ONLY... !";
                                        return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    if (Scanvalue.Substring(0, 3) == "MA1" && Scanvalue.Length == 17)
                                    {
                                        validatemsg = "Please Scan EngineSr No Now... !";
                                        return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                                    }
                                    if (Scanvalue.Length == 10)
                                    {
                                        validatemsg = "Please Scan 2D Barcode First!";
                                        return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                        }
                        else
                        {
                            return RedirectToAction("StationCheck", new { Scanvalue, userhost });
                        }

                    }

                    return RedirectToAction("StationCheck", new { Scanvalue, userhost });
                }

            }
        }


        public string[] Scancheck(string Scanvalue)
        {
            String[] strArr = Scanvalue.Split(new char[] { ':' });
            return strArr;
        }

        public ActionResult StationCheck(string Scanvalue, string userhost)
        {
            object BOMData;
            string[] strArr = Scancheck(Scanvalue);
            if (strArr.Length == 2)
            {
                mModel = strArr[1];
                mSerialNo = strArr[0];
                if (mSerialNo.Length > 10 || mSerialNo.Length > 10)
                {
                    validatemsg = "Vehicle  Engine srno not match.....!";
                    return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    BOMData = PopulateBOM(mModel, userhost);
                    return Json(new { strArr, BOMData }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (strArr.Length == 1)
            {
                if (Scanvalue.Substring(0, 3) == "MA1")
                {
                    string modelcode, vinno;
                    int cntString, vinLength, modelLength;
                    cntString = Scanvalue.Length;
                    vinno = Scanvalue.Substring(0, 17);
                    vinLength = vinno.Length;
                    modelLength = (cntString - vinLength);
                    modelcode = Scanvalue.Substring(17, modelLength);
                    var model = (from m in db.RS_Model_Master
                                 where m.Model_Code == modelcode
                                 select new
                                 {
                                     m.Model_Code,
                                     m.Model_Description
                                 }).FirstOrDefault();
                    if (model != null)
                    {
                        BOMData = PopulateBOM(model.Model_Code, userhost);
                        return Json(new { strArr, BOMData }, JsonRequestBehavior.AllowGet);
                    }

                }
                //else if (Scanvalue.Length == 17)
                //{
                //    var Tprint = (from t in db.RS_Stations
                //                  where t.Station_Host_Name == userhost
                //                  select new
                //                  {
                //                      t.Tprint
                //                  }).FirstOrDefault();
                //    if (Tprint != null)
                //    {
                //        if (Tprint.Tprint == true)
                //        {
                //            if (Scanvalue.Substring(0, 3) == "MA1")
                //            {
                //                printBarCode(mSerialNo, userhost);
                //            }
                //        }
                //    }
                //}
            }
            return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult printBarCode(string mSerialNo, string userhost)
        {
            //DateTime date = System.DateTime.Now;

            var date = DateTime.Now.ToString("dd/MMM/yyy HH:mm");
            var Tprint = (from t in db.RS_Stations
                          where t.Station_Host_Name == userhost
                          select new
                          {
                              t.Tprint,
                              t.Station_ID,
                              t.Shop_ID,
                              t.Line_ID,
                          }).FirstOrDefault();


            if (Tprint != null && Tprint.Tprint == true)
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int cnt3 = 0;
                RS_PRN_Tracebility mmPRNObj = new RS_PRN_Tracebility();
                mmPRNObj.Plant_ID = plantId;
                mmPRNObj.Shop_ID = Tprint.Shop_ID;
                mmPRNObj.Line_ID = Tprint.Line_ID;
                mmPRNObj.Station_ID = Tprint.Station_ID;

                var tracbility = (from t in db.RS_Traceability
                                  where t.EngineSrNo == mSerialNo
                                  select new
                                  {
                                      t.EngineSrNo,
                                      t.STATUS,
                                  }).ToList();
                if (tracbility.Count > 0)
                {
                    foreach (var id in tracbility)
                    {
                        if (id.STATUS == true)
                        {
                            cnt3++;
                        }
                    }
                    if (tracbility.Count == cnt3)
                    {
                        var trace = (from t in db.RS_Traceability
                                     where t.EngineSrNo == mSerialNo
                                     select new
                                     {
                                         t.ModelCode,
                                         t.EngineSrNo,
                                     }).FirstOrDefault();
                        if (trace != null)
                        {
                            string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/TRACEABILITYOK-ZM300.prn"));
                            prnFile = prnFile.Replace("EnSr", trace.EngineSrNo.Trim().ToUpper());
                            prnFile = prnFile.Replace("ModelNo", trace.ModelCode.Trim().ToUpper());
                            prnFile = prnFile.Replace("dt", date.ToUpper());
                            mmPRNObj.PRN_Text = prnFile;
                            mmPRNObj.Is_OrderStart = true;
                            mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mmPRNObj.Inserted_Date = DateTime.Now;
                            mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            db.RS_PRN_Tracebility.Add(mmPRNObj);
                            db.SaveChanges();
                            validatemsg = "Print Scussfully !";
                            return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        var trace = (from t in db.RS_Traceability
                                     where t.EngineSrNo == mSerialNo
                                     select new
                                     {
                                         t.ModelCode,
                                         t.EngineSrNo,
                                     }).FirstOrDefault();
                        var trace1 = (from t in db.RS_Traceability
                                      join
        m in db.RS_PartID on
        t.PARTID equals m.PartID
                                      where t.EngineSrNo == mSerialNo && t.STATUS == false
                                      select new
                                      {

                                          t.PARTID,
                                          m.PartIDDescription,

                                      }).ToList();
                        string part1, part2, part3, part4, partdes1, partdes2, partdes3, partdes4;
                        if (trace1.Count() > 3)
                        {
                            part1 = trace1[0].PARTID;
                            part2 = trace1[1].PARTID;
                            part3 = trace1[2].PARTID;
                            part4 = trace1[3].PARTID;
                            partdes1 = trace1[0].PartIDDescription;
                            partdes2 = trace1[1].PartIDDescription;
                            partdes3 = trace1[2].PartIDDescription;
                            partdes4 = trace1[3].PartIDDescription;
                        }
                        else if (trace1.Count() > 2)
                        {
                            part1 = trace1[0].PARTID;
                            part2 = trace1[1].PARTID;
                            part3 = trace1[2].PARTID;
                            part4 = "";
                            partdes1 = trace1[0].PartIDDescription;
                            partdes2 = trace1[1].PartIDDescription;
                            partdes3 = trace1[2].PartIDDescription;
                            partdes4 = "";
                        }
                        else if (trace1.Count() > 1)
                        {
                            part1 = trace1[0].PARTID;
                            part2 = trace1[1].PARTID;
                            part3 = "";
                            part4 = "";
                            partdes1 = trace1[0].PartIDDescription;
                            partdes2 = trace1[1].PartIDDescription;
                            partdes3 = "";
                            partdes4 = "";
                        }
                        else
                        {
                            part1 = trace1[0].PARTID;
                            part2 = "";
                            part3 = "";
                            part4 = "";
                            partdes1 = trace1[0].PartIDDescription;
                            partdes2 = "";
                            partdes3 = "";
                            partdes4 = "";
                        }


                        //List<string> Partid = new List<string>();
                        //List<string> PartDesc = new List<string>();
                        //foreach (var id in trace1)
                        //{
                        //    Partid.Add(id.PARTID);
                        //    PartDesc.Add(id.PartIDDescription);
                        //}

                        string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/TRACEABILITYNotOK-ZM300.prn"));
                        prnFile = prnFile.Replace("EnSr", trace.EngineSrNo.Trim().ToUpper());
                        prnFile = prnFile.Replace("ModelNo", trace.ModelCode.Trim().ToUpper());
                        prnFile = prnFile.Replace("dt", date.ToUpper());
                        prnFile = prnFile.Replace("Part1", part1);
                        prnFile = prnFile.Replace("Part2", part2);
                        prnFile = prnFile.Replace("Part3", part3);
                        prnFile = prnFile.Replace("Part4", part4);
                        prnFile = prnFile.Replace("PartDesc1", partdes1);
                        prnFile = prnFile.Replace("PartDesc2", partdes2);
                        prnFile = prnFile.Replace("PartDesc3", partdes3);
                        prnFile = prnFile.Replace("PartDesc4", partdes4);
                        mmPRNObj.PRN_Text = prnFile;
                        mmPRNObj.Is_OrderStart = true;
                        mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmPRNObj.Inserted_Date = DateTime.Now;
                        mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.RS_PRN_Tracebility.Add(mmPRNObj);
                        db.SaveChanges();
                        validatemsg = "Print Scussfully !";
                        return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
                    }
                }


            }
            return View();
        }
        public object PopulateBOM(string modelcode, string hostname)
        {
            List<Tracebilityview> obj = new List<Tracebilityview>();
            try
            {



                var BOMobj = (from m in db.RS_Model_Master
                              join
                              bm in db.RS_BOM_Item on
                              m.Model_Code equals bm.Model_Code
                              join
                              pm in db.RS_Partmaster on
                              bm.Part_No equals pm.Part_No
                              join
                              pi in db.RS_PartID on
                              pm.PartID equals pi.RowID
                              join
                              partStatId in db.RS_PartID_Station on
                              pi.RowID equals partStatId.PartID
                              join
                              st in db.RS_Stations on
                              partStatId.Station_ID equals st.Station_ID
                              where m.Model_Code == modelcode
                              && st.Station_Host_Name == hostname
                              select new
                              {
                                  pi.PartID,
                                  pi.PartIDDescription,
                                  bm.Part_No,
                                  bm.Model_Code

                              }).Distinct().ToList();


                foreach (var id in BOMobj)
                {
                    Tracebilityview scan = new Tracebilityview(id.PartID, id.PartIDDescription, id.Part_No,
                                             null, null, null, null, null, null, null, null, null);
                    obj.Add(scan);
                }


                return obj;
            }
            catch (Exception ex)
            {
                return obj;
            }

        }


        [HttpPost]
        public JsonResult scan(List<TracebilityPartList> data, bool LineMode, bool ReworkMode)
        {
            string todayYear, val1, val2;

            DateTime todaydate = System.DateTime.Now;
         
            string Scanvalue = TempData["scandata"].ToString();
            List<Tracebilityview> tracbility = new List<Tracebilityview>();
            String[] strArr = Scanvalue.Split(new char[] { ':' });
            mPartID = strArr[0];
            mPartNo = strArr[1];
            mVendor = strArr[2];
            mDate = strArr[3];
            mShift = strArr[4];
            mSerialNo = strArr[5];
            foreach (var id in data)
            {
                if (mPartID == id.Part_Id)
                {

                    if (LineMode == true)
                    {
                        if (mPartNo == (id.Part_NO_BOM).Trim())
                        {
                            status = "YES";
                        }
                        else
                        {
                            status = "NO";
                        }

                    }
                    else if (ReworkMode == true)
                    {

                    }

                    if (mDate.Length > 6)
                    {
                        var day = mDate.Substring(0, 2);
                        var month = mDate.Substring(2, 2);
                        var year = mDate.Substring(4, 4);
                        mdate2 = day + "-" + month + "-" + year;
                        //mdate2 = mDate.Substring(0, 2) + "/" + mDate.Substring(2, 2) + "/" + mDate.Substring(4, 4);
                    }
                    else if (mDate.Length == 6)
                    {
                        mdate2 = mDate.Substring(0, 2) + "-" + mDate.Substring(2, 2) + "-" + mDate.Substring(4, 2);
                    }
                    else if (mDate.Length == 5)
                    {

                        todaydate = todaydate.Date;
                        todayYear = todaydate.Year.ToString();
                        todayYear = (todayYear.Substring(todayYear.Length - 2));
                        var val = mDate.Substring(mDate.Length - 2);
                        if ((todayYear == val) && mDate.Substring(0, 2).Length == 2)
                        {
                            String valnew = "0" + mDate.Substring(2, 1);
                            mdate2 = mDate.Substring(0, 2) + "-" + valnew + "-" + val;
                        }
                    }
                    else if (mDate.Length == 4)
                    {

                        todayYear = todaydate.Year.ToString(mDate);
                        todayYear = (todayYear.Substring(todayYear.Length - 2));
                        var val = mDate.Substring(mDate.Length - 2);
                        if (todayYear == val)
                        {
                            val1 = (mDate.Substring(0, 1));
                            val2 = (mDate.Substring(1, 1));
                            if (val1.Length == 1)
                            {
                                val1 = "0" + val1;
                            }
                            if (val1.Length == 1)
                            {
                                val2 = "0" + val2;
                            }
                            mdate2 = val1 + "-" + val2 + "-" + val;
                        }

                    }
                    DateTime fdate = Convert.ToDateTime(mdate2);
                    var Partdesc = id.Part_Desc.Trim();

                    Tracebilityview scandata = new Tracebilityview(mPartID, id.Part_Desc.Trim(), id.Part_NO_BOM.Trim(),
                                                Scanvalue, status, mVendor, fdate.ToString("dd-MM-yy"), mShift,
                                                 mSerialNo, mPartNo, null, null);
                    tracbility.Add(scandata);

                }
                else
                {
                    Tracebilityview scandata1 = new Tracebilityview(id.Part_Id.Trim(), id.Part_Desc.Trim(), id.Part_NO_BOM.Trim(),
                                            id.Scan_Value, id.Status, id.VCode, id.MFGDATE,
                                             id.MFGSHIFT, id.MFGSRNO, id.PartNo, id.Zpart, id.Error
                                            );
                    tracbility.Add(scandata1);
                }

            }

            return Json(tracbility, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        //public JsonResult Savescandata(bool LineMode,bool ReworkMode,string Engine,string Model)
        //{
        //    plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //    var host = ((FDSession)this.Session["FDSession"]).userHost;
        //    var user = ((FDSession)this.Session["FDSession"]).userId;

        //    try
        //    {
        //        List<TraceabilitySave> tracbility = new List<TraceabilitySave>();
        //        //foreach (var item in data1)
        //        //{
        //        //    var Engine = item.EngineNo;

        //        //    var StationID = db.RS_Stations.Where(m => m.Station_Host_Name == host).Select(m => m.Station_ID).FirstOrDefault();
        //        //    var ShopID = db.RS_Stations.Find(StationID).Shop_ID;

        //        //    if (item.Linemode == true)
        //        //    {

        //        //        var existtracebility = (from t in db.RS_Traceability
        //        //                                where t.EngineSrNo == Engine && t.Station_ID == StationID
        //        //                                select new
        //        //                                {
        //        //                                    t.Trace_ID
        //        //                                }).ToList();

        //        //        if (existtracebility.Count() > 0)
        //        //        {
        //        //            validatemsg = "Engine Serial Number Already Scanned.....!";
        //        //            TraceabilitySave savedata1 = new TraceabilitySave(validatemsg, false);
        //        //            tracbility.Add(savedata1);

        //        //            //return Json(tracbility, JsonRequestBehavior.AllowGet);

        //        //        }
        //        //        else
        //        //        {
        //        //            foreach (var id in item.obj)
        //        //            {
        //        //                RS_Traceability tracebility = new RS_Traceability();
        //        //                tracebility.PARTID = id.Part_Id;
        //        //                tracebility.BOMPARTNO = id.Part_NO_BOM;
        //        //                tracebility.EngineSrNo = Engine;
        //        //                tracebility.Inserted_Date = System.DateTime.Now;
        //        //                tracebility.Inserted_Host = host;
        //        //                tracebility.Inserted_User_ID = user;
        //        //                tracebility.IsLineMode = item.Linemode;
        //        //                tracebility.IsReworkMode = item.Reworkmode;
        //        //                tracebility.MFGDATE = Convert.ToDateTime(id.MFGDATE);
        //        //                tracebility.MFGSHIFT = id.MFGSHIFT;
        //        //                tracebility.ModelCode = item.ModelCode;
        //        //                tracebility.PartNo = id.PartNo;
        //        //                tracebility.Plant_ID = plantId;
        //        //                tracebility.ScanValue = id.Scan_Value;
        //        //                tracebility.Shop_ID = ShopID;
        //        //                tracebility.Station_ID = StationID;
        //        //                if (id.Status == "YES")
        //        //                {
        //        //                    tracebility.STATUS = true;

        //        //                }
        //        //                else
        //        //                {
        //        //                    tracebility.STATUS = false;
        //        //                }
        //        //                tracebility.TDATE = System.DateTime.Today;

        //        //                var spam = DateTime.Now.ToString("HH:mm:ss tt");
        //        //                //var time = TimeSpan.Parse(spam);
        //        //                tracebility.TTIME = spam;
        //        //                tracebility.VCODE = id.VCode;
        //        //                db.RS_Traceability.Add(tracebility);
        //        //                db.SaveChanges();


        //        //                RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == StationID).Single();

        //        //                mmStationTrackingObj.Station_ID = StationID;
        //        //                mmStationTrackingObj.Track_SerialNo = Engine;
        //        //                mmStationTrackingObj.Inserted_Date = DateTime.Now;
        //        //                mmStationTrackingObj.Is_Edited = true;
        //        //                db.Entry(mmStationTrackingObj).State = EntityState.Modified;
        //        //                db.SaveChanges();
        //        //            }
        //        //        }
        //        //        stationhost = System.Environment.MachineName;
        //        //        //String[] LoginSplit = stationhost.Split(new char[] { '-' });
        //        //        string userhost = stationhost;

        //        //        var Tprint = (from t in db.RS_Stations
        //        //                      where t.Station_Host_Name == userhost
        //        //                      select new
        //        //                      {
        //        //                          t.Tprint
        //        //                      }).FirstOrDefault();
        //        //        if (Tprint != null && Tprint.Tprint == true)
        //        //        {
        //        //            printBarCode(Engine, userhost);
        //        //        }
        //        //        validatemsg = "Traceability Part Save Successfully.....!";
        //        //        TraceabilitySave savedata = new TraceabilitySave(validatemsg, true);
        //        //        tracbility.Add(savedata);

        //        //        //return Json(tracbility, JsonRequestBehavior.AllowGet);

        //        //    }
        //        //    else
        //        //    {
        //        //        foreach (var id in item.obj)
        //        //        {
        //        //            var rework = (from t in db.RS_Traceability
        //        //                          where t.EngineSrNo == Engine && t.PARTID == id.Part_Id
        //        //                          select new { t.Trace_ID }).FirstOrDefault();
        //        //            if (rework != null)
        //        //            {
        //        //                RS_Traceability tracebility = db.RS_Traceability.Find(rework.Trace_ID);
        //        //                tracebility.PARTID = id.Part_Id;
        //        //                tracebility.BOMPARTNO = id.Part_NO_BOM;
        //        //                tracebility.EngineSrNo = Engine;
        //        //                tracebility.Inserted_Date = System.DateTime.Now;
        //        //                tracebility.Inserted_Host = host;
        //        //                tracebility.Inserted_User_ID = user;
        //        //                tracebility.IsLineMode = item.Linemode;
        //        //                tracebility.IsReworkMode = item.Reworkmode;
        //        //                tracebility.MFGDATE = Convert.ToDateTime(id.MFGDATE);
        //        //                tracebility.MFGSHIFT = id.MFGSHIFT;
        //        //                tracebility.ModelCode = item.ModelCode;
        //        //                tracebility.PartNo = id.PartNo;
        //        //                tracebility.Plant_ID = plantId;
        //        //                tracebility.ScanValue = id.Scan_Value;
        //        //                if (id.Status == "YES")
        //        //                {
        //        //                    tracebility.STATUS = true;

        //        //                }
        //        //                else
        //        //                {
        //        //                    tracebility.STATUS = false;
        //        //                }
        //        //                tracebility.TDATE = System.DateTime.Today;

        //        //                var spam = DateTime.Now.ToString("HH:mm:ss tt");

        //        //                tracebility.TTIME = spam;
        //        //                tracebility.VCODE = id.VCode;
        //        //                //db.RS_Traceability.Add(tracebility);
        //        //                db.SaveChanges();
        //        //            }
        //        //        }
        //        //        validatemsg = "Traceability Part Updated Successfully.....!";
        //        //        TraceabilitySave savedata = new TraceabilitySave(validatemsg, true);
        //        //        tracbility.Add(savedata);
        //        //    }

        //        //}

        //        validatemsg = "Traceability Part Updated Successfully.....!";
        //        TraceabilitySave savedata = new TraceabilitySave(validatemsg, true);
        //        return Json(tracbility, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        //return RedirectToAction("Index");
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}


        public JsonResult Savescandata(List<PartListModel> data1)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
            //stationhost = System.Environment.MachineName;

            string host = stationhost.Split('.')[0];
            var user = ((FDSession)this.Session["FDSession"]).userId;
            List<TraceabilitySave> tracbility = new List<TraceabilitySave>();
            try
            {
                foreach (var item in data1)
                {
                    var Engine = item.EngineNo;
                    var StationID = db.RS_Stations.Where(m => m.Station_Host_Name.ToUpper() == host.ToUpper()).Select(m => m.Station_ID).FirstOrDefault();


                    var ShopID = db.RS_Stations.Find(StationID).Shop_ID;

                    if (item.Linemode == true)
                    {

                        var existtracebility = (from t in db.RS_Traceability
                                                where t.EngineSrNo == Engine && t.Station_ID == StationID
                                                select new
                                                {
                                                    t.Trace_ID
                                                }).ToList();

                        if (existtracebility.Count() > 0)
                        {

                            validatemsg = "Engine Serial Number Already Scanned.....!";
                            TraceabilitySave savedata1 = new TraceabilitySave(validatemsg, false);
                            tracbility.Add(savedata1);

                            return Json(tracbility, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {

                            foreach (var id in item.obj)
                            {
                                RS_Traceability tracebility = new RS_Traceability();
                                tracebility.PARTID = id.Part_Id;
                                tracebility.BOMPARTNO = id.Part_NO_BOM;
                                tracebility.EngineSrNo = Engine;
                                tracebility.Inserted_Date = System.DateTime.Now;
                                tracebility.Inserted_Host = host;
                                tracebility.Inserted_User_ID = user;
                                tracebility.IsLineMode = item.Linemode;
                                tracebility.IsReworkMode = item.Reworkmode;
                                tracebility.MFGDATE = Convert.ToDateTime(id.MFGDATE);
                                tracebility.MFGSHIFT = id.MFGSHIFT;
                                tracebility.ModelCode = item.ModelCode;
                                tracebility.PartNo = id.PartNo;
                                tracebility.Plant_ID = plantId;
                                tracebility.ScanValue = id.Scan_Value;
                                tracebility.Shop_ID = ShopID;
                                tracebility.Station_ID = StationID;
                                if (id.Status == "YES")
                                {
                                    tracebility.STATUS = true;

                                }
                                else
                                {
                                    tracebility.STATUS = false;
                                }
                                tracebility.TDATE = System.DateTime.Today;

                                var spam = DateTime.Now.ToString("HH:mm:ss tt");
                                //var time = TimeSpan.Parse(spam);
                                tracebility.TTIME = spam;
                                tracebility.VCODE = id.VCode;
                                db.RS_Traceability.Add(tracebility);
                                db.SaveChanges();


                                RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == StationID).Single();

                                mmStationTrackingObj.Station_ID = StationID;
                                mmStationTrackingObj.Track_SerialNo = Engine;
                                mmStationTrackingObj.Inserted_Date = DateTime.Now;
                                mmStationTrackingObj.Is_Edited = true;
                                db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                        }
                        stationhost = System.Environment.MachineName;
                        //String[] LoginSplit = stationhost.Split(new char[] { '-' });
                        string userhost = stationhost;

                        var Tprint = (from t in db.RS_Stations
                                      where t.Station_Host_Name == userhost
                                      select new
                                      {
                                          t.Tprint
                                      }).FirstOrDefault();
                        if (Tprint != null && Tprint.Tprint == true)
                        {
                            printBarCode(Engine, userhost);
                        }
                        validatemsg = "Traceability Part Save Successfully.....!";
                        //TraceabilitySave savedata = new TraceabilitySave(validatemsg, true);
                        //tracbility.Add(savedata);

                        //return Json(tracbility, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        foreach (var id in item.obj)
                        {
                            var rework = (from t in db.RS_Traceability
                                          where t.EngineSrNo == Engine && t.PARTID == id.Part_Id
                                          select new { t.Trace_ID }).FirstOrDefault();
                            if (rework != null)
                            {
                                RS_Traceability tracebility = db.RS_Traceability.Find(rework.Trace_ID);
                                tracebility.PARTID = id.Part_Id;
                                tracebility.BOMPARTNO = id.Part_NO_BOM;
                                tracebility.EngineSrNo = Engine;
                                tracebility.Inserted_Date = System.DateTime.Now;
                                tracebility.Inserted_Host = host;
                                tracebility.Inserted_User_ID = user;
                                tracebility.IsLineMode = item.Linemode;
                                tracebility.IsReworkMode = item.Reworkmode;
                                tracebility.MFGDATE = Convert.ToDateTime(id.MFGDATE);
                                tracebility.MFGSHIFT = id.MFGSHIFT;
                                tracebility.ModelCode = item.ModelCode;
                                tracebility.PartNo = id.PartNo;
                                tracebility.Plant_ID = plantId;
                                tracebility.ScanValue = id.Scan_Value;
                                if (id.Status == "YES")
                                {
                                    tracebility.STATUS = true;

                                }
                                else
                                {
                                    tracebility.STATUS = false;
                                }
                                tracebility.TDATE = System.DateTime.Today;

                                var spam = DateTime.Now.ToString("HH:mm:ss tt");

                                tracebility.TTIME = spam;
                                tracebility.VCODE = id.VCode;
                                //db.RS_Traceability.Add(tracebility);
                                db.SaveChanges();
                            }
                        }
                        validatemsg = "Traceability Part Updated Successfully.....!";
                        // TraceabilitySave savedata = new TraceabilitySave(validatemsg, true);
                        //tracbility.Add(savedata);
                    }

                }

                TraceabilitySave savedata = new TraceabilitySave(validatemsg, true);
                tracbility.Add(savedata);
                return Json(tracbility, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                validatemsg += "In Exception";
                TraceabilitySave savedata = new TraceabilitySave(validatemsg, true);
                tracbility.Add(savedata);
                //return RedirectToAction("Index");
                return Json(tracbility, JsonRequestBehavior.AllowGet);
            }
        }

    }
}