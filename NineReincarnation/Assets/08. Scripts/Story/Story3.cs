using System.Collections;
using Febucci.UI.Core.Parsing;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Story3 : Story
{
    private void Start()
    {
        StoryManager.Instance.eventObj["혼령1"].AnimEvent("Ghost_Down");
        StartStory();
    }

    public override void PlayStory(string eventFunc)
    {
        switch(eventFunc) 
        {
            case "Event1-1":
                StartCoroutine(AnimStory());
                break;
            case "Event1-2":
                StartCoroutine(DialogueStory());
                break;
            case "Event1-3":
                StartCoroutine(AnimStory());
                break;
            case "Event1-4":
                StartCoroutine(DialogueStory());
                break;
            case "Event1-5":
                StartCoroutine(AnimStory());
                break;
            case "Event1-6":
                StartCoroutine(TextEventStory());
                break;
            case "Event1-7":
                StartCoroutine(TextEventStory());
                break;
            case "Event1-8":
                StartCoroutine(DialogueStory());
                break;
            case "Event1-9":
                StartCoroutine(Event1_9());
                break;
            case "Event1-10":
                StartCoroutine(Event1_10());
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
                StoryManager.Instance.eventObj["혼령1"]?.StartAnim("Ghost_Finger");
                break;
        }
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
