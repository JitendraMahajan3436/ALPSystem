
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using REIN_MES_System.App_LocalResources;
using System.IO;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class AddDocumentController : BaseController
    {
        // GET: AddDocument
        RS_Documents cbmpc = new RS_Documents();

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

       

        // GET: AddDocument
        public ActionResult Index()
        {
            //plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = "Add Document";
            ViewBag.GlobalDataModel = globalData;
            string path = Server.MapPath("~/Content/documents/");
            return View(db.RS_Documents.ToList());
        }

        public ActionResult ViewDocument()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = "Document Viewer";
            ViewBag.GlobalDataModel = globalData;
            string path = Server.MapPath("/Documents/");
            return View(db.RS_Documents.ToList());
        }
        public ActionResult getview(int id)
        {
            object model = null;

            {
                using (REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities())
                {
                    var obj = db.RS_Documents.Single(s => s.Doc_ID == id);
                    model = db.RS_Documents.Find(obj.Doc_ID);
                    ViewBag.DocumentName = obj.File_Name;
                }
            }
            return View(model);

        }
        public ActionResult getview2(int id)
        {
            object model = null;

            {
                using (REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities())
                {
                    var obj = db.RS_Documents.Single(s => s.Doc_ID == id);
                    model = db.RS_Documents.Find(obj.Doc_ID);
                    ViewBag.DocumentName = obj.File_Name;
                }
            }
            return View(model);

        }
        public FileResult Download(int? Doc_Id)
        {
            RS_Documents rS_Documents = db.RS_Documents.Find(Doc_Id);
            byte[] fileContent = null;
            string mimetype = ""; string filename = "";
            fileContent = rS_Documents.Doc_Content;
            mimetype = rS_Documents.Doc_Type.ToString();
            filename = rS_Documents.File_Name.ToString();



            return File(fileContent, mimetype, filename);
        }

        
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Documents rS_Documents = db.RS_Documents.Find(id);
            
            return View(rS_Documents);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                //int b = 0;
                //float a = 5 / b;
                RS_Documents rS_Documents = db.RS_Documents.Find(id);

                var filename = rS_Documents.Doc_Name;
                string path = Server.MapPath("~/Content/documents/") + filename;
                System.IO.File.Delete(path);

                db.RS_Documents.Remove(rS_Documents);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = "Document Delete Successfully";
                //globalData.messageDetail = ResourceModules.Plant + " " + ResourceMessages.Add_Success;
                TempData["globalData"] = globalData;
                //Delete  records from MTTUW DB
                //Associate_MGMT = db_1.MM_MTTUW_Plants.Find(id);
                //db_1.MM_MTTUW_Plants.Remove(Associate_MGMT);
                //db_1.SaveChanges();
                //End


                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return RedirectToAction("Delete", id);
            }

        }

        [HttpGet]
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = "Add Documents";
            //globalData.subTitle = ResourceGlobal.Create;
            //globalData.controllerName = "CBMParameterCategory";
            //globalData.actionName = ResourceGlobal.Create;

            ViewBag.GlobalDataModel = globalData;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Doc_Name,Doc_Description,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")]  RS_Documents rS_Documents, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                    if(file != null)
                    {
                        var result = db.RS_Documents.Where(m => m.Doc_Name == rS_Documents.Doc_Name).Select(m=> m.Doc_Name).ToList();
                        if(result.Count > 0)
                        {
                            ModelState.AddModelError("Doc_Name", ResourceValidation.Exist);
                            return View(rS_Documents);
                        }
                        else
                        {
                            string path = Server.MapPath("~/Documents");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            file.SaveAs(path + file.FileName);
                            if (file != null && file.ContentLength > 0)
                            {
                                using (var reader = new System.IO.BinaryReader(file.InputStream))
                                {
                                    
                                    rS_Documents.Doc_Type = Path.GetExtension(file.FileName);
                                    rS_Documents.Content_Type = file.ContentType;
                                    rS_Documents.Doc_Content = reader.ReadBytes(file.ContentLength);
                                }
                            }

                            rS_Documents.File_Name = System.IO.Path.GetFileName(file.FileName);
                        
                            rS_Documents.Doc_Name = rS_Documents.Doc_Name;
                            rS_Documents.Inserted_Date = DateTime.Now;
                            rS_Documents.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            rS_Documents.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.RS_Documents.Add(rS_Documents);
                            db.SaveChanges();

                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = "Document Upload Successfully";
                            TempData["globalData"] = globalData;
                        }
                        
                    }
                    else
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = "Please Select Document";
                        TempData["globalData"] = globalData;
                    }



                    //if (file != null && file.ContentLength > 0)
                    //{
                    //    {
                    //        using (var reader = new System.IO.BinaryReader(file.InputStream))
                    //        {
                    //            rS_Documents.Doc_Name = System.IO.Path.GetFileName(file.FileName);
                    //            rS_Documents.Doc_Description = System.IO.Path.GetFileName(file.FileName);
                    //            rS_Documents.Doc_Type = Path.GetExtension(file.FileName);
                    //            rS_Documents.Content_Type = file.ContentType;
                    //            rS_Documents.Doc_Content = reader.ReadBytes(file.ContentLength);
                    //            //string _FileName = Path.GetFileName(file.FileName);
                    //            //string _path = Path.Combine(Server.MapPath("~/Documents/PDFs"), _FileName);
                    //            //file.SaveAs(_path);

                    //        }
                    //    }
                    //    rS_Documents.Inserted_Date = DateTime.Now;
                    //    rS_Documents.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    //    rS_Documents.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //    db.rS_Documents.Add(rS_Documents);
                    //    db.SaveChanges();
                    //    globalData.isSuccessMessage = true;
                    //    globalData.messageTitle = "Document Upload Successfully";
                    //    //globalData.messageDetail = ResourceModules.Plant + " " + ResourceMessages.Add_Success;
                    //    TempData["globalData"] = globalData;
                    //}
                    //else
                    //{
                    //    globalData.isErrorMessage = true;
                    //    globalData.messageTitle = "Please Select Document";
                    //    TempData["globalData"] = globalData;
                    //}



                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    //globalData.isErrorMessage = true;
                    //globalData.messageTitle = ResourceModules.CBM;
                    //globalData.messageDetail = ex.Message.ToString();
                    //ViewBag.GlobalDataModel = globalData;
                    TempData["globalData"] = globalData;
                }
            }


            //globalData.pageTitle = ResourceModules.CBMPC_Config;
            //globalData.subTitle = ResourceGlobal.Create;
            //globalData.controllerName = "CBMParameterCategory";
            //globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;


            return View(rS_Documents);
        }

    }
}