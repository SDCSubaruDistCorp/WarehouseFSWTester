using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class SalesGeoInfoModel
    {
        private SDCManagementSystem_Test3Entities db = null;

        public SalesGeoInfo GetSale()
        {
            SalesGeoInfo sale = new SalesGeoInfo();
            using (db = new SDCManagementSystem_Test3Entities())
            {
                sale = db.SalesGeoInfoes.Where(x => x.SaleId == 487834).FirstOrDefault();

                return sale;
            }
        }

        public void UpdateSaleGeoInfo(int SaleId, string lat, string lng)
        {
            using (db = new SDCManagementSystem_Test3Entities())
            {
                SalesGeoInfo vhs = db.SalesGeoInfoes.Where(x => x.SaleId == SaleId).FirstOrDefault();
                if (vhs != null)
                {
                    vhs.lat = lat;
                    vhs.lng = lng;
                    db.SaveChanges();
                }
            }
        }
    }
}