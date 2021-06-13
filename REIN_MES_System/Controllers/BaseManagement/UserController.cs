using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.BaseManagement
{
    public class UserController : Controller
    {
        FDSession fdSession = new FDSession();
        General general = new General();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_Employee users = new RS_Employee();
        GlobalData globalData = new GlobalData();

        /* Action Name                : Index
        *  Description                : Action used to check the user session and redirect to login if session is empty
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        //RS_User_Roles userRole;
        // GET: Login
        public ActionResult Index()
        {
            try
            {
                // get session objects
                fdSession = (FDSession)this.Session["FDSession"];

                // check user is login or not
                if (fdSession == null || fdSession.userId == 0)
                {
                    String userIpAddress = "";
                    if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                    {
                        userIpAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                    }
                    else if (Request.UserHostAddress.Length != 0)
                    {
                        userIpAddress = Request.UserHostAddress;
                    }
                    ViewBag.IPAddress = userIpAddress;
                    //general.addControllerException(null, "UserController", "Index(" + userIpAddress + ")");

                    decimal stationID = getStationID(userIpAddress);
                    //-----------------ADD STATION ID IN WEB CONFIG FOR TESTING------------------------------------
                    decimal configStationID = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings.Get("StationID"));
                    if (configStationID > 0)
                    {
                        stationID = configStationID;
                    }
                    //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                    if (stationID > 0)//CHECK IF IP ASSIGNED TO ANY STATION
                    {
                        FDSession fdSessionObj = new FDSession();
                        fdSessionObj.stationId = (int)stationID;

                        RS_Stations stationObj = db.RS_Stations.Where(a => a.Station_ID == stationID).FirstOrDefault();
                        fdSessionObj.stationName = stationObj.Station_Name;
                        fdSessionObj.plantId = (int)stationObj.RS_Shops.RS_Plants.Plant_ID;
                        fdSessionObj.shopId = (int)stationObj.Shop_ID;
                        fdSessionObj.lineId = (int)stationObj.Line_ID;
                        fdSessionObj.userHost = userIpAddress;
                        bool isorderstart = false;
                        if(stationObj.Is_Order_Start!=true)
                        {
                            isorderstart = false;
                        }
                        fdSessionObj.isOrderStartStation = isorderstart;
                        var stationRoleObj = db.RS_Station_Roles.Where(a => a.Station_ID == stationID).ToList();

                        if (stationRoleObj.Count > 0)
                        {
                            List<decimal> roleList = new List<decimal>();
                            foreach (var station in stationRoleObj)
                            {
                                roleList.Add(station.Role_ID);
                            }

                            IEnumerable<RS_Menus> menuObj = db.RS_Menu_Role.Where(a => roleList.Contains(a.Role_ID)).Select(a => a.RS_Menus).Distinct().ToList();
                            fdSessionObj.menuObj = menuObj;
                        }
                        this.Session["FDSession"] = fdSessionObj;

                        if (checkifStationCritical(stationID))//CHECK IF STATION IS CRITICAL STATION
                        {
                            return RedirectToAction("Create", "CriticalStationLogin");
                        }
                        else
                        {
                            //if(fdSessionObj.isOrderStartStation==true)
                            //{
                            //    return RedirectToAction("WorkInstructionSubmission", "WorkInstruction");
                            //}
                            //else
                            //{
                                return View("ShopLogin");
                            //}
                         
                        }
                    }
                    else //(ADMIN USERS)
                    {
                        //-----------------KEEP THIS CODE FOR PRODUCTION------------------------------------
                        string winFullLoginName = "";
                        //winFullLoginName = System.Configuration.ConfigurationManager.AppSettings.Get("TokenNo").ToString();
                        //if (String.IsNullOrWhiteSpace(winFullLoginName))
                        //{
                        //    if (!String.IsNullOrWhiteSpace(HttpContext.User.Identity.Name))
                        //    {
                        //        winFullLoginName = HttpContext.User.Identity.Name;
                        //    }
                        //    else if (!String.IsNullOrWhiteSpace(HttpContext.Request.LogonUserIdentity.Name))
                        //    {
                        //        winFullLoginName = HttpContext.Request.LogonUserIdentity.Name;
                        //    }
                        //    else
                        //    {
                        //        globalData.isErrorMessage = true;
                        //        globalData.messageTitle = "Error";
                        //        globalData.messageDetail = "System Login credentials not found !";
                        //        ViewBag.GlobalDataModel = globalData;
                        //        return View("NotificationPage");
                        //    }
                        //}

                        //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                        try
                        {

                            string userhost = System.Environment.MachineName;
                            winFullLoginName = Request.LogonUserIdentity.Name;
                            String[] LoginSplit = winFullLoginName.Split(new char[] { '\\' });
                            if (LoginSplit.Count() < 2)
                            {
                                globalData.isErrorMessage = true;
                                globalData.messageTitle = "Error";
                                globalData.messageDetail = "Login details are not valid. User - " + winFullLoginName;
                                ViewBag.GlobalDataModel = globalData;
                                return View("NotificationPage");
                            }

                            string winLoginUserID = LoginSplit[1].ToString();
                            String userName = winLoginUserID;
                            FDSession adsessionobj = new FDSession();
                            var UserObj1 = (from e in db.RS_Employee
                                            join userplant in db.RS_AM_UserPlant
                                            on e.Employee_ID equals userplant.Employee_ID
                                            join plant in db.RS_Plants
                                            on userplant.Plant_ID equals plant.Plant_ID
                                            where e.Employee_No == userName
                                            select new
                                            {
                                                userplant.Plant_ID,
                                                plant.Plant_Name,
                                                e.Employee_ID,
                                                e.Employee_Name,
                                                e.Inserted_Date

                                            }).Distinct().ToList();

                            if (UserObj1 != null && UserObj1.Count!= 0)//IF USER EXIST IN EMPLOYEE TABLE
                            {
                                if (UserObj1.Count == 1)
                                {
                                    adsessionobj.plantId = (int)UserObj1[0].Plant_ID;
                                    adsessionobj.userId = (int)UserObj1[0].Employee_ID;
                                    adsessionobj.userName = UserObj1[0].Employee_Name;
                                    adsessionobj.userHost = userhost;
                                    adsessionobj.plantName = UserObj1[0].Plant_Name;
                                    this.Session["FDSession"] = adsessionobj;
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {


                                    //  fdSessionObj.plantId = (int)userObj.Plant_ID;
                                    adsessionobj.userId = (int)UserObj1[0].Employee_ID;
                                    adsessionobj.userName = UserObj1[0].Employee_Name;
                                    adsessionobj.userHost = userhost;
                                    adsessionobj.insertedDate = (DateTime)UserObj1[0].Inserted_Date;
                                    this.Session["FDSession"] = adsessionobj;
                                    ViewBag.Plant_ID = new SelectList(UserObj1, "Plant_ID", "Plant_Name");
                                    return View("PlantList", ViewBag.Plant_ID);
                                }
                            }
                            //RS_Employee userObj = db.RS_Employee.Where(a => a.Employee_No == userName).FirstOrDefault();
                            //if (userObj != null)//IF USER EXIST IN EMPLOYEE TABLE
                            //{
                            //    FDSession fdSessionObj = new FDSession();
                            //    fdSessionObj.stationId = 0;
                            //    fdSessionObj.plantId = (int)userObj.Plant_ID;
                            //    fdSessionObj.userId = (int)userObj.Employee_ID;
                            //    fdSessionObj.userName = userObj.Employee_Name;
                            //    fdSessionObj.userHost = userIpAddress;
                            //    fdSessionObj.insertedDate = (DateTime)userObj.Inserted_Date;
                            //    this.Session["FDSession"] = fdSessionObj;

                            //    String defaultController = "Home";
                            //    String defaultAction = "Index";

                            //    MM_AM_Default_URL defaultUrlObj = db.MM_AM_Default_URL.Where(p => p.Plant_ID == userObj.Plant_ID && p.Employee_ID == userObj.Employee_ID).FirstOrDefault();
                            //    if (defaultUrlObj != null)
                            //    {
                            //        defaultController = defaultUrlObj.Default_Controller;
                            //        defaultAction = defaultUrlObj.Default_Action;
                            //    }

                            //    // redirect the user to dashboard page softuser
                            //    return RedirectToAction(defaultAction, defaultController);

                            //}
                            else//USER NOT FOUND IN SYSTEM
                            {
                                globalData.isErrorMessage = true;
                                globalData.messageTitle = "Error";
                                globalData.messageDetail = "Login details are not valid. User - " + winFullLoginName;
                                ViewBag.GlobalDataModel = globalData;
                                return View("NotificationPage");
                            }
                        }
                        catch (Exception exp)
                        {
                            General genObj = new General();
                            genObj.addControllerException(exp, "User", "Index", 1);

                            globalData.isErrorMessage = true;
                            globalData.messageTitle = "Error";
                            globalData.messageDetail = winFullLoginName + " --- " + exp.Message;
                            ViewBag.GlobalDataModel = globalData;
                            return View("NotificationPage");
                        }
                    }
                }
                else
                {
                    // redirect the user to landing page
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception exp)
            {
                General genObj = new General();
                genObj.addControllerException(exp, " User", "Index", 1);

                globalData.isErrorMessage = true;
                globalData.messageTitle = "Error";
                globalData.messageDetail = exp.Message;
                ViewBag.GlobalDataModel = globalData;
                return View("NotificationPage");
            }
        }

        private decimal getStationID(string ipAddr)
        {
            try
            {
                var stationObj = db.RS_Stations.Where(a => a.Station_IP_Address == ipAddr).FirstOrDefault();
                if (stationObj == null)
                {
                    return 0;
                }
                else
                {
                    return stationObj.Station_ID;
                }
            }
            catch (Exception ex)
            {
                General genObj = new General();
                genObj.addControllerException(ex, "UserController", "getStationID", 1);
                return 0;
            }
        }
        private bool checkifStationCritical(decimal Station_ID)
        {
            try
            {
                return db.RS_Stations.Any(a => a.Station_ID == Station_ID && a.Is_Critical_Station == true);
            }
            catch (Exception ex)
            {
                General genObj = new General();
                genObj.addControllerException(ex, "UserController", "checkifStationCritical", 1);
                return false;
            }
        }

        public ActionResult NotificationPage()
        {
            return View();
        }

        /* Action Name                : CheckLogin
        *  Description                : Action used to check user credentials added in login form and show success, error messages
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : userLogin
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        [HttpPost]
        public ActionResult CheckLogin(UserLogin userLogin)
        {
            try
            {
                if (userLogin == null)
                {
                    return View("ShopLogin");
                }
                else if (userLogin.User_Name == "mainlineuser")
                {
                    fdSession.userId = 5;
                    fdSession.userName = "Main Line User";
                    this.Session["FDSession"] = fdSession;

                    return RedirectToAction("ShopScreen", "Manifest");
                }
                else
                {
                    String userName = userLogin.User_Name;
                    String password = userLogin.User_Password;
                    String encrptedPassword = general.getEncryptedString(password);

                    var query = db.RS_Employee.Where(p => p.Employee_No == userName && p.Employee_Password == encrptedPassword);
                    users = query.FirstOrDefault<RS_Employee>();
                    if (users != null)
                    {

                        fdSession = (FDSession)this.Session["FDSession"];
                        // get the user details and store in session
                        fdSession.userId = (int)users.Employee_ID;
                        fdSession.userName = users.Employee_Name;
                        fdSession.insertedDate = (DateTime)users.Inserted_Date;
                        fdSession.userRoleId = (int)users.Role_ID;

                        this.Session["FDSession"] = fdSession;

                        return RedirectToAction("ShopScreen", "Manifest");

                    }
                    else
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = "Error";
                        globalData.messageDetail = "Login details are not valid.";
                        ViewBag.GlobalDataModel = globalData;
                        return View("Login", userLogin);
                    }
                }


                //   1.    //to do take ip address
                //   2.    //check if ip address is mapped to station
                //   3.   //if mapped chech whether user allocated to station, 
                //if yes than sho user screen and store station in session variable else aat madhe  if that station id is critical or not if yes than line stop logic else
                //show him invalid user message
                // process to validate the use record

                // else  of 2red step (ip adrees nt mapped to any station)than check user is valid or nt if user is valid than store role in session if invalid than show invalid message in login screen




                //return RedirectToAction("Index");
                // }
            }
            catch (Exception ex)
            {
                // exception occur, log the expection by module
                return View("Login", userLogin);
            }
        }

        /* Action Name                : CheckShopLogin
        *  Description                : Action used to check Shop User credentials
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : userLogin
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        [HttpPost]
        public ActionResult CheckShopLogin(UserLogin userLogin)
        {
            try
            {
                if (userLogin == null)
                {
                    return View("ShopLogin");
                }

                else
                {
                    String userName = userLogin.User_Name;
                    FDSession fdSessionObj = (FDSession)this.Session["FDSession"];
                    RS_Employee usersObj = db.RS_Employee.Where(p => p.Employee_No == userName).FirstOrDefault();
                    if(usersObj==null)
                    {
                        
                            globalData.isErrorMessage = true;
                            globalData.messageTitle = "Error";
                            globalData.messageDetail = "Login details are not valid for this Station.";
                            ViewBag.GlobalDataModel = globalData;
                            return View("ShopLogin", userLogin);
                    }
                    decimal empID = usersObj.Employee_ID;
                    int stationID = fdSessionObj.stationId;
                    int lineID = fdSessionObj.lineId;
                    RS_Quality_Captures cap = new RS_Quality_Captures();
                     var currentshift = cap.getCurrentRunningShiftByShopID(Convert.ToInt32(fdSessionObj.shopId));
                  
                    //bool isOperatorToStation = db.RS_AM_Operator_Station_Allocation.Any(a => a.Employee_ID == empID && a.Station_ID == stationID && a.Shift_ID == currentshift.Shift_ID);

                    // process to check employee is present or not for the day

                    //bool isOperatorToStation = true;
                    if (usersObj != null/* && isOperatorToStation == true*/)
                    {

                        // get the user details and store in session
                        fdSessionObj.userId = (int)usersObj.Employee_ID;
                        fdSessionObj.userName = usersObj.Employee_Name;
                        fdSessionObj.insertedDate = (DateTime)usersObj.Inserted_Date;
                        //fdSessionObj.userRoleId = (int)usersObj.Role_ID;

                        this.Session["FDSession"] = fdSessionObj;



                        // process to add user details
                        RS_AM_User_Sessions mmAMUserSessionObj = new RS_AM_User_Sessions();
                        mmAMUserSessionObj.Plant_ID = fdSessionObj.plantId;
                        mmAMUserSessionObj.Shop_ID = fdSessionObj.shopId;
                        mmAMUserSessionObj.Line_ID = fdSessionObj.lineId;
                        mmAMUserSessionObj.Station_ID = fdSessionObj.stationId;
                        mmAMUserSessionObj.Employee_ID = fdSessionObj.userId;
                        mmAMUserSessionObj.Session_ID = HttpContext.Session.SessionID;
                        mmAMUserSessionObj.Login_Date = DateTime.Now;
                        db.RS_AM_User_Sessions.Add(mmAMUserSessionObj);
                        db.SaveChanges();


                        //SHOW ORDER START SCREEN IF ORDERSTART STATION
                        //bool isLineInPartGroup = db.RS_Partgroup.Any(a => a.Line_ID == lineID && a.Station_ID == stationID);
                        bool isOrderstart = db.RS_Stations.Any(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Is_Order_Start==true);
                        bool isStartStation = db.RS_Route_Configurations.Any(a => a.Station_ID == stationID && a.Is_Start_Station == true);
                        if (isOrderstart)
                        {
                            //RS_Partgroup partGroupObj = db.RS_Partgroup.Where(a => a.Line_ID == lineID && a.Station_ID == stationID && a.Order_Create == true).FirstOrDefault();

                            //bool isMain = (from shop in db.RS_Shops
                            //               join st in db.RS_Stations on shop.Shop_ID equals st.Shop_ID
                            //               where st.Station_ID == stationID
                            //               select shop.Is_Main).FirstOrDefault();
                            //if (isMain && partGroupObj != null)
                            //{
                            //    return RedirectToAction("Create", "DockOrderStart");
                            //}
                            //else
                            //{
                                return RedirectToAction("Create", "OrderStart");
                            //}
                        }
                        else if(isOrderstart==false)
                        {
                            return RedirectToAction("WorkInstructionSubmission", "WorkInstruction");
                        }
                        //else if (new RS_Quality_Station_List().isStationAddedInQualityList(stationID))
                        //{
                        //    return RedirectToAction("Create", "QualityCaptures");
                        //}
                        //else
                        //{
                        //    return RedirectToAction("ShopScreen", "Manifest");
                        //}
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = "Error";
                        globalData.messageDetail = "Login details are not valid for this Station.";
                        ViewBag.GlobalDataModel = globalData;
                        return View("ShopLogin", userLogin);
                    }
                    else
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = "Error";
                        globalData.messageDetail = "Login details are not valid for this Station.";
                        ViewBag.GlobalDataModel = globalData;
                        return View("ShopLogin", userLogin);
                    }
                }
            }
            catch (Exception ex)
            {
                general.addShopControllerException(ex, "User", "CheckShopLogin", ((FDSession)this.Session["FDSession"]).stationId, ((FDSession)this.Session["FDSession"]).plantId, ((FDSession)this.Session["FDSession"]).shopId, ((FDSession)this.Session["FDSession"]).lineId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Error";
                globalData.messageDetail = "Something went wrong .Please Try Again !";
                ViewBag.GlobalDataModel = globalData;
                return View("ShopLogin", userLogin);
            }
        }

        public bool IsEmployeeAllocated(int userId)  //, int stationId
        {
            try
            {
                bool result = db.RS_AM_Operator_Station_Allocation.Any(p => p.Employee_ID == userId);
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsUserExist1(int userId)
        {
            try
            {
                bool result = db.RS_Employee.Any(p => p.Employee_ID == userId);
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public decimal IsUserExist(int userId)
        {
            try
            {
                decimal result = 0;

                result = db.RS_Employee.Where(p => p.Employee_ID == userId).FirstOrDefault().Employee_ID;


                if (result > 0)
                    return result;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public decimal IsCriticalStation(int stationId)
        {
            try
            {
                decimal result1 = 0;

                result1 = db.RS_Stations.Where(S => S.Station_ID == stationId && S.Is_Critical_Station == true).FirstOrDefault().Station_ID;


                if (result1 > 0)
                    return result1;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public decimal IsExistStation(string myIP)
        {
            try
            {
                decimal result = 0;
                result = db.RS_Stations.Where(p => p.Station_IP_Address == myIP).FirstOrDefault().Station_ID;


                if (result > 0)

                    return result;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //public decimal UserAllocatedToStation(decimal userId)
        //{
        //    try
        //    {
        //        decimal result = 0;

        //        result = db.RS_AM_Operator_Station_Allocation.Where(p => p.Employee_ID == userId).FirstOrDefault().Station_ID;


        //        if (result > 0)

        //            return result;
        //        else
        //            return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}

        //public decimal IsUserAllocatedToStation(decimal stationId)
        //{
        //    try
        //    {
        //        decimal result;

        //        result = db.RS_AM_Operator_Station_Allocation.Where(p => p.Station_ID == stationId).FirstOrDefault().Employee_ID;

        //        if (result > 0)

        //            return result;
        //        else
        //            return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}

        public ActionResult ShopLogin()
        {
            return View("ShopLogin");
        }

        public ActionResult Logout()
        {
            // process to add user details
            String sessionId = HttpContext.Session.SessionID;

            this.Session["FDSession"] = null;
            return RedirectToAction("Index");
        }

        /* Action Name                : ForgotPassword
        *  Description                : Action used to show forgot password screen
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult ForgotPassword()
        {
            return View();
        }

        
        public ActionResult GetMenuRoleData(string MenuID,string RoleID)
        {
            int RID = Convert.ToInt32(RoleID);
            int MID = Convert.ToInt32(MenuID);
            var RoleName = db.RS_Roles.Where(m => m.Role_ID == MID).Select(m => m.Role_Name).FirstOrDefault();
            var MenuName = db.RS_Menus.Where(m => m.Menu_ID == RID).Select(m => m.LinkName).FirstOrDefault();
            return Json(new { Role = RoleName, Menu = MenuName}, JsonRequestBehavior.AllowGet);
        }
        /* Action Name                : ExecuteCore
        *  Description                : Action used to set the user culture
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : None
        *  Return Type                : void
        *  Revision                   : 1.0
        */
        protected override void ExecuteCore()
        {
            int culture = 0;
            if (this.Session == null || this.Session["CurrentCulture"] == null)
            {

                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Culture"], out culture);
                this.Session["CurrentCulture"] = culture;
            }
            else
            {
                culture = (int)this.Session["CurrentCulture"];
            }
            // calling CultureHelper class properties for setting  
            CultureHelper.CurrentCulture = culture;

            base.ExecuteCore();

            //this.IsUserLogin();
        }

        public ActionResult ChangePlant(int plant_id)
        {
            FDSession fdSessionObj = new FDSession();
            FDSession adSession = (FDSession)this.Session["FDSession"];
            var plantName = db.RS_Plants.Where(m => m.Plant_ID == plant_id).Select(m => m.Plant_Name).FirstOrDefault();

            adSession.plantName = plantName;
            adSession.plantId = plant_id;

            this.Session["FDSession"] = adSession;
            if (plant_id != 0)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Error";
                globalData.messageDetail = "Login details are not valid. User - ";
                ViewBag.GlobalDataModel = globalData;
                return View("NotificationPage");
            }
        }
    }
}