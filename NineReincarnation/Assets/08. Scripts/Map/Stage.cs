using UnityEngine;
using Utilities;

public class Stage : MonoBehaviour
{
    [Header("--- 씬 경로 ---")]
    [SerializeField] private string _scenePath;

    [Header("--- 로드 기준이 되는 거리 ---")]
    [SerializeField] public float _loadRadius;

    //스테이지 로드
    private void LoadContent()
    {
        if (!string.IsNullOrEmpty(_scenePath))
        {
            SceneLoader.Instance.LoadSceneAdditivelyByPath(_scenePath);
        }
    }

    //스테이지 언로드
    private void UnloadContent()
    {
        if (!string.IsNullOrEmpty(_scenePath))
        {
            SceneLoader.Instance.UnloadSceneByPath(_scenePath);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("응애");
        if(collision.CompareTag("Player"))
        {
            LoadContent();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnloadContent();
        }
    }
}
