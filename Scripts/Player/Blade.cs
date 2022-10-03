using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade: MonoBehaviour {

    [SerializeField] private GameObject ps_burst;

    private void OnTriggerEnter2D(Collider2D collision) {
        Enemy e = collision.gameObject.GetComponent<Enemy>();
        if (e && !e.hit) {
            Vector2 dif = e.transform.position - transform.position;
            e.GetHit(Player.PlayerRef.damage*0.5f, dif.normalized);
            if (Player.PlayerRef.weapon.eleChain > 0) e.DoChain(Player.PlayerRef.weapon.eleChain);
            Rigidbody2D ps_rb2d = Instantiate(ps_burst, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
            ps_rb2d.velocity = dif.normalized * 5;
        }
    }
}
