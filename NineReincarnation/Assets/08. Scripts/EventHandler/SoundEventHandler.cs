using System;
using UnityEngine;

public class SoundEventHandler
{
    public static Action<float> OnUpdateBgmVolmue;

    public static Action<float> OnUpdateSfxVolmue;

    public static float OnReturnBgmVolmue;

    public static float OnReturnSfxVolmue;
}
