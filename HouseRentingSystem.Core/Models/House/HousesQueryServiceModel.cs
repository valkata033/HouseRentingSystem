using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Core.Models.House
{
    public class HousesQueryServiceModel
    {
        public int TotalHouseCount { get; set; }

        public IEnumerable<HouseServiceModel> Houses { get; set; }
            = new List<HouseServiceModel>();
    }
}
