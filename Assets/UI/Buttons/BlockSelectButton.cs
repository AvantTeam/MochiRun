using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelectButton : MonoBehaviour
{
    public Block block;
    public Color color, selectedColor;

    private bool loadedTexture = false;
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
        if(block.hasObject && !loadedTexture && !AssetPreview.IsLoadingAssetPreviews()) LoadSprite(block.prefab);

        if(LChunkLoader.main.cursor.state == CursorControl.STATE.PLACE && LChunkLoader.main.cursor.block == block) {
            image.color = selectedColor;
        }
        else {
            image.color = color;
        }
    }

    void Clicked() {
        if(block == null) return;
        LChunkLoader.main.cursor.SetBlock(block);
    }

    public void SetBlock(Block block) {
        this.block = block;
        if(block.hasObject) {
            loadedTexture = !AssetPreview.IsLoadingAssetPreviews();
            if(loadedTexture) LoadSprite(block.prefab);
        }
        //todo "sprite-only" blocks e.g. PitStarter
    }

    public void LoadSprite(GameObject o) {
        transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = AssetPreview.GetAssetPreview(o);
        loadedTexture = true;
    }
}
