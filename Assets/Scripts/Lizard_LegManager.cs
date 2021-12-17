using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard_LegManager : MonoBehaviour
{
    [System.Serializable]
    public class Stepper
    {
        public GameObject foot;
        public GameObject effector;
        public Transform home;
        public float stepDistance = 0.3f;
        public float stepOvershoot = 0.6f;
        public float stepDuration = 0.1f;
        public bool moving = false;
    }

    public Stepper stepper1 = new Stepper();
    public Stepper stepper2 = new Stepper();
    public Stepper stepper3 = new Stepper();
    public Stepper stepper4 = new Stepper();


    bool oneSteppedLast = true;
    

    // Start is called before the first frame update
    public void init()
    {
        Lizard_IK IK = gameObject.GetComponent<Lizard_IK>();
        if (IK)
        {
            stepper1.effector = IK.leg1.endConstraint;
            stepper1.foot = IK.leg1.foot;
            stepper1.home = new GameObject().transform;
            stepper1.home.SetParent(IK.leg1.shoulder.transform.root);
            stepper1.home.transform.position = stepper1.foot.transform.position;
            //stepper1.home.transform.rotation = stepper1.foot.transform.rotation;

            stepper2.effector = IK.leg2.endConstraint;
            stepper2.foot = IK.leg2.foot;
            stepper2.home = new GameObject().transform;
            stepper2.home.SetParent(IK.leg2.shoulder.transform.root);
            stepper2.home.transform.position = stepper2.foot.transform.position;
            //stepper2.home.transform.rotation = stepper2.foot.transform.rotation;

            stepper3.effector = IK.leg3.endConstraint;
            stepper3.foot = IK.leg3.foot;
            stepper3.home = new GameObject().transform;
            stepper3.home.SetParent(IK.leg3.shoulder.transform.root);
            stepper3.home.transform.position = stepper3.foot.transform.position;
            //stepper3.home.transform.rotation = stepper3.foot.transform.rotation;

            stepper4.effector = IK.leg4.endConstraint;
            stepper4.foot = IK.leg4.foot;
            stepper4.home = new GameObject().transform;
            stepper4.home.SetParent(IK.leg4.shoulder.transform.root);
            stepper4.home.transform.position = stepper4.foot.transform.position;
            //stepper4.home.transform.rotation = stepper1.foot.transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (oneSteppedLast && (!stepper1.moving && !stepper4.moving))
        {
            Step(stepper2);
            Step(stepper3);
            oneSteppedLast = false;
        }
        if (!oneSteppedLast && (!stepper2.moving && !stepper3.moving))
        {
            Step(stepper1);
            Step(stepper4);
            oneSteppedLast = true;
        }
       
    }
    void Step(Stepper stepper)
    {
        if (stepper.moving)
        {
            return;
        }
        float distFromHome = Vector3.Distance(stepper.foot.transform.position, stepper.home.position);
        if (distFromHome > stepper.stepDistance)
        {
            StartCoroutine(MoveToHome(stepper));
        }
    }
    IEnumerator MoveToHome(Stepper stepper)
    {
        stepper.moving = true;

        Quaternion startRot = stepper.effector.transform.rotation;
        Vector3 start = stepper.effector.transform.position;

        Quaternion endRot = stepper.home.rotation;
        Vector3 end = stepper.home.position + (stepper.home.forward * stepper.stepOvershoot);

        Vector3 centerPoint = (start + end) / 2;

        RaycastHit hit;
        int layerMask = 1 << 2;
        layerMask = ~layerMask;
        if (Physics.Raycast(stepper.foot.transform.parent.parent.parent.transform.position, stepper.effector.transform.position - stepper.foot.transform.parent.parent.parent.transform.position, out hit, 1f, layerMask))
        {
            end = hit.point + (hit.normal * 0.04f);
            endRot = stepper.effector.transform.rotation;
            endRot *= Quaternion.FromToRotation(stepper.effector.transform.up, hit.normal);
        }

        float timeElapsed = 0;

        while (timeElapsed < stepper.stepDuration)
        {
            timeElapsed += Time.deltaTime;
            float param = timeElapsed / stepper.stepDuration;

            // Interpolate position and rotation
            stepper.effector.transform.position = Vector3.Lerp(Vector3.Lerp(start, centerPoint, param), Vector3.Lerp(centerPoint, start, param), param);
            stepper.effector.transform.rotation = Quaternion.Slerp(startRot, endRot, param);

            yield return null;
        }
        stepper.effector.transform.position = end;

        stepper.moving = false;
    }
}
