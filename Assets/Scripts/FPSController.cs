using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSController : MonoBehaviour, IRestart
{
    // m es miembro de la clase, l es local
    [Header("Movement")]
    float m_Yaw;
    float m_Pitch;

    

    public float m_YawRotationalSpeed = 360.0f;
    public float m_PitchRotationalSpeed = 180.0f;
    public float m_MinPitch = -80.0f;
    public float m_MaxPitch = 50.0f;
    public Transform m_PitchControllerTransform;
    public bool m_InvertedYaw = false;
    public bool m_InvertedPitch = true;

    [Header("Controls")]
    CharacterController m_CharacterController;
    public float m_Speed = 10.0f;
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;

    public KeyCode m_AttachObjectKeyCode = KeyCode.E;

    public KeyCode m_DebugLockAngleKeyCode=KeyCode.I;
    public KeyCode m_DebugLockKeyCode=KeyCode.O;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    [Header("Gravity")]
    //gravedad
    public float m_GravityMultiplayer = 1.2f;
    float m_VerticalSpeed = 0.0f;
    bool m_OnGround = false;
    float m_time = 0.0f;

    [Header("Run and jump")]
    // saltar y correr
    //float m_VerticalSpeed = 0.0f;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public float m_FastSpeedMultiplier = 1.2f;
    public float m_JumpSpeed = 10.0f;

    [Header("Shoot")]
    public LayerMask m_ShootLayerMask;
    public GameObject m_HitParticle;
    public Camera m_PlayerCamera;

    public int m_Life = 100;
    public int m_Shield = 50;

    int m_StartLife;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    public float m_DotToEnterPortal =0.5f;//60 grados
    Vector3 m_Direction;

   public Transform m_AttachPositionTransform;
    GameObject m_AttachedObject;
    private float m_CurrentAttachedObjectTime;
    private float m_AttachedObjectTime=1f;
    public float m_DetachForce = 10f;

    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public GameObject m_Dummy;

    public Image m_ImageCrosshair;
    
    public Sprite m_EmptyCrosshair;
    public Sprite m_FullCrosshair;
    public Sprite m_EmptyOrangeCrosshair;
    public Sprite m_EmptyBlueCrosshair;

    bool m_Shooted = false;
    public float m_MaxCadence = 0.25f;
    float m_currentCadence = 0.0f;
    Vector3 m_InitialScalePortal;

    bool m_IsAlive = true;


    [Header("Rest")]
    List<Transform> m_Checkpoints;
    int m_CurrentCheckpoint = 0;

    [Header("Audio")]
    AudioSource m_AudioSource;
    public AudioClip m_stepsSound;
    public AudioClip m_pickSound;
    public AudioClip m_throwSound;

    private void Awake()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchControllerTransform.localRotation.eulerAngles.x;

        m_CharacterController = GetComponent<CharacterController>();

        // guardas la pos y rot inicial
        m_StartLife = m_Life;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_InitialScalePortal = m_BluePortal.transform.localScale;//Las escalas iniciales del portal son iguales
        m_IsAlive = true;
        GameController.GetGameController().SetPlayer(this);
        m_AudioSource = GetComponent<AudioSource>();
    }
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_Checkpoints = GameController.GetGameController().GetLevelData().GetCheckpointsList();

    }

    void Update()
    {
        if (GameController.GetGameController().GetGameStates() == GameStates.PLAY)
        {

            // este if sirve para que solo se use el codigo en el editor de unity, no en la build
#if UNITY_EDITOR
            //cursor
            if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
                m_AngleLocked = !m_AngleLocked;
            if (Input.GetKeyDown(m_DebugLockKeyCode))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.None;
                else
                    Cursor.lockState = CursorLockMode.Locked;
                m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
            }
#endif

            //y
            float l_MouseAxisY = Input.GetAxis("Mouse Y");
            float l_MouseAxisX = Input.GetAxis("Mouse X");

#if UNITY_EDITOR
            if (m_AngleLocked)
            {
                l_MouseAxisX = 0.0f;
                l_MouseAxisY = 0.0f;
            }
#endif

            //y
            float l_PitchMovement = l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime; // x=x inicial + v*t

            if (m_InvertedPitch)
                m_Pitch -= l_PitchMovement;
            else
                m_Pitch += l_PitchMovement;

            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
            //x
            float l_YawMovement = l_MouseAxisX * m_YawRotationalSpeed * Time.deltaTime; // x=x inicial + v*t

            if (m_InvertedYaw)
                m_Yaw -= l_YawMovement;
            else
                m_Yaw += l_YawMovement;

            transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f); // pq solo se mueve en eje y
            m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

            //… wasd
            Vector3 l_Movement = Vector3.zero;

            float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
            float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
            Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
            Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));


            if (Input.GetKey(m_UpKeyCode))
                l_Movement = l_Forward;
            else if (Input.GetKey(m_DownKeyCode))
                l_Movement = -l_Forward;
            if (Input.GetKey(m_RightKeyCode))
                l_Movement += l_Right;
            else if (Input.GetKey(m_LeftKeyCode))
                l_Movement -= l_Right;
            l_Movement.Normalize(); // valor unitario para que al ir en diagonal no vayas mas rapido (asi siempre sera de -1 a 1)

            m_Direction = l_Movement;//Dir para teleport
            l_Movement = l_Movement * Time.deltaTime * m_Speed;

            //CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement); //aplicas el movimiento

            // aplicando gravedad
            m_VerticalSpeed += (Physics.gravity.y * m_GravityMultiplayer) * Time.deltaTime;
            l_Movement.y = m_VerticalSpeed * Time.deltaTime;
            CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement); // aplicas el movimiento
            if ((l_CollisionFlags & CollisionFlags.Below) != 0)
            {
                m_OnGround = true;
                m_VerticalSpeed = 0.0f;
                m_time = Time.time;
            }
            else
            {
                if (Time.time - m_time > 0.3)
                {
                    m_OnGround = false;
                }
            }
            if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f) // si hay techo, para que no se quede clavado y asi caiga
                m_VerticalSpeed = 0.0f;

            // saltar y correr
            float l_SpeedMultiplier = 1.0f;
            if (Input.GetKey(m_RunKeyCode))
                l_SpeedMultiplier = m_FastSpeedMultiplier;
            //…
            l_Movement *= Time.deltaTime * m_Speed * l_SpeedMultiplier;

            //…
            if (m_OnGround && Input.GetKeyDown(m_JumpKeyCode))
                m_VerticalSpeed = m_JumpSpeed; //impulso y despues cae

            if (m_OrangePortal.gameObject.activeSelf == false && m_BluePortal.gameObject.activeSelf == false)
            {
                m_ImageCrosshair.sprite = m_EmptyCrosshair;
            }
            else if (m_OrangePortal.gameObject.activeSelf == false && m_BluePortal.gameObject.activeSelf == true)
            {
                m_ImageCrosshair.sprite = m_EmptyOrangeCrosshair;
            }
            else if (m_OrangePortal.gameObject.activeSelf == true && m_BluePortal.gameObject.activeSelf == false)
            {
                m_ImageCrosshair.sprite = m_EmptyBlueCrosshair;
            }
            else if (m_OrangePortal.gameObject.activeSelf == true && m_BluePortal.gameObject.activeSelf == true)
            {
                m_ImageCrosshair.sprite = m_FullCrosshair;
            }

            if (CanShootPortal())
            {
                if (Input.GetMouseButton(0))
                {
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                    {
                        if (m_BluePortal.transform.localScale != m_InitialScalePortal * 2f)//Este vector es el doble de la nomral(la maxima medida que puede tener)
                            m_BluePortal.transform.localScale = 2 * m_BluePortal.transform.localScale;
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                    {
                        if (m_BluePortal.transform.localScale != m_InitialScalePortal * 0.5f)
                            m_BluePortal.transform.localScale = 0.5f * m_BluePortal.transform.localScale;
                    }

                    ShowDummyOrPortal(m_BluePortal);
                }
                else if (Input.GetMouseButton(1))
                {
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                    {

                        if (m_OrangePortal.transform.localScale != m_InitialScalePortal * 2f)
                            m_OrangePortal.transform.localScale = 2 * m_OrangePortal.transform.localScale;
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                    {
                        if (m_OrangePortal.transform.localScale != m_InitialScalePortal * 0.5f)
                            m_OrangePortal.transform.localScale = 0.5f * m_OrangePortal.transform.localScale;
                    }

                    ShowDummyOrPortal(m_OrangePortal);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    ShootPortal(m_BluePortal);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    ShootPortal(m_OrangePortal);
                }
            }


            if (CanAtach() && Input.GetKeyDown(m_AttachObjectKeyCode))
            {
                Attach();
            }
            if (m_AttachedObject != null && Input.GetKeyDown(m_AttachObjectKeyCode))
            {
                Detach(0f);
            }
            if (m_AttachedObject != null && Input.GetMouseButtonDown(0))
            {
                Detach(m_DetachForce);
            }
            UpdateAttachedObject();
            // tambien se podria hacer con rigidbody, menos manual, mas automatico
        }
    }

    private void ShowDummyOrPortal(Portal _Portal)
    {
        Ray l_Ray = m_PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        _Portal.gameObject.SetActive(false);
        if (Physics.Raycast(l_Ray, out l_RaycastHit, 200.0f, m_ShootLayerMask.value))
        {
            if (l_RaycastHit.collider.tag == "Drawable")
            {
                bool l_IsValid = _Portal.IsValidPosition(l_RaycastHit.point, l_RaycastHit.normal);
                if(l_IsValid)
                {
                    _Portal.gameObject.SetActive(true);
                    m_Dummy.SetActive(false);
                }
                else
                {
                    _Portal.gameObject.SetActive(false);
                    ShowDummy(l_RaycastHit.point, l_RaycastHit.normal, _Portal);
                }
            }
            else
            {
                ShowDummy(l_RaycastHit.point, l_RaycastHit.normal, _Portal);
            }
        }
        else
        {
            ShowDummy(l_RaycastHit.point, l_RaycastHit.normal, _Portal);
        }
    }

    void ShowDummy(Vector3 Position, Vector3 Normal, Portal _Portal)
    {
        m_Dummy.SetActive(true);
        m_Dummy.transform.position = Position;
        m_Dummy.transform.rotation = Quaternion.LookRotation(Normal);
        m_Dummy.transform.localScale = _Portal.transform.localScale;
    }

    public void ShootPortal(Portal _Portal)
    {
        m_Dummy.SetActive(false);
        Ray l_Ray = m_PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        _Portal.gameObject.SetActive(false);
        if (Physics.Raycast(l_Ray, out l_RaycastHit, 200.0f, m_ShootLayerMask.value))
        {
            if (l_RaycastHit.collider.tag == "Drawable")
            {
                bool l_IsValid = _Portal.IsValidPosition(l_RaycastHit.point, l_RaycastHit.normal);
                _Portal.gameObject.SetActive(l_IsValid);
                if(l_IsValid)
                {
                    _Portal.m_PlacePortalSound.Play();
                }
            }
        }
    }

    bool CanShootPortal()
    {
        return m_AttachedObject == null;
    }
    
    bool CanAtach()
    {
        return m_AttachedObject == null; ;
    }
    void Detach(float Force)
    {

        //if(m_CurrentAttachedObjectTime >= m_AttachedObjectTime)
        if (m_AttachedObject.transform.parent == m_AttachPositionTransform)
        {
            Rigidbody l_RigidBody = m_AttachedObject.GetComponent<Rigidbody>();
            l_RigidBody.isKinematic = false;
            l_RigidBody.AddForce(m_PlayerCamera.transform.forward * Force);
            m_AttachedObject.transform.SetParent(null);
            switch (m_AttachedObject.tag)
            {
                case "Companion":
                    Companion l_Companion = m_AttachedObject.GetComponent<Companion>();
                    l_Companion.SetTeleportActive(true);
                    break;
                case "Turret":
                    Turret l_turret = m_AttachedObject.GetComponent<Turret>();
                    l_turret.SetTeleportActive(true);
                    break;
            }
            m_AudioSource.PlayOneShot(m_throwSound);
            StartCoroutine(CadenceShootCompanion());
          //  m_AttachedObject = null;
        }
    }    
    IEnumerator CadenceShootCompanion()//Cambiar nombre
    {
        yield return new WaitForSeconds(m_MaxCadence);
        m_AttachedObject = null;
    }
    void Attach ()
    {
        Ray l_Ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, 200.0f, m_ShootLayerMask.value))
        {

            if(l_RaycastHit.collider.tag == "Companion" || l_RaycastHit.collider.tag == "Turret")
            {
                StartAttachObject(l_RaycastHit.collider.gameObject);
                m_AudioSource.PlayOneShot(m_pickSound);
            }
        }
    }

    private void StartAttachObject(GameObject AttachObject)
    {
        if (m_AttachedObject == null)
        {
            m_AttachedObject = AttachObject;
            m_AttachedObject.GetComponent<Rigidbody>().isKinematic = true;
            switch (AttachObject.tag)
            {
                case "Companion":
                    Companion l_Companion = m_AttachedObject.GetComponent<Companion>();
                    l_Companion.SetTeleportActive(false);
                    break;
                case "Turret":
                    Turret l_turret = m_AttachedObject.GetComponent<Turret>();
                    l_turret.SetTeleportActive(false);
                    break;
            }
            

            m_CurrentAttachedObjectTime = 0.0f;
        }

    }
    void UpdateAttachedObject()
    {
        if (m_AttachedObject != null && m_CurrentAttachedObjectTime < m_AttachedObjectTime)
        {
            m_CurrentAttachedObjectTime += Time.deltaTime;
            float l_Pct = Mathf.Min(1.0f, m_CurrentAttachedObjectTime / m_AttachedObjectTime);
            m_AttachedObject.transform.position = Vector3.Lerp(m_AttachedObject.transform.position, m_AttachPositionTransform.position, l_Pct);
            m_AttachedObject.transform.rotation = Quaternion.Lerp(m_AttachedObject.transform.rotation, m_AttachPositionTransform.rotation, l_Pct);
            if (l_Pct == 1.0f)
                m_AttachedObject.transform.SetParent(m_AttachPositionTransform);
        }
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Portal")
            Teleport(other.GetComponent<Portal>());
        if (other.tag == "PortalButton")
            other.GetComponent<PortalButtonSpawner>().Spawn();
        if (other.tag == "Checkpoint")
        {
            SetCurrentCheckpoint(other.gameObject.transform);
        }
            
    }
    public void Teleport(Portal _Portal)
    {
        Vector3 l_PortalForwardXZ = _Portal.transform.forward;
        l_PortalForwardXZ.y = 0.0f;
        l_PortalForwardXZ.Normalize();
        //Miramos el producto escalar entre la - direcion que me muevo y el forward del portal y si es menor de 60 grados permito hacer tp
        float l_Dot = Vector3.Dot(_Portal.transform.forward, -m_Direction);//-1 valores opuestos  tiene que estar entre -5(60 grados) y -1, negamos la dir para trabajar en positivo
        if(l_Dot > m_DotToEnterPortal)
        {

            m_CharacterController.enabled = false;
            //Pasamos cordenadas de mundo al portal que estoy atravesando
            Vector3 l_LocalPosition = _Portal.transform.InverseTransformPoint(transform.position);
            Vector3 l_LocalDirection = _Portal.transform.InverseTransformDirection(-transform.forward);
            //las locales las pasamos a mundo desde el otro portal
            transform.position = _Portal.m_OtherPortal.transform.TransformPoint(l_LocalPosition);
            transform.forward = _Portal.m_OtherPortal.transform.TransformDirection(l_LocalDirection);
            m_CharacterController.enabled = true;
            //Recalculamos el yaw (angulo de la camara en y)
            m_Yaw = transform.rotation.eulerAngles.y;
            Vector3 l_LocalMovementDirection = _Portal.transform.InverseTransformDirection(-m_Direction);//Ponemos en local 
            m_Direction = _Portal.m_OtherPortal.transform.TransformDirection(l_LocalMovementDirection);//Volvemos a pasar a mundo respecto al que salgo
        }

    }
    public bool GetLife()
    {
        return m_IsAlive;
    }
    public void SetLife(bool l_state)
    {
        m_IsAlive = l_state;
        if (m_IsAlive == false)
        {
            GameController.GetGameController().ShowDeathHud();
        }
        else
        {
            GameController.GetGameController().ShowPlayerHud();
        }
    }
    public void Restart()
    {
        m_CharacterController.enabled = false;
        transform.position = m_Checkpoints[m_CurrentCheckpoint].position;

        transform.rotation = m_Checkpoints[m_CurrentCheckpoint].rotation;
        m_CharacterController.enabled = true;

        m_IsAlive = true;
    }
    public void SetCurrentCheckpoint(Transform Checkpoint)
    {
        for (int i = 0; i < m_Checkpoints.Count; i++)
        {
            if (Checkpoint == m_Checkpoints[i])
            {
                m_CurrentCheckpoint = i;
            }
        }
    }
}
