//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WarehouseFSWTester
{
    using System;
    using System.Collections.Generic;
    
    public partial class CrossSellDealerAOR
    {
        public int CrossSellDealerAORid { get; set; }
        public string AORName { get; set; }
        public Nullable<int> Dealer_id { get; set; }
        public Nullable<System.DateTime> SalesFrom { get; set; }
        public Nullable<System.DateTime> SalesThrough { get; set; }
        public Nullable<int> TotalSales { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string DealerCity { get; set; }
        public Nullable<int> DealerSaler { get; set; }
        public Nullable<decimal> pctAORSales { get; set; }
    }
}