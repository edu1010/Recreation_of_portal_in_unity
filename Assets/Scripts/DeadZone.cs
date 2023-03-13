using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        {
            if (other.GetComponent<FPSController>().GetLife())
            {
                other.GetComponent<FPSController>().SetLife(false);
            }
            
        }
        else if(other.tag == "Companion" || other.tag == "Turret")
        {
            other.gameObject.SetActive(false);
        }
    }
}
