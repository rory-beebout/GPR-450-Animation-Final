using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard_IK : MonoBehaviour
{
    public GameObject neck;

    public GameObject lookat;
    private Quaternion targetRot;

    public GameObject leg1_footTarget, leg1_ElbowConstraint, leg1_Shoulder;

    public float headFollowSpeed = 1f;

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

        solveLeg(leg1_footTarget, leg1_Shoulder, leg1_ElbowConstraint);
    }

    private void solveNeck()
    {
        Vector3 towardObjectFromHead = lookat.transform.position - neck.transform.position;
        targetRot = Quaternion.LookRotation(towardObjectFromHead, transform.up);
        neck.transform.rotation = Quaternion.Slerp(neck.transform.rotation, targetRot, headFollowSpeed * Time.deltaTime);
    }

    private void solveLeg(GameObject footTarget, GameObject shoulder, GameObject elbowConstraint)
    {
        // get shoulder-to-constraint vector
        // a3real3Diff(baseToConstraint.v, controlLocator_wristConstraint.v, jointTransform_shoulder.v3.v);
        Vector3 shoulderToFoot = footTarget.transform.position - shoulder.transform.position;
        Vector3 shoulderToElbow = shoulder.transform.GetChild(0).position - shoulder.transform.position;

        float sideLength1 = shoulderToElbow.magnitude;
        float sideLength2 = (elbowConstraint.transform.position - footTarget.transform.position).magnitude;
        sideLength1 = 0.25f;
        sideLength2 = 0.25f;
        float baseToEndLength = shoulderToFoot.magnitude;
        Vector3 elbowPos;


        if (baseToEndLength > sideLength1 + sideLength2)
        {
            shoulder.transform.GetChild(0).transform.rotation = Quaternion.LookRotation(shoulderToFoot, transform.up);
            shoulder.transform.GetChild(0).transform.GetChild(0).transform.position += shoulderToFoot;
        }
        else
        {
            // calculate triangle plane and triangle "up" direction
            Vector3 planeNormal, triangleUp;
            shoulderToFoot.Normalize();
            shoulderToElbow.Normalize();
            planeNormal = Vector3.Cross(shoulderToElbow, shoulderToFoot);
            triangleUp = Vector3.Cross(shoulderToFoot, planeNormal);

            // Use Heron's formula to calculate triangle area, then solve for height, then base-length
            float s = 0.5f * (baseToEndLength + sideLength1 + sideLength2);
            float triangleArea = Mathf.Sqrt(s * (s - baseToEndLength) * (s - sideLength1) * (s - sideLength2));
            float triangleHeight = 2 * triangleArea / baseToEndLength;
            float triangleBaseLength = Mathf.Sqrt((sideLength1 * sideLength1) - (triangleHeight * triangleHeight));

            shoulderToFoot *= triangleBaseLength;

            triangleUp *= triangleHeight;

            elbowPos = triangleUp + shoulderToFoot;// + shoulder.transform.position;
            shoulder.transform.GetChild(0).transform.position = shoulder.transform.position + elbowPos;
            shoulder.transform.GetChild(0).transform.GetChild(0).transform.position = footTarget.transform.position;
            shoulder.transform.rotation = Quaternion.LookRotation(shoulder.transform.position - elbowPos, transform.up);
        }
    }
}
