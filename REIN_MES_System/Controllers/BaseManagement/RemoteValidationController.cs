using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.BaseManagement
{
    public class RemoteValidationController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        // GET: RemoteValidation
        public ActionResult RemotePlantName(string Plant_Name, int? Plant_ID)
        {
            var result = false;
            if (Plant_ID == null)
            {
                result = (!db.RS_Plants.Any(pname => pname.Plant_Name.ToLower() == Plant_Name.ToLower()));
            }
            else
            {
                result = (!db.RS_Plants.Any(pname => pname.Plant_Name.ToLower() == Plant_Name.ToLower() && pname.Plant_ID != Plant_ID));
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemotePlantSAPCode(string Plant_Code_SAP, int? Plant_ID)
        {
            var result = false;
            if (Plant_ID == null)
            {
                result = (!db.RS_Plants.Any(psapCode => psapCode.Plant_Code_SAP.ToLower() == Plant_Code_SAP.ToLower()));
            }
            else
            {
                result = (!db.RS_Plants.Any(psapCode => psapCode.Plant_Code_SAP.ToLower() == Plant_Code_SAP.ToLower() && psapCode.Plant_ID != Plant_ID));
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoteShopName(string Shop_Name, int? Shop_ID, int Plant_ID)
        {
            var result = false;
            if (Shop_ID == null)
            {
                result = (!db.RS_Shops.Any(sname => sname.Shop_Name.ToLower() == Shop_Name.ToLower() && sname.Plant_ID == Plant_ID));

            }
            else
            {
                result = (!db.RS_Shops.Any(sname => sname.Shop_Name.ToLower() == Shop_Name.ToLower() && sname.Shop_ID != Shop_ID && sname.Plant_ID == Plant_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult RemoteLineName(int? Line_ID, int? Shop_ID, string Line_Name = "", int plant_Id = 0)
        {
            var result = false;
            if (Line_ID == null && Shop_ID != null)
            {
                result = (!db.RS_Lines.Any(lname => lname.Line_Name.ToLower() == Line_Name.ToLower() && lname.Shop_ID == Shop_ID));

            }
            else
            {
                result = (db.RS_Lines.Any(lname => lname.Line_Name.ToLower() != Line_Name.ToLower() && lname.Line_ID != Line_ID && lname.Shop_ID != Shop_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoteLineCode(int? Line_ID, string Line_Code = "", int? Shop_ID = 0, int Plant_ID = 0)
        {
            var result = false;
            if (Line_ID == null)
            {
                result = (!db.RS_Lines.Any(lname => lname.Line_Code.ToLower() == Line_Code.ToLower() && lname.Shop_ID == Shop_ID));

            }
            else
            {
                result = (!db.RS_Lines.Any(sname => sname.Line_Code.ToLower() == Line_Code.ToLower() && sname.Line_ID != Line_ID && sname.Shop_ID == Shop_ID));

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoteStationName(string Station_Name, int? Shop_ID, int? Line_ID, int? Station_ID)
        {
            var result = false;
            if (Station_ID == null && Line_ID != null && Shop_ID != null)
            {
                result = (!db.RS_Stations.Any(sname => sname.Station_Name.ToLower() == Station_Name.ToLower() && sname.Shop_ID == Shop_ID && sname.Line_ID == Line_ID));

            }
            else
            {
                result = (db.RS_Stations.Any(sname => sname.Station_Name != Station_Name && sname.Station_ID != Station_ID && sname.Shop_ID != Shop_ID && sname.Line_ID != Line_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoteStationIPAddress(string Station_IP_Address, int? Station_ID)
        {
            var result = false;
            if (Station_ID == null)
            {
                result = (!db.RS_Stations.Any(sname => sname.Station_IP_Address.ToLower() == Station_IP_Address.ToLower()));

            }
            else
            {
                result = (db.RS_Stations.Any(sname => sname.Station_IP_Address != Station_IP_Address && sname.Station_ID != Station_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoteActivityName(int? Activity_ID, string Activity_Name = "")
        {
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var result = false;
            if (Activity_ID == null)
            {
                result = (!db.RS_PM_Activity.Any(name => name.Activity_Name.ToLower() == Activity_Name.ToLower() && name.Plant_ID == plantID));

            }
            else
            {
                result = (!db.RS_PM_Activity.Any(name => name.Activity_Name.ToLower() == Activity_Name.ToLower() && name.Activity_ID != Activity_ID && name.Plant_ID == plantID));
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemotePartName(int? Maintenance_Part_ID, string Part_Name = "")
        {
            var result = false;
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            if (Maintenance_Part_ID == null)
            {
                result = (!db.RS_Maintenance_Part.Any(name => name.Part_Name.ToLower() == Part_Name.ToLower() && name.Plant_ID == plantID));

            }
            else
            {
                result = (!db.RS_Maintenance_Part.Any(name => name.Part_Name.ToLower() == Part_Name.ToLower() && name.Maintenance_Part_ID != Maintenance_Part_ID && name.Plant_ID == plantID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemotePartGroupCode(string Partgroup_Code, decimal? Partgroup_ID = null, int? Plant_ID = null)
        {
            //change type of partgroup id in new db
            var result = false;
            if (Partgroup_ID == null)
            {
                result = (!db.RS_Partgroup.Any(sname => sname.Partgroup_Code.ToLower() == Partgroup_Code.ToLower() && sname.Plant_ID == Plant_ID));

            }
            else
            {
                result = (db.RS_Partgroup.Any(sname => sname.Partgroup_Code != Partgroup_Code && sname.Partgroup_ID != Partgroup_ID && sname.Plant_ID == Plant_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult RemoteRackAreaName(int? Rack_Area_ID, int? Shop_ID, string Rack_Area_Name = "")
        //{
        //    var result = false;
        //    if (Rack_Area_ID == null && Shop_ID != null)
        //    {
        //        result = (!db.MM_Rack_Area.Any(astring => astring.Rack_Area_Name.ToLower() == Rack_Area_Name.ToLower() && astring.Shop_ID == Shop_ID));

        //    }
        //    else
        //    {
        //        result = (!db.MM_Rack_Area.Any(astring => astring.Rack_Area_Name.ToLower() == Rack_Area_Name.ToLower() && astring.Rack_Area_ID != Rack_Area_ID && astring.Shop_ID == Shop_ID));

        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        #region Rack
        //public ActionResult RemoteRackName(int? Rack_ID, int? Rack_Area_ID, int? Shop_ID, string Rack_Name = "")
        //{
        //    var result = false;
        //    if (Rack_ID == null && Shop_ID != null && Rack_Area_ID != null)
        //    {
        //        result = (!db.MM_Rack.Any(astring => astring.Rack_Name.ToLower() == Rack_Name.ToLower() && astring.Shop_ID == Shop_ID
        //            && astring.Rack_Area_ID == Rack_Area_ID));
        //    }
        //    else
        //    {
        //        result = (!db.MM_Rack.Any(astring => astring.Rack_Name.ToLower() == Rack_Name.ToLower() && astring.Rack_ID != Rack_ID && astring.Shop_ID == Shop_ID
        //             && astring.Rack_Area_ID == Rack_Area_ID));
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult PartWarningCapasityValidation(int Capacity, int Warning_Capacity)
        {
            var result = false;
            if (Capacity >= Warning_Capacity)
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult DuplicatepartInRackArea(string partNo, int? partAreaID)
        //{
        //    var result = false;
        //    if (partNo != null && partAreaID != null)
        //    {
        //        result = (db.MM_Rack_Cells.Any(astring => astring.Part_No.ToLower() == partNo.ToLower() && astring.Rack_Area_ID == partAreaID));
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult RowValidation(int Total_Row)
        {
            var result = false;
            string size = Convert.ToString(Total_Row);
            if (size.Length < 3)
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ColValidation(int Total_Column)
        {
            var result = false;
            string size = Convert.ToString(Total_Column);
            if (size.Length < 3)
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult CheckDuplicateSequence(int? Rack_Area_ID, int? Shop_ID, int? Sequence)
        //{
        //    var result = false;
        //    if (Rack_Area_ID != null && Shop_ID != null && Sequence != null)
        //    {
        //        result = (!db.MM_Rack.Any(astring => astring.Sequence == Sequence && astring.Rack_Area_ID == Rack_Area_ID && astring.Shop_ID == Shop_ID));

        //    }
        //    else
        //    {
        //        result = true;
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        #endregion
        #region KittingZone
        public ActionResult RemoteKittingZoneName(int? Shop_ID, int? Kitting_Zone_ID, string Zone_Name = "")
        {
            var result = false;
            if (Kitting_Zone_ID == null && Shop_ID != null)
            {
                result = (!db.RS_Kitting_Zone.Any(astring => astring.Zone_Name.ToLower() == Zone_Name.ToLower() && astring.Shop_ID == Shop_ID));
            }
            else
            {
                result = (!db.RS_Kitting_Zone.Any(astring => astring.Zone_Name.ToLower() == Zone_Name.ToLower() && astring.Shop_ID == Shop_ID
                     && astring.Kitting_Zone_ID != Kitting_Zone_ID));
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region MachineController
        public ActionResult RemoteMachineNumber(string Machine_Number, int? Machine_ID, int Plant_ID)
        {
            var result = false;
            if (Machine_ID == null)
            {
                result = (!db.RS_MT_Machines.Any(m => m.Machine_Number.ToLower() == Machine_Number.ToLower() && m.Plant_ID == Plant_ID));
            }
            else
            {
                result = (!db.RS_MT_Machines.Any(m => m.Machine_Number.ToLower() == Machine_Number.ToLower() && m.Machine_ID != Machine_ID && m.Plant_ID == Plant_ID));
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoteMachineName(string Machine_Name, int? Machine_ID, int Plant_ID)
        {
            var result = false;
            if (Machine_ID == null)
            {
                result = (!db.RS_MT_Machines.Any(m => m.Machine_Name.ToLower() == Machine_Name.ToLower() && m.Plant_ID == Plant_ID));
            }
            else
            {
                result = (!db.RS_MT_Machines.Any(m => m.Machine_Name.ToLower() == Machine_Name.ToLower() && m.Machine_ID != Machine_ID && m.Plant_ID == Plant_ID));
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region RemoteEmployeeName
        //public ActionResult RemoteEmployeeName(string Employee_Name, int? Emp_Level)
        //{
        //    var result = false;
        //    result = (!db.MM_Notification_Config.Any(sname => sname.Employee_Name.ToLower() == Employee_Name.ToLower() && sname.Emp_Level == Emp_Level));
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region ModuleName


        public ActionResult RemoteModuleName(string Module_Name, int? Module_ID)
        {
            var result = false;
            if (Module_ID == null)
            {
                result = (!db.RS_Notification_Modules.Any(sname => sname.Module_Name.ToLower() == Module_Name.ToLower()));

            }
            else
            {
                result = (!db.RS_Notification_Modules.Any(sname => sname.Module_Name.ToLower() == Module_Name.ToLower() && sname.Module_ID != Module_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Analyst

        //public ActionResult RemoteAnalyticsName(string Analytics_Name, int? Analytics_ID)
        //{
        //    bool result = false;
        //    if (Analytics_ID == null || Analytics_ID == 0)
        //    {
        //        result = (!db.MM_Quality_Analytics.Any(pname => pname.Analytics_Name.ToLower() == Analytics_Name.ToLower()));
        //    }
        //    else
        //    {
        //        result = (!db.MM_Quality_Analytics.Any(pname => pname.Analytics_Name.ToLower() == Analytics_Name.ToLower() && pname.Analytics_ID != Analytics_ID));
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}

        public class Result
        {
            public bool success;
        }

        #endregion

        public ActionResult RemoteStyleCode(int? StyleCode_ID, string Style_Code = "")
        {
            var result = false;
            if (StyleCode_ID == null)
            {
                result = (!db.RS_Style_Code.Any(astring => astring.Style_Code.ToLower().Trim() == Style_Code.ToLower().Trim()));

            }
            else
            {
                result = (!db.RS_Style_Code.Any(astring => astring.Style_Code.ToLower().Trim() == Style_Code.ToLower().Trim() && astring.StyleCode_ID != StyleCode_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult RemoteStyleCodeCheckPoint(int? Check_Point_ID, int? Plant_ID, string Check_Point_Name = "")
        //{
        //    var result = false;
        //    if (Check_Point_ID == null && Plant_ID != null)
        //    {
        //        result = (!db.MM_StyleCode_CheckPoints.Any(astring => astring.Check_Point_Name.ToLower() == Check_Point_Name.ToLower() && astring.Plant_ID == Plant_ID));

        //    }
        //    else
        //    {
        //        result = (!db.MM_StyleCode_CheckPoints.Any(astring => astring.Check_Point_Name.ToLower() == Check_Point_Name.ToLower() && astring.Check_Point_ID != Check_Point_ID && astring.Plant_ID == Plant_ID));

        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult RemoteStyleCodeCheckList(int? CheckList_ID, int? Plant_ID, string CheckList_Name = "")
        //{
        //    var result = false;
        //    if (CheckList_ID == null && Plant_ID != null)
        //    {
        //        result = (!db.MM_StyleCode_CheckList.Any(astring => astring.CheckList_Name.ToLower() == CheckList_Name.ToLower() && astring.Plant_ID == Plant_ID));

        //    }
        //    else
        //    {
        //        result = (!db.MM_StyleCode_CheckList.Any(astring => astring.CheckList_Name.ToLower() == CheckList_Name.ToLower() && astring.CheckList_ID != CheckList_ID && astring.Plant_ID == Plant_ID));

        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult IsAlreadyExistPlatformName(int? Platform_ID, int? Shop_ID, int? Line_ID, string Platform_Name)
        {
            var result = false;
            if (Platform_ID == null && Shop_ID != null && Line_ID != null)
            {
                result = (!db.RS_OM_Platform.Any(bstring => bstring.Platform_Name.ToLower() == Platform_Name.ToLower() && bstring.Shop_ID == Shop_ID
                    && bstring.Line_ID == Line_ID));

            }
            else
            {
                result = (!db.RS_OM_Platform.Any(bstring => bstring.Platform_Name.ToLower() == Platform_Name.ToLower() && bstring.Platform_ID != Platform_ID && bstring.Shop_ID == Shop_ID
                     && bstring.Line_ID == Line_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemotePartGroupName(string Partgrup_Desc, int? Partgroup_ID, int? Shop_ID, int Plant_ID)
        {
            var result = false;
            if (Partgroup_ID == null && Shop_ID != null)
            {
                result = (!db.RS_Partgroup.Any(pname => pname.Partgrup_Desc.ToLower() == Partgrup_Desc.ToLower() && pname.Shop_ID == Shop_ID && pname.Plant_ID == Plant_ID));

            }
            else
            {
                result = (!db.RS_Partgroup.Any(pname => pname.Partgrup_Desc == Partgrup_Desc && pname.Shop_ID == Shop_ID && pname.Partgroup_ID != Partgroup_ID && pname.Plant_ID == Plant_ID));

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsPartNoValid(string Part_No, Boolean Is_ParentModel_Manifest)
        {
            if (Is_ParentModel_Manifest == false)
            {
                if (String.IsNullOrWhiteSpace(Part_No))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(db.RS_PartList_View.Any(a => a.Part_No == Part_No), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #region Colour
        //public ActionResult RemoteColourName(string Colour_Desc, int? Row_id, int Plant_ID)
        //{
        //    var result = false;
        //    if (Row_id == null)
        //    {
        //        result = (!db.RS_Colour.Any(name => name.Colour_Desc.ToLower() == Colour_Desc.ToLower() && name.Plant_ID == Plant_ID));

        //    }
        //    else
        //    {
        //        result = (!db.RS_Colour.Any(name => name.Colour_Desc.ToLower() == Colour_Desc.ToLower() && name.Row_id != Row_id && name.Plant_ID == Plant_ID));

        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        #endregion
    }
}