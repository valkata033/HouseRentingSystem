using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Infrastructure.Data;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace HouseRentingSystem.Core.Services
{
    public class AgentService : IAgentService
    {
        private readonly IRepository repo;

        public AgentService(IRepository _repo)
        {
            repo = _repo;
        }

        public async Task<int> GetAgentId(string userId)
        {
            return (await repo.AllReadonly<Agent>()
                .FirstOrDefaultAsync(x => x.UserId == userId))?.Id ?? 0;
        }

        public async Task Create(string userId, string phoneNumber)
        {
            var agent = new Agent()
            {
                UserId = userId,
                PhoneNumber = phoneNumber
            };

            await repo.AddAsync(agent);
            await repo.SaveChangesAsync();
        }

        public async Task<bool> ExistById(string userId)
        {
            return await repo.All<Agent>().AnyAsync(x => x.UserId == userId);

        }

        public async Task<bool> UserHasRents(string userId)
        {
            return await repo.All<House>().AnyAsync(x => x.RenterId == userId);
        }

        public async Task<bool> UserWithPhoneNumberExist(string phoneNumber)
        {
            return await repo.All<Agent>().AnyAsync(x => x.PhoneNumber == phoneNumber);
        }
    }
}
