using System;
using UnityEngine;

public class PlayerAnimationController
{
    private readonly Animator _animator;
    private AnimState _currentState;
    private bool _wasGrounded;
    public AnimState currentState => _currentState;

    public PlayerAnimationController(Animator animator)
    {
        _animator = animator;
        _currentState = AnimState.Idle;
    }

    public void UpdateMovementAnim(Vector2 velocity, bool isGrounded)
    {
        _animator.SetFloat("xVelocity", Math.Abs(velocity.x));
        _animator.SetFloat("yVelocity", velocity.y);

        if(isGrounded == true)
        {
            _animator.SetBool("isJumping", false);
        }

        if(!isGrounded && velocity.y < 0f)
        {
            SetState(AnimState.Fall);
        }
        else if (isGrounded)
        {
            if(Math.Abs(velocity.x) > 0f)
            {
                SetState(AnimState.Run);
            }
            else
            {
                SetState(AnimState.Idle);
            }
        }
        _wasGrounded = isGrounded;
    }

    public void SetState(AnimState newState)
    {
        if(_currentState == newState) return;
        _currentState = newState;
        switch (newState)
        {
            case AnimState.Jump:
                _animator.SetBool("isJumping", true);
                break;
        }
        Debug.Log($"[AnimationState] → {_currentState}");
    }
}

public enum AnimState
{
    Idle,
    Run,
    Jump,
    Fall,
    End
}
