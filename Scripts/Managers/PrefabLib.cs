using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLib : MonoBehaviour {

    [SerializeField]
    private GameObject _p_bullet;
    static public GameObject p_bullet;

    [SerializeField]
    private GameObject _p_blade;
    static public GameObject p_blade;

    [SerializeField]
    private GameObject _exp_orb;
    static public GameObject exp_orb;

    [SerializeField]
    private List<GameObject> _enemies;
    static public List<GameObject> enemies;

    [SerializeField]
    private GameObject _splater;
    static public GameObject splater;

    static public bool prefabs_done = false;

    void Start() {
        p_bullet = _p_bullet;
        exp_orb = _exp_orb;
        enemies = _enemies;
        splater = _splater;
        p_blade = _p_blade;

        prefabs_done = true;
    }
}
