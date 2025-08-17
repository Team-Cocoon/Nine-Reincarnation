using Manager;
using State.SceneState;

namespace StateMachine.SceneStateMachine
{
    public class SceneStateMachine : StateMachine
    {
        //각 상태들
        public TitleState _titleState;
        public StageState _stageState;
        public StoryState _storyState;
        public ClearState _clearState;

        private SceneStateManager _sceneManager;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="player"></param>
        public SceneStateMachine(SceneStateManager sceneManager)
        {
            _sceneManager = sceneManager;

            _titleState = new TitleState(sceneManager);
            _stageState = new StageState(sceneManager);
            _storyState = new StoryState(sceneManager);
            _clearState = new ClearState(sceneManager);
        }

        public override void Excute()
        {
            AnyState();

            base.Excute();
        }

        private void AnyState()
        {

        }

        /// <summary>
        /// 트랜지션 전환
        /// </summary>
        /// <param name="state"></param>
        public void TransitionState(SceneState state)
        {
            switch (state)
            {
                case SceneState.Title:
                    TransitionTo(_titleState);
                    break;
                case SceneState.Story:
                    TransitionTo(_storyState);
                    break;
                case SceneState.Stage:
                    TransitionTo(_stageState);
                    break;
                case SceneState.Clear:
                    TransitionTo(_clearState);
                    break;
            }
        }
    }
}