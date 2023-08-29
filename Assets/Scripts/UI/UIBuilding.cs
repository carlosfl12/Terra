using UnityEngine;

public class UIBuilding : MonoBehaviour
{
    public BuildingSO[] buildingPrefabs; // Prefabs de los edificios
    private GameObject pendingObject;
    public Vector3 pos;
    public LayerMask groundLayer; // Capa del suelo para raycast

    public RaycastHit hit;

    private void Update() {
        if (pendingObject != null) {
            pendingObject.transform.position = pos;
            
            if (Input.GetMouseButtonDown(0)) {
                PlaceObject();
            }
        }
    }
    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, groundLayer)) {
            pos = hit.point;   
        }
    }

    public void PlaceObject() {
        pendingObject = null;
    }
    public void SelectObject(int Index) {
        if (!CanBuild(buildingPrefabs[Index].stoneCost, buildingPrefabs[Index].lumberCost)) return;
        pendingObject = Instantiate(buildingPrefabs[Index].buildingPrefab, pos, transform.rotation);
        // Substract resources
        GameManager.sharedInstance.RemoveStoneAmount(buildingPrefabs[Index].stoneCost);
        GameManager.sharedInstance.RemoveWoodAmount(buildingPrefabs[Index].lumberCost);
    }

    public bool CanBuild(int StoneCost, int LumberCost) {
        return StoneCost <= GameManager.sharedInstance.GetStoneAmount() && LumberCost <= GameManager.sharedInstance.GetWoodAmount();
    }

    
}
