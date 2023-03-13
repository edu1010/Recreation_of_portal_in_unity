using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : LaserController
{
    public enum TPortalType
    {
        BLUE = 0,
        ORANGE
    }
    public TPortalType m_PortalType;
    public Camera m_Camera;
    public Transform m_OtherPortalTransform;
    public Portal m_OtherPortal;
    public FPSController m_Player;
    public float m_NearPlaneOffset = 0.3f;
    public List<Transform> m_ValidPoints;
    public float m_maxDistancePortal=250f;
    public LayerMask m_LayerMask;
    public float m_ValidDot = 0.99f; //cos 8 grados
    public float m_MaxValidDistance = 0.1f;
    public LayerMask SecondlayerMask;
    public AudioSource m_PlacePortalSound;

    bool m_IsShooting = false;
    private void Start()
    {
        m_PlacePortalSound = GetComponent<AudioSource>();
    }
   
    void LateUpdate() //o late
    {
       
        Vector3 l_LocalPosition = m_OtherPortalTransform.InverseTransformPoint(m_Player.m_PlayerCamera.transform.position);
        m_OtherPortal.m_Camera.transform.position = m_OtherPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_LocalDirection = m_OtherPortalTransform.InverseTransformDirection(m_Player.m_PlayerCamera.transform.forward);
        m_OtherPortal.m_Camera.transform.forward = m_OtherPortal.transform.TransformDirection(l_LocalDirection);

        float l_Distance = Vector3.Distance(m_OtherPortal.m_Camera.transform.position, m_OtherPortal.transform.position);
        m_OtherPortal.m_Camera.nearClipPlane = l_Distance + m_NearPlaneOffset;
    }
    public bool IsValidPosition(Vector3 Position, Vector3 Normal)
    {
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(Normal);
        for (int i = 0; i < m_ValidPoints.Count; i++)
        {
            Vector3 l_CameraPosition = m_Player.m_PlayerCamera.transform.position;
            Vector3 l_Direction = m_ValidPoints[i].position - l_CameraPosition;
            l_Direction.Normalize();
            Ray l_Ray = new Ray(l_CameraPosition, l_Direction);
            RaycastHit l_RaycastHit;
            if(Physics.Raycast(l_Ray, out l_RaycastHit,m_maxDistancePortal,m_LayerMask.value))
            {
                if(l_RaycastHit.collider.tag == "Drawable")
                {
                    if (Vector3.Dot(Normal, l_RaycastHit.normal) > m_ValidDot)
                    {
                        float l_Distance = Vector3.Distance(m_ValidPoints[i].transform.position, l_RaycastHit.point);
                        if(l_Distance >= m_MaxValidDistance)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

   public override void ShootLaser(Vector3 point, Vector3 dir)
    {
        if (m_IsShooting)
            return;
        m_IsShooting = true;

        Vector3 l_LocalPosition = transform.InverseTransformPoint(point);
        dir.Normalize();
        Vector3 l_LocalDirection = transform.InverseTransformDirection(dir);
        m_OtherPortal.m_LineRenderer.gameObject.SetActive(true);
        m_OtherPortal.ActiveLaser(l_LocalPosition, l_LocalDirection);
    }

    public  void ActiveLaser(Vector3 point,Vector3 dir)
    {
        m_LineRenderer.gameObject.SetActive(true);
        
      //  m_LineRenderer.transform.forward = m_OtherPortal.transform.TransformDirection(dir);

       
        Ray l_Ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        float l_Distance = m_LaserMaxDistance;
        Debug.Log(l_Ray);
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_LaserMaxDistance, SecondlayerMask.value))
        {
            Debug.Log(l_RaycastHit.collider.tag);
            Debug.Log(l_RaycastHit.collider.gameObject.name);

            l_Distance = l_RaycastHit.distance;
            if (l_RaycastHit.collider.tag == "RefractionCube")
            {
                l_RaycastHit.collider.GetComponent<RefractionCube>().ShootLaser();
            }
            else if (l_RaycastHit.collider.tag == "Player")
            {
                GameController.GetGameController().GetPlayer().SetLife(false);
            }
            else if (l_RaycastHit.collider.tag == "PortalButton")
            {
                l_RaycastHit.collider.GetComponent<PortalButtonDoors>().OpenDoor();
            }
            else if (l_RaycastHit.collider.tag == "Portal")
            {
                l_RaycastHit.collider.GetComponent<Portal>().ShootLaser();
            }
        }
        m_LineRenderer.SetPosition(1, new Vector3(0.0f, 0.0f, l_Distance));
    }
    public void DesactiveLine()
    {
        m_OtherPortal.m_IsShooting = false;
        m_IsShooting = false;
        m_LineRenderer.gameObject.SetActive(false);
        m_OtherPortal.m_LineRenderer.gameObject.SetActive(false);
    }
}
