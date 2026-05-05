using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;

public class GoogleSheetDownloader : EditorWindow
{
    private string sheetId = "";  // 스프레드시트 ID
    private string saveFolderPath = "Assets/08. Scripts/03. Data"; // 저장할 폴더 경로
    private string saveFileName = "DialogueData.xlsx"; // 저장할 파일 이름

//저장할 키값 (프로젝트마다 겹치지 않게 이름 포함)
    private string PrefKeyId => Application.productName + "_SheetID";
    private string PrefKeyPath => Application.productName + "_SheetPath";
    private string PrefKeyFile => Application.productName + "_SheetFile";

    [MenuItem("Tools/Download Google Sheet")]
    public static void ShowWindow()
    {
        GetWindow<GoogleSheetDownloader>("Sheet Downloader");
    }

    // 창이 켜질 때 로컬 저장소(내 컴퓨터)에서 값을 불러옴
    private void OnEnable()
    {
        sheetId = EditorPrefs.GetString(PrefKeyId, "");
        saveFolderPath = EditorPrefs.GetString(PrefKeyPath, "Assets");
        saveFileName = EditorPrefs.GetString(PrefKeyFile, "GameData.xlsx");
    }

    // 창이 꺼지거나 컴파일될 때 값을 로컬 저장소에 저장함
    private void OnDisable()
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        EditorPrefs.SetString(PrefKeyId, sheetId);
        EditorPrefs.SetString(PrefKeyPath, saveFolderPath);
        EditorPrefs.SetString(PrefKeyFile, saveFileName);
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheet Downloader (Local Save)", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 1. Sheet ID 입력 (변경 시 즉시 저장하지 않고 창 닫을 때 저장)
        EditorGUI.BeginChangeCheck();
        sheetId = EditorGUILayout.TextField("Sheet ID", sheetId);
        
        GUILayout.Space(5);

        // 2. 폴더 경로
        GUILayout.Label("Save Folder:");
        EditorGUILayout.BeginHorizontal();
        {
            saveFolderPath = EditorGUILayout.TextField(saveFolderPath);
            if (GUILayout.Button("Select Folder", GUILayout.Width(100)))
            {
                OpenFolderDialog();
            }
        }
        EditorGUILayout.EndHorizontal();

        // 3. 파일 이름
        saveFileName = EditorGUILayout.TextField("File Name", saveFileName);

        GUILayout.Space(15);

        // 4. 안내 문구 (보안 관련)
        GUIStyle helpStyle = new GUIStyle(EditorStyles.helpBox);
        helpStyle.fontSize = 10;
        GUILayout.Label("Note: ID and Path are saved on your PC only.\nThey will NOT be included in version control (Git).", helpStyle);

        GUILayout.Space(5);

        // 5. 다운로드 버튼
        GUI.enabled = !string.IsNullOrEmpty(sheetId) && !string.IsNullOrEmpty(saveFileName) && !string.IsNullOrEmpty(saveFolderPath);
        
        if (GUILayout.Button("Download & Overwrite", GUILayout.Height(30)))
        {
            SaveSettings(); // 버튼 누를 때도 한 번 저장해줌
            DownloadSheet();
        }
        
        GUI.enabled = true;
    }

    private void OpenFolderDialog()
    {
        string path = EditorUtility.OpenFolderPanel("Select Save Folder", saveFolderPath, "");
        if (!string.IsNullOrEmpty(path))
        {
            if (path.StartsWith(Application.dataPath))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }
            saveFolderPath = path;
            SaveSettings(); // 경로 선택 후 즉시 저장
        }
    }

    private async void DownloadSheet()
    {
        if (!saveFileName.EndsWith(".xlsx")) saveFileName += ".xlsx";

        string finalPath = saveFolderPath;
        if (saveFolderPath.StartsWith("Assets"))
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            finalPath = Path.Combine(projectPath, saveFolderPath);
        }
        finalPath = Path.Combine(finalPath, saveFileName);

        string url = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=xlsx";

        Debug.Log($"Downloading...");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone) await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                SaveFile(finalPath, request.downloadHandler.data);
            }
        }
    }

    private void SaveFile(string path, byte[] data)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
            File.WriteAllBytes(path, data);
            Debug.Log($"<color=green>Saved:</color> {path}");
            
            if (path.Contains(Application.dataPath) || path.Contains("Assets"))
            {
                AssetDatabase.Refresh();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"File Save Error: {e.Message}");
        }
    }
}