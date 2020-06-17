
namespace Management.Errors
{
    public static class GameErrorHandler
    {
        public static void LogError(string className, string description)
        {
            UnityEngine.Debug.LogError("GAME_ERROR !! : " + className + " : " + description);
        }

        public static void LogError<T>(object obj, string description)
        {
            LogError(((T)obj).GetType().Name, description);
        }
    }
}