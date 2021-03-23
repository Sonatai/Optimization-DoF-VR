/*
 * Path Agent that can be configured from Unity Inspector
 */

public class ConfigurablePathAgent : AbstractPathAgent
{
    public PathPoint[] positions = new PathPoint[1];

    protected override void SetupTest()
    {
        path = positions;
        //_path.Add(new PathPoint{ position = new Vector3(x: 0f, y: 0f, z: 0f), isRelativePosition = true,  speed = 1f, timeout = 0f, hasXChange = true, hasYChange = true, hasZChange = true, });
    }
}