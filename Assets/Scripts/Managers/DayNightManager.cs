using System;
using System.Collections;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    public static DayNightManager Instance; // Singleton for easy access

    [Header("Settings")]
    public float dayDuration = 240f;   // Seconds for Day
    public float nightDuration = 120f; // Seconds for Night

    [Header("Info")]
    public int currentDay = 1;
    public bool isNight = false;

    private float timer;

    // THE EVENTS - Units will listen to these!
    public event Action OnDayStart;
    public event Action OnNightStart;

    public Light sunLight;
    public float transitionTime = 10f; // Time to rotate

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!isNight && timer >= dayDuration)
        {
            StartNight();
        }
        else if (isNight && timer >= nightDuration)
        {
            StartDay();
        }
    }

    void StartNight()
    {
        isNight = true;
        timer = 0;

        OnNightStart?.Invoke(); // Shout to everyone!
        StartCoroutine(TransitionSun(240f)); // was 180
    }

    void StartDay()
    {
        isNight = false;
        currentDay++;
        timer = 0;
        OnDayStart?.Invoke();

        StartCoroutine(TransitionSun(50f));
    }

    private IEnumerator TransitionSun(float targetAngleX)
    {
        Quaternion startRotation = sunLight.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetAngleX, -30f, 0f); // -30y gives nice shadows

        float elapsedTime = 0;

        while (elapsedTime < transitionTime)
        {
            // Lerp = Linear Interpolation (Move smoothly from A to B)
            sunLight.transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / transitionTime);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // Ensure we end exactly at the target
        sunLight.transform.rotation = endRotation;
    }
}
