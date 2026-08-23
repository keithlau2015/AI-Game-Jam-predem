using UnityEngine;

/// <summary>
/// External credentials for bug reporters. Place at Resources/BugReportConfig.
/// Never commit real production secrets; use local overrides / CI secrets.
/// </summary>
[CreateAssetMenu(fileName = "BugReportConfig", menuName = "Null Template/Bug Report Config")]
public class BugReportConfig : ScriptableObject
{
    [Header("Trello")]
    public string trelloApiKey = "";
    public string trelloApiToken = "";
    public string trelloBoardId = "";
    public string trelloDefaultListName = "Player Bug Report";

    public bool HasTrelloCredentials
    {
        get
        {
            return !string.IsNullOrWhiteSpace(trelloApiKey)
                && !string.IsNullOrWhiteSpace(trelloApiToken)
                && !string.IsNullOrWhiteSpace(trelloBoardId);
        }
    }

    public static BugReportConfig Load()
    {
        return Resources.Load<BugReportConfig>("BugReportConfig");
    }
}
