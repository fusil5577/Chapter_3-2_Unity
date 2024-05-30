using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public Transform platform;
    public Transform startPos;
    public Transform endPos;
    public float moveSpeed = 2f;

    private bool movingToEnd = true;

    void Update()
    {
        if (movingToEnd)
        {
            platform.position = Vector3.MoveTowards(platform.position, endPos.position, moveSpeed * Time.deltaTime);
            if (platform.position == endPos.position)
                movingToEnd = false;
        }
        else
        {
            platform.position = Vector3.MoveTowards(platform.position, startPos.position, moveSpeed * Time.deltaTime);
            if (platform.position == startPos.position)
                movingToEnd = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = platform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
