using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Ontriggerenter {name}");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"OnTriggerExit  {name}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"OnCollisonEnter {name}");
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log($"OncollisionExit {name}");
    }
}
