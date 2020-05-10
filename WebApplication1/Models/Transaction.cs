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

        [Column("FromHouse")]
        [StringLength(500)]
        public string FromHouse { get; set; }

        [Column("FromRoom")]
        [StringLength(500)]
        public string FromRoom { get; set; }

        [Column("ToHouse")]
        [StringLength(500)]
        public string ToHouse { get; set; }

        [Column("ToRoom")]
        [StringLength(500)]
        public string ToRoom { get; set; }

        [Column("FromStatus")]
        [StringLength(500)]
        public string FromStatus { get; set; }

        [Column("ToStatus")]
        [StringLength(500)]
        public string ToStatus { get; set; }

        [Column("IsVerified")]
        public Boolean IsVerified { get; set; }

        public int TransactionCategoryId { get; set; }
        public virtual TransactionCategory TransactionCategory { get; set; }
    }
}