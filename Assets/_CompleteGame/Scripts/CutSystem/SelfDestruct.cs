using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        
        Invoke("DestroySelf", 1f);
        
        // if (collision.gameObject.CompareTag("Ground")) // Assuming the ground has a tag "Ground"
        // {
        //     Invoke("DestroySelf", 1f);
        // }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}