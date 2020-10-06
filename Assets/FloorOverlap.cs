using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorOverlap : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
    }
}
