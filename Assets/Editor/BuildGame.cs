// Unity Editor build script — triggered via command line with -executeMethod
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class BuildGame
{
    public static void BuildWindows64()
    {
        string outputPath = "e:/codes/Hero_Game/Build/HeroReborn.exe";

        string[] scenes = new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity"
        };

        BuildPlayerOptions opts = new BuildPlayerOptions
        {
            scenes            = scenes,
            locationPathName  = outputPath,
            target            = BuildTarget.StandaloneWindows64,
            options           = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(opts);
        Debug.Log($"[Build] Done → {outputPath}");
    }
}
#endif
