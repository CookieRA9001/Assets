using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player: MonoBehaviour {
    [Header("Diffuculty")]
    public float level_exp_multi = 1.2f;
    public float hp_per_level = 1;
    public float dam_percent_per_level = 0.1f;
    public float shoot_speed_decrese_per_level = 0.05f;
    public float speed_percent_per_level = 0.01f;
    public float exp_multi = 1f;

    [Header("Player Stats [base]")]
    public Gun weapon;
    [SerializeField] private float _max_health = 10;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _shoot_speed = 0.5f;
    [SerializeField] private float _damage = 1;
    [SerializeField] private float _exp_range = 4;

    [Header("Player Stats [current]")]
    public float health;
    public float exp;
    public float next_level_exp;
    public float level;
    public float max_health;
    public float speed;
    public float shoot_speed;
    public float damage;
    public float exp_range;
    private int upgrades_taken = 0;

    [Header("UI Components")]
    public TMP_Text health_text;
    public RectTransform health_bar;
    public TMP_Text level_text;
    public TMP_Text upgrades_point_text;
    public RectTransform level_bar;
    public RectTransform upgrade_menu;
    public RectTransform lose_screen;
    public TMP_Text lose_score;

    [Header("Action Tracking")]
    public Transform sr_t;
    public Rigidbody2D rb2d;
    [SerializeField] private float hit_shake_time = 0.1f;
    [SerializeField] private float hit_shake = 0.03f;
    [HideInInspector]
    public float shoot_delay_time = 0;
    public bool fliped = false;
    public Animator anim;
    [HideInInspector]
    public CameraShake c_shake;
    [HideInInspector]
    static public AudioManager audio_m;
    [HideInInspector]
    public GameManager gm;

    [Header("Upgrads")]
    public Sprite Upgrade_Health;
    public Sprite Upgrade_Damage;
    public Sprite Upgrade_Utility;
    public Sprite Upgrade_New;
    public Sprite Upgrade_Upgrade;
    public Sprite Upgrade_Null;
    private HashSet<Upgrade.UpgradeType> SeenUpgrades;
    private Dictionary<Upgrade.UpgradeType, int> GottenUpgrade;
    [Header("Upgrads UI Refs")]
    [SerializeField] private TMP_Text op1_title;
    [SerializeField] private TMP_Text op2_title;
    [SerializeField] private TMP_Text op1_text;
    [SerializeField] private TMP_Text op2_text;
    [SerializeField] private Image op1_upgrade_sprite;
    [SerializeField] private Image op2_upgrade_sprite;
    [SerializeField] private Image op1_marker_sprite;
    [SerializeField] private Image op2_marker_sprite;

    static public Player PlayerRef;

    [Header("Upgrades")]
    public Upgrade Option1, Option2;

    // unused, yet
    public void BaseValues() {
        hp_per_level = 1;
        dam_percent_per_level = 0.1f;
        shoot_speed_decrese_per_level = 0.05f;
        speed_percent_per_level = 0.01f;
        exp_multi = 1f;
        max_health = _max_health;
        speed = _speed;
        shoot_speed = _shoot_speed;
        damage = _damage;
        exp_range = _exp_range;
        exp = 0;
        upgrades_taken = 0;
        GottenUpgrade = new Dictionary<Upgrade.UpgradeType, int>();
        SeenUpgrades = new HashSet<Upgrade.UpgradeType>();
        Option1 = null;
        Option2 = null;

        ChangeHealth(max_health - health);
        ChangeExp(0);
    }

    private void Awake() {
        PlayerRef = this;
        // referances to global objects/managers
        if (upgrade_menu) upgrade_menu.gameObject.SetActive(false);
        if (lose_screen) lose_screen.gameObject.SetActive(false);
        c_shake = Camera.main.gameObject.GetComponent<CameraShake>();
        if (!audio_m) audio_m = FindObjectOfType<AudioManager>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start() {
        BaseValues();
    }

    private void Update() {
        // MOVEMENT
        Vector2 move = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            move.y += 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            move.x += 1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            move.x -= 1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            move.y -= 1;
        }
        if (move.x != 0 && move.y != 0) {
            move /= Mathf.Sqrt(2);
        }

        if (move.x == 0 && move.y == 0) {
            anim.Play("Player Idle");
        }
        else {
            anim.Play("Player Run");
        }

        if (move.x < 0 != fliped && move.x != 0) {
            fliped = !fliped;
            sr_t.localScale = new Vector2(fliped ? -1 : 1, 1);
        }
        rb2d.velocity = move * speed;

        if (Input.GetKeyDown(KeyCode.E)) {
            OpenUpgradeMenu();
        }
    }

    public void GetHit(float dam) {
        ChangeHealth(-dam);
        StartCoroutine(c_shake.Shake(hit_shake_time, hit_shake));
        audio_m.Play("getHit");

        if (health <= 0) {
            Death();
        }
        else {
            Invoke(nameof(UnHit), 0.2f);
        }
    }

    private void Death() {
        if (lose_screen) { 
            lose_screen.gameObject.SetActive(true);
            lose_score.text = $"The singularity survived {WaveManager.enemy_cycle} waves";
        }

        Destroy(gameObject);
    }

    private void UnHit() {

    }

    public void UpdateUpgradePointCounter() {
        if (upgrades_point_text) upgrades_point_text.text = upgrades_taken!=WaveManager.enemy_cycle ? $"You have {WaveManager.enemy_cycle - upgrades_taken} unused points! [E]" : "";
    }

    public void ChangeHealth(float dif) {
        health = Mathf.Min(health+dif, max_health);
        if (health_bar) { 
            health_bar.anchorMin = new Vector2((1-((int)health/max_health))*0.5f, health_bar.anchorMin.y);
            health_bar.anchorMax = new Vector2(0.5f + ((int)health / max_health * 0.5f), health_bar.anchorMax.y);
            health_text.text = $"{(int)health} / {(int)max_health}";
        }
    }

    public void ChangeExp(float dif) {
        exp += dif*exp_multi;

        while ((int)exp >= (int)next_level_exp) {
            exp -= next_level_exp;
            next_level_exp *= level_exp_multi;
            LevelUp();
        }

        if (level_bar) {
            level_bar.anchorMin = new Vector2((1 - (exp / next_level_exp)) * 0.5f, level_bar.anchorMin.y);
            level_bar.anchorMax = new Vector2(0.5f + (exp / next_level_exp * 0.5f), level_bar.anchorMax.y);
            level_text.text = $"[LV: {(int)level}] {(int)exp} / {(int)next_level_exp}";
        }
    }

    private void LevelUp() {
        level++;
        //Debug.Log($"Leveled up : {(int)level}");

        damage += _damage * dam_percent_per_level;
        max_health += hp_per_level;
        speed += _speed * speed_percent_per_level;
        shoot_speed -= shoot_speed * shoot_speed_decrese_per_level;
        ChangeHealth(hp_per_level+1);
    }

    private void OpenUpgradeMenu() {
        if (!upgrade_menu) return;

        upgrade_menu.gameObject.SetActive(!upgrade_menu.gameObject.activeSelf);

        if (upgrade_menu.gameObject.activeSelf) {
            Time.timeScale = 0;

            if (upgrades_taken < WaveManager.enemy_cycle) {
                if (Option1 == null || Option2 == null || 
                    Option1.type == Upgrade.UpgradeType.Null ||
                    Option2.type == Upgrade.UpgradeType.Null) {
                    GenerateNewOptions(); 
                }
            }
            else {
                ClearOptions();
            }
        }
        else {
            Time.timeScale = 1;
        }
    }

    private void UpdateUpgradeMenuElements() {
        if (Option1 != null) {
            op1_title.text = Option1.title;
            op1_text.text = Option1.description;
            op1_upgrade_sprite.sprite = getUpgradeClassSprite(Option1.sub_class);

            if (!SeenUpgrades.Contains(Option1.type)) {
                SeenUpgrades.Add(Option1.type);
                op1_marker_sprite.sprite = Upgrade_New;
            }
            else if (GottenUpgrade.ContainsKey(Option1.type)) {
                op1_marker_sprite.sprite = Upgrade_Upgrade;
            }
            else {
                op1_marker_sprite.sprite = Upgrade_Null;
            }
        }

        if (Option2 != null) {
            op2_text.text = Option2.description;
            op2_title.text = Option2.title;
            op2_upgrade_sprite.sprite = getUpgradeClassSprite(Option2.sub_class);

            if (!SeenUpgrades.Contains(Option2.type)) {
                SeenUpgrades.Add(Option2.type);
                op2_marker_sprite.sprite = Upgrade_New;
            }
            else if (GottenUpgrade.ContainsKey(Option2.type)) {
                op2_marker_sprite.sprite = Upgrade_Upgrade;
            }
            else {
                op2_marker_sprite.sprite = Upgrade_Null;
            }
        }
            
    }

    private void GenerateNewOptions() {
        // get the options
        Option1 = gm.upgrade_list[Random.Range(0,gm.upgrade_list.Length)];
        Option2 = gm.upgrade_list[Random.Range(0, gm.upgrade_list.Length)];
        while (Option2 == Option1) {
            Option2 = gm.upgrade_list[Random.Range(0, gm.upgrade_list.Length)];
        }

        UpdateUpgradeMenuElements();
    }

    private Sprite getUpgradeClassSprite(Upgrade.UpgradeClass uc) {
        switch (uc) {
            case Upgrade.UpgradeClass.Damage: {
                return Upgrade_Damage;
            }
            case Upgrade.UpgradeClass.Health: {
                return Upgrade_Health;
            }
            case Upgrade.UpgradeClass.Utility: {
                return Upgrade_Utility;
            }
        }
        return null;
    }

    public void PickUpgradeOption1() {
        if (Option1 != null) ApplyUpgrade(Option1);
    }

    public void PickUpgradeOption2() {
        if (Option2 != null) ApplyUpgrade(Option2);
    }

    private void ApplyUpgrade(Upgrade upg) {
        if (upg.type != Upgrade.UpgradeType.Null) {
            upgrades_taken++;
            UpdateUpgradePointCounter();

            if (GottenUpgrade.ContainsKey(upg.type)) {
                GottenUpgrade[upg.type]++;
            }
            else {
                GottenUpgrade.Add(upg.type, 1);
            }

            HandleUpgrades(upg.type);
        }

        if (upgrades_taken < WaveManager.enemy_cycle) {
            GenerateNewOptions();
        }
        else {
            ClearOptions();
        }
    }

    public void ClearOptions() {
        Option1 = null;
        Option2 = null;
        op1_text.text = "";
        op2_text.text = "";
        op1_title.text = "";
        op2_title.text = "";
        op1_upgrade_sprite.sprite = Upgrade_Null;
        op2_upgrade_sprite.sprite = Upgrade_Null;
        op1_marker_sprite.sprite = Upgrade_Null;
        op2_marker_sprite.sprite = Upgrade_Null;
    }

    private void HandleUpgrades(Upgrade.UpgradeType u_type) {
        switch (u_type) {
            case Upgrade.UpgradeType.DamageUp: {
                damage *= 1.5f;
                break;
            }
            case Upgrade.UpgradeType.DamageGrowth: {
                dam_percent_per_level += 0.1f;
                damage += 0.1f * level;
                break;
            }
            case Upgrade.UpgradeType.HealthUp: {
                max_health *= 1.5f;
                ChangeHealth(max_health*0.33f);
                break;
            }
            case Upgrade.UpgradeType.HealthGrowth: {
                hp_per_level += 2;
                max_health += level*2;
                ChangeHealth(level*2);
                break;
            }
            case Upgrade.UpgradeType.Heal: {
                ChangeHealth(max_health);
                break;
            }
            case Upgrade.UpgradeType.ExpMulti: {
                exp_multi += 0.2f;
                break;
            }
            case Upgrade.UpgradeType.ShootSpeedUp: {
                shoot_speed -= shoot_speed * 0.05f;
                break;
            }
            case Upgrade.UpgradeType.ShootSpeedGrowth: {
                shoot_speed_decrese_per_level += 0.003f;
                shoot_speed -= (shoot_speed * 0.003f * level); // their are some loses here (mybe), but should be fine, bc shoot speed is already op
                break;
            }
            case Upgrade.UpgradeType.SpeedUp: {
                speed += _speed*0.1f;
                break;
            }
            case Upgrade.UpgradeType.SpeedGrowth: {
                speed_percent_per_level += 0.01f;
                speed += 0.01f * level;
                break;
            }
            case Upgrade.UpgradeType.PickUpRange: {
                exp_range += 1f;
                break;
            }
            case Upgrade.UpgradeType.MoreBullets: {
                weapon.bulletCount += 1;
                break;
            }
            case Upgrade.UpgradeType.BulletSpread: {
                weapon.bulletArc += 10;
                break;
            }
            case Upgrade.UpgradeType.PierceBullet: {
                weapon.pierce += 1;
                break;
            }
            case Upgrade.UpgradeType.LevelUp: {
                LevelUp();
                ChangeExp(0);
                break;
            }
            case Upgrade.UpgradeType.NextExpDown: {
                next_level_exp *= 0.85f;
                ChangeExp(0);
                break;
            }
            case Upgrade.UpgradeType.ChainHits:
            {
                weapon.eleChain += 1;
                break;
            }
            case Upgrade.UpgradeType.Blades: {
                Instantiate(PrefabLib.p_blade, transform);
                break;
            }
        }
    }
}
