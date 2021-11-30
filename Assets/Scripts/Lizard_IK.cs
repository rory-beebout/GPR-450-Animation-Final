using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard_IK : MonoBehaviour
{
    public GameObject neck;

    public GameObject lookat;

    public Vector3 crossUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 towardObjectFromHead = lookat.transform.position - neck.transform.position;
        towardObjectFromHead = Vector3.Cross(transform.up, towardObjectFromHead);
        neck.transform.rotation = Quaternion.LookRotation(towardObjectFromHead, neck.transform.up);
    }
}
