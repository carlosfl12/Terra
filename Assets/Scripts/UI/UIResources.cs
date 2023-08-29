using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIResources : MonoBehaviour
{
    [SerializeField] TMP_Text lumberText;
    [SerializeField] TMP_Text stoneText;
    [SerializeField] TMP_Text foodText;
    [SerializeField] TMP_Text villagersText;

    private void Update() {
        lumberText.text = GameManager.sharedInstance.GetWoodAmount().ToString();
        stoneText.text = GameManager.sharedInstance.GetStoneAmount().ToString();
        foodText.text = GameManager.sharedInstance.GetFoodAmount().ToString();
        villagersText.text = GameManager.sharedInstance.GetVillagersAmount().ToString() + "/" + GameManager.sharedInstance.GetAvailableHouseAmount();
    }
}
