using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyframeData
{
    enum constant
    {
        keyframeData_maxName = 32,
    };

    struct Keyframe
    {
        public uint index;

        public float duration, durationInv;

        public uint data;
    };

    struct KeyframePool
    {
        public Keyframe[] keyframe;
        public uint count;
    }

    struct Clip
    {
        public byte[] name;

        public uint index;

        public float duration, durationInv;

        public uint keyframeCount;
        
        public uint first_keyframe;

        public uint last_keyframe;

        public KeyframePool[] framePool;
    }

    struct ClipPool
    {
        public Clip[] clip;

        public uint count;
    }

    int keyframeInit(Keyframe keyframe_out, float duration, uint value_x)
    {
        keyframe_out.duration = duration;
        keyframe_out.durationInv = 1.0f / duration;

        keyframe_out.data = value_x;

        return -1;
    }

    int keyframePoolCreate(KeyframePool keyframePool_out, uint count)
    {
        keyframePool_out.keyframe = new Keyframe[32];
        keyframePool_out.count = count;

        return -1;
    }

    int clipInit(Clip clip_out, byte[] clipName, KeyframePool[] keyframePool, uint firstKeyframeIndex, uint lastKeyframeIndex)
    {
        clipName = new byte[32];

        for(uint i = 0; i < 32; i ++)
        {
            clip_out.name[i] = clipName[i];
        }

        clip_out.framePool = keyframePool;

        clip_out.first_keyframe = firstKeyframeIndex;
        clip_out.last_keyframe = lastKeyframeIndex;
        return -1;
    }

    int keyframePoolRelease(KeyframePool keyframePool)
    {
        return -1;
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
