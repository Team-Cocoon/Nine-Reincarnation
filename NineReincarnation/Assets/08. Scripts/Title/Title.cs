using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void StartNextScene()
    {
        UIEventHandler.OnSceneFadeOut(() => { SceneManager.LoadScene("Story3"); });
        AudioManger.Instance.PlaySfx(AudioManger.Sfx.Click);
    }

    public void ExitGame()
    {
        Application.Quit();
        AudioManger.Instance.PlaySfx(AudioManger.Sfx.Click);
    }
}
