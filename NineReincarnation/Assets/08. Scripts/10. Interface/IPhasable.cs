using UnityEngine;

public interface IPhasable
{
    public bool IsConnected { get; }
    // 청연 연결
    public void PhaseIn();

    // 청연 연결 해제
    public void PhaseOut();

    // 청연 유지 시간이 남아 있어도 즉시 연결 전 상태로 되돌린다(전환 시 사용).
    public void ForceDisconnect();
}
