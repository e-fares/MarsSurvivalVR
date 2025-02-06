using UnityEngine;

public class UpdateUVLight : MonoBehaviour
{
    public Renderer targetRenderer; // L'objet qui contient le message caché
    private Material targetMaterial;

    void Start()
    {
        if (targetRenderer != null)
        {
            targetMaterial = targetRenderer.material;
        }
    }

    void Update()
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetVector("_LightPos", transform.position);
        }
    }
}
