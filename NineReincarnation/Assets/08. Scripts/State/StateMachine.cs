using System;
using State;
namespace StateMachine
{
    public class StateMachine
    {
        //현재 상태
        public IState CurrentState { get; protected set; }

        //상태 변화 시 실행할 이벤트 모음
        public event Action<IState> stateChanged;

        /// <summary>
        /// 상태들 초기화
        /// </summary>
        public void Initialize(IState state)
        {
            CurrentState = state;
            state.Enter();

            //상태 변화 시 이벤트 발생
            stateChanged?.Invoke(state);
        }

        /// <summary>
        /// 상태 변환 시 실행
        /// </summary>
        /// <param name="state"></param>
        public void TransitionTo(IState nextState)
        {
            if (CurrentState == nextState) return;

            CurrentState.Exit();
            CurrentState = nextState;
            nextState.Enter();

            //상태 변화 시 이벤트 실행
            stateChanged?.Invoke(nextState);
        }

        /// <summary>
        /// 해당 상태에서 실행해야 할 내용 실행
        /// </summary>
        virtual public void Excute()
        {
            if (CurrentState != null)
            {
                CurrentState.Execute();
            }
        }
    }
}