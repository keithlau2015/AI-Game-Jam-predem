#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DevTools
{
    public class EncryptFileEditor : EditorWindow
    {
        private string filePath = "";
        private string csvRawData;

        [MenuItem("NPI/DevTools/File Editor/Encrypt Target File")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(EncryptFileEditor), false, "Encrypt File Eidtor");
        }

        private Vector2 scrollPosition = Vector2.zero;
        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
            GUILayout.Space(10);
            GUILayout.Label("Target CSV");
            GUILayout.Label(filePath);

            GUILayout.Space(10);
            if (GUILayout.Button("Select CSV"))
            {
                filePath = $"{EditorUtility.OpenFilePanel("Select File", "", "")}";
                csvRawData = FileManager.LoadCSV(filePath);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Encrpyt"))
            {
                float progress = 0;
                EditorUtility.DisplayProgressBar("Writing CSV Progress Bar", "Storing CSV...", progress);
                FileManager.SaveCSV(Path.GetFileName(filePath).Split('.')[0], csvRawData).GetAwaiter();
                progress = 100;
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Close"))
            {
                this.Close();
            }

            GUILayout.Space(10);
            GUILayout.EndScrollView();
        }
    }

    public class EncrpytFolderEditor : EditorWindow
    {
        private string folderPath = "";
        private List<string> fnameList = new List<string>();
        private List<string> csvDataList = new List<string>();
        [MenuItem("NPI/DevTools/File Editor/Encrypt Folder Files")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(EncrpytFolderEditor), false, "Encrypt Folder Files Eidtor");
        }

        private Vector2 scrollPosition = Vector2.zero;
        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
            GUILayout.Space(10);
            GUILayout.Label("Target Folder");
            GUILayout.Label(folderPath);

            GUILayout.Space(10);
            if (GUILayout.Button("Select Folder"))
            {
                folderPath = $"{EditorUtility.OpenFolderPanel("Select Folder", "", "")}";
                foreach(string fname in FileManager.GetAllFileInDirectory(folderPath, new string[] { ".meta" }))
                {
                    string csvData = FileManager.LoadCSV($"{folderPath}/{fname}");
                    fnameList.Add(fname);
                    csvDataList.Add(csvData);
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Encrpyt"))
            {
                float progress = 0;
                EditorUtility.DisplayProgressBar("Writing CSV Progress Bar", "Storing CSV...", progress);
                for (int i = 0; i < csvDataList.Count; i++)
                {
                    string csvData = csvDataList[i];
                    string fname = fnameList[i];
                    FileManager.SaveCSV(Path.GetFileName(fname).Split('.')[0], csvData).GetAwaiter();
                    progress = i/csvDataList.Count;
                }
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Close"))
            {
                this.Close();
            }

            GUILayout.Space(10);
            GUILayout.EndScrollView();
        }
    }

    public class GameFileManagment
    {
        [MenuItem("NPI/DevTools/Reset Game Data")]
        public static void ResetGameData()
        {
            float progress = 0;
            EditorUtility.DisplayProgressBar("Reset Game Data", "Deleting Game Data...", progress);
            string[] allFiles = Directory.GetFiles($"{Application.persistentDataPath}/Player/");
            for (int i = 0; i < allFiles.Length; i++)
            {
                File.Delete(allFiles[i]);
                progress = i / allFiles.Length;
                EditorUtility.DisplayProgressBar("Reset Game Data", "Deleting Game Data...", progress);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}
#endif