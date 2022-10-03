using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    public float shaking = 0;

    public IEnumerator Shake(float time, float strength) {
        if (shaking >= strength) {
            yield return null;
        }

        shaking = strength;

        float elapsed = 0;

        while (elapsed < time) {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            transform.localPosition = new Vector3(x, y, transform.localPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = new Vector3(0,0, transform.localPosition.z);
        shaking = 0;
    }
}
