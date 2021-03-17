using UnityEngine;

public class TestPosition: MonoBehaviour
{
    [SerializeField] private  Transform userPosition;
    [SerializeField] private  Transform focusPoint1;
    [SerializeField] private  Transform focusPoint2;

    public Transform UserPosition
    {
        get => userPosition;
    }

    public Transform FocusPoint1
    {
        get => focusPoint1;
    }

    public Transform FocusPoint2
    {
        get => focusPoint2;
    }

}