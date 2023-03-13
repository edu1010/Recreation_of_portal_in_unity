using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTimeController : MonoBehaviour
{
    public KeyCode m_AcceleratorKeyCode = KeyCode.RightControl;
    public KeyCode m_ReduceSpeedKeyCode = KeyCode.RightShift;
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(m_AcceleratorKeyCode))
            Time.timeScale = 10.0f;
        if (Input.GetKeyUp(m_AcceleratorKeyCode))
            Time.timeScale = 1.0f;

        if (Input.GetKeyDown(m_ReduceSpeedKeyCode))
            Time.timeScale = 0.1f;
        if (Input.GetKeyUp(m_ReduceSpeedKeyCode))
            Time.timeScale = 1.0f;
    }
#endif
}
