using System.Collections;
using UnityEngine;

public class Story3 : MonoBehaviour
{
    private void Start()
    {
        StoryManager.Instance.eventObj["혼령1"].AnimEvent("Ghost_Down");
        StartStory();
    }

    /* 스토리 진행 */
    private void StartStory()
    {
        StoryManager.Instance.NextDialogue();
        if(StoryManager.Instance.DialogueEvent.dialogue == null) // 스토리 스테이지 끝
        {
            return;
        }
        PlayStory(StoryManager.Instance.DialogueEvent.dialogue.eventName);
    }

    private void PlayStory(string eventFunc)
    {
        switch(eventFunc) 
        {
            case "Event1-1":
                StartCoroutine(Event1_1());
                break;
            case "Event1-2":
                StartCoroutine(Event1_2());
                break;
            case "Event1-3":
                StartCoroutine(Event1_3());
                break;
            case "Event1-4":
                StartCoroutine(Event1_4());
                break;
            case "Event1-5":
                StartCoroutine(Event1_5());
                break;
            case "Event1-6":
                StartCoroutine(Event1_6());
                break;
            case "Event1-7":
                StartCoroutine(Event1_7());
                break;
            case "Event1-8":
                StartCoroutine(Event1_8());
                break;
            case "Event1-9":
                StartCoroutine(Event1_9());
                break;
            case "Event1-10":
                StartCoroutine(Event1_10());
                break;
        }
    }

    private IEnumerator Event1_1()
    {
        StoryManager.Instance.StartAnim(StartStory);
        yield return null;
    }
    private IEnumerator Event1_2()
    {
        DialogueManager.Instance.StartDialogue();
        StoryManager.Instance.SetDialogueData();
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        StartStory();
    }
    private IEnumerator Event1_3()
    {
        yield return null;
        StartStory();
    }
    private IEnumerator Event1_4()
    {
        DialogueManager.Instance.StartDialogue();
        StoryManager.Instance.SetDialogueData();
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        StartStory();
    }
    private IEnumerator Event1_5()
    {
        yield return null;
        StartStory();
    }
    private IEnumerator Event1_6()
    {
        DialogueManager.Instance.StartDialogue();
        StoryManager.Instance.SetDialogueData();
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        StartStory();
    }
    private IEnumerator Event1_7()
    {
        DialogueManager.Instance.StartDialogue();
        StoryManager.Instance.SetDialogueData();
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        StartStory();
    }
    private IEnumerator Event1_8()
    {
        DialogueManager.Instance.StartDialogue();
        StoryManager.Instance.SetDialogueData();
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        StartStory();
    }
    private IEnumerator Event1_9()
    {
        yield return null;
        StartStory();
    }
    private IEnumerator Event1_10()
    {
        DialogueManager.Instance.StartDialogue();
        StoryManager.Instance.SetDialogueData();
        yield return new WaitUntil(() => DialogueManager.Instance.EndDialogue());
        StartStory();
    }
}
