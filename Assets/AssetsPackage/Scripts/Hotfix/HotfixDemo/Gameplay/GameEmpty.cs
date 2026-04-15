using MsbFramework.Events;
using MsbFramework.UI;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace MsbFramework
{
    public class GameEmpty: MonoBehaviour
    {
       

        public void InitGame()
        {
            Debug.Log("初始化成功-------------------");
            UIKit.OpenPanel<UILogin>(UILevel.Common);
            //string location = "Main";
            ////加载场景
            //YooAssetKit.LoadSceneAsync(location, LoadSceneMode.Single, LocalPhysicsMode.None, false, (progress) =>
            //{
            //    //更新进度
            //    TypeEventSystem.Global.Send(new OnSceneloadUpdateEvent() { progress = progress, desc = "场景加载中" });
            //}, (sceneHandle) =>
            //{
            //    //加载完成
            //    ActionKit.Delay(0.2f, () =>
            //    {
            //        //UIPanelRoot.Instance.CloseLoadingPanel();
            //        //UIPanelRoot.Instance.ClearScreen();
            //    }).Start(this);
            //});
        }
    }
}

