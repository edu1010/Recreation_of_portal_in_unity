using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CanvasType
{
    PlayerHud=0,
    EnimiesHud,
    DeathHud,
    WinCanvas
}
public class TypeOfCanvas : MonoBehaviour
{
    public CanvasType m_type;
}
