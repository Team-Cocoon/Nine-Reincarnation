using UnityEngine;
using VContainer.Unity;
using VContainer;

public class StoryCoreScope : LifetimeScope
{
    [SerializeField] private SubSceneLoader _subSceneLoader;

    protected override void Configure(IContainerBuilder builder)
    {
        SceneDataManager sceneDataManager = Parent.Container.Resolve<SceneDataManager>();
        SaveManager saveManager = Parent.Container.Resolve<SaveManager>();
        SaveDataSO saveData = saveManager.SaveData;

        int StoryIndex = saveData.StoryIndex;

        string scenePath = sceneDataManager.GetStorySubScene(StoryIndex, 0);

        builder.RegisterInstance(scenePath);

        builder.RegisterComponent<SubSceneLoader>(_subSceneLoader);


        //서브 씬 로더에서 실행할 씬을 미리 주입
    }
}