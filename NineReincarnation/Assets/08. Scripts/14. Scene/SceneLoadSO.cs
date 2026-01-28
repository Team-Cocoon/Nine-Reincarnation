using UnityEngine;

[CreateAssetMenu(fileName = "SceneLoadSO", menuName = "Scriptable Objects/SceneLoadSO")]
public class SceneLoadSO : ScriptableObject
{
    private bool _isSceneLoading = false;

    public bool IsSceneLoading => _isSceneLoading;

    public void IsLoadingComplete()
    {
        _isSceneLoading = false;
    }

    public void IsLoadingNotComplete()
    {
        _isSceneLoading = true;
    }
}
