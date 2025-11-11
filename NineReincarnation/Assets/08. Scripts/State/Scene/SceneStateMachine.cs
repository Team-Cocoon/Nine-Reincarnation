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

        private CoreSceneLoader _sceneManager;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="player"></param>
        public SceneStateMachine(CoreSceneLoader sceneManager)
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

        public ISceneState GetStateByEnum(SceneStateType stateEnum)
        {
            switch (stateEnum)
            {
                case SceneStateType.Title: return _titleState;
                case SceneStateType.Story: return _storyState;
                case SceneStateType.Stage: return _stageState;
                case SceneStateType.Clear: return _clearState;
                default: return null;
            }
        }

        /// <summary>
        /// 트랜지션 전환
        /// </summary>
        /// <param name="state"></param>
        public void TransitionState(SceneStateType state)
        {
            switch (state)
            {
                case SceneStateType.Title:
                    TransitionTo(_titleState);
                    break;
                case SceneStateType.Story:
                    TransitionTo(_storyState);
                    break;
                case SceneStateType.Stage:
                    TransitionTo(_stageState);
                    break;
                case SceneStateType.Clear:
                    TransitionTo(_clearState);
                    break;
            }
        }
    }
}