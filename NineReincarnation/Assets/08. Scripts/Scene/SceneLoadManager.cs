using Cysharp.Threading.Tasks;
using UnityEngine;
using Utilities;
using VContainer;

public class SceneLoadManager : MonoBehaviour
{
    [Inject] private SubSceneLoader _subSceneLoader;
    private SceneDataManager _sceneDataManager;
    private SaveManager _saveManager;

    private GameProgressData _gameData => _saveManager.GameData;

    [Inject] 
    public void Construct(SceneDataManager sceneDataManager, SaveManager saveManager)
    {
        _sceneDataManager = sceneDataManager;
        _saveManager = saveManager;

        _subSceneLoader.SubScenePath = GetScenePath();
    }

    public async UniTaskVoid LoadNextScene()
    {
        _subSceneLoader.IncrementLoadCount();
        _subSceneLoader.SubScenePath = GetScenePath();
        await _subSceneLoader.LoadSubScene();
        _subSceneLoader.DecrementLoadCount();
    }

    public string GetScenePath()
    {
        _saveManager.Save();

        string scenePath = "";
        bool isReturn    = false;

        switch (_gameData.State)
        {
            case GameState.Stoty:
                scenePath = _sceneDataManager.GetStorySubScene(_gameData.StoryIndex, _gameData.StorySubIndex);
                isReturn = _sceneDataManager.NextStory(ref _gameData.StoryIndex, ref _gameData.StorySubIndex);
                break;
            case GameState.Stage:
                scenePath = _sceneDataManager.GetStageSubScene(_gameData.StageIndex, _gameData.StageSubIndex);
                isReturn = _sceneDataManager.NextStage(ref _gameData.StageIndex, ref _gameData.StageSubIndex);
                break;
        }

        if(!isReturn)
        {
            _saveManager.Save();
            GameEventHandler.TitleExcuted_Invoke();
        }

        return scenePath;
    }

}
