using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionGrass : MonoBehaviour, IEventInterface
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private float _wobbleTime = 1f;
    private bool _isWobble = false;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public async UniTask ExecuteEvent(int index)
    {
        switch(index)
        {
            case 0: await StartWobble(); break;
            case 1: await StopWobble(); break;
        }

    }

    private async UniTask StartWobble()
    {
        _isWobble = true;

        _animator.SetTrigger("IsWobble");
        PlayWobbleSound();
        await UniTask.WaitForSeconds(_wobbleTime);

        _animator.SetTrigger("IsWobble");
        PlayWobbleSound();
        await UniTask.WaitForSeconds(_wobbleTime);

        _animator.SetBool("IsLoopWobble", true);
    }

    private async UniTask StopWobble()
    {
        _animator.SetBool("IsLoopWobble", false);
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
