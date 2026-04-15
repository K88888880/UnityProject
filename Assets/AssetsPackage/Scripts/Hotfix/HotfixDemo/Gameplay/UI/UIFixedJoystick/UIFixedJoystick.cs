using UnityEngine;
using QFramework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MsbFramework.UI
{
    /// <summary>
    /// 固定摇杆，检测拖拽方向并输出范围[-1,1]的Vector2
    /// </summary>
    public partial class UIFixedJoystick : UIPanel, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        PlayerMovement playerMovement;
        public JoystickButtonCtrl JoystickButtonCtrl;

        protected override void OnInit(IUIData uiData = null)
        {
            base.OnInit(uiData);
            if (Camera.main != null)
            {
                Camera.main.GetComponent<CameraController>().enabled = false;
            }
            else
            {
                LogKit.E("空---------------1111");
            }
            var playerass = YooAssetKit.LoadAssetSync<GameObject>("Assets/AssetsPackage/Prefab/Entities/Player.prefab");
            if (playerass != null)
            {
                var player = Instantiate(playerass);
                playerMovement = player.GetComponent<PlayerMovement>();
                playerMovement._handle = GetComponent<UIFixedJoystick>();
                playerMovement._button1 = playerMovement._button2 = JoystickButtonCtrl;
                Camera.main.transform.gameObject.AddComponent<CameraFollow>().target = player.transform;

            }
            else
            {
                LogKit.E("空---------------");
            }



            Camera.main.transform.gameObject.GetComponent<CameraFollow>().followRotation = true;




        }

        public Image _background;
        public Image _handle;
        public float handleRange = 100f; // 摇杆可移动范围

        private Vector2 inputVector; // 输出的方向向量
        private Vector2 defaultPosition; // 摇杆默认位置


        void Start()
        {
            // 记录默认位置
            defaultPosition = _background.rectTransform.anchoredPosition;

            // 确保摇杆初始位置正确
            ResetJoystick();
        }

        // 当鼠标/触摸按下时
        public void OnPointerDown(PointerEventData eventData)
        {
            // 拖动开始时直接移动到触摸位置
            OnDrag(eventData);
        }
        // 当鼠标/触摸拖动时
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position;

            // 将屏幕位置转换为摇杆背景的本地位置
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out position))
            {
                // 计算位置比例
                position.x = (position.x / _background.rectTransform.sizeDelta.x);
                position.y = (position.y / _background.rectTransform.sizeDelta.y);

                // 计算输入向量
                inputVector = new Vector2(position.x * 2, position.y * 2);
                inputVector = (inputVector.magnitude > 1) ? inputVector.normalized : inputVector;

                // 限制摇杆移动范围
                _handle.rectTransform.anchoredPosition = new Vector2(
                    inputVector.x * handleRange,
                    inputVector.y * handleRange
                );
            }
        }
        // 当鼠标/触摸释放时
        public void OnPointerUp(PointerEventData eventData)
        {
            ResetJoystick();
        }

        // 重置摇杆位置
        private void ResetJoystick()
        {
            inputVector = Vector2.zero;
            _handle.rectTransform.anchoredPosition = Vector2.zero;
        }

        // 获取水平输入
        public float GetHorizontal()
        {
            return inputVector.x;
        }

        // 获取垂直输入
        public float GetVertical()
        {
            return inputVector.y;
        }

        // 获取输入向量
        public Vector2 GetInputVector()
        {
            return inputVector;
        }

        protected override void OnClose()
        {

        }
    }

}
