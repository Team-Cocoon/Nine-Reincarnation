using UnityEngine;

public class Continue : MonoBehaviour
{
    public void Next()
    {
        GameEventHandler.StageExcuted_Invoke();
    }
}
