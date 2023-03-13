using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalButtonSpawner : PortalButton
{
    public Transform m_SpawnPosition;
    public GameObject m_CompanionPrefab;
    AudioSource m_AudioPortalButton;

    private void Start()
    {
        m_AudioPortalButton = GetComponent<AudioSource>();
    }

    public void Spawn()
    {
        GameObject l_GameObject = Instantiate(m_CompanionPrefab);
        l_GameObject.transform.position = m_SpawnPosition.position;
        l_GameObject.transform.rotation = m_SpawnPosition.rotation;
        m_AudioPortalButton.Play();
    }
}
