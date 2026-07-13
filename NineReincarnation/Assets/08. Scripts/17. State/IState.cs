namespace State
{
    public interface IState
    {
        /// <summary>
        /// 상태 진입 시 실행
        /// </summary>
        public void Enter();

        /// <summary>
        /// 해당 상태에서 계속 실행
        /// </summary>
        public void Execute();

        /// <summary>
        /// 다른 상태로 변환 시 실행
        /// </summary>
        public void Exit();
    }
}
