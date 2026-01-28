using UnityEngine;

public class ResourceDepot : MonoBehaviour
{
    // We can add "Storage Capacity" logic here later.
    // For now, it just acts as a valid destination tag.

    public void DepositResources(int amount, ResourceType type)
    {
        // One-liner to find the global manager and pay up
        // Note: You might need to make your GameManager a Singleton (Instance) 
        // or find it dynamically.
        FindObjectOfType<GameManager>().AddResource(type, amount);
    }
}
