using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FaceMovement : MonoBehaviour
{
    public FaceAI.Modifier modifier = FaceAI.Modifier.NONE;

    [Header("Helix Movement Variables")]
    public Vector2 forwardDirection = Vector2.up; //main direction
    public float speed = 2f;
    public float frequency = 2f;
    public float amplitude = 1f;

    private float time;
    private Vector2 perpendicular;

    [Header("Random Movement Variables")]
    public float x = 5;
    public float y = 5;

    [Header("Other Variables")]
    public bool movementLocked = false;
    public float halfWidth = 9.8f;
    public float halfHeight = 5.5f;

    void Start()
    {
        perpendicular = new Vector2(-forwardDirection.y, forwardDirection.x).normalized;
    }
    private void Update()
    {
        OutOfBoundsCheck();
        if (!movementLocked)
        {
            switch (modifier)
            {
                case FaceAI.Modifier.NONE:
                    break;
                case FaceAI.Modifier.HELIX:
                    HelixMovement();
                    break;
                case FaceAI.Modifier.RANDOM:
                    RandomMovement();
                    break;
            }
        }
    }

    //May need to be tweaked with new size
    public void OutOfBoundsCheck()
    {
        float halfSizeX = transform.localScale.x / 2f;
        float halfSizeY = transform.localScale.y / 2f;

        Vector3 pos = transform.position;

        //horizontal wrap
        if (pos.x > halfWidth + halfSizeX)
            pos.x -= (halfWidth * 2f + transform.localScale.x);

        else if (pos.x < -halfWidth - halfSizeX)
            pos.x += (halfWidth * 2f + transform.localScale.x);

        //vertical wrap
        if (pos.y > halfHeight + halfSizeY)
            pos.y -= (halfHeight * 2f + transform.localScale.y);

        else if (pos.y < -halfHeight - halfSizeY)
            pos.y += (halfHeight * 2f + transform.localScale.y);

        transform.position = pos;
    }

    private void HelixMovement()
    {
        time += Time.deltaTime;

        //forward movement
        Vector2 forward = forwardDirection.normalized * speed * Time.deltaTime;

        //helix offset
        Vector2 sway = perpendicular * Mathf.Sin(time * frequency) * amplitude * Time.deltaTime;

        transform.Translate(forward + sway);
    }

    //HARD CODED FOR NOW
    public void ConfigRandomMovement(float xx, float yy)
    {
        this.x = xx * 0.01f;
        this.y = yy * 0.01f;
    }

    private void RandomMovement()
    {
        transform.position += new Vector3(x, y, 0);
    }

}
