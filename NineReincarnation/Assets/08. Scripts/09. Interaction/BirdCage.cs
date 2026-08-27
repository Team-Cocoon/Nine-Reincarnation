using UnityEngine;

public class BirdCage : PhasableObject
{
    [SerializeField] private Feather _chicken;
    [SerializeField] private Collider2D _interactCollider;
    [SerializeField] private DeadZone _dZone;

    protected override void SetSolid(bool solid)
    {
        base.SetSolid(solid);

        _chicken.enabled = solid == false;
        _interactCollider.enabled = solid;
        _dZone.enabled = solid;
    }
}
