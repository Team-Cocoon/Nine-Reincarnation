using Player.Controller;
using UnityEngine;

public class Continue : MonoBehaviour, ICollidable
{
    public void Enter(GameObject go = null)
    {
        PlayerController player = go.GetComponent<PlayerController>();
        if (player != null)
        {
            GameEventHandler.GameClearExcuted_Invoke();
            //UIEventHandler.OnSceneFadeOut(() => { SceneManager.LoadScene("Continue"); });
        }
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
