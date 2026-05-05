using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Rigidbody2D target;
    public float smoothSpeed = 0.1f;
    public Vector3 offset = new Vector3(0f, 2f, 0f);

    void Start()
    {
        // Se ninguém setou o target ainda, tenta achar o Player na cena
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.GetComponent<Rigidbody2D>();
            }
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        ) + offset;

        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed);
    }

    // Chamado pelo PlayerSpawner depois de instanciar o personagem
    public void SetTarget(Rigidbody2D newTarget)
    {
        target = newTarget;
    }
}