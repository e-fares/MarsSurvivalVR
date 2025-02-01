using UnityEngine;
using TMPro;

public class ConsoleLogger : MonoBehaviour
{
    public TextMeshProUGUI consoleText;
    private string logContent = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logContent += logString + "\n";
        if (logContent.Length > 3000) // Limite la longueur pour éviter trop de texte
        {
            logContent = logContent.Substring(logContent.Length - 3000);
        }
        consoleText.text = logContent;
    }
}
