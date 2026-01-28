using UnityEngine;

public class UIAction : MonoBehaviour
{
    public void ActionToggleMainUI()
    {

    }

    public void ActionToggleSettingUI()
    {
        UIEventHandler.ToggleSettingUI_Invoke(true);
    }
}
