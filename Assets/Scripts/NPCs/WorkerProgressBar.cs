using UnityEngine;
using UnityEngine.UI;

public class WorkerProgressBar : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;       // Drag the "Fill" image of the slider here

    [Header("Colors")]
    public Color harvestingColor = Color.yellow;
    public Color fullColor = Color.yellowNice;

    private Transform target;

    public void Setup(Transform workerTarget)
    {
        target = workerTarget;
        fillImage.color = harvestingColor;
        slider.value = 0;
    }

    void Update()
    {
        // Make it hover above the worker just like the health bar!
        if (target != null)
        {
            transform.position = target.position + Vector3.up * 2.5f;
            transform.rotation = Camera.main.transform.rotation;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateProgress(float currentTimer, float maxTime)
    {
        slider.maxValue = maxTime;
        slider.value = currentTimer;
    }

    public void SetFullState(bool isFull)
    {
        if (isFull)
        {
            slider.value = slider.maxValue;
            fillImage.color = fullColor;
        }
        else
        {
            slider.value = 0;
            fillImage.color = harvestingColor;
        }
    }
}
