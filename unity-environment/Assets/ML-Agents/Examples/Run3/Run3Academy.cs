using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run3Academy : Academy {

    public static float agentSpeed = 10f;
    public static float configuration = 1f;

    public override void InitializeAcademy()
    {
        Physics.gravity *= 5f;
    }

    public override void AcademyReset()
    {
        agentSpeed = resetParameters["agentSpeed"];
        configuration = resetParameters["configuration"];
    }

}
