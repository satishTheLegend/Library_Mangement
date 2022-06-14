namespace Library_Mangement.Services.PlatformServices
{
    public interface IDeviceInfo
    {
        string DeviceID { get; }
        string Version { get; }
        string PackageName { get; }
        string OSVersion { get; }
        string Model { get; }
        string AppName { get; }
        string Platform { get; }
        string BuildNumber { get; }
        string GetIdentifier { get; }
        string GetDeviceID { get; }
        string GetPhoneNumber { get; }
        string GetAppVersion { get; }
        string GetNetworkOperatorName { get; }
        string DeviceName { get; }

    }

}
