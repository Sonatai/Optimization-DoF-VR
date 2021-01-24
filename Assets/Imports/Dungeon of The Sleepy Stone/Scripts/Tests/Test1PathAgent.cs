
using System;
using System.Diagnostics;
using UnityEngine;
using Valve.VR;
using Debug = UnityEngine.Debug;

public class Test1PathAgent : AbstractPathAgent
{
    protected override void SetupTest()
    {
        slave = GameObject.Find("PlayerVR");
        
        path = new []{
            new PathPoint {position = new Vector3(1.4f, 5f, 25.0f), speed = 8f},
            new PathPoint {position = new Vector3(-0.38f, 5f, 6.95f), speed = 4f},
            new PathPoint {position = new Vector3(-2.12f, 5f, -3.22f), speed = 4f, timeout = 2.0f, test = () => true},
            new PathPoint {position = new Vector3(-9.0f, 5f, 5.5f), speed = 4f},
            new PathPoint {position = new Vector3(9.0f, 4f, -11.5f), speed = 30f}
            //_path.Add(new PathPoint{ position = new Vector3(x: 0f, y: 0f, z: 0f), isRelativePosition = true,  speed = 1f, timeout = 0f, hasXChange = true, hasYChange = true, hasZChange = true, });
        };
        
    }
}
