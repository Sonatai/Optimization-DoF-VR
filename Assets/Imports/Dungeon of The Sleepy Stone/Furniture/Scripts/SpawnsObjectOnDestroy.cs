using UnityEngine;

public class SpawnsObjectOnDestroy : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    
    void OnDestroy()
    {
        if (prefab)
        {
            Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}