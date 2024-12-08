using UnityEngine;
using UnityEngine.UI; // For UI elements
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public float slowDownFactor = 0.05f;
    public Slider bulletTimeSlider; // Reference to the slider
    public float sliderDepletionRate = 0.1f; // How fast the slider depletes per second

    private bool isBulletTimeActive = false;
    private bool isBulletTimeDepleted = false; // Track if Bullet Time is permanently unavailable
    private float bulletTimeElapsed = 0f;

    private void Update()
    {
        // Only allow Bullet Time if not in cooldown, slider has value, and it's not permanently depleted
        

        // Manage Bullet Time
        if (isBulletTimeActive)
        {
            Time.timeScale = slowDownFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            bulletTimeElapsed += Time.unscaledDeltaTime;
            bulletTimeSlider.value -= sliderDepletionRate * Time.unscaledDeltaTime; // Deplete slider

            // Stop Bullet Time if slider is empty and mark it as depleted
            if (bulletTimeSlider.value <= 0)
            {
                bulletTimeSlider.value = 0; // Clamp to zero
                isBulletTimeDepleted = true; // Permanently disable Bullet Time
                StopBulletTime();
            }
        }
    }

    public void StartBulletTime()
    {
        isBulletTimeActive = true;
        bulletTimeElapsed = 0f;
    }

    public void StopBulletTime()
    {
        isBulletTimeActive = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    
}
