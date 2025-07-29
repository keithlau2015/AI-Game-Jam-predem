using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetsProcessPipline : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(
         string[] importedAssets,
         string[] deletedAssets,
         string[] movedAssets,
         string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            //Debug.Log("Reimported Asset: " + str);
            string[] splitStr = str.Split('/', '.');
            string folder = splitStr[splitStr.Length - 3];
            string fileName = splitStr[splitStr.Length - 2];
            string extension = splitStr[splitStr.Length - 1];
            //Debug.Log("File name: " + fileName);
            //Debug.Log("File type: " + extension);
            //Debug.Log("Folder: " + folder);
            if(extension.Equals("csv"))
                FileManager.SaveCSV(fileName, FileManager.LoadCSV(str)).GetAwaiter();
        }

        /*
        foreach (string str in deletedAssets)
            Debug.Log("Deleted Asset: " + str);

        for (int i = 0; i < movedAssets.Length; i++)
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        */
    }
}
