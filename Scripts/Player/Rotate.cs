using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    public float speed;
    private float current_agl;
    private Transform t;
    void Start() {
        t = GetComponent<Transform>();
        current_agl = t.rotation.eulerAngles.z;
    }

    void Update() {
        current_agl = current_agl + speed * Player.PlayerRef.speed*0.4f * Time.deltaTime;
        t.rotation = Quaternion.Euler(0, 0, current_agl);
    }
}
