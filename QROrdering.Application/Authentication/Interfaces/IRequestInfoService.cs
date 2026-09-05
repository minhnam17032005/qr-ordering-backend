namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IRequestInfoService
    {
        string IpAddress { get; }

        string UserAgent { get; }

        string DeviceName { get; }
    }
}
