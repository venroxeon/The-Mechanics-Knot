using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnRunMono : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}
