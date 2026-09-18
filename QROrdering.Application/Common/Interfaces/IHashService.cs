namespace QROrdering.Application.Common.Interfaces
{
    public interface IHashService
    {
        string Hash(string value);
        bool Verify(string value, string hashedValue);
    }
}
