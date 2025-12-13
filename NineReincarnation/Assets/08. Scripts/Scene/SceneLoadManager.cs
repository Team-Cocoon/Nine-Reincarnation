using UnityEngine;
using VContainer;

public class SceneLoadManager : MonoBehaviour
{
    [Inject] private SceneDataManager _sceneDataManager;
    [Inject] private SaveManager _saveManager;
    private bool _isNextStage;

    private GameProgressData _gameData => _saveManager.GameData;

    [Inject]
    public void Construct(SceneDataManager sceneDataManager, SaveManager saveManager)
    {
        _sceneDataManager = sceneDataManager;
        _saveManager = saveManager;
    }

    public bool GetScenePath(ref string scenePath)
    {
        _saveManager.Save();

        switch (_gameData.State)
        {
            case GameState.Stoty:
                return GetStoryScenePath(ref scenePath);
            case GameState.Stage:
                return GetStageScenePath(ref scenePath);
        }

        return false;
    }

    private bool GetStoryScenePath(ref string scenePath)
    {
        bool isReturn = _sceneDataManager.HasStory(_gameData.StoryIndex);

        if (!isReturn)
        {
            _saveManager.Save();
            scenePath = null;
            GameEventHandler.GameClearExcuted_Invoke();
            return false;
        }

        scenePath = _sceneDataManager.GetStorySubScene(_gameData.StoryIndex, _gameData.StorySubIndex);

        _sceneDataManager.NextStory(ref _gameData.StoryIndex, ref _gameData.StorySubIndex);

        return true;
    }

    private bool GetStageScenePath(ref string scenePath)
    {
        bool isReturn = _sceneDataManager.HasStage(_gameData.StageIndex);

        if (!isReturn)
        {
            _saveManager.Save();
            scenePath = null;
            GameEventHandler.GameClearExcuted_Invoke();
            return false;
        }

        scenePath = _sceneDataManager.GetStageSubScene(_gameData.StageIndex, _gameData.StageSubIndex);

        _isNextStage = _sceneDataManager.NextStage(ref _gameData.StageIndex, ref _gameData.StageSubIndex);

        return true;
    }
}
