namespace WorldEnsemble
{
    using System.IO;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
#if UNITY_IOS
    using UnityEditor.iOS.Xcode;
#endif // UNITY_IOS

    /// <summary>
    /// Project specific postprocess build script.
    /// </summary>
    public sealed class CustomPostprocessBuild : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
#if UNITY_IOS
            string projectPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
            ModifyPBXProject(projectPath);
            ModifyInfoPlist(report.summary.outputPath);
#endif // UNITY_IOS
        }

#if UNITY_IOS
        private static void ModifyPBXProject(string projectPath)
        {
            // Workaround for Xcode 15 issue:
            // Skip this if you are running earlier versions of Xcode

            // Add -ld_classic to OTHER_LDFLAGS
            // https://forum.unity.com/threads/project-wont-build-using-xode15-release-candidate.1491761/
            // https://developer.apple.com/documentation/xcode-release-notes/xcode-15-release-notes#Linking

            PBXProject pbxProject = new();
            pbxProject.ReadFromFile(projectPath);

            string target = pbxProject.GetUnityFrameworkTargetGuid();

            pbxProject.AddBuildProperty(target, "OTHER_LDFLAGS", "-ld_classic");
            pbxProject.WriteToFile(projectPath);
        }

        private static void ModifyInfoPlist(string projectPath)
        {
            string plistPath = Path.Join(projectPath, "/Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            plist.root.SetString("ITSAppUsesNonExemptEncryption", "false");

            plist.WriteToFile(plistPath);
        }
#endif // UNITY_IOS
    }
}
