using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCube : LaserController
{
    bool m_IsRefracting = false;

    void Update()
    {
        m_IsRefracting = false;
        m_LineRenderer.gameObject.SetActive(false);
    }

    public override void ShootLaser()
    {
        if (m_IsRefracting)
            return;

        m_IsRefracting = true;
        base.ShootLaser();
    }
}
