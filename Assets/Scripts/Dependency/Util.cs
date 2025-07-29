//Dependency management
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dependency {
    public class Util
    {

        private static string dependencyPath = "Assets/Scripts/Dependency/Dependency.properties";
        private static string basePath = "Assets/Scripts/";

        public static List<string> ReadProperties(string filePath)
        {
            var properties = new List<string>();

            if (!File.Exists(filePath))
                return properties;

            foreach (var line in File.ReadAllLines(filePath))
            {
                string trimmedLine = line.Trim();

                // Skip comments and empty lines
                if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#") || trimmedLine.StartsWith("!"))
                    continue;

                properties.Add(trimmedLine);
            }

            return properties;
        }

        public static void ConsolidateDependencyProperties()
        {
            // List to store all unique properties
            var consolidatedProperties = new HashSet<string>();

            // Get all directories in the base path
            string[] moduleDirectories = Directory.GetDirectories(basePath);

            // Scan each module directory for dependency.properties
            foreach (string moduleDir in moduleDirectories)
            {
                string propertiesPath = Path.Combine(moduleDir, "dependency.properties");
                if (File.Exists(propertiesPath))
                {
                    // Read properties from each module's dependency file
                    var moduleProperties = ReadProperties(propertiesPath);

                    // Add all properties to the HashSet (automatically handles duplicates)
                    foreach (var prop in moduleProperties)
                    {
                        consolidatedProperties.Add(prop);
                    }
                }
            }

            // Write consolidated properties to output file
            using (StreamWriter writer = new StreamWriter(dependencyPath))
            {
                foreach (var prop in consolidatedProperties)
                {
                    writer.WriteLine(prop);
                }
            }
        }

        public static void RemoveAllNonDependencyModules()
        {
            List<string> properties = ReadProperties(dependencyPath);
            foreach (var property in properties)
            {
                if (!property.Contains("Dependency"))
                {
                    File.Delete(basePath + property);
                }
            }
        }


#if UNITY_EDITOR
        [MenuItem("NPS/Dependency/Remove Non-Dependency Modules")]
        public static void RemoveNonDependencyModulesMenuItem()
        {
            if (EditorUtility.DisplayDialog("Remove Non-Dependency Modules",
                "Are you sure you want to remove all non-dependency modules? This action cannot be undone.",
                "Yes", "No"))
            {
                try
                {
                    RemoveAllNonDependencyModules();
                    AssetDatabase.Refresh();
                    Debug.Log("Non-dependency modules have been removed.");
                }
                catch (Exception ex)
                {
                    Debug.LogError("An error occurred while removing modules. Check the console for details.");
                    EditorUtility.DisplayDialog("Error",
                        "An error occurred while removing modules. Check the console for details.",
                        "OK");
                }
            }
        }
#endif
    }
}
