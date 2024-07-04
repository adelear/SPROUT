using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] Transform[] movingPoints;
    [SerializeField] float movingSpeed = 5f; 
    private int currentPos = 0;
    private int direction = 1; 

    void Start()
    {
        currentPos = 0; 
        transform.position = movingPoints[currentPos].position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movingPoints[currentPos].position, movingSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, movingPoints[currentPos].position) < 0.1f)
        {
            currentPos += direction;
            if (currentPos >= movingPoints.Length || currentPos < 0)
            {
                // Reverse direction
                direction *= -1;
                currentPos = Mathf.Clamp(currentPos, 0, movingPoints.Length - 1);
            }
        }
    }
}
