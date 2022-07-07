using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimationScript : MonoBehaviour
{
    [SerializeField] LayerMask raycastLayer = default;
    [SerializeField] private Transform raycastSource;
    private Vector3 _oldPosition;
    
    
    
    private void Start()
    {
        Ray ray = new Ray(raycastSource.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10, raycastLayer.value))
        {
            _oldPosition = info.point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _oldPosition;
    }
    
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_oldPosition, 0.02f);
    }
}
