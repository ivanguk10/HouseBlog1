using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseBlog.Models
{
    public class House
    {
        public int HouseId { get; set; }
        public string HouseName { get; set; }
        public string HouseDescription { get; set; }

        public string HouseImage { get; set; }
    }
}
