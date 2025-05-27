using UnityEngine;

/* 기본 실 (Render만 하는 용도) */
public class BasicThread : Thread
{
    protected override void UpdateThread()
    {
        UpdateSegments();
        for (int i = 0; i < segmentCount / 2; i++)
        {
            ApplyConstraint();
        }
        RenderThread();
    }
}
