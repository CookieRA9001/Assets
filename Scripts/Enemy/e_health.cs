using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_health : MonoBehaviour{
    public Enemy e;
    public Color full_color, low_color, base_color;
    private SpriteRenderer sr;
    private float last_resio;
    
    void Awake(){
        sr = GetComponent<SpriteRenderer>();
        last_resio = 0;
    }
    void Update(){
        if(e){
            if(last_resio != e.health/e.max_health){
                last_resio = e.health/e.max_health;
                if (last_resio < 0) last_resio = 0;

                transform.localScale = new Vector3(last_resio, transform.localScale.y, 0);

                if (last_resio == 1) {
                    sr.color = full_color;
                }
                else{
                    if(last_resio < 0.25f){
                        sr.color = low_color;
                    }
                    else{
                        sr.color = base_color;
                    }
                }
            }
        }
    }
}
