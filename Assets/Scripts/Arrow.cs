using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 20f;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}