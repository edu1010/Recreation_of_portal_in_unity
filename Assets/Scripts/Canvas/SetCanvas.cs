using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvas : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake()
    {
        GameController.GetGameController().SetCanvas(gameObject);
    }
}
