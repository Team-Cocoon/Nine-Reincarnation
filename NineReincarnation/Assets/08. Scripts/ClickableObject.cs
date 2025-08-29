using UnityEngine;

public interface IClickable
{
    public void OnClicked();
}

public class ClickableObject : MonoBehaviour, IClickable
{
    public void OnClicked()
    {
        
    }
}
