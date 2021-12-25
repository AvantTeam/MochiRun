using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelectButton : MonoBehaviour
{
    public Block block;
    public Color color, selectedColor;
    public Texture2D defaultSprite;

    Image image;
    void Start()
    {
        image = GetComponent<Image>();
        image.color = color;

        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    void Update()
    {
        if(block == null) return;
        //if(block.hasObject && !loadedTexture && !AssetPreview.IsLoadingAssetPreviews()) LoadSprite(block.prefab);

        if(LChunkLoader.main.cursor.state == CursorControl.STATE.PLACE && LChunkLoader.main.cursor.block == block) {
            image.color = selectedColor;
        }
        else {
            image.color = color;
        }
    }

    void Clicked() {
        if(block == null) return;
        if(LChunkLoader.main.cursor.block == block) {
            LChunkLoader.main.cursor.SetState(CursorControl.STATE.NONE);
        }
        else {
            LChunkLoader.main.cursor.SetBlock(block);
        }
    }

    public void SetBlock(Block block) {
        this.block = block;
        if(block.hasObject) {
            //loadedTexture = !AssetPreview.IsLoadingAssetPreviews();
            LoadSprite(block.sprite);
        }
        //todo "sprite-only" blocks e.g. PitStarter
    }

    public void LoadSprite(Texture2D sprite) {
        transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = sprite == null ? defaultSprite : sprite;
    }
}
