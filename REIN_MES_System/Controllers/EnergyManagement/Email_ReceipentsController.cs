using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using ZHB_AD.Models;
using System.Data;
using System.Data.Entity;
using ZHB_AD.Helper;
using ZHB_AD.App_LocalResources;



namespace ZHB_AD.Controllers.ZHB_AD
{

    public class Email_ReceipentsController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession fdSession = new FDSession();
        General genral = new General();


        // GET: Email_Receipents
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
                globalData.pageTitle = ResourceEmail_Receipents.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                var mukesh = (from er in db.MM_EMAIL_RECEIPENTS
                              select (er)).ToList();
                return View(mukesh);
                //return View(db.MM_EMAIL_RECEIPENTS.ToList());



            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");
            }

        }
        // GET: Email_Receipements/Create
        public ActionResult Create()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.plantID = plantID;
                globalData.pageTitle = ResourceEmail_Receipents.pageTitle;
                return View();

            }
            catch
            {
                return RedirectToAction("Index");
            }
            
        }
        //POST:Email_Receipents/Create
        [HttpPost]
        public ActionResult Create(MM_EMAIL_RECEIPENTS mM_EMAIL_RECEIPENTS)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                mM_EMAIL_RECEIPENTS.PlantID = plantID;

                if (ModelState.IsValid)
                {
                    if (mM_EMAIL_RECEIPENTS.IsEmailExistsEdit(mM_EMAIL_RECEIPENTS.EmailID, plantID, 0))
                    {
                        ModelState.AddModelError("EmailID", ResourceValidation.Exist);
                    }
                    else
                    {
                        db.MM_EMAIL_RECEIPENTS.Add(mM_EMAIL_RECEIPENTS);
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceEmail_Receipents.pageTitle;
                        globalData.messageDetail = ResourceEmail_Receipents.Email_Receipents_Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }

                    
                }
               
                    return View();
             
                         

                
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Email_Receipents/Edit/5
        public ActionResult Edit(decimal id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                MM_EMAIL_RECEIPENTS mM_EMAIL_RECEIPENTS = db.MM_EMAIL_RECEIPENTS.Find(id);
                if (mM_EMAIL_RECEIPENTS == null)
                {
                    return HttpNotFound();
                }
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                int PlantId = ((FDSession)this.Session["FDSession"]).plantId;
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name", mM_PowerTarget.Plant_ID);
                  globalData.pageTitle = ResourceEmail_Receipents.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                return View(mM_EMAIL_RECEIPENTS);
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_EMAIL_RECEIPENTS mM_EMAIL_RECEIPENTS)
        {
            int PlantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                if (mM_EMAIL_RECEIPENTS.IsEmailExistsEdit(mM_EMAIL_RECEIPENTS.EmailID, PlantId, mM_EMAIL_RECEIPENTS.ID))
                {
                    ModelState.AddModelError("EmailID", ResourceValidation.Exist);
                }
                else
                {


                    db.Entry(mM_EMAIL_RECEIPENTS).State = EntityState.Modified;
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceEmail_Receipents.pageTitle;
                    globalData.messageDetail = ResourceEmail_Receipents.Email_Receipents__Edit_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }
            return View(mM_EMAIL_RECEIPENTS);
        }

        // GET: Email_Receipents/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_EMAIL_RECEIPENTS mM_EMAIL_RECEIPENTS = db.MM_EMAIL_RECEIPENTS.Find(id);
            if (mM_EMAIL_RECEIPENTS == null)
            {
                return HttpNotFound();
            }
            return View(mM_EMAIL_RECEIPENTS);
        }

        // POST: Email_Receipents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                MM_EMAIL_RECEIPENTS mM_EMAIL_RECEIPENTS = db.MM_EMAIL_RECEIPENTS.Find(id);
                db.MM_EMAIL_RECEIPENTS.Remove(mM_EMAIL_RECEIPENTS);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceEmail_Receipents.pageTitle;
                globalData.messageDetail = ResourceEmail_Receipents.Email_Receipents__Delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                genral.addControllerException(ex, "Email_Receipents", "delete", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceEmail_Receipents.pageTitle;
                globalData.messageDetail = ResourceEmail_Receipents.Email_Receipents__Error_Log;
                TempData["globalData"] = globalData;
                //ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Category.Plant_ID);
                return RedirectToAction("Index");
            }

        }



    }
}
