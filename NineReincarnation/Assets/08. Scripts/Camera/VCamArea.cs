using UnityEngine;
using VContainer;

public class VCamArea : MonoBehaviour
{
    [Inject] 
    [SerializeField] private VirtualCameraManager _virtualCameraManager;
    public int Index = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("영역 변경" + Index);
            _virtualCameraManager.SetPrioriy(Index);
        }
        else
        {
            Debug.Log(collision.tag);
        }
    }
}
