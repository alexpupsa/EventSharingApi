namespace EventSharingApi.Repository
{
    public interface IUserRepository
    {
        Task<bool> Register(string username);
    }
}
