﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [Table("HitPosOrders")]
    [DisplayName("HitPosOrders")]
    public class HitPosOrdersDTO
    {
        [Column("CurId", Order = 1, TypeName = "BIGINT")]
        [Key]
        [Editable(true)]
        [Phone]
        [DisplayName("HitPosOrders_PK")]
        public long CurId { get; set; }

        [Column("id", Order = 2, TypeName = "INT")]
        [Required]
        public int id { get; set; }

        [Column("orderno", Order = 3, TypeName = "INT")]
        public Nullable<int> orderno { get; set; }

        [Column("pos", Order = 4, TypeName = "INT")]
        public Nullable<int> pos { get; set; }

        [Column("shop_id", Order = 5, TypeName = "NVARCHAR(50)")]
        public string shop_id { get; set; }

        [Column("item_group", Order = 6, TypeName = "INT")]
        public Nullable<int> item_group { get; set; }

        [Column("item_code", Order = 7, TypeName = "INT")]
        public Nullable<int> item_code { get; set; }

        [Column("item_descr", Order = 8, TypeName = "NVARCHAR(60)")]
        public string item_descr { get; set; }

        [Column("item_subgroup", Order = 9, TypeName = "INT")]
        public Nullable<int> item_subgroup { get; set; }

        [Column("item_vat", Order = 10, TypeName = "INT")]
        public Nullable<int> item_vat { get; set; }

        [Column("cont_line", Order = 11, TypeName = "DECIMAL(18,0)")]
        public Nullable<decimal> cont_line { get; set; }

        [Column("sp", Order = 12, TypeName = "NTEXT")]
        public string sp { get; set; }

        [Column("prep_time", Order = 13, TypeName = "DATETIME")]
        public Nullable<System.DateTime> prep_time { get; set; }

        [Column("start_time", Order = 14, TypeName = "DATETIME")]
        public Nullable<System.DateTime> start_time { get; set; }

        [Column("load_time", Order = 15, TypeName = "INT")]
        public Nullable<int> load_time { get; set; }

        [Column("otd", Order = 16, TypeName = "INT")]
        public Nullable<int> otd { get; set; }

        [Column("qty", Order = 17, TypeName = "INT")]
        public Nullable<int> qty { get; set; }

        [Column("amount", Order = 18, TypeName = "INT")]
        public Nullable<int> amount { get; set; }

        [Column("total", Order = 19, TypeName = "INT")]
        public Nullable<int> total { get; set; }

        [Column("waiter", Order = 20, TypeName = "INT")]
        public Nullable<int> waiter { get; set; }

        [Column("ttable", Order = 21, TypeName = "INT")]
        public Nullable<int> ttable { get; set; }

        [Column("listino", Order = 22, TypeName = "INT")]
        public Nullable<int> listino { get; set; }

        [Column("receipt", Order = 23, TypeName = "INT")]
        public Nullable<int> receipt { get; set; }

        [Column("member", Order = 24, TypeName = "NVARCHAR(50)")]
        public string member { get; set; }

        [Column("priority", Order = 25, TypeName = "INT")]
        public Nullable<int> priority { get; set; }

        [Column("kdws", Order = 26, TypeName = "INT")]
        public Nullable<int> kdws { get; set; }

        [Column("ready", Order = 27, TypeName = "INT")]
        public Nullable<int> ready { get; set; }

        [Column("rqty", Order = 28, TypeName = "INT")]
        public Nullable<int> rqty { get; set; }

        [Column("nieko_flag", Order = 29, TypeName = "INT")]
        public Nullable<int> nieko_flag { get; set; }

        [Column("status", Order = 30, TypeName = "INT")]
        public Nullable<int> status { get; set; }

        [Column("status_time", Order = 31, TypeName = "DATETIME")]
        public Nullable<System.DateTime> status_time { get; set; }

        [Column("rest_time", Order = 32, TypeName = "INT")]
        public Nullable<int> rest_time { get; set; }

        [Column("room", Order = 33, TypeName = "NVARCHAR(50)")]
        public string room { get; set; }

        [Column("payment", Order = 34, TypeName = "NVARCHAR(50)")]
        public string payment { get; set; }
        
        [Column("type", Order = 35, TypeName = "NVARCHAR(50)")]
        public string type { get; set; }

        [Column("comments", Order = 36, TypeName = "NVARCHAR(255)")]
        public string comments { get; set; }

        [Column("mqty", Order = 37, TypeName = "INT")]
        public Nullable<int> mqty { get; set; }

        [Column("rec_time_start", Order = 38, TypeName = "DATETIME")]
        public Nullable<System.DateTime> rec_time_start { get; set; }

        [Column("status_time2", Order = 39, TypeName = "DATETIME")]
        public Nullable<System.DateTime> status_time2 { get; set; }

        [Column("status_time3", Order = 40, TypeName = "DATETIME")]
        public Nullable<System.DateTime> status_time3 { get; set; }

        [Column("status_time4", Order = 41, TypeName = "DATETIME")]
        public Nullable<System.DateTime> status_time4 { get; set; }

        [Column("status_time5", Order = 42, TypeName = "DATETIME")]
        public Nullable<System.DateTime> status_time5 { get; set; }

        [Column("fo_day", Order = 43, TypeName = "DATETIME")]
        public Nullable<System.DateTime> fo_day { get; set; }

        [Column("delivery_time", Order = 44, TypeName = "DATETIME")]
        public Nullable<System.DateTime> delivery_time { get; set; }

        [Column("agent", Order = 45, TypeName = "INT")]
        public Nullable<int> agent { get; set; }

        [Column("flag_up", Order = 46, TypeName = "INT")]
        public Nullable<int> flag_up { get; set; }

        [Column("sent", Order = 47, TypeName = "INT")]
        public Nullable<int> sent { get; set; }

        [Column("correct", Order = 48, TypeName = "INT")]
        public Nullable<int> correct { get; set; }

        [Column("completed", Order = 49, TypeName = "INT")]
        public Nullable<int> completed { get; set; }

        [Column("CreationDate", Order = 50, TypeName = "DATETIME")]
        public Nullable<System.DateTime> CreationDate { get; set; }
    }
}
