using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("移动设置")]
    public float dragSpeed = 0.005f;        // 手机滑动灵敏度（已降低）
    public float mouseDragSpeed = 0.3f;     // PC 鼠标灵敏度
    public float smoothTime = 0.1f;
    public bool useSmoothing = false;

    [Header("缩放设置")]
    public float zoomSpeed = 5f;            // 鼠标滚轮速度
    public float touchZoomSpeed = 0.02f;    // 双指缩放速度（提高至 0.02）
    public float minZoom = 5f;
    public float maxZoom = 20f;

    [Header("初始视角")]
    public Vector3 initialPosition = new Vector3(10f, 15f, -10f);
    public Vector3 initialRotation = new Vector3(35f, 45f, 0f);

    private Vector3 targetPosition;
    public Camera cam;

    // 触摸相关
    private Vector2 lastTouchPos;
    private bool isDragging = false;
    private float initialTouchDistance;
    private float initialZoomValue;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraController: 未找到 Camera 组件！");
            return;
        }
        transform.position = initialPosition;
        transform.rotation = Quaternion.Euler(initialRotation);
        targetPosition = transform.position;
        Input.multiTouchEnabled = true;//开启多点触碰
        Debug.Log("CameraController 初始化完成，位置: " + transform.position + " 旋转: " + transform.rotation.eulerAngles);
    }

    void Update()
    {
        //if (cam == null) return;

        if (Application.isEditor)
        {
            // 编辑器下根据目标平台决定（也可以手动添加开关）
            if (Application.platform == RuntimePlatform.Android) // 注意：编辑器下平台是 WindowsEditor 或 OSXEditor，不会等于 Android
            {
                // 实际需要结合 BuildTarget 判断，参考上面方法
            }
            else
            {
                HandlePCInput();
            }
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("安卓");
            HandleMobileInput();
        }
        // 平滑移动
        if (useSmoothing)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / smoothTime);
        else
           transform.position = targetPosition;
    }

    #region PC输入
    void HandlePCInput()
    {
        // 鼠标右键拖拽
        if (Input.GetMouseButtonDown(1))
        {
            lastTouchPos = Input.mousePosition;
            isDragging = true;
            Debug.Log("PC: 开始拖拽");
        }
        if (Input.GetMouseButton(1) && isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPos;
            MoveCameraByScreenDelta(delta, mouseDragSpeed);
            lastTouchPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
            Debug.Log("PC: 结束拖拽");
        }

        // 滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Zoom(-scroll * zoomSpeed);
            Debug.Log("PC: 滚轮缩放 " + scroll);
        }
    }
    #endregion
    private int prevTouchCount;
    #region 手机输入
    void HandleMobileInput()
    {
        int touchCount = Input.touchCount;
        if (touchCount <= 0) return;
        // 处理双指开始（只在数量从非2变为2时初始化）
        if (touchCount == 2 && prevTouchCount != 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);
            initialTouchDistance = Vector2.Distance(t1.position, t2.position);
            initialZoomValue = GetCurrentZoom();
            Debug.Log($"双指开始，距离={initialTouchDistance}，缩放={initialZoomValue}");
        }

        if (touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPos = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastTouchPos;
                if (delta.magnitude > 0.1f)
                    MoveCameraByScreenDelta(delta, dragSpeed);
                lastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        else if (touchCount == 2)
        {
            // 只有双指保持且不进行初始化重置时才计算缩放
            if (prevTouchCount == 2) // 之前也是双指，进行缩放计算
            {
                Touch t1 = Input.GetTouch(0);
                Touch t2 = Input.GetTouch(1);
                float currentDistance = Vector2.Distance(t1.position, t2.position);
                float delta = currentDistance - initialTouchDistance;
                float newZoom = initialZoomValue - delta * touchZoomSpeed;
                newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
                SetZoom(newZoom);
                // 可选：实时更新初始距离，使缩放更平滑（但会失去基于初始点的缩放速度）
                // initialTouchDistance = currentDistance;
                // initialZoomValue = newZoom;
            }
        }
        else
        {
            isDragging = false;
        }

        prevTouchCount = touchCount;
    }
    #endregion

    #region 核心移动与缩放方法
    /// <summary>
    /// 根据屏幕坐标增量移动相机（在 XZ 平面移动）
    /// </summary>
    private void MoveCameraByScreenDelta(Vector2 screenDelta, float sensitivity)
    {
        // 获取相机自身的右方向和前方向（水平投影）
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        // 计算世界移动量
        Vector3 move = (right * screenDelta.x + forward * screenDelta.y) * sensitivity;
        targetPosition += move;

        // 可选：限制边界（可后续添加）
        // Debug.Log($"移动量: {move}, 新目标位置: {targetPosition}");
    }

    private float GetCurrentZoom()
    {
        return cam.orthographic ? cam.orthographicSize : cam.fieldOfView;
    }

    private void SetZoom(float value)
    {
        if (cam.orthographic)
            cam.orthographicSize = value;
        else
            cam.fieldOfView = value;
    }

    private void Zoom(float delta)
    {
        float newZoom = GetCurrentZoom() - delta;
        SetZoom(Mathf.Clamp(newZoom, minZoom, maxZoom));
    }
    #endregion
}