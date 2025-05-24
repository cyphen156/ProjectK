using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

using Debug = UnityEngine.Debug;
public static class Logger
{
    private static readonly string logFilePath = Path.Combine(Application.persistentDataPath, "log.txt");

    [Conditional("DEV_VER")]
    public static void Info(string message)
    {
        Debug.LogFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-mm-dd HH:mmLss.fff"), message);
    }

    [Conditional("DEV_VER")]
    public static void Warning(string message)
    {
        Debug.LogWarningFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-mm-dd HH:mmLss.fff"), message);
    }
    public static void Error(string message)
    {
        Debug.LogErrorFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-mm-dd HH:mmLss.fff"), message);
    }


    public static void Log(string message)
    {
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string fullMessage = $"[{timeStamp}] {message}";

        // 콘솔 출력
        Debug.Log(logFilePath);
        Debug.LogError(fullMessage);

        // 파일 출력
        try
        {
            File.AppendAllText(logFilePath, fullMessage + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Logger] 파일 로그 쓰기 실패: {ex.Message}");
        }
    }

}
