using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    bool m_TeleportActive = true;
    Rigidbody m_Rigidbody;
    public float m_DotToEnterPortal = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if(CanBeTeleported())
        {
            Vector3 l_PortalForwardXZ = _Portal.transform.forward;
            l_PortalForwardXZ.y = 0.0f;
            l_PortalForwardXZ.Normalize();
            Vector3 l_VelocityXZ = m_Rigidbody.velocity;
            l_VelocityXZ.y = 0.0f;
            l_VelocityXZ.Normalize();
            float l_Dot = Vector3.Dot(l_PortalForwardXZ, -l_VelocityXZ);
            if(l_Dot>m_DotToEnterPortal)
            {
                Vector3 l_Velocity = _Portal.m_OtherPortalTransform.transform.InverseTransformDirection(m_Rigidbody.velocity);
                m_Rigidbody.isKinematic = true;
                Vector3 l_LocalPosition = _Portal.transform.InverseTransformPoint(transform.position);
                transform.position = _Portal.m_OtherPortal.transform.TransformPoint(l_LocalPosition);
                m_Rigidbody.isKinematic = false;
                m_Rigidbody.velocity = _Portal.m_OtherPortal.transform.TransformDirection(l_Velocity);
                transform.localScale *= (_Portal.m_OtherPortal.transform.localScale.x / _Portal.transform.localScale.x);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Portal")
        {
            Teleport(other.GetComponent<Portal>());
        }
        else if(other.tag == "PortalButton")
        {
            other.GetComponent<PortalButtonDoors>()?.OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PortalButton")
        {
            other.GetComponent<PortalButtonDoors>()?.CloseDoor();
        }
    }
}
