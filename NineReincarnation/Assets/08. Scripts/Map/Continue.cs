using Player.Controller;
using UnityEngine;

public class Continue : MonoBehaviour, ICollidable
{
    public void Enter(GameObject go = null)
    {
        PlayerController player = go.GetComponent<PlayerController>();
        if (player != null)
        {
            //GameEventHandler.GameClearExcuted_Invoke();
            SceneEventHandler.SceneStateChangedAndLoadScenes_Invoke(SceneDataManager.Instance.StageCoreScene, SceneDataManager.Instance.StageCoreScene, SceneDataManager.Instance.GetStageSubScene(1));
            //UIEventHandler.OnSceneFadeOut(() => { SceneManager.LoadScene("Continue"); });
        }
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
