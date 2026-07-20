using Unity.Cinemachine;
using UnityEngine;

public class ChangeCameraBound : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner2D _camera;
    [SerializeField] private PolygonCollider2D _changeBound;

    public void ChangeBound()
    {
        ChangeBound(_camera, _changeBound);
    }

    public void ChangeBound(CinemachineConfiner2D camera, PolygonCollider2D changeBound)
    {
        if(camera == null || changeBound == null) return;

        camera.BoundingShape2D = changeBound;
        camera.InvalidateBoundingShapeCache();
    }
}
