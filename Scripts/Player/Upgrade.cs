using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Upgrade {
    public enum UpgradeType {
        Null,
        DamageUp,
        DamageGrowth,
        HealthUp,
        HealthGrowth,
        Heal,
        ExpMulti,
        ShootSpeedUp,
        ShootSpeedGrowth,
        SpeedUp,
        SpeedGrowth,
        PickUpRange,

        MoreBullets,
        BulletSpread,
        PierceBullet,
        LevelUp,
        NextExpDown,
        ChainHits,
        Blades
    };

    public enum UpgradeClass {
        Damage,
        Health,
        Utility,
    };

    public UpgradeType type;
    public string title;
    public string description;
    public UpgradeClass sub_class;
}
