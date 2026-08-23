#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
namespace DevTools
{
    [InitializeOnLoad]
    public class VersionIncrementor
    {
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            Debug.Log("Build version: " + PlayerSettings.bundleVersion + " (" + PlayerSettings.Android.bundleVersionCode + ")");
            //IncreaseBuild();

            if (buildTarget == BuildTarget.iOS)
            {
                PlayerSettings.iOS.buildNumber = GetBuildNum().ToString();
                UnityEngine.Debug.Log("Finished with bundleversioncode:" + PlayerSettings.iOS.buildNumber + "and version" + PlayerSettings.bundleVersion);

            }
            else if (buildTarget == BuildTarget.Android)
            {
                PlayerSettings.Android.bundleVersionCode = GetBuildNum();
                UnityEngine.Debug.Log("Finished with bundleversioncode:" + PlayerSettings.Android.bundleVersionCode + "and version" + PlayerSettings.bundleVersion);
            }
            // It's important that you do not chane your project settings during a build in the cloud.


            // commit the settings to git only if you are in cloud build. If you save locally, we save your project settings so that you can commit them.
#if CLOUD_BUILD
        AssetDatabase.SaveAssets(); // should only be project version
#endif
        }

        private static void IncrementVersion(int majorIncr, int minorIncr, int revision, int package)
        {
            string[] splits = PlayerSettings.bundleVersion.Split(' ');
            string[] lines;
            if (splits.Length == 1)
                lines = splits[0].Split('.');
            else
                lines = splits[1].Split('.');
            try
            {
                int MajorVersion = int.Parse(lines[0]) + majorIncr;
                int MinorVersion = int.Parse(lines[1]) + minorIncr;
                int Revision = int.Parse(lines[2]) + revision;
                int Package = int.Parse(lines[3]) + package;

                if (splits.Length == 0)
                    PlayerSettings.bundleVersion = MajorVersion.ToString("0") + "." +
                                            MinorVersion.ToString("0") + "." +
                                            Revision.ToString("0") + "." +
                                            Package.ToString("0");
                else
                    PlayerSettings.bundleVersion = splits[0] + " " + MajorVersion.ToString("0") + "." +
                                            MinorVersion.ToString("0") + "." +
                                            Revision.ToString("0") + "." +
                                            Package.ToString("0");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                UnityEngine.Debug.LogError("AutoIncrementBuildVersion script failed. Make sure your current bundle version is in the format X.X.X (e.g. 1.0.0) and not X.X (1.0) or X (1).");
            }
        }

        private static void SwitchDevelopmentStage(string stageName)
        {
            string[] splits = PlayerSettings.bundleVersion.Split(' ');
            string[] lines;
            if (splits.Length == 1)
                lines = splits[0].Split('.');
            else
                lines = splits[1].Split('.');

            try
            {
                int MajorVersion = int.Parse(lines[0]);
                int MinorVersion = int.Parse(lines[1]);
                int Revision = int.Parse(lines[2]);
                int Package = int.Parse(lines[3]);

                if (string.IsNullOrEmpty(stageName))
                    PlayerSettings.bundleVersion = MajorVersion.ToString("0") + "." +
                                            MinorVersion.ToString("0") + "." +
                                            Revision.ToString("0") + "." +
                                            Package.ToString("0");
                else
                    PlayerSettings.bundleVersion = stageName + " " + MajorVersion.ToString("0") + "." +
                                            MinorVersion.ToString("0") + "." +
                                            Revision.ToString("0") + "." +
                                            Package.ToString("0");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                UnityEngine.Debug.LogError("AutoIncrementBuildVersion script failed. Make sure your current bundle version is in the format X.X.X (e.g. 1.0.0) and not X.X (1.0) or X (1).");
            }
        }

        [MenuItem("NPI/QA/Increase Major Version")]
        public static void IncreaseMajor()
        {
            //Reset Minor & Build version
            string[] splits = PlayerSettings.bundleVersion.Split(' ');
            string[] lines;
            if (splits.Length == 1)
                lines = splits[0].Split('.');
            else
                lines = splits[1].Split('.');

            int MajorVersion = int.Parse(lines[0]);
            int MinorVersion = 0;
            int Revision = 0;
            int Package = 0;

            if (splits.Length == 0)
                PlayerSettings.bundleVersion = PlayerSettings.bundleVersion = MajorVersion.ToString("0") + "." +
                                            MinorVersion.ToString("0") + "." +
                                            Revision.ToString("0") + "." +
                                            Package.ToString("0");
            else
                PlayerSettings.bundleVersion = splits[0] + " " + MajorVersion.ToString("0") + "." +
                                            MinorVersion.ToString("0") + "." +
                                            Revision.ToString("0") + "." +
                                            Package.ToString("0");

            IncrementVersion(1, 0, 0, 0);
        }

        [MenuItem("NPI/QA/Increase Minor Version")]
        public static void IncreaseMinor()
        {
            IncrementVersion(0, 1, 0, 0);
        }

        [MenuItem("NPI/QA/Increase Revision Version")]
        public static void IncreaseRevision()
        {
            IncrementVersion(0, 0, 1, 0);
        }

        [MenuItem("NPI/QA/Increase Package Version")]
        public static void IncreasePackage()
        {
            IncrementVersion(0, 0, 0, 1);
        }

        [MenuItem("NPI/QA/Stage/Dev")]
        public static void Dev()
        {
            SwitchDevelopmentStage("Dev");
        }

        [MenuItem("NPI/QA/Stage/Prototype")]
        public static void Prototype()
        {
            SwitchDevelopmentStage("Prototype");
        }

        [MenuItem("NPI/QA/Stage/Alpha")]
        public static void Alpha()
        {
            SwitchDevelopmentStage("Alpha");
        }

        [MenuItem("NPI/QA/Stage/Beta")]
        public static void Beta()
        {
            SwitchDevelopmentStage("Beta");
        }

        [MenuItem("NPI/QA/Stage/Production")]
        public static void Production()
        {
            SwitchDevelopmentStage(string.Empty);
        }

        /*
        public static void IncreaseBuild()
        {
            IncrementVersion(0, 0, 1);
        }
        */

        private static int GetBuildNum()
        {
            string[] splits = PlayerSettings.bundleVersion.Split(' ');
            string[] lines;
            if (splits.Length == 1)
                lines = splits[0].Split('.');
            else
                lines = splits[1].Split('.');

            try
            {
                int MajorVersion = int.Parse(lines[0]);
                int MinorVersion = int.Parse(lines[1]);
                int Revision = int.Parse(lines[2]);
                int Package = int.Parse(lines[3]);

                PlayerSettings.bundleVersion = MajorVersion.ToString("0") + "." +
                                            MinorVersion.ToString("0") + "." +
                                            Revision.ToString("0") + "." +
                                            Package.ToString("0");

                return MajorVersion + MinorVersion + Revision + Package;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                UnityEngine.Debug.LogError("AutoIncrementBuildVersion script failed. Make sure your current bundle version is in the format X.X.X (e.g. 1.0.0) and not X.X (1.0) or X (1).");
                return 0;
            }
        }
    }
}
#endif