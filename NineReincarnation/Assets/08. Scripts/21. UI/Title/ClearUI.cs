using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ClearUI : MonoBehaviour
{
    [Inject] private SceneDataManager _sceneDataManager;
    [Inject] private SceneLoader _sceneLoader;
    [Inject] private SceneTransitionManager _transitionManager;
    [Header("---- Button ----")]
    [SerializeField] private Button _titleButton;

    private void Start()
    {
        _titleButton.onClick.AddListener(GameEvent_Title);
    }

    private void OnDestroy()
    {
        _titleButton.onClick.RemoveAllListeners();
    }

    private void GameEvent_Title()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);

        if (!_sceneLoader.LoadedScenes.Contains(_sceneDataManager.TitleScene))
        {
            // 2. 타이틀 씬만 로드하도록 매니저에게 요청
            List<string> scenesToLoad = new List<string> { _sceneDataManager.TitleScene };
            _transitionManager.TransitionToScenes(scenesToLoad).Forget();

            // 3. 기존 이벤트 호출 (이벤트 구독자들이 필요한 처리를 할 수 있도록 유지)
            GameEventHandler.TitleExcuted_Invoke();
        }
    }
}

