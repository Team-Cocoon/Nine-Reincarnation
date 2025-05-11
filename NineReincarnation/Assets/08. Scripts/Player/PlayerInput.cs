using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float _speed;

    private bool _isGround = false;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        if(context.started)
        {
            Debug.Log("이동 시작");
        }
        else if(context.performed)
        {
            Debug.Log("이동 중 " + value);
        }
        else if(context.canceled)
        {
            Debug.Log("이동 종료 " + value);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("점프 시작");
        }
        else if (context.performed)
        {
            Debug.Log("점프 중");
        }
        else if (context.canceled)
        {
            Debug.Log("점프 종료");
        }
    }
}
