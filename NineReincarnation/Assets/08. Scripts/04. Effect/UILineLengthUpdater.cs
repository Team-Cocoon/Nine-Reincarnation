using UnityEngine;
using UnityEngine.UI;

// LineLengthUpdater의 UI(Canvas) 버전.
// LineRenderer 대신 RectTransform의 가로 폭을 "선 길이"로 사용해 셰이더의 _LineLength에 전달한다.
// (홍연 실에 쓰는 파형 셰이더를 Canvas 타깃으로 만든 머티리얼을 UI Image에 물렸을 때 사용)
[RequireComponent(typeof(RectTransform))]
public class UILineLengthUpdater : MonoBehaviour
{
    [SerializeField] private Graphic _graphic;          // Material을 사용하는 UI(Image 등). 비우면 자동으로 찾음
    [SerializeField] private float _lengthScale = 1f;   // 파형 밀도 튜닝용 배율(UI 단위 ≠ 월드 단위라 조정 필요)
    [SerializeField] private bool _updateEveryFrame = true; // 크기가 고정이면 false로 두고 OnEnable 때만 반영

    private RectTransform _rect;
    private Material _materialInstance;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        if (_graphic == null) _graphic = GetComponent<Graphic>();

        // 공유 머티리얼(에셋) 오염을 막기 위해 인스턴스를 만들어 이 UI에만 적용한다.
        if (_graphic != null && _graphic.material != null)
        {
            _materialInstance = new Material(_graphic.material);
            _graphic.material = _materialInstance;
        }
    }

    private void OnEnable() => Apply();

    private void Update()
    {
        if (_updateEveryFrame) Apply();
    }

    private void Apply()
    {
        if (_materialInstance == null) return;

        float length = _rect.rect.width * _lengthScale;
        _materialInstance.SetFloat("_LineLength", length);
    }

    private void OnDestroy()
    {
        if (_materialInstance != null) Destroy(_materialInstance);
    }
}
