namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IPasswordService
    {
        string Hash(string password);

        bool Verify(string password, string passwordHash);
    }
}
