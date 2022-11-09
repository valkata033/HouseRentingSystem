using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Infrastructure.Data;

namespace HouseRentingSystem.Core.Services
{
    public class AgentService : IAgentService
    {
        private readonly ApplicationDbContext context;

        public AgentService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async void Create(string userId, string phoneNumber)
        {
            var agent = new Agent()
            {
                UserId = userId,
                PhoneNumber = phoneNumber
            };

            await context.Agents.AddAsync(agent);
            await context.SaveChangesAsync();
        }

        public bool ExistById(string userId)
        {
            return context.Agents.Any(x => x.UserId == userId);

        }

        public bool UserHasRents(string userId)
        {
            return context.Houses.Any(x => x.RenterId == userId);
        }

        public bool UserWithPhoneNumberExist(string phoneNumber)
        {
            return context.Agents.Any(x => x.PhoneNumber == phoneNumber);
        }
    }
}
