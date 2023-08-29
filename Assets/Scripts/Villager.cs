using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Villager : MonoBehaviour
{
    public enum VillagerState
    {
        Idle,
        Working,
        Resting,
        Socializing,
        Eating
    };

    public enum VillagerJobs {
        None,
        Lumberjack,
        Miner,
        Farmer,
        Builder
    };

    // Villager Properties
    [Header("Villager Properties")]
    [SerializeField] float maxEnergy = 100f;
    [SerializeField] float maxHunger = 100f;
    [SerializeField] float energyDecreaseRate = 0.1f;
    [SerializeField] float energyThresholdForRest = 30f;
    [SerializeField] float currentEnergy;
    [SerializeField] float currentHunger; 
    [SerializeField] bool canRest;
    [SerializeField] bool hasHouse;
    [SerializeField] bool isStopped;
    [SerializeField] Housing currentHouse;
    [SerializeField] int amountToCarry = 20;
    // [SerializeField] Renderer jobColor;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] bool canDoAction;
    [SerializeField] Animator animator;

    [Header("Resources")]
    [SerializeField] int woodCount;
    [SerializeField] int stoneCount;
    [SerializeField] int foodCount;
    [SerializeField] int collectableAmount = 2;

    [Header("Villager States")]
    //Villager State
    public VillagerState currentState;
    public VillagerJobs currentJob;
    public Resources currentJobResource;
    [SerializeField] Vector3 jobPosition;


    public bool availableForWork;
    
    public List<Building> availableBuilding = new List<Building>();
    public Building currentBuilding;

    /**
    * TODO: AI MOVEMENT AND MORE, MAX 3 VILLAGERS IN FARM POSITION, IF VILLAGER DOESNT HAVE JOBRESOURCE STAY IN IDLE, ADD MODELS AND ANIMS
    */
    // Start is called before the first frame update
    void Start()
    {
        currentEnergy = maxEnergy;
        currentHunger = maxHunger;
        InvokeRepeating("UpdateVillager", 1f, 1f);
        // jobColor = GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected virtual void UpdateVillager() {     
        currentHunger -= 0.1f;
        currentEnergy -= 0.1f;

        switch (currentState)
        {
            case VillagerState.Idle:
                availableForWork = true;
                jobPosition = Vector3.zero;
                // TODO:  Decide Job or do other things
                DecideJob();
                break;
            case VillagerState.Working:
                availableForWork = false;
                break;
            case VillagerState.Resting:
                canRest = true;
                StartCoroutine(Rest());
                break;
            case VillagerState.Eating:
                Vector3 buildingPosition = GetClosestBuildingOfType(Building.BuildingType.Storaging).transform.position;
                agent.SetDestination(buildingPosition);
                CloseEnoughForAction(buildingPosition, 2.5f);
                if (canDoAction) {
                    StartCoroutine(Eat());
                }
                break;
            default:
                break;
        }

        if ((woodCount >= amountToCarry || foodCount >= amountToCarry || stoneCount >= amountToCarry) && currentState == VillagerState.Working) {;
            Vector3 buildingPosition = GetClosestBuildingOfType(Building.BuildingType.Storaging).transform.position;
            animator.SetBool("isStopped", false);
            isStopped = false;
            agent.isStopped = false;
            agent.SetDestination(buildingPosition);
            CloseEnoughForAction(buildingPosition, 0.5f);
            if (canDoAction) {
                StoreResources();
                DecideJob();
            }
            // Play animation and decide new job
        }

        if (currentJob == VillagerJobs.Builder && GameManager.sharedInstance.availableBuildings.Count > 0) {
            FindAvailableBuilding();
            float distance = Vector3.Distance(transform.position, currentBuilding.transform.position);
            DecideJob();
            if (distance <= 3f) {
                Build(currentBuilding);
            }
        } else if (currentJob == VillagerJobs.Builder && GameManager.sharedInstance.availableBuildings.Count <= 0) {
            DecideJob();
        }

        CheckVillagerNeeds();
    }

    void Build(Building Building) {
        Building.IncreaseConstructionProgress(5f);
        if (!Building.IsAvailableForWork()) {
            Debug.Log("<color=red> " + gameObject.name +  " </color>Is not available to build ");
            GameManager.sharedInstance.availableBuildings.Remove(Building);
            currentBuilding = null;
            currentState = VillagerState.Idle;
        }
        
    }

    void FindAvailableBuilding() {
        // TODO: Available building = Near random available building
        Building[] buildings = FindObjectsOfType<Building>();
        foreach (Building building in buildings) {
            if (building.IsAvailableForWork()) {
                if (!GameManager.sharedInstance.availableBuildings.Contains(building)) {
                    GameManager.sharedInstance.availableBuildings.Add(building);
                }
                currentBuilding = building;
                currentState = VillagerState.Working;
            }
        }
    }

    IEnumerator Rest() {
        agent.SetDestination(currentHouse.transform.position);
        yield return new WaitForSeconds(5f);
        if (canRest) {
            currentEnergy += energyThresholdForRest;
        }
        canRest = false;
        currentState = VillagerState.Idle;
        currentJob = VillagerJobs.None;

    }

    IEnumerator Eat() {
        // TODO: Eating animaton
        yield return new WaitForSeconds(5f);
        if (canDoAction) {
            currentHunger += 30f;
        }
        canDoAction = false;
        currentState = VillagerState.Idle;
        currentJob = VillagerJobs.None;
    }

    public void Gather() {
        if (currentJob == VillagerJobs.Builder || !currentJobResource) return;
        if (isStopped) {
            currentJobResource.GiveResource(currentJobResource.resourceType, collectableAmount, this);
            CollectResource(currentJobResource.resourceType, collectableAmount, currentJob);
        }
        agent.isStopped = true;
        Debug.Log("Gathering");

    }

    void CheckVillagerNeeds() {
        if (currentHunger <= 0) {
            currentState = VillagerState.Eating;
        }
        else if (currentBuilding == null && currentJob == VillagerJobs.None ) {
            currentState = VillagerState.Idle;
        }
        else if (currentEnergy < energyThresholdForRest) {
            currentState = VillagerState.Resting;
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Refactor later to other function this is for testing
        // Play gather anim 
        Resources resource = other.gameObject.GetComponent<Resources>();
        if (resource != null && resource.resourceType == currentJobResource.resourceType) {
            isStopped = true;
            animator.SetBool("isStopped", true);
            Debug.Log(gameObject.name + " Colliding with <color=green> " + resource.name + " </color>");
        }

        if (other.gameObject.CompareTag("Door")) {
            canDoAction = true;
        }
    }

    void CollectResource(Resources.ResourceType ResourceType, int Amount, VillagerJobs JobType) {
        switch (ResourceType) {
            case Resources.ResourceType.Wood when JobType == VillagerJobs.Lumberjack:
                woodCount += Amount;
                break;
            case Resources.ResourceType.Food when JobType == VillagerJobs.Farmer:
                foodCount += Amount;
                break;
            case Resources.ResourceType.Stone when JobType == VillagerJobs.Miner:
                stoneCount += Amount;
                break;
        }
    }

    public void DecideJob() {
        // Count jobs
        int lumberjackCount = GetJobCount(VillagerJobs.Lumberjack);
        int minerCount = GetJobCount(VillagerJobs.Miner);
        int farmerCount = GetJobCount(VillagerJobs.Farmer);
        int builderCount = GetJobCount(VillagerJobs.Builder);

        float foodPreference = CalculateResourcePreference(VillagerJobs.Farmer);
        float woodPreference = CalculateResourcePreference(VillagerJobs.Lumberjack);
        float stonePreference = CalculateResourcePreference(VillagerJobs.Miner);


        // Find the job with the highest adjusted preference
        VillagerJobs chosenJob = DetermineJobBasedOnPreferences(foodPreference, woodPreference, stonePreference);
        currentJob = chosenJob;

        if (builderCount < 3 && GameManager.sharedInstance.availableBuildings.Count > 0) {
            Debug.LogWarning("Deberian haber constructores");
            currentJob = VillagerJobs.Builder;
        }

        foreach (Resources resources in FindObjectsOfType<Resources>()) {
            if (resources.GetResourceCountOfType(Resources.ResourceType.Wood) <= 0) {
                currentJobResource = null;
                currentState = VillagerState.Idle;
                currentJob = VillagerJobs.None;
            }
        }

        agent.isStopped = false;
        isStopped = false;

        switch (currentJob) {
            case VillagerJobs.Lumberjack:
                Debug.Log(FindNearestResource(transform.position, Resources.ResourceType.Wood).transform.position);
                SetJobPosition(FindNearestResource(transform.position, Resources.ResourceType.Wood).transform.position);
                animator.SetBool("isStopped", false);
                agent.isStopped = false;
                break;
            case VillagerJobs.Farmer:
                SetJobPosition(FindNearestResource(transform.position, Resources.ResourceType.Food).transform.position);
                animator.SetBool("isStopped", false);
                agent.isStopped = false;
                break;
            case VillagerJobs.Miner:
                SetJobPosition(FindNearestResource(transform.position, Resources.ResourceType.Stone).transform.position);
                animator.SetBool("isStopped", false);
                agent.isStopped = false;
                break;
            case VillagerJobs.Builder:
                FindAvailableBuilding();
                SetJobPosition(currentBuilding.transform.position);
                // jobColor.material.color = Color.cyan;
                if (GameManager.sharedInstance.availableBuildings.Count <= 0) {
                    currentBuilding = null;
                }
                break;
        }

        // Stay around the job
        Vector3 randomJobPosition = Random.insideUnitSphere * Random.Range(2.5f, 3f);
        randomJobPosition += jobPosition;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomJobPosition, out hit, 2.5f, NavMesh.AllAreas);
        currentState = VillagerState.Working;
        if (isStopped)return;
        agent.SetDestination(hit.position);
        
           
    }

    VillagerJobs DetermineJobBasedOnPreferences(float foodPreference, float woodPreference, float stonePreference)
    {
        if (foodPreference >= woodPreference && foodPreference >= stonePreference)
        {
            return VillagerJobs.Farmer;
        }
        else if (woodPreference >= foodPreference && woodPreference >= stonePreference)
        {
            return VillagerJobs.Lumberjack;
        }
        else if (stonePreference >= foodPreference && stonePreference >= woodPreference)
        {
            return VillagerJobs.Miner;
        }

        return VillagerJobs.None;
    }

    float CalculateResourcePreference(VillagerJobs JobType) {
        int resourceStored = 0;
        switch (JobType) {
            case VillagerJobs.Lumberjack:
                resourceStored = GameManager.sharedInstance.GetWoodAmount();
                break;
            case VillagerJobs.Miner:
                resourceStored = GameManager.sharedInstance.GetStoneAmount();
                break;
            case VillagerJobs.Farmer:
                resourceStored = GameManager.sharedInstance.GetFoodAmount();
                break;
            case VillagerJobs.Builder:
                resourceStored = availableBuilding.Count * 1000;
                break;
        }

        float resourceWeight = 1f;
        float normalizedResource = (float)resourceStored / GameManager.sharedInstance.GetMaxStorageAmount();

        float resourcePreference = resourceWeight * (1f - normalizedResource);

        foreach (Resources resource in FindObjectsOfType<Resources>()) {
            if (resource.GetResourceCountOfType(Resources.ResourceType.Wood) <= 0 && JobType == VillagerJobs.Lumberjack) {
                resourcePreference = 0;
            }
            else if (resource.GetResourceCountOfType(Resources.ResourceType.Food) <= 0 && JobType == VillagerJobs.Farmer) {
                resourcePreference = 0;
            }
            else if (resource.GetResourceCountOfType(Resources.ResourceType.Stone) <= 0 && JobType == VillagerJobs.Miner) {
                resourcePreference = 0;
            }
        }
        Debug.Log("<color=green>" + JobType.ToString() +  "</color> Preference: " + resourcePreference);

        return resourcePreference;
    }

    public int GetJobCount(VillagerJobs JobType) {
        int count = 0;
        foreach (Villager villager in FindObjectsOfType<Villager>()) {
            if (villager.currentJob == JobType) {
                count++;
            }
        }
        return count;
    }

    public Resources FindNearestResource(Vector3 VillagerPosition, Resources.ResourceType Type) {
        Resources nearestResource = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Resources resource in FindObjectsOfType<Resources>()) {
            float distance = Vector3.Distance(VillagerPosition, resource.transform.position);
            if (resource.resourceType == Type && distance < shortestDistance) {
                shortestDistance = distance;
                nearestResource = resource;
                currentJobResource = resource;
            }

        }
        return nearestResource;
    }

    Transform GetClosestBuildingOfType(Building.BuildingType BuildingType) {
        Transform nearestBuilding = transform;
        float shortestDistance = Mathf.Infinity;

        foreach (Building building in FindObjectsOfType<Building>()) {
            if (building.buildingType == BuildingType && !building.IsUnderConstruction()) {
                float distance = Vector3.Distance(transform.position, building.transform.position);

                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    nearestBuilding = building.transform.Find("DoorPosition");
                }
            }
        }
        return nearestBuilding;
    }

    public void StoreResources() {
        GameManager.sharedInstance.AddFoodAmount(foodCount);
        GameManager.sharedInstance.AddStoneAmount(stoneCount);
        GameManager.sharedInstance.AddStoneAmount(woodCount);
        woodCount = 0;
        foodCount = 0;
        stoneCount = 0;
        canDoAction = false;
    }

    public void CloseEnoughForAction(Vector3 Target, float MinDistance) {
        float distance = Vector3.Distance(transform.position, Target);

        if (distance <= MinDistance) {
            canDoAction = true;
        }
    }
    public bool GetHasHouse() {
        return hasHouse;
    }
    
    public void SetHasHouse(bool Value, Housing House) {
        hasHouse = Value;
        currentHouse = House;
    }

    public void SetJobPosition(Vector3 Position) {
        jobPosition = Position;
    }

    public Vector3 GetJobPosition() {
        return jobPosition;
    }
    public void SetCanDoAction(bool Value) {
        canDoAction = Value;
    }
}
