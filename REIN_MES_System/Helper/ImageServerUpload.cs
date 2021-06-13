using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using REIN_MES_System.Models;
using System.IO;

namespace REIN_MES_System.Helper
{
    public class ImageServerUpload
    {
        public int Width { get; set; }

        public int Height { get; set; }

        // folder for the upload, you can put this in the web.config
        private readonly string UploadPath = "~/Content/images/SOP_IMAGE_TEMP/";

        public ImageResult RenameUploadFile(HttpPostedFileBase file, string finalFileName, string ControllerName)
        {
            //var fileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(file.FileName);

            finalFileName = (finalFileName.ToString()) + "_IMAGE" + extension;

            //if (System.IO.File.Exists
            //    (HttpContext.Current.Request.MapPath(UploadPath + finalFileName)))
            //{
            //    //delete the old image if exist
            //    System.IO.File.Delete(HttpContext.Current.Request.MapPath(UploadPath + finalFileName));
            //}
            //file doesn't exist, upload item but validate first
            return UploadFile(file, finalFileName, ControllerName);
        }

        private ImageResult UploadFile(HttpPostedFileBase file, string fileName, string ControllerName)
        {
            ImageResult imageResult = new ImageResult { Success = true, ErrorMessage = null };

            var path = Path.Combine(HttpContext.Current.Request.MapPath(UploadPath), fileName);
            string extension = Path.GetExtension(file.FileName);

            //make sure the file is valid
            if (!ValidateExtension(extension))
            {
                imageResult.Success = false;
                imageResult.ErrorMessage = "Invalid Extension";
                return imageResult;
            }

            try
            {
                file.SaveAs(path);

                Image imgOriginal = Image.FromFile(path);

                // pass in whatever value you want
                //note- Jitendra Mahajan some cases scale method return invalid data that time it throw exception e.g png file more than 15 mb

                //  Image imgActual = Scale(imgOriginal);
                imgOriginal.Dispose();
                // imgActual.Save(path);
                // imgActual.Dispose();
                imageResult.ImageName = fileName;
                byte[] imageArray = File.ReadAllBytes(path);




                //  int fileSizeInBytes = file.ContentLength;
                //===============

                MemoryStream target = new MemoryStream();
                //  imgOriginal.Save(imageArray, imgOriginal.RawFormat);
                // byte[] imageArray = target.ToArray();
                // string bytes = Convert.ToBase64String(imageArray);

                //SR1.ImageServerServiceClient obj = new SR1.ImageServerServiceClient();
                //bool serviceResponse = obj.GetImage(imageArray, fileName, ControllerName, extension);
                //if (serviceResponse == true)
                //{
                //    //delete file from local folder after work is finished

                //        if (File.Exists(path))
                //        {
                //            System.IO.File.Delete(path);
                //        }

                //    //if (System.IO.File.Exists(path))
                //    //{
                //    //    //delete the old image if exist
                //    //    System.IO.File.Delete(path);
                //    //}


                //}


                return imageResult;
            }
            catch (Exception ex)
            {
                // you might NOT want to show the exception error for the user
                // this is generally logging or testing
                if (File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                imageResult.Success = false;
                imageResult.ErrorMessage = ex.Message;
                return imageResult;
            }
        }

        private bool ValidateExtension(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return true;
                case ".png":
                    return true;
                case ".jpeg":
                    return true;
                case ".JPEG":
                    return true;
                case ".PNG":
                    return true;
                case ".JPG":
                    return true;
                default:
                    return false;
            }
        }

        private Image Scale(Image imgPhoto)
        {
            float sourceWidth = imgPhoto.Width;
            float sourceHeight = imgPhoto.Height;
            float destHeight = 0;
            float destWidth = 0;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            // force resize, might distort image
            if (Width != 0 && Height != 0)
            {
                destWidth = Width;
                destHeight = Height;
            }
            // change size proportially depending on width or height
            else if (Height != 0)
            {
                destWidth = (float)(Height * sourceWidth) / sourceHeight;
                destHeight = Height;
            }
            else
            {
                destWidth = Width;
                destHeight = (float)(sourceHeight * Width / sourceWidth);
            }

            //Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight,
            //                            PixelFormat.Format32bppPArgb);
            Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight,
                                         PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.CompositingQuality = CompositingQuality.HighSpeed;


            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;
        }

        /// <summary>
        /// code written by Jitendra Mahajan
        /// date 26-05-2017
        /// used to return image array as a string so code reundancy reduced
        /// </summary>
        /// <param name="imageName">pass image name to method  </param>
        /// <param name="ControllerName">controller name for identify which controller calling function</param>
        /// <returns> string in base64 format</returns>
        public string getimageFromService(string imageName, string ControllerName)
        {
            //code for get image from server in byte format
            //SR1.ImageServerServiceClient obj = new SR1.ImageServerServiceClient();
            //byte[] imageUrl = obj.SetImage(imageName, ControllerName);
            //var base64 = Convert.ToBase64String(imageUrl);
            //var imgSrc = String.Format("data:image/jpg;base64,{0}", base64);
            return imageName;
        }
    }
}