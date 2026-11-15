#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Static build entry point for headless Unity / GameCI.
// The scenes come from EditorBuildSettings, so Overworld remains scene 0.
public static class AfkAndroidBuild
{
    public static void Build()
    {
        var root = Directory.GetCurrentDirectory();
        var outputDirectory = GetArgument("-afkOutput", Path.Combine(root, "build", "Android"));
        if (!Path.IsPathRooted(outputDirectory))
            outputDirectory = Path.GetFullPath(Path.Combine(root, outputDirectory));
        // GameCI computes this as <workspace>/<buildsPath>/<target>/<buildName>.apk.
        var outputPath = GetArgument("-customBuildPath", "");
        if (string.IsNullOrEmpty(outputPath))
        {
            var buildName = GetArgument("-customBuildName", "AFKGacha");
            outputPath = Path.Combine(outputDirectory, buildName + ".apk");
            Directory.CreateDirectory(outputDirectory);
        }
        else
        {
            outputPath = Path.GetFullPath(outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        }

        var enabledScenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        if (enabledScenes.Length == 0)
            throw new InvalidOperationException("EditorBuildSettings has no enabled scenes.");

        var options = new BuildPlayerOptions
        {
            scenes = enabledScenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None
        };

        // The development device is arm64-v8a-only. Keep the CI artifact
        // installable there and use the workflow run number for upgrades.
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        var versionCode = GetArgument("-androidVersionCode", "");
        if (int.TryParse(versionCode, out var parsedVersionCode) && parsedVersionCode > 0)
            PlayerSettings.Android.bundleVersionCode = parsedVersionCode;

        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;
        Debug.Log($"AFK Android build: {summary.result}, output={summary.outputPath}, " +
                  $"errors={summary.totalErrors}, warnings={summary.totalWarnings}, size={summary.totalSize}");
        if (summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException($"Android build failed with {summary.totalErrors} error(s).");

        // GameCI recognizes this exact marker. Unity can report non-fatal
        // import issues in totalErrors while still producing a valid player.
        Debug.Log("Build succeeded!");
    }

    private static string GetArgument(string name, string fallback)
    {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length - 1; i++)
            if (string.Equals(args[i], name, StringComparison.Ordinal))
                return args[i + 1];
        return fallback;
    }
}
#endif
