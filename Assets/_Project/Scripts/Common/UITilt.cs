using System;
using UnityEngine;

public class UITilt : MonoBehaviour
{
    public RectTransform target;
    public float tilt = 10f;

    public bool chto;

  public void Start()
    {
        if (target == null)
        {
            target = GetComponent<RectTransform>();
        }

        r = target.localRotation;
        p = target.localPosition;

        if (chto) tilt *= 4f;
    }

    private Quaternion r;
    private Vector3 p;

    void Update()
    {
        // if (target == null) Start();
        
        Vector2 mouse = Input.mousePosition;
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 dir = (mouse - center) / Screen.width;

        float rotX =  dir.y * tilt;
        float rotY = -dir.x * tilt;

        if (!chto)
        {
            target.localRotation = Quaternion.Euler(rotX, rotY, 0f);
            target.localPosition = new Vector3(rotY, -rotX, 0f);
        }
        else
        {
            target.localRotation = r * Quaternion.Euler(rotX, rotY, 0f);
            target.localPosition = p + new Vector3(rotY, -rotX, 0f);
        }
    }
}