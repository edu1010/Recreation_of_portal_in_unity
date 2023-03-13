using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalButtonDoors : PortalButton
{
    public GameObject m_Door;
    Animator m_DoorAnimator;
    AudioSource m_AudioPortalButton;

    private void Start()
    {
        m_DoorAnimator = m_Door.GetComponent<Animator>();
        m_AudioPortalButton = GetComponent<AudioSource>();
    }

    public void OpenDoor()
    {
        m_DoorAnimator.SetBool("Open", true);
        m_AudioPortalButton.Play();
    }

    public void CloseDoor()
    {
        m_DoorAnimator.SetBool("Open", false);
    }
}
