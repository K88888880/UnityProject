using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickButtonCtrl : MonoBehaviour, IPointerUpHandler, IPointerClickHandler
{
    public bool _isPressed { get; private set; } // 按钮1是否被按下

    public void OnPointerClick(PointerEventData eventData)
    {
        _isPressed = true;
        StartCoroutine(ResetState()); // 启动协程立即重置
    }

    // 当按钮被释放时
    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
    }

    // 协程：一帧内重置状态
    private IEnumerator ResetState()
    {
        yield return null; // 等待一帧，确保状态被检测到
        _isPressed = false; // 重置为false
    }
}