using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelectButton : MonoBehaviour
{
    public Block block;
    public Color color, selectedColor, favoriteColor;
    public Texture2D defaultIcon;
    public Sprite defaultSprite, favoriteSprite;
    public Category favoriteCategory;

    private bool isDefaultSprite = true;
    Image image;
    void Start()
    {
        image = GetComponent<Image>();
        image.color = color;

        GetComponent<Button>().onClick.AddListener(Clicked);
        isDefaultSprite = true;
        image.sprite = defaultSprite;
    }

    void Update()
    {
        if(block == null) return;
        //if(block.hasObject && !loadedTexture && !AssetPreview.IsLoadingAssetPreviews()) LoadSprite(block.prefab);

        if(LChunkLoader.main.cursor.state == CursorControl.STATE.PLACE && LChunkLoader.main.cursor.block == block) {
            image.color = selectedColor;
            SetSprite(false);
        }
        else if(LChunkLoader.main.frag.category != favoriteCategory && IsFavorite()) {
            image.color = favoriteColor;
            SetSprite(true);
        }
        else {
            image.color = color;
            SetSprite(false);
        }
    }

    void Clicked() {
        if(block == null) return;

        if(KeyBinds.Get("Favorite")) {
            //favorite it
            ToggleFavorite();
            //todo maybe a sound cue here?
        }
        else {
            if(LChunkLoader.main.cursor.block == block && LChunkLoader.main.cursor.state == CursorControl.STATE.PLACE) {
                LChunkLoader.main.cursor.SetState(CursorControl.STATE.NONE);
            }
            else {
                LChunkLoader.main.cursor.SetBlock(block);
            }
        }
        
    }

    public void ToggleFavorite() {
        if(block == null) return;
        if(IsFavorite()) {
            LChunkLoader.main.frag.favorites.Remove(block);
        }
        else {
            LChunkLoader.main.frag.favorites.Add(block);
        }
    }

    public bool IsFavorite() {
        return LChunkLoader.main.frag.favorites.Contains(block);
    }

    public void SetBlock(Block block) {
        this.block = block;
        if(block.sprite != null) {
            LoadSprite(block.sprite.texture, block.hasObject);
        }
    }

    private void SetSprite(bool fav) {
        if(fav && isDefaultSprite) {
            isDefaultSprite = false;
            image.sprite = favoriteSprite;
        }
        else if(!fav && !isDefaultSprite) {
            isDefaultSprite = true;
            image.sprite = defaultSprite;
        }
    }

    public void LoadSprite(Texture2D sprite, bool cut) {
        RawImage im = transform.GetChild(0).gameObject.GetComponent<RawImage>();
        im.texture = sprite == null ? defaultIcon : sprite;
        if(!cut) {
            im.uvRect = new Rect(0f, 0f, 1f, 1f);
        }
    }
}
