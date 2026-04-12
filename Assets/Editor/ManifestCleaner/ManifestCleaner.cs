using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class ManifestCleaner : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.Android)
        {
            // 根据当前的 footprint 决定是否删除
            if (IsFootprintDirty())
            {
                DeleteOldManifests();
            }
        }
    }

    private bool IsFootprintDirty()
    {
        // 比较上次保存的 footprint 与当前设置是否一致
        // 例如：存储包名+版本号+权限列表的哈希值，并对比
        // 为简化，此处返回 true 表示总是清理
        return true;
    }

    private void DeleteOldManifests()
    {
        string[] manifestPaths = {
            "Temp/StagingArea/AndroidManifest-main.xml",
            "Temp/StagingArea/AndroidManifest.xml",
            "Library/PlayerDataCache/Android/AndroidManifest.xml"
        };
        foreach (string path in manifestPaths)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                UnityEngine.Debug.Log($"Deleted old manifest: {path}");
            }
        }
    }
}