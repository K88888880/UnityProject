using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    [Header("预制体")]
    public GameObject wallPrefab;      // 主墙预制体
    public GameObject connectorPrefab; // 连接件预制体

    // 存储所有墙体实例，key为格子坐标
    private Dictionary<Vector2Int, GameObject> walls = new Dictionary<Vector2Int, GameObject>();

    // 存储所有连接件实例，key为边的两个端点坐标（排序后的小->大）
    private Dictionary<(Vector2Int, Vector2Int), GameObject> connectors = new Dictionary<(Vector2Int, Vector2Int), GameObject>();

    // 四个正交方向
    private Vector2Int[] dirs = new Vector2Int[]
    {
        Vector2Int.up,    // (0,1)
        Vector2Int.down,  // (0,-1)
        Vector2Int.left,  // (-1,0)
        Vector2Int.right  // (1,0)
    };

    /// <summary>
    /// 在指定格子放置城墙
    /// </summary>
    public void PlaceWall(int x, int z)
    {
        Vector2Int pos = new Vector2Int(x, z);
        if (walls.ContainsKey(pos))
        {
            Debug.Log("该位置已有城墙");
            return;
        }

        // 实例化主墙
        GameObject newWall = Instantiate(wallPrefab, new Vector3(x, 0, z), Quaternion.identity);
        walls.Add(pos, newWall);

        // 更新当前格子及相邻格子上的连接件
        UpdateConnectionsAt(x, z);
        foreach (var dir in dirs)
        {
            UpdateConnectionsAt(x + dir.x, z + dir.y);
        }
    }

    /// <summary>
    /// 移除指定格子的城墙（可选功能）
    /// </summary>
    public void RemoveWall(int x, int z)
    {
        Vector2Int pos = new Vector2Int(x, z);
        if (!walls.ContainsKey(pos)) return;

        // 移除墙体
        Destroy(walls[pos]);
        walls.Remove(pos);

        // 移除与该格子相关的所有连接件
        RemoveConnectionsOfCell(x, z);

        // 重新检查相邻格子，它们可能需要重建连接
        foreach (var dir in dirs)
        {
            UpdateConnectionsAt(x + dir.x, z + dir.y);
        }
    }

    /// <summary>
    /// 移除与指定格子相连的所有连接件（四条边）
    /// </summary>
    private void RemoveConnectionsOfCell(int x, int z)
    {
        Vector2Int center = new Vector2Int(x, z);
        foreach (var dir in dirs)
        {
            Vector2Int neighbor = center + dir;
            var edgeKey = GetEdgeKey(center, neighbor);
            if (connectors.ContainsKey(edgeKey))
            {
                Destroy(connectors[edgeKey]);
                connectors.Remove(edgeKey);
            }
        }
    }

    /// <summary>
    /// 更新某个格子周围四条边的连接件（根据当前墙体情况）
    /// </summary>
    private void UpdateConnectionsAt(int x, int z)
    {
        Vector2Int cell = new Vector2Int(x, z);
        if (!walls.ContainsKey(cell)) return; // 没有墙体则无需处理

        foreach (var dir in dirs)
        {
            Vector2Int neighbor = cell + dir;
            if (walls.ContainsKey(neighbor))
            {
                // 存在相邻墙体，确保连接件存在
                EnsureConnector(cell, neighbor);
            }
            else
            {
                // 不存在相邻墙体，移除可能存在的连接件
                var edgeKey = GetEdgeKey(cell, neighbor);
                if (connectors.TryGetValue(edgeKey, out GameObject conn))
                {
                    Destroy(conn);
                    connectors.Remove(edgeKey);
                }
            }
        }
    }

    /// <summary>
    /// 确保两个相邻墙体之间存在连接件
    /// </summary>
    private void EnsureConnector(Vector2Int a, Vector2Int b)
    {
        // 计算边的中点位置
        Vector3 posA = new Vector3(a.x, 0, a.y);
        Vector3 posB = new Vector3(b.x, 0, b.y);
        Vector3 midPoint = (posA + posB) / 2f;

        // 确定连接件的旋转方向
        Quaternion rotation = GetConnectorRotation(a, b);

        var edgeKey = GetEdgeKey(a, b);
        if (!connectors.ContainsKey(edgeKey))
        {
            // 创建新连接件
            GameObject connector = Instantiate(connectorPrefab, midPoint, rotation);
            connectors.Add(edgeKey, connector);
        }
        else
        {
            // 已存在连接件，更新位置和旋转（防止因其他操作导致偏移）
            connectors[edgeKey].transform.position = midPoint;
            connectors[edgeKey].transform.rotation = rotation;
        }
    }

    /// <summary>
    /// 根据两个墙体的相对方向返回连接件的旋转
    /// </summary>
    private Quaternion GetConnectorRotation(Vector2Int a, Vector2Int b)
    {
        Vector2Int delta = b - a;
        // 水平方向（左右）：模型默认朝右，无需旋转
        if (delta.x != 0) return Quaternion.identity;
        // 垂直方向（上下）：需要绕Y轴旋转90度
        if (delta.y != 0) return Quaternion.Euler(0, 90, 0);
        return Quaternion.identity; // 不会发生
    }

    /// <summary>
    /// 生成边的唯一键（两个端点坐标排序）
    /// </summary>
    private (Vector2Int, Vector2Int) GetEdgeKey(Vector2Int a, Vector2Int b)
    {
        // 按字典序排序，确保 (a,b) 与 (b,a) 得到同一键
        if (a.x != b.x)
            return a.x < b.x ? (a, b) : (b, a);
        else
            return a.y < b.y ? (a, b) : (b, a);
    }

    // 可选：在场景中显示辅助调试信息（例如绘制网格）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (var wall in walls)
        {
            Vector3 pos = new Vector3(wall.Key.x, 0, wall.Key.y);
            Gizmos.DrawWireCube(pos, Vector3.one * 0.9f);
        }
    }
}