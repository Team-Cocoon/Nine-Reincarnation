using UnityEngine;

public interface IPhasable
{
    public bool IsConnected { get; }
    // 청연 연결
    public void PhaseIn();

    // 청연 연결 해제
    public void PhaseOut();
}
