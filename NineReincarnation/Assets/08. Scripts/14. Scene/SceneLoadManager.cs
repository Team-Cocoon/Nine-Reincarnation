using UnityEngine;
using VContainer;

public enum GameState
{
    Stoty,
    Stage,
}

public class SceneLoadManager
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

    public bool GetInitScenePath(ref string scenePath)
    {
        switch (_gameData.State)
        {
            case SceneStateType.Stage:
                scenePath = _sceneDataManager.GetStageSubScene(_gameData.StageIndex, 0);
                return true;
        }

        return false;
    }

    public string GetScenePath()
    {
        _saveManager.Save();

        switch (_gameData.State)
        {
            case SceneStateType.Stage:
                return GetStageScenePath();
        }

        return null;
    }

    private string GetStageScenePath()
    {
        bool isReturn = _sceneDataManager.HasStage(_gameData.StageIndex);

        if (!isReturn)
        {
            _saveManager.Save();
            return null;
        }

        _isNextStage = _sceneDataManager.NextStage(ref _gameData.StageIndex, ref _gameData.StageSubIndex);
        return  _sceneDataManager.GetStageSubScene(_gameData.StageIndex, _gameData.StageSubIndex);
    }
}
