using UnityEngine;

public class NewPlayerController : MonoBehaviour, IPawnController
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    
    public void Move(int direction)
    {

    }

    public void Jump()
    {
        
    }
}
