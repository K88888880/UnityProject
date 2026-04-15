using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;
using MsbFramework.Events;

namespace MsbFramework.UI
{

    public partial class UILogin : UIPanel
    {
        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            BtnLogin.onClick.AddListener(() =>
            {

                //GameObject g = new GameObject();
                //g.AddComponent<WallManager>();
                //g.AddComponent<WallPlacer>();
                //UIKit.ClosePanel<UILogin>();
                //UIPanelRoot.Instance.OpenLoadingPanel();
                string location = "Main";
                //加载场景
                YooAssetKit.LoadSceneAsync(location, LoadSceneMode.Single, LocalPhysicsMode.None, false, (progress) =>
                {
                    //更新进度
                    TypeEventSystem.Global.Send(new OnSceneloadUpdateEvent() { progress = progress, desc = "场景加载中" });
                }, (sceneHandle) =>
                {
                    //加载完成
                    ActionKit.Delay(0.2f, () =>
                    {
                        UIPanelRoot.Instance.CloseLoadingPanel();
                        UIPanelRoot.Instance.ClearScreen();
                        UIKit.OpenPanel<UIFixedJoystick>(UILevel.Common);
                        UIKit.ClosePanel<UILogin>();
                    }).Start(this);
                });
            });
        }


        protected override void OnClose()
        {

        }
    }
}
