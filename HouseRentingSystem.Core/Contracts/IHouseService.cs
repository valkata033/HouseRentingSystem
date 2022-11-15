using HouseRentingSystem.Core.Models.House;

namespace HouseRentingSystem.Core.Contracts
{
    public interface IHouseService
    {
        Task<IEnumerable<HouseHomeModel>> LastThreeHouses();

        Task<IEnumerable<HouseCategoryServiceModel>> AllCategories();

        Task<bool> CategoryExist(int categoryId);

        Task<int> Create(HouseFormModel model, int agentId);

        Task<HousesQueryServiceModel> All(string category = null, string searchItem = null,
            HouseSorting sorting = HouseSorting.Newest, int currentPage = 1,
            int housePerPage = 1);

        Task<IEnumerable<string>> AllCategoriesNames();

    }
}
