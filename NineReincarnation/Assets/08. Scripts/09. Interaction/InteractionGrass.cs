using Cysharp.Threading.Tasks;
using UnityEngine;

public class InteractionGrass : MonoBehaviour, IEventInterface
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _playerTag = "Player";
    private bool _isWobble = false;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public async UniTask ExecuteEvent(int index)
    {
        switch (index)
        {
            case 0:
                _animator.SetBool("IsLoopWobble", true);
                break;
            case 1:
                _animator.SetBool("IsLoopWobble", false);
                _spriteRenderer.sortingOrder = 0;
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isWobble) return;
        if (!collision.CompareTag(_playerTag)) return;

        PlayWobbleSound();
        _animator.SetTrigger("IsWobble");
        _isWobble = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_isWobble) return;
        if (!collision.CompareTag(_playerTag)) return;

        _isWobble = false;
    }

    public void PlayWobbleSound()
    {
        AudioManager.Instance?.PlaySfx(AudioManager.Sfx.GrassHide);
    }
}
