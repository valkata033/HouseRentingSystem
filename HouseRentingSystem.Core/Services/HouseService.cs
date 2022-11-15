using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.House;
using HouseRentingSystem.Infrastructure.Data;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace HouseRentingSystem.Core.Services
{
    public class HouseService : IHouseService
    {
        private readonly IRepository repo;

        public HouseService(IRepository _repo)
        {
            repo = _repo;
        }

        public async Task<HousesQueryServiceModel> All(string category = null, string searchItem = null,
            HouseSorting sorting = HouseSorting.Newest, int currentPage = 1, int housePerPage = 1)
        {
            var housesQuery = repo.All<House>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                housesQuery = repo.All<House>().Where(x => x.Category.Name == category);
            }

            if (!string.IsNullOrWhiteSpace(searchItem))
            {
                housesQuery = housesQuery.Where(x =>
                    x.Title.ToLower().Contains(searchItem.ToLower()) ||
                    x.Address.ToLower().Contains(searchItem.ToLower()) ||
                    x.Description.ToLower().Contains(searchItem.ToLower()));
            }

            housesQuery = sorting switch
            {
                HouseSorting.Price => housesQuery.OrderBy(x => x.PricePerMonth),
                HouseSorting.NotRentedFirst => housesQuery.OrderBy(x => x.RenterId != null)
                    .ThenByDescending(x => x.Id),
                HouseSorting.Newest => housesQuery.OrderByDescending(x => x.Id)
            };

            var houses = housesQuery
                .Skip((currentPage - 1) * housePerPage)
                .Take(housePerPage)
                .Select(x => new HouseServiceModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Address = x.Address,
                    ImageUrl = x.ImageUrl,
                    IsRented = x.RenterId != null,
                    PricePerMonth = x.PricePerMonth
                })
                .ToList();

            var totalHouses = housesQuery.Count();

            return new HousesQueryServiceModel
            {
                Houses = houses,
                TotalHouseCount = totalHouses
            };
        }

        public async Task<IEnumerable<HouseCategoryServiceModel>> AllCategories()
        {
            return await repo.AllReadonly<Category>()
                .OrderBy(c => c.Name)
                .Select(x => new HouseCategoryServiceModel 
                { 
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> AllCategoriesNames()
        {
            return await repo.AllReadonly<Category>()
                .Select(x => x.Name)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> CategoryExist(int categoryId)
        {
            return await repo.All<Category>().AnyAsync(c => c.Id == categoryId);
        }

        public async Task<int> Create(HouseFormModel model, int agentId)
        {
            var house = new House()
            {
                Title = model.Title,
                Address = model.Address,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                PricePerMonth = model.PricePerMonth,
                CategoryId = model.CategoryId,
                AgentId = agentId
            };

            await repo.AddAsync(house);
            await repo.SaveChangesAsync();

            return house.Id;
        }

        public async Task<IEnumerable<HouseHomeModel>> LastThreeHouses()
        {
            return await repo.AllReadonly<House>()
                .OrderByDescending(x => x.Id)
                .Select(x => new HouseHomeModel 
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Title = x.Title
                })
                .Take(3)
                .ToListAsync();
        }
    }
}
