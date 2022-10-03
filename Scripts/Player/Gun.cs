using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    [HideInInspector]
    public Vector3 dir;

    [Header("Gun rotation and knock back")]
    [SerializeField] private float base_gun_radius = 0.75f;
    [SerializeField] private float gun_radius = 0;
    [SerializeField] private float knock_back = -0.25f;
    [SerializeField] private float kb_recovery_speed = 0.1f;
    [SerializeField] private float min_gun_radius = 0.4f;
    [SerializeField] private float shoot_shake_time = 0.1f;
    [SerializeField] private float shoot_shake = 0.0625f;
    [SerializeField] private Transform GunTarget;
    [SerializeField] private Transform OtherHandTarget;
    [Header("Gun stats")]
    [SerializeField] private Transform gun_spawn_point;
    [SerializeField] private bool burst = false;
    [SerializeField] private float burstDelay = 0.1f;
    [SerializeField] private int burstCount = 1;
    [SerializeField] private bool randomSpray = false;
    public int bulletCount = 1;
    public float bulletArc = 0f;
    public float bulletSpeed = 10f;
    public int eleChain = 0;
    [SerializeField] private bool lockAim = true;
    public int pierce = 1;
    private float burstDir;
    private float burstTimer;
    private int burstShotsLeft;

    private void Start() {
        gun_radius = base_gun_radius;
    }

    void Update() {
        if (!Player.PlayerRef) return;

        if (gun_radius < base_gun_radius) {
            gun_radius = Mathf.Min(gun_radius + kb_recovery_speed * Time.deltaTime, base_gun_radius);
        }

        // AMING
        Vector3 mousePos = Input.mousePosition;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        dir = (mousePos - object_pos).normalized;

        GunTarget.position = transform.position + (dir * gun_radius);
        OtherHandTarget.position = transform.position + (dir * gun_radius);

        if (burst)
        {
            burstTimer -= Time.fixedDeltaTime;
            if (burstTimer <= 0 && burstShotsLeft > 0)
            {
                burstTimer = burstDelay;
                burstShotsLeft--;
                if (lockAim)
                {
                    ShootPlayer(burstDir);
                    ShotEffects();
                }
                else
                {
                    ShootPlayer();
                    ShotEffects();
                }
            }
        }

        // SHOOTING
        if (Player.PlayerRef.shoot_delay_time <= 0) {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) {
                if (burst)
                {
                    burstDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    burstTimer = burstDelay;
                    burstShotsLeft = burstCount - 1;
                }

                ShootPlayer();
                ShotEffects();
                Player.PlayerRef.shoot_delay_time = Player.PlayerRef.shoot_speed;

                //Bullet b = Instantiate(PrefabLib.p_bullet, gun_spawn_point.position, Quaternion.identity).GetComponent<Bullet>();
                //b.dir = dir;
                //b.dam = Player.PlayerRef.damage;
                gun_radius = Mathf.Max(gun_radius + knock_back, min_gun_radius);
            }
        }
        else {
            Player.PlayerRef.shoot_delay_time -= Time.deltaTime;
        }
    }

    void ShootPlayer()
    {
        if (randomSpray)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                CreateBullet((Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - Random.Range(bulletArc * -0.5f, bulletArc * 0.5f));
            }
        }
        else if (bulletCount > 1)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                CreateBullet((Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - bulletArc * 0.5f + i * (bulletArc / (bulletCount - 1)));

            }
        }
        else
        {
            CreateBullet(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }
    }

    void ShootPlayer(float dir)
    {
        if (randomSpray)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                CreateBullet(dir - Random.Range(bulletArc * -0.5f, bulletArc * 0.5f));
            }
        }
        else if (bulletCount > 1)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                CreateBullet(dir - bulletArc * 0.5f + i * (bulletArc / (bulletCount - 1)));

            }
        }
        else
        {
            CreateBullet(dir);
        }
    }

    void CreateBullet(float dir)
    {
        GameObject bullet = Instantiate(PrefabLib.p_bullet, gun_spawn_point.position, Quaternion.identity) as GameObject;
        Bullet bScript = bullet.GetComponent<Bullet>();
        bScript.dir = new Vector2(Mathf.Cos(dir * Mathf.Deg2Rad), Mathf.Sin(dir * Mathf.Deg2Rad));
        bScript.speed = bulletSpeed;
        bScript.dam = Player.PlayerRef.damage;
        bScript.pierce = pierce;
        bScript.chainingCount = eleChain;
    }

    void ShotEffects()
    {
        // screen shake on shoot
        Player.audio_m.Play("basicShot");
        StartCoroutine(Player.PlayerRef.c_shake.Shake(shoot_shake_time, shoot_shake));
    }
}
