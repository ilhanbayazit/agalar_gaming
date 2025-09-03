using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Yatay ve dikey input alıyoruz (WASD & ok tuşları)
        float yatay = Input.GetAxisRaw("Horizontal"); // A(-1) D(+1)
        float dikey = Input.GetAxisRaw("Vertical");   // S(-1) W(+1)

        // Yön vektörü
        Vector3 hareket = new Vector3(yatay, 0f, dikey).normalized;

        // Hareket uygula
        transform.position += hareket * speed * Time.deltaTime;
    }
}
