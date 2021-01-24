using UnityEngine;

public class Pillar : MonoBehaviour
{
    [SerializeField]
    private Animation collapseAnimation;
    
    private bool _hasCollapsed = false;
    public void Collapse()
    {
        if (!_hasCollapsed)
        {
            collapseAnimation.Play();
            _hasCollapsed = true;
        }
    }
}