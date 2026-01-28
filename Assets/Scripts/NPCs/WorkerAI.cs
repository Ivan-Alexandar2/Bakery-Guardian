using UnityEngine;

public class WorkerAI : Unit
{
    private enum WorkerState { Idle, Gathering, Delivering, Returning, Hiding }

    [Header("Assignments")]
    public Building myWorkplace;
    public ResourceDepot myDepot;
    public ResourceType resourceToGather = ResourceType.Bread; // What does this worker make?

    [Header("Debug")]
    [SerializeField] private WorkerState currentState;
    [SerializeField] private int currentLoad = 0;
    [SerializeField] private float harvestTimer = 0;

    protected override void Start()
    {
        base.Start();

        // Coach Tip: Find the depot ONCE. If you build depots dynamically,
        // we will need a "FindClosestDepot" method later.
        myDepot = FindObjectOfType<ResourceDepot>();

        DayNightManager.Instance.OnNightStart += HandleNightfall;
        DayNightManager.Instance.OnDayStart += HandleSunrise;

        // Start by working
        currentState = WorkerState.Returning;
    }

    // Don't forget OnDestroy or the game errors when you reload scenes!
    private void OnDestroy()
    {
        DayNightManager.Instance.OnNightStart -= HandleNightfall;
        DayNightManager.Instance.OnDayStart -= HandleSunrise;  
    }

    void Update()
    {
        switch (currentState)
        {
            case WorkerState.Gathering:
                harvestTimer += Time.deltaTime;

                if (harvestTimer >= stats.harvestTime)
                {
                    harvestTimer = 0;
                    currentLoad++;

                    Debug.Log("Harvested! Load: " + currentLoad);

                    // Is backpack full?
                    if (currentLoad >= stats.resourceCapacity)
                    {
                        currentState = WorkerState.Delivering;
                        // Tell NavMesh to move immediately
                        agent.SetDestination(myDepot.transform.position);
                    }
                }
                break;

            case WorkerState.Delivering:
                // NavMesh Logic: Are we there yet?
                if (!agent.pathPending && agent.remainingDistance < 1.0f)
                {
                    // We arrived at Depot!
                    myDepot.DepositResources(currentLoad, resourceToGather);
                    currentLoad = 0; // Empty backpack

                    // NIGHT CHECK: If it turned night while walking here, HIDE NOW.
                    if (DayNightManager.Instance.isNight)
                    {
                        currentState = WorkerState.Hiding;
                        agent.SetDestination(myWorkplace.transform.position);
                    }
                    else
                    {
                        currentState = WorkerState.Returning;
                        agent.SetDestination(myWorkplace.transform.position);
                    }
                }
                break;

            case WorkerState.Returning:
                // Move back to work
                if (!agent.pathPending && agent.remainingDistance < 1.0f)
                {
                    currentState = WorkerState.Gathering;
                }
                break;

            case WorkerState.Hiding:
                // Move back to work to hide
                if (!agent.pathPending && agent.remainingDistance < 1.0f)
                {
                    // Hide logic
                    // TODO: Add to building's "Hidden List" here later
                    gameObject.SetActive(false);
                }
                break;
        }
    }

    void HandleNightfall()
    {
        // If we are currently walking with a heavy bag, finish the job first.
        // If we are just gathering or returning empty-handed, RUN!
        if (currentState != WorkerState.Delivering)
        {
            currentState = WorkerState.Hiding;
            if (myWorkplace != null)
                agent.SetDestination(myWorkplace.transform.position);
        }
    }

    void HandleSunrise()
    {
        // When we wake up (SetActive true), we reset state
        currentState = WorkerState.Returning;
        if (myWorkplace != null)
            agent.SetDestination(myWorkplace.transform.position);
    }
}
