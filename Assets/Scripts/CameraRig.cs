using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Drag your Player transform here")]
    public Transform player;

    [Tooltip("Height (and optionally back–forward) offset from the player")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    // Fixed pitch so camera always looks straight down
    private const float pitch = 90f;

    void LateUpdate()
    {
        // if (player == null) return;

        // // 1) Follow position
        transform.position = player.position;// + offset;

        // 2) Match the player’s yaw, preserve our pitch
        float yaw = player.eulerAngles.z;
        transform.rotation = Quaternion.Euler(0f, 0f, yaw);
    }
}
