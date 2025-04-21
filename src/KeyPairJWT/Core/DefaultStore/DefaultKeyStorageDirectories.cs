using System.Runtime.InteropServices;

namespace KeyPairJWT.Core.DefaultStore;

internal sealed class DefaultKeyStorageDirectories
{
    private static readonly Lazy<DirectoryInfo> _defaultDirectoryLazy = new Lazy<DirectoryInfo>(new Func<DirectoryInfo>(GetKeyStorageDirectoryImpl));
    private const string DataProtectionKeysFolderName = "DataProtection-Keys";

    private DefaultKeyStorageDirectories()
    {
    }

    public static DefaultKeyStorageDirectories Instance { get; } = new DefaultKeyStorageDirectories();

    public DirectoryInfo GetKeyStorageDirectory() => _defaultDirectoryLazy.Value;

    private static DirectoryInfo GetKeyStorageDirectoryImpl()
    {
        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var localAppData = Environment.GetEnvironmentVariable("LOCALAPPDATA");
        var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
        var home = Environment.GetEnvironmentVariable("HOME");
        DirectoryInfo directoryInfo;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !string.IsNullOrEmpty(folderPath))
            directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(folderPath);
        else if (localAppData != null)
            directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(localAppData);
        else if (userProfile != null)
            directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(Path.Combine(userProfile, "AppData", "Local"));
        else if (home != null)
        {
            directoryInfo = new DirectoryInfo(Path.Combine(home, ".aspnet", "DataProtection-Keys"));
        }
        else
        {
            if (string.IsNullOrEmpty(folderPath))
                return null;
            directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(folderPath);
        }
        try
        {
            directoryInfo.Create();
            return directoryInfo;
        }
        catch
        {
            return null;
        }
    }

    public DirectoryInfo GetKeyStorageDirectoryForAzureWebSites()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")))
        {
            string environmentVariable = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(environmentVariable))
                return GetKeyStorageDirectoryFromBaseAppDataPath(environmentVariable);
        }
        return null;
    }

    private static DirectoryInfo GetKeyStorageDirectoryFromBaseAppDataPath(
        string basePath)
    {
        return new DirectoryInfo(Path.Combine(basePath, "ASP.NET", "DataProtection-Keys"));
    }
}
