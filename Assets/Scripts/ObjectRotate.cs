﻿using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    float _xSpeed = 5.0f;
    float _ySpeed = 5.0f;

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
            //_y -= _ySpeed;
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            this.transform.rotation = rotation;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            _x += _xSpeed;
            //_y += _ySpeed;
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