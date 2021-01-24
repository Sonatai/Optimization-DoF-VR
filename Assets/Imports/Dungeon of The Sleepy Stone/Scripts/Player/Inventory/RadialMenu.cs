using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField]
    private Transform selectionTransform = null;
    [SerializeField]
    private Transform cursorTransform = null;

    [Header("Events")]
    [SerializeField]
    private RadialSection top = null;
    [SerializeField]
    private RadialSection right = null;
    [SerializeField]
    private RadialSection bottom = null;
    [SerializeField]
    private RadialSection left = null;

    private Vector2 touchPosition = Vector2.zero;
    private List<RadialSection> radialSections = null;
    private RadialSection highlightedSection = null;

    private readonly float degreeIncrement = 90.0f;
    
    //... Crossbow control variables
    private bool isReloading = false;
    private bool isShooting = false;

    public bool IsReloading
    {
        get => isReloading;
        set => isReloading = value;
    }
    public bool IsShooting
    {
        get => isShooting;
        set => isShooting = value;
    }
    private void Awake()
    {
        CreateAndSetupSections();
    }


    public void ActivateButton(int index)
    {
        RadialSection radialSection =
            index == 0 ? top :
            index == 1 ? right :
            index == 2 ? bottom :
            left;
        
        radialSection.OnPress.SetPersistentListenerState(0,UnityEventCallState.RuntimeOnly);
        radialSection.Obj.SetActive(true);
    }
    
    private void Update()
    {
        Vector2 direction = Vector2.zero + touchPosition;
        float rotation = GetDegree(direction);

        SetCursorPosition();

        SetSelectionRotation(rotation);
        SetSelectedEvent(rotation);
    }

    private void CreateAndSetupSections()
    {
        radialSections = new List<RadialSection>()
        {
            top,
            right,
            bottom,
            left
        };
        foreach(RadialSection section in radialSections)
        {
            section.IconRenderer.sprite = section.Icon;
        }

        highlightedSection = radialSections[1];
    }

    public void Show(bool value)
    {
        if(gameObject)
            gameObject.SetActive(value);
    }

    private float GetDegree(Vector2 direction)
    {
        float value = Mathf.Atan2(direction.x, direction.y);
        value *= Mathf.Rad2Deg;
        if (value < 0f)
            value += 360.0f;
        return value;
    }
    private void SetCursorPosition()
    {
        cursorTransform.localPosition = touchPosition;
    }
    
    private void SetSelectionRotation(float newRotation)
    {
        float snappedRotation = SnapRotation(newRotation);
        selectionTransform.localEulerAngles = new Vector3(0f, 0f, -snappedRotation);
    }
    private float SnapRotation(float rotation)
    {
        return GetNearestIncrement(rotation) * degreeIncrement;
    }
    private int GetNearestIncrement(float rotation)
    {
        return Mathf.RoundToInt(rotation / degreeIncrement);
    }
    private void SetSelectedEvent(float currentRotation)
    {
        int index = GetNearestIncrement(currentRotation);

        if (index == radialSections.Count)
            index = 0;

        highlightedSection = radialSections[index];
    }
    public void SetTouchPosition(Vector2 newValue)
    {
        touchPosition = newValue;
    }
    public void ActivateHighlightedSection()
    {
        if (!isReloading /*&& !isShooting*/)
        {
            highlightedSection.OnPress.Invoke();
            Show(false);
        }
        
    }
}
