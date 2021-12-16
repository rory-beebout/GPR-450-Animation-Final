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
        public float stepDuration = 0.1f;
        public bool moving = false;
    }

    public Stepper stepper1 = new Stepper();
    public Stepper stepper2 = new Stepper();
    public Stepper stepper3 = new Stepper();
    public Stepper stepper4 = new Stepper();

    

    // Start is called before the first frame update
    public void init()
    {
        Lizard_IK IK = gameObject.GetComponent<Lizard_IK>();
        if (IK)
        {
            stepper1.effector = IK.leg1.endConstraint;
            stepper1.foot = IK.leg1.foot;
            stepper1.home = new GameObject().transform;
            stepper1.home.SetParent(IK.leg1.shoulder.transform.parent);
            stepper1.home.transform.position = stepper1.foot.transform.position;
            //stepper1.home.transform.rotation = stepper1.foot.transform.rotation;

            stepper2.effector = IK.leg2.endConstraint;
            stepper2.foot = IK.leg2.foot;
            stepper2.home = new GameObject().transform;
            stepper2.home.SetParent(IK.leg2.shoulder.transform.parent);
            stepper2.home.transform.position = stepper2.foot.transform.position;
            //stepper2.home.transform.rotation = stepper2.foot.transform.rotation;

            stepper3.effector = IK.leg3.endConstraint;
            stepper3.foot = IK.leg3.foot;
            stepper3.home = new GameObject().transform;
            stepper3.home.SetParent(IK.leg3.shoulder.transform.parent);
            stepper3.home.transform.position = stepper3.foot.transform.position;
            //stepper3.home.transform.rotation = stepper3.foot.transform.rotation;

            stepper4.effector = IK.leg4.endConstraint;
            stepper4.foot = IK.leg4.foot;
            stepper4.home = new GameObject().transform;
            stepper4.home.SetParent(IK.leg4.shoulder.transform.parent);
            stepper4.home.transform.position = stepper4.foot.transform.position;
            //stepper4.home.transform.rotation = stepper1.foot.transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Step(stepper1);
        Step(stepper2);
        Step(stepper3);
        Step(stepper4);
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
        Vector3 startPoint = stepper.effector.transform.position;

        Quaternion endRot = stepper.home.rotation;
        Vector3 endPoint = stepper.home.position;

        Collider[] collisions = Physics.OverlapSphere(stepper.home.position, 0.25f);
        for (int j = 0; j < collisions.Length; j++)
        {
            if (collisions[j].transform.root.name != "Iguana")
            {
                RaycastHit hit;
                int layerMask = 1 << 2;
                layerMask = ~layerMask;
                if (Physics.Raycast(stepper.foot.transform.parent.parent.parent.transform.position, collisions[j].ClosestPoint(stepper.home.position) - stepper.foot.transform.parent.parent.parent.transform.position, out hit, 2f, layerMask))
                {
                    endPoint = hit.point + (hit.normal * 0.04f);
                    endRot = Quaternion.FromToRotation(stepper.effector.transform.up, hit.normal);
                }
            }
        }


        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / stepper.stepDuration;

            // Interpolate position and rotation
            stepper.effector.transform.position = Vector3.Lerp(startPoint, endPoint, normalizedTime);
            stepper.effector.transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < stepper.stepDuration);

        stepper.moving = false;
    }
}
