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
    static class CrossSellZipFiles
    {
        public static List<string> ProcessFiles(string filePath)
        {
            int newRows = 0;
            int updatedRow = 0;
            List<string> result = new List<string>();

            #region Process File

            string file = filePath;
            string fileLine = string.Empty;

            if (File.Exists(file))
            {
                Application excel = new Application();
                Workbook wb = excel.Workbooks.Open(file);
                Worksheet excelSheet = wb.ActiveSheet;
                Range xlRange = excelSheet.UsedRange;

                List<CrossSellZip> list = new List<CrossSellZip>();
                if (excelSheet.Cells[1, 1].Value.ToString() == "DATE")
                {
                    for (int i = 2; i < xlRange.Columns[1].Rows.Count; i++)
                    {

                        CrossSellZip vhs = new CrossSellZip();

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
                System.IO.File.Delete(sourcePath);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}