﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SITAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class sitdbEntities : DbContext
    {
        public sitdbEntities()
            : base("name=sitdbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<bet> bets { get; set; }
        public virtual DbSet<cfgUnit> cfgUnits { get; set; }
        public virtual DbSet<company> companys { get; set; }
        public virtual DbSet<game> games { get; set; }
        public virtual DbSet<topic> topics { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<choice> choices { get; set; }
        public virtual DbSet<payout> payouts { get; set; }
        public virtual DbSet<cfgGameStatu> cfgGameStatus { get; set; }
        public virtual DbSet<choiceOdd> choiceOdds { get; set; }
        public virtual DbSet<betCount> betCounts { get; set; }
        public virtual DbSet<bonu> bonus { get; set; }
        public virtual DbSet<choiceStr> choiceStrs { get; set; }
        public virtual DbSet<gamedragon> gamedragons { get; set; }
        public virtual DbSet<prizepoolRecord> prizepoolRecords { get; set; }
        public virtual DbSet<bonusSetting> bonusSettings { get; set; }
        public virtual DbSet<gameSetting> gameSettings { get; set; }
        public virtual DbSet<choiceSetting> choiceSettings { get; set; }
        public virtual DbSet<topicSetting> topicSettings { get; set; }
    }
}
