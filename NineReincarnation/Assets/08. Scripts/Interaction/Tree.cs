using System;
using UnityEngine;

public class Tree : DrawOutline, IClickInteractableToggle
{
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
