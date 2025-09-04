public interface IClickInteractableToggle
{
    public bool IsClickControlToSelf { get; }
    public void EnableClickInteraction();
    public void DisableClickInteraction();
}
