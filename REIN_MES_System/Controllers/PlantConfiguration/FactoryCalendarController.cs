using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;

using ZHB_AD.Controllers.BaseManagement;

namespace ZHB_AD.Controllers.PlantConfiguration
{
    public class FactoryCalendarController : BaseController
    {


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoadCalendar()
        {
            return View();
        }

        //public string Init()
        //{
        //    //bool rslt = Utils.InitialiseDiary();
        //   // return rslt.ToString();
        //}

        public void UpdateEvent(int id, string NewEventStart, string NewEventEnd)
        {
            DiaryEvent.UpdateDiaryEvent(id, NewEventStart, NewEventEnd);
        }


        public bool SaveEvent(string Title, string NewEventDate, string EventNote = "")
        {
            return DiaryEvent.CreateNewEvent(Title, NewEventDate, EventNote);
        }

        public bool SaveHoliday(string Title, string NewDate1, string NewDate2)
        {
            return DiaryEvent.CreateNewHoliday(Title, NewDate1, NewDate2);
        }


        public JsonResult GetDiarySummary(double start, double end)



        {
            var ApptListForDate = DiaryEvent.LoadAppointmentSummaryInDateRange(start, end);
            var eventList = from e in ApptListForDate
                            select new
                            {
                                id = e.ID,
                                title = e.Title,
                                start = e.StartDateString,
                                end = e.EndDateString,
                                someKey = e.SomeImportantKeyID,
                                allDay = false
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDiaryEvents(double start, double end)
        {
            var ApptListForDate = DiaryEvent.LoadAllAppointmentsInDateRange(start, end);
            var eventList = from e in ApptListForDate
                            select new
                            {
                                id = e.ID,
                                title = e.Title,
                                start = e.StartDateString,
                                end = e.EndDateString,
                                color = e.StatusColor,
                                className = e.ClassName,
                                someKey = e.SomeImportantKeyID,
                                allDay = true,
                                EventNote=e.EventNote
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
    }
}
