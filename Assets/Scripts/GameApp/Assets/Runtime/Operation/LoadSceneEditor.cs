#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace GameApp.Assets
{
    public class LoadSceneEditor : AsyncOperationBase
    {
        private string _path;

        public Scene Scene { get; private set; }

        public LoadSceneEditor(string path)
        {
            _path = path;
        }

        public override void StartAsync()
        {
            base.StartAsync();
            var loadParams = new LoadSceneParameters {loadSceneMode = LoadSceneMode.Additive};
            var req = EditorSceneManager.LoadSceneAsyncInPlayMode(_path, loadParams);
            req.allowSceneActivation = true;
            req.completed += operation =>
            {
                Scene = SceneManager.GetSceneByPath(_path);
                Result = Scene;
                if (Scene.isLoaded)
                {
                    OnSuccess();
                }
                else
                {
                    OnError();
                }
            };
        }
    }
}
#endif