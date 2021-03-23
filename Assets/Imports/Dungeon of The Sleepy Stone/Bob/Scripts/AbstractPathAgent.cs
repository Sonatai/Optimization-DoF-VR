using System;
using UnityEngine;

public abstract class AbstractPathAgent : MonoBehaviour
{
    protected GameObject slave;

    //TODO: get weapon and shoot enemies! 

    //TODO: add lambdas which are executed at certain path points (external test lambas)
    /**
     * What do test lambdas do ?
     * They return true or false if the current game state is as expected!
     */
    [Serializable]
    public class PathPoint // Path point class (anonymous)
    {
        public Vector3 position = Vector3.zero;
        public bool hasXChange = true;
        public bool hasYChange = true;
        public bool hasZChange = true;
        public float speed = 1f;
        public float timeout = 0f;
        public bool isRelativePosition = false;
        public bool requiresTrigger = false;
        public Func<bool> test = null;

        public bool StartTest() // If a lambda test is present it will be executed when agent reaches this PathPoint!
        {
            bool testResult = test.Invoke();
            test = null;
            return testResult; // Test result is returned and will be evaluated!
        }

        public bool HasTest()
        {
            return test != null;
        }

        public void SetAbsolutePosition(Vector3 currentPosition)
        {
            if (isRelativePosition)
            {
                position.x += currentPosition.x;
                position.y += currentPosition.y;
                position.z += currentPosition.z;
                isRelativePosition = false;
            }
        }
    }

    protected PathPoint[] path;
    private int _pathStops;
    private int _currentPathIndex;
    private bool _requiresTrigger;

    protected abstract void SetupTest();

    void Start()
    {
        _currentPathIndex = 0;
        slave = this.gameObject;
        SetupTest();
        _pathStops = path.Length;

        if (_currentPathIndex < path.Length && path[_currentPathIndex].isRelativePosition)
        {
            path[_currentPathIndex].SetAbsolutePosition(slave.transform.position);
        }

        _requiresTrigger = path[_currentPathIndex].requiresTrigger;
    }

    private void Move()
    {
        if (_currentPathIndex < _pathStops)
        {
            //Debug.Log(_path[_current_path_index].timeout+" ... index: "+_current_path_index);
            //Debug.Log(_current_path_index + " " + _pathStops);

            var position = slave.transform.position;
            float vX = (path[_currentPathIndex].hasXChange
                ? path[_currentPathIndex].position.x - position.x
                : 0);
            float vY = (path[_currentPathIndex].hasYChange
                ? path[_currentPathIndex].position.y - position.y
                : 0);
            float vZ = (path[_currentPathIndex].hasZChange
                ? path[_currentPathIndex].position.z - position.z
                : 0);

            float d = VectorLength(vX, vY, vZ);
            float shift = Time.deltaTime * path[_currentPathIndex].speed;

            if (d > 0.1f && d > shift)
            {
                vX = (vX / d) * shift; // adjusting length to shift (speed*timeDelta)...
                vY = (vY / d) * shift;
                vZ = (vZ / d) * shift;
                //Move closer:
                Vector3 newPosition = new Vector3(
                    position.x + vX,
                    position.y + vY,
                    position.z + vZ
                );

                slave.transform.position = newPosition;
            }
            else
            {
                slave.transform.position = path[_currentPathIndex].position;
                //Agent has reached next path way point:
                if (path[_currentPathIndex].HasTest()) //Checks if current PathPoint contains a lambda test:
                {
                    bool success = path[_currentPathIndex].StartTest(); //Custom test is executed and logged!
                    //Debug.Log("[AI-TestAgent]: Test result: " + success);
                }

                if (path[_currentPathIndex].timeout > 0.0) //decrements timeout if present...
                {
                    path[_currentPathIndex].timeout -= Time.deltaTime;
                    return;
                }

                if (_currentPathIndex + 1 < _pathStops)
                {
                    if (path[_currentPathIndex + 1].isRelativePosition)
                    {
                        path[_currentPathIndex + 1].SetAbsolutePosition(slave.transform.position);
                    }


                    _currentPathIndex++; //Next path way point is targeted by incrementing index !

                    _requiresTrigger = path[_currentPathIndex].requiresTrigger;
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_requiresTrigger)
        {
            Move();
        }
    }

    public void StartMoving()
    {
        _requiresTrigger = false;
    }

    private static float VectorLength(float vX, float vY, float vZ)
    {
        return Mathf.Pow( // Length of vector (between agent and current path point)
            Mathf.Pow(vX, 2) + Mathf.Pow(vY, 2) + Mathf.Pow(vZ, 2)
            , 0.5f
        );
    }
}