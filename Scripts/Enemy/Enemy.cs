using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float health, max_health, damage, speed, distanceToDespawn;
    public int xpDrop;
    public int xpOrbCount;
    [SerializeField]
    private Rigidbody2D rb2d;
    [SerializeField]
    private Transform sr_t;
    [SerializeField]
    public SpriteRenderer[] sub_srs;
    private bool fliped_x = false;
    [HideInInspector] public bool hit = false;
    public Color spriteColor = Color.white;
    public GameObject linePrefab;

    void Start() {
        max_health += (max_health * Mathf.Pow(5, WaveManager.tier-1) * WaveManager.enemy_cycle * 0.2f);
        health = max_health;
        speed += (speed * Mathf.Pow(2, WaveManager.tier - 1) * WaveManager.enemy_cycle * 0.001f);
        damage += (damage * Mathf.Pow(3, WaveManager.tier - 1) * WaveManager.enemy_cycle * 0.1f);

        float xp = xpDrop * xpOrbCount;
        xp += (xp * Mathf.Pow(10, WaveManager.tier - 1) * WaveManager.enemy_cycle * 0.25f);

        int xp_d = 1;
        while (xp_d * 5 <= xp) {
            xp_d *= 5;
        }

        xpDrop = xp_d;
        xpOrbCount = (int)(xp/xp_d);
    }

    void Update() {
        if (Player.PlayerRef) {
            Vector3 playerPos = Player.PlayerRef.transform.position;
            Vector3 e_pos = transform.position;

            if (Vector3.Distance(playerPos, e_pos) > distanceToDespawn) Destroy(gameObject);

            Vector3 dir = (playerPos - e_pos).normalized;

            if (!hit) {
                rb2d.velocity = dir * speed;

                if (fliped_x != (dir.x < 0)) {
                    fliped_x = !fliped_x;
                    sr_t.localScale = new Vector2(fliped_x ? Mathf.Abs(sr_t.localScale.x) : -Mathf.Abs(sr_t.localScale.x), sr_t.localScale.y);
                }
            }
        }
    }

    public void GetHit(float damage, Vector2 dir) {
        if (!hit) {
            health -= damage;
            Player.audio_m.Play("enemyHit");

            // knock back, based on how much health is removed
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(Mathf.Min((damage/max_health), 1.5f) * 500 * dir);

            hit = true;

            if (sub_srs.Length > 0) {
                foreach (SpriteRenderer sub in sub_srs) {
                    sub.color = new Color(0, 0.83f, 0.95f);
                }
            }

            if (health <= 0) {
                Instantiate(PrefabLib.splater, transform.position + (new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f))), PrefabLib.splater.transform.rotation);
            }
            Invoke(nameof(UnHit), 0.2f);
        }
    }

    private void UnHit() {
        if (health <= 0) {
            Death();
        }

        hit = false;

        if (sub_srs.Length > 0) {
            foreach (SpriteRenderer sub in sub_srs) {
                sub.color = spriteColor;
            }
        }
    }

    void Death() {
        for (int i = 0; i < xpOrbCount; i++)
        {
            Exp xp = Instantiate(PrefabLib.exp_orb, transform.position, Quaternion.identity).GetComponent<Exp>();
            xp.value = xpDrop;
        }
        Destroy(gameObject);
    }

    public void DoChain(int left)
    {
        left--;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.transform.position, 4);
        List<Enemy> enemies = new List<Enemy>();
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Enemy")
            {
                enemies.Add(hitCollider.gameObject.GetComponent<Enemy>());
            }
        }
        int rand = Random.Range(0, enemies.Count);
        CreateLine(new Vector2(enemies[rand].transform.position.x, enemies[rand].transform.position.y));
        if (left > 0)
        {
            enemies[rand].DoChain(left);
        }
        enemies[rand].GetHit(Player.PlayerRef.damage*0.5f, Vector2.zero);
    }

    public void CreateLine(Vector2 targetPos)
    {
        Vector2 selfPos = new Vector2(transform.position.x, transform.position.y);

        Vector2 middlePos = new Vector2(selfPos.x + (targetPos.x - selfPos.x) * 0.5f, selfPos.y + (targetPos.y - selfPos.y) * 0.5f);

        Vector2 toOther = (selfPos - targetPos).normalized;
        float angle = Mathf.Atan2(toOther.y, toOther.x) * Mathf.Rad2Deg + 90;

        float distance = Vector2.Distance(selfPos, targetPos);

        GameObject line = Instantiate(linePrefab, new Vector2(middlePos.x, middlePos.y), Quaternion.Euler(0, 0, angle)) as GameObject;
        line.transform.localScale = new Vector3(0.05f, distance - 0.025f, line.transform.localScale.z);
    }
}
