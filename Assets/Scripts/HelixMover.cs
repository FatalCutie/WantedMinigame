using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixMover : MonoBehaviour
{
    public float speed = 2f;        //forward speed
    public float frequency = 2f;    //how fast it sways
    public float amplitude = 1f;    //how wide the sway is

    private float time;

    void Update()
    {
        OutOfBoundsCheck();
        time += Time.deltaTime;

        //forward movement (upward on Y)
        float y = transform.position.y + speed * Time.deltaTime;

        //sway on X axis using sine
        float x = Mathf.Sin(time * frequency) * amplitude;

        transform.position = new Vector3(x, y, 0);
    }

    public void OutOfBoundsCheck()
    {
        if (transform.position.x > 9.8) transform.position = new Vector3(-9.8f, transform.position.y, 0);
        else if (transform.position.x < -9.8) transform.position = new Vector3(9.8f, transform.position.y, 0);
        if (transform.position.y >= 5.5) transform.position -= new Vector3(0, 11f, 0);
        else if (transform.position.y <= -5.5) transform.position += new Vector3(0, 11f, 0);
    }
}
