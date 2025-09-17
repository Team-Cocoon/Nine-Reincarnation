using UnityEngine;

public class Continue : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneEventHandler.SceneStateChangedAndLoadScenes_Invoke(SceneDataManager.Instance.StageCoreScene, SceneDataManager.Instance.StageCoreScene, SceneDataManager.Instance.GetStageSubScene(1));
            Destroy(gameObject);
        }
    }
}
