namespace WebWaves.Server.AsyncRequests.Settings
{
    public class AsyncRequestSettings
    {
        public static int CancellationTokenCacheLifeMinutes { get; set; } = 20;
        public static TimeSpan CancellationTokenCacheLifeTimeSpan { get; set; } = TimeSpan.FromMinutes(CancellationTokenCacheLifeMinutes);
    }
}
