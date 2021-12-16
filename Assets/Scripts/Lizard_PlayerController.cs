using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard_PlayerController : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 10 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation *= Quaternion.Euler(0,55 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation *= Quaternion.Euler(0, -55 * Time.deltaTime, 0);
        }
    }
}
