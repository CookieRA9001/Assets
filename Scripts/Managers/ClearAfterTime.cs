using UnityEngine;

public class ClearAfterTime : MonoBehaviour {
    public float time = 1f;

    void Update() {
        if (time > 0) {
            time -= Time.deltaTime;
        }
        else {
            Destroy(gameObject);
        }
    }
}
