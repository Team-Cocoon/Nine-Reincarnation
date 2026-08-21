public interface IHoverInteractableToggle
{
    public bool IsHoverControlToSelf { get; }
    public void EnableHoverInteraction();
    public void DisableHoverInteraction();
}
