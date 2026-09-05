namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IHashService
    {
        string Hash(string value);
        bool Verify(string value, string hashedValue);
    }
}
