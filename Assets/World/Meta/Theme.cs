using UnityEngine;

[CreateAssetMenu(fileName = "NewTheme", menuName = "Theme", order = 3)]
public class Theme : ScriptableObject {
    public Sprite background1, background2;
    public bool backgroundScrollY = true;
    public Color sky = new Color(174 / 255f, 205 / 255f, 255 / 255f);
    public Color fog = new Color(0 / 255f, 17 / 255f, 36 / 255f, 109 / 255f);
    public Texture2D icon;

    public GameObject floor;
}
