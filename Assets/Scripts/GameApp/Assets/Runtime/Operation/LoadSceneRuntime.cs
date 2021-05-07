using UnityEngine.SceneManagement;

namespace GameApp.Assets
{
    public class LoadSceneRuntime : AppendOperation
    {
        public Scene Scene { get; private set; }

        public override bool ShouldStart => !IsRunning && !IsFinish || !Scene.isLoaded;

        public LoadSceneRuntime(string name, AsyncOperationBase prevOperation = null) : base(name, prevOperation)
        {
        }

        protected override void LoadAsset()
        {
            var sceneName = AssetConfig.GetSceneName(name);
            var loadParams = new LoadSceneParameters {loadSceneMode = LoadSceneMode.Additive};
            var req = SceneManager.LoadSceneAsync(sceneName, loadParams);
            req.completed += operation =>
            {
                Scene = SceneManager.GetSceneByName(sceneName);
                Result = Scene;
                if (Scene.IsValid() && Scene.isLoaded)
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