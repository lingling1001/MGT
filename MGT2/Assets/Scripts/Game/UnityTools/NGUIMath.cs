using UnityEngine;

static public class NGUIMath
{
    /// <summary>
    /// Ensure that the angle is within -180 to 180 range.
    /// </summary>

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    static public float WrapAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

}