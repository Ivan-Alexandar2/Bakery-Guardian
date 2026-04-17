using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType
{
    health, damage, attackSpeed, range, unitSpawnCount
}

public struct UpgradeDataStructAAA
{
    [Header("Upgrade Settings")]
    public UpgradeType upgradeType;
    public int upgradeAmount;

    public int maxUpgrades;
    public int currentUpgradeCount;

    [Header("Gem Prices")]
    public int currentCost;
    public int costIncrease;  // How much it goes up each time

    //[Header("UI")]
    //public TextMeshProUGUI costText;
    //public TextMeshProUGUI nameText;
    //public TextMeshProUGUI maxCurrentUpgradesText;
    //public Image upgradeIcon;
    //public Button upgradeButton;
}


public class Upgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    public UpgradeType upgradeType;
    public int upgradeAmount;

    public int maxUpgrades = 5;
    private int currentUpgradeCount = 0;

    [Header("Gem Prices")]
    public int currentCost = 5;
    public int costIncrease = 5;  // How much it goes up each time

    [Header("UI")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI maxCurrentUpgradesText;
    public Image upgradeIcon;
    public Button upgradeButton;

    public UpgradeDataStructAAA data;

    void Start()
    {
        nameText.text = upgradeType.ToString();
        costText.text = currentCost.ToString();
        maxCurrentUpgradesText.text = $"({ currentUpgradeCount}/{ maxUpgrades})";
    }

    void Update()
    {
        if( currentUpgradeCount >= maxUpgrades )
            upgradeButton.interactable = false;
    }

    public void PressUpgrade()
    {
        if (currentUpgradeCount >= maxUpgrades)
        {
            Debug.Log("Max upgrades reached!");
            return;
        }

        List<ResourceCost> costToBuy = new List<ResourceCost>
        {
            new ResourceCost { type = ResourceType.Gems, amount = currentCost } // wtf I didn't know that
        };

        if (GameManager.Instance.TryBuyUpgrade(costToBuy))
        {
            currentUpgradeCount++;
            currentCost += costIncrease;

            costText.text = currentCost.ToString();

            Debug.Log($"{upgradeType} Upgraded! ({currentUpgradeCount}/{maxUpgrades})");
            maxCurrentUpgradesText.text = $"({currentUpgradeCount}/{maxUpgrades})";
            SaveUpdateData(this);

            // ApplyUpgrades();
        }
    }

    public void SaveUpdateData(Upgrade upgrade)
    {
        data.upgradeType = upgrade.upgradeType;
        data.currentCost = upgrade.currentCost;
        data.currentUpgradeCount = upgrade.currentUpgradeCount;
        data.maxUpgrades = upgrade.maxUpgrades;
    }

    public void LoadUpdateData()
    {

    }
}
