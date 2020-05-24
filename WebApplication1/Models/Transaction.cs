using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("Transaction")]
    public class Transaction : BaseEntity
    {
        [Column("Date")]
        public long Date { get; set; }

        [Column("ItemId")]
        public long ItemId { get; set; }

        [Column("FromHouseId")]
        public long FromHouseId { get; set; }

        [Column("FromRoomId")]
        public long FromRoomId { get; set; }

        [Column("FromStatusId")]
        public long FromStatusId { get; set; }

        [Column("ToHouseId")]
        public long ToHouseId { get; set; }

        [Column("ToRoomId")]
        public long ToRoomId { get; set; }

        [Column("ToStatusId")]
        public long ToStatusId { get; set; }

        [Column("MediaId")]
        public long MediaId { get; set; }
        public virtual Media Media { get; set; }

        [Column("IsVerified")]
        public Boolean IsVerified { get; set; }

        [Column("VerifiedBy")]
        public long VerifiedById { get; set; }

        [Column("Description")]
        public String Description { get; set; }
    }
}