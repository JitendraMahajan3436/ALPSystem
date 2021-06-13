using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using System.IO;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using REIN_MES_System.Controllers.BaseManagement;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Data.OleDb;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /* Class Name                 : AddUsersController
    *  Description                : This class is used to do the basic operations insert, update, edit and delete operations of Adduserform
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    public class AddUsersController : BaseController
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

        //MM_MTTUW_Employee mmEmp = new MM_MTTUW_Employee();
        General generalObj = new General();
        /*	    Action Name		    : Index
        *		Description		    : To Display the users information in grid
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AddUsers
        public ActionResult Index()
        {
            try
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = ResourceModules.User_Config;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = ResourceModules.User;
                globalData.actionName = ResourceGlobal.Index;
                globalData.contentTitle = ResourceGlobal.User + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceGlobal.User + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;

                var RS_Employee = db.RS_Employee.Where(m => m.Is_Deleted != true && m.Plant_ID == plantId);
                return View(RS_Employee.ToList());
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_User;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }


        /*	    Action Name		    : Details
         *		Description		    : To show the users detailed information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : id of employee whose information is to be displayed 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // GET: AddUsers/Details/5
        public ActionResult Details(decimal id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RS_Employee RS_Employee = db.RS_Employee.Find(id);
                if (RS_Employee == null)
                {
                    return HttpNotFound();
                }
                globalData.pageTitle = ResourceModules.User_Config;
                globalData.subTitle = ResourceGlobal.Details;
                globalData.controllerName = "Users";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.User + " " + ResourceGlobal.Details;
                globalData.contentFooter = ResourceGlobal.User + " " + ResourceGlobal.Details;
                ViewBag.GlobalDataModel = globalData;
                return View(RS_Employee);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_User;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }


        /*	    Action Name		    : Create
        *		Description		    : To read the user info which is to be saved
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AddUsers/Create
        public ActionResult Create()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceModules.User_Config;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = ResourceModules.Add_User;
                globalData.actionName = ResourceGlobal.Index;
                globalData.contentTitle = ResourceGlobal.User + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceGlobal.User + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Category_Id = new SelectList(db.RS_AM_Category.OrderBy(c => c.Category_Name), "Category_Id", "Category_Name");
                // ViewBag.Plants_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", plantID);
                ViewBag.Plants_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", plantID);
                ViewBag.PlantName = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
                ViewBag.Shops_ID = new SelectList(db.RS_Shops.Where(c => c.Plant_ID == plantID).OrderBy(s => s.Shop_Name), "Shop_ID", "Shop_Name");
                ViewBag.Lines_ID = new SelectList((db.RS_Lines.OrderBy(m => m.Line_Name)), "Line_ID", "Line_Name");
                // ViewBag.Trade_ID = new SelectList(db.MM_Trade.OrderBy(m => m.Trade), "Trade_ID", "Trade");
                ViewBag.Stage_Allocation_ID = new SelectList(db.RS_Stations.OrderBy(m => m.Station_Name), "Station_ID", "Station_Name");
                ViewBag.BWT_ID = new SelectList(db.RS_Lines.OrderBy(m => m.Line_Name), "Line_ID", "Line_Name");
                ViewBag.Shift_ID = new SelectList(db.RS_Shift.OrderBy(m => m.Shift_Name), "Shift_ID", "Shift_Name");
                return View();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_User;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }


        /*	    Action Name		    : Create
         *		Description		    : To save the users information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : RS_Employee object 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // POST: AddUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Employee RS_Employee, HttpPostedFileBase upload)
        {
            try
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        //RS_Employee.Image_Name = System.IO.Path.GetFileName(upload.FileName);
                        //RS_Employee.Image_Type = Path.GetExtension(upload.FileName);
                        //RS_Employee.Content_Type = upload.ContentType;
                        //RS_Employee.Image_Content = reader.ReadBytes(upload.ContentLength);
                    }
                }

                decimal Plants_ID = ((FDSession)this.Session["FDSession"]).plantId;
                RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping = new RS_AM_Shop_Manager_Mapping();
                RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping = new RS_AM_Line_Supervisor_Mapping();
                RS_AM_UserPlant RS_AM_UserPlant = new RS_AM_UserPlant();


                string DM = 1999 + "-" + RS_Employee.Month + "-" + RS_Employee.Date;

                // Category 1 - Cell Member
                // Category 2 - Line Officer
                // Category 3 - Manager

                // Cateogry 4 - Department Head
                // Category 5 - Plant Head
                // Category 6 - Division Head

                // Category 7 - COO
                bool isValid = true;
                if (RS_Employee.IsEmailExists(RS_Employee.Email_Address, Plants_ID))
                {
                    ModelState.AddModelError("Email_Address", ResourceValidation.Exist);
                    isValid = false;
                }

                if (!(String.IsNullOrEmpty(RS_Employee.Date) || RS_Employee.Date == "0"))
                {
                    if (String.IsNullOrEmpty(RS_Employee.Month) || RS_Employee.Month == "0")
                    {

                        // select birthdate
                        isValid = false;
                        ModelState.AddModelError("Month", "Please select month");
                    }
                }

                if (!(String.IsNullOrEmpty(RS_Employee.Month) || RS_Employee.Month == "0"))
                {
                    if (String.IsNullOrEmpty(RS_Employee.Date) || RS_Employee.Date == "0")
                    {
                        // select date
                        isValid = false;
                        ModelState.AddModelError("Date", "Please select date");
                    }
                }
                empno = RS_Employee.Employee_No;
                if (RS_Employee.IsEmpNoExists(empno, Plants_ID))
                {
                    isValid = false;
                    ModelState.AddModelError("Employee_No", ResourceValidation.Exist);
                }
                else
                {

                    int userCategory = Convert.ToInt16(RS_Employee.Category_ID);

                    if (userCategory == 0)
                    {
                        isValid = false;
                        ModelState.AddModelError("Category_ID", "Please select category");
                    }


                    if (userCategory != 0)
                    {


                        if (userCategory != 1)
                        {
                            // mobile number and email address is required
                            if (String.IsNullOrEmpty(RS_Employee.Contact_No.ToString()))
                            {
                                isValid = false;
                                ModelState.AddModelError("Contact_No", "Please enter contact number");
                            }
                            else
                                if (RS_Employee.Contact_No.ToString().Count() > 10 || RS_Employee.Contact_No.ToString().Count() < 10)
                            {
                                isValid = false;
                                ModelState.AddModelError("Contact_No", "Please enter valid contact number");
                            }
                            else
                            {
                                if (!RS_Employee.Contact_No.ToString().All(char.IsDigit))
                                {
                                    isValid = false;
                                    ModelState.AddModelError("Contact_No", "Please enter valid contact number");
                                }
                            }

                            // process to check email address
                            if (String.IsNullOrEmpty(RS_Employee.Email_Address))
                            {
                                isValid = false;
                                ModelState.AddModelError("Email_Address", "Please enter email address");
                            }

                            else if (new EmailAddressAttribute().IsValid(RS_Employee.Email_Address) == false)
                            {
                                isValid = false;
                                ModelState.AddModelError("Email_Address", "Please enter valid email address");
                            }
                            if (RS_Employee.IsEmailExists(RS_Employee.Email_Address, Plants_ID))
                            {
                                ModelState.AddModelError("Email_Address", ResourceValidation.Exist);
                            }
                        }

                        // process to check the gender
                        if (RS_Employee.Gender == null)
                        {
                            isValid = false;
                            ModelState.AddModelError("Gender", "Please select gender");
                        }

                        // check plant selection required
                        // COO, Division Head

                        if (userCategory == 7 || userCategory == 6)
                        {
                            if (!(RS_Employee.Plants_ID.Count() > 0))
                            {

                                isValid = false;
                                ModelState.AddModelError("Plants_ID", "Please select plant");
                            }

                            if (RS_Employee.Plants_ID.Count() == 1 && RS_Employee.Plants_ID[0] == 0)
                            {
                                isValid = false;
                                ModelState.AddModelError("Plants_ID", "Please select plant");
                            }
                        }

                        // process to check line office with shop and line
                        // manager
                        if (userCategory == 2 || userCategory == 3)
                        {
                            if (RS_Employee.Shops_ID == null)
                            {
                                isValid = false;
                                ModelState.AddModelError("Shops_ID", "Please select shop");
                            }
                            else
                                if (RS_Employee.Shops_ID.Count() == 1 && RS_Employee.Shops_ID[0] == 0)
                            {
                                isValid = false;
                                ModelState.AddModelError("Shops_ID", "Please select shop");
                            }
                        }

                        if (userCategory == 2)
                        {
                            if (RS_Employee.Lines_ID == null)
                            {
                                isValid = false;
                                ModelState.AddModelError("Lines_ID", "Please select line");
                            }

                            else
                                if (RS_Employee.Lines_ID.Count() == 1 && RS_Employee.Lines_ID[0] == 0)
                            {
                                isValid = false;
                                ModelState.AddModelError("Lines_ID", "Please select line");
                            }
                        }
                        // process to chech the manager shop
                        //if (userCategory == 3)
                        //{

                        //    if (!(RS_Employee.Shops_ID.Count() > 0))
                        //    {
                        //        isValid = false;
                        //        ModelState.AddModelError("Shops_ID", "Please select shop");
                        //    }
                        //}

                    }
                }
                // process to check the employee number

                if (String.IsNullOrEmpty(RS_Employee.Employee_No))
                {
                    isValid = false;
                    ModelState.AddModelError("Employee_No", "Please enter employee number");
                }
                else
                    if (RS_Employee.IsEmpNoExists(empno, Plants_ID))
                {
                    isValid = false;
                    ModelState.AddModelError("Employee_No", ResourceValidation.Exist);
                }


                if (isValid == true)
                {

                    //if (!String.IsNullOrWhiteSpace(RS_Employee.Trade))
                    //{
                    //    MM_Trade stdObj = db.MM_Trade.Where(a => a.Trade == RS_Employee.Trade).FirstOrDefault();

                    //    if (stdObj == null)
                    //    {
                    //        MM_Trade standardObj = new MM_Trade();
                    //        standardObj.Trade = RS_Employee.Trade;
                    //        standardObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //        standardObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    //        standardObj.Inserted_Date = DateTime.Now;
                    //        standardObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    //        db.MM_Trade.Add(standardObj);
                    //        db.SaveChanges();
                    //        RS_Employee.Trade_ID = standardObj.Trade_ID;
                    //    }
                    //    else
                    //    {
                    //        RS_Employee.Trade_ID = stdObj.Trade_ID;
                    //    }
                    //}



                    email = RS_Employee.Email_Address;
                    empno = RS_Employee.Employee_No;

                    int[] plantid = RS_Employee.Plants_ID;
                    int[] shopid = RS_Employee.Shops_ID;
                    int[] lineid = RS_Employee.Lines_ID;
                    category = Convert.ToInt32(RS_Employee.Category_ID);



                    RS_Employee.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Employee.Inserted_Date = DateTime.Now;
                    RS_Employee.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    RS_Employee.Employee_Password = "e6e061838856bf47e1de730719fb2609";

                    RS_Employee.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;// RS_Employee.Plants_ID[0];
                    db.RS_Employee.Add(RS_Employee);
                    db.SaveChanges();

                    ////Add record into MTTUW Employee
                    //mmEmp.Employee_ID = RS_Employee.Employee_ID;
                    //mmEmp.Employee_Name = RS_Employee.Employee_Name;
                    //mmEmp.Employee_Password = RS_Employee.Employee_Password;
                    //mmEmp.Employee_No = RS_Employee.Employee_No;
                    //mmEmp.Email_Address = RS_Employee.Email_Address;
                    //mmEmp.DOB = RS_Employee.DOB;
                    //mmEmp.Category_ID = RS_Employee.Category_ID;
                    //mmEmp.Gender = RS_Employee.Gender;
                    //mmEmp.Country_Code = mmEmp.Country_Code;
                    //mmEmp.Contact_No = RS_Employee.Contact_No;
                    //mmEmp.Plant_ID = RS_Employee.Plant_ID;
                    //mmEmp.Inserted_Date = RS_Employee.Inserted_Date;
                    //mmEmp.Inserted_Host = RS_Employee.Inserted_Host;
                    //mmEmp.Inserted_User_ID = RS_Employee.Inserted_User_ID;
                    //mmEmp.Is_Transfered = false;
                    //mmEmp.Is_Purgeable = false;
                    //db_1.MM_MTTUW_Employee.Add(mmEmp);
                    //db_1.SaveChanges();
                    ////End

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Add_User;
                    globalData.messageDetail = ResourceGlobal.User + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    foreach (var item in plantid)
                    {
                        if (item == 0)
                            continue;
                        RS_AM_UserPlant.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_AM_UserPlant.Inserted_Date = DateTime.Now;
                        RS_AM_UserPlant.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        RS_AM_UserPlant.Plant_ID = Convert.ToDecimal(item);
                        RS_AM_UserPlant.Employee_ID = RS_Employee.Employee_ID;
                        RS_AM_UserPlant.Is_QDMS = true;
                        db.RS_AM_UserPlant.Add(RS_AM_UserPlant);
                        db.SaveChanges();
                    }

                    if (category == 3)
                    {
                        foreach (var item in shopid)
                        {
                            if (item == 0)
                                continue;
                            RS_AM_Shop_Manager_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            RS_AM_Shop_Manager_Mapping.Inserted_Date = DateTime.Now;
                            RS_AM_Shop_Manager_Mapping.Inserted_Host = "1";
                            RS_AM_Shop_Manager_Mapping.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;// Convert.ToDecimal(RS_Employee.Plants_ID[0]);
                            RS_AM_Shop_Manager_Mapping.Shop_ID = Convert.ToDecimal(item);
                            RS_AM_Shop_Manager_Mapping.Employee_ID = RS_Employee.Employee_ID;
                            db.RS_AM_Shop_Manager_Mapping.Add(RS_AM_Shop_Manager_Mapping);
                            db.SaveChanges();
                        }
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Add_User;
                        globalData.messageDetail = ResourceGlobal.User + " " + ResourceMessages.Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");

                    }
                    else if (category == 2)
                    {
                        foreach (var item in lineid)
                        {
                            if (item == 0)
                                continue;
                            RS_AM_Line_Supervisor_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            RS_AM_Line_Supervisor_Mapping.Inserted_Date = DateTime.Now;
                            RS_AM_Line_Supervisor_Mapping.Inserted_Host = "1";
                            RS_AM_Line_Supervisor_Mapping.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId; //Convert.ToDecimal(RS_Employee.Plants_ID[0]);
                            RS_AM_Line_Supervisor_Mapping.Shop_ID = Convert.ToDecimal(RS_Employee.Shops_ID[0]);
                            RS_AM_Line_Supervisor_Mapping.Line_ID = Convert.ToDecimal(item);
                            RS_AM_Line_Supervisor_Mapping.Employee_ID = RS_Employee.Employee_ID;
                            db.RS_AM_Line_Supervisor_Mapping.Add(RS_AM_Line_Supervisor_Mapping);
                            db.SaveChanges();
                        }
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Add_User;
                        globalData.messageDetail = ResourceGlobal.User + " " + ResourceMessages.Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }

                    return RedirectToAction("Index");
                }


                globalData.pageTitle = ResourceModules.User_Config;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Users";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceModules.Add_User + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceModules.Add_User + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Plants_ID = new SelectList(db.RS_Plants.Where(c => c.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_Employee.Plants_ID);

                decimal shopId = 0;
                if (RS_Employee.Shops_ID != null)
                {
                    shopId = Convert.ToDecimal(RS_Employee.Shops_ID[0]);
                    ViewBag.Shops_ID = new SelectList(db.RS_Shops.Where(c => c.Plant_ID == plantId).OrderBy(s => s.Shop_Name), "Shop_ID", "Shop_Name", shopId);

                }
                else
                {
                    ViewBag.Shops_ID = new SelectList(db.RS_Shops.Where(c => c.Plant_ID == plantId).OrderBy(s => s.Shop_Name), "Shop_ID", "Shop_Name");
                }

                ViewBag.Lines_ID = new SelectList(db.RS_Lines.Where(c => c.Shop_ID == shopId).OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Employee.Lines_ID);

                ViewBag.Category_Id = new SelectList(db.RS_AM_Category.OrderBy(c => c.Category_Name), "Category_Id", "Category_Name", RS_Employee.Category_ID);
                ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(x => x.Is_Deleted != true).OrderBy(c => c.Employee_Name), "Employee_ID", "Employee_Name", RS_Employee.Employee_ID);
                //ViewBag.BWT_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_ID", RS_Employee.BWT_ID);
                // ViewBag.Stage_Allocation_ID = new SelectList(db.RS_Stations.OrderBy(c => c.Station_Name), "Station_ID", "Station_Name", RS_Employee.Stage_Allocation_ID);
                //ViewBag.Shift_ID = new SelectList(db.RS_Shift.OrderBy(c => c.Shift_Name), "Shift_ID", "Shift_Name", RS_Employee.Shift_ID);
                return View(RS_Employee);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_User;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Create", RS_Employee);
                }
            }

        }


        /*	    Action Name		    : Edit
        *		Description		    : To read the users information which is to be edited
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AddUsers/Edit/5
        public ActionResult Edit(decimal id)
        {

            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            RS_Plants[] mmPlantsObj = null;
            RS_Shops[] mmShopsObj = null;
            RS_Lines[] mmLinesObj = null;
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RS_Employee RS_Employee = db.RS_Employee.Find(id);

                var abc = db.RS_AM_UserPlant.Where(x => x.Employee_ID == id).Select(x => (x.Plant_ID)).ToArray();
                ViewBag.Plants_ID = new MultiSelectList(db.RS_Plants, "Plant_ID", "Plant_Name", abc);

                var categoryID = RS_Employee.Category_ID;
                if (categoryID == 3)
                {
                    var pqr = db.RS_AM_Shop_Manager_Mapping.Where(x => x.Employee_ID == id).Select(x => x.Shop_ID).ToArray();
                    ViewBag.Shops_ID = new MultiSelectList(db.RS_Shops, "Shop_ID", "Shop_Name", pqr);
                }


                if (RS_Employee.DOB != null)
                {
                    ViewBag.DOB = RS_Employee.DOB.Value.ToShortDateString();
                    var age = GetAge(RS_Employee.DOB.ToString());
                    // ViewBag.Age = ((REIN_MES_System.Models.RS_Employee)((System.Web.Mvc.JsonResult)age).Data).Age;
                }

                //if(RS_Employee.Join_Date != null)
                //{
                //    ViewBag.Join_Date = RS_Employee.Join_Date.ToShortDateString();
                //    var exp = GetExperience(RS_Employee.Join_Date.ToString());
                //    ViewBag.Experiance = ((REIN_MES_System.Models.RS_Employee)((System.Web.Mvc.JsonResult)exp).Data).Experiance;
                //}

                //ViewBag.Seperation_Date = RS_Employee.Seperation_Date.ToShortDateString();
                //ViewBag.Actual_Seperation_Date = RS_Employee.Actual_Seperation_Date!= null ? RS_Employee.Actual_Seperation_Date.Value.ToShortDateString() : null;
                globalData.pageTitle = ResourceModules.User_Config;
                globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "Users";
                globalData.actionName = ResourceGlobal.Edit;
                globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceGlobal.User + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceGlobal.User + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                decimal category = Convert.ToDecimal(RS_Employee.Category_ID);
                if (category == 6 || category == 7)
                {

                    mmPlantsObj = (from plants in db.RS_Plants
                                   where (from userPlant in db.RS_AM_UserPlant where userPlant.Employee_ID == id select userPlant.Plant_ID).Contains(plants.Plant_ID)
                                   select plants).ToArray();

                    ViewBag.Category_Id = new SelectList(db.RS_AM_Category.OrderBy(c => c.Category_Name), "Category_Id", "Category_Name", RS_Employee.Category_ID);
                    //ViewBag.Plants_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", RS_Employee.Plant_ID.Value);
                    ViewBag.Shops_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantID).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Employee.Shop_ID);
                    ViewBag.Lines_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", ((FDSession)this.Session["FDSession"]).lineId);
                    // ViewBag.Trade_ID = new SelectList(db.MM_Trade.OrderBy(c => c.Trade), "Trade_ID", "Trade",RS_Employee.Trade_ID);
                    // ViewBag.Stage_Allocation_ID = new SelectList(db.RS_Stations.OrderBy(c => c.Station_Name), "Station_ID", "Station_Name",RS_Employee.Stage_Allocation_ID);
                    // ViewBag.BWT_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name",RS_Employee.BWT_ID);
                    // ViewBag.Shift_ID = new SelectList(db.RS_Shift.OrderBy(c => c.Shift_Name), "Shift_ID", "Shift_Name", RS_Employee.Shift_ID);
                    return View(RS_Employee);


                }
                else if (category == 3)
                {

                    mmShopsObj = (from shops in db.RS_Shops
                                  where (from shopManagerMapping in db.RS_AM_Shop_Manager_Mapping where shopManagerMapping.Employee_ID == id select shopManagerMapping.Shop_ID).Contains(shops.Shop_ID)
                                  select shops).ToArray();

                    ViewBag.Category_Id = new SelectList(db.RS_AM_Category.OrderBy(c => c.Category_Name), "Category_Id", "Category_Name", RS_Employee.Category_ID);
                    //ViewBag.Plants_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", ((FDSession)this.Session["FDSession"]).plantId);
                    //ViewBag.Shops_ID = new MultiSelectList(db.RS_Shops.Where(p => p.Plant_ID == plantID).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", mmShopsObj.Select(x => x.Shop_ID).ToArray());
                    ViewBag.Lines_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", ((FDSession)this.Session["FDSession"]).lineId);
                    //ViewBag.Trade_ID = new SelectList(db.MM_Trade.OrderBy(c => c.Trade), "Trade_ID", "Trade", RS_Employee.Trade_ID);
                    //ViewBag.Stage_Allocation_ID = new SelectList(db.RS_Stations.OrderBy(c => c.Station_Name), "Station_ID", "Station_Name", RS_Employee.Stage_Allocation_ID);
                    //ViewBag.BWT_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Employee.BWT_ID);
                    //ViewBag.Shift_ID = new SelectList(db.RS_Shift.OrderBy(c => c.Shift_Name), "Shift_ID", "Shift_Name", RS_Employee.Shift_ID);

                    return View(RS_Employee);

                }
                else if (category == 2)
                {
                    mmShopsObj = (from shops in db.RS_Shops
                                  where (from lineSupervisorMapping in db.RS_AM_Line_Supervisor_Mapping where lineSupervisorMapping.Employee_ID == id select lineSupervisorMapping.Shop_ID).Contains(shops.Shop_ID)
                                  select shops).ToArray();

                    mmLinesObj = (from lines in db.RS_Lines
                                  where (from lineSupervisorMapping in db.RS_AM_Line_Supervisor_Mapping where lineSupervisorMapping.Employee_ID == id select lineSupervisorMapping.Line_ID).Contains(lines.Line_ID)
                                  select lines).ToArray();


                    ViewBag.Category_Id = new SelectList(db.RS_AM_Category.OrderBy(c => c.Category_Name), "Category_Id", "Category_Name", RS_Employee.Category_ID);
                    //ViewBag.Plants_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", ((FDSession)this.Session["FDSession"]).plantId);
                    ViewBag.Shops_ID = new MultiSelectList(db.RS_Shops.Where(p => p.Plant_ID == plantID).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", mmShopsObj.Select(x => x.Shop_ID).ToArray());
                    decimal shopID = mmShopsObj[0].Shop_ID;
                    ViewBag.Lines_ID = new MultiSelectList(db.RS_Lines.Where(l => l.Shop_ID == shopID).OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", mmLinesObj.Select(x => x.Line_ID).ToArray());
                    //ViewBag.Trade_ID = new SelectList(db.MM_Trade.OrderBy(c => c.Trade), "Trade_ID", "Trade", RS_Employee.Trade_ID);
                    //ViewBag.Stage_Allocation_ID = new SelectList(db.RS_Stations.OrderBy(c => c.Station_Name), "Station_ID", "Station_Name", RS_Employee.Stage_Allocation_ID);
                    // ViewBag.BWT_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Employee.BWT_ID);
                    //ViewBag.Shift_ID = new SelectList(db.RS_Shift.OrderBy(c => c.Shift_Name), "Shift_ID", "Shift_Name", RS_Employee.Shift_ID);


                    return View(RS_Employee);

                }
                else
                {

                    ViewBag.Category_Id = new SelectList(db.RS_AM_Category.OrderBy(c => c.Category_Name), "Category_Id", "Category_Name", RS_Employee.Category_ID);
                    //ViewBag.Plants_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", ((FDSession)this.Session["FDSession"]).plantId);
                    ViewBag.Shops_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantID).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", ((FDSession)this.Session["FDSession"]).shopId);
                    ViewBag.Lines_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", ((FDSession)this.Session["FDSession"]).lineId);
                    //ViewBag.Trade_ID = new SelectList(db.MM_Trade.OrderBy(c => c.Trade), "Trade_ID", "Trade", RS_Employee.Trade_ID);
                    // ViewBag.Stage_Allocation_ID = new SelectList(db.RS_Stations.OrderBy(c => c.Station_Name), "Station_ID", "Station_Name", RS_Employee.Stage_Allocation_ID);
                    //ViewBag.BWT_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Employee.BWT_ID);
                    // ViewBag.Shift_ID = new SelectList(db.RS_Shift.OrderBy(c => c.Shift_Name), "Shift_ID", "Shift_Name", RS_Employee.Shift_ID);


                    return View(RS_Employee);
                }

                if (RS_Employee == null)
                {
                    return HttpNotFound();
                }




            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_User;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }


        /*	    Action Name		    : Edit
        *		Description		    : To edit the users information
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : RS_Employee object 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // POST: AddUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Employee RS_Employee, HttpPostedFileBase upload)
        {
            RS_AM_UserPlant UserPlantobj = new RS_AM_UserPlant();
            RS_Employee Employeeobj = new RS_Employee();
            try
            {
                decimal shopid2 = 0;
                RS_Employee mmUsersObj = new RS_Employee();
                RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping = new RS_AM_Shop_Manager_Mapping();
                RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping = new RS_AM_Line_Supervisor_Mapping();
                RS_AM_UserPlant RS_AM_UserPlant = new RS_AM_UserPlant();
                if (ModelState.IsValid)
                {
                    //if (!String.IsNullOrWhiteSpace(RS_Employee.Trade))
                    //{
                    //    MM_Trade stdObj = db.MM_Trade.Where(a => a.Trade == RS_Employee.Trade).FirstOrDefault();

                    //    if (stdObj == null)
                    //    {
                    //        MM_Trade standardObj = new MM_Trade();
                    //        standardObj.Trade = RS_Employee.Trade;
                    //        standardObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //        standardObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    //        standardObj.Inserted_Date = DateTime.Now;
                    //        standardObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    //        db.MM_Trade.Add(standardObj);
                    //        db.SaveChanges();
                    //        RS_Employee.Trade_ID = standardObj.Trade_ID;
                    //    }
                    //    else
                    //    {
                    //        RS_Employee.Trade_ID = stdObj.Trade_ID;
                    //    }
                    //}

                    email = RS_Employee.Email_Address;
                    empno = RS_Employee.Employee_No;
                    empId = RS_Employee.Employee_ID;

                    decimal Plants_ID = ((FDSession)this.Session["FDSession"]).plantId; //RS_Employee.Plants_ID[0];
                    int[] plantid = RS_Employee.Plants_ID;
                    int[] shopid = RS_Employee.Shops_ID;
                    int[] lineid = RS_Employee.Lines_ID;
                    decimal SHOP = RS_Employee.Shop_ID;


                    string DM = 1999 + "-" + RS_Employee.Month + "-" + RS_Employee.Date;
                    bool isValid = true;
                    category = Convert.ToInt32(RS_Employee.Category_ID);
                    if (category != 1)
                    {
                        if (email == null)
                        {
                            isValid = false;
                            ModelState.AddModelError("Email_Address", ResourceValidation.Required);
                        }
                        if (String.IsNullOrEmpty(RS_Employee.Contact_No.ToString()))
                        {
                            isValid = false;
                            ModelState.AddModelError("Contact_No", "Please enter contact number");
                        }
                        else
                           if (RS_Employee.Contact_No.ToString().Count() > 10 || RS_Employee.Contact_No.ToString().Count() < 10)
                        {
                            isValid = false;
                            ModelState.AddModelError("Contact_No", "Please enter valid contact number");
                        }
                        else
                        {
                            if (!RS_Employee.Contact_No.ToString().All(char.IsDigit))
                            {
                                isValid = false;
                                ModelState.AddModelError("Contact_No", "Please enter valid contact number");
                            }
                        }

                        // process to check email address
                        if (String.IsNullOrEmpty(RS_Employee.Email_Address))
                        {
                            isValid = false;
                            ModelState.AddModelError("Email_Address", "Please enter email address");
                        }

                        else if (new EmailAddressAttribute().IsValid(RS_Employee.Email_Address) == false)
                        {
                            isValid = false;
                            ModelState.AddModelError("Email_Address", "Please enter valid email address");
                        }
                        if (RS_Employee.IsEmailExistsEdit(RS_Employee.Email_Address, RS_Employee.Employee_ID, Plants_ID))
                        {
                            ModelState.AddModelError("Email_Address", ResourceValidation.Exist);
                        }

                        if (category == 7 || category == 6)
                        {
                            if (!(RS_Employee.Plants_ID.Count() > 0))
                            {

                                isValid = false;
                                ModelState.AddModelError("Plants_ID", "Please select plant");
                            }

                            if (RS_Employee.Plants_ID.Count() == 1 && RS_Employee.Plants_ID[0] == 0)
                            {
                                isValid = false;
                                ModelState.AddModelError("Plants_ID", "Please select plant");
                            }
                        }

                        // process to check line office with shop and line
                        // manager
                        if (category == 2 || category == 3)
                        {
                            if (RS_Employee.Shop_ID == null)
                            {
                                isValid = false;
                                ModelState.AddModelError("Shops_ID", "Please select shop");
                            }
                            else
                                if (RS_Employee.Shops_ID != null && RS_Employee.Shops_ID.Count() == 1 && RS_Employee.Shops_ID[0] == 0)
                            {
                                isValid = false;
                                ModelState.AddModelError("Shops_ID", "Please select shop");
                            }
                        }

                        if (category == 2)
                        {
                            var lineSupervisorItem = (from userLine in db.RS_AM_Line_Supervisor_Mapping
                                                      where (userLine.Employee_ID == RS_Employee.Employee_ID)
                                                      select userLine.Shop_ID);
                            foreach (var item in lineSupervisorItem)
                            {
                                shopid2 = Convert.ToDecimal(item);
                            }


                            if (RS_Employee.Lines_ID == null)
                            {
                                isValid = false;
                                ModelState.AddModelError("Lines_ID", "Please select line");
                            }

                            else if (RS_Employee.Lines_ID.Count() == 1 && RS_Employee.Lines_ID[0] == 0)
                            {
                                isValid = false;
                                ModelState.AddModelError("Lines_ID", "Please select line");
                            }
                        }
                    }
                    if (RS_Employee.IsEmailExistsEdit(email, empId, Plants_ID))
                    {
                        if (category == 1)
                        {
                            if (email != null)
                            {
                                isValid = false;
                                ModelState.AddModelError("Email_Address", ResourceValidation.Exist);
                            }
                            else
                            {

                            }

                        }
                        else
                        {
                        }
                    }
                    if (RS_Employee.IsEmpNoExistsEdit(empno, empId, Plants_ID))
                    {
                        isValid = false;
                        ModelState.AddModelError("Employee_No", ResourceValidation.Exist);
                    }

                    if (RS_Employee.IsEmailExistsEdit(email, empId, Plants_ID))
                    {
                        if (email != null)
                        {
                            isValid = false;
                            ModelState.AddModelError("Email_Address", ResourceValidation.Exist);
                        }
                    }

                    else
                    {
                        if (isValid == true)
                        {


                            Employeeobj = db.RS_Employee.Find(RS_Employee.Employee_ID);

                            if (upload != null && upload.ContentLength > 0)
                            {
                                using (var reader = new System.IO.BinaryReader(upload.InputStream))
                                {
                                    //Employeeobj.Image_Name = System.IO.Path.GetFileName(upload.FileName);
                                    //Employeeobj.Image_Type = Path.GetExtension(upload.FileName);
                                    //Employeeobj.Content_Type = upload.ContentType;
                                    //Employeeobj.Image_Content = reader.ReadBytes(upload.ContentLength);
                                }
                            }
                            Employeeobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            Employeeobj.Updated_Date = DateTime.Now;
                            Employeeobj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                            Employeeobj.Employee_Name = RS_Employee.Employee_Name;
                            Employeeobj.Gender = RS_Employee.Gender;
                            Employeeobj.Country_Code = RS_Employee.Country_Code;
                            Employeeobj.Contact_No = RS_Employee.Contact_No;
                            Employeeobj.Category_ID = RS_Employee.Category_ID;
                            Employeeobj.Employee_No = RS_Employee.Employee_No;
                            Employeeobj.Email_Address = RS_Employee.Email_Address;
                            Employeeobj.DOB = RS_Employee.DOB;
                            //Employeeobj.Join_Date = RS_Employee.Join_Date;
                            //Employeeobj.Seperation_Date = RS_Employee.Seperation_Date;
                            //Employeeobj.Actual_Seperation_Date = RS_Employee.Actual_Seperation_Date;
                            //Employeeobj.Additional_Skills = RS_Employee.Additional_Skills;
                            //Employeeobj.Qualification = RS_Employee.Qualification;

                            //Employeeobj.Stage_Allocation = RS_Employee.Stage_Allocation;
                            //Employeeobj.Trade = RS_Employee.Trade;
                            //Employeeobj.Org_Unit = RS_Employee.Org_Unit;
                            //Employeeobj.Bus_Route = RS_Employee.Bus_Route;
                            //Employeeobj.BWT = RS_Employee.BWT;

                            //Employeeobj.Dexterity_level = RS_Employee.Dexterity_level;
                            //Employeeobj.Experiance = RS_Employee.Experiance;

                            //Employeeobj.Locker_Number = RS_Employee.Locker_Number;
                            //Employeeobj.Trade_ID = RS_Employee.Trade_ID;
                            //Employeeobj.BWT_ID = RS_Employee.BWT_ID;
                            //Employeeobj.Stage_Allocation_ID = RS_Employee.Stage_Allocation_ID;
                            //Employeeobj.Shift_ID = RS_Employee.Shift_ID;

                            Employeeobj.Is_Edited = true;
                            Employeeobj.Category_ID = RS_Employee.Category_ID;
                            Employeeobj.Is_Edited = true;
                            Employeeobj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId; //RS_Employee.Plants_ID[0];
                            db.Entry(Employeeobj).State = EntityState.Modified;

                            db.SaveChanges();


                            ////Save records to Employee in MTTUW db
                            //mmEmp = db_1.MM_MTTUW_Employee.Find(Employeeobj.Employee_ID);
                            //mmEmp.Employee_Name = Employeeobj.Employee_Name;
                            //mmEmp.Gender = Employeeobj.Gender;
                            //mmEmp.Country_Code = Employeeobj.Country_Code;
                            //mmEmp.Contact_No = Employeeobj.Contact_No;
                            //mmEmp.Category_ID = Employeeobj.Category_ID;
                            //mmEmp.Employee_No = Employeeobj.Employee_No;
                            //mmEmp.Employee_Password = Employeeobj.Employee_Password;
                            //mmEmp.Email_Address = Employeeobj.Email_Address;
                            //mmEmp.Is_Edited = true;
                            //mmEmp.Plant_ID = Employeeobj.Plant_ID;
                            //mmEmp.Updated_Date = Employeeobj.Updated_Date;
                            //mmEmp.Updated_Host = Employeeobj.Updated_Host;
                            //mmEmp.Updated_User_ID = Employeeobj.Updated_User_ID;
                            //db_1.Entry(mmEmp).State = EntityState.Modified;
                            //db_1.SaveChanges();
                            ////End
                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceModules.Add_User;
                            globalData.messageDetail = ResourceGlobal.User + " " + ResourceMessages.Edit_Success;
                            TempData["globalData"] = globalData;
                            var userPlantItem = from userPlant in db.RS_AM_UserPlant
                                                where (userPlant.Employee_ID == RS_Employee.Employee_ID)
                                                select userPlant;
                            foreach (var items in userPlantItem.ToList())
                            {

                                db.RS_AM_UserPlant.Remove(items);
                                db.SaveChanges();
                            }
                            foreach (var id in plantid)
                            {
                                UserPlantobj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                UserPlantobj.Inserted_Date = DateTime.Now;
                                UserPlantobj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                UserPlantobj.Plant_ID = Convert.ToDecimal(id);
                                UserPlantobj.Employee_ID = RS_Employee.Employee_ID;
                                UserPlantobj.Is_Edited = true;
                                UserPlantobj.Is_QDMS = true;
                                db.RS_AM_UserPlant.Add(UserPlantobj);

                                db.SaveChanges();
                            }



                            var lineSupervisorItem = from userLine in db.RS_AM_Line_Supervisor_Mapping
                                                     where (userLine.Employee_ID == RS_Employee.Employee_ID)
                                                     select userLine;
                            foreach (var items in lineSupervisorItem.ToList())
                            {
                                db.RS_AM_Line_Supervisor_Mapping.Remove(items);
                                db.SaveChanges();
                            }

                            var shopManagerItem = from usershop in db.RS_AM_Shop_Manager_Mapping
                                                  where (usershop.Employee_ID == RS_Employee.Employee_ID)
                                                  select usershop;
                            foreach (var items in shopManagerItem.ToList())
                            {
                                shopid2 = items.Shop_ID;
                                db.RS_AM_Shop_Manager_Mapping.Remove(items);
                                db.SaveChanges();
                            }


                            //End
                            if (category == 3)
                            {
                                shopManagerItem = from usershop in db.RS_AM_Shop_Manager_Mapping
                                                  where (usershop.Employee_ID == RS_Employee.Employee_ID)
                                                  select usershop;
                                foreach (var items in shopManagerItem.ToList())
                                {
                                    shopid2 = items.Shop_ID;
                                    db.RS_AM_Shop_Manager_Mapping.Remove(items);
                                    db.SaveChanges();
                                }
                                if (shopid != null)
                                {
                                    foreach (var item in shopid)
                                    {

                                        RS_AM_Shop_Manager_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        RS_AM_Shop_Manager_Mapping.Inserted_Date = DateTime.Now;
                                        RS_AM_Shop_Manager_Mapping.Inserted_Host = "1";
                                        RS_AM_Shop_Manager_Mapping.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId; //Convert.ToDecimal(RS_Employee.Plants_ID[0]);
                                        RS_AM_Shop_Manager_Mapping.Shop_ID = Convert.ToDecimal(item);
                                        RS_AM_Shop_Manager_Mapping.Employee_ID = RS_Employee.Employee_ID;
                                        db.RS_AM_Shop_Manager_Mapping.Add(RS_AM_Shop_Manager_Mapping);

                                        db.SaveChanges();
                                    }
                                }

                                globalData.isSuccessMessage = true;
                                globalData.messageTitle = ResourceModules.Add_User;
                                globalData.messageDetail = ResourceGlobal.User + " " + ResourceMessages.Edit_Success;
                                TempData["globalData"] = globalData;
                                return RedirectToAction("Index");

                            }
                            else if (category == 2)
                            {
                                lineSupervisorItem = null;
                                lineSupervisorItem = from userLine in db.RS_AM_Line_Supervisor_Mapping
                                                     where (userLine.Employee_ID == RS_Employee.Employee_ID)
                                                     select userLine;
                                foreach (var items in lineSupervisorItem.ToList())
                                {
                                    db.RS_AM_Line_Supervisor_Mapping.Remove(items);
                                    db.SaveChanges();
                                }
                                foreach (var item in lineid)
                                {

                                    RS_AM_Line_Supervisor_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    RS_AM_Line_Supervisor_Mapping.Inserted_Date = DateTime.Now;
                                    RS_AM_Line_Supervisor_Mapping.Inserted_Host = "1";
                                    RS_AM_Line_Supervisor_Mapping.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;// Convert.ToDecimal(RS_Employee.Plants_ID[0]);
                                    RS_AM_Line_Supervisor_Mapping.Shop_ID = Convert.ToDecimal(RS_Employee.Shops_ID[0]);
                                    RS_AM_Line_Supervisor_Mapping.Line_ID = Convert.ToDecimal(item);
                                    RS_AM_Line_Supervisor_Mapping.Employee_ID = RS_Employee.Employee_ID;

                                    db.RS_AM_Line_Supervisor_Mapping.Add(RS_AM_Line_Supervisor_Mapping);
                                    db.SaveChanges();
                                }
                                globalData.isSuccessMessage = true;
                                globalData.messageTitle = ResourceModules.Add_User;
                                globalData.messageDetail = ResourceGlobal.User + " " + ResourceMessages.Edit_Success;
                                TempData["globalData"] = globalData;
                                return RedirectToAction("Index");
                            }
                            return RedirectToAction("Index");

                        }
                    }
                }
                globalData.pageTitle = ResourceModules.User_Config;
                globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "Users";
                globalData.actionName = ResourceGlobal.Edit;
                globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceGlobal.User + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceGlobal.User + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                int plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

                ViewBag.Plants_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plant_ID), "Plant_ID", "Plant_Name", plant_ID);
                ViewBag.Shops_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", shopid2);
                if (shopid2 != 0)
                {
                    decimal shopid = shopid2;
                    ViewBag.Lines_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == shopid).OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Employee.Line_ID);
                }
                else
                {
                    ViewBag.Lines_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Employee.Line_ID);
                }


                ViewBag.Category_ID = new SelectList(db.RS_AM_Category.OrderBy(c => c.Category_Name), "Category_ID", "Category_Name", RS_Employee.Category_ID);
                //ViewBag.Trade_ID = new SelectList(db.MM_Trade.OrderBy(c => c.Trade), "Trade_ID", "Trade", RS_Employee.Trade_ID);
                //ViewBag.BWT_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Employee.BWT_ID);
                //ViewBag.Stage_Allocation_ID = new SelectList(db.RS_Stations.OrderBy(c => c.Station_Name), "Station_ID", "Station_Name", RS_Employee.Stage_Allocation_ID);
                //ViewBag.Shift_ID = new SelectList(db.RS_Shift.OrderBy(c => c.Shift_Name), "Shift_ID", "Shift_Name", RS_Employee.Stage_Allocation_ID);
                return View(RS_Employee);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_User;
                    globalData.messageDetail = ex.ToString();
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }

        }



        /*	    Action Name		    : Delete
        *		Description		    : To Display the users information which is to be deleted
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id to be deleted row  
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AddUsers/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Employee RS_Employee = db.RS_Employee.Find(id);
            if (RS_Employee == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.User_Config;
            globalData.subTitle = ResourceGlobal.Delete;

            globalData.controllerName = "Users";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceGlobal.User + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceGlobal.User + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Employee);
        }



        /*	    Action Name		    : DeleteConfirmed
        *		Description		    : To delete the users record
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id of user whose record is to be deleted
        *		Return Type		    : redirected to the index page
        *		Revision		    :
        */
        // POST: AddUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {


                var eSKilloperatorID = db.RS_AM_Employee_SkillSet.Where(a => a.Employee_ID == id).Select(a => a.Employee_ID).ToList();
                foreach (var item in eSKilloperatorID)
                {
                    RS_AM_Employee_SkillSet RS_AM_Employee_SkillSet = db.RS_AM_Employee_SkillSet.Where(a => a.Employee_ID == item).Select(a => a).FirstOrDefault();
                    db.RS_AM_Employee_SkillSet.Remove(RS_AM_Employee_SkillSet);
                    db.SaveChanges();
                }

                var AoperatorID = db.RS_Assign_OperatorToSupervisor.Where(a => a.AssignedOperator_ID == id).Select(a => a.AssignedOperator_ID).ToList();
                foreach (var item in AoperatorID)
                {
                    RS_Assign_OperatorToSupervisor RS_Assign_OperatorToSupervisor = db.RS_Assign_OperatorToSupervisor.Where(a => a.AssignedOperator_ID == item).Select(a => a).FirstOrDefault();
                    db.RS_Assign_OperatorToSupervisor.Remove(RS_Assign_OperatorToSupervisor);
                    db.SaveChanges();
                }
                var operators = db.RS_AM_Operator_Station_Allocation.Where(op => op.Employee_ID == id).Select(op => op.Employee_ID).ToList();
                foreach (var item in operators)
                {
                    decimal empID = Convert.ToDecimal(item);
                    RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation = db.RS_AM_Operator_Station_Allocation.Where(op => op.Employee_ID == empID).FirstOrDefault();
                    db.RS_AM_Operator_Station_Allocation.Remove(RS_AM_Operator_Station_Allocation);
                    db.SaveChanges();
                }

                var operators_hist = db.RS_AM_Operator_Station_Allocation_History.Where(op => op.Employee_ID == id).Select(op => op.Employee_ID).ToList();
                foreach (var item in operators_hist)
                {
                    decimal empID = Convert.ToDecimal(item);
                    RS_AM_Operator_Station_Allocation_History RS_AM_Operator_Station_Allocation_History = db.RS_AM_Operator_Station_Allocation_History.Where(op => op.Employee_ID == empID).FirstOrDefault();
                    db.RS_AM_Operator_Station_Allocation_History.Remove(RS_AM_Operator_Station_Allocation_History);
                    db.SaveChanges();
                }
                //new
                var UserPlantEmpID = db.RS_AM_UserPlant.Where(a => a.Employee_ID == id).Select(a => a.Employee_ID).ToList();
                foreach (var item in UserPlantEmpID)
                {
                    RS_AM_UserPlant RS_AM_UserPlant = db.RS_AM_UserPlant.Where(a => a.Employee_ID == item).Select(a => a).FirstOrDefault();
                    db.RS_AM_UserPlant.Remove(RS_AM_UserPlant);
                    db.SaveChanges();
                }
                var sMMoperatorID = db.RS_AM_Shop_Manager_Mapping.Where(a => a.Employee_ID == id).Select(a => a.Employee_ID).ToList();
                foreach (var item in sMMoperatorID)
                {
                    RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping = db.RS_AM_Shop_Manager_Mapping.Where(a => a.Employee_ID == item).Select(a => a).FirstOrDefault();
                    db.RS_AM_Shop_Manager_Mapping.Remove(RS_AM_Shop_Manager_Mapping);
                    db.SaveChanges();
                }

                var lSMoperatorID = db.RS_AM_Line_Supervisor_Mapping.Where(a => a.Employee_ID == id).Select(a => a.Employee_ID).ToList();
                foreach (var item in lSMoperatorID)
                {
                    RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping = db.RS_AM_Line_Supervisor_Mapping.Where(a => a.Employee_ID == item).Select(a => a).FirstOrDefault();
                    db.RS_AM_Line_Supervisor_Mapping.Remove(RS_AM_Line_Supervisor_Mapping);
                    db.SaveChanges();
                }


                var aSMoperatorID = db.RS_Assign_SupervisorToManager.Where(a => a.Manager_ID == id).Select(a => a.Manager_ID).ToList();
                foreach (var item in lSMoperatorID)
                {
                    RS_Assign_SupervisorToManager RS_Assign_SupervisorToManager = db.RS_Assign_SupervisorToManager.Where(a => a.Manager_ID == item).Select(a => a).FirstOrDefault();
                    db.RS_Assign_SupervisorToManager.Remove(RS_Assign_SupervisorToManager);
                    db.SaveChanges();
                }

                RS_Employee emp = db.RS_Employee.Find(id);
                db.RS_Employee.Remove(emp);
                db.SaveChanges();



                generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Employee", "Employee_No", emp.Employee_No.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                

                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = ResourceModules.User_Config;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Add_User;
                globalData.messageDetail = ResourceGlobal.User + " " + ResourceMessages.Delete_Success;
                globalData.controllerName = "Users";
                globalData.actionName = ResourceGlobal.Delete;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceGlobal.User + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceGlobal.User + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Add_User;
                    globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Delete");
                }
            }
        }

        public ActionResult AllConfigurationExcelUpload()
        {
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                List<SelectListItem> listModel = new List<SelectListItem>();
                ViewBag.Line_ID = new SelectList(listModel);

                if (TempData["OrderUploadRecords"] != null)
                {
                    ViewBag.OrderUploadRecords = TempData["OrderUploadRecords"];
                }

                globalData.pageTitle = ResourceGlobal.Excel + " " + "All Configuration" + " " + ResourceGlobal.Form;
                globalData.subTitle = ResourceGlobal.Upload;
                globalData.controllerName = "EmployeeSkillSet";
                globalData.actionName = ResourceGlobal.Upload;
                globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Add_User + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Add_User + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }

        public FileResult Download(string fileName)
        {
            var FileVirtualPath = "~/App_Data/" + fileName;

            string file = @"~\App_Data\RS_All_Configuration.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult AllConfigurationExcelUpload(ExcelAllConfiguration formData)
        {
            try
            {
                string day = "", fromDay = "";
                String createdOrders = "";
                if (ModelState.IsValid)
                {
                    GlobalOperations globalOperations = new GlobalOperations();
                    string fileName = Path.GetFileName(formData.Excel_File.FileName);
                    string fileExtension = Path.GetExtension(formData.Excel_File.FileName);
                    string fileLocation = Server.MapPath("~/App_Data/" + fileName);
                    System.IO.File.Delete(fileName);
                    DataTable dt = ExcelToDataTable(formData.Excel_File, fileLocation, fileExtension);


                    DateTime fromDate = Convert.ToDateTime(formData.FromDate);
                    DateTime toDate = Convert.ToDateTime(formData.ToDate);
                    fromDay = formData.FromDay;
                    day = formData.Day;

                    if (dt.Rows.Count > 0)
                    {
                        AllConfigurationUploadRecords[] orderUploadRecordsObj = new AllConfigurationUploadRecords[dt.Rows.Count];


                        int i = 0;
                        int ExcelNumber = db.RS_All_Configuration.Max(m => m.Excel_No).Value;
                        foreach (DataRow checkListRow in dt.Rows)
                        {


                            String PlantName = checkListRow[0].ToString() != null ? checkListRow[0].ToString().Trim() : null;
                            String ShopName = checkListRow[1].ToString() != null ? checkListRow[1].ToString().Trim() : null;
                            String TokenNumber = checkListRow[2].ToString() != null ? checkListRow[2].ToString().Trim() : null;
                            String AssociateName = checkListRow[3].ToString() != null ? checkListRow[3].ToString().Trim() : null;
                            String LineName = checkListRow[4].ToString() != null ? checkListRow[4].ToString().Trim() : null;
                            String ShiftName = checkListRow[5].ToString() != null ? checkListRow[5].ToString().Trim() : null;
                            String SetupName = checkListRow[6].ToString() != null ? checkListRow[6].ToString().Trim() : null;
                            String StationName = checkListRow[7].ToString() != null ? checkListRow[7].ToString().Trim() : null;
                            String SkillSet = checkListRow[8].ToString() != null ? checkListRow[8].ToString().Trim() : null;
                            String LineOfficer = checkListRow[9].ToString() != null ? checkListRow[9].ToString().Trim() : null;
                            String OperatorTokenNo = checkListRow[10].ToString() != null ? checkListRow[10].ToString().Trim() : null;
                            String ManagerName = checkListRow[11].ToString() != null ? checkListRow[11].ToString().Trim() : null;
                            String ManagerTokenNo = checkListRow[12].ToString() != null ? checkListRow[12].ToString().Trim() : null;

                            orderUploadRecordsObj[i] = new AllConfigurationUploadRecords();

                            RS_All_Configuration obj = new RS_All_Configuration();
                            RS_AM_Employee_SkillSet maes = new RS_AM_Employee_SkillSet();


                            var plantId = db.RS_Plants.Where(m => m.Plant_Name == PlantName).Select(m => m.Plant_ID).FirstOrDefault();
                            var shopId = db.RS_Shops.Where(m => m.Shop_Name == ShopName).Select(m => m.Shop_ID).FirstOrDefault();
                            var lineId = db.RS_Lines.Where(m => m.Line_Name == LineName).Select(m => m.Line_ID).FirstOrDefault();
                            var stationId = db.RS_Stations.Where(m => m.Station_Name == StationName).Select(m => m.Station_ID).FirstOrDefault();
                            var shiftId = db.RS_Shift.Where(m => m.Shift_Name == ShiftName).Select(m => m.Shift_ID).FirstOrDefault();
                            var skillId = db.RS_AM_Skill_Set.Where(m => m.Skill_Set == SkillSet).Select(m => m.Skill_ID).FirstOrDefault();


                            var empId = db.RS_Employee.Where(m => m.Employee_No == TokenNumber).Select(m => m.Employee_ID).FirstOrDefault();
                            var LineOfficerId = db.RS_Employee.Where(m => m.Employee_No == OperatorTokenNo).Select(m => m.Employee_ID).FirstOrDefault();
                            var ManagerId = db.RS_Employee.Where(m => m.Employee_No == ManagerTokenNo).Select(m => m.Employee_ID).FirstOrDefault();


                            if (db.RS_All_Configuration.Where(m => m.Token_No == TokenNumber && m.Setup_Name == SetupName && m.Excel_No == ExcelNumber + 1).Count() > 0)
                            {
                                continue;
                            }
                            else
                            {
                                if (PlantName != "" && ShopName != "" && LineName != "" && StationName != "" && TokenNumber != "" && OperatorTokenNo != "")
                                {
                                    obj.Plant_Name = PlantName;
                                    obj.Shop_Name = ShopName;
                                    obj.Line_Name = LineName;
                                    obj.Station_Name = StationName;
                                    obj.Shift_Name = ShiftName;
                                    obj.Token_No = TokenNumber;
                                    obj.Skill_Set = SkillSet;
                                    obj.Line_Officer = LineOfficer;
                                    obj.Manager_Name = ManagerName;
                                    obj.Line_Officer_Token = OperatorTokenNo;
                                    obj.Manager_Token = ManagerTokenNo;
                                    obj.Status = false;
                                    obj.FromDate = fromDate;
                                    obj.ToDate = toDate;
                                    obj.FromDay = fromDay.ToString();
                                    obj.ToDay = day.ToString();
                                    obj.Inserted_Date = DateTime.Now;
                                    obj.Excel_No = ExcelNumber + 1;
                                    obj.Setup_Name = SetupName;
                                    db.RS_All_Configuration.Add(obj);
                                    db.SaveChanges();
                                }
                            }

                            i = i + 1;
                        }
                        //var Excel_Upload= db.Sp_All_Configuration(ExcelNumber+1).ToList();
                        //AllConfigurationUploadRecords orderUploadObj = new AllConfigurationUploadRecords();
                        //int count = 0;
                        //int totalCount = 0;
                        //int totalSuccess = 0;
                        //int totalError = 0;
                        //foreach (var item in Excel_Upload)
                        //{

                        //    orderUploadRecordsObj[count].Plant_Name = item.Plant_Name;
                        //    orderUploadRecordsObj[count].Shop_Name = item.Shop_Name;
                        //    orderUploadRecordsObj[count].Line_Name = item.Line_Name;
                        //    orderUploadRecordsObj[count].Station_Name = item.Station_Name;
                        //    orderUploadRecordsObj[count].Setup_Name = item.Setup_Name;
                        //    orderUploadRecordsObj[count].Shift_Name = item.Shift_Name;
                        //    orderUploadRecordsObj[count].Token_No = item.Token_No;
                        //    orderUploadRecordsObj[count].Skill_Set = item.Skill_Set;
                        //    orderUploadRecordsObj[count].Line_Officer_Name = item.Line_Officer;
                        //    orderUploadRecordsObj[count].LineOfficer_Token_No = item.Line_Officer_Token;
                        //    orderUploadRecordsObj[count].Manager_Name = item.Manager_Name;
                        //    orderUploadRecordsObj[count].Manager_Token_No = item.Manager_Token;
                        //    orderUploadRecordsObj[count].SS_Error_Sucess = item.Error_Msg != null ? item.Error_Msg : "Record add successfully";

                        //    totalCount++;
                        //    if (orderUploadRecordsObj[count].SS_Error_Sucess == "Record add successfully")
                        //        totalSuccess++;
                        //    else
                        //        totalError++;

                        //    count++;
                        //}

                        TempData["OrderUploadRecords"] = orderUploadRecordsObj;

                        //ViewBag.TotalRecord = totalCount;
                        //ViewBag.TotalSuccess = totalSuccess;
                        //ViewBag.TotalError = totalError;

                        ViewBag.OrderUploadRecords = orderUploadRecordsObj;
                        ViewBag.dt = orderUploadRecordsObj;
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = "All Configuration";
                        globalData.messageDetail = ResourceGlobal.Excel + " " + ResourceMessages.Upload_Success;
                        globalData.pageTitle = ResourceGlobal.Excel + " " + "All Configuration" + " " + ResourceGlobal.Form;
                        globalData.subTitle = ResourceGlobal.Upload;
                        globalData.controllerName = "AddUsers";
                        globalData.actionName = ResourceGlobal.Upload;
                        globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
                        globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
                        ViewBag.GlobalDataModel = globalData;

                        ViewBag.createdOrders = createdOrders;
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
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

        /*	    Action Name		    : Dispose
        *		Description		    : To clear the memory allocated by objects
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : disposing bool value 
        *		Return Type		    :
        *		Revision		    :
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        /*	    Action Name		    : Create()
        *		Description		    : To display the excel information
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        public ActionResult Upload()
        {
          
            globalData.pageTitle = ResourceModules.User_Config;
            globalData.subTitle = ResourceGlobal.Excel + " " + ResourceGlobal.Upload;
            globalData.controllerName = "AddUsers";
            globalData.actionName = ResourceGlobal.Upload;
           

            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }



        /*	    Action Name		    : uploadFile
        *		Description		    : To upload the excel sheet
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : RS_Employee object 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        //GET: GET The file from upload control 
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult uploadFile(HttpPostedFileBase files, [Bind(Include = "Employee_ID,Employee_Name,Employee_Password,Employee_No,Email_Address,DOB,Contact_No,Gender,Plant_ID,Shop_ID,Line_ID,Category_ID,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Employee RS_Employee)
        {
            RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping = new RS_AM_Shop_Manager_Mapping();
            RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping = new RS_AM_Line_Supervisor_Mapping();
            RS_AM_UserPlant RS_AM_UserPlant = new RS_AM_UserPlant();
            GlobalOperations globalOperations = new GlobalOperations();
            string fileName = Path.GetFileName(files.FileName);
            string fileExtension = Path.GetExtension(files.FileName);
            string fileLocation = Server.MapPath("~/App_Data/" + fileName);
            DataTable dt = globalOperations.ExcelToDataTable(files, fileLocation, fileExtension);


            try
            {
                DataTable dtNotInserted = dt.Clone();
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                   
                    email = Convert.ToString(dr["Email_Address"].ToString().Trim());
                    empno = Convert.ToString(dr["Employee_No"].ToString().Trim());
                    string empName = Convert.ToString(dr["Employee_Name"].ToString().Trim());
                    DateTime Date = Convert.ToDateTime(dr["DOB"]);
                    string DM = 1999 + "-" + Date;
                    DateTime dts = Convert.ToDateTime(DM);
                    DateTime DOB = dts;
                    
                    string contryCode = dr["Country_Code"].ToString().Trim();
                    string contactNo = dr["Contact_No"].ToString().Trim();
                    Boolean gender = Convert.ToBoolean(dr["Gender"]);
                    int Category = Convert.ToInt32(dr["Category_Name"]);
                    string Plant = "1"; //dr["Plant_Name"].ToString().Trim();
                    string Shop = dr["Shop_Name"].ToString().Trim();
                    string Line = dr["Line_Name"].ToString().Trim();
                    if (Category == null || Category > 7)
                    {
                        dtNotInserted.ImportRow(dt.Rows[i]);
                        i++;
                    }
                    else
                    {
                        if (Category == 3)
                        {
                            if (empno == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (empName == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (gender == null)
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (DOB == null)
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (contryCode == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (contactNo == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (Shop == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (contactNo == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }

                            else if (contactNo.Length != 10)
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            if (email == "")
                            {
                                
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (RS_Employee.IsEmailExists(email, 0))
                            {
                                
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (RS_Employee.IsEmpNoExists(empno, 0))
                            {
                                
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }

                            else //ModelState.IsValid
                            {
                                RS_Employee.Employee_Name = Convert.ToString(dr["Employee_Name"].ToString().Trim());
                                RS_Employee.Employee_Password = "e6e061838856bf47e1de730719fb2609";
                                RS_Employee.Employee_No = Convert.ToString(dr["Employee_No"].ToString().Trim());
                                RS_Employee.Email_Address = Convert.ToString(dr["Email_Address"].ToString().Trim());
                                RS_Employee.DOB = Convert.ToDateTime(dr["DOB"]);
                                RS_Employee.Country_Code = Convert.ToInt32(dr["Country_Code"]);
                                RS_Employee.Contact_No = Convert.ToDecimal(dr["Contact_No"]);
                                RS_Employee.Gender = Convert.ToBoolean(dr["Gender"]);
                                RS_Employee.Shop_ID = Convert.ToInt32(dr["Shop_Name"]);
                                RS_Employee.Plant_ID = 1;
                                RS_Employee.Inserted_Host = "1";
                                RS_Employee.Inserted_User_ID = 1;
                                RS_Employee.Inserted_Date = DateTime.Now;
                                db.RS_Employee.Add(RS_Employee);
                                db.SaveChanges();

                                RS_AM_UserPlant.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                RS_AM_UserPlant.Inserted_Date = DateTime.Now;
                                RS_AM_UserPlant.Inserted_Host = "1";
                                RS_AM_UserPlant.Plant_ID = 1;
                                RS_AM_UserPlant.Employee_ID = RS_Employee.Employee_ID;
                                db.RS_AM_UserPlant.Add(RS_AM_UserPlant);
                                db.SaveChanges();

                                RS_AM_Shop_Manager_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                RS_AM_Shop_Manager_Mapping.Inserted_Date = DateTime.Now;
                                RS_AM_Shop_Manager_Mapping.Inserted_Host = "1";
                                RS_AM_Shop_Manager_Mapping.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;// Convert.ToDecimal(RS_Employee.Plants_ID[0]);
                                RS_AM_Shop_Manager_Mapping.Shop_ID = 1;
                                RS_AM_Shop_Manager_Mapping.Employee_ID = RS_Employee.Employee_ID;
                                db.RS_AM_Shop_Manager_Mapping.Add(RS_AM_Shop_Manager_Mapping);
                                db.SaveChanges();

                                i++;
                            }



                        }

                        else if (Category == 2)
                        {
                            if (empno == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (empName == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (gender == null)
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (DOB == null)
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (contryCode == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (contactNo == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (Shop == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (Line == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (contactNo == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }

                            else if (contactNo.Length != 10)
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            if (email == "")
                            {
                                
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (RS_Employee.IsEmailExists(email, 0))
                            {
                                
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (RS_Employee.IsEmpNoExists(empno, 0))
                            {
                                
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }

                            else //ModelState.IsValid
                            {
                                RS_Employee.Employee_Name = Convert.ToString(dr["Employee_Name"].ToString().Trim());
                                RS_Employee.Employee_Password = "e6e061838856bf47e1de730719fb2609";
                                RS_Employee.Employee_No = Convert.ToString(dr["Employee_No"].ToString().Trim());
                                RS_Employee.Email_Address = Convert.ToString(dr["Email_Address"].ToString().Trim());
                                RS_Employee.DOB = Convert.ToDateTime(dr["DOB"]);
                                RS_Employee.Country_Code = Convert.ToInt32(dr["Country_Code"]);
                                RS_Employee.Contact_No = Convert.ToDecimal(dr["Contact_No"]);
                                RS_Employee.Gender = Convert.ToBoolean(dr["Gender"]);
                                RS_Employee.Shop_ID = Convert.ToInt32(dr["Shop_Name"]);
                                RS_Employee.Plant_ID = 1;
                                RS_Employee.Inserted_Host = "1";
                                RS_Employee.Inserted_User_ID = 1;
                                RS_Employee.Inserted_Date = DateTime.Now;
                                db.RS_Employee.Add(RS_Employee);
                                db.SaveChanges();

                                RS_AM_UserPlant.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                RS_AM_UserPlant.Inserted_Date = DateTime.Now;
                                RS_AM_UserPlant.Inserted_Host = "1";
                                RS_AM_UserPlant.Plant_ID = 1;
                                RS_AM_UserPlant.Employee_ID = RS_Employee.Employee_ID;
                                db.RS_AM_UserPlant.Add(RS_AM_UserPlant);
                                db.SaveChanges();

                                RS_AM_Line_Supervisor_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                RS_AM_Line_Supervisor_Mapping.Inserted_Date = DateTime.Now;
                                RS_AM_Line_Supervisor_Mapping.Inserted_Host = "1";
                                RS_AM_Line_Supervisor_Mapping.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId; //Convert.ToDecimal(RS_Employee.Plants_ID[0]);
                                RS_AM_Line_Supervisor_Mapping.Shop_ID = Convert.ToDecimal(RS_Employee.Shops_ID[0]);
                                RS_AM_Line_Supervisor_Mapping.Line_ID = Convert.ToInt32(Line);
                                RS_AM_Line_Supervisor_Mapping.Employee_ID = RS_Employee.Employee_ID;
                                db.RS_AM_Line_Supervisor_Mapping.Add(RS_AM_Line_Supervisor_Mapping);
                                db.SaveChanges();

                                i++;
                            }


                        }

                        else
                        {
                            if (empno == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (empName == "")
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (gender == null)
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }

                            else if (contactNo.Length != 10)
                            {
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (RS_Employee.IsEmailExists(email, 0))
                            {
                                
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }
                            else if (RS_Employee.IsEmpNoExists(empno, 0))
                            {
                                
                                dtNotInserted.ImportRow(dt.Rows[i]);
                                i++;
                            }

                            else //ModelState.IsValid
                            {
                                RS_Employee.Employee_Name = Convert.ToString(dr["Employee_Name"].ToString().Trim());
                                RS_Employee.Employee_Password = "e6e061838856bf47e1de730719fb2609";
                                RS_Employee.Employee_No = Convert.ToString(dr["Employee_No"].ToString().Trim());
                                RS_Employee.Email_Address = Convert.ToString(dr["Email_Address"].ToString().Trim());
                                RS_Employee.DOB = Convert.ToDateTime(dr["DOB"]);
                                RS_Employee.Country_Code = Convert.ToInt32(dr["Country_Code"]);
                                RS_Employee.Contact_No = Convert.ToDecimal(dr["Contact_No"]);
                                RS_Employee.Gender = Convert.ToBoolean(dr["Gender"]);
                                RS_Employee.Shop_ID = Convert.ToInt32(dr["Shop_Name"]);
                                RS_Employee.Plant_ID = 1;
                                RS_Employee.Inserted_Host = "1";
                                RS_Employee.Inserted_User_ID = 1;
                                RS_Employee.Inserted_Date = DateTime.Now;
                                db.RS_Employee.Add(RS_Employee);
                                db.SaveChanges();

                                RS_AM_UserPlant.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                RS_AM_UserPlant.Inserted_Date = DateTime.Now;
                                RS_AM_UserPlant.Inserted_Host = "1";
                                RS_AM_UserPlant.Plant_ID = 1;
                                RS_AM_UserPlant.Employee_ID = RS_Employee.Employee_ID;
                                db.RS_AM_UserPlant.Add(RS_AM_UserPlant);
                                db.SaveChanges();

                                i++;
                            }


                        }

                    }
                }
            }
            // }
            catch (Exception ex)
            {

            }




            globalData.pageTitle = ResourceModules.User_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "AddUsers";
            globalData.actionName = ResourceGlobal.Create;
           

            globalData.isSuccessMessage = true;
            
            globalData.messageDetail = ResourceGlobal.User + " " + ResourceMessages.Add_Success;
            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;
            return View("Index", db.RS_Employee.ToList());
        }


        /*	    Action Name		    : InsertIntoDataTable
        *		Description		    : To save the users information through excel sheet
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : RS_Employee object and datatabel dt 
        *		Return Type		    : boolean value
        *		Revision		    :
        */
        //Insert all Data of DataTable into respective Database Table
        private bool InsertIntoDataTable(DataTable dt, RS_Employee RS_Employee)
        {

            return true;
        }


        public ActionResult GetPlant(int Category_Id)
        {
            try
            {
               
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from plant in db.RS_Plants
                         where plant.Plant_ID == plantID
                         orderby plant.Plant_Name
                        
                         select new
                         {
                             Id = plant.Plant_ID,
                             Value = plant.Plant_Name
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult GetStageByBWTID(int lineId)
        {
            try
            {
                var st = from stage in db.RS_Stations
                         where stage.Line_ID == lineId
                         orderby stage.Station_Name
                         select new
                         {
                             Id = stage.Station_ID,
                             Value = stage.Station_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetLineByShopID(int[] shopId)
        {
            try
            {
                decimal shop = Convert.ToInt32(shopId[0]);
                var st = from line in db.RS_Lines
                         where line.Shop_ID == shop
                         orderby line.Line_Name
                         select new
                         {
                             Id = line.Line_ID,
                             Value = line.Line_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetShopByPlantID(int[] plantId)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from shop in db.RS_Shops
                         where shop.Plant_ID == plantID
                         orderby shop.Shop_Name
                         select new
                         {
                             Id = shop.Shop_ID,
                             Value = shop.Shop_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetShop()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                
                var st = from shop in db.RS_Shops
                         where shop.Plant_ID == plantID
                         orderby shop.Shop_Name
                         select new
                         {
                             Id = shop.Shop_ID,
                             Value = shop.Shop_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult GetAge(string date)
        {
            if (date != "")
            {
                DateTime bdate = Convert.ToDateTime(date);

                DateTime now = DateTime.Today;
                int age = now.Year - bdate.Year;
                if (now < bdate.AddYears(age)) age--;

                RS_Employee emp = new RS_Employee
                {
                    //Age = age.ToString()
                };
                return Json(emp);
            }
            return Json(false);
        }

        public ActionResult GetExperience(string date)
        {
            if (date != "")
            {
                DateTime jdate = Convert.ToDateTime(date);
                DateTime now = DateTime.Today;
                int exp = now.Year - jdate.Year;
                if (now < jdate.AddYears(exp)) exp--;
                RS_Employee emp = new RS_Employee
                {
                    //Experiance = exp.ToString()
                };
                return Json(emp);
            }
            return Json(false);
        }
        

        public ActionResult OJTDataUpload()
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

                globalData.pageTitle = "OJT Training Data Upload";
                globalData.subTitle = ResourceGlobal.Upload;
                globalData.controllerName = "EmployeeSkillSet";
                globalData.actionName = ResourceGlobal.Upload;
                globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Add_User + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Add_User + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }


        public FileResult DownloadOJT(string fileName)
        {
            var FileVirtualPath = "~/App_Data/" + fileName;

            string file = @"~\App_Data\OJT_Training_Configuration.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult OJTDataUpload(OJTUpload formData)
        {
            String createdOrders = "";
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
                    OJTUpload1[] orderUploadRecordsObj = new OJTUpload1[dt.Rows.Count];
                    //RS_Maintenance_Machine_Part mmOrderCreationObj = new RS_Maintenance_Machine_Part();

                    int i = 0;
                    //int ExcelNumber = db.RS_All_Configuration.Max(m => m.Excel_No).Value;
                    foreach (DataRow checkListRow in dt.Rows)
                    {

                        String PlantName = checkListRow[0].ToString() != null ? checkListRow[0].ToString().Trim() : null;
                        String ShopName = checkListRow[1].ToString() != null ? checkListRow[1].ToString().Trim() : null;
                        String TokenNumber = checkListRow[2].ToString() != null ? checkListRow[2].ToString().Trim() : null;
                        String LineName = checkListRow[3].ToString() != null ? checkListRow[3].ToString().Trim() : null;
                        String StationName = checkListRow[6].ToString() != null ? checkListRow[6].ToString().Trim() : null;
                        String ShiftName = checkListRow[4].ToString() != null ? checkListRow[4].ToString().Trim() : null;
                        String SetupName = checkListRow[5].ToString() != null ? checkListRow[5].ToString().Trim() : null;
                        String OJTDATE = checkListRow[7].ToString() != null ? checkListRow[7].ToString().Trim() : null;

                        orderUploadRecordsObj[i] = new OJTUpload1();
                        OJTUpload1 oJTUpload1 = new OJTUpload1();
                        RS_On_Job_Training_Data obj = new RS_On_Job_Training_Data();
                        oJTUpload1.Plant_Name = PlantName;
                        oJTUpload1.Shop_Name = ShopName;
                        oJTUpload1.Token_No = TokenNumber;
                        oJTUpload1.Line_Name = LineName;
                        oJTUpload1.Shift_Name = ShiftName;
                        oJTUpload1.Setup_Name = SetupName;
                        oJTUpload1.Station_Name = StationName;
                        oJTUpload1.OJT_Date=(OJTDATE);





                        var plantId = db.RS_Plants.Where(m => m.Plant_Name == PlantName).Select(m => m.Plant_ID).FirstOrDefault();
                        var shopId = db.RS_Shops.Where(m => m.Shop_Name == ShopName).Select(m => m.Shop_ID).FirstOrDefault();
                        var lineId = db.RS_Lines.Where(m => m.Line_Name == LineName).Select(m => m.Line_ID).FirstOrDefault();
                        var stationId = db.RS_Stations.Where(m => m.Station_Name == StationName).Select(m => m.Station_ID).FirstOrDefault();
                        var shiftId = db.RS_Shift.Where(m => m.Shift_Name == ShiftName).Select(m => m.Shift_ID).FirstOrDefault();
                        
                        var setupId = db.RS_Setup.Where(m => m.RS_Lines.Line_Name == LineName).Select(m => m.Setup_ID).FirstOrDefault();

                        var empId = db.RS_Employee.Where(m => m.Employee_No == TokenNumber).Select(m => m.Employee_ID).FirstOrDefault();

                        DateTime date = Convert.ToDateTime(OJTDATE);
                        if (db.RS_On_Job_Training_Data.Where(m => m.RS_Employee.Employee_ID == empId && m.OJT_Date == date).Count() > 0)
                        {
                            
                            {
                                oJTUpload1.SS_Error_Sucess = "Record already Exists";
                            }
                            orderUploadRecordsObj[i] = oJTUpload1;
                            i = i + 1;
                            continue;

                        }
                        else
                        {
                            if (PlantName != "" && ShopName != "" && SetupName != "" && LineName != "" && StationName != "" && TokenNumber != "" && OJTDATE != "")
                            {
                                obj.Plant_ID = plantId;
                                obj.Shop_ID = shopId;
                                obj.Line_ID = lineId;
                                obj.Station_ID = stationId;
                                obj.Shift_ID = shiftId;
                                obj.Employee_ID = empId;
                                obj.Setup_ID = setupId;
                                obj.Inserted_Date = DateTime.Now;
                                obj.Inserted_Host = HttpContext.Request.UserHostAddress;
                                obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                obj.OJT_Date = Convert.ToDateTime(OJTDATE);
                                db.RS_On_Job_Training_Data.Add(obj);
                                db.SaveChanges();
                                oJTUpload1.SS_Error_Sucess = "Record add successfully";
                            }
                            else
                            {
                                oJTUpload1.SS_Error_Sucess = "Error";
                            }

                            orderUploadRecordsObj[i] = oJTUpload1;
                        }


                        i = i + 1;

                    }

                    TempData["OrderUploadRecords"] = orderUploadRecordsObj;
                   
                    ViewBag.OrderUploadRecords = orderUploadRecordsObj;
                    ViewBag.dt = orderUploadRecordsObj;
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "All Configuration";
                    globalData.messageDetail = "Excel Upload successfully";
                    globalData.pageTitle = ResourceGlobal.Excel + " " + "All Configuration" + " " + ResourceGlobal.Form;
                    globalData.subTitle = ResourceGlobal.Upload;
                    globalData.controllerName = "AddUsers";
                    globalData.actionName = ResourceGlobal.Upload;
                    ViewBag.GlobalDataModel = globalData;

                    ViewBag.createdOrders = createdOrders;
                }

                

            }

            return View();
        }

        private static OJTUpload1 GetOrderUploadObj(OJTUpload1 orderUploadObj)
        {
            return orderUploadObj;
        }

    }
}
