using System;
using UnityEngine;
using VContainer;

public enum SceneLoadType
{
    Init = 0,
    Add = 1,
    Change = 2,
    Load = 3
}

public class SceneTrigger
{
    private SubSceneLoader _sceneLoader;
    private SceneLoadManager _sceneLoadManager;

    [Inject]
    public SceneTrigger(SubSceneLoader sceneLoader, SceneLoadManager sceneLoadManager)
    {
        _sceneLoader = sceneLoader;
        _sceneLoadManager = sceneLoadManager;
    }

    public async void LoadScene(SceneLoadType type)
    {
        if(type == SceneLoadType.Init)
        {
            _sceneLoadManager.GetInitScenePath(ref _sceneLoader.SubScenePath);
        }
        else
        {
            _sceneLoadManager.GetScenePath(ref _sceneLoader.SubScenePath);
        }

        if (string.IsNullOrEmpty(_sceneLoader.SubScenePath)) return;

        switch (type)
        {
            case SceneLoadType.Init:
                await _sceneLoader.AddSubScene();
                break;
            case SceneLoadType.Add:
                await _sceneLoader.AddSubScene();
                break;
            case SceneLoadType.Change:
                await _sceneLoader.ChangeSubScene();
                break;
            case SceneLoadType.Load:
                await _sceneLoader.LoadSubScene();
                break;
        }
    }
}
