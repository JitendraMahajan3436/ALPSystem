using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class GlobalData
    {
        // User Navigation Contents
        public String controllerName { get; set; }
        public String actionName { get; set; }

        // Footer Content
        public String plantName { get; set; }
        public String serverIPAddress { get; set; }
        public String osUSer { get; set; }
        public String stationIPAddress { get; set; }
        public String hostName { get; set; }
        public String serverDate { get; set; }
        public String serverTime { get; set; }

        // User Content
        public String fdUserName { get; set; }
        public String fdUserImage { get; set; }
        public int fdUserId { get; set; }

        // Page Content
        public String pageTitle { get; set; }
        public String subTitle { get; set; }
        public String contentTitle { get; set; }
        public String contentFooter { get; set; }

        // Message Content
        public String messageTitle { get; set; }
        public String messageDetail { get; set; }
        public bool isSuccessMessage { get; set; }
        public bool isErrorMessage { get; set; }
        public bool isAlertMessage { get; set; }

        //Shop Screen Content
        public String ShopTitle { get; set; }

        /* Method Name                : GlobalData
         *  Description                : Constructor of GlobalData class
         *  Author, Timestamp          : Ajay Wagh
         *  Input parameter            : None
         *  Return Type                : None
         *  Revision                   : 1.0
         */
        public GlobalData()
        {
            controllerName = "Home";
            actionName = "Index";
            plantName = "";
            serverIPAddress = "";
            osUSer = "Administrator";
            //stationIPAddress = "10.192.66.203";
            stationIPAddress = HttpContext.Current.Request.UserHostAddress;
            hostName = "";
            serverDate = DateTime.Now.ToString("d");
            serverTime = DateTime.Now.ToString("T");

            fdUserName = "Administrator";
            fdUserImage = "";
            fdUserId = 1;

            pageTitle = "";
            subTitle = "";
            contentTitle = "";
            contentFooter = "";
        }
    }
}