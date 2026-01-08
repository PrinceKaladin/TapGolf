using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;

public class BuildScript
{
    private static string[] scenes = {
        "Assets/Scenes/MainMenuScene.unity",
        "Assets/Scenes/GamePlayScene.unity"
    };

    private static void ConfigureKeystore()
    {
        string keystoreBase64 = "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";
        string keystorePass ="iefuewio";
        string keyAlias = "tapgolffys";
        string keyPass = "iefuewio";

        if (!string.IsNullOrEmpty(keystoreBase64))
        {
            string tempKeystorePath = Path.Combine(Path.GetTempPath(), "TempKeystore.jks");
            File.WriteAllBytes(tempKeystorePath, Convert.FromBase64String(keystoreBase64));

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = tempKeystorePath;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasName = keyAlias;
            PlayerSettings.Android.keyaliasPass = keyPass;

            Debug.Log("Android signing configured from Base64 keystore.");
        }
        else
        {
            Debug.LogWarning("Keystore Base64 not set. Build will be unsigned.");
            PlayerSettings.Android.useCustomKeystore = false;
        }
    }

    public static void PerformBuildAAB()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        ConfigureKeystore();

        EditorUserBuildSettings.buildAppBundle = true;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "TapGolf.aab",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log("✅ AAB build succeeded!");
        else
            Debug.LogError("❌ AAB build failed!");
    }

    public static void PerformBuildAPK()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        ConfigureKeystore();

        EditorUserBuildSettings.buildAppBundle = false;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "TapGolf.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log("✅ APK build succeeded!");
        else
            Debug.LogError("❌ APK build failed!");
    }
}