using Player.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Continue : MonoBehaviour, ICollidable
{
    public void Enter(GameObject go = null)
    {
        PlayerController player = go.GetComponent<PlayerController>();
        if (player != null)
        {
            UIEventHandler.OnSceneFadeOut(() => { SceneManager.LoadScene("Continue"); });
        }
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
