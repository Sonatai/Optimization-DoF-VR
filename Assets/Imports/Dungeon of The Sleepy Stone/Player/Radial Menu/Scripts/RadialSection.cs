using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public class RadialSection
{
    [SerializeField]
    private Sprite icon = null;
    [SerializeField]
    private SpriteRenderer iconRenderer = null;
    [FormerlySerializedAs("obj")] [SerializeField] 
    private GameObject gameObject = null;
    [SerializeField]
    private UnityEvent onPress = new UnityEvent();
    
    public SpriteRenderer IconRenderer
    {
        set { iconRenderer = value; }
        get { return iconRenderer; }
    }
    public Sprite Icon
    {
        set { icon = value; }
        get { return icon; }
    }
    public UnityEvent OnPress {
        get { return onPress; }
    }

    public GameObject Obj
    {
        get => gameObject;
    }
}
