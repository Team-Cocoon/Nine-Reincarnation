using UnityEngine;

public class BirdCage : PhasableObject
{
    [SerializeField] private Feather _chicken;

    protected override void SetSolid(bool solid)
    {
        base.SetSolid(solid);

        _chicken.enabled = solid == false;
    }
}
