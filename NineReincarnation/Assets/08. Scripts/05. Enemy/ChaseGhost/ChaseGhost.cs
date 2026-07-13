using Player.Controller;
using Unity.Behavior;
using UnityEngine;
using VContainer;

public class ChaseGhost : MonoBehaviour, ICollidable
{
    [SerializeField] private Transform _player;
    [SerializeField] private BehaviorGraphAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _alertAnimator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _spawnPoint;

    public BlackboardVariable<bool> IsTargetDetected;
    public BlackboardVariable<bool> IsTargetCatched;
    public BlackboardVariable<bool> IsClear;
    private float _prevX;

    [Inject]
    public void Construct(PlayerController player)
    {
        _player = player.transform;
    }

    private void Awake()
    {
        _prevX = transform.position.x;

        _agent.Graph.BlackboardReference.GetVariable("isClear", out IsClear);
        _agent.Graph.BlackboardReference.GetVariable("isTargetDetected", out IsTargetDetected);
        _agent.Graph.BlackboardReference.GetVariable("isTargetCatched", out IsTargetCatched);

        _agent.BlackboardReference.SetVariableValue("Player", _player);
        _agent.BlackboardReference.SetVariableValue("ChaseStopPosition", (Vector2)transform.position);
    }

    private void OnEnable()
    {
        GameEventHandler.OnPlayerDead += Clear;


    }
    private void OnDisable()
    {
        GameEventHandler.OnPlayerDead -= Clear;

    }

    private void Clear()
    {
        IsClear.Value = true;
    }

    void LateUpdate()
    {
        if (Mathf.Sign(transform.position.x - _prevX) >= float.Epsilon)
        {
            Flip(false);
        }
        else
        {
            Flip(true);
        }

        _prevX = transform.position.x;
    }

    public void Flip(bool isLeft)
    {
        if (_spriteRenderer.flipX != isLeft)
        {
            _spriteRenderer.flipX = isLeft;
        }
    }

    public void Restart()
    {
        transform.position = _spawnPoint.position;
    }

    public void StartBehavior()
    {
        _agent.enabled = true;
    }

    public void EndBehavior()
    {
        _agent.enabled = false;
    }

    public void Enter(GameObject go = null)
    {
        if (IsTargetDetected)
        {
            Debug.Log("닿음");
            IsTargetCatched.Value = true;
            IsTargetDetected.Value = false;
            go.GetComponent<PlayerController>().Dead();
        }
    }

    public void SoundPlay()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.GhoseDetected);
    }

    public void Exit(GameObject go = null)
    {

    }
}
