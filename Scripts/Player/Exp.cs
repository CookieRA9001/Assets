using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour {
    public int value;

    [SerializeField]
    private Rigidbody2D rb2d;
    [SerializeField]
    private SpriteRenderer sprite;

    private void OnTriggerStay2D(Collider2D collision) {
        Player p = collision.gameObject.GetComponent<Player>();

        if (p) {
            p.ChangeExp(value);
            Player.audio_m.Play("xpPickup");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        IDictionary<int, List<Exp>> dict = new Dictionary<int, List<Exp>>();

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.transform.position, 2);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "xp")
            {
                Exp xp = hitCollider.gameObject.GetComponent<Exp>();
                if (!dict.ContainsKey(xp.value)) dict[xp.value] = new List<Exp>();
                if (xp != this) dict[xp.value].Add(xp);
                if (dict[xp.value].Count >= 4)
                {
                    for(int i = 0; i < 4; i++)
                    {
                        Exp tmp = dict[xp.value][0];
                        dict[xp.value].RemoveAt(0);
                        Destroy(tmp.gameObject);
                    }
                    value = xp.value * 5;
                }
            }
        }

        //sprite.color = Player.PlayerRef.gm.IntColorDictionary[value];
        int tmp_v = value;
        while (tmp_v >= 78125) tmp_v = (int)(tmp_v * 0.0000128f);
        sprite.sprite =  Player.PlayerRef.gm.getSrpite(tmp_v).sprite;
    }

    private void Update()
    {
        if (Player.PlayerRef)
        {
            Vector3 playerPos = Player.PlayerRef.transform.position;
            Vector3 e_pos = transform.position;

            float dist = Vector3.Distance(playerPos, e_pos);
            if (dist > 75) Destroy(this.gameObject);
            else if (dist < Player.PlayerRef.exp_range)
            {
                Vector3 dir = (playerPos - e_pos).normalized;

                rb2d.velocity = dir * 20;
            }
        }
    }
}
