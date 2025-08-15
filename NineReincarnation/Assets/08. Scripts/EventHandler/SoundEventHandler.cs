using System;
using UnityEngine;

public static class SoundEventHandler
{
    public static event Action<float> OnUpdateBgmVolmue;

    public static event Action<float> OnUpdateSfxVolmue;


    #region Invoke 처리
    public static void OnUpdateBgmVolmue_Invoke(float value) => OnUpdateBgmVolmue?.Invoke(value);
    public static void OnUpdateSfxVolmue_Invoke(float value) => OnUpdateSfxVolmue?.Invoke(value);
    #endregion
}