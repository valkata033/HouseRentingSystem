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

        Task<IEnumerable<HouseServiceModel>> AllHousesByAgentId(int agentId);

        Task<IEnumerable<HouseServiceModel>> AllHousesByUserId(string userId);

        Task<bool> Exists(int Id);

        Task<HouseDetailsServiceModel> HouseDetailsById(int Id);

        Task Edit(int houseId, string title, string address, string description,
            string imageUrl, decimal price, int categoryId);

        Task<bool> HasAgentWithId(int houseId, string currentUserId);

        Task<int> GetHouseCategoryId(int houseId);

        Task Delete(int houseId);

        Task<bool> IsRented(int id);

        Task<bool> IsRentedByUserById(int houseId, string userId);

        Task Rent(int houseId, string userId);

        Task Leave(int houseId);

    }
}
