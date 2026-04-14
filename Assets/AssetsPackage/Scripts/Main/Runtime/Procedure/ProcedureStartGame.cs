using MsbFramework.Procedure;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using YooAsset;
using System.Reflection;
using System;

namespace MsbFramework.Procedure
{
    public class ProcedureStartGame : AbstractState<ResPackageStates, ProcedureManager>
    {
        public ProcedureStartGame(FSM<ResPackageStates> fsm, ProcedureManager manager) : base(fsm, manager)
        {
        }

        protected override bool OnCondition()
        {
            //if(mTarget._playMode == EPlayMode.OfflinePlayMode)
            //    return mFSM.CurrentStateId == ResPackageStates.CreateDownloader;
            return mFSM.CurrentStateId == ResPackageStates.ClearCacheBundle;
        }

        protected override void OnEnter()
        {
            //LogKit.I("开始游戏！");
            LogKit.I("处理开始逻辑！");
            mTarget.SetFinish();
            //UIKit.OpenPanel("UILogin", UILevel.Common);
            Assembly assembly = Assembly.Load("HotfixDemo");//加载程序集,返回类型是一个Assembly
            //Type[] typeArray = assembly.GetTypes();
            //for (int i = 0; i < typeArray.Length; i++)
            //{
            //    Debug.Log(typeArray[i].Name);//这里显示dll下所有的类名
            //}
            //Type type = Type.GetType("HotfixDemo.GameEmpty,HotfixDemo");
            var type = assembly.GetType("MsbFramework.GameEmpty");
            if (type != null)
            {
                var obj = Activator.CreateInstance(type);
                var fun = type.GetMethod("InitGame");
                fun?.Invoke(obj, null);
            }
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }
    }
}