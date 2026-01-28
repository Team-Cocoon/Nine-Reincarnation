using UnityEngine;
using VContainer;

public class VCamArea : MonoBehaviour
{
    [Inject]
    [SerializeField] private VirtualCameraManager _virtualCameraManager;
    public int Index = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _virtualCameraManager.SetPrioriy(Index);
        }
    }
}
