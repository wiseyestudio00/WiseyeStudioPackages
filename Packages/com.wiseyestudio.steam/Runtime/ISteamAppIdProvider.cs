namespace WiseyeStudio.Steam
{
    /// <summary>
    /// Provides the Steam App ID for the current build.
    /// Implemented by the hosting game and injected through <see cref="SteamworksManager.Initialize"/>.
    /// </summary>
    public interface ISteamAppIdProvider
    {
        uint GetAppId();
    }
}
