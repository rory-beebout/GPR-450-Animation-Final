using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class KeyframeData
{
    enum constant
    {
        keyframeData_maxName = 32,
    };

    public class Keyframe
    {
        public int index;

        public float _duration, _durationInv;

        public int data;

        public Keyframe(float duration, int value_x)
        {
            _duration = duration;
            _durationInv = 1.0f / duration;
            data = value_x;
        }
    };

    public class KeyframePool
    {
        public Keyframe[] _keyframe;
        public int _count;

        public KeyframePool(Keyframe[] keyframes)
        {
            _keyframe = keyframes;
            _count = keyframes.Length;
        }

        public Keyframe this[int i]
        {
            get { return _keyframe[i]; }
            set { _keyframe[i] = value; }
        }
    }

    public class Clip
    {
        public string _name;

        public int index;

        public float duration, durationInv;

        public int keyframeCount;

        public int first_keyframe;

        public int last_keyframe;

        public KeyframePool framePool;

        public Clip(string name, KeyframePool keyframePool, int firstKeyframeIndex, int lastKeyframeIndex)
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
        }

        public Keyframe this[int i]
        {
            get { return framePool[i]; }
        }
    }

    public class ClipPool
    {
        public Clip[] clip;

        public int count;

        public ClipPool(Clip[] clips)
        {
            count = clips.Length;
            clip = clips;
            for(int i = 0; i < clips.Length; i++)
            {
                clip[i].index = i;
            }
        }
        public Clip GetClipIndexByName(string name)
        {
            for (int i = 0; i < count; i++)
            {
                if (clip[i]._name == name)
                {
                    return clip[i];
                }
            }
            return null;
        }
        public Clip this[int i]
        {
            get { return clip[i]; }
            set { clip[i] = value; }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
