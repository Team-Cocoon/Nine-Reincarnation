using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public class SceneLoader : MonoBehaviour
    {
        protected CancellationToken _token;
        private List<string> _scenePathList = new();

        public string LoadingScenePath => SceneDataManager.Instance.LoadingScene;

        protected virtual void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();
        }

        public async UniTask LoadLoadingScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LoadingScenePath, LoadSceneMode.Additive);

            while (!asyncLoad.isDone) //언로드 될때까지 대기
            {
                await UniTask.Yield(_token);
            }

            await asyncLoad.WithCancellation(_token);
        }

        public async UniTask UnLoadLoadingScene()
        {
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
            if (_scenePathList.Count < 1) return;

            int last = _scenePathList.Count - 1;

            for (int i = last; i >= 0; i--)
            {
                await UnloadLastScene();
            }
        }
    }
}

