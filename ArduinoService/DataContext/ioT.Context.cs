﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArduinoService.DataContext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ioTEntities : DbContext
    {
        public ioTEntities()
            : base("name=ioTEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<D_DEVICE_SENSOR_DETAIL> D_DEVICE_SENSOR_DETAIL { get; set; }
        public virtual DbSet<D_DEVICE_SENSOR_HT> D_DEVICE_SENSOR_HT { get; set; }
        public virtual DbSet<D_DEVICE_SENSOR_LIGHT> D_DEVICE_SENSOR_LIGHT { get; set; }
        public virtual DbSet<D_DEVICE_SENSOR_MOISTURE> D_DEVICE_SENSOR_MOISTURE { get; set; }
        public virtual DbSet<D_GROUP_PERMISSION> D_GROUP_PERMISSION { get; set; }
        public virtual DbSet<D_PACKED> D_PACKED { get; set; }
        public virtual DbSet<D_SETTING_CONTROL> D_SETTING_CONTROL { get; set; }
        public virtual DbSet<D_SETTING_SENSOR> D_SETTING_SENSOR { get; set; }
        public virtual DbSet<S_ARDUINO_TYPE> S_ARDUINO_TYPE { get; set; }
        public virtual DbSet<S_ARDUINO_TYPE_DETAIL> S_ARDUINO_TYPE_DETAIL { get; set; }
        public virtual DbSet<S_CODE> S_CODE { get; set; }
        public virtual DbSet<S_DEVICE> S_DEVICE { get; set; }
        public virtual DbSet<S_DEVICE_CATEGORY> S_DEVICE_CATEGORY { get; set; }
        public virtual DbSet<S_DEVICE_CONTROL_DETAIL> S_DEVICE_CONTROL_DETAIL { get; set; }
        public virtual DbSet<S_GARDEN> S_GARDEN { get; set; }
        public virtual DbSet<S_GROUP_SENSOR> S_GROUP_SENSOR { get; set; }
        public virtual DbSet<S_UNIT> S_UNIT { get; set; }
        public virtual DbSet<S_USER> S_USER { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    }
}
