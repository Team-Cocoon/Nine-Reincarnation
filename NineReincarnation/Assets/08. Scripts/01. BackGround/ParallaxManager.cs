using System.Linq;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Header("--- 원근법 기준이 되는 위치 ---")]
    [SerializeField] private Vector3 _defaultPosition; // 보통 플레이어 위치
    [SerializeField] private float _width;
    [SerializeField] private float _speed;

    [Header("--- 수직(위/아래) 페럴렉스 설정 ---")]
    [SerializeField] private bool _enableVerticalScroll; // 수직 페럴렉스 활성화 여부
    [SerializeField] private float _minY;                // 배경이 내려갈 수 있는 최하한값 (World Y)
    [SerializeField] private float _maxY;                // 배경이 올라갈 수 있는 최상한값 (World Y)

    private Transform[] _transforms; // 자식 배경들의 Transform
    private float[] _offsets;        // 각 배경마다 조절할 길이 비율
    private float _cameraToDefault;  // 카메라와 기준위치 사이 거리
    private Transform _camera;       // 메인 카메라 트랜스폼
    private Vector3 _prevPosition;   // 카메라의 이전 프레임 위치

    void Awake()
    {
        SetInit();
    }

    void Start()
    {
        SetOffset();
        _prevPosition = _camera.position;
    }

    private void LateUpdate()
    {
        MoveBackground();
        _prevPosition = _camera.position;
    }

    // 내부 변수 초기화
    private void SetInit()
    {
        _camera = Camera.main.transform;
        _transforms = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray(); // 본인은 제외
        _offsets = new float[_transforms.Length];
        _cameraToDefault = _camera.position.z - _defaultPosition.z;
    }

    // 배경을 움직이는 함수
    private void MoveBackground()
    {
        // 1. 수평(X축) 페럴렉스 및 무한 루프 처리
        float distanceX = Mathf.Abs(_camera.position.x - _prevPosition.x);
        if (distanceX >= float.Epsilon)
        {
            Vector3 defaultX = (_prevPosition.x < _camera.position.x) ? Vector3.right : Vector3.left;

            for (int i = 0; i < _transforms.Length; i++)
            {
                _transforms[i].position += distanceX * _offsets[i] * defaultX * _speed;
                float toCamera = _transforms[i].position.x - _camera.position.x;

                // 왼쪽으로 완전히 벗어났다면 우측 끝으로 순간이동
                if (toCamera + _width <= -1f)
                {
                    _transforms[i].position += Vector3.right * _width * 2f;
                }
                // 오른쪽으로 완전히 벗어났다면 좌측 끝으로 순간이동
                else if (toCamera - _width >= 1f)
                {
                    _transforms[i].position -= Vector3.right * _width * 2f;
                }
            }
        }

        // 2. 수직(Y축) 페럴렉스 및 상하한 한계치 처리 (활성화 시에만 작동)
        if (_enableVerticalScroll)
        {
            float distanceY = Mathf.Abs(_camera.position.y - _prevPosition.y);
            if (distanceY >= float.Epsilon)
            {
                Vector3 defaultY = (_prevPosition.y < _camera.position.y) ? Vector3.up : Vector3.down;

                for (int i = 0; i < _transforms.Length; i++)
                {
                    // 기존 X축 연산 스타일을 유지하여 Y축 이동량 계산
                    float nextY = _transforms[i].position.y + (distanceY * _offsets[i] * defaultY.y * _speed);
                    
                    // ⚠️ 카메라 밖으로 삐져나가지 않도록 인스펙터에서 설정한 상하한값으로 제한(Clamp)
                    nextY = Mathf.Clamp(nextY, _minY, _maxY);

                    _transforms[i].position = new Vector3(_transforms[i].position.x, nextY, _transforms[i].position.z);
                }
            }
        }
    }

    // 배경이 움직이는 비율 초기화 (Z축 깊이 기준 계산은 Y축에도 그대로 적용됩니다)
    private void SetOffset()
    {
        float defaultToTarget;
        for (int i = 0; i < _transforms.Length; i++)
        {
            defaultToTarget = _transforms[i].position.z - _defaultPosition.z;
            if (defaultToTarget <= float.Epsilon && defaultToTarget >= -float.Epsilon) _offsets[i] = 1f;
            else _offsets[i] = _cameraToDefault / defaultToTarget;
        }
    }
}