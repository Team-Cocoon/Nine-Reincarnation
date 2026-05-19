#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Unity 메인 툴바(Play 버튼 줄)에 UI를 붙이는 "뷰 전용" 클래스.
/// 실제 기능 로직은 외부에서 콜백으로 주입해서 사용.
/// </summary>
[InitializeOnLoad]
public static class ToolbarPlayButtonsView
{
    private const string rootElementName = "ToolbarPlayToggleViewRoot";
    // 외부에서 연결할 콜백들
    private static bool isSelected = true;

    public static bool OnGetCoreMode => isSelected;  // 현재 Core 모드 상태 반환

    private static ToolbarToggle coreToggle;

    static ToolbarPlayButtonsView()
    {
        EditorApplication.update -= tryInstall;
        EditorApplication.update += tryInstall;
    }


    private static void tryInstall()
    {
        var toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        if (toolbarType == null) return;

        var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
        if (toolbars == null || toolbars.Length == 0) return;

        var toolbar = toolbars[0];

        var rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
        if (rootField == null) return;

        var root = rootField.GetValue(toolbar) as VisualElement;
        if (root == null) return;

        // 중복 생성 방지
        if (root.Q<VisualElement>(rootElementName) != null)
        {
            EditorApplication.update -= tryInstall;
            return;
        }

        // Unity 버전별 zone 이름 차이를 대비한 후보군
        var targetZone =
            root.Q("ToolbarZonePlayMode") ??
            root.Q("ToolbarZoneMiddle") ??
            root.Q("ToolbarZoneCenter") ??
            root.Q("ToolbarZoneRightAlign") ??
            root.Q("ToolbarZoneRight") ??
            root;

        var container = new VisualElement
        {
            name = rootElementName
        };
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;
        container.style.marginLeft = 6;
        container.style.marginRight = 6;

        // Core 토글
        coreToggle = new ToolbarToggle { text = "Core" };
        coreToggle.tooltip = "Core(Base) 씬부터 시작";
        coreToggle.style.marginRight = 4;
        coreToggle.value = isSelected;

        coreToggle.RegisterValueChangedCallback(evt =>
        {
            isSelected = !isSelected;
        });

        container.Add(coreToggle);

        targetZone.Add(container);

        // 설치 완료 후 update 해제
        EditorApplication.update -= tryInstall;
    }
}

#endif