using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.Agent;
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

        public async Task<IEnumerable<HouseServiceModel>> AllHousesByAgentId(int agentId)
        {
            var houses = await repo
                .AllReadonly<House>()
                .Where(x => x.AgentId == agentId)
                .ToListAsync();

            return ProjectToModel(houses);
        }

        private List<HouseServiceModel> ProjectToModel(List<House> houses)
        {
            var resultHouses = houses
                .Select(x => new HouseServiceModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Address = x.Address,
                    ImageUrl = x.ImageUrl,
                    PricePerMonth = x.PricePerMonth,
                    IsRented = x.RenterId != null
                })
                .ToList();

            return resultHouses;
        }

        public async Task<IEnumerable<HouseServiceModel>> AllHousesByUserId(string userId)
        {
            var houses = await repo
            .AllReadonly<House>()
                .Where(x => x.RenterId == userId)
                .ToListAsync();

            return ProjectToModel(houses);
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

        public async Task<bool> Exists(int Id)
        {
            return await repo.AllReadonly<House>().AnyAsync(c => c.Id == Id);
        }

        public async Task<HouseDetailsServiceModel> HouseDetailsById(int Id)
        {
            return await repo.AllReadonly<House>()
                .Where(x => x.Id == Id)
                .Select(x => new HouseDetailsServiceModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Address = x.Address,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    PricePerMonth = x.PricePerMonth,
                    IsRented = x.RenterId != null,
                    Category = x.Category.Name,
                    Agent = new AgentServiceModel()
                    {
                        PhoneNumber = x.Agent.PhoneNumber,
                        Email = x.Agent.User.Email
                    }
                })
                .FirstOrDefaultAsync();
        }

        public async Task Edit(int houseId, string title, string address, string description,
            string imageUrl, decimal price, int categoryId)
        {
            var house = repo.All<House>().First(x => x.Id == houseId);

            house.Title = title;
            house.Address = address;
            house.Description = description;
            house.ImageUrl = imageUrl;
            house.PricePerMonth = price;
            house.CategoryId = categoryId;

            await repo.SaveChangesAsync();
        }

        public async Task<bool> HasAgentWithId(int houseId, string currentUserId)
        {
            var house = await repo.AllReadonly<House>().FirstOrDefaultAsync(x => x.Id == houseId);

            var agent = await repo.AllReadonly<Agent>().FirstOrDefaultAsync(x => x.Id == house.AgentId);

            if (agent == null)
            {
                return false;
            }

            if (agent.UserId != currentUserId)
            {
                return false;
            }

            return true;
        }

        public async Task<int> GetHouseCategoryId(int houseId)
        {
            var house = await repo.AllReadonly<House>().FirstOrDefaultAsync(x => x.Id == houseId);
            return house?.CategoryId ?? -1;
        }

        public async Task Delete(int houseId)
        {
            var house = await repo.All<House>().FirstOrDefaultAsync(x => x.Id == houseId);

            repo.Delete(house);
            await repo.SaveChangesAsync();
        }

        public async Task<bool> IsRented(int id)
        {
            var house = await repo.AllReadonly<House>().FirstOrDefaultAsync(x => x.Id == id);

            return house.RenterId != null;
        }

        public async Task<bool> IsRentedByUserById(int houseId, string userId)
        {
            var house = await repo.AllReadonly<House>().FirstOrDefaultAsync(x => x.Id == houseId);

            if (house == null)
            {
                return false;
            }

            if (house.RenterId != null)
            {
                return false;
            }

            return true;
        }

        public async Task Rent(int houseId, string userId)
        {
            var house = await repo.AllReadonly<House>().FirstOrDefaultAsync(x => x.Id == houseId);

            house.RenterId = userId;
            await repo.SaveChangesAsync();
        }

        public async Task Leave(int houseId)
        {
            var house = await repo.AllReadonly<House>().FirstOrDefaultAsync(x => x.Id == houseId);

            house.RenterId = null;
            await repo.SaveChangesAsync();
        }
    }
}
