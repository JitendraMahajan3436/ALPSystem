using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.TrackingManagement
{
    public class ErrorProofingController : BaseController
    {
        int plantId = 0, shopId = 0, lineId = 0;

        ErrorProofing errorProofingObj = new ErrorProofing();

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        //
        // GET: /ErrorProofing/
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult getErrorProofingGeneologyParts(String serialNo, int stationId)
        {
            try
            {
                int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
                String modelCode = errorProofingObj.getModelcodeBySerialNumber(serialNo);
                RS_Partmaster[] mmPartmasterObj = errorProofingObj.getStationPartList(stationId, modelCode);

                return Json(mmPartmasterObj, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult getErrorProofingGeneologyPartsByModelCode(String modelCode, int stationId)
        {
            try
            {                
                //RS_Partmaster[] mmPartmasterObj = errorProofingObj.getStationPartList(stationId, modelCode);
                var partMaster = (from mmPartMaster in db.RS_Partmaster
                                  where (mmPartMaster.Genealogy == true || mmPartMaster.Error_Proofing == true) && mmPartMaster.Station_ID == stationId &&
                                  (from mmBomItem in db.RS_BOM_Item where mmBomItem.Model_Code == modelCode select mmBomItem.Part_No).Contains(mmPartMaster.Part_No)
                                  select new
                                  {
                                      Plant_ID=mmPartMaster.Plant_ID,
                                      Shop_ID=mmPartMaster.Shop_ID,
                                      Line_ID=mmPartMaster.Line_ID,
                                      Station_ID=mmPartMaster.Station_ID,

                                      Part_No=mmPartMaster.Part_No
                                  });
                
                return Json(partMaster.ToList(), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult checkErrorProofing(String inputChildSerialNo, String serialNo)
        {
            try
            {
                int plantID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).plantId);
                String modelCode=errorProofingObj.getModelcodeBySerialNumber(serialNo);

                String childPartNumber=errorProofingObj.getModelcodeBySerialNumber(inputChildSerialNo);

                // process to check the child part number is present in serial number bom

                bool res=errorProofingObj.isErrorProofingOK(modelCode,childPartNumber);

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}