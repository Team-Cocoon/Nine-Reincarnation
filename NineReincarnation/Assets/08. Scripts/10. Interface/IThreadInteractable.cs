using UnityEngine;

public enum ThreadCompatibility
{
    None = 0,
    Red = 1 << 0,  
    Blue = 1 << 1, 
    Both = Red | Blue 
}

public interface IThreadInteractable : IClickInteractableToggle
{
    public ThreadCompatibility AllowedThreads { get; }
    public void OnThreadHit(ThreadType threadType);
}
