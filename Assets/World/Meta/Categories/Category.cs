using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "Blocks/Category", order = 200)]
public class Category : ScriptableObject {
    public Color color;
    public Texture2D icon;
    public bool special;
    public bool blackIcon;
}
