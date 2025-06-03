using UnityEngine;
using Utilities;

public class Stage : MonoBehaviour
{
    [Header("--- 씬 경로 ---")]
    [SerializeField] private string _scenePath;

    [Header("--- 로드 기준이 되는 거리 ---")]
    [SerializeField] public float _loadRadius;

    public bool IsLoaded { get; private set; } = false;
    public bool IsDirty { get; private set; } = false;


    // Reset the dirty flag after updating
    public void Clean()
    {
        IsDirty = false;
    }


    //스테이지 로드
    public void LoadContent()
    {
        if (!string.IsNullOrEmpty(_scenePath))
        {
            SceneLoader.Instance.LoadSceneAdditivelyByPath(_scenePath);
        }
    }

    //스테이지 언로드
    public void UnloadContent()
    {
        if (!string.IsNullOrEmpty(_scenePath))
        {
            SceneLoader.Instance.UnloadSceneByPath(_scenePath);
        }
    }
}
