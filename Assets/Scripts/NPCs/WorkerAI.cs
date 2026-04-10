using UnityEngine;

public class WorkerAI : Unit
{
    private enum WorkerState { Idle, Gathering, Delivering, Returning, Hiding }

    [Header("Assignments")]
    public Building myWorkplace;
    public ResourceDepot myDepot;
    public ResourceType resourceToGather = ResourceType.Bread;
    public string myJobType;

    [Header("Refugee Settings")]
    public AggroSensor aggroSensor;
    public float searchInterval = 1f;
    private float searchTimer;
    private float combatCooldown;
    private Unit combatTarget;

    [Header("Refugee Patrol Settings")]
    public float patrolRadius = 16f;     // How far from the bakery they can wander
    public float patrolWaitTime = 3f;   // How long they stand still before picking a new spot
    private float patrolTimer;
    private bool isPatrollingBase = false;

    [Header("Harvesting UI")]
    public WorkerProgressBar progressBarPrefab;
    private WorkerProgressBar myProgressBar;

    [Header("Debug")]
    [SerializeField] private WorkerState currentState;
    [SerializeField] private int currentLoad = 0;
    [SerializeField] private float harvestTimer = 0;
    private float pathUpdateTimer = 0f;

    protected override void Start()
    {
        base.Start();

        myDepot = FindObjectOfType<ResourceDepot>();

        DayNightManager.Instance.OnNightStart += HandleNightfall;
        DayNightManager.Instance.OnDayStart += HandleSunrise;

        // Start by working
        currentState = WorkerState.Returning;

        if (myWorkplace != null)
        {
            myJobType = myWorkplace.jobType;
            myWorkplace.AddWorker(this);
        }

        if (progressBarPrefab != null)
        {
            myProgressBar = Instantiate(progressBarPrefab, transform.position, Quaternion.identity);
            myProgressBar.Setup(transform);
        }
    }

    // Don't forget OnDestroy or the game errors when you reload scenes!
    private void OnDestroy()
    {
        DayNightManager.Instance.OnNightStart -= HandleNightfall;
        DayNightManager.Instance.OnDayStart -= HandleSunrise;

        if (myProgressBar != null) Destroy(myProgressBar.gameObject);
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;
        if (pathUpdateTimer > 0) pathUpdateTimer -= Time.deltaTime;
        if (myWorkplace == null) RefugeeLogic();

        else
        {
            switch (currentState)
            {
                case WorkerState.Gathering:
                    harvestTimer += Time.deltaTime;
                    transform.LookAt(myWorkplace.transform);
                    myProgressBar.gameObject.SetActive(true);
                    if (myProgressBar != null) myProgressBar.UpdateProgress(harvestTimer, stats.harvestTime);

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
                            if (myProgressBar != null) myProgressBar.SetFullState(true);
                        }
                        else
                        {
                            if (myProgressBar != null) myProgressBar.UpdateProgress(0f, stats.harvestTime);
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

                        if (myProgressBar != null) myProgressBar.SetFullState(false);

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
                        if (myProgressBar != null) myProgressBar.gameObject.SetActive(false);
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

            // USE THE MEMORIZED JOB TYPE HERE!
            Building newJob = GameManager.Instance.GetFirstAvailableWorkplace(myJobType);

            if (newJob != null)
            {
                myWorkplace = newJob;
                isPatrollingBase = false;
                // Use the new Adopt method
                myWorkplace.AdoptWorker(this);

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
            // 3. NO JOB, NO ENEMIES -> Patrol around the Base
            // (This prevents them from standing still)
            GameObject baseObj = GameObject.FindGameObjectWithTag("MainBase");
            if (baseObj != null)
            {
                // Are we standing still / reached our destination?
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    patrolTimer += Time.deltaTime;

                    // Time to pick a new spot!
                    if (patrolTimer >= patrolWaitTime || !isPatrollingBase)
                    {
                        patrolTimer = 0;
                        isPatrollingBase = true;

                        // 1. Pick a random 2D circle point
                        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;

                        // 2. Convert to 3D world position around the Base
                        Vector3 randomTarget = baseObj.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

                        // 3. (Crucial) Ask the NavMesh for the closest walkable point 
                        // so they don't try to walk inside a tree or wall
                        UnityEngine.AI.NavMeshHit hit;
                        if (UnityEngine.AI.NavMesh.SamplePosition(randomTarget, out hit, patrolRadius, UnityEngine.AI.NavMesh.AllAreas))
                        {
                            agent.SetDestination(hit.position);
                        }
                    }
                }
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
                combatTarget.TakeDamage(stats.damage);
                combatCooldown = stats.attackSpeed;

                // Animation trigger for future
                // anim.SetTrigger("Attack"); 
            }
        }
        else
        {
            if (pathUpdateTimer <= 0)
            {
                agent.SetDestination(combatTarget.transform.position);
                pathUpdateTimer = 0.2f; // Only ask for a path 5 times a second
            }
            isPatrollingBase = false;
        }
    }

    // If a building gets destroyed and its night time
    public void EvictFromBuilding()
    {
        gameObject.SetActive(true);
        myWorkplace = null;

        currentState = WorkerState.Returning;
        if (myProgressBar != null) myProgressBar.gameObject.SetActive(true);
    }
}
