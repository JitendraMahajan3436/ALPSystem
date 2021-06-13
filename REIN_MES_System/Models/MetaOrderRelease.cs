using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaOrderRelease))]
    public partial class RS_OM_OrderRelease
    {
        public List<string> CUMNDATA { get; set; }
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        General generalHelper = new General();
        public String oldSeriesCode = "";
        //  public string Series_Code { get; set; }
        /*               Action Name               : getTotalOrderReleasedByDate
         *               Description               : Function used to find date wise Total Order Release. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : int
         *               Revision                  : 1
        */
        public int getTotalOrderReleasedByDate(int shopId, int Line_ID)
        {
            try
            {
                DateTime time = DateTime.Now;             // Use current time.
                string format = "MM-dd-yy";   // Use this format.
                string dtObj = time.ToString(format);

                String query = "select * from RS_OM_OrderRelease where CONVERT(VARCHAR(10),Inserted_Date,10)='" + dtObj + "' and Shop_Id='" + shopId + "' and Line_ID='" + Line_ID + "'";
                IEnumerable<RS_OM_OrderRelease> totalRelease = db.Database.SqlQuery<RS_OM_OrderRelease>(query);

                query = "select * from RS_OM_Deleted_Orders where CONVERT(VARCHAR(10),Inserted_Date,10)='" + dtObj + "' and Shop_Id='" + shopId + "' and Line_ID='" + Line_ID + "'";
                IEnumerable<RS_OM_Deleted_Orders> totalDeletedOrders = db.Database.SqlQuery<RS_OM_Deleted_Orders>(query);


                return totalRelease.Count() + totalDeletedOrders.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /*               Action Name               : generateOrderNumber
         *               Description               : Function used to genrating the order numbr for Order Release form. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : cnt,partGroupDesc
         *               Return Type               : int
         *               Revision                  : 1
        */
        public String generateOrderNumber(int cnt, string model_code)
        {
            try
            {
                String number = "";
                number = "N";

                //RS_Partgroup partGroup = db.RS_Partgroup.Where(p => p.Partgroup_ID == partGroupID).Single();

                //if (partGroup != null)
                //{
                //    number = number + partGroup.Partgroup_Code;
                //}

                //find year code

                var platfrom_Code = (from model in db.RS_Model_Master
                                     join platfrom in db.RS_OM_Platform
                on model.Platform_Id equals platfrom.Platform_ID
                                     where model.Model_Code == model_code
                                     select new { code = platfrom.Serial_No_Code }).ToList();
                if (platfrom_Code != null)
                {
                    number += platfrom_Code[0].code;
                }

                int year = @DateTime.Now.Year;

                RS_Year series = db.RS_Year.Where(p => p.Year == year && p.Identifier_ID == 1).Single();
                string year_code = series.Year_Code.Trim();
                number = number + year_code;

                //find month code   
                int month = @DateTime.Now.Month;

                RS_Month months = db.RS_Month.Where(p => p.Month_No == month && p.Identifier_ID == 1).Single();
                string month_code = months.Month_Code.Trim();

                number = number + month_code;

                int day = DateTime.Now.Day;
                if (day < 10)
                {
                    number = number + "0" + day.ToString();
                }

                else
                {
                    number = number + "" + day.ToString();
                }

                if (cnt < 10)
                {
                    number = number + "000" + cnt.ToString();
                }
                else if (cnt >= 10 && cnt <= 99)
                {
                    number = number + "00" + cnt.ToString();
                }
                else
                    if (cnt > 99 && cnt < 999)
                {
                    number = number + "0" + cnt.ToString();
                }
                else if (cnt > 999 && cnt < 9999)
                {
                    number = number + "" + cnt.ToString();
                }
                else
                if (cnt > 9999 && cnt < 99999)
                {
                    number = number + " " + cnt.ToString();
                }

                if (number.Length == 13)
                {
                    number = number.Remove(8, 1);
                }

                return number;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /*               Action Name               : generateDSNNumber
         *               Description               : Function used to genrating the DSN number for Order Release form. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : plantId,shopId,date
         *               Return Type               : int
         *               Revision                  : 1
        */
        public decimal generateORNNumber(int plantId, DateTime insert_date)
        {
            //string dsnNumber = "";
            //DateTime date = DateTime.Now.Date;   // Use current Datetime.

            try
            {
                DateTime today = DateTime.Today;
                decimal orn = db.RS_OM_OrderRelease.Where(p => p.Plant_ID == plantId && p.Order_Status == "Release" && DbFunctions.TruncateTime(p.Inserted_Date) == today).Max(p => p.ORN).GetValueOrDefault(0);
                orn++;
                return orn;
            }
            catch (Exception ex)
            {
                generalHelper.addMetaException(ex, "RS_OM_OrderRelease", "generateORNNumber", 1);
                return 1;
            }

            //var item = db.RS_OM_OrderRelease.AsEnumerable()
            //    .Where(c => c.Plant_ID == plantId && c.Inserted_Date.ToString("dd.MM.yy") == insert_date.ToString("dd.MM.yy")).Select(c => new { c.RSN }).ToList();
            //if (item.Count > 0)
            //{
            //    var max_configno = db.RS_OM_OrderRelease.AsEnumerable()
            //                         .Where(c => c.Inserted_Date.ToString("dd.MM.yy") == insert_date.ToString("dd.MM.yy"))
            //                         .Max(r => r.ORN);

            //    string s = max_configno.ToString();
            //    int rsn = Convert.ToInt16(s);
            //    int rsn_no = rsn + 1;
            //    dsnNumber = Convert.ToString(rsn_no);
            //}
            //else
            //{
            //    string s = "1";
            //    dsnNumber = s;;
            //}


            //return dsnNumber;
        }


        /*               Action Name               : generateRSNNumber
         *               Description               : Function used to genrating the DSN number for Order Release form. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : plantId,shopId,date
         *               Return Type               : int
         *               Revision                  : 1
        */

        public decimal generateRSNNumber(int plantId, int shopId, DateTime? PlannedDate)
        {
            try
            {
                decimal rsn = 0;
                decimal mrsn = db.RS_OM_OrderRelease.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId).Max(q => (decimal?)(q.RSN)) ?? 0;
                if (mrsn != null)
                {
                    //rsn = db.RS_OM_OrderRelease.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId).Max(p => p.RSN);
                    mrsn++;
                }
                else
                {
                    mrsn = 1;
                }
                return mrsn;
                //if (PlannedDate != null)
                //{
                //    decimal rsn = db.RS_OM_OrderRelease.Where(p =>  p.Plant_ID == plantId && p.Shop_ID == shopId && DbFunctions.TruncateTime(p.Planned_Date.Value.Date) == DbFunctions.TruncateTime(PlannedDate.Value.Date)).Max(p => p.RSN);
                //    rsn++;
                //    return rsn;
                //}
                //else
                //{
                //    decimal rsn = db.RS_OM_OrderRelease.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId).Max(p => p.RSN)== null? 0: db.RS_OM_OrderRelease.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId).Max(p => p.RSN);

                //    rsn++;
                //    return rsn;
                //}
            }
            catch (Exception ex)
            {
                generalHelper.addMetaException(ex, "RS_OM_OrderRelease", "generateRSNNumber", 1);
                return 1;
            }
        }


        //Data Available in BOM or NOT
        public bool isBOMavailableOrNot(string model_code)
        {
            try
            {
                RS_BOM[] mmBOMData = (from bomItem in db.RS_BOM
                                      where bomItem.Model_Code == model_code
                                      select bomItem).ToArray();
                if (mmBOMData.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }


        }


        public RS_PartgroupItem getPartGroupItemByPartGroupAndModelCode(decimal? partgroupId, String modelCode)
        {
            try
            {
                RS_PartgroupItem[] mmPartgroupItemobj = (from partgroupItem in db.RS_PartgroupItem
                                                         where partgroupItem.Partgroup_ID == partgroupId && partgroupItem.Is_Order_Create == true && (from bomItem in db.RS_BOM_Item where bomItem.Model_Code == modelCode select bomItem.Part_No).Contains(partgroupItem.Part_No)
                                                         select partgroupItem).ToArray();
                if (mmPartgroupItemobj.Count() > 0)
                {
                    return mmPartgroupItemobj[0];
                }
                else
                {
                    return null;
                }
                //return mmPartgroupItemobj;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public RS_Partmaster getPartmasterByPartNumber(String partNo)
        {
            try
            {
                RS_Partmaster mmPartMasterObj = db.RS_Partmaster.Where(p => p.Part_No == partno).Single();
                return mmPartMasterObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*               Action Name               : getTodayReleasedOrderDetails
         *               Description               : Function used to find out today release order details. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : shopId
         *               Return Type               : RS_OM_OrderRelease[]
         *               Revision                  : 1
        */

        public RS_OM_OrderRelease[] getTodayReleasedOrderDetails(int shopId)
        {
            try
            {
                RS_OM_OrderRelease[] mmOmOrderRelease = (from orderRelease in db.RS_OM_OrderRelease
                                                         where orderRelease.Shop_ID == shopId
                                                         select orderRelease).ToArray();

                return mmOmOrderRelease;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int getGroupNo()
        {
            try
            {
                int groupNo = db.RS_OM_Planned_Orders.Max(p => p.Group_No).GetValueOrDefault();
                return groupNo + 1;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        public bool addRecordToPlannedOrders(RS_OM_OrderRelease omOrderRelease, int groupId, String modelcode = "", decimal? seriescode = null)
        {
            try
            {
                RS_OM_Planned_Orders plannedOrdersObj = new RS_OM_Planned_Orders();
                plannedOrdersObj.Group_No = groupId;

                plannedOrdersObj.Order_ID = omOrderRelease.Row_ID;
                plannedOrdersObj.Order_No = omOrderRelease.Order_No;
                plannedOrdersObj.Order_Status = omOrderRelease.Order_Status;
                plannedOrdersObj.Parent_Model_Code = modelcode;
                plannedOrdersObj.Parent_Series_Code = seriescode;
                if (modelcode == "")
                {
                    plannedOrdersObj.Parent_Model_Code = omOrderRelease.Model_Code;
                    plannedOrdersObj.Model_Code = omOrderRelease.Model_Code;
                }
                else
                {
                    plannedOrdersObj.Parent_Model_Code = modelcode;
                    plannedOrdersObj.Model_Code = omOrderRelease.partno;
                }
                if (seriescode == null)
                {
                    plannedOrdersObj.Parent_Series_Code = omOrderRelease.Series_Code;
                    plannedOrdersObj.Series_Code = omOrderRelease.Series_Code;
                }
                else
                {
                    plannedOrdersObj.Parent_Series_Code = seriescode;

                    String model = omOrderRelease.partno;
                    RS_Model_Master modelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == model).Single();
                    plannedOrdersObj.Series_Code = modelMasterObj.Series_Code;
                }
                plannedOrdersObj.RSN = omOrderRelease.RSN;
                plannedOrdersObj.Planned_Date = DateTime.Now.Date;
                plannedOrdersObj.Inserted_Time = omOrderRelease.Inserted_Date;
                db.RS_OM_Planned_Orders.Add(plannedOrdersObj);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public String isOrderValidToCreate(String modelCode, decimal? seriesCode = null)
        {
            try
            {
                // process to check the BOM is available or not in system
                RS_BOM[] mmBomObj = db.RS_BOM.Where(p => p.Model_Code == modelCode).ToArray();

                String res = null;
                if (mmBomObj.Count() > 0)
                {
                    // process to check the BOM item
                    RS_BOM_Item[] mmBomItemObj = db.RS_BOM_Item.Where(p => p.Model_Code == modelCode).ToArray();
                    if (mmBomItemObj.Count() > 0)
                    {

                        // process to check the series for two or more models
                        //var modelObj = from modelMasterObj in db.RS_Model_Master where modelMasterObj.Series_Code == seriesCode select modelMasterObj;

                        //if (modelObj.Count() == 1)
                        //{
                        // process to check the part master
                        RS_Partmaster[] partMasterObj = db.RS_Partmaster.Where(p => p.Part_No == modelCode).ToArray();
                        if (!(partMasterObj.Count() > 0))
                        {
                            res = "Part master configuration not available for this modelcode";
                        }
                        else
                        {
                            // process to check the series contains i,o and q characters
                            //RS_OM_Creation mmOmCreationObj = new RS_OM_Creation();
                            //if(mmOmCreationObj.isStringContainSpecialCharacters(seriesCode))
                            //{
                            //    res = "Order not created." + System.Environment.NewLine + "Series contain special characters I, O or Q";
                            //}

                        }

                        //}
                        //else
                        //{
                        //    res = "Order not created." + System.Environment.NewLine + "Invalid series configuration done";
                        //}
                    }
                    else
                    {
                        res = "No BOM found for this model";
                    }
                }
                else
                {
                    res = "No BOM found for this model";
                }

                return res;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    public class MetaOrderRelease
    {
        //[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        //[Required]
        //public DateTime Planned_Date { get; set; }
    }
    public class OrderPattern
    {
        public decimal Platform_ID { get; set; }
        public int Ratio { get; set; }

        public int Priority { get; set; }
        //[DisplayFormat(DataFormatString ="{mm/dd/yy}", ApplyFormatInEditMode =true)]
        public DateTime Planned_Date { get; set; }

        //public int Tact_Time { get; set; }

        public string Platform_Name { get; set; }

        public int Production_Count { get; set; }
        public string ModelCode { get; set; }
        public int OrderQty { get; set; }
        public string Color { get; set; }
        public string Drive { get; set; }
        public int shopID { get; set; }
        public decimal? lineID { get; set; }

    }
    public class Shift
    {
        public decimal Shift_ID { get; set; }
        public string Shift_Name { get; set; }
    }
}