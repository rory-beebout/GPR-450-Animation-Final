using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard_IK : MonoBehaviour
{
    public GameObject neck;

    public GameObject lookat;
    private Quaternion targetRot;

    public GameObject leg1_footConstraint, leg1_elbowConstraint, leg1_shoulder, leg1_elbow, leg1_foot;

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

        solveLeg(leg1_footConstraint, leg1_elbowConstraint, leg1_shoulder, leg1_elbow, leg1_foot);
    }

    private void solveNeck()
    {
        Vector3 towardObjectFromHead = lookat.transform.position - neck.transform.position;
        targetRot = Quaternion.LookRotation(towardObjectFromHead, transform.up);
        neck.transform.rotation = Quaternion.Slerp(neck.transform.rotation, targetRot, headFollowSpeed * Time.deltaTime);
    }

    private void solveLeg(GameObject footTarget, GameObject elbowConstraint, GameObject shoulder, GameObject elbow, GameObject foot)
    {
        // get shoulder-to-constraint vector
        // a3real3Diff(baseToConstraint.v, controlLocator_wristConstraint.v, jointTransform_shoulder.v3.v);
        Vector3 shoulderToFoot = footTarget.transform.position - shoulder.transform.position;
        Vector3 shoulderToElbow = elbowConstraint.transform.position - shoulder.transform.position;

        float sideLength1 = shoulderToElbow.magnitude;
        float sideLength2 = (elbowConstraint.transform.position - footTarget.transform.position).magnitude;
        sideLength1 = 0.6f;
        sideLength2 = 0.6f;
        float baseToEndLength = shoulderToFoot.magnitude;
        Vector3 elbowPos;


        if (baseToEndLength > sideLength1 + sideLength2)
        {
            elbow.transform.rotation = Quaternion.LookRotation(shoulderToFoot, transform.up);
            foot.transform.position = shoulderToFoot*sideLength2;
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
            elbow.transform.position = shoulder.transform.position + elbowPos;
            foot.transform.position = footTarget.transform.position;
            foot.transform.rotation = footTarget.transform.rotation;
            //elbow.transform.rotation = Quaternion.LookRotation(elbow.transform.position - foot.transform.position, transform.up);
            elbow.transform.rotation = Quaternion.LookRotation(foot.transform.position - elbow.transform.position, transform.up);
            //shoulder.transform.rotation = Quaternion.LookRotation(shoulder.transform.position - elbowPos, transform.up);
            shoulder.transform.rotation = Quaternion.LookRotation(elbowPos - shoulder.transform.position, transform.up);

        }
    }
}
