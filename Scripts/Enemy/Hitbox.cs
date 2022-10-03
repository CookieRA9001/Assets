using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {
    [SerializeField]
    private Enemy enemy;
    [SerializeField]
    private float hit_delay = 0.5f;
    private float delay = 0;

    private void Update() {
        if (delay > 0) {
            delay -= Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (delay <= 0) {
            Player p = Player.PlayerRef;

            if (p) {
                p.GetHit(enemy.damage);

                delay = hit_delay;
            }
        }
    }
}
