using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity; // LifetimeScope를 사용하기 위해 추가

public class MapSkipUI : MonoBehaviour
{
    [SerializeField] private Button _skipButton;
    
    private void Start()
    {
        if (_skipButton != null)
        {
            _skipButton.onClick.AddListener(OnSkipButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (_skipButton != null)
        {
            _skipButton.onClick.RemoveListener(OnSkipButtonClicked);
        }
    }

    private void OnSkipButtonClicked()
    {
        // 중복 클릭 방지
        _skipButton.interactable = false;
        
        Debug.Log("[UI] 스킵 버튼 클릭 - 다음 맵으로 이동을 요청합니다.");
        
        //  DI 주입을 받지 않고, 현재 살아있는 StageCoreScope를 찾아 컨테이너에 접근.
        // 자주 쓰지 말자 제발... 어쩔 수 없이 쓴거야...
        var stageCoreScope = LifetimeScope.Find<StageCoreScope>();
        
        if (stageCoreScope != null)
        {
            // 컨테이너 안에서 StageManager를 직접 꺼내와서 실행
            var stageManager = stageCoreScope.Container.Resolve<StageManager>();
            stageManager.GoToNextMap().Forget();
        }
        else
        {
            Debug.LogError("[MapSkipUI] StageCoreScope를 찾을 수 없습니다! 매니저 호출에 실패했습니다.");
            _skipButton.interactable = true; // 에러가 났으니 다시 누를 수 있게 복구
        }
    }
}