using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using YooAsset;

namespace MsbFramework.Procedure
{
    /// <summary>
    /// 清理缓存
    /// </summary>
    public class ProcedureClearCacheBundle : AbstractState<ResPackageStates, ProcedureManager>
    {
        public ProcedureClearCacheBundle(FSM<ResPackageStates> fsm, ProcedureManager manager) : base(fsm, manager)
        {

        }

        protected override bool OnCondition()
        {
            return mFSM.CurrentStateId == ResPackageStates.LoadAssemblies;
        }

        protected override void OnEnter()
        {
            var packageName = mTarget._packageName;
            var package = YooAssets.GetPackage(packageName);
            var operation = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
            operation.Completed += OnClearCacheFilesCompleted;
        }

        protected override void OnExit()
        {

        }

        protected override void OnUpdate()
        {

        }

        public void OnClearCacheFilesCompleted(AsyncOperationBase obj)
        {
            LogKit.I("资源清理完成");
            mFSM.ChangeState(ResPackageStates.StartGame);
        }
    }
}
