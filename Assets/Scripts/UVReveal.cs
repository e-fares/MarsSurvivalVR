using UnityEngine;

public class UVReveal : MonoBehaviour
{
    public Light uvLight; // Référence à la lumière UV
    private Material mat;
    private Color hiddenEmission = Color.black; // Invisible au départ
    private Color visibleEmission = new Color(1f, 0.5f, 0f, 1f); // Orange brillant sous UV
    private bool isNearUV = false;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.SetColor("_EmissionColor", hiddenEmission);
    }

    void Update()
    {
        if (isNearUV)
        {
            mat.SetColor("_EmissionColor", visibleEmission);
        }
        else
        {
            mat.SetColor("_EmissionColor", hiddenEmission);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == uvLight.gameObject)
        {
            isNearUV = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == uvLight.gameObject)
        {
            isNearUV = false;
        }
    }
}
