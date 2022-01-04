using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TriggerUpdater;

public class RightClickPopup : MonoBehaviour
{
    public Color active = Color.white, inactive = Color.gray;

    public Button rangeL, rangeR, dataL, dataR;
    public Text rangeText, dataText, dataTitle;

    private Trigger trigger;
    private LBlockUpdater block;
    Camera cam;
    RectTransform rect;

    void Start() {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        rect = GetComponent<RectTransform>();
        rangeL.onClick.AddListener(RangeLClicked);
        rangeR.onClick.AddListener(RangeRClicked);
        dataL.onClick.AddListener(DataLClicked);
        dataR.onClick.AddListener(DataRClicked);
    }

    void Update() {
        if(block != null){
            move();
        }
    }

    public void SetBlock(Trigger trigger, LBlockUpdater block) {
        this.trigger = trigger;
        this.block = block;
        rangeText.text = range();

        if(trigger.maxData <= 0) {
            //no modifiers
            dataTitle.text = "Modifier";
            dataText.text = "-";
            dataText.color = inactive;
            dataTitle.color = inactive;
            dataL.interactable = dataR.interactable = false;
        }
        else {
            dataTitle.text = trigger.modifierName;
            dataText.text = data();
            dataText.color = active;
            dataTitle.color = active;
            dataL.interactable = dataR.interactable = true;
        }
        rebuild();
        gameObject.SetActive(true);
        //move();
        //rect.position = (Vector2)cam.WorldToScreenPoint(block.gameObject.transform.position);
    }

    private void rebuild() {
        if(trigger != null) {
            rangeL.interactable = CtypeRange(block.ctype) > TagRange.One;
            rangeR.interactable = CtypeRange(block.ctype) < TagRange.Infinite;
            rangeText.text = range();
            if(trigger.maxData > 0){
                dataL.interactable = (block.ctype >> 3) > 0;
                dataR.interactable = (block.ctype >> 3) < trigger.maxData;
                dataText.text = data();
            }
        }
    }

    public void move() {
        if(cam == null) cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        Vector2 vmin = cam.WorldToScreenPoint(cam.transform.position - new Vector3(-cam.orthographicSize * Screen.width / Screen.height, cam.orthographicSize, 0));
        rect.position = (Vector2)cam.WorldToScreenPoint(block.gameObject.transform.position);
        if(rect.position.y - rect.sizeDelta.y < vmin.y) {
            Vector3 v = rect.position;
            v.y = vmin.y + rect.sizeDelta.y;
            rect.position = v;
        }
        if(rect.position.x + rect.sizeDelta.x > vmin.x) {
            Vector3 v = rect.position;
            v.x = vmin.x - rect.sizeDelta.x;
            rect.position = v;
        }
    }

    public string range() {
        if(block == null) return "-";
        TagRange r = CtypeRange(block.ctype);
        if(r < TagRange.CrossX) {
            return ((int)r) * 2 + 1 + " Blocks";
        }
        else switch(r) {
            case TagRange.CrossX: return "Player > X";
            case TagRange.CrossY: return "Player > Y";
            case TagRange.Infinite: return "On Screen";
        }
        return "-";
    }

    public string data() {
        if(block == null || trigger.maxData <= 0) return "-";
        else {
            return trigger.dataToModifier(block.ctype >> 3).ToString();
        }
    }

    void RangeLClicked() {
        if(CtypeRange(block.ctype) > TagRange.One) setCtype((int)(CtypeRange(block.ctype) - 1) | (block.ctype & 0b11111000));
    }

    void RangeRClicked() {
        if(CtypeRange(block.ctype) < TagRange.Infinite) setCtype((int)(CtypeRange(block.ctype) + 1) | (block.ctype & 0b11111000));
    }

    void DataLClicked() {
        if((block.ctype >> 3) > 0) setCtype((block.ctype & 0b111) | (((block.ctype >> 3) - 1) << 3));
    }

    void DataRClicked() {
        if((block.ctype >> 3) < trigger.maxData) setCtype((block.ctype & 0b111) | (((block.ctype >> 3) + 1) << 3));
    }

    private void setCtype(int c) {
        byte ctype = (byte)c;
        block.SetBlock(block.type, ctype);
        block.save = new ChunkLoader.BlockSave(block.type, block.save.x, block.save.y, ctype);
        rebuild();
    }
}
