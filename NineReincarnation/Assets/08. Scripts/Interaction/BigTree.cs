using System;

public class BigTree : DrawOutline, IClickInteractableToggle
{
    public bool IsClickControlToSelf => false;

    public event Action SetAction;

    public void DisableClickInteraction()
    {
        throw new System.NotImplementedException();
    }

    public void EnableClickInteraction()
    {
        SetAction?.Invoke();
    }
}