using UnityEngine;
using UnityEngine.Events;

public class StoryEventConnecter : MonoBehaviour
{
    [SerializeField] private EventTrigger _trigger;
    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;
    [SerializeField] private UnityEvent _onDialogueStart;
    [SerializeField] private UnityEvent _onDialogueEnd;

    [SerializeField] private bool _isStaringDialogue;

    private void Awake()
    {
        if(_isStaringDialogue)
        {
            _onDialogueStart?.Invoke();
            _dialogueManager.DialogueEndAddListener(
                () => _onDialogueEnd?.Invoke());
            return;
        }

        _trigger.OnDialogueStart.AddListener(ConnectEvents);
    }

    private void ConnectEvents()
    {
        _onDialogueStart?.Invoke();
        _dialogueManager.DialogueEndAddListener(() => _onDialogueEnd?.Invoke());
    }
}
