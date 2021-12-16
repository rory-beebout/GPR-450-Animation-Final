using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientToGround : MonoBehaviour
{
    Vector3 rotationOffset;

    private void Start()
    {
        rotationOffset = transform.rotation.eulerAngles;
        //rotationOffset = new Vector3(90,0,0);
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            slopeRotation *= Quaternion.Euler(rotationOffset);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 10 * Time.deltaTime);
        }
    }
}
