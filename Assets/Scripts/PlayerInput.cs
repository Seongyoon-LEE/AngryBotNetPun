using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string jumpButton = "Jump";
    public string fireButton = "Fire1";
    public float h = 0f;
    public float v = 0f;
    public bool isMouseClick = false;
    void Start()
    {
        
    }

    void Update()
    {
        h = Input.GetAxis(horizontalAxis);
        v = Input.GetAxis(verticalAxis);
        isMouseClick = Input.GetButtonDown(fireButton);
    }
}
