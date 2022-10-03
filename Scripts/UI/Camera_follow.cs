using UnityEngine;

public class Camera_follow : MonoBehaviour{
    //public Transform target;
    public float smoothing = 0.25f;

    void FixedUpdate(){
        if (Player.PlayerRef) {
            Vector3 desPosito = new Vector3(Player.PlayerRef.transform.position.x, Player.PlayerRef.transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desPosito, smoothing);
        }
    }
}
