using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void StartNextScene()
    {
        AudioManger.Instance.PlaySfx(AudioManger.Sfx.Click);
        UIEventHandler.OnSceneFadeOut(() => { SceneManager.LoadScene("Story3"); });
    }

    public void ExitGame()
    {
        AudioManger.Instance.PlaySfx(AudioManger.Sfx.Click);
        Application.Quit();
    }
}
