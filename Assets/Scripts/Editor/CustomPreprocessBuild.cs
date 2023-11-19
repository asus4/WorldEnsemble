namespace WorldEnsemble
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;

    /// <summary>
    /// Project specific preprocess build script.
    /// </summary>
    public sealed class CustomPreprocessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            // Increment build number
            var platform = report.summary.platform;
            switch (platform)
            {
                case BuildTarget.iOS:
                    PlayerSettings.iOS.buildNumber = IncrementIntStr(PlayerSettings.iOS.buildNumber);
                    break;
                case BuildTarget.Android:
                    PlayerSettings.Android.bundleVersionCode++;
                    break;
                default:
                    Debug.LogError($"App does not support platform: {platform}");
                    break;
            }
        }

        private static string IncrementIntStr(string intStr)
        {
            return (int.Parse(intStr) + 1).ToString();
        }
    }
}
