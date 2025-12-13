using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class ChaseGhost : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent _agent;
    [SerializeField] private Animator           _animator;
    [SerializeField] private Transform          _player;
    [SerializeField] private Animator           _alertAnimator;
    [SerializeField] private SpriteRenderer     _spriteRenderer;

    public BlackboardVariable<bool> IsTargetDetected;
    private float _prevX;

    private void Awake()
    {
        if (_player == null)
        {
            _player = InputManager.Instance.CurPlayer.transform;
        }

        _prevX = transform.position.x;

        _agent.Graph.BlackboardReference.GetVariable("isTargetDetected", out IsTargetDetected);

        _agent.BlackboardReference.SetVariableValue("Player", _player);
        _agent.BlackboardReference.SetVariableValue("ChaseStopPosition", (Vector2)transform.position);
    }

    void LateUpdate()
    {
        if(Mathf.Sign(transform.position.x - _prevX) >= float.Epsilon)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
