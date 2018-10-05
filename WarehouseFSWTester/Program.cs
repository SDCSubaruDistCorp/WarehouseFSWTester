using System;
using System.Configuration;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace WarehouseFSWTester
{
    class Program
    {
        static int i = 1;
        static string year = ConfigurationManager.AppSettings["year"];
        static string month = "1";
        static string day = "1";
        static void Main(string[] args)
        {

            FileSystemWatcher FSW = new FileSystemWatcher
            {
                Path = ConfigurationManager.AppSettings["ObjetivePath"],
                Filter = "*.*",
                EnableRaisingEvents = true
            };
            FSW.Created += FSWCreateFile;
            while (i==1)
            {
                Console.WriteLine("check");
                System.Threading.Thread.Sleep(2000);
            }
        }

        static void FSWCreateFile (object sender, System.IO.FileSystemEventArgs e)
        {
            if (!e.Name.Contains("~"))
            {
                Console.WriteLine(e.FullPath);
                if (e.Name.Contains("Wholesale Retail Comparison Report"))
                {
                    RetailComparisonFile.ProcessFiles(e.FullPath);
                    RetailComparisonFile.MoveFile(e.FullPath, e.Name);
                }
                if (e.Name.Contains("Cross Sell In"))
                {
                    CrossSalesAORFile.ProcessFiles(e.FullPath);
                    CrossSalesAORFile.MoveFile(e.FullPath, e.Name);
                }
                if (e.Name.Contains("Areas Of Responsability"))
                {
                    AORZipCodeFile.ProcessFiles(e.FullPath);
                    AORZipCodeFile.MoveFile(e.FullPath, e.Name);
                }
                else if (e.Name.Contains("xlsx"))
                {
                    CrossSalesZipFile.ProcessFiles(e.FullPath);
                    CrossSalesZipFile.MoveFile(e.FullPath, e.Name);
                }



                else if (e.Name.Contains("xlsm"))
                {
                    WareHouseReceiptFiles.ProcessFiles(e.FullPath);
                    WareHouseReceiptFiles.PriceUpdate();
                    WareHouseReceiptFiles.MoveFile(e.FullPath, e.Name);
                    var foundIndexes = new List<int>();
                    for (int i = e.Name.IndexOf('-'); i > -1; i = e.Name.IndexOf('-', i + 1))
                    {
                        foundIndexes.Add(i);
                    }

                    string[] info = e.Name.Split('-');

                    //month = e.Name.Substring(0, foundIndexes[0]);
                    month = info[0];

                    //day = e.Name.Substring(foundIndexes[0] + 1, foundIndexes[1] - 2);
                    day = info[1];

                    WareHouseReceiptFiles.SafePDF(year, month, day);
                    
                }
                
                Console.WriteLine("done");
            }
        }

    }
}
