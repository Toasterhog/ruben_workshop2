using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject thingToFollow;
    [SerializeField] private float speed = 0.8f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = thingToFollow.transform.position - transform.position;
        float amount = Mathf.Min(speed * move.sqrMagnitude * Time.deltaTime, 1.0f);
        transform.Translate(amount * move);
    }
}
