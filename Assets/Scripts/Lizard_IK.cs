using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Leg
{
    public GameObject endConstraint, poleConstraint, shoulder, elbow, foot;
    public Vector3 shoulderRotationOffset, elbowRotationOffset, footRotationOffset;
    public float shoulderToElbowLength, elbowToWristLength;

    //public Leg(GameObject end, GameObject pole, GameObject theShoulder, GameObject theElbow, GameObject theFoot)
    //{
    //    endConstraint = end;
    //    poleConstraint = pole;
    //    shoulder = theShoulder;
    //    elbow = theElbow;
    //    foot = theFoot;
    //    shoulderRotationOffset = theShoulder.transform.rotation.eulerAngles;
    //    elbowRotationOffset = theElbow.transform.rotation.eulerAngles;
    //    footRotationOffset = theFoot.transform.rotation.eulerAngles;
    //}
}
public class Lizard_IK : MonoBehaviour
{
    //-----------------------------------------------------------------------------------------------------------------------------
    // Neck vars
    public GameObject neck;

    public GameObject lookat;
    private Quaternion targetRot;
    private Vector3 neckOffset;
    //-----------------------------------------------------------------------------------------------------------------------------
    // Leg vars



    public Leg leg1 = new Leg();
    public Leg leg2 = new Leg();
    public Leg leg3 = new Leg();
    public Leg leg4 = new Leg();

    //-----------------------------------------------------------------------------------------------------------------------------
    // Tail vars
    public struct RopeSegment
    {
        public Vector3 currentPos;
        public Vector3 oldPos;
        public float segmentLength;
        public GameObject bone;
        public Vector3 boneRotationOffset;

        public RopeSegment(Vector3 pos, float length, GameObject currentBone)
        {
            bone = currentBone;
            boneRotationOffset = currentBone.transform.rotation.eulerAngles;
            currentPos = pos;
            oldPos = pos;
            segmentLength = length;
        }
    }

    public GameObject tail_BaseBone;
    private List<RopeSegment> tailSegments = new List<RopeSegment>();
    //-----------------------------------------------------------------------------------------------------------------------------


    public float headFollowSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        initializeNeck();

        initializeLeg(leg1);
        initializeLeg(leg2);
        initializeLeg(leg3);
        initializeLeg(leg4);

        initializeTail();

        gameObject.GetComponent<Lizard_LegManager>().init();
    }
    private void Awake()
    {
        //gameObject.GetComponent<Animator>().enabled = true;
    }

    void initializeNeck()
    {
        neckOffset = neck.transform.rotation.eulerAngles;
    }
    private void initializeLeg(Leg leg)
    {
        //leg.shoulderRotationOffset = leg.shoulder.transform.rotation.eulerAngles;
        leg.shoulderRotationOffset = new Vector3(90, 0, 0);
        //leg.elbowRotationOffset = leg.elbow.transform.rotation.eulerAngles;
        leg.elbowRotationOffset = new Vector3(90, 0, 0);
        leg.footRotationOffset = leg.foot.transform.rotation.eulerAngles;

        leg.shoulderToElbowLength = Vector3.Distance(leg.elbow.transform.position, leg.shoulder.transform.position);
        leg.elbowToWristLength = Vector3.Distance(leg.foot.transform.position, leg.elbow.transform.position);
    }

    private void initializeTail()
    {
        GameObject currentBone = tail_BaseBone;
        while (currentBone.transform.childCount > 0)
        {
            tailSegments.Add(new RopeSegment(currentBone.transform.position, Vector3.Distance(currentBone.transform.position, currentBone.transform.GetChild(0).position), currentBone));
            currentBone = currentBone.transform.GetChild(0).gameObject;
        }
        tailSegments.Add(new RopeSegment(currentBone.transform.position, Vector3.Distance(currentBone.transform.position, currentBone.transform.position), currentBone));
    }
    private void LateUpdate()
    //void OnAnimatorIK()
    {
        solveNeck();

        solveLeg(leg1);
        solveLeg(leg2);
        solveLeg(leg3);
        solveLeg(leg4);

        solveTail();
    }

    private void solveNeck()
    {
        Vector3 towardObjectFromHead = lookat.transform.position - neck.transform.position;
        targetRot = Quaternion.LookRotation(towardObjectFromHead, transform.up);
        targetRot *= Quaternion.Euler(neckOffset);
        neck.transform.rotation = Quaternion.Slerp(neck.transform.rotation, targetRot, headFollowSpeed * Time.deltaTime);
    }

    private void solveLeg(Leg leg)
    {
        // get shoulder-to-constraint vector
        Vector3 shoulderToFoot = leg.endConstraint.transform.position - leg.shoulder.transform.position;
        Vector3 shoulderToElbow = leg.poleConstraint.transform.position - leg.shoulder.transform.position;

        float baseToEndLength = shoulderToFoot.magnitude;
        Vector3 elbowPos;


        if (baseToEndLength > leg.shoulderToElbowLength + leg.elbowToWristLength)
        {
            shoulderToFoot.Normalize();
            elbowPos = (shoulderToFoot * leg.shoulderToElbowLength) + leg.shoulder.transform.position;

            Quaternion targetShoulderRot = Quaternion.LookRotation(elbowPos - leg.shoulder.transform.position, leg.shoulder.transform.up);
            targetShoulderRot *= Quaternion.Euler(leg.shoulderRotationOffset);
            leg.shoulder.transform.rotation = targetShoulderRot;

            Quaternion targetElbowRot = Quaternion.LookRotation(shoulderToFoot, leg.elbow.transform.up);
            targetElbowRot *= Quaternion.Euler(leg.elbowRotationOffset);
            leg.elbow.transform.rotation = targetElbowRot;

            Quaternion targetFootRot = leg.endConstraint.transform.rotation;
            targetFootRot *= Quaternion.Euler(leg.footRotationOffset);
            leg.foot.transform.rotation = targetFootRot;


            leg.elbow.transform.position = elbowPos;
            leg.foot.transform.position = (elbowPos) + (shoulderToFoot * leg1.elbowToWristLength);
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
            float s = 0.5f * (baseToEndLength + leg.shoulderToElbowLength + leg.elbowToWristLength);
            float triangleArea = Mathf.Sqrt(s * (s - baseToEndLength) * (s - leg.shoulderToElbowLength) * (s - leg.elbowToWristLength));
            float triangleHeight = 2 * triangleArea / baseToEndLength;
            float triangleBaseLength = Mathf.Sqrt((leg.shoulderToElbowLength * leg.shoulderToElbowLength) - (triangleHeight * triangleHeight));

            shoulderToFoot *= triangleBaseLength;
            triangleUp *= triangleHeight;
            elbowPos = triangleUp + shoulderToFoot + leg.shoulder.transform.position;
            
            Quaternion targetShoulderRot = Quaternion.LookRotation(elbowPos - leg.shoulder.transform.position, leg.shoulder.transform.up);
            targetShoulderRot *= Quaternion.Euler(leg.shoulderRotationOffset);
            leg.shoulder.transform.rotation = targetShoulderRot;

            leg.elbow.transform.position = elbowPos;
            Vector3 towardFoot = leg.endConstraint.transform.position - leg.elbow.transform.position;
            Quaternion targetElbowRot = Quaternion.LookRotation(towardFoot, leg.elbow.transform.up);
            targetElbowRot *= Quaternion.Euler(leg.elbowRotationOffset);
            leg.elbow.transform.rotation = targetElbowRot;

            leg.foot.transform.position = leg.endConstraint.transform.position;
            Quaternion targetFootRot = leg.endConstraint.transform.rotation;
            targetFootRot *= Quaternion.Euler(leg.footRotationOffset);
            leg.foot.transform.rotation = targetFootRot;
        }
    }


    private void solveTail()
    {
        // Snap tail back
        Vector3 gravity = 2f*-transform.forward;

        for (int i = 0; i < tailSegments.Count; i++)
        {
            RopeSegment segment1 = tailSegments[i];

            Vector3 newPos = segment1.currentPos + (segment1.currentPos - segment1.oldPos) + (gravity * Time.fixedDeltaTime);
            segment1.oldPos = segment1.currentPos;
            segment1.currentPos = newPos;

            tailSegments[i] = segment1;
        }

        // Apply constraints
        for (int i = 0; i < 16; i++)
        {
            ApplyTailConstraints();
        }

        // Apply transformations
        for (int i = 0; i < tailSegments.Count - 1; i++)
        {
            tailSegments[i].bone.transform.position = tailSegments[i].currentPos;
            Vector3 towardParent = tailSegments[i].bone.transform.parent.position - tailSegments[i].bone.transform.position;
            Quaternion targetRot = Quaternion.LookRotation(towardParent, transform.up);
            targetRot *= Quaternion.Euler(tailSegments[i].boneRotationOffset);
            tailSegments[i].bone.transform.rotation = targetRot;
        }
    }

    private void ApplyTailConstraints()
    {
        // Because structs are copy-based, we do this assignment in a roundabout way
        RopeSegment startSegment = tailSegments[0];
        startSegment.currentPos = tail_BaseBone.transform.position;
        tailSegments[0] = startSegment;

        // for all segments
        for (int i = 0; i < tailSegments.Count - 1; i++)
        {
            RopeSegment segment1 = tailSegments[i];
            RopeSegment segment2 = tailSegments[i + 1];

            // Collision
            Collider[] collisions = Physics.OverlapSphere(segment1.currentPos, 0.4f);
            for (int j = 0; j < collisions.Length; j++)
            {
                if (collisions[j].transform.root.name != "Iguana")
                {
                    RaycastHit hit;
                    int layerMask = 1 << 2;
                    layerMask = ~layerMask;
                    if (Physics.Raycast(segment1.currentPos, collisions[j].ClosestPoint(segment1.currentPos) - segment1.currentPos, out hit, 2f, layerMask))
                    {
                        segment1.currentPos = collisions[j].ClosestPoint(segment1.currentPos) + (hit.normal*0.4f);
                    }
                }
            }

            // actual rope behavior constraints
            float dist = (segment1.currentPos - segment2.currentPos).magnitude;
            float error = dist - tailSegments[i].segmentLength;
            Vector3 changeDir = (segment1.currentPos - segment2.currentPos).normalized;
            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                segment1.currentPos -= changeAmount * 0.5f;
                tailSegments[i] = segment1;
                segment2.currentPos += changeAmount * 0.5f;
                tailSegments[i + 1] = segment2;
            }
            else
            {
                segment2.currentPos += changeAmount;
                tailSegments[i + 1] = segment2;
            }
        }
    }
}
