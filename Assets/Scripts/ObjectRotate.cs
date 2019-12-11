using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    float _xSpeed = 1.5f;
    float _ySpeed = 1.5f;

    float _x = 0.0f;
    float _y = 0.0f;

    float _zoomSpeed = 1f;
    float _zoomMin = 1f;
    float _zoomMax = 5f;


    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            _x += Input.GetAxis("Mouse X") * _xSpeed;
            _y -= Input.GetAxis("Mouse Y") * _ySpeed;
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            this.transform.rotation = rotation;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _x -= _xSpeed;
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            this.transform.rotation = rotation;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            _x += _xSpeed;
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            this.transform.rotation = rotation;
        }


        if (Input.GetKey(KeyCode.UpArrow))
        {
            _y += _ySpeed;
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            this.transform.rotation = rotation;
        }


        if (Input.GetKey(KeyCode.DownArrow))
        {
            _y -= _ySpeed;
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            this.transform.rotation = rotation;
        }


        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float zoom = -Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
            this.transform.localPosition += new Vector3(0, 0, zoom);
            float zPos = this.transform.localPosition.z;
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, ClampZ(zPos, _zoomMin, _zoomMax));
        }

        if (Input.GetKey("w"))
        {
            float zoom = 0f;
            zoom -= 0.01f * _zoomSpeed;
            this.transform.localPosition += new Vector3(0, 0, zoom);
            float zPos = this.transform.localPosition.z;
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, ClampZ(zPos, _zoomMin, _zoomMax));
        }

        if (Input.GetKey("s"))
        {
            float zoom = 0f;
            zoom += 0.01f * _zoomSpeed;
            this.transform.localPosition += new Vector3(0, 0, zoom);
            float zPos = this.transform.localPosition.z;
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, ClampZ(zPos, _zoomMin, _zoomMax));
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