using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboXAcademy : Academy {

    public static float offset;

    public override void AcademyReset()
    {
        offset = resetParameters["offset"];
    }
}

