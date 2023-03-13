using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : LaserController,IRestart
{
    bool m_TeleportActive = true;
    Rigidbody m_Rigidbody;
    public float m_DotToEnterPortal = 0.5f;
    public float m_DotAlife = 0.2f;
    Vector3 m_InitPos;
    Quaternion m_InitRot;
    // Start is called before the first frame update
    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_InitPos = transform.position;
        m_InitRot = transform.rotation;
    }
    void Update()
    {
        ShootLaser();
        float l_DotAngle = Vector3.Dot(transform.up, Vector3.up);
        m_LineRenderer.gameObject.SetActive(l_DotAngle > m_DotAlife);
    }


    public bool CanBeTeleported()
    {
        return m_TeleportActive;
    }
    public void SetTeleportActive(bool TeleportActive)
    {
        m_TeleportActive = TeleportActive;
    }
    public void Teleport(Portal _Portal)
    {
        if (CanBeTeleported())
        {
            Vector3 l_PortalForwardXZ = _Portal.transform.forward;
            l_PortalForwardXZ.y = 0.0f;
            l_PortalForwardXZ.Normalize();
            Vector3 l_VelocityXZ = m_Rigidbody.velocity;
            l_VelocityXZ.y = 0.0f;
            l_VelocityXZ.Normalize();
            float l_Dot = Vector3.Dot(l_PortalForwardXZ, -l_VelocityXZ);
            if (l_Dot > m_DotToEnterPortal)
            {
                Vector3 l_Velocity = _Portal.m_OtherPortalTransform.transform.InverseTransformDirection(m_Rigidbody.velocity);
                m_Rigidbody.isKinematic = true;
                Vector3 l_LocalPosition = _Portal.transform.InverseTransformPoint(transform.position);
                transform.position = _Portal.m_OtherPortal.transform.TransformPoint(l_LocalPosition);
                m_Rigidbody.isKinematic = false;
                m_Rigidbody.velocity = _Portal.m_OtherPortal.transform.TransformDirection(l_Velocity);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Portal")
        {
            Teleport(other.GetComponent<Portal>());
        }
    }

    public void Restart()
    {
        gameObject.SetActive(true);
        transform.position = m_InitPos;
        transform.rotation = m_InitRot;
    }
}
