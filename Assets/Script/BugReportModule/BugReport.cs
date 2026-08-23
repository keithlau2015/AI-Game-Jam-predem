using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BugReportSystem
{
    public class BugReport
    {
        public enum SupportItemIndex
        {
            /// <summary>
            /// parameters: API_KEY, API_TOKEN, BOARD_NAME, DEFAULT_LIST_NAME
            /// </summary>
            Trello = 0,
            Notion = 1,
        }

        public struct BugReportParam
        {
            public SupportItemIndex index;
            public bool isActive;
            public Dictionary<string, string> parameters;
        }

        public string title;
        public string summary;
        public string sysInfo;
        public byte[] attachment;
        public DateTime sendTime;

        public Dictionary<SupportItemIndex, BugReportParam> allItems { get; private set; } =
            new Dictionary<SupportItemIndex, BugReportParam>();

        public static List<string> errorReportedRecords { get; private set; } = new List<string>();

        public BugReport(Dictionary<SupportItemIndex, bool> activation)
        {
            BugReportParam trelloParam = new BugReportParam
            {
                parameters = new Dictionary<string, string>
                {
                    { "API_KEY", string.Empty },
                    { "API_TOKEN", string.Empty },
                    { "BOARD_NAME", string.Empty },
                    { "DEFAULT_LIST_NAME", "Player Bug Report" }
                },
                index = SupportItemIndex.Trello,
                isActive = true
            };
            allItems.Add(trelloParam.index, trelloParam);

            if (activation == null)
                return;

            foreach (KeyValuePair<SupportItemIndex, bool> item in activation)
            {
                if (allItems.TryGetValue(item.Key, out BugReportParam bugReportParam))
                {
                    bugReportParam.isActive = item.Value;
                    allItems[item.Key] = bugReportParam;
                }
            }
        }

        public async Task<bool> SendReport()
        {
            bool result = true;
            foreach (var item in allItems.Values)
            {
                if (!item.isActive)
                    continue;

                IReporter reporter = null;
                if (item.index == SupportItemIndex.Trello)
                {
                    if (string.IsNullOrWhiteSpace(item.parameters["API_KEY"])
                        || string.IsNullOrWhiteSpace(item.parameters["API_TOKEN"])
                        || string.IsNullOrWhiteSpace(item.parameters["BOARD_NAME"]))
                    {
                        Debug.LogError("[BugReport] Trello credentials are empty.");
                        result = false;
                        continue;
                    }
                    reporter = new TrelloReporter(item.parameters);
                }
                else
                {
                    Debug.LogWarning($"[BugReport] Reporter {item.index} is not implemented.");
                    result = false;
                    continue;
                }

                result = await reporter.SendReport(this) && result;
            }
            return result;
        }
    }
}
