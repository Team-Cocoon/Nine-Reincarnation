using UnityEngine;
using Utilities;

public class StageManager : MonoBehaviour
{
    [Header("--- 스테이지 모음 ---")]
    [SerializeField] private Stage[] _stages;

    private void Start()
    {
        UIEventHandler.OnSceneWipeFadeIn(() => { InputManager.Instance.Action.Player.IsBusy = false; });

        for (int i = 0; i < _stages.Length; ++i)
        {
            string scenePath = _stages[i].ScenePath;
            if (!string.IsNullOrEmpty(scenePath))
            {
                //SceneLoader.Instance.LoadSceneAdditivelyByPath(scenePath);
            }
        }
    }
}
