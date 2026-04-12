using UnityEngine;

public class WallPlacer : MonoBehaviour
{
    [Header("引用")]
    public WallManager wallManager;      // 城墙管理器，需在 Inspector 中拖入
    public LayerMask groundLayer;        // 地面的 Layer，用于射线检测

    void Update()
    {

#if UNITY_EDITOR
        // 鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            PlaceWallAtMousePosition(Input.mousePosition);
        }
#else
Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                 Debug.Log("点ppppppppppppppppppppp击-------------");
                PlaceWallAtMousePosition(touch.position);
            }
#endif


        // 手机: 单指触摸开始
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
             
        }


        //if (Input.GetMouseButton(0))
        //{
        //    PlaceWallAtMousePosition();
        //}
    }

    /// <summary>
    /// 根据鼠标点击位置摆放城墙
    /// </summary>
    void PlaceWallAtMousePosition(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Debug.Log($"发射射线，屏幕坐标: {screenPos}，射线方向: {ray.direction}");
        Debug.Log(0);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"击中物体: {hit.collider.name}，世界坐标: {hit.point}");
            // 获取点击点的世界坐标
            Vector3 point = hit.point;
            // 计算格子坐标：四舍五入取整（因为每个格子边长为1，中心位于整数坐标）
            int x = Mathf.RoundToInt(point.x);
            int z = Mathf.RoundToInt(point.z);

            // 调用 WallManager 的放置方法
            if (wallManager != null)
            {
                Debug.Log("生成");
                wallManager.PlaceWall(x, z);
            }
            else
            {
                Debug.LogError("WallPlacer: 未指定 WallManager 引用");
            }
        }
    }
}