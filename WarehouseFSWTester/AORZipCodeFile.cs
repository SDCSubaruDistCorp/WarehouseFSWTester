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
    static class AORZipCodeFile
    {
        public static List<string> ProcessFiles(string filePath)
        {
            int newRows = 0;
            int updatedRow = 0;
            List<string> result = new List<string>();

            List<AORzipCode> listAOR = new List<AORzipCode>();

            #region Process File

            string file = filePath;
            string fileLine = string.Empty;

            if (File.Exists(file))
            {
                Application excel = new Application();
                Workbook wb = excel.Workbooks.Open(file);

                Worksheet excelSheet = wb.ActiveSheet ;
                Range xlRange = excelSheet.UsedRange;

                AORzipCode vhsAOR = null;

                for (int i = 1; i < xlRange.Columns[1].Rows.Count; i++)
                {
                    if (excelSheet.Cells[i, 1].Value2 != null)
                    {
                        string _description = string.Empty;
                        string _code = string.Empty;
                        int _aorcode = 0;


                        if (excelSheet.Cells[i, 1].Value2 != null)
                        { 
                            _code = excelSheet.Cells[i, 1].Value.ToString();
                            using (SDCManagementSystem_Test3Entities db = new SDCManagementSystem_Test3Entities())
                            {
                                AOR vhs = db.AORs.Where(
                                x => x.AORName.Contains(_code)).FirstOrDefault();
                                if (vhs != null)
                                { 
                                    _aorcode = vhs.AORId;
                            
                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                        _description = excelSheet.Cells[i, 4].Value.ToString(); 

                                    if (excelSheet.Cells[i, 3].Value2 != null)
                                    {
                                        string val = excelSheet.Cells[i, 3].Value.ToString();
                                        string[] zips = val.Split(' ');
                                        foreach (string zip in zips)
                                        {
                                            vhsAOR = new AORzipCode();
                                            vhsAOR.AORId = _aorcode;
                                            vhsAOR.ZIPCode = zip;
                                            vhsAOR.Description = _description;

                                            listAOR.Add(vhsAOR);

                                        }
                                    }
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
                    foreach (AORzipCode dealer in listAOR)
                    {
                        if (dealer.AORId != 0)
                        {
                            AORzipCode vhs = db.AORzipCodes.Where(
                                x => x.AORId == dealer.AORId &&  x.ZIPCode == dealer.ZIPCode).FirstOrDefault();

                            if (vhs != null)
                            {   //modify record    
                                vhs.ZIPCode = dealer.ZIPCode;
                                vhs.Description = dealer.Description;
                                updatedRow++;
                            }
                            else
                            {
                                db.AORzipCodes.Add(dealer);
                                newRows++;
                            }
                        }
                        else
                        {
                            db.AORzipCodes.Add(dealer);
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