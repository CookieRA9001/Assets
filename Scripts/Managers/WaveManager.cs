using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public float timeBetween = 1f, spawnOffsetX = 11, spawnOffsetY = 11, noSpawnRadius = 10;
    public int inputSAttempts = 4, sAttemptsCap = 120;
    [SerializeField]
    private float sAttemptsScaling = 4f;
    private float timer;
    [SerializeField]
    private int sAttempts = 4;
    static public int tier = 1;

    static public float enemy_cycle = 0;
    public float enemy_cycle_timer = 10;

    public TMP_Text Timer;

    public Color[] colors;

    void Start()
    {
        sAttempts = inputSAttempts;
        enemy_cycle = 0;
        timer = timeBetween;
        enemy_cycle_timer = 10;
        sAttemptsScaling = sAttempts;
        tier = 1;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            sWave(sAttempts);
            timer = timeBetween;
        }

        enemy_cycle_timer -= Time.deltaTime;
        if (enemy_cycle_timer <= 0) {
            enemy_cycle += 1;
            enemy_cycle_timer = 10;
            if (sAttemptsScaling * 1.1f < sAttemptsCap)
            {
                sAttemptsScaling *= 1.1f;
                sAttempts = (int)sAttemptsScaling;
            } else
            {
                sAttempts = sAttemptsCap;
                newTier();
            }
            Player.PlayerRef.UpdateUpgradePointCounter();
        }

        Timer.text = $"{(int)enemy_cycle_timer}";
    }

    void sWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = new Vector2(0, 0);

            while (Vector2.Distance(spawnPos, new Vector2(0, 0)) < noSpawnRadius)
            {
                spawnPos.x = Random.Range(-spawnOffsetX, spawnOffsetX);
                spawnPos.y = Random.Range(-spawnOffsetY, spawnOffsetY);
            }

            if (Player.PlayerRef) { 
                Vector3 playerPos = Player.PlayerRef.transform.position;
                Enemy enemy = Instantiate(
                    PrefabLib.enemies[Random.Range(0,PrefabLib.enemies.Count)],
                    new Vector2(playerPos.x + spawnPos.x, playerPos.y + spawnPos.y),
                    Quaternion.identity
                ).GetComponent<Enemy>();
                enemy.spriteColor = colors[tier-1];
                if(enemy.sub_srs.Length > 0) {
                    foreach (SpriteRenderer sub in enemy.sub_srs)
                    {
                        sub.color = colors[tier-1];
                    }
                }
            }
            
        }
    }

    void newTier()
    {
        sAttempts = inputSAttempts;
        sAttemptsScaling = inputSAttempts;
        tier++;
    }
}
