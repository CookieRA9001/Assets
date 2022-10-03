using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameManager : MonoBehaviour {
    public float wave = 0;
    public float time = 0;

    public IntColorDictionary IntColorDictionary;

    public IntSprite[] exp_sprites;

    public Upgrade[] upgrade_list;

    void Start() {
    }

    void Update() {
        time += Time.deltaTime;
        if (time >= 10) {
            wave++;
        }
    }

    public IntSprite getSrpite(int value) {
        return Array.Find(exp_sprites, i_s => i_s.value == value);
    }
}
