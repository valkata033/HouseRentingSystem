namespace HouseRentingSystem.Core.Contracts
{
    public interface IAgentService
    {
        bool ExistById(string userId);

        bool UserWithPhoneNumberExist(string phoneNumber);

        bool UserHasRents(string userId);

        void Create(string userId, string phoneNumber);

    }
}
