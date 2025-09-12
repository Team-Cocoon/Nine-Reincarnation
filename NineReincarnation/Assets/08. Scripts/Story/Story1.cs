using System;
using System.Collections;
using EventHandler;
using Febucci.UI.Core.Parsing;
using Manager;
using Player.Controller;
using UnityEngine;

public class Story1 : Story
{
    [Header("안나")]
    private PlayerController _annaController => InputManager.Instance.CurPlayer;
    [SerializeField] private GameObject _npcAnna;
    
    [Header("Event 4-2")]
    [SerializeField] private GameObject _dialogue4_2;
    private bool _isTextShowed = false;

    [Header("Event 4-3")]
    private bool _isCheck = false;
    private bool _isMove = true;
    [SerializeField] private Transform _targetPosition;
    [SerializeField] private NPCAnna _npcAnnaAnim;
    [SerializeField] private float _moveSpeed;

    [Header("Event 4-4")]
    [SerializeField] private float _cameraOriginSize = 4f;
    private Vector3 _cameraOriginPos;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _zoomInSize = 3.5f;
    [SerializeField] private float _zoomDuration = 2f;
    [SerializeField] private GameObject _dialogue4_4;

    [Header("Event 4-5")]
    [SerializeField] private GameObject _dialogue4_5;

    [Header("Event 4-7")]
    [SerializeField] private GameObject _wall;

    private float _stageCameraSize;

    private void Awake()
    {
        _stageCameraSize = CameraManager.Instance.CinemachineCamera.Lens.OrthographicSize;
        //CameraManager.Instance.CinemachineCamera.Lens.OrthographicSize = _cameraOriginSize;
        //_camera.orthographicSize = _cameraOriginSize;
    }

    void Start()
    {
        StartCoroutine(StartSetting(StartStory));
    }

    public override void PlayStory(string eventFunc)
    {
        base.PlayStory(eventFunc);
        switch (eventFunc)
        {
            case "Event4-1":
                StartCoroutine(Event4_1());
                break;
            case "Event4-2":
                StartCoroutine(Event4_2());
                break;
            case "Event4-3":
                AnnaActive(true);
                StartCoroutine(Event4_3());
                break;
            case "Event4-4":
                StartCoroutine(Event4_4());
                break;
            case "Event4-5":
                StartCoroutine(Event4_5());
                break;
            case "Event4-6":
                //CameraEventHandler.ZoomToTarget(_camera, CameraManager.Instance.CinemachineCamera, _cameraOriginPos, _cameraOrginSize, _zoomDuration, StartStory);
                StartStory();
                break;
            case "Event4-7":
                StartCoroutine(Event4_7());
                // 안나로 바꿀 때 다시 넣어야됨
                //CameraManager.Instance.ChangeTarget(_annaController.transform);
                break;
        }
    }

    /* NPCAnna, Anna 스왑 */
    private void AnnaActive(bool isActive)
    {
        if (isActive == true) // Anna로
        {
            InputEventHandler.OnChangedForceActionToPlayer_Invoke();
            _annaController.gameObject.SetActive(isActive);
            _npcAnna.SetActive(!isActive);
            _annaController.CheckPoint = _npcAnna.transform.position;
            _annaController.gameObject.transform.position = _npcAnna.transform.position;
            _annaController.gameObject.GetComponent<SpriteRenderer>().flipX
                           = _npcAnna.GetComponent<SpriteRenderer>().flipX;
        }
        else // NPCAnna로
        {
            InputEventHandler.OnChangedForceActionToUI_Invoke();
            _annaController.gameObject.SetActive(isActive);
            _npcAnna.SetActive(!isActive);
            _npcAnna.transform.position = _annaController.gameObject.transform.position;
            _npcAnna.GetComponent<SpriteRenderer>().flipX
                = _annaController.gameObject.GetComponent<SpriteRenderer>().flipX;
        }
    }

    #region Event 4-1
    private IEnumerator Event4_1()
    {
        yield return new WaitForSeconds(0.5f);
        _npcAnna.GetComponent<SpriteRenderer>().flipX = true;
        yield return new WaitForSeconds(2f);
        _npcAnna.GetComponent<SpriteRenderer>().flipX = false;
        yield return new WaitForSeconds(2f);
        StartStory();
    }
    #endregion

    #region Event 4-2
    private IEnumerator Event4_2()
    {
        _dialogue4_2.SetActive(true);
        yield return new WaitUntil(() => _isTextShowed);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        _dialogue4_2.SetActive(false);
        StartStory();
    }
    public void TextShowed()
    {
        _isTextShowed = true;
    }
    #endregion

    #region Event 4-3
    private IEnumerator Event4_3()
    {
        yield return new WaitUntil(() => !_isMove);
        AnnaActive(false);
        _npcAnnaAnim.Animator.Play("Anna_Move");
        Vector3 vector3 = _npcAnna.transform.position;
        CameraManager.Instance.ChangeTarget(_npcAnna.transform);
        while (_npcAnna.transform.position.x < _targetPosition.position.x)
        {
            vector3.x += Time.deltaTime * _moveSpeed;
            _npcAnna.transform.position = vector3;
            yield return null;
        }
        AudioManager.Instance.StopLoopingSfx(AudioManager.LoopSfx.Walk);
        _npcAnnaAnim.StartAnim("Anna_Idle");
        StartStory();
    }
    #endregion

    #region Event 4-4
    private IEnumerator Event4_4()
    {
        _cameraOriginSize = _camera.orthographicSize;
        _cameraOriginPos = _camera.transform.position;
        bool isZoomFinished = false;
        CameraEventHandler.Zoom(_camera, CameraManager.Instance.CinemachineCamera, _zoomInSize, _zoomDuration, () => isZoomFinished = true);
        _dialogue4_4.SetActive(true);
        yield return new WaitUntil(() => _isTextShowed);
        yield return new WaitUntil(() => isZoomFinished && Input.GetMouseButtonDown(0));
        _isTextShowed = false;
        _dialogue4_4.SetActive(false);
        StartStory();
    }
    #endregion

    #region Event 4-5
    private IEnumerator Event4_5()
    {
        bool isZoomFinished = false;
        //CameraEventHandler.Zoom(_camera, CameraManager.Instance.CinemachineCamera, _zoomInSize - 0.5f, _zoomDuration, () => isZoomFinished = true);
        CameraEventHandler.ZoomInOut(_camera, CameraManager.Instance.CinemachineCamera, _zoomInSize - 0.5f, _cameraOriginSize, _zoomDuration, 0.5f, () => isZoomFinished = true);
        _dialogue4_5.SetActive(true);
        yield return new WaitUntil(() => _isTextShowed);
        yield return new WaitUntil(() => isZoomFinished && Input.GetMouseButtonDown(0));
        _isTextShowed = false;
        _dialogue4_5.SetActive(false);
        StartStory();
    }
    #endregion

    #region Event 4-7
    private IEnumerator Event4_7()
    {
        bool isZoomFinished = false;
        CameraEventHandler.Zoom(_camera, CameraManager.Instance.CinemachineCamera, _stageCameraSize, _zoomDuration, () => isZoomFinished = true);
        yield return new WaitUntil(() => isZoomFinished);
        AnnaActive(true);
        CameraManager.Instance.ChangeTarget(_annaController.transform);
        _wall.SetActive(false);
    }
    #endregion

    private IEnumerator StartSetting(Action action)
    {
        yield return new WaitUntil(() => _annaController.IsGround);
        AnnaActive(false);
        yield return new WaitForSeconds(1f);
        bool isZoomFinished = false;
        CameraEventHandler.Zoom(_camera, CameraManager.Instance.CinemachineCamera, _cameraOriginSize, _zoomDuration, () => isZoomFinished = true);
        yield return new WaitUntil(() => isZoomFinished);
        yield return new WaitForSeconds(1f);
        action?.Invoke();
    }
    public override void Enter(GameObject go = null)
    {
        if (_isCheck) return;
        _isCheck = true;
        _isMove = false;
    }
}
