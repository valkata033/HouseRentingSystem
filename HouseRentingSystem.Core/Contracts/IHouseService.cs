using HouseRentingSystem.Core.Models.House;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Core.Contracts
{
    public interface IHouseService
    {
        Task<IEnumerable<HouseHomeModel>> LastThreeHouses();

        Task<IEnumerable<HouseCategoryServiceModel>> AllCategories();

        bool CategoryExist(int categoryId);

        int Create(string title, string address, string description,
            string imageUrl, decimal price, int categoryId, int agentId);


    }
}
