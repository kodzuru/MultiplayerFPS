using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoadingImage : MonoBehaviour
{
    Vector3 rotationEuler;
    [SerializeField]
    float rotationSpeed = 360f;


    // Update is called once per frame
    void Update ()
    {
        rotationEuler += Vector3.forward * rotationSpeed * Time.deltaTime; //increment 30 degrees every second
        transform.rotation = Quaternion.Euler(rotationEuler);
    }
}
