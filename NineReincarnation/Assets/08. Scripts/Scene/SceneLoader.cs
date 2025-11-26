using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Utilities
{
    public class SceneLoader : MonoBehaviour
    {
        [Inject] protected SceneDataManager _sceneDataManager;
        [Inject] protected SaveManager _saveManager;

        static private int _loadSceneCount = 0;
        static private Stack<SceneLoader> _sceneLoaders = new();

        protected CancellationToken _token;
        private List<string> _scenePathList = new();

        public int LoadSceneCount => _loadSceneCount;
        public void IncrementLoadCount() => _loadSceneCount++;
        public void DecrementLoadCount() => _loadSceneCount--;
        public string LoadingScenePath => _sceneDataManager.LoadingScene;

        protected virtual void Awake()
        {
            _sceneLoaders.Push(this);
            _token = this.GetCancellationTokenOnDestroy();
        }

        private void OnDestroy()
        {
            if(_sceneLoaders.Peek() == this) 
            { 
                _sceneLoaders.Pop(); 
            }
        }

        public async UniTask LoadLoadingScene()
        {
            if (_loadSceneCount > 1) return;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LoadingScenePath, LoadSceneMode.Additive);

            while (!asyncLoad.isDone) //언로드 될때까지 대기
            {
                await UniTask.Yield(_token);
            }

            await asyncLoad.WithCancellation(_token);
        }

        public async UniTask UnLoadLoadingScene()
        {
            if(_loadSceneCount > 1) return;

            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(LoadingScenePath);

            while (!asyncLoad.isDone) //언로드 될때까지 대기
            {
                await UniTask.Yield(_token);
            }

            await asyncLoad.WithCancellation(_token);
        }


        /// <summary>
        /// 씬 열기
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async UniTask LoadSceneByPath(string path)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);

            while (!asyncLoad.isDone) //언로드 될때까지 대기
            {
                await UniTask.Yield(_token);
            }

            await asyncLoad.WithCancellation(_token);

            Debug.Log("로드 되는 씬 : " + path);

            Scene scene = SceneManager.GetSceneByPath(path);
            SceneManager.SetActiveScene(scene);
            _scenePathList.Add(path);
        }

        /// <summary>
        /// 해당 경로의 씬 닫기
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async UniTask UnloadSceneByPath(string path)
        {
            Scene scene = SceneManager.GetSceneByPath(path);

            //해당 씬이 유효한지 확인
            if (scene.IsValid())
            {
                _scenePathList.Remove(path);

                //해당 씬이 활성화 씬이고 이어받을 씬이 존재하면
                if (SceneManager.GetActiveScene() == scene && _scenePathList.Count > 1)
                {
                    string lastScenePath = _scenePathList.Last();

                    //활성화 씬 변경
                    scene = SceneManager.GetSceneByPath(lastScenePath);
                    SceneManager.SetActiveScene(scene);
                }

                Debug.Log("언로드 되는 씬 : " + path);
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(path);

                while (!asyncUnload.isDone) //언로드 될때까지 대기
                {
                    await UniTask.Yield(_token);
                }
            }
        }


        /// <summary>
        /// 가장 마지막에 열린 씬만 닫기
        /// </summary>
        /// <returns></returns>
        public async UniTask UnloadLastScene()
        {
            if (_scenePathList.Count < 1) return;

            string lastScenePath = _scenePathList.Last();

            await UnloadSceneByPath(lastScenePath);
        }


        /// <summary>
        /// 리스트 내의 모든 씬 닫기
        /// </summary>
        /// <returns></returns>
        public async UniTask UnloadAllScene()
        {
            int last = _scenePathList.Count - 1;

            for (int i = last; i >= 0; i--)
            {
                await UnloadLastScene();
            }
        }

        public async UniTask UnloadStack()
        {
            int count = _sceneLoaders.Count;

            for(int i = 0; i < count; ++i)
            {
                SceneLoader top = _sceneLoaders.Peek();
                if (top == this) return;

                await top.UnloadAllScene();
                _sceneLoaders.Pop();
            }            
        }
    }
}

