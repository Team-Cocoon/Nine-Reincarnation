using System.Collections;
using EventHandler;
using Febucci.UI.Core.Parsing;
using Player.Controller;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class Story3 : Story
{
    [Header("페이드 객체")]
    [SerializeField] private Fade _fade;

    [Header("안나")]
    [SerializeField] private PlayerController _annaController;
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
    [Header("Event3-7")]
    [SerializeField] private float _cameraSize = 6f;
    [SerializeField] private float _zoomDuration = 2f;
    [SerializeField] private Interaction _interaction;
    [SerializeField] private DrawOutline _outline;
    [Header("Event3-8")]
    [SerializeField] private GameObject _dialogue3_8;
    private bool _isTextShowed = false;

    /* Event1-10 에 필요한 변수 */
    private int _laughingCount = 0;

    private void Start()
    {
        StoryManager.Instance.eventObj["혼령1"].StartAnim("Ghost_Down");
        StoryManager.Instance.eventObj["혼령2"].StartAnim("Ghost_Down");
        StoryManager.Instance.eventObj["안나"].StartAnim("Anna_Down");
        _fade.FadeInStart(StartStory);
    }

    public override void PlayStory(string eventFunc)
    {
        base.PlayStory(eventFunc);
        switch(eventFunc) 
        {
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
            case "Event3-7":
                // 나무 클릭하는 기능 추가해야 함
                CameraEventHandler.Zoom(_camera, _cameraSize, _zoomDuration, TreeClickEvent);
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
                break;
        }
    }
    public override void OnTextEvent(EventMarker eventMarker)
    {
        switch(eventMarker.name)
        {
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
        if(isActive == true) // Anna로
        {
            _annaController.gameObject.SetActive(isActive);
            _npcAnna.SetActive(!isActive);
            _annaController.CheckPoint = _npcAnna.transform.position;
            _annaController.gameObject.transform.position = _npcAnna.transform.position;
            _annaController.gameObject.GetComponent<SpriteRenderer>().flipX
                           = _npcAnna.GetComponent<SpriteRenderer>().flipX;
        }
        else // NPCAnna로
        {
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
    private IEnumerator Event3_2()
    {
        _npcAnna.GetComponent<SpriteRenderer>().flipX = true;
        yield return new WaitForSeconds(2f);
        _npcAnna.GetComponent<SpriteRenderer>().flipX = false;
        yield return new WaitForSeconds(2f);
        StartStory();
    }
    #endregion

    #region Event 3-7
    void TreeClickEvent()
    {
        _interaction.IsInteraction = true;
        _interaction.SetAction(NoClick);
    }
    void NoClick()
    {
        StartStory();
        _interaction.IsInteraction = false;
        _outline.IsOutline = false;
    }
    #endregion

    #region Event 3-8
    private IEnumerator Event3_8()
    {
        _dialogue3_8.SetActive(true);
        yield return new WaitUntil(() => _isTextShowed);
        yield return new WaitUntil(()=> Input.GetMouseButtonDown(0));
        _dialogue3_8.SetActive(false);
        StartStory();
    }
    public void TextShowed()
    {
        _isTextShowed = true;
    }
    #endregion

    public override void Enter(GameObject go = null)
    {
        SceneManager.LoadScene("Stage3");
    }
}
