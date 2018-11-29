using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

public class LeapData {
    // EntryPoint must match with the name used in the C++ library

    [DllImport("LeapData", EntryPoint = "collectCurrentFrameData")]
    public static extern void collectCurrentFrameData(float[] a, int length);




}
