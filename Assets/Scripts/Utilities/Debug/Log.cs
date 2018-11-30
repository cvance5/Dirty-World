namespace Utilities.Debug
{
    public class Log
    {
        private readonly string _preface;

        public Log(string preface) => _preface = $"[{preface}] ";

        public void Info(string message, string colorName = "black") => PrintInfo($"<color={colorName}>" + _preface + message + "</color>");

        public void Warning(string message) => PrintWarning(_preface + message);

        public void Error(string message) => PrintError(_preface + message);

        public void ErrorIfNull(object obj, string message)
        {
            if (obj == null) PrintError(_preface + message);
        }

        private static void PrintError(string message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(message);
#endif
        }

        private static void PrintWarning(string message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(message);
#endif
        }

        private static void PrintInfo(string message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
#endif
        }
    }
}