using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationProg
{
    // clip controller = the animation playhead
    public class ClipController
    {
        public ClipPool clipPool;
        public string name;

        public int clipIndex;
        public float clipTime;
        public float clipParam;

        public int keyframeIndex;
        public float keyframeTime;
        public float keyframeParam;

        public float playbackSpeed;
        public float playbackDirection;

        public ClipController(string name, ClipPool pool, int initClip)
        {
            this.name = name;
            clipPool = pool;

            clipIndex = initClip;
            clipTime = 0f;
            clipParam = 0f;

            keyframeIndex = clipPool[clipIndex].first_keyframe;
            keyframeTime = 0f;
            keyframeParam = 0f;

            playbackSpeed = 1f;
            playbackDirection = 1f;
        }

        public ClipController(string name, ClipPool pool, string initClip) : this(name, pool, pool.GetClipIndexByName(initClip)) { }

        // update the referenced controller over time
        public void Update(float dt)
        {
            // break if playback speed is equal to 0 (paused)
            if (playbackDirection == 0 ||playbackSpeed <= 0f)
            {
                return;
            }
            // increment clip and keyframe times
            float timeStep = dt * playbackSpeed * playbackDirection;
            keyframeTime += timeStep;
            // positive playback direction (playing)
            if (playbackDirection == 1f)
            {
                if (keyframeTime >= clipPool[clipIndex][keyframeIndex]._duration)
                {
                    // resolve keyframe going over duration by reversing direction
                    keyframeTime -= clipPool[clipIndex][keyframeIndex]._duration;
                    // resolve the last keyframe
                    if (keyframeIndex >= clipPool[clipIndex].last_keyframe)
                    {
                        clipPool[clipIndex]._forwardTransition.DoTransition(clipPool, ref clipIndex, ref keyframeIndex, ref keyframeTime, ref playbackDirection);
                    }
                    // continue playing
                    else
                    {
                        keyframeIndex++;
                    }
                }
            }
            else if (playbackDirection == -1)
            {
                if (keyframeTime < 0)
                {
                    // resolve keyframe going past the initial keyframe
                    if (keyframeIndex <= clipPool[clipIndex].first_keyframe)
                    {
                        clipPool[clipIndex]._reverseTransition.DoTransition(clipPool, ref clipIndex, ref keyframeIndex, ref keyframeTime, ref playbackDirection);
                    }
                    // reverse playing direction
                    else
                    {
                        keyframeIndex--;
                        keyframeTime = clipPool[clipIndex][keyframeIndex]._duration - keyframeTime;
                    }
                }
            }
            // normalize parameters
            keyframeParam = keyframeTime * clipPool[clipIndex][keyframeIndex]._durationInv;

            // subtract start time of first keyframe from current keyframe and add current keyframe time to get the clip time
            clipTime = clipPool[clipIndex][keyframeIndex].time - clipPool[clipIndex][clipPool[clipIndex].first_keyframe].time + keyframeTime;
            clipParam = clipTime * clipPool[clipIndex].durationInv;
        }
        public Clip GetCurrentClip()
        {
            return clipPool[clipIndex];
        }

        public Keyframe GetCurrentKeyframe()
        {
            return GetCurrentClip().GetKeyframe(keyframeIndex);
        }
        public void SetCurrentClip(int index)
        {
            clipIndex = index;
            keyframeIndex = clipPool[clipIndex].first_keyframe;
            keyframeTime = 0;
        }

        public void SetCurrentClip(string clipName)
        {
            SetCurrentClip(clipPool.GetClipIndexByName(clipName));
        }

        public void GoToNextClip()
        {
            int index = clipIndex + 1;
            if (index > clipPool.count - 1)
            {
                index = 0;
            }
            SetCurrentClip(index);
        }

        public void GoToPrevClip()
        {
            int index = clipIndex - 1;
            if (index < 0)
            {
                index = clipPool.count - 1;
            }
            SetCurrentClip(index);
        }
    }
}