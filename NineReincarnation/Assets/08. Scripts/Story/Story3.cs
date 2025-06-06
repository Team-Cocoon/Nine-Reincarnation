using System.Collections;
using UnityEngine;

public class Story3 : MonoBehaviour
{
    private void Start()
    {
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
        }
    }

    private IEnumerator Event1_1()
    {
        yield return null;
        StartStory();
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
}
