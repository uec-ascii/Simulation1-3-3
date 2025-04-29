using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ease{
    public static float SineEaseInOut(float t){
        return -0.5f * (Mathf.Cos(Mathf.PI * t) - 1);
    }

    public static float SineEaseInOut(float t, float totalTime){
        return SineEaseInOut(t / totalTime);
    }
}