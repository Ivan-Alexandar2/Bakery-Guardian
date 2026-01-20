using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Resources")]
    public float wood;
    public float stone;
    public float bread;
    public float fish;
    public float gems;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI breadText;
    [SerializeField] private TextMeshProUGUI fishText;
    [SerializeField] private TextMeshProUGUI gemsText;


    void Start()
    {
        wood = 5;
        bread = 20;
    }

    void Update()
    {
        woodText.text = wood.ToString();
        stoneText.text = stone.ToString();
        breadText.text = bread.ToString();
        fishText.text = fish.ToString();
        gemsText.text = gems.ToString();
    }
}
