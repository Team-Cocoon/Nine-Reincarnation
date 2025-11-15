using System.Collections;
using EventHandler;
using Febucci.UI.Core.Parsing;
using Player.Controller;
using UnityEngine;

public class Story3 : Story
{
    [Header("페이드 객체")]
    [SerializeField] private Fade _fade;

    [Header("안나")]
    private PlayerController _annaController => InputManager.Instance.CurPlayer;
    [SerializeField] private GameObject _npcAnna;

    [Header("가위")]
    [SerializeField] private GameObject _scissors;

    [Header("Event1-9")]
    [SerializeField] private GameObject _dialogue1_9;
    [Header("Event2-5")]
    [SerializeField] private Camera _camera;
    [SerializeField] private float _shakeDuration = 3f;
    [Header("Event2-7")]
    [SerializeField] private GameObject _ghost1;
    [SerializeField] private GameObject _ghost2;
    [SerializeField] private float _fadeTime = 1f;
    private float _time = 0f;
    [Header("Event3-2")]
    [SerializeField] private GameObject _dialogue3_2;
    [Header("Event3-3")]
    [SerializeField] private GameObject _dialogue3_3;
    [Header("Event3-4")]
    private float _cameraOrginSize;
    private Vector3 _cameraOriginPos;
    [SerializeField] private float _zoomInSize = 3f;
    [SerializeField] private GameObject _dialogue3_4;
    [Header("Event3-5")]
    [SerializeField] private GameObject _dialogue3_5;
    [Header("Event3-7")]
    [SerializeField] private float _zoomOutSize = 6f;
    [SerializeField] private float _zoomDuration = 2f;
    [SerializeField] private GameObject _bigTree;
    [SerializeField] private DrawOutline _outline;
    [Header("Event3-8")]
    [SerializeField] private GameObject _dialogue3_8;
    private bool _isTextShowed = false;
    [Header("Event3-12")]
    //[SerializeField] private GameObject _mKey;

    /* Event1-10 에 필요한 변수 */
    private int _laughingCount = 0;

    private void Start()
    {
        SceneEventHandler.SceneStarted += SetStart;
    }

    private void OnDestroy()
    {
        SceneEventHandler.SceneStarted -= SetStart;
    }
    private void SetStart()
    {
        InputEventHandler.OnChangedForceActionToUI_Invoke();
        //StoryManager.Instance.eventObj["혼령1"].StartAnim("Ghost_Down");
        //StoryManager.Instance.eventObj["혼령2"].StartAnim("Ghost_Down");
        StoryManager.Instance.eventObj["안나"].StartAnim("Anna_Down");
        _cameraOrginSize = _camera.orthographicSize;
        _cameraOriginPos = _camera.transform.position;
        StartStory();
    }

    public override void PlayStory(string eventFunc)
    {
        base.PlayStory(eventFunc);
        switch (eventFunc)
        {
            case "Event":
                StartStory(); // 임시 테스트용
                break;
            case "Event1-7":
                EventFinger();
                break;
            case "Event1-9":
                StartCoroutine(Event1_9());
                break;
            case "Event1-10":
                StartCoroutine(Event1_10());
                break;
            case "Event2-5":
                CameraEventHandler.Shake(_camera, _shakeDuration, 0.1f, 10, 90, true, StartStory);
                break;
            case "Event2-7":
                StoryManager.Instance.StartAnim(EventFade);
                StoryManager.Instance.eventObj["혼령2"].StartAnim("Ghost_NoWake");
                break;
            case "Event3-2":
                StartCoroutine(Event3_2());
                break;
            case "Event3-3":
                StartCoroutine(Event3_3());
                break;
            case "Event3-4":
                StartCoroutine(Event3_4());
                break;
            case "Event3-5":
                StartCoroutine(Event3_5());
                break;
            case "Event3-6":
                StartCoroutine(Event3_6());
                break;
            case "Event3-7":
                // 나무 클릭하는 기능 추가해야 함
                CameraEventHandler.Zoom(_camera, _zoomOutSize, _zoomDuration, TreeClickEvent);
                break;
            case "Event3-8":
                StartCoroutine(Event3_8());
                break;
            case "Event3-10":
                _scissors.SetActive(true);
                StoryManager.Instance.eventObj["가위"].TriggerEvent("Move", StartStory);
                break;
            case "Event3-12":
                // 안나가 오른쪽 밖으로 나가는 기능 추가해야 함
                AnnaActive(true);
                //UIEventHandler.OnOpenListUpdateToolTipUI(() => {  });
                StartCoroutine(KeyShow());
                break;
        }
    }
    public override void OnTextEvent(EventMarker eventMarker)
    {
        switch (eventMarker.name)
        {
            case "sound":
                DialogueManager.Instance.PlayTextSound();
                break;
            case "event1_6":
                StoryManager.Instance.eventObj["혼령2"]?.StartAnim("Ghost_Laughing");
                break;
            case "event1_7":
                StoryManager.Instance?.StartAnim(StartEvent1_7);
                break;
            case "event1_10":
                RepeatLaughing();
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

    #region Event 1-7
    private void EventFinger()
    {
        DialogueManager.Instance?.StartDialogue();
        StoryManager.Instance?.SetDialogueData();
        StoryManager.Instance?.StartAnim();
        DialogueManager.Instance?.TypeWriter.onMessage.RemoveListener(OnTextEvent);
        DialogueManager.Instance?.TypeWriter.onMessage.AddListener(OnTextEvent);
    }
    private void StartEvent1_7()
    {
        DialogueManager.Instance?.TypeWriter.onMessage.RemoveListener(OnTextEvent);
        StartCoroutine(Event1_7());
    }
    private IEnumerator Event1_7()
    {
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        StartStory();
    }
    #endregion

    #region Event 1-9
    private IEnumerator Event1_9()
    {
        _dialogue1_9.SetActive(true);
        yield return null;
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        _dialogue1_9.SetActive(false);
        StartStory();
    }
    #endregion

    #region Event 1-10
    private IEnumerator Event1_10()
    {
        DialogueManager.Instance?.StartDialogue();
        StoryManager.Instance?.SetDialogueData();
        StoryManager.Instance?.StartAnim();
        DialogueManager.Instance?.TypeWriter.onMessage.RemoveListener(OnTextEvent);
        DialogueManager.Instance?.TypeWriter.onMessage.AddListener(OnTextEvent);
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        DialogueManager.Instance?.TypeWriter.onMessage.RemoveListener(OnTextEvent);
        yield return new WaitUntil(() => _laughingCount == 3);
        yield return new WaitForSeconds(2f);
        StartStory();
    }

    private void RepeatLaughing()
    {
        if (_laughingCount < 3)
        {
            _laughingCount++;
            StoryManager.Instance.StartAnim(RepeatLaughing);
        }
    }
    #endregion

    #region Event 2-7
    private void EventFade()
    {
        StartCoroutine(FadeIn());
    }
    private IEnumerator FadeIn()
    {
        Color alpha = _ghost1.GetComponent<SpriteRenderer>().color;
        while (alpha.a > 0f)
        {
            _time += Time.deltaTime / _fadeTime;
            alpha.a = Mathf.Lerp(1, 0, _time);
            _ghost1.GetComponent<SpriteRenderer>().color = alpha;
            _ghost2.GetComponent<SpriteRenderer>().color = alpha;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        StartStory();
    }
    #endregion

    #region Event 3-2
    /* 두리번 거리는 이벤트 */
    //private IEnumerator Event3_2()
    //{
    //    _npcAnna.GetComponent<SpriteRenderer>().flipX = true;
    //    yield return new WaitForSeconds(2f);
    //    _npcAnna.GetComponent<SpriteRenderer>().flipX = false;
    //    yield return new WaitForSeconds(2f);
    //    StartStory();
    //}
    private IEnumerator Event3_2()
    {
        _dialogue3_2.SetActive(true);
        yield return new WaitUntil(() => _isTextShowed);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        _isTextShowed = false;
        _dialogue3_2.SetActive(false);
        yield return null;
        StartStory();
    }
    #endregion

    #region Event 3-3
    private IEnumerator Event3_3()
    {
        _dialogue3_3.SetActive(true);
        yield return new WaitUntil(() => _isTextShowed);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        _isTextShowed = false;
        _dialogue3_3.SetActive(false);
        yield return null;
        StartStory();
    }
    #endregion

    #region Event 3-4
    private IEnumerator Event3_4()
    {
        CameraEventHandler.Shake(_camera, _shakeDuration, 0.1f, 10, 90, true);
        bool isZoomFinished = false;
        CameraEventHandler.ZoomToTarget(_camera, _npcAnna.transform.position, _zoomInSize, _zoomDuration, () => isZoomFinished = true);
        _dialogue3_4.SetActive(true);
        yield return new WaitUntil(() => _isTextShowed);
        yield return new WaitUntil(() => isZoomFinished && Input.GetMouseButtonDown(0));
        _isTextShowed = false;
        _dialogue3_4.SetActive(false);
        yield return null;
        StartStory();
    }
    #endregion

    #region Event 3-5
    private IEnumerator Event3_5()
    {
        CameraEventHandler.Shake(_camera, _shakeDuration, 0.1f, 10, 90, true);
        bool isZoomFinished = false;
        CameraEventHandler.ZoomToTarget(_camera, _npcAnna.transform.position, _zoomInSize - 0.3f, _zoomDuration, () => isZoomFinished = true);
        _dialogue3_5.SetActive(true);
        yield return new WaitUntil(() => _isTextShowed);
        yield return new WaitUntil(() => isZoomFinished && Input.GetMouseButtonDown(0));
        _isTextShowed = false;
        _dialogue3_5.SetActive(false);
        yield return null;
        StartStory();
    }
    #endregion

    #region Event 3-6
    private IEnumerator Event3_6()
    {
        CameraEventHandler.ZoomToTarget(_camera, _cameraOriginPos, _cameraOrginSize, _zoomDuration);
        yield return new WaitForSeconds(4f);
        StartStory();
    }
    #endregion

    #region Event 3-7
    void TreeClickEvent()
    {
        _bigTree.GetComponent<Interaction>().IsInteraction = true;
        _bigTree.GetComponent<BigTree>().SetAction += NoClick;
        _bigTree.GetComponent<Interaction>().enabled = true;
    }
    void NoClick()
    {
        StartStory();
        _bigTree.GetComponent<Interaction>().IsInteraction = false;
        _outline.IsOutline = false;
    }
    #endregion

    #region Event 3-8
    private IEnumerator Event3_8()
    {
        _dialogue3_8.SetActive(true);
        yield return new WaitUntil(() => _isTextShowed);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        _dialogue3_8.SetActive(false);
        yield return null;
        StartStory();
    }
    public void TextShowed()
    {
        _isTextShowed = true;
    }
    #endregion

    #region Event 3-12
    private IEnumerator KeyShow()
    {
        //_mKey.SetActive(true);
        //yield return new WaitForSeconds(3f);
        //_mKey.SetActive(true);
        //while (true)
        //{
        //    _mKey.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        //    yield return new WaitForSeconds(0.5f);
        //    _mKey.transform.localScale = new Vector3(0.3f, 0.2f, 1f);
        //    yield return new WaitForSeconds(0.5f);
        //}
        yield break;
    }
    #endregion

    public override void Enter(GameObject go = null)
    {
        GameEventHandler.StageExcuted_Invoke();
    }

    private bool isNext = false;
    public void NextStage()
    {
        Debug.Log("스킵 버튼 실행");
        Enter(null);
    }
}
