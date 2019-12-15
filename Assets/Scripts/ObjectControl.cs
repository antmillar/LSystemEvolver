using UnityEngine;

//allows user to rotate, zoom object when zoomed in on specific L system in GUI
public class ObjectControl : MonoBehaviour
{
    //rotation speeds
    float _xSpeed = 1.5f;
    float _ySpeed = 1.5f;

    float _x = 0.0f;
    float _y = 0.0f;

    //zoom settings
    float _zoom = 2.0f;
    float _zoomSpeed = 0.01f;
    float _zoomMin = 1f;
    float _zoomMax = 5f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;
    }

    //after everything has loaded, set up the controls
    void LateUpdate()
    {
        //rotation controls

        //if middle mouse down, allow rotation
        if (Input.GetMouseButton(2))
        {
            _x += Input.GetAxis("Mouse X") * _xSpeed;
            _y -= Input.GetAxis("Mouse Y") * _ySpeed;
        }

        //rotate x using left arrow
        if (Input.GetKey(KeyCode.LeftArrow))
            _x -= _xSpeed;

        //rotate x using right arrow
        if (Input.GetKey(KeyCode.RightArrow))
            _x += _xSpeed;

        //rotate y using up arrow
        if (Input.GetKey(KeyCode.UpArrow))
            _y += _ySpeed;

        //rotate y using down arrow
        if (Input.GetKey(KeyCode.DownArrow))
            _y -= _ySpeed;


        //assign rotation
        Quaternion rotation = Quaternion.Euler(_y, _x, 0);
        this.transform.rotation = rotation;


        //zoom controls

        //zoom using mouse scroll
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            _zoom -= Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed * 100f;

        //smoother zoom in using w key
        if (Input.GetKey("w"))
            _zoom -= _zoomSpeed;

        //smoother zoom out using s key
        if (Input.GetKey("s"))
            _zoom += _zoomSpeed;


        //assign zoom
        _zoom = ClampZ(_zoom, _zoomMin, _zoomMax);
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, _zoom);

    }

    //clamp the zoom so object doesn't go off screen
    public float ClampZ(float zoom, float min, float max)
    {
        if (zoom < min)
            zoom = min;
        if (zoom > max)
            zoom = max;
        return Mathf.Clamp(zoom, min, max);
    }
}