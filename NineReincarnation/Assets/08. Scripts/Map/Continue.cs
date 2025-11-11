using UnityEngine;

public class Continue : MonoBehaviour
{
    public void Next()
    {
        SceneEventHandler.SceneStateChangedAndLoadScenes_Invoke(SceneDataManager.Instance.StoryCoreScene, SceneDataManager.Instance.StoryCoreScene, SceneDataManager.Instance.GetStorySubScene(1));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneEventHandler.SceneStateChangedAndLoadScenes_Invoke(SceneDataManager.Instance.StageCoreScene, SceneDataManager.Instance.StageCoreScene, SceneDataManager.Instance.GetStageSubScene(1));
            Destroy(gameObject);
        }
    }
}
