using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;

namespace ZHB_AD.Controllers.MasterManagement
{
    public class ParameterMasterController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();
        // GET: ParameterMaster
        public ActionResult Index()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                //Session["PageTitle"] = "Main Feeder (Meter) Configuration";
                globalData.pageTitle = ResourceParameterMaster.pageTitle;

                //globalData.controllerName = "StationRoles";
                //globalData.actionName = ResourceGlobal.Details;
                //globalData.contentTitle = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
                //globalData.contentFooter = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
                ViewBag.GlobalDataModel = globalData;
                //return View(db.MM_Parameter.ToList());
                return View((from p in db.MM_Parameter
                            
                             select (p)).ToList());

            }
            catch
            {
                return RedirectToAction("Index", "user");
            }
          
        }

        // GET: ParameterMaster/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Parameter mM_Parameter = db.MM_Parameter.Find(id);
            if (mM_Parameter == null)
            {
                return HttpNotFound();
            }
            return View(mM_Parameter);
        }

        // GET: ParameterMaster/Create
        public ActionResult Create()
        {
            try
            {
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                int PlantId = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceParameterMaster.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name");
                ViewBag.Plant_ID = PlantId;
                return View();

            }
            catch
            {
                return RedirectToAction("Index", "user");
            }
            //int userID = ((FDSession)this.Session["FDSession"]).userRoleId;
          
        }

        // POST: ParameterMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( MM_Parameter mM_Parameter)
        {

        
           
            if (ModelState.IsValid)
            {

                if (mM_Parameter.IsParameterExists(mM_Parameter.Prameter_Name, Convert.ToInt16(mM_Parameter.Plant_ID),0))
                {
                    ModelState.AddModelError("Prameter_Name", ResourceValidation.Exist);
                 
                }
                else
                {
                    db.MM_Parameter.Add(mM_Parameter);
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceParameterMaster.pageTitle;
                    globalData.messageDetail = ResourceParameterMaster.ParameterMaster_Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
               
            }
            int userID = ((FDSession)this.Session["FDSession"]).userId;
            int Plant = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(s=>s.Plant_ID == Plant), "Plant_ID", "Plant_Name");
            return View(mM_Parameter);
        }

        // GET: ParameterMaster/Edit/5
        public ActionResult Edit(decimal? id)
        {
            try
            {
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                int PlantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MM_Parameter mM_Parameter = db.MM_Parameter.Find(id);
                if (mM_Parameter == null)
                {
                    return HttpNotFound();
                }
            
                globalData.pageTitle = ResourceParameterMaster.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = PlantId;
                return View(mM_Parameter);

            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceParameterMaster.pageTitle;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");

            }

        }

        // POST: ParameterMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_Parameter mM_Parameter)
        {
            try
            {
             
                if (ModelState.IsValid)
                {
                    if (mM_Parameter.IsParameterExists(mM_Parameter.Prameter_Name, Convert.ToInt16(mM_Parameter.Plant_ID), Convert.ToInt16(mM_Parameter.Prameter_ID)))
                    {
                        ModelState.AddModelError("Prameter_Name", ResourceValidation.Exist);

                    }
                    else
                    {

                  
                    db.Entry(mM_Parameter).State = EntityState.Modified;
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceParameterMaster.pageTitle;
                    globalData.messageDetail = ResourceParameterMaster.ParameterMaster_Edit_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
                }
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.ToList(), "Plant_ID", "Plant_Name",mM_Parameter.Plant_ID);
                return View(mM_Parameter);
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceParameterMaster.pageTitle;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }

        }

        // GET: ParameterMaster/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Parameter mM_Parameter = db.MM_Parameter.Find(id);
            if (mM_Parameter == null)
            {
                return HttpNotFound();
            }
            return View(mM_Parameter);
        }

        // POST: ParameterMaster/Delete/5
      
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                MM_Parameter mM_Parameter = db.MM_Parameter.Find(id);
                db.MM_Parameter.Remove(mM_Parameter);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceParameterMaster.pageTitle;
                globalData.messageDetail = ResourceParameterMaster.ParameterMaster_Delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceParameterMaster.pageTitle;
                globalData.messageDetail = ResourceParameterMaster.ParameterMaster_Error_Log;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
          
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
