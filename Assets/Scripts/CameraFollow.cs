using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float offsetY = 4f;

    private Vector3 startPos;
    private float highestY;

    private void Start()
    {
        startPos = transform.position;
        highestY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float targetY = target.position.y + offsetY;
        if (targetY > highestY)
            highestY = targetY;

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, highestY, smoothSpeed * Time.deltaTime);
        pos.x = 0f;
        transform.position = pos;
    }

    public void ResetCamera(float startY)
    {
        highestY = startY;
        transform.position = new Vector3(0f, startY, -10f);
    }

    public void ResetCamera()
    {
        highestY = startPos.y;
        transform.position = new Vector3(0f, startPos.y, -10f);
    }
}
