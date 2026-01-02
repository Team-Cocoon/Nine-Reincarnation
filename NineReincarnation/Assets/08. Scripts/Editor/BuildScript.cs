using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;

public class BuildScript
{
    // 젠킨스에서 보낸 "-buildOutput" 뒤의 경로를 읽어오는 함수
    private static string GetBuildPathFromArgs()
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            // Jenkinsfile에서 적은 "-buildOutput" 이라는 키워드를 찾음
            if (args[i] == "-buildOutput" && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        // 인자가 없으면 기본 경로 반환 (테스트용)
        return "Builds/Game.exe";
    }

    public static void BuildWindows()
    {
        string path = GetBuildPathFromArgs();
        CreateDirectory(path);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnableScenes(),
            locationPathName = $"{path}/NineReincarnation.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        ZipBuild(path);
    }

    public static void BuildWebGL()
    {
        string path = GetBuildPathFromArgs();
        CreateDirectory(path);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnableScenes(),
            locationPathName = $"{path}",
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        ZipBuild(path);
    }

    public static void CreateDirectory(string path)
    {
        //디렉터리 존재 여부 확인
        if(Directory.Exists(path))
        {   
            //디렉터리 생성
            Directory.Exists(path);
        }
    }

    private static string[] GetEnableScenes()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    private static void ZipBuild(string buildPath)
    {
        string zipPath = buildPath + ".zip";
        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }
        ZipFile.CreateFromDirectory(buildPath, zipPath);
    }
}
