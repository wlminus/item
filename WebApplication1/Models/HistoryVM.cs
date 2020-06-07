using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class HistoryVM
    {
        public long Id { get; set; }
        public long Date { get; set; }

        public long ItemId { get; set; }

        public String Item{ get; set; }

        public long FromHouseId { get; set; }
        public long FromRoomId { get; set; }
        public long FromStatusId { get; set; }

        public String FromHouse { get; set; }
        public String FromRoom { get; set; }
        public String FromStatus { get; set; }


        public long ToHouseId { get; set; }
        public long ToRoomId { get; set; }
        public long ToStatusId { get; set; }

        public String ToHouse { get; set; }
        public String ToRoom { get; set; }
        public String ToStatus { get; set; }

        public long MediaId { get; set; }
        public virtual Media Media { get; set; }

        public Boolean IsVerified { get; set; }

        public long VerifiedBy { get; set; }
        public String Verified { get; set; }

        public String Description { get; set; }


    }
}