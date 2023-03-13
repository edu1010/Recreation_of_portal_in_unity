using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public LayerMask m_RayLayerMask;
    public float m_LaserMaxDistance = 250.0f;
    Portal m_LastPortal;
    bool m_WasLastHitPortal = false;
    public virtual void ShootLaser()
    {
        m_LineRenderer.gameObject.SetActive(true);
        Ray l_Ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        float l_Distance = m_LaserMaxDistance;

        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_LaserMaxDistance, m_RayLayerMask.value))
        {
            l_Distance = l_RaycastHit.distance;
            if (l_RaycastHit.collider.tag == "RefractionCube")
            {
                l_RaycastHit.collider.GetComponent<RefractionCube>().ShootLaser();
            }
            else if (l_RaycastHit.collider.tag == "Player")
            {
                GameController.GetGameController().GetPlayer().SetLife(false);
            }
            else if(l_RaycastHit.collider.tag == "PortalButton")
            {
                l_RaycastHit.collider.GetComponent<PortalButtonDoors>().OpenDoor();
            }
            else if (l_RaycastHit.collider.tag == "Portal")
            {
                m_LastPortal = l_RaycastHit.collider.GetComponent<Portal>();
                l_RaycastHit.collider.GetComponent<Portal>().ShootLaser(l_RaycastHit.point, m_LineRenderer.transform.forward);
                m_WasLastHitPortal = true;
            }else if (m_WasLastHitPortal)
            {
                m_WasLastHitPortal = false;
                m_LastPortal.DesactiveLine();
            }
            else if(l_RaycastHit.collider.tag == "WinButton")
            {
                GameController.GetGameController().ShowVictoryHud();
            }
        }
        m_LineRenderer.SetPosition(1, new Vector3(0.0f, 0.0f, l_Distance));
    }
    public virtual void ShootLaser(Vector3 point,Vector3 dir)
    {
        m_LineRenderer.gameObject.SetActive(true);
    }
}
