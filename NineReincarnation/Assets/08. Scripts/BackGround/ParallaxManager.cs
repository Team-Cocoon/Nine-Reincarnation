using System.Linq;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Header("--- 원근법 기준이 되는 위치 ---")]
    [SerializeField] private Vector3 _defaultPosition; //보통 플레이어 위치
    [SerializeField] private float _width;
    [SerializeField] private float _speed;

    private Transform[] _transforms; // 
    private float[] _offsets; //각 배경마다 조절할 길이 비율
    private float _cameraToDefault; //카메라와 기준위치 사이 거리
    private Transform _camera; //메인 카메라 트랜스폼
    private Vector3 _prevPosition; //카메라의 이전 프레임 위치

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

    //내부 변수 초기화
    private void SetInit()
    {
        _camera = Camera.main.transform;
        _transforms = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray(); //본인은 제외
        _offsets = new float[_transforms.Length];
        _cameraToDefault = _camera.position.z - _defaultPosition.z;
    }

    //배경을 움직이는 함수
    private void MoveBackground()
    {
        float distance = Mathf.Abs(_camera.position.x - _prevPosition.x);
        if (distance >= float.Epsilon)
        {
            for (int i = 0; i < _transforms.Length; i++)
            {
                Vector3 defalut;
                //오른쪽 전진
                if (_prevPosition.x < _camera.position.x)
                {
                    defalut = Vector3.right;
                }
                else
                {
                    defalut = Vector3.left;
                }

                _transforms[i].position += distance * _offsets[i] * defalut * _speed;
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
    }

    //배경이 움직이는 비율 초기화
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
