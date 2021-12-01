using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard_IK : MonoBehaviour
{
    public GameObject neck;

    public GameObject lookat;

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
        solveNeck();
    }

    private void solveNeck()
    {
        Vector3 towardObjectFromHead = lookat.transform.position - neck.transform.position;
        neck.transform.rotation = Quaternion.LookRotation(towardObjectFromHead, transform.up);
    }
}
