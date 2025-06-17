using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void StartNextScene()
    {
        UIEventHandler.OnSceneFadeOut(() => { SceneManager.LoadScene("Story3"); });
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
