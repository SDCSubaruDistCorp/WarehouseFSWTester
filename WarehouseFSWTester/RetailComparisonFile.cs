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
    static class RetailComparisonFile
    {
        public static List<string> ProcessFiles(string filePath)
        {
            int newRows = 0;
            int updatedRow = 0;
            List<string> result = new List<string>();
            List<RetailComparison> list = new List<RetailComparison>();

            #region Process File

            string file = filePath;
            string fileLine = string.Empty;

            if (File.Exists(file))
            {
                Application excel = new Application();
                Workbook wb = excel.Workbooks.Open(file);
                int _sheets = excel.ActiveWorkbook.Sheets.Count;

                //for (int s = 1; s < _sheets + 1; s++)
                for (int s = 1; s < 2 ; s++)
                {
                    int _dealerid = 0;
                    int _Year = DateTime.Now.Year;
                    int _Month = DateTime.Now.Month;

                    Worksheet excelSheet = wb.Sheets[1] ;
                    Range xlRange = excelSheet.UsedRange;

                    for (int i = 1; i < xlRange.Columns[1].Rows.Count; i++)
                    {
                        if (excelSheet.Cells[i, 1].Value2 != null)
                        {

                            if (excelSheet.Cells[i, 1].Value.ToString() != string.Empty)
                            {
                                string val = "0" + excelSheet.Cells[i, 1].Value.ToString();

                                SDCManagementSystem_Test3Entities db = new SDCManagementSystem_Test3Entities();
                                DWT_Dealers d = db.DWT_Dealers.Where(x => x.Code == val).FirstOrDefault();
                                if (d != null)
                                { 
                                    _dealerid = d.DealerId;

                                    RetailComparison vhs = new RetailComparison();

                                    vhs.Dealer_id = _dealerid;
                                    vhs.Year = _Year;
                                    vhs.Month = _Month -1 ;


                                    if (excelSheet.Cells[i, 3].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 3].Value.ToString(), out int _district))
                                            vhs.district = _district;
                                    }

                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 4].Value.ToString(), out int _Whlse_currentMY))
                                            vhs.Whlse_currentMY = _Whlse_currentMY;
                                    }

                                    if (excelSheet.Cells[i, 5].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 5].Value.ToString(), out int _Whlse_passMY))
                                            vhs.Whlse_passMY = _Whlse_passMY;
                                    }

                                    if (excelSheet.Cells[i, 6].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 6].Value.ToString(), out int _Whlse_ChangeYOY))
                                            vhs.Whlse_ChangeYOY = _Whlse_ChangeYOY;
                                    }

                                    if (excelSheet.Cells[i, 7].Value2 != null)
                                    {
                                        if (decimal.TryParse(excelSheet.Cells[i, 7].Value.ToString(), out decimal _Pct_WhlseYOY))
                                            vhs.Pct_WhlseYOY = _Pct_WhlseYOY * 100 ;
                                    }

                                    if (excelSheet.Cells[i, 8].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 8].Value.ToString(), out int _Whlse_CurrenMonth))
                                            vhs.Whlse_CurrenMonth = _Whlse_CurrenMonth;
                                    }

                                    if (excelSheet.Cells[i, 9].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 9].Value.ToString(), out int _RtlSls_currentMY))
                                            vhs.RtlSls_currentMY = _RtlSls_currentMY;
                                    }

                                    if (excelSheet.Cells[i, 10].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 10].Value.ToString(), out int _RtlSls_passMY))
                                            vhs.RtlSls_passMY = _RtlSls_passMY;
                                    }

                                    if (excelSheet.Cells[i, 11].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 11].Value.ToString(), out int _RtlSls_ChangeYOY))
                                            vhs.RtlSls_ChangeYOY = _RtlSls_ChangeYOY ;
                                    }

                                    if (excelSheet.Cells[i, 12].Value2 != null)
                                    {
                                        if (decimal.TryParse(excelSheet.Cells[i, 12].Value.ToString(), out decimal _Pct_RtlSlsYOY))
                                            vhs.Pct_RtlSlsYOY = _Pct_RtlSlsYOY * 100;
                                    }

                                    if (excelSheet.Cells[i, 13].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 13].Value.ToString(), out int _RtlSls_CurrenMonth))
                                            vhs.RtlSls_CurrenMonth = _RtlSls_CurrenMonth;
                                    }

                                    list.Add(vhs);
                                }
                            }
                        }
                    }
                }

                object misValue = System.Reflection.Missing.Value;
                wb.Close(false, misValue, misValue);
                excel.Quit();

                using (SDCManagementSystem_Test3Entities db = new SDCManagementSystem_Test3Entities())
                {
                    foreach (RetailComparison dealer in list)
                    {
                        if (dealer.Dealer_id != 0)
                        {
                            RetailComparison vhs = db.RetailComparisons.Where(
                                x => x.Dealer_id == dealer.Dealer_id && x.Year == dealer.Year && x.Month == dealer.Month).FirstOrDefault();

                            if (vhs != null)
                            {   //modify record    
                                vhs.Whlse_currentMY = dealer.Whlse_currentMY;
                                vhs.Whlse_passMY = dealer.Whlse_passMY;
                                vhs.Whlse_ChangeYOY = dealer.Whlse_ChangeYOY;
                                vhs.Pct_WhlseYOY = dealer.Pct_WhlseYOY;
                                vhs.Whlse_CurrenMonth = dealer.Whlse_CurrenMonth;
                                vhs.RtlSls_currentMY = dealer.RtlSls_currentMY;
                                vhs.RtlSls_passMY = dealer.RtlSls_passMY;
                                vhs.RtlSls_ChangeYOY = dealer.RtlSls_ChangeYOY;
                                vhs.Pct_RtlSlsYOY = dealer.Pct_RtlSlsYOY;
                                vhs.RtlSls_CurrenMonth = dealer.RtlSls_CurrenMonth;
                                updatedRow++;
                            }
                            else
                            {
                                db.RetailComparisons.Add(dealer);
                                newRows++;
                            }
                        }
                        else
                        {
                            db.RetailComparisons.Add(dealer);
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

}