using UnityEngine;

public class WorkerAI : Unit
{
    private enum WorkerState { Idle, Gathering, Delivering, Returning, Hiding }

    [Header("Assignments")]
    public Building myWorkplace;
    public ResourceDepot myDepot;
    public ResourceType resourceToGather = ResourceType.Bread; // What does this worker make?

    [Header("Refugee Settings")]
    public AggroSensor aggroSensor;
    public float searchInterval = 1f;
    private float searchTimer;
    private float combatCooldown;
    private Unit combatTarget;

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
        if (myWorkplace == null) RefugeeLogic();

        else
        {
            switch (currentState)
            {
                case WorkerState.Gathering:
                    harvestTimer += Time.deltaTime;
                    transform.LookAt(myWorkplace.transform);
                    if (harvestTimer >= stats.harvestTime)
                    {
                        harvestTimer = 0;
                        currentLoad++;

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
        gameObject.SetActive(true);
        currentState = WorkerState.Returning;
        if (myWorkplace != null)
            agent.SetDestination(myWorkplace.transform.position);
    }

    void RefugeeLogic()
    {
        // A. SEARCH FOR JOBS (Every 1 second)
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0;
            Building newJob = GameManager.Instance.GetFirstAvailableWorkplace(ResourceType.Wood); // Or generic

            if (newJob != null)
            {
                // Found a job! Re-assign and return to normal
                myWorkplace = newJob;
                myWorkplace.AddWorker(this);
                // Reset combat stuff
                combatTarget = null;
                agent.ResetPath();
                return;
            }
        }

        // B. SELF DEFENSE (Militia Mode)
        // 1. Look for enemies
        if (combatTarget == null)
        {
            combatTarget = aggroSensor.GetTarget()?.GetComponent<Unit>();
        }

        // 2. Fight or Guard
        if (combatTarget != null)
        {
            FightLogic();
        }
        else
        {
            // 3. NO JOB, NO ENEMIES -> Guard the Base
            // (This prevents them from standing still in the woods)
            GameObject baseObj = GameObject.FindGameObjectWithTag("MainBase");
            if (baseObj != null && Vector3.Distance(transform.position, baseObj.transform.position) > 10f)
            {
                agent.SetDestination(baseObj.transform.position);
            }
        }
    }

    void FightLogic()
    {
        float dist = Vector3.Distance(transform.position, combatTarget.transform.position);

        if (dist <= stats.attackRange)
        {
            // Stop and Hit
            agent.ResetPath();
            transform.LookAt(combatTarget.transform);

            combatCooldown -= Time.deltaTime;
            if (combatCooldown <= 0)
            {
                // USE YOUR SCRIPTABLE OBJECT STATS HERE!
                combatTarget.TakeDamage(stats.damage);
                combatCooldown = stats.attackSpeed;

                // Animation trigger if you have one
                // anim.SetTrigger("Attack"); 
            }
        }
        else
        {
            // Chase
            agent.SetDestination(combatTarget.transform.position);
        }
    }
}
