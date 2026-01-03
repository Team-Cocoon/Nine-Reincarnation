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

        string[] excludedFolders = new string[]
        {
            "NineReincarnation_BurstDebugInformation_DoNotShip",
            "NineReincarnation_BackUpThisFolder_ButDontShipItWithYourGame"
        };

        Console.WriteLine($"Start Zipping: {zipPath}");

        using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            // buildPath 내부의 모든 파일을 가져옴 (하위 폴더 포함)
            string[] allFiles = Directory.GetFiles(buildPath, "*.*", SearchOption.AllDirectories);

            foreach (string filePath in allFiles)
            {
                // 전체 경로에서 buildPath를 떼어내어 상대 경로를 만듦
                string relativePath = filePath.Substring(buildPath.Length + 1).Replace('\\', '/');

                // 제외할 폴더 이름으로 시작하는지 검사
                bool isExcluded = false;
                foreach (string excluded in excludedFolders)
                {
                    // 폴더 이름으로 시작하면 제외 (폴더 자체이거나 그 하위 파일인 경우)
                    if (relativePath.StartsWith(excluded, StringComparison.OrdinalIgnoreCase))
                    {
                        isExcluded = true;
                        break;
                    }
                }

                // 제외 대상이 아니면 압축 파일에 추가
                archive.CreateEntryFromFile(filePath, relativePath);
            }
        }
    }
}
