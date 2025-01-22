namespace Web1.Repository
{
    public interface IPasswordRepository
    {
        public Task<string> GetPassword(string email, string userName);

        public Task UpdatePassword(string email, string userName, string password);
    }
}
