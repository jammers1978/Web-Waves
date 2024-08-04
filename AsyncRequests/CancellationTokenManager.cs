using WebWaves.Server.Infrastructure;
using WebWaves.Server.AsyncRequests.Settings;

namespace WebWaves.Server.AsyncRequests;
public class CancellationTokenManager
{
    private bool _disposed = false;

    public class CancellationTokenSourceCacheData
    {         
        public CancellationTokenSource CancellationTokenSource { get;set; }
        public string UserId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
    }

    public CancellationTokenSourceCacheData GetCancellationTokenSourceForSession(string asyncRequestID, string userId = "",string sessionId = "")
    {
        if (string.IsNullOrEmpty(asyncRequestID))
        {
            throw new ArgumentException("Invalid asyncRequestID");
        }
        CancellationTokenSourceCacheData cancellationTokenSourceCacheData = new CancellationTokenSourceCacheData();

        var cacheProvider = CacheProvider.Instance;

        cancellationTokenSourceCacheData = cacheProvider.GetFromCache<CancellationTokenSourceCacheData>(asyncRequestID);

        if (cancellationTokenSourceCacheData != null)
        {
            return cancellationTokenSourceCacheData;
        }
        else 
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSourceCacheData = new CancellationTokenSourceCacheData 
            {
                CancellationTokenSource=cancellationTokenSource,
                UserId=userId,
                SessionId=sessionId
            };
            
            cacheProvider.AddToCache(asyncRequestID, cancellationTokenSourceCacheData, AsyncRequestSettings.CancellationTokenCacheLifeTimeSpan);
        }

        return cancellationTokenSourceCacheData;
    }
    
    public void ClearToken(string asyncRequestID)
    {
        if (string.IsNullOrEmpty(asyncRequestID))
        {
            throw new ArgumentException("Invalid session id.");
        }
        
        var cacheProvider = CacheProvider.Instance;

        DisposeToken(asyncRequestID);

        cacheProvider.RemoveFromCache(asyncRequestID);
    }

    private void DisposeToken(string asyncRequestID)
    {
        var cacheProvider = CacheProvider.Instance;

        CancellationTokenSource cancellationTokenSource = cacheProvider.GetFromCache<CancellationTokenSource>(asyncRequestID);
        if (cancellationTokenSource!=null)
        {
            cancellationTokenSource.Dispose();
        }
    }

}
