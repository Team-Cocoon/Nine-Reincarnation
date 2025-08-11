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
        [Header("--- 로딩 씬 경로 ---")]
        [SerializeField] private string LoadingScenePath;

        [Header("--- 스토리 씬 경로 ---")]

        private Scene _mainScene; //메인 씬
        private Scene _lastLoadedScene; //가장 최근 열린 씬
        private Stack<string> _additiveScenePaths = new Stack<string>(); //현재 열려있는 씬 리스트

        private void Awake()
        {
            _mainScene = SceneManager.GetActiveScene();//현재 열려있는 씬을 메인 씬으로 할당
        }

        private void OnEnable()
        {
            SceneEventHandler.SceneLoadedByPath += SceneEvents_LoadSceneByPath;
            SceneEventHandler.SceneStateChanged += SceneEvents_ChangeSceneStateByPath;
            SceneEventHandler.SceneLoadedAdditivelyByPath += SceneEvents_LoadSceneAdditivelyByPath;
            SceneEventHandler.SceneUnloadedByPath += SceneEvents_UnloadSceneByPath;
            SceneEventHandler.AllSceneUnloaded += SceneEvents_AllSceneUnloaded;
            SceneEventHandler.LastSceneUnloaded += SceneEvents_LastSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneEventHandler.SceneLoadedByPath -= SceneEvents_LoadSceneByPath;
            SceneEventHandler.SceneStateChanged -= SceneEvents_ChangeSceneStateByPath;
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

        private void SceneEvents_ChangeSceneStateByPath(string scenePath, string lastScenePath)
        {
            ChangeSceneStateByPath(scenePath, lastScenePath);
        }

        private void SceneEvents_UnloadSceneByPath(string scenePath)
        {
            UnloadSceneByPath(scenePath);
        }

        private void SceneEvents_LoadSceneAdditivelyByPath(string scenePath)
        {
            LoadSceneAdditivelyByPath(scenePath);
        }

        public IEnumerator LoadSceneRoutine(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath)) //Null이거나 비어있다면?
            {
                yield break;
            }

            yield return UnloadLastSceneRoutine(); //가장 마지막에 열린 씬 언로드
            yield return LoadAdditiveSceneRoutine(scenePath); //새로운 씬을 로드
        }

        /// <summary>
        /// 씬 실행
        /// </summary>
        /// <param name="buildIndex"></param>
        public void LoadScene(int buildIndex)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);//빌드 세팅에 등록된 인덱스에 해당하는 씬 경로 반환

            if (string.IsNullOrEmpty(scenePath))
            {
                return;
            }

            SceneManager.LoadScene(scenePath);
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
        /// 빌드 세팅 상에서 다음 씬 실행
        /// </summary>
        public void LoadNextScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }

        /// <summary>
        /// 빌드 세팅 인덱스를 통한 씬 로드
        /// </summary>
        /// <param name="buildIndex"></param>
        public void LoadSceneAdditively(int buildIndex)
        {
            StartCoroutine(LoadAdditiveSceneRoutine(buildIndex));
        }

        /// <summary>
        /// 씬의 상태 변경
        /// </summary>
        /// <param name="scenePath"></param>
        public void ChangeSceneStateByPath(string scenePath, string lastScenePath)
        {
            StartCoroutine(LoadScene(scenePath, lastScenePath));
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
            if (string.IsNullOrEmpty(scenePath))
            {
                yield break;
            }

            yield return UnloadLastSceneRoutine();
            yield return LoadAdditiveSceneRoutine(scenePath);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        /// <summary>
        /// 현재 겹쳐서 연 모든 씬을 닫고 다음 씬을 로드
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public IEnumerator LoadScene(string scenePath, string lastScenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
            {
                yield break;
            }

            yield return SceneEventHandler.SceneFadeOut?.Invoke().WaitForCompletion();
            SceneEventHandler.SceneExited?.Invoke();

            yield return LoadingSceneRoutine(scenePath, lastScenePath);

            yield return SceneEventHandler.SceneFadeIn?.Invoke().WaitForCompletion();
            SceneEventHandler.SceneStarted?.Invoke();

            yield return new WaitForSecondsRealtime(0.5f);
        }

        /// <summary>
        /// 현재 겹쳐서 연 모든 씬을 닫고 다음 씬을 로드
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public IEnumerator LoadScene(string[] scenePaths, string lastScenePath)
        {
            foreach (string scenePath in scenePaths)
            {
                if (string.IsNullOrEmpty(scenePath))
                {
                    yield break;
                }
            }

            yield return SceneEventHandler.SceneFadeOut?.Invoke().WaitForCompletion();
            SceneEventHandler.SceneExited?.Invoke();

            yield return LoadingSceneRoutine(scenePaths, lastScenePath);

            yield return SceneEventHandler.SceneFadeIn?.Invoke().WaitForCompletion();
            SceneEventHandler.SceneStarted?.Invoke();

            yield return new WaitForSecondsRealtime(0.5f);
        }

        private IEnumerator LoadingSceneRoutine(string scenePath, string lastScenePath)
        {
            yield return SceneManager.LoadSceneAsync(LoadingScenePath, LoadSceneMode.Additive);

            yield return UnloadAllAdditiveScenesRoutine();
            _lastLoadedScene = SceneManager.GetSceneByPath(lastScenePath);

            yield return UnloadLastSceneRoutine();
            yield return LoadAdditiveSceneRoutine(scenePath);

            yield return SceneManager.UnloadSceneAsync(LoadingScenePath);
        }


        private IEnumerator LoadingSceneRoutine(string[] scenePaths, string lastScenePath)
        {
            yield return SceneManager.LoadSceneAsync(LoadingScenePath, LoadSceneMode.Additive);

            yield return UnloadAllAdditiveScenesRoutine();
            _lastLoadedScene = SceneManager.GetSceneByPath(lastScenePath);

            yield return UnloadLastSceneRoutine();
            foreach(string scenePath in scenePaths)
            {
                yield return LoadAdditiveSceneRoutine(scenePath);
            }

            yield return SceneManager.UnloadSceneAsync(LoadingScenePath);
        }


        /// <summary>
        /// 씬을 리스트에 등록하고 겹쳐서 로드
        /// </summary>
        /// <param name="scenePath"></param>
        public void LoadSceneAdditivelyByPath(string scenePath)
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
                StartCoroutine(LoadAdditiveSceneRoutine(scenePath));
            }
        }

        //씬 겹쳐서 로드하는 코루틴
        private IEnumerator LoadAdditiveSceneRoutine(string scenePath)
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

            _lastLoadedScene = SceneManager.GetSceneByPath(scenePath); //현재 씬을 lastLoadedScene에 할당

            SceneManager.SetActiveScene(_lastLoadedScene); //활성 씬을 현재 씬으로 변경
        }

        //인덱스로 씬 겹쳐서 로드
        private IEnumerator LoadAdditiveSceneRoutine(int buildIndex)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            yield return LoadAdditiveSceneRoutine(scenePath);
        }

        /// <summary>
        /// 씬 패스에 해당하는 씬 언로드
        /// </summary>
        /// <param name="scenePath"></param>
        public void UnloadSceneByPath(string scenePath)
        {
            Scene sceneToUnload = SceneManager.GetSceneByPath(scenePath);
            if (sceneToUnload.IsValid())
            {
                StartCoroutine(UnloadSceneRoutine(sceneToUnload));
            }
        }


        /// <summary>
        /// 가장 최근에 열린 씬만 언로드
        /// </summary>
        /// <returns></returns>
        public IEnumerator UnloadLastSceneRoutine()
        {
            if (!_lastLoadedScene.IsValid())
                yield break;

            if (_lastLoadedScene != _mainScene)
            {
                yield return UnloadSceneRoutine(_lastLoadedScene);
            }
        }

        /// <summary>
        /// 리스트에 등록된 활성화 씬들 모두 언로드
        /// </summary>
        /// <returns></returns>
        public IEnumerator UnloadAllAdditiveScenesRoutine()
        {
            if (!_lastLoadedScene.IsValid())
                yield break;

            yield return UnloadLastSceneRoutine();

            UnloadAllAdditiveScenes();
        }

        // 씬 언로드 코루틴
        private IEnumerator UnloadSceneRoutine(Scene scene)
        {
            if (SceneManager.sceneCount <= 1) //현재 메모리에 로드 된 씬 개수 
            {
                //1개도 없으면 언로드할 씬이 없음
                yield break;
            }

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);

            while (!asyncUnload.isDone) //언로드 될때까지 대기
            {
                yield return null;
            }
        }

        /// <summary>
        /// 활성화 된 모든 씬 언로드 
        /// </summary>
        public void UnloadAllAdditiveScenes()
        {
            foreach (string scenePath in _additiveScenePaths)
            {
                Scene scene = SceneManager.GetSceneByPath(scenePath);
                if (scene.IsValid() && scene != _mainScene)
                {
                    StartCoroutine(UnloadSceneRoutine(scene));
                }
            }
            _additiveScenePaths.Clear();
        }

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