using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace WarehouseFSWTester
{
    static class WareHouseReceiptFiles
    {
        public static List<string> ProcessFiles(string filePath)
        {
            int newRows = 0;
            int updatedRow = 0;
            List<string> result = new List<string>();

            #region Process File

            string file = filePath;
            string fileLine = string.Empty;

            CheckURL();


            if (File.Exists(file))
            {
                Application excel = new Application();
                Workbook wb = excel.Workbooks.Open(file);
                Worksheet excelSheet = wb.ActiveSheet;
                Range xlRange = excelSheet.UsedRange;

                List<WareHouseReceipt> list = new List<WareHouseReceipt>();
                List<Pagination> listP = new List<Pagination>();
                if (excelSheet.Cells[1, 1].Value.ToString() == "DATE")
                {
                    for (int i = 2; i < xlRange.Columns[1].Rows.Count +1; i++)
                    {

                        WareHouseReceipt vhs = new WareHouseReceipt();

                        if (excelSheet.Cells[i, 1].Value2 != null)
                        {
                            if (DateTime.TryParse(excelSheet.Cells[i, 1].Value.ToString(), out DateTime _DateFile))
                                vhs.DateFile = _DateFile;

                            vhs.Activity = excelSheet.Cells[i, 2].Value.ToString();

                            vhs.ItemNumber = excelSheet.Cells[i, 3].Value.ToString();
                            var isNumeric = int.TryParse(vhs.ItemNumber, out int n);
                            if (isNumeric)
                            {
                                if (vhs.ItemNumber.Length < 9)
                                {
                                    vhs.ItemNumber = vhs.ItemNumber.PadLeft(9, '0');
                                }
                            }

                            if (int.TryParse(excelSheet.Cells[i, 4].Value.ToString(), out int _Quantity))
                                vhs.Quantity = _Quantity;

                            vhs.SDNNumber = excelSheet.Cells[i, 6].Value.ToString();
                            vhs.PONumber = excelSheet.Cells[i, 7].Value.ToString();
                            vhs.WH = excelSheet.Cells[i, 8].Value.ToString();
                            vhs.Picker = excelSheet.Cells[i, 9].Value.ToString();
                            list.Add(vhs);

                            if (excelSheet.Cells[i, 15].Value2 != null)
                            {
                                string var = excelSheet.Cells[i, 15].Value2;
                                if (!var.Contains("Page"))
                                {
                                    Pagination p = new Pagination
                                    {
                                        PageNumber = excelSheet.Cells[i, 15].Value.ToString(),
                                    };
                                    if (p.PageNumber.Trim() != string.Empty)
                                    { 
                                        if(int.TryParse(excelSheet.Cells[i, 16].Value.ToString(), out int _count))
                                            p.Count = _count;
                                    
                                        listP.Add(p);
                                    }
                                }
                            }
                        }
                    }
                }

                
                

                object misValue = System.Reflection.Missing.Value;
                wb.Close(false, misValue, misValue);
                excel.Quit();

                //Add PageNumber
                int l = 0;
                foreach (Pagination p in listP)
                {
                    for (int i = 0; i < p.Count; i++)
                    {
                        list[l].PageNumber = p.PageNumber;
                        l++;
                    }

                }




                using (SDCManagementSystem_Test3Entities db = new SDCManagementSystem_Test3Entities())
                {
                    foreach (WareHouseReceipt dealer in list)
                    {
                        if (dealer.Activity != string.Empty)
                        {
                            WareHouseReceipt vhs = db.WareHouseReceipts.Where(
                                x => x.DateFile == dealer.DateFile.Date && x.PONumber == dealer.PONumber && x.ItemNumber == dealer.ItemNumber && x.SDNNumber == dealer.SDNNumber && x.Quantity == dealer.Quantity).FirstOrDefault();

                            if (vhs != null)
                            {   //modify record    
                                vhs.Activity = dealer.Activity;
                                vhs.Quantity = dealer.Quantity;
                                vhs.TotalPrice = dealer.TotalPrice;
                                vhs.WH = dealer.WH;
                                vhs.Picker = dealer.Picker;
                                vhs.PageNumber = dealer.PageNumber;
                                updatedRow++;
                            }
                            else
                            {
                                db.WareHouseReceipts.Add(dealer);
                                newRows++;
                            }
                        }
                        else
                        {
                            db.WareHouseReceipts.Add(dealer);
                            newRows++;
                        }
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }//End of Entity
                result.Add("The " + filePath + " file was uploaded: New Row added: " + newRows.ToString() + " Row updated: " + updatedRow.ToString() + Environment.NewLine);
            }// End If File>exits
            #endregion
            return result;
        }

        public static void PriceUpdate()
        {
            using (SDCManagementSystem_Test3Entities db = new SDCManagementSystem_Test3Entities())
            {
                try
                {
                   
                    db.spu_warehousePriceUpdate2();
                    db.SaveChanges();
                }
                catch (Exception )
                {
                    throw ;
                }
            }
        }

        public static void MoveFile(string sourcePath, string fileName)
        {
            try
            {
                string targetPath = ConfigurationManager.AppSettings["targetPath"]; 
                System.IO.File.Copy(sourcePath, targetPath + fileName,true);
                System.IO.File.Delete(sourcePath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void CheckURL()
        {
            try
            {
                string URL = "http://sdc-sql2012b/reportserver?/RP/RPWHprices&rs:Command=Render&rs:Format=PDF";

                //We can get values of these parameters from Request object.

                string paramReportYear = "&year=" + DateTime.Now.Year;
                string paramReportMonth = "&month=" + DateTime.Now.Month;
                string paramReportDay = "&day=" + DateTime.Now.Day;

                URL = URL + paramReportYear + paramReportMonth + paramReportDay;

                System.Net.HttpWebRequest Req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
                Req.Credentials = System.Net.CredentialCache.DefaultCredentials;
                Req.Method = "GET";
            }
            catch (Exception)
            {

              
            }
            
        }
        public static void SafePDF(string y, string m, string d)
        {
          //string URL = "http://sdc-sql2012b/reportserver?/RP/RPWHprices&rs:Command=Render&rs:Format=PDF";

            string URL = "http://sdc-qa/reportserver?/Prod/RP/RPWHprices&rs:Command=Render&rs:Format=PDF";
            //We can get values of these parameters from Request object.

            string paramReportYear = "&year=" + y;
            string paramReportMonth = "&month=" + m;
            string paramReportDay = "&day=" + d;

            URL = URL + paramReportYear + paramReportMonth + paramReportDay;

            System.Net.HttpWebRequest Req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            Req.Credentials = System.Net.CredentialCache.DefaultCredentials;
            Req.Method = "GET";
            //Specify the path for saving.
            string path = ConfigurationManager.AppSettings["PDFPath"] + m + "-" + d + " " + @"RP - Warehouse Reconciliation Report with prices.pdf";

            if (File.Exists(path))
                File.Delete(path);
            try
            {
                WebResponse objResponse = Req.GetResponse();
                FileStream fs = new FileStream(path, System.IO.FileMode.Create);
                Stream stream = objResponse.GetResponseStream();
                byte[] buf = new byte[1024];
                int len = stream.Read(buf, 0, 1024);
                while (len > 0)
                {
                    fs.Write(buf, 0, len);
                    len = stream.Read(buf, 0, 1024);
                }
                stream.Close();
                fs.Close();

                SendEmail(path);
            }
            catch (System.Net.WebException ex)
            {
                if(ex.Message.StartsWith("The operation has timed out"))
                    SafePDF(y, m, d);
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void SendEmail(string filePDF)
        {
            MailMessage message = new MailMessage(ConfigurationManager.AppSettings["EmailFrom"], ConfigurationManager.AppSettings["EmailTo"])
            {
                Subject = "Reconciliation Process: Warehouse Reconciliation Report with prices",
                Body = ""
            };

            Attachment data = new Attachment(filePDF, MediaTypeNames.Application.Octet);
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(filePDF);
            disposition.ModificationDate = File.GetLastWriteTime(filePDF);
            disposition.ReadDate = File.GetLastAccessTime(filePDF);
            message.Attachments.Add(data);

            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["EmailServer"])
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = ConfigurationManager.AppSettings["emailPath"],
                Credentials = CredentialCache.DefaultNetworkCredentials
            };
        client.Send(message);
        }
    }

    internal class Pagination
    {
        public int Count { get; set; }
        public string PageNumber { get; set; }
    }
}