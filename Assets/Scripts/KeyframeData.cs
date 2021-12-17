using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AnimationProg
{ 
    // single keyframe = a moment in time
    public class Keyframe
    {
        public int index;

        public float _duration, _durationInv, time;

        public int data;

        public Keyframe(float start, float end, int value_x)
        {
            data = value_x;
            time = start;
            _duration = end - start;
            _durationInv = 1.0f / _duration;
        }
    };

    // collection of keyframes
    public class KeyframePool
    {
        public Keyframe[] _keyframe;
        public int _count;

        public KeyframePool(params Keyframe[] keyframes)
        {
            _keyframe = keyframes;
            for(int i = 0; i < keyframes.Length; i++)
            {
                keyframes[i].index = i;
            }
            _count = keyframes.Length;
        }

        public Keyframe GetKeyframe(int i)
        {
            return _keyframe[i];
        }
        public Keyframe this[int i]
        {
            get { return _keyframe[i]; }
            set { _keyframe[i] = value; }
        }
    }

    // single clip = timeline
    public class Clip
    {
        public string _name;

        public int index;

        public float duration, durationInv;

        public int keyframeCount;

        public int first_keyframe;

        public int last_keyframe;

        public ClipTransition _forwardTransition;
        public ClipTransition _reverseTransition;

        public KeyframePool framePool;

        public Clip(string name, KeyframePool keyframePool, int firstKeyframeIndex, int lastKeyframeIndex, ClipTransition forwardTransition, ClipTransition reverseTransition)
        {
            _name = name;

            framePool = keyframePool;

            first_keyframe = firstKeyframeIndex;
            last_keyframe = lastKeyframeIndex;

            for (int i = firstKeyframeIndex; i <= lastKeyframeIndex; i++)
            {
                duration += framePool[i]._duration;
            }
            durationInv = 1 / duration;

            _forwardTransition = forwardTransition;
            _reverseTransition = reverseTransition;
        }
        public Keyframe GetKeyframe(int i)
        {
            return framePool[i];
        }

        public Keyframe this[int i]
        {
            get { return framePool[i]; }
        }
    }

    // collection of clips
    public class ClipPool
    {
        public Clip[] clip;

        public int count;

        public ClipPool(params Clip[] clips)
        {
            count = clips.Length;
            clip = clips;
            for(int i = 0; i < clips.Length; i++)
            {
                clip[i].index = i;
            }
        }
        public int GetClipIndexByName(string name)
        {
            for (int i = 0; i < count; i++)
            {
                if (clip[i]._name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public Clip GetClipByName(string name)
        {
            return clip[GetClipIndexByName(name)];
        }

        public Clip this[int i]
        {
            get { return clip[i]; }
            set { clip[i] = value; }
        }
    }

    // transition between clips in different directions
    public class ClipTransition
    {
        public ClipPool[] clipPool;

        public int clipIndex;

        public float clipTime;

        public float playbackDirection;

        public string _transitionName;

        public ClipTransition(float playbackSpeed, string clipName = null)
        {
            playbackDirection = playbackSpeed;
            _transitionName = clipName;
        }

        public void DoTransition(ClipPool clipPool, ref int clipIndex, ref int keyframeIndex, ref float keyframeTime, ref float playbackDirection)
        {
            Clip nextClip = GetNextClip(clipPool, _transitionName);
            if(nextClip == null)
            {
                nextClip = clipPool[clipIndex];
            }

            float bounds = Mathf.Abs(keyframeTime);

            switch(playbackDirection)
            {
                case 0f:
                    switch(playbackDirection)
                    {
                        case 1f:
                            keyframeIndex = nextClip.last_keyframe;
                            keyframeTime = nextClip[keyframeIndex]._duration;
                            break;
                        case -1f:
                            keyframeIndex = nextClip.first_keyframe;
                            keyframeTime = 0f;
                            break;
                        case 0f:
                            throw new System.Exception("A paused clip shouldn't be able to transition...");// can't transition
                        default:
                            throw new System.ArgumentOutOfRangeException(nameof(ClipTransition.playbackDirection), this.playbackDirection, null);
                    }
                    playbackDirection = 0f;
                    break;
                case 1f:
                    clipIndex = nextClip.index;
                    keyframeIndex = nextClip.first_keyframe;
                    keyframeTime = bounds;
                    break;
                case -1f:
                    clipIndex = nextClip.index;
                    keyframeIndex = nextClip.last_keyframe;
                    keyframeTime = nextClip[keyframeIndex]._duration - bounds;
                    playbackDirection = -1f;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        private Clip GetNextClip(ClipPool clipPool, string transitionName)
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                return null;
            }
            Clip clip = null;
            int index = clipPool.GetClipIndexByName(transitionName);
            if (index >= 0)
            {
                clip = clipPool[index];
            }

            return clip;
        }
    }
}
