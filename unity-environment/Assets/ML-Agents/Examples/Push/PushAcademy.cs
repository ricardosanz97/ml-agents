using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAcademy : Academy {

    public static float max_offset;
    public static float floorWidth;
    public static float limit;
    public static float cubeLimit;

    public override void InitializeAcademy()
    {
        Physics.gravity *= 5;

        Transform floor = GameObject.Find("Floor").transform;
        floorWidth = floor.localScale.x;
        limit = (floorWidth / 2f) - 0.5f;
        cubeLimit = limit - 1.5f;

    }

    public override void AcademyReset()
    {
        max_offset = resetParameters["max_offset"];

    }
}
