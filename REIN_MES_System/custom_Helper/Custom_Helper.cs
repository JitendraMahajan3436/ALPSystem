using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.custom_Helper
{
    public static class Custom_Helper
    {
        public static MvcHtmlString YesNo(this HtmlHelper htmlhelper,bool yesno)
        {
            var text = yesno ? "Yes" : "No";
            return new MvcHtmlString(text);
        }
        public static MvcHtmlString YesNo(this HtmlHelper htmlhelper, string yesno)
        {
            var text = "";
            if (yesno.Equals("1")|| yesno.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                text = "Yes";
            }
            else if (yesno.Equals("0")|| yesno.Equals("false", StringComparison.CurrentCultureIgnoreCase))
            {
                text = "No";
            }
          
            return new MvcHtmlString(text);
        }
    }
}