using Cysharp.Threading.Tasks;

public interface IEventInterface
{
    public UniTask ExecuteEvent(int index);
    // 강제 종료용 이벤트 (치트용)
    public void FinishEvent(int index);
}
