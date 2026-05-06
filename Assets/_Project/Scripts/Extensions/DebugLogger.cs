using UnityEngine;

namespace Extensions
{
    public static class DebugLogger
    {
        public static void Log(string msg)
        {
    #if DEBUG || UNITY_EDITOR
            Debug.Log(msg);
    #endif
        }
        public static void Log(object msg)
        {
    #if DEBUG || UNITY_EDITOR
            Debug.Log(msg.ToString());
    #endif
        }
        public static void LogFormat(string msg, params object[] args)
        {
    #if DEBUG || UNITY_EDITOR
            Debug.LogFormat(msg, args);
    #endif
        }
        public static void LogWarning(string msg)
        {
    #if DEBUG || UNITY_EDITOR
            Debug.LogWarning(msg);
    #endif
        }
        public static void LogWarningFormat(string msg, params object[] args)
        {
    #if DEBUG || UNITY_EDITOR
            Debug.LogWarningFormat(msg, args);
    #endif
        }
        public static void LogError(string msg)
        {
    #if DEBUG || UNITY_EDITOR
            Debug.LogError(msg);
    #endif
        }
        public static void LogErrorFormat(string msg, params object[] args)
        {
    #if DEBUG || UNITY_EDITOR
            Debug.LogErrorFormat(msg, args);
    #endif
        }
    }
}