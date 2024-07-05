using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUI : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float height = 5f;
    private RectTransform transf;
    private float startingPos; 

    private void Start()
    {
        transf = GetComponent<RectTransform>();
        startingPos = transf.anchoredPosition.y; 
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * height;

        Vector2 newPos = transf.anchoredPosition;
        newPos.y = startingPos + offset; 
        transf.anchoredPosition = newPos;
    }
}
 