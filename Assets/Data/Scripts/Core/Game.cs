public static class Game
{
    private static bool _isInitialized = false;
    public static bool IsInitialized => _isInitialized;
    public static void InitGame()
    {
        _isInitialized = true;
    }
}