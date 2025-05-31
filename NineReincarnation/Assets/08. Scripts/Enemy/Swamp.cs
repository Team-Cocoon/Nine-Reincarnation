using Unity.VisualScripting;
using UnityEngine;

public class Swamp : MonoBehaviour, ICollidable
{
    [Header("Player Data")]
    [SerializeField] private float _gravity = 0.1f;
    [SerializeField] private float _damping = 10f;
    [SerializeField] private float _speed = 2f;
    private float _initGravity;
    private float _initDamping;
    private float _initSpeed;
    private bool isSave = false;

    /* 초기값 저장 */
    private void RestoreInitData(float gravity, float damping, float speed)
    {
        _initGravity = gravity;
        _initDamping = damping;
        _initSpeed = speed;
    }

    public void Enter(GameObject go)
    {
        if (!isSave)
        {
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            IObjectData objectData = go.GetComponent<IObjectData>();
            RestoreInitData(rb.gravityScale, rb.linearDamping, objectData.Speed);

            rb.gravityScale = _gravity;
            rb.linearDamping = _damping;
            objectData.Speed = _speed;
            
            isSave = true;
        }
    }

    public void Exit(GameObject go)
    {
        if (isSave) 
        {
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            IObjectData objectData = go.GetComponent<IObjectData>();
            
            rb.gravityScale = _initGravity;
            rb.linearDamping = _initDamping;
            objectData.Speed = _initSpeed;
            
            isSave = false;
        }
    }
}
