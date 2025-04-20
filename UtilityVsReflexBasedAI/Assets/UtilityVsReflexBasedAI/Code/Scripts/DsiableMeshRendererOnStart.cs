using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DsiableMeshRendererOnStart : MonoBehaviour
{
    void Start()
    {
        if (TryGetComponent(out MeshRenderer meshRenderer))
        {
            meshRenderer.enabled = false;
        }
        else if (TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
        {
            skinnedMeshRenderer.enabled = false;
        }
        else
        {
            Debug.LogWarning("MeshRenderer component not found on this GameObject.");
        }
    }
}
