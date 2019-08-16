using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Resources
{
    public const float scale = 1 / 3.33f;

    public const float Stay = 0.0f;
    public const float SlowWalk = 1f * scale;
    public const float Walk = 1.39f * scale;
    public const float MaxWalk = 2.08f * scale;
    public const float Run = 3.125f * scale;
    public const float MaxRun = 4.17f * scale;
    public const float Sprint = 5.56f * scale;
    public const float MaxSprint = 6.94f * scale;

    public static readonly float[] Near = { 0f, 5f * scale };
    public static readonly float[] Far = { 5f * scale, 100f * scale };

    public const int MAX_FLOOR = 3;

    public static readonly Vector3 NullVector = new Vector3(-999f, -999f, -999f);

}
