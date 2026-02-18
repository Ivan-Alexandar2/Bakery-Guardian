using UnityEngine;

public class ResourceDepot : MonoBehaviour
{

    public void DepositResources(int amount, ResourceType type)
    {
        FindObjectOfType<GameManager>().AddResource(type, amount);
    }
}
