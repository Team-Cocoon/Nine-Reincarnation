using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public class SceneLoader : MonoBehaviour
    {
        private Scene _mainScene; //메인 씬
        private string _lastLoadedScenePath; //가장 최근 열린 씬
        private Stack<string> _additiveScenePaths = new Stack<string>(); //현재 열려있는 씬 리스트
        private string LoadingScenePath => SceneDataManager.Instance.LoadingScene;

        bool isLoading = false;

        private void Awake()
        {
            _mainScene = SceneManager.GetActiveScene();//현재 열려있는 씬을 메인 씬으로 할당
        }

        private void OnEnable()
        {
            SceneEventHandler.SceneLoadedByPath += SceneEvents_LoadSceneByPath;
            SceneEventHandler.SceneStateChanged += SceneEvents_ChangeSceneState;
            SceneEventHandler.SceneStateChangedAndLoadScenes += SceneEvents_ChangeSceneStateAndLoadScenes;
            SceneEventHandler.SceneLoadedAdditivelyByPath += SceneEvents_LoadSceneAdditivelyByPath;
            SceneEventHandler.SceneUnloadedByPath += SceneEvents_UnloadSceneByPath;
            SceneEventHandler.AllSceneUnloaded += SceneEvents_AllSceneUnloaded;
            SceneEventHandler.LastSceneUnloaded += SceneEvents_LastSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneEventHandler.SceneLoadedByPath -= SceneEvents_LoadSceneByPath;
            SceneEventHandler.SceneStateChanged -= SceneEvents_ChangeSceneState;
            SceneEventHandler.SceneStateChangedAndLoadScenes -= SceneEvents_ChangeSceneStateAndLoadScenes;
            SceneEventHandler.SceneLoadedAdditivelyByPath -= SceneEvents_LoadSceneAdditivelyByPath;
            SceneEventHandler.SceneUnloadedByPath -= SceneEvents_UnloadSceneByPath;
            SceneEventHandler.AllSceneUnloaded -= SceneEvents_AllSceneUnloaded;
            SceneEventHandler.LastSceneUnloaded -= SceneEvents_LastSceneUnloaded;
        }

        private void SceneEvents_LastSceneUnloaded()
        {
            UnloadLastLoadedScene();
        }

        private void SceneEvents_AllSceneUnloaded()
        {
            UnloadAllAdditiveScenes();
        }

        private void SceneEvents_LoadSceneByPath(string scenePath)
        {
            LoadSceneByPath(scenePath);
        }

        private void SceneEvents_ChangeSceneStateAndLoadScenes(string coreScenePath, string prevScenePath, List<string> subScenePaths)
        {
            ChangeSceneStateAndLoadScenes(coreScenePath, prevScenePath, subScenePaths);
        }

        private void SceneEvents_ChangeSceneState(string coreScenePath, string prevScenePath)
        {
            ChangeSceneState(coreScenePath, prevScenePath);
        }

        private void SceneEvents_UnloadSceneByPath(string scenePath)
        {
            UnloadSceneByPath(scenePath);
        }

        private void SceneEvents_LoadSceneAdditivelyByPath(string scenePath)
        {
            LoadSceneAdditivelyByPath(scenePath, false);
        }

        /// <summary>
        /// 현재 씬 재실행
        /// </summary>
        public void ReloadScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        /// <summary>
        /// 씬의 상태 변경
        /// </summary>
        /// <param name="scenePath"></param>
        public void ChangeSceneState(string coreScenePath, string prevCoreScenePath)
        {
            StartCoroutine(LoadScene(coreScenePath, prevCoreScenePath));
        }

        /// <summary>
        /// 씬의 상태 변경
        /// </summary>
        /// <param name="scenePath"></param>
        public void ChangeSceneStateAndLoadScenes(string coreScenePath, string prevScenePath, List<string> subScenePaths)
        {
            StartCoroutine(LoadScene(coreScenePath, prevScenePath, subScenePaths));
        }

        /// <summary>
        /// 경로를 통한 씬 변경
        /// </summary>
        /// <param name="scenePath"></param>
        public void LoadSceneByPath(string scenePath)
        {
            StartCoroutine(LoadScene(scenePath));
        }

        /// <summary>
        /// 마지막에 열린 씬 언로드
        /// </summary>
        public void UnloadLastLoadedScene()
        {
            StartCoroutine(UnloadLastSceneRoutine());
        }

        /// <summary>
        /// 현재 겹쳐서 연 모든 씬을 닫고 다음 씬을 로드(상태 전환 X)
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public IEnumerator LoadScene(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath) || isLoading)
            {
                yield break;
            }
            isLoading = true;

            yield return UnloadLastSceneRoutine();
            yield return LoadAdditiveSceneRoutine(scenePath, false);

            isLoading = false;
        }

        #region 씬 상태 변경 및 단일 씬 로드
        private IEnumerator LoadingSceneRoutine(string coreScenePath, string prevCoreScenePath)
        {
            yield return SceneManager.LoadSceneAsync(LoadingScenePath, LoadSceneMode.Additive);

            yield return UnloadAllAdditiveScenesRoutine();

            yield return UnloadSceneRoutine(prevCoreScenePath);
            yield return LoadAdditiveSceneRoutine(coreScenePath, true);

            yield return new WaitForSecondsRealtime(0.5f);
            yield return SceneManager.UnloadSceneAsync(LoadingScenePath);
        }

        /// <summary>
        /// 현재 겹쳐서 연 모든 씬을 닫고 다음 씬을 로드
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public IEnumerator LoadScene(string coreScenePath, string prevCoreScenePath)
        {
            if (string.IsNullOrEmpty(coreScenePath) || isLoading)
            {
                yield break;
            }

            isLoading = true;

            yield return SceneEventHandler.SceneFadeOut_Invoke().WaitForCompletion();
            SceneEventHandler.SceneExited_Invoke();

            yield return LoadingSceneRoutine(coreScenePath, prevCoreScenePath);

            SceneEventHandler.SceneStarted_Invoke();
            yield return SceneEventHandler.SceneFadeIn_Invoke().WaitForCompletion();

            isLoading = false;
            yield return new WaitForSecondsRealtime(0.5f);
        }
        #endregion

        #region 씬 상태 변경 및 여러 씬 동시 로드
        private IEnumerator LoadingSceneRoutine(string coreScenePath, string prevCoreScenePath, List<string> subScenePaths)
        {
            yield return SceneManager.LoadSceneAsync(LoadingScenePath, LoadSceneMode.Additive);

            yield return UnloadAllAdditiveScenesRoutine();

            yield return UnloadSceneRoutine(prevCoreScenePath);

            List<AsyncOperation> ops = new List<AsyncOperation>();
            ops.Add(SceneManager.LoadSceneAsync(coreScenePath, LoadSceneMode.Additive));

            // 서브 씬들을 추가적으로 로드
            foreach (string scenePath in subScenePaths)
            {
                if (!_additiveScenePaths.Contains(scenePath))
                {
                    _additiveScenePaths.Push(scenePath);
                }
                ops.Add(SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive));
            }

            bool allOperationsAreDone = false;
            while (!allOperationsAreDone)
            {
                float totalProgress = 0f;
                allOperationsAreDone = true;

                foreach (var op in ops)
                {
                    totalProgress += op.progress;
                    if (!op.isDone)
                    {
                        allOperationsAreDone = false;
                    }
                }

                float averageProgress = totalProgress / ops.Count;

                yield return null; // 다음 프레임까지 대기
            }

            _lastLoadedScenePath = _additiveScenePaths.Last(); //가장 마지막에 열린 씬 할당
            Scene scene = SceneManager.GetSceneByPath(_lastLoadedScenePath);
            SceneManager.SetActiveScene(scene); //활성 씬을 현재 씬으로 변경

            yield return new WaitForSecondsRealtime(0.5f);
            yield return SceneManager.UnloadSceneAsync(LoadingScenePath);
        }
        public IEnumerator LoadScene(string coreScenePath, string prevCoreScenePath, List<string> subScenePaths)
        {
            if (string.IsNullOrEmpty(coreScenePath) || isLoading)
            {
                yield break;
            }
            isLoading = true;

            yield return SceneEventHandler.SceneFadeOut_Invoke().WaitForCompletion();
            SceneEventHandler.SceneExited_Invoke();

            yield return LoadingSceneRoutine(coreScenePath, prevCoreScenePath, subScenePaths);

            SceneEventHandler.SceneStarted_Invoke();
            yield return SceneEventHandler.SceneFadeIn_Invoke().WaitForCompletion();

            isLoading = false;
            yield return new WaitForSecondsRealtime(0.5f);
        }
        #endregion

        #region 단순 씬 로드

        /// <summary>
        /// 씬을 리스트에 등록하고 겹쳐서 로드
        /// </summary>
        /// <param name="scenePath"></param>
        public void LoadSceneAdditivelyByPath(string scenePath, bool isCore)
        {
            Scene sceneToLoad = SceneManager.GetSceneByPath(scenePath); //이미 열려있는 씬 중에서 해당 경로에 있는 씬 반환
            if (!sceneToLoad.IsValid()) //로드가 안되어있으면
            {
                // 내부 리스트에 등록
                if (!_additiveScenePaths.Contains(scenePath))
                {
                    _additiveScenePaths.Push(scenePath);
                }

                //해당 씬 로드
                StartCoroutine(LoadAdditiveSceneRoutine(scenePath, isCore));
            }
        }

        //씬 겹쳐서 로드하는 코루틴
        private IEnumerator LoadAdditiveSceneRoutine(string scenePath, bool isCore)
        {
            if (string.IsNullOrEmpty(scenePath)) //경로 검사
            {
                yield break;
            }

            //비동기 방식으로 씬 로드
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);

            while (!asyncLoad.isDone) //씬 로딩이 덜 되었다면?
            {
                float progress = asyncLoad.progress; //현재 로딩 체크, 보통 로딩 바랑 연결하여 씀 
                yield return null;
            }

            if (isCore) yield break;

            _lastLoadedScenePath = _additiveScenePaths.Last(); //가장 마지막에 열린 씬 할당
            Scene scene = SceneManager.GetSceneByPath(_lastLoadedScenePath);
            SceneManager.SetActiveScene(scene); //활성 씬을 현재 씬으로 변경
        }
        #endregion

        #region 씬 언로드

        /// <summary>
        /// 씬 패스에 해당하는 씬 언로드
        /// </summary>
        /// <param name="scenePath"></param>
        public void UnloadSceneByPath(string scenePath)
        {
            StartCoroutine(UnloadSceneRoutine(scenePath));
        }

        /// <summary>
        /// 가장 최근에 열린 씬만 언로드
        /// </summary>
        /// <returns></returns>
        public IEnumerator UnloadLastSceneRoutine()
        {
            yield return UnloadSceneRoutine(_lastLoadedScenePath);
        }

        /// <summary>
        /// 리스트에 등록된 활성화 씬들 모두 언로드
        /// </summary>
        /// <returns></returns>
        public IEnumerator UnloadAllAdditiveScenesRoutine()
        {
            foreach (string scenePath in _additiveScenePaths)
            {
                yield return StartCoroutine(UnloadSceneRoutine(scenePath));
            }

            _additiveScenePaths.Clear();
        }

        //씬 언로드 코루틴
        private IEnumerator UnloadSceneRoutine(string scenePath)
        {
            if (SceneManager.sceneCount <= 1) //현재 메모리에 로드 된 씬 개수 
            {
                //1개도 없으면 언로드할 씬이 없음
                yield break;
            }

            Scene scene = SceneManager.GetSceneByPath(scenePath);
            if (scene.IsValid() && scene != _mainScene)
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);

                while (!asyncUnload.isDone) //언로드 될때까지 대기
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// 활성화 된 모든 씬 언로드 
        /// </summary>
        public void UnloadAllAdditiveScenes()
        {
            foreach (string scenePath in _additiveScenePaths)
            {
                StartCoroutine(UnloadSceneRoutine(scenePath));
            }
            _additiveScenePaths.Clear();
        }
        #endregion

        /// <summary>
        /// 현재 씬 경로 출력
        /// </summary>
        public static void ShowCurrentScenePath()
        {
            string scenePath = SceneManager.GetActiveScene().path;
            Debug.Log("Current scene path: " + scenePath);
        }
    }
}