using System.Diagnostics;
using UnityEngine;
using Valve.VR;
using Debug = UnityEngine.Debug;

//Moves Player for testing closed Gate in level 2
public class GatePathTestAgent : AbstractPathAgent
{
    protected override void SetupTest()
    {
        slave = GameObject.Find("PlayerVR");

        path = new[]
        {
            new PathPoint {position = new Vector3(-40f, 5f, -6f), speed = 4f}
        };
        //_path.Add(new PathPoint{ position = new Vector3(x: 0f, y: 0f, z: 0f), isRelativePosition = true,  speed = 1f, timeout = 0f, hasXChange = true, hasYChange = true, hasZChange = true, });
    }
}