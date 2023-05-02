
using UnityEngine;
using UnityEngine.AI;
using System.Collections;


[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(FishCharacter))]
public class AIFishControl : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
    public FishCharacter character { get; private set; } // the character we are controlling
    //  public Transform target;                                    // target to aim for
    public Transform playerRoot;

    private Vector3 minBoundsPoint;
    private Vector3 maxBoundsPoint;
    private float boundsSize = float.NegativeInfinity;
    private bool findTarget;
    [Header("Fish Settings")]
    public string Fish_Name = "Koi Carp";
    public string Fish_Size = "30 cm";
    public float Player_Fishing_Force = 0.05f;
    bool PlayerIsInTrigger;
    bool isPlayingFastSwimEffect;
    public GameObject Fast_Swim_Effect;
    public AudioClip Splash_Sound;
    [Header("Fish Bite Settings")]
    //Bite_Volume = how strong is his bite
    public float Bite_Volume;
    public bool Bite;
    public Transform Point_To_Bite;
    public bool canBite;
    [Header("Destinations")]
    public GameObject[] Destinations;
    public Transform Current_Destination;
    [Header("Behavior")]
    public bool isPredatoryFish = false;
    public bool isSwimmingAfterBite;
    public bool isSwimmingWithFloater;
    public bool isFishAttachedToFloater;
    [Header("Floater")]
    public Transform Floater;
    public bool isPlayerPullUpFish = false;
    [Header("Strong Swim")]
    public int maxStrongSwimValue = 5;

    protected virtual void MuckAbout()
    {
        if (this.agent.desiredVelocity.magnitude < 0.1f)
            this.agent.SetDestination(GetRandomTargetPoint());
    }


    private Vector3 GetRandomTargetPoint()
    {

        if (boundsSize < 0)
        {
            minBoundsPoint = Vector3.one * float.PositiveInfinity;
            maxBoundsPoint = -minBoundsPoint;
            var vertices = UnityEngine.AI.NavMesh.CalculateTriangulation().vertices;
            foreach (var point in vertices)
            {
                if (minBoundsPoint.x > point.x)
                    minBoundsPoint = new Vector3(point.x, minBoundsPoint.y, minBoundsPoint.z);
                if (minBoundsPoint.y > point.y)
                    minBoundsPoint = new Vector3(minBoundsPoint.x, point.y, minBoundsPoint.z);
                if (minBoundsPoint.z > point.z)
                    minBoundsPoint = new Vector3(minBoundsPoint.x, minBoundsPoint.y, point.z);
                if (maxBoundsPoint.x < point.x)
                    maxBoundsPoint = new Vector3(point.x, maxBoundsPoint.y, maxBoundsPoint.z);
                if (maxBoundsPoint.y < point.y)
                    maxBoundsPoint = new Vector3(maxBoundsPoint.x, point.y, maxBoundsPoint.z);
                if (maxBoundsPoint.z < point.z)
                    maxBoundsPoint = new Vector3(maxBoundsPoint.x, maxBoundsPoint.y, point.z);
            }
            boundsSize = Vector3.Distance(minBoundsPoint, maxBoundsPoint);
        }
        var randomPoint = new Vector3(
            Random.Range(minBoundsPoint.x, maxBoundsPoint.x),
            Random.Range(minBoundsPoint.y, maxBoundsPoint.y),
            Random.Range(minBoundsPoint.z, maxBoundsPoint.z)
        );
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, boundsSize / 100.0f, 1);
        //  posr = hit.position;
        return hit.position;
    }



    private void Start()
    {
        if (Floater == null)
            Floater = GameObject.FindGameObjectWithTag("Fishing_Float").transform;
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<FishCharacter>();

        //   target.parent = null;

        agent.updateRotation = false;
        agent.updatePosition = true;

        //Clear and search all destinations and put them into the array.
        Destinations = null;
        Destinations = GameObject.FindGameObjectsWithTag("Fish_Destination");
        Fish_Size = "";
        //All valid characters.
        const string Numbers = "123456789";
        //Takes a random number and add them to the fish size.
        //Primary number.
        int charAmount = Random.Range(1, 1);
        for (int i = 0; i < charAmount; i++)
        {
            Fish_Size += Numbers[Random.Range(0, Numbers.Length)];
        }
        //Secondary number.
        for (int i = 0; i < charAmount; i++)
        {
            Fish_Size += Numbers[Random.Range(0, Numbers.Length)] + " cm";
        }

        StartCoroutine(Search_Destination());
    }



    void DisableRagdoll(bool active)
    {
        Component[] Rigidbodys = playerRoot.GetComponentsInChildren(typeof(Rigidbody));
        Component[] Colliders = playerRoot.GetComponentsInChildren(typeof(Collider));

        foreach (Rigidbody rigidbody in Rigidbodys)
            rigidbody.isKinematic = !active;


        foreach (Collider collider in Colliders)
            collider.enabled = active;

    }



    void OnTriggerStay(Collider player)
    {
        //If player is near to the fish, then will the fish swim fast away
        if(player.tag == "Player")
        {
            PlayerIsInTrigger = true;
            this.GetComponent<NavMeshAgent>().speed = 4;
            //Current_Destination = null;
            if (isPlayingFastSwimEffect == false)
            {
                isPlayingFastSwimEffect = true;
                Fast_Swim_Effect.SetActive(true);
                //play splash sound
                this.GetComponent<AudioSource>().loop = false;
                this.GetComponent<AudioSource>().clip = Splash_Sound;
                this.GetComponent<AudioSource>().Play();
            }
        }
    }
    //Play fast swim splash effect
    IEnumerator Play_Fast_Swim_Effect()
    {
        yield return new WaitForSeconds(2);
        Fast_Swim_Effect.SetActive(false);
        isPlayingFastSwimEffect = false;
    }

    void OnTriggerExit(Collider player)
    {
        if(PlayerIsInTrigger == true)
        {
            this.GetComponent<NavMeshAgent>().speed = 1;
        }
        PlayerIsInTrigger = false;
    }


    private void Update()
    {

        if (!this.agent.enabled) return;

        if (this.agent.enabled)
        {
            if (this.agent.pathPending)
                return;
            this.MuckAbout();
        }
       

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity / 2.0f, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }

        if (canBite == true & Bite == true)
        {
            if (isSwimmingAfterBite == true)
            {
                if(isSwimmingWithFloater == true)
                {
                    this.GetComponent<NavMeshAgent>().destination = Point_To_Bite.transform.position;
                    isSwimmingWithFloater = false;
                    StartCoroutine(Swimming_With_Floater());
                }

            }
            else
            {
                this.GetComponent<NavMeshAgent>().destination = Point_To_Bite.transform.position;
                //If isSwimmingAfterBite = true, will the floater attach to the fish position
            }
        }
    }
    IEnumerator Swimming_With_Floater()
    {
        yield return new WaitForSeconds(2);
        isFishAttachedToFloater = true;
    }

    IEnumerator Search_Destination()
    {
        yield return new WaitForSeconds(10);

        if (Bite == false)
        {
            //Search a random destinations to swim, so if Bite = false
            int index = Random.Range(0, Destinations.Length);
            Current_Destination = Destinations[index].transform;
            this.GetComponent<NavMeshAgent>().destination = Current_Destination.transform.position;
        }
        if (isFishAttachedToFloater == true)
        {
            if(isPlayerPullUpFish == false)
            {
                this.GetComponent<NavMeshAgent>().speed = 4;
                //Search a random destinations to swim
                int index = Random.Range(0, Destinations.Length);
                Current_Destination = Destinations[index].transform;
                this.GetComponent<NavMeshAgent>().destination = Current_Destination.transform.position;
            }
            else
            {
                this.GetComponent<NavMeshAgent>().speed = 2;
                //follow player floater.
                this.GetComponent<NavMeshAgent>().destination = Floater.transform.position;
            }
        }
        StartCoroutine(Search_Destination());
    }

    public void Set_Fish_Speed_To_Normal()
    {
        this.GetComponent<NavMeshAgent>().speed = 1;
    }

    public void Search_New_Destination()
    {
        if (Bite == false)
        {
            //Search a random destinations to swim, so if Bite = false
            int index = Random.Range(0, Destinations.Length);
            Current_Destination = Destinations[index].transform;
            this.GetComponent<NavMeshAgent>().destination = Current_Destination.transform.position;
        }
        if (isFishAttachedToFloater == true)
        {
            if (isPlayerPullUpFish == false)
            {
                this.GetComponent<NavMeshAgent>().speed = 4;
                //Search a random destinations to swim
                int index = Random.Range(0, Destinations.Length);
                Current_Destination = Destinations[index].transform;
                this.GetComponent<NavMeshAgent>().destination = Current_Destination.transform.position;
            }
            else
            {
                this.GetComponent<NavMeshAgent>().speed = 2;
                //follow player floater.
                this.GetComponent<NavMeshAgent>().destination = Floater.transform.position;
            }
        }
    }
}

