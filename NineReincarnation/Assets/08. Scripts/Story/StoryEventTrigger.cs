using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] private int             _id;
    [SerializeField] private DialogueSpace.DialogueManager _dialogueManager;
    [SerializeField] private string          _playerTag = "Player";

    private bool isTrigger = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTrigger) return;
        if (!collision.CompareTag(_playerTag)) return;

        isTrigger = true;
        _dialogueManager.DialogueExctute(_id);
    }
}
