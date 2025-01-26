using UnityEngine;

public class ViseurScript : MonoBehaviour
{
    public float pointSize = 20f;  // Taille du viseur (cercle)
    public Color viseurColor = Color.white;  // Couleur du viseur (modifiable dans l'inspector)

    private Texture2D circleTexture;

    void Start()
    {
        // Créer une texture circulaire
        circleTexture = CreateCircleTexture((int)pointSize);
    }

    void OnGUI()
    {
        // Position du centre de l'écran
        float xPos = Screen.width / 2 - pointSize / 2;
        float yPos = Screen.height / 2 - pointSize / 2;

        // Appliquer la couleur du viseur
        GUI.color = viseurColor;

        // Dessiner le cercle au centre de l'écran
        GUI.DrawTexture(new Rect(xPos, yPos, pointSize, pointSize), circleTexture);
    }

    // Créer une texture circulaire
    Texture2D CreateCircleTexture(int diameter)
    {
        Texture2D texture = new Texture2D(diameter, diameter);
        Color[] pixels = new Color[diameter * diameter];

        // Calculer le rayon du cercle
        float radius = diameter / 2f;

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                // Calculer la distance par rapport au centre du cercle
                Vector2 point = new Vector2(x - radius, y - radius);
                if (point.magnitude <= radius)
                {
                    pixels[y * diameter + x] = Color.white;  // Mettre des pixels blancs dans le cercle
                }
                else
                {
                    pixels[y * diameter + x] = new Color(0, 0, 0, 0);  // Transparence pour l'extérieur du cercle
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
