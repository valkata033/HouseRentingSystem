using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.House;
using HouseRentingSystem.Infrastructure.Data;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace HouseRentingSystem.Core.Services
{
    public class HouseService : IHouseService
    {
        //private readonly IRepository repo;
        private readonly HouseRentingDbContext context;

        public HouseService(HouseRentingDbContext _context)
        {
            //repo = _repo;
            context = _context;
        }

        public async Task<IEnumerable<HouseCategoryServiceModel>> AllCategories()
        {
            return await context.Categories
                .Select(x => new HouseCategoryServiceModel 
                { 
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public bool CategoryExist(int categoryId)
        {
            return context.Categories.Any(c => c.Id == categoryId);
        }

        public int Create(string title, string address, string description, string imageUrl, decimal price, int categoryId, int agentId)
        {
            var house = new House()
            {
                Title = title,
                Address = address,
                Description = description,
                ImageUrl = imageUrl,
                PricePerMonth = price,
                AgentId = agentId,
                CategoryId = categoryId
            };

            context.Houses.AddAsync(house);
            context.SaveChangesAsync();

            return house.Id;
        }

        public async Task<IEnumerable<HouseHomeModel>> LastThreeHouses()
        {
            return await context.Houses
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
