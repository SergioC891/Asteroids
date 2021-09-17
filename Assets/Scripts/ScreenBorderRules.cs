using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBorderRules : MonoBehaviour
{
    Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    public Vector3 screenBordersRoutine(Transform _transform)
    {
        Vector3 pos = _camera.WorldToScreenPoint(_transform.position);

        if (pos.x < 0.0f)
        {
            pos = _camera.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, 0.0f));
            _transform.position = new Vector3(pos.x, _transform.position.y, 0.0f);
        }
        else if (pos.x > Screen.width)
        {
            pos = _camera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
            _transform.position = new Vector3(pos.x, _transform.position.y, 0.0f);
        }
        else if (pos.y < 0.0f)
        {
            pos = _camera.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, 0.0f));
            _transform.position = new Vector3(_transform.position.x, pos.y, 0.0f);
        }
        else if (pos.y > Screen.height)
        {
            pos = _camera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
            _transform.position = new Vector3(_transform.position.x, pos.y, 0.0f);
        }

        return _transform.position;
    }
}
