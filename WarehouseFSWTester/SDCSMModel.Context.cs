﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SDCManagementSystem_Test3Entities : DbContext
    {
        public SDCManagementSystem_Test3Entities()
            : base("name=SDCManagementSystem_Test3Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<WareHouseReceipt> WareHouseReceipts { get; set; }
        public virtual DbSet<DWT_Dealers> DWT_Dealers { get; set; }
        public virtual DbSet<CrossSellZip> CrossSellZips { get; set; }
        public virtual DbSet<RetailComparison> RetailComparisons { get; set; }
        public virtual DbSet<AORDealerHistory> AORDealerHistories { get; set; }
        public virtual DbSet<CrossSellDealerAOR> CrossSellDealerAORs { get; set; }
        public virtual DbSet<AOR> AORs { get; set; }
        public virtual DbSet<AORzipCode> AORzipCodes { get; set; }
    
        public virtual ObjectResult<spu_warehousePriceUpdate_Result> spu_warehousePriceUpdate()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spu_warehousePriceUpdate_Result>("spu_warehousePriceUpdate");
        }
    
        public virtual int spu_warehousePriceUpdate2()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spu_warehousePriceUpdate2");
        }
    }
}
