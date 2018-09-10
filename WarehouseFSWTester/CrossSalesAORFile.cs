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
    static class CrossSalesAORFile
    {
        public static List<string> ProcessFiles(string filePath)
        {
            int newRows = 0;
            int updatedRow = 0;
            List<string> result = new List<string>();

            List<AOR> listAOR = new List<AOR>();
            List<AORDealerHistory> listAORH = new List<AORDealerHistory>();
            List<CrossSellDealerAOR> listAORCrossSale = new List<CrossSellDealerAOR>();
            string valLineBefore = string.Empty;


            #region Process File

            string file = filePath;
            string fileLine = string.Empty;

            if (File.Exists(file))
            {
                Application excel = new Application();
                Workbook wb = excel.Workbooks.Open(file);

                int _dealerid = 0;
                DateTime _SalesFrom = DateTime.MinValue;
                DateTime _SalesThrough = DateTime.MinValue;
                int _ObjTotalSales = 0;


                Worksheet excelSheet = wb.ActiveSheet ;
                Range xlRange = excelSheet.UsedRange;

                AOR vhsAOR = null;
                CrossSellDealerAOR vhsCrossSellDealerAOR = null;
                AORDealerHistory vhsAORDealerHistory = new AORDealerHistory();

                for (int i = 1; i < xlRange.Columns[1].Rows.Count; i++)
                {
                    if (excelSheet.Cells[i, 1].Value2 != null)
                    {

                        if (excelSheet.Cells[i, 1].Value.ToString() != string.Empty)
                        {
                            string val = excelSheet.Cells[i, 1].Value.ToString();
                            if (val != string.Empty)
                            {
                                if (val.StartsWith("AOR"))
                                {
                                    vhsAOR = new AOR();
                                    vhsAOR.AORName = excelSheet.Cells[i, 1].Value.ToString();
                                    string valResponsable = excelSheet.Cells[i+1, 1].Value.ToString();

                                    SDCManagementSystem_Test3Entities db = new SDCManagementSystem_Test3Entities();
                                    DWT_Dealers d = db.DWT_Dealers.Where(x => x.Code == valResponsable.Substring(0, 6)).FirstOrDefault();
                                    if (d != null)
                                    {
                                        int _dealerResponsable = d.DealerId;
                                        vhsAOR.Dealer_IdResponsable = _dealerResponsable;
                                    }
                                }

                                if (valLineBefore == "Dealer Code and Name" && !val.StartsWith("AOR") )
                                {
                                    vhsCrossSellDealerAOR = new CrossSellDealerAOR();
                                    SDCManagementSystem_Test3Entities db = new SDCManagementSystem_Test3Entities();
                                    DWT_Dealers d = db.DWT_Dealers.Where(x => x.Code == val.Substring(0, 6)).FirstOrDefault();
                                    if (d != null)
                                    {
                                        _dealerid = d.DealerId;
                                        vhsCrossSellDealerAOR.Dealer_id = _dealerid;
                                    }


                                    vhsCrossSellDealerAOR.DealerCode = val.Substring(0, 6);
                                    int t = val.Length;
                                    vhsCrossSellDealerAOR.DealerName = val.Substring(7);

                                    vhsCrossSellDealerAOR.SalesFrom = _SalesFrom;
                                    vhsCrossSellDealerAOR.SalesThrough = _SalesThrough;
                                    vhsCrossSellDealerAOR.TotalSales = _ObjTotalSales;
                                    vhsCrossSellDealerAOR.AORName = vhsAOR.AORName;

                                    if (excelSheet.Cells[i, 2].Value2 != null)
                                        vhsCrossSellDealerAOR.DealerCity = excelSheet.Cells[i, 2].Value.ToString();

                                    if (excelSheet.Cells[i, 3].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 3].Value.ToString(), out int _DealerSaler))
                                            vhsCrossSellDealerAOR.DealerSaler = _DealerSaler;
                                    }

                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                    {
                                        if (decimal.TryParse(excelSheet.Cells[i, 4].Value.ToString(), out decimal _pctAORSales))
                                            vhsCrossSellDealerAOR.pctAORSales = _pctAORSales * 100;
                                    }
                                }

                                if (val.StartsWith("For"))
                                {
                                    //vhsCrossSellDealerAOR.SalesFrom = DateTime.Parse(val.Substring(15, 10));
                                    //vhsCrossSellDealerAOR.SalesThrough = DateTime.Parse(val.Substring(34, 10));
                                    _SalesFrom = DateTime.Parse(val.Substring(15, 10));
                                    _SalesThrough = DateTime.Parse(val.Substring(34, 10));

                                    if (excelSheet.Cells[i, 5].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 5].Value.ToString(), out int _TotalSales))
                                        {
                                            //vhsCrossSellDealerAOR.TotalSales = _TotalSales;
                                            _ObjTotalSales = _TotalSales;
                                        }

                                        }

                                }

                                if (val.StartsWith("Current Population"))
                                {
                                    if (excelSheet.Cells[i, 2].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 2].Value.ToString(), out int _CurrentPopulation))
                                            vhsAOR.CurrentPopulation = _CurrentPopulation;
                                    }
                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 4].Value.ToString(), out int _C5YrsProyectedPopulation))
                                            vhsAOR.C5YrsProyectedPopulation = _C5YrsProyectedPopulation;
                                    }

                                }

                                if (val.StartsWith("Current Households"))
                                {
                                    if (excelSheet.Cells[i, 2].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 2].Value.ToString(), out int _CurrentHouseholds))
                                            vhsAOR.CurrentHouseholds = _CurrentHouseholds;
                                    }
                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 4].Value.ToString(), out int _C5YrsProyectedHouseholds))
                                            vhsAOR.C5YrsProyectedHouseholds = _C5YrsProyectedHouseholds;
                                    }

                                }

                                if (val.StartsWith("Current HH Under $50,000"))
                                {
                                    if (excelSheet.Cells[i, 2].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 2].Value.ToString(), out int _CurrentHouseholdsUnder50))
                                            vhsAOR.CurrentHouseholdsUnder50 = _CurrentHouseholdsUnder50;
                                    }
                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 4].Value.ToString(), out int C5YrsProyectedHouseholdsUnder50))
                                            vhsAOR.C5YrsProyectedHouseholdsUnder50 = C5YrsProyectedHouseholdsUnder50;
                                    }

                                }

                                if (val.StartsWith("Current HH $50,000 - $100,000"))
                                {
                                    if (excelSheet.Cells[i, 2].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 2].Value.ToString(), out int _CurrentHouseholds50to100))
                                            vhsAOR.CurrentHouseholds50to100 = _CurrentHouseholds50to100;
                                    }
                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 4].Value.ToString(), out int _C5YrsProyectedHouseholds50to100))
                                            vhsAOR.C5YrsProyectedHouseholds50to100 = _C5YrsProyectedHouseholds50to100;
                                    }

                                }

                                if (val.StartsWith("Current HH Over $100,000"))
                                {
                                    if (excelSheet.Cells[i, 2].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 2].Value.ToString(), out int _CurrentHouseholdsOver100))
                                            vhsAOR.CurrentHouseholdsOver100 = _CurrentHouseholdsOver100;
                                    }
                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                    {
                                        if (int.TryParse(excelSheet.Cells[i, 4].Value.ToString(), out int _C5YrsProyectedHouseholdsOver100))
                                            vhsAOR.C5YrsProyectedHouseholdsOver100 = _C5YrsProyectedHouseholdsOver100;
                                    }

                                }

                                if (valLineBefore.Contains("DlrNum") && !val.StartsWith("Current Population"))
                                {
                                    vhsAORDealerHistory = new AORDealerHistory();
                                    vhsAORDealerHistory.AORName = vhsAOR.AORName;
                                    vhsAORDealerHistory.DealerCode = "0" + excelSheet.Cells[i, 1].Value.ToString();
                                    vhsAORDealerHistory.DealerName = excelSheet.Cells[i, 2].Value.ToString();
                                    vhsAORDealerHistory.DealerCity = excelSheet.Cells[i, 3].Value.ToString();

                                    if (excelSheet.Cells[i, 4].Value2 != null)
                                    {
                                        if (DateTime.TryParse(excelSheet.Cells[i, 4].Value.ToString(), out DateTime _ApptDate))
                                            vhsAORDealerHistory.ApptDate = _ApptDate;
                                    }

                                    if (excelSheet.Cells[i, 5].Value2 != null)
                                    {
                                        if (DateTime.TryParse(excelSheet.Cells[i, 5].Value.ToString(), out DateTime _TermDate))
                                            vhsAORDealerHistory.TermDate = _TermDate;
                                    }
                                }
                            }
                            if (val.StartsWith("DlrNum")) 
                                valLineBefore = val;
                            if (valLineBefore.StartsWith("DlrNum") && val.StartsWith("Current Population"))
                                valLineBefore = string.Empty;

                            if (valLineBefore == string.Empty && val.StartsWith("Dealer Code and Name"))
                                valLineBefore = "Dealer Code and Name";

                            if (valLineBefore == "Dealer Code and Name" && val.StartsWith("AOR"))
                                valLineBefore = string.Empty;

                            if (vhsAOR!=null)
                                listAOR.Add(vhsAOR);
                            if (vhsAORDealerHistory != null)
                            { 
                                listAORH.Add(vhsAORDealerHistory);
                                vhsAORDealerHistory = null;
                            }
                            if (vhsCrossSellDealerAOR != null)
                                listAORCrossSale.Add(vhsCrossSellDealerAOR);
                        }

                    }
                }
                

                object misValue = System.Reflection.Missing.Value;
                wb.Close(false, misValue, misValue);
                excel.Quit();

                using (SDCManagementSystem_Test3Entities db = new SDCManagementSystem_Test3Entities())
                {
                    foreach (AOR dealer in listAOR)
                    {
                        if (dealer.AORName != string.Empty)
                        {
                            AOR vhs = db.AORs.Where(
                                x => x.AORName == dealer.AORName).FirstOrDefault();

                            if (vhs != null)
                            {   //modify record    
                                vhs.CurrentPopulation = dealer.CurrentPopulation;
                                vhs.C5YrsProyectedPopulation = dealer.C5YrsProyectedPopulation;
                                vhs.CurrentHouseholds = dealer.CurrentHouseholds;
                                vhs.C5YrsProyectedHouseholds = dealer.C5YrsProyectedHouseholds;
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
                                db.AORs.Add(dealer);
                                newRows++;
                            }
                        }
                        else
                        {
                            db.AORs.Add(dealer);
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

                    foreach (AORDealerHistory dealer in listAORH)
                    {
                        if (dealer.AORName != null && dealer.AORName != string.Empty )
                        {
                            AORDealerHistory vhs = db.AORDealerHistories.Where(
                                x => x.AORName == dealer.AORName).FirstOrDefault();

                            if (vhs != null)
                            {   //modify record    
                                vhs.DealerCode = dealer.DealerCode;
                                vhs.DealerName = dealer.DealerName;
                                vhs.DealerCity = dealer.DealerCity;
                                vhs.ApptDate = dealer.ApptDate;
                                vhs.TermDate = dealer.TermDate;
                                updatedRow++;
                            }
                            else
                            {
                                db.AORDealerHistories.Add(dealer);
                                newRows++;
                            }
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

                    foreach (CrossSellDealerAOR dealer in listAORCrossSale)
                    {
                        if (dealer.Dealer_id != 0)
                        {
                            CrossSellDealerAOR vhs = db.CrossSellDealerAORs.Where(
                                x => x.Dealer_id == dealer.Dealer_id && x.SalesFrom == dealer.SalesFrom && x.SalesThrough == dealer.SalesThrough).FirstOrDefault();

                            if (vhs != null)
                            {   //modify record    
                                vhs.TotalSales = dealer.TotalSales;
                                vhs.DealerCode = dealer.DealerCode;
                                vhs.DealerName = dealer.DealerName;
                                vhs.DealerCity = dealer.DealerCity;
                                vhs.DealerSaler = dealer.DealerSaler;
                                vhs.pctAORSales = dealer.pctAORSales;
                                updatedRow++;
                            }
                            else
                            {
                                db.CrossSellDealerAORs.Add(dealer);
                                newRows++;
                            }
                        }
                        else
                        {
                            db.CrossSellDealerAORs.Add(dealer);
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