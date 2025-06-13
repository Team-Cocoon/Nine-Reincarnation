using UnityEngine;
using Utilities;

public class Stage : MonoBehaviour
{
    [Header("--- 씬 경로 ---")]
    [SerializeField] private string _scenePath;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
