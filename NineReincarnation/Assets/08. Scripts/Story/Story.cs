using System.Collections;
using UnityEngine;

public class Story : MonoBehaviour, ICollidable
{
    /* 스토리 진행 */
    protected void StartStory()
    {
        StoryManager.Instance.NextDialogue();
        if (StoryManager.Instance.DialogueEvent.dialogue == null) // 스토리 스테이지 끝
        {
            return;
        }
        PlayStory(StoryManager.Instance.DialogueEvent.dialogue.eventName);
    }

    /* 각 스테이지 별 스토리 진행 */
    public virtual void PlayStory(string eventFunc)
    {
        switch (eventFunc)
        {
            case "AnimStory":
                StartCoroutine(AnimStory());
                break;
            case "DialogueStory":
                StartCoroutine(DialogueStory());
                break;
            case "TextEventStory":
                StartCoroutine(TextEventStory());
                break;
        }
    }

    /* 각 스테이지 별 텍스트 이벤트 */
    public virtual void OnTextEvent(Febucci.UI.Core.Parsing.EventMarker eventMarker)
    {
        return;
    }

    /// <summary>
    /// 애니메이션만 플레이
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AnimStory()
    {
        StoryManager.Instance?.StartAnim(StartStory);
        yield return null;
    }
    /// <summary>
    /// 대화 + 애니메이션 플레이
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DialogueStory()
    {
        DialogueManager.Instance?.StartDialogue();
        StoryManager.Instance?.SetDialogueData();
        StoryManager.Instance?.StartAnim();
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        StartStory();
    }
    /// <summary>
    /// 대화 + 애니메이션 + 텍스트 이벤트 플레이
    /// </summary>
    /// <returns></returns>
    protected IEnumerator TextEventStory()
    {
        DialogueManager.Instance?.StartDialogue();
        StoryManager.Instance?.SetDialogueData();
        StoryManager.Instance?.StartAnim();
        DialogueManager.Instance?.TypeWriter.onMessage.RemoveListener(OnTextEvent);
        DialogueManager.Instance?.TypeWriter.onMessage.AddListener(OnTextEvent);
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        DialogueManager.Instance?.TypeWriter.onMessage.RemoveListener(OnTextEvent);
        StartStory();
    }

    public virtual void Enter(GameObject go = null)
    {
        throw new System.NotImplementedException();
    }

    public virtual void Exit(GameObject go = null)
    {
        throw new System.NotImplementedException();
    }
}
