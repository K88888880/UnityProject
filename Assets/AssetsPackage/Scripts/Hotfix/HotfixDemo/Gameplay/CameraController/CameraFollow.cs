using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;           // 角色Transform
    [Header("偏移参数")]
    public Vector3 offset = new Vector3(0, 5, -8);   // 相对角色的偏移
    [Header("跟随平滑度")]
    public float smoothSpeed = 5f;
    [Header("是否固定Y轴旋转（王者风格一般固定视角，只跟随位置）")]
    public bool followRotation = false; // 通常设为false，相机只跟随位置，不旋转

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("相机跟随目标未设置！");
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        if (followRotation)
        {
            // 如果希望相机也一直看着目标（像传统第三人称），可以LookAt
            transform.LookAt(target);
        }
        // 否则保持相机原来的旋转角度（手动调整好相机初始角度即可）
    }
}