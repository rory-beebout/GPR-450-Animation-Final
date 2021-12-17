using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipController : KeyframeData
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

    public ClipController(string name, ClipPool pool, int startingClip)
    {
        this.name = name;
        clipPool = pool;

        clipIndex = startingClip;
        clipTime = 0;
        clipParam = 0;

        keyframeIndex = clipPool[clipIndex].first_keyframe;
        keyframeTime = 0;
        keyframeParam = 0;

        playbackSpeed = 1;
    }
}
