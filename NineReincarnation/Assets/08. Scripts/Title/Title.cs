using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void StartNextScene()
    {
        UIEventHandler.OnSceneWipeFadeOut(() => { SceneManager.LoadScene("Story3"); });
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
