using MsbFramework;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("虚拟摇杆")]
     public MsbFramework.UI.UIFixedJoystick _handle;
    public JoystickButtonCtrl _button1;
    public JoystickButtonCtrl _button2;

    [Header("胶囊体颜色")]
    public Renderer _capsuleRenderer;

    [Header("刚体")]
    public Rigidbody _rb;
    public float _forceAmount = 3f;

    [Header("地面检测")]
    public Transform _groundCheckPoint;
    public bool _isGrounded;
    public LayerMask _groundLayer;

    //水平和垂直输入
    private float _horizontalInput;
    private float _verticalInput;

    void Start()
    {
        _capsuleRenderer = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //地面检测
        _isGrounded = true;
        PlayerMove();
        PlayerJump();
        PlayerColorChange();
    }

    private void PlayerMove()
    {
        if (_isGrounded == true)
        {
            // 通过虚拟摇杆获取水平和垂直输入
            _horizontalInput = _handle != null ? _handle.GetHorizontal() : 0;
            _verticalInput = _handle != null ? _handle.GetVertical() : 0;

            //用WSAD控制玩家移动
            float horizontal = _horizontalInput;
            float vertical = _verticalInput;
            Vector3 deltaMove = transform.forward * vertical + transform.right * horizontal;
            transform.position += Time.deltaTime * deltaMove * 10;
        }
    }

    private void PlayerJump()
    {
        if (_isGrounded == true)
        {
            if (_button1._isPressed)
            {
                _rb.AddForce(Vector3.up * _forceAmount, ForceMode.Impulse);
            }
        }
    }

    private void PlayerColorChange()
    {
        if (_button2._isPressed)
        {
            if (_capsuleRenderer != null)
            {
                // 生成0-255范围内的随机RGB值，并转换为0-1范围（Unity颜色值使用0-1范围）
                float r = Random.Range(0, 256) / 255f;
                float g = Random.Range(0, 256) / 255f;
                float b = Random.Range(0, 256) / 255f;

                // 创建随机颜色并应用到材质
                Color randomColor = new Color(r, g, b);
                _capsuleRenderer.material.color = randomColor;
            }
        }
    }

    // 在Scene视图中绘制检测范围（辅助调试）
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_groundCheckPoint != null)
        {
            Gizmos.DrawWireSphere(_groundCheckPoint.position, 0.1f);
        }
    }

 
}