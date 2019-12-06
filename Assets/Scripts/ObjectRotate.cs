using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    float xSpeed = 120.0f;
    float ySpeed = 120.0f;

    float x = 0.0f;
    float y = 0.0f;

    float zoomSpeed = 1f;
    float zoomMin = 1f;
    float zoomMax = 5f;


    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            this.transform.rotation = rotation;
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            this.transform.localPosition += new Vector3(0, 0, zoom);
            float zPos = this.transform.localPosition.z;
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, ClampZ(zPos, zoomMin, zoomMax));
        }
    }

    public static float ClampZ(float zoom, float min, float max)
    {
        if (zoom < min)
            zoom = min;
        if (zoom > max)
            zoom = max;
        return Mathf.Clamp(zoom, min, max);
    }

}