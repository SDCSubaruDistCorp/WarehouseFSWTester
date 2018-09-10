using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SalesController : Controller
    {
        // GET: Sales
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSales()
        {
            SalesGeoInfoModel sm = new SalesGeoInfoModel();
            var s = sm.GetSale();
            return Json(new { Result = s }, JsonRequestBehavior.AllowGet);
        }

        public void UpdateCoordinate(int saleId, string lat, string lng)
        {
            SalesGeoInfoModel sm = new SalesGeoInfoModel();
            sm.UpdateSaleGeoInfo(saleId, lat, lng);
        }

    }
}