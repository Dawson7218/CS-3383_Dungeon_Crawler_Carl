using UnityEngine;

public class BackgroundParalax : MonoBehaviour
{
    private float startPos, length, height;
    public GameObject camera;
    public float parallaxEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void FixedUpdate()
    {
        UpdateParallax(camera.transform.position.x, camera.transform.position.y);
    }

    private void UpdateParallax(float posX, float posY)
    {
        float distancex = posX * parallaxEffect;
        float distancey = posY * parallaxEffect;
        float movementx = posX * (1 - parallaxEffect);
        float movementy = posY * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distancex, startPos + distancey, transform.position.z);

        if(movementx > startPos + length) {
            startPos += length;
        }
        else if (movementx < startPos - length)
        {
            startPos -= length;
        }

        if(movementy > startPos + height) {
            startPos += height;
        }
        else if (movementy < startPos - height)
        {
            startPos -= height;
        }
    }
}
