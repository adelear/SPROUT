using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManualAnimation : MonoBehaviour
{
    public float frameRate = 0.1f;
    public Image[] animationFrames;

    private int currentFrameIndex = 0;
    private float timer;

    void Start()
    {
        ShowFrame(currentFrameIndex);
        timer = frameRate;
    }

    void Update()
    {
        // Update the timer
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Move to the next frame
            currentFrameIndex++;
            if (currentFrameIndex >= animationFrames.Length)
            {
                currentFrameIndex = 0; // Loop back to the start
            } 

            ShowFrame(currentFrameIndex);

            // Reset the timer
            timer = frameRate;
        }
    }

    void ShowFrame(int frameIndex)
    {
        for (int i = 0; i < animationFrames.Length; i++)
        {
            animationFrames[i].gameObject.SetActive(false);
        }
        animationFrames[frameIndex].gameObject.SetActive(true);
    }
}
