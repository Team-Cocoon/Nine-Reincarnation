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
        // Path.GetFullPath를 통해 절대 경로로 변환 후 구분자 통일
        string normalizedBuildPath = Path.GetFullPath(buildPath).Replace('\\', '/');
        // 끝에 슬래시가 있다면 제거 (계산 정확도를 위해)
        normalizedBuildPath = normalizedBuildPath.TrimEnd('/');

        string zipPath = normalizedBuildPath + ".zip";
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
            string[] allFiles = Directory.GetFiles(normalizedBuildPath, "*.*", SearchOption.AllDirectories);

            foreach (string filePath in allFiles)
            {
                // [수정 4] 파일 경로도 정규화
                string normalizedFilePath = filePath.Replace('\\', '/');

                // buildPath 길이 + 1(슬래시) 부터 자름
                string relativePath = normalizedFilePath.Substring(normalizedBuildPath.Length);

                // 만약 맨 앞에 슬래시가 남아있다면 제거 (예: "/Folder" -> "Folder")
                if (relativePath.StartsWith("/"))
                {
                    relativePath = relativePath.Substring(1);
                }

                bool isExcluded = false;
                foreach (string excluded in excludedFolders)
                {
                    // 그냥 StartsWith를 쓰면 "FolderA"가 "FolderA_Backup"까지 제외할 위험이 있음(물론 여기선 이름이 길어서 괜찮지만 안전하게)
                    // 파일 자체가 제외 대상인지(폴더 내부 파일인지) 확인
                    if (relativePath.StartsWith(excluded, StringComparison.OrdinalIgnoreCase))
                    {
                        isExcluded = true;
                        break;
                    }
                }

                if (!isExcluded)
                {
                    archive.CreateEntryFromFile(filePath, relativePath);
                }
            }
        }

        Console.WriteLine($"Zip Created: {zipPath}");
    }
}
