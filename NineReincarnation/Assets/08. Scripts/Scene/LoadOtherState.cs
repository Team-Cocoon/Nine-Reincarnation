using State.SceneState;
using UnityEngine;

public class LoadOtherState : MonoBehaviour
{
    [SerializeField] private SceneStateType sceneStateType;

    public void NextScene()
    {
        switch (sceneStateType)
        {
            case SceneStateType.Stage:
                GameEventHandler.StageExcuted_Invoke();
                break;
            case SceneStateType.Story:
                GameEventHandler.StoryExcuted_Invoke();
                break;
            case SceneStateType.Clear:
                GameEventHandler.GameClearExcuted_Invoke();
                break;
            case SceneStateType.Title:
                GameEventHandler.TitleExcuted_Invoke();
                break;
        }
    }
}
