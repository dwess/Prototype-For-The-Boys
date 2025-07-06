using UnityEngine;

public class Wall : MonoBehaviour
{
    
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        // handle hitting walls/enemies
        Rigidbody2D rb2d = other.gameObject.GetComponent<Rigidbody2D>();
        rb2d.linearVelocity = Vector3.zero;
        
        Debug.Log(rb2d.linearVelocity);
        Debug.Log("yes");
    }
}
