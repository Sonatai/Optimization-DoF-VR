using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Valve.VR;
using Debug = UnityEngine.Debug;

public class MoveThroughFloorPathTestAgent : AbstractPathAgent
{
    protected override void SetupTest()
    {
        path = new []{
            new PathPoint{position = new Vector3( 0f, -90f, 0f), speed = 30f}
        };
        //_path.Add(new PathPoint{ position = new Vector3(x: 0f, y: 0f, z: 0f), isRelativePosition = true,  speed = 1f, timeout = 0f, hasXChange = true, hasYChange = true, hasZChange = true, });
    }
}