using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] protected int _id;
    [SerializeField] protected DialogueSpace.DialogueManager _dialogueManager;
    [SerializeField] protected string _playerTag = "Player";

    protected bool isTrigger = false;

    public UnityEvent OnDialogueStart { get; private set; } = new UnityEvent();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTrigger) return;
        if (!collision.CompareTag(_playerTag)) return;

        isTrigger = true;
        StartDialogue();
    }

    protected virtual void StartDialogue()
    {
        OnDialogueStart?.Invoke();
        OnDialogueStart.RemoveAllListeners();
        _dialogueManager.DialogueExctute(_id).Forget();
    }
}
