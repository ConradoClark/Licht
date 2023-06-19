using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Licht.Impl.Orchestration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Licht.Unity.UI.UIActions
{
    [AddComponentMenu("L!> UI Action: Change Scene")]
    public class UIChangeSceneAction : UIAction
    {
        public delegate IEnumerable<IEnumerable<Action>> ChangeSceneDelegate();

        [field:SerializeField]
        public string SceneName { get; private set; }
        public event ChangeSceneDelegate OnChangeScene;

        public override IEnumerable<IEnumerable<Action>> DoAction()
        {
            foreach (var subscriber in OnChangeScene?.GetInvocationList()
                         .OfType<ChangeSceneDelegate>() ?? Enumerable.Empty<ChangeSceneDelegate>())
            {
                yield return subscriber().AsCoroutine();
            }

            DefaultMachinery.FinalizeWith(() =>
            {
                SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
            });
        }

        public override void OnInit()
        {
            if (SceneUtility.GetBuildIndexByScenePath(SceneName) == -1)
            {
                UnityEngine.Debug.LogWarning($"UIChangeSceneAction scene not on build or non-existent. Scene Name: '{SceneName}'");
            }
        }
    }
}
