using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Vector2 dir;
    public float speed;
    public float dam;
    public int pierce = 1;
    public int chainingCount = 0;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private GameObject ps_burst;

    void Update() {
        rb2d.velocity = dir * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Enemy e = collision.gameObject.GetComponent<Enemy>();
        if (e && !e.hit) {
            Vector2 dif = e.transform.position - transform.position;
            e.GetHit(dam, dif.normalized);
            if (chainingCount > 0) e.DoChain(chainingCount);
            Rigidbody2D ps_rb2d = Instantiate(ps_burst, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
            ps_rb2d.velocity = dir * (speed * 0.5f);

            pierce--;
            if (pierce <= 0) {
                Destroy(gameObject);
            }
        }
    }
}
