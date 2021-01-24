using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingStone : Grabable
{

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate (Vector3.up * 25 * Time.deltaTime, Space.Self);
    }
}
