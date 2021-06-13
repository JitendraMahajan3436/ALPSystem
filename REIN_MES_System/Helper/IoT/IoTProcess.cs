using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace REIN_MES_System.Helper.IoT
{
    public class IoTProcess
    {
        General generalObj = new General();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public String getPostReadTagString(String []tagList)
        {
            try
            {
                String str = "[";
                for(int i=0;i<tagList.Count();i++)
                {
                    if(i>0)
                    {
                        str += ",";
                    }

                    str += "\"" + tagList[i] + "\"";
                }
                str+="]";
                return str;
            }
            catch(Exception ex)
            {
                generalObj.addMetaException(ex, "IoTProcess", "getPostReadTagString", 1);
                return null;
            }
        }

        public ReadResults[] readPLCTag(String[] tagArray)
        {
            try
            {
                var baseAddress = WebConfigurationManager.AppSettings["IoTReadURL"];
                
                var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";
                           
                
                String parsedContent = this.getPostReadTagString(tagArray);
                
                //string parsedContent = "[\"Channel1.Device1.tag1\",\"Channel1.Device1.tag2\"]";
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(parsedContent);
                //http.UseDefaultCredentials = true;
                //http.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                //HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                var response1 = http.GetResponse();

                var stream = response1.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();

                ReadIoTResults d = (ReadIoTResults)new JavaScriptSerializer().Deserialize(content, typeof(ReadIoTResults));

                  return d.readResults;
            }
            catch(Exception ex)
            {
                generalObj.addMetaException(ex, "IoTProcess", "readPLCTag", 1);
                return null;
            }
        }


        public WriteResults[] writePLCTag(WriteIoT[] writeIoT, int shopId)
        {
            try
            {
                RS_IoT_Url ioTUrl = db.RS_IoT_Url.Where(p => p.Shop_ID == shopId && p.Is_Write == true).FirstOrDefault();
                if (ioTUrl == null)
                {
                    return null;
                }
                //var baseAddress = ConfigurationManager.AppSettings.Get("IoTWriteURL").ToString();
                var baseAddress = ioTUrl.URL;

                var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";

                String parsedContent = new JavaScriptSerializer().Serialize(writeIoT);

                //string parsedContent = "[{ \"id\": \"Channel1.Device1.tag1\", \"v\": 42},{ \"id\": \"Channel1.Device1.tag2\", \"v\": 42}]";
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(parsedContent);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response1 = http.GetResponse();

                var stream = response1.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();

                WriteIoTResults d = (WriteIoTResults)new JavaScriptSerializer().Deserialize(content, typeof(WriteIoTResults));

                return d.writeResults;
            }
            catch(Exception ex)
            {
                generalObj.addMetaException(ex, "IoTProcess", "writePLCTag", 1);
                return null;
            }
        }
    }
}