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
    static class CrossSalesZipFile
    {
        public static List<string> ProcessFiles(string filePath)
        {
            int newRows = 0;
            int updatedRow = 0;
            List<string> result = new List<string>();
            List<CrossSellZip> list = new List<CrossSellZip>();

            #region Process File

            string file = filePath;
            string fileLine = string.Empty;

            if (File.Exists(file))
            {
                Application excel = new Application();
                Workbook wb = excel.Workbooks.Open(file);
                int _sheets = excel.ActiveWorkbook.Sheets.Count;

                for (int s = 1; s < _sheets + 1; s++)
                {
                    int _dealerid = 0;
                    DateTime _SalesFrom = DateTime.MinValue;
                    DateTime _SalesThrough = DateTime.MinValue;


                    Worksheet excelSheet = wb.Sheets[s] ;
                    Range xlRange = excelSheet.UsedRange;

                    for (int i = 1; i < xlRange.Columns[1].Rows.Count; i++)
                    {
                        if (excelSheet.Cells[i, 1].Value2 != null)
                        {

                            if (excelSheet.Cells[i, 1].Value.ToString() != string.Empty)
                            {
                                string val = excelSheet.Cells[i, 1].Value.ToString();

                                if (val.Contains ("Dealer"))
                                {
                                    SDCManagementSystem_Test3Entities db = new SDCManagementSystem_Test3Entities();
                                    DWT_Dealers d = db.DWT_Dealers.Where(x => x.Code == val.Substring(val.IndexOf(" ")+1, 6)).FirstOrDefault();
                                    if (d != null)
                                        _dealerid = d.DealerId;
                                }

                                if(val.StartsWith("For"))
                                {
                                    _SalesFrom = DateTime.Parse(val.Substring(15, 10));
                                    _SalesThrough = DateTime.Parse(val.Substring(34, 10));
                                }

                                if (int.TryParse(val, out int saleszip))
                                {


                                    CrossSellZip vhs = new CrossSellZip();

                                    vhs.dealer_id = _dealerid;
                                    vhs.SalesFrom = _SalesFrom;
                                    vhs.SalesThrough = _SalesThrough;


                                    if (excelSheet.Cells[i, 1].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 1].Value.ToString(), out int _SalesinZIP))
                                            vhs.SalesinZIP = _SalesinZIP;
                                    }

                                    if (excelSheet.Cells[i, 2].Value2 != null)
                                    {
                                        if (decimal.TryParse(excelSheet.Cells[i, 2].Value.ToString(), out decimal _pctTotalSales))
                                            vhs.pctTotalSales = _pctTotalSales;
                                    }

                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                        vhs.ZIP = excelSheet.Cells[i, 4].Value.ToString();

                                    if (excelSheet.Cells[i, 6].Value2 != null)
                                        vhs.ZIPName = excelSheet.Cells[i, 6].Value.ToString();

                                    if (excelSheet.Cells[i, 9].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 9].Value.ToString(), out int _CurrentPopulation))
                                            vhs.CurrentPopulation = _CurrentPopulation;
                                    }

                                    if (excelSheet.Cells[i, 10].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 10].Value.ToString(), out int _C5YrsProyectedPopulation))
                                            vhs.C5YrsProyectedPopulation = _C5YrsProyectedPopulation;
                                    }

                                    if (excelSheet.Cells[i, 12].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 12].Value.ToString(), out int _CurrentHouseholds))
                                            vhs.CurrentHouseholds = _CurrentHouseholds;
                                    }

                                    if (excelSheet.Cells[i, 13].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 13].Value.ToString(), out int _C5YrsProyectedHouseholds))
                                            vhs.C5YrsProyectedHouseholds = _C5YrsProyectedHouseholds;
                                    }

                                    if (excelSheet.Cells[i, 15].Value2 != null)
                                    {
                                        if (decimal.TryParse(excelSheet.Cells[i, 15].Value.ToString(), out decimal _CurrentMediaHouseholdsIncome))
                                            vhs.CurrentMediaHouseholdsIncome = _CurrentMediaHouseholdsIncome;
                                    }

                                    if (excelSheet.Cells[i, 15].Value2 != null)
                                    {
                                        if (decimal.TryParse(excelSheet.Cells[i, 16].Value.ToString(), out decimal _C5YrsMediaHouseholdsIncome))
                                            vhs.C5YrsMediaHouseholdsIncome = _C5YrsMediaHouseholdsIncome;
                                    }

                                    if (excelSheet.Cells[i, 18].Value2 != null)
                                    {
                                        if (decimal.TryParse(excelSheet.Cells[i, 18].Value.ToString(), out decimal _CurrentAVGHouseholdsIncome))
                                            vhs.CurrentAVGHouseholdsIncome = _CurrentAVGHouseholdsIncome;
                                    }

                                    if (excelSheet.Cells[i, 20].Value2 != null)
                                    {
                                        if (decimal.TryParse(excelSheet.Cells[i, 20].Value.ToString(), out decimal _C5YrsAVGHouseholdsIncome))
                                            vhs.C5YrsAVGHouseholdsIncome = _C5YrsAVGHouseholdsIncome;
                                    }

                                    if (excelSheet.Cells[i, 22].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 22].Value.ToString(), out int _CurrentHouseholdsUnder50))
                                            vhs.CurrentHouseholdsUnder50 = _CurrentHouseholdsUnder50;
                                    }

                                    if (excelSheet.Cells[i, 25].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 25].Value.ToString(), out int _C5YrsProyectedHouseholdsUnder50))
                                            vhs.C5YrsProyectedHouseholdsUnder50 = _C5YrsProyectedHouseholdsUnder50;
                                    }

                                    if (excelSheet.Cells[i, 27].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 27].Value.ToString(), out int _CurrentHouseholds50to100))
                                            vhs.CurrentHouseholds50to100 = _CurrentHouseholds50to100;
                                    }

                                    if (excelSheet.Cells[i, 29].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 29].Value.ToString(), out int _C5YrsProyectedHouseholds50to100))
                                            vhs.C5YrsProyectedHouseholds50to100 = _C5YrsProyectedHouseholds50to100;
                                    }

                                    if (excelSheet.Cells[i, 31].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 31].Value.ToString(), out int _CurrentHouseholdsOver100))
                                            vhs.CurrentHouseholdsOver100 = _CurrentHouseholdsOver100;
                                    }

                                    if (excelSheet.Cells[i, 33].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 33].Value.ToString(), out int _C5YrsProyectedHouseholdsOver100))
                                            vhs.C5YrsProyectedHouseholdsOver100 = _C5YrsProyectedHouseholdsOver100;
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
                    foreach (CrossSellZip dealer in list)
                    {
                        if (dealer.dealer_id != 0)
                        {
                            CrossSellZip vhs = db.CrossSellZips.Where(
                                x => x.dealer_id == dealer.dealer_id && x.SalesFrom == dealer.SalesFrom && x.SalesThrough == dealer.SalesThrough).FirstOrDefault();

                            if (vhs != null)
                            {   //modify record    
                                vhs.SalesinZIP = dealer.SalesinZIP;
                                vhs.pctTotalSales = dealer.pctTotalSales;
                                vhs.ZIP = dealer.ZIP;
                                vhs.ZIPName = dealer.ZIPName;
                                vhs.CurrentPopulation = dealer.CurrentPopulation;
                                vhs.C5YrsProyectedPopulation = dealer.C5YrsProyectedPopulation;
                                vhs.CurrentHouseholds = dealer.CurrentHouseholds;
                                vhs.C5YrsProyectedHouseholds = dealer.C5YrsProyectedHouseholds;
                                vhs.CurrentMediaHouseholdsIncome = dealer.CurrentMediaHouseholdsIncome;
                                vhs.C5YrsMediaHouseholdsIncome = dealer.C5YrsMediaHouseholdsIncome;
                                vhs.CurrentAVGHouseholdsIncome = dealer.CurrentAVGHouseholdsIncome;
                                vhs.C5YrsAVGHouseholdsIncome = dealer.C5YrsAVGHouseholdsIncome;
                                vhs.CurrentHouseholdsUnder50 = dealer.CurrentHouseholdsUnder50;
                                vhs.C5YrsProyectedHouseholdsUnder50 = dealer.C5YrsProyectedHouseholdsUnder50;
                                vhs.CurrentHouseholds50to100 = dealer.CurrentHouseholds50to100;
                                vhs.C5YrsProyectedHouseholds50to100 = dealer.C5YrsProyectedHouseholds50to100;
                                vhs.CurrentHouseholdsOver100 = dealer.CurrentHouseholdsOver100;
                                vhs.C5YrsProyectedHouseholdsOver100 = dealer.C5YrsProyectedHouseholdsOver100;
                                updatedRow++;
                            }
                            else
                            {
                                db.CrossSellZips.Add(dealer);
                                newRows++;
                            }
                        }
                        else
                        {
                            db.CrossSellZips.Add(dealer);
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
                if (System.IO.File.Exists(sourcePath + fileName))
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