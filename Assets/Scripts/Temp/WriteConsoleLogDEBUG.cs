using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WriteConsoleLogDEBUG : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Log Rect Position")]
    public void LogPosition()
    {
        Debug.Log("Rect Position: " + transform.GetComponent<RectTransform>().position);
    }
#endif
}
