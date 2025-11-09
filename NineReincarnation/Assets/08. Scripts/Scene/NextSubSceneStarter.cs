using UnityEngine;

public class NextSubSceneStarter : MonoBehaviour
{
    [SerializeField] private string scenePath;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneEventHandler.SceneLoadedByPath_Invoke(scenePath);
    }
}
