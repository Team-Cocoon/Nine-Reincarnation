using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;
using VContainer.Unity;

public class TitleScope : LifetimeScope
{
    [SerializeField] private TitleUI _titleUI;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<TitleUI>(_titleUI);
    }
}
