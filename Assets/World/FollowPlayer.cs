using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, 0);
    GameObject player;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = player.transform.position + offset;
    }

    // Update is called once per frame
    void Update() {
        transform.position = player.transform.position + offset;
    }
}
