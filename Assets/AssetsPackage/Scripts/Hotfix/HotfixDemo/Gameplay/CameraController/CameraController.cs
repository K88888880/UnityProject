using MsbFramework.UI;
using QFramework;
using UnityEngine;

public class CameraController : MonoBehaviour, ISingleton
{
    [Header("移动设置")]
    public float panSpeedTouch = 0.5f;   // 触摸滑动速度
    public float panSpeedMouse = 20f;    // 鼠标拖拽速度
    public bool invertMouseY = false;    // 是否反转鼠标Y轴

    [Header("缩放设置")]
    public float zoomSpeedTouch = 2f;    // 双指缩放速度
    public float zoomSpeedMouse = 5f;    // 鼠标滚轮速度
    public float minZoom = 2f;           // 最小正交大小（最近）
    public float maxZoom = 20f;          // 最大正交大小（最远）

    [Header("边界限制（可选）")]
    public bool enableBoundary = false;
    public Vector2 minPosition = new Vector2(-10f, -10f);
    public Vector2 maxPosition = new Vector2(10f, 10f);

    private Camera cam;
    private Vector2 lastTouchPos;         // 单指触摸上一帧位置
    private bool isDragging = false;
    public static CameraController Instance
    {
        get
        {
            return MonoSingletonProperty<CameraController>.Instance;
        }
    }

    public void OnSingletonInit()
    {
    }
   public void Start()
    {
        cam = Camera.main;
        if (!cam.orthographic)
        {
            Debug.LogWarning("相机不是正交模式，缩放逻辑需要调整");
        }
    }

    public void Update()
    {
        // 判断是否使用触摸模式（移动端真机 或 模拟器模拟了触摸）
        bool useTouch = Input.touchCount > 0;

        if (useTouch)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }

        // 边界限制（移动或缩放后都执行一次）
        if (enableBoundary)
            ApplyBoundary();
    }

    // ------------------- 触摸逻辑（真机 + 支持多点触控的模拟器）--------------------
    private void HandleTouchInput()
    {
        // 双指缩放
        if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            // 计算两指之间距离的变化
            Vector2 prevDelta = (t1.position - t1.deltaPosition) - (t2.position - t2.deltaPosition);
            Vector2 currDelta = t1.position - t2.position;
            float deltaMag = prevDelta.magnitude - currDelta.magnitude;

            if (cam.orthographic)
            {
                cam.orthographicSize += deltaMag * zoomSpeedTouch * Time.deltaTime;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            }
            else
            {
                // 透视相机：沿Z轴移动
                transform.Translate(Vector3.forward * deltaMag * zoomSpeedTouch * Time.deltaTime);
            }

            isDragging = false; // 缩放时禁用拖拽标志
        }
        // 单指移动
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastTouchPos;
                Vector3 move = new Vector3(-delta.x, -delta.y, 0) * panSpeedTouch * Time.deltaTime;
                transform.Translate(move);
                lastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }
    }

    // ------------------- 鼠标/模拟器备选逻辑（无触摸时自动生效）--------------------
    private void HandleMouseInput()
    {
        // 鼠标左键拖拽移动
        if (Input.GetMouseButton(0))  // 0 = 左键
        {
            float moveX = Input.GetAxis("Mouse X") * panSpeedMouse * Time.deltaTime;
            float moveY = Input.GetAxis("Mouse Y") * panSpeedMouse * Time.deltaTime;
            if (invertMouseY) moveY = -moveY;
            transform.Translate(new Vector3(-moveX, -moveY, 0));
        }

        // 鼠标滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            if (cam.orthographic)
            {
                cam.orthographicSize -= scroll * zoomSpeedMouse;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            }
            else
            {
                transform.Translate(Vector3.forward * scroll * zoomSpeedMouse);
            }
        }
    }

    // ------------------- 限制相机移动范围（防止看到场景外）--------------------
    private void ApplyBoundary()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(pos.y, minPosition.y, maxPosition.y);
        transform.position = pos;

        // 如果是正交相机，还需限制正交大小已经由 Clamp 完成
    }
}