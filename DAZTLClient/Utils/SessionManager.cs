public class SessionManager
{
    private static SessionManager _instance;
    private static readonly object _lock = new object();

    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }
    public string UserRole { get; private set; }

    public static SessionManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new SessionManager();
                }
                return _instance;
            }
        }
    }

    private SessionManager()
    {
    }

    public void StartSession(string accessToken, string refreshToken, string userRole)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        UserRole = userRole;
    }

    public void EndSession()
    {
        AccessToken = null;
        RefreshToken = null;
        UserRole = null;
    }

    public bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(AccessToken);
    }
}
