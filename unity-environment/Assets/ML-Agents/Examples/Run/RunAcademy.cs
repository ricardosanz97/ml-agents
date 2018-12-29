using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAcademy : Academy {
    public static float objectiveOffset;
    public static float spikesOffset;
    public static float pitOffset;
    public static float speed;

    public override void InitializeAcademy()
    {
        Physics.gravity *= 5f;
    }
    public override void AcademyReset()
    {
        objectiveOffset = resetParameters["objectiveOffset"];
        spikesOffset = resetParameters["spikesOffset"];
        pitOffset = resetParameters["pitOffset"];
        speed = resetParameters["speed"];
    }
}
