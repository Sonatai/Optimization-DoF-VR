using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCrossbowPlankSmash : AbstractPathAgent
{
    protected override void SetupTest()
    {
        slave = GameObject.Find("Crossbow(the right one)");
        
        path = new []{
            new PathPoint {position = new Vector3(35.88f, 1.01f, 4.3f), speed = 8f, timeout = 2f},
            new PathPoint {position = new Vector3(37.88f, 2.01f, 5.3f), speed = 8f},
            new PathPoint {position = new Vector3(44f, 2.0f, 7.003f), speed = 8f}
            //_path.Add(new PathPoint{ position = new Vector3(x: 0f, y: 0f, z: 0f), isRelativePosition = true,  speed = 1f, timeout = 0f, hasXChange = true, hasYChange = true, hasZChange = true, });
        };
        
    }
}
