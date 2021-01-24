using System.Diagnostics;
using UnityEngine;
using Valve.VR;
using Debug = UnityEngine.Debug;

public class GateCloseAgent : AbstractPathAgent
{
    protected override void SetupTest()
    {
        path = new[]
        {
            new PathPoint
                {position = new Vector3(-42.777f, 0f, -8.5f), speed = 4f, hasXChange = false, hasZChange = false, requiresTrigger = true}
        };
        
    }
}