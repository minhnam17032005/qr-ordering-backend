namespace QROrdering.Application.Common.Interfaces
{
    public interface IRequestInfoService
    {
        string IpAddress { get; }

        string UserAgent { get; }

        string DeviceName { get; }
    }
}
