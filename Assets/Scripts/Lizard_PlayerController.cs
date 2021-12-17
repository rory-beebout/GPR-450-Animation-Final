using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationProg
{

    public class Lizard_PlayerController : MonoBehaviour
    {
        Lizard_LegManager legManager;
        public float moveSpeed = 3.0f;

        float step1Dist;
        float step2Dist;
        float step3Dist;
        float step4Dist;

        float step1Over;
        float step2Over;
        float step3Over;
        float step4Over;

        public float idlingHeight;
        private ClipController clipController;

        void Start()
        {
            legManager = transform.GetComponent<Lizard_LegManager>();
            step1Dist = legManager.stepper1.stepDistance;
            step2Dist = legManager.stepper2.stepDistance;
            step3Dist = legManager.stepper3.stepDistance;
            step4Dist = legManager.stepper4.stepDistance;

            step1Over = legManager.stepper1.stepOvershoot;
            step2Over = legManager.stepper2.stepOvershoot;
            step3Over = legManager.stepper3.stepOvershoot;
            step4Over = legManager.stepper4.stepOvershoot;

        }
        private void Awake()
        {
            //InitAnim();
        }
        void InitAnim()
        {
            Keyframe[] keyframes = new Keyframe[4];
            keyframes[0] = new Keyframe(0.0f, idlingHeight, 1);
            keyframes[1] = new Keyframe(idlingHeight, idlingHeight * 2, 2);
            keyframes[2] = new Keyframe(0.0f, idlingHeight, 3);
            keyframes[3] = new Keyframe(idlingHeight, idlingHeight * 2, 4);
            KeyframePool keyframePool = new KeyframePool(keyframes);
            Clip idleClip = new Clip("Idle", keyframePool, 1, 4, new ClipTransition(1), new ClipTransition(-1));
            ClipPool clipPool = new ClipPool(idleClip);
            clipController = new ClipController("Lizard Idle", clipPool, 0);
        }
        // Update is called once per frame
        void Update()
        {
            clipController.Update(Time.deltaTime);
            RaycastHit hit;
            int layerMask = 1 << 4;
            layerMask = ~layerMask;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 4f, layerMask))
            {
                Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 10 * Time.deltaTime);
            }

            RaycastHit hit2;
            int layerMask2 = 1 << 2;
            layerMask2 = ~layerMask2;
            Vector3 offset = transform.forward * 0.5f;
            if (Physics.Raycast(transform.position + (transform.up * 1f) + offset, -transform.up, out hit2, 500f, layerMask2))
            {
                transform.position = hit2.point + (transform.up * 0.2f) - offset;
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
                transform.rotation *= Quaternion.Euler(0, 55 * Time.deltaTime, 0);
                HalfStepDist();
            }
            else
            {
                RestoreStepDist();
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.rotation *= Quaternion.Euler(0, -55 * Time.deltaTime, 0);
                HalfStepDist();
            }
            else
            {
                RestoreStepDist();
            }
        }
        void HalfStepDist()
        {
            legManager.stepper1.stepDistance = step1Dist * 0.5f;
            legManager.stepper2.stepDistance = step2Dist * 0.5f;
            legManager.stepper3.stepDistance = step3Dist * 0.5f;
            legManager.stepper4.stepDistance = step4Dist * 0.5f;

            legManager.stepper1.stepOvershoot = 0;
            legManager.stepper2.stepOvershoot = 0;
            legManager.stepper3.stepOvershoot = 0;
            legManager.stepper4.stepOvershoot = 0;
        }
        void RestoreStepDist()
        {
            legManager.stepper1.stepDistance = step1Dist;
            legManager.stepper2.stepDistance = step2Dist;
            legManager.stepper3.stepDistance = step3Dist;
            legManager.stepper4.stepDistance = step4Dist;

            legManager.stepper1.stepOvershoot = step1Over;
            legManager.stepper2.stepOvershoot = step2Over;
            legManager.stepper3.stepOvershoot = step3Over;
            legManager.stepper4.stepOvershoot = step4Over;
        }
    }
}