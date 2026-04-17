using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager that prevents units from stuttering when trying to find a path due a lot of units on the map
/// </summary>
public class AIPathManager : MonoBehaviour
{
    public static AIPathManager Instance;

    private List<SoldierAI> soldiers = new List<SoldierAI>();
    private int currentIndex = 0;
    public int unitsPerFrame = 5; // Tune this number

    void Awake() => Instance = this;

    public void Register(SoldierAI unit) => soldiers.Add(unit);
    public void Unregister(SoldierAI unit) => soldiers.Remove(unit);

    void Update()
    {
        if (soldiers.Count == 0) return;

        // Only tick a batch of units per frame
        for (int i = 0; i < unitsPerFrame; i++)
        {
            if (currentIndex >= soldiers.Count) currentIndex = 0;
            soldiers[currentIndex].ManagedTick();
            currentIndex++;
        }
    }
}
