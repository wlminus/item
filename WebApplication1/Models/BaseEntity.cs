using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class BaseEntity
    {
        [Key]
        public long Id { get; set; }

        //[StringLength(50)]
        //public string CreateBy { get; set; }

        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        //public DateTime? ICreateDate { get; set; }

        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        //public DateTime? IUpdateDate { get; set; }

        //[StringLength(50)]
        //public string IUpdateBy { get; set; }

    }
}