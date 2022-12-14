using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    private enum FSMStates
    {
        Patrol, Chase, Aim, Shoot, Evade
    }

    [SerializeField]
    private FSMStates currentState = FSMStates.Patrol;
    public int health;
    private Vector3 destPos;
    public GameObject turret;
    public GameObject bullet;
    public Transform playerTransform;
    public GameObject bulletSpawnPoint;
    public List<GameObject> pointList;
    public float curSpeed;
    public float rotSpeed;
    public float turretRotSpeed;
    public float maxForwardSpeed;
    public float maxBackwardSpeed;
    public float shootRate;
    private float elapsedTime;
    public float patrolRadius;
    public float chaseRadius;
    public float AttackRadius;
    public float evadeTime;
    private int index = -1;
    
    private int timePassed;
    public bool hit;
    private int evade;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        curSpeed = 2f;
        rotSpeed = 3f;
        turretRotSpeed = 5f;
        maxForwardSpeed = 30f;
        maxBackwardSpeed = -30f;
        shootRate = 0.5f;
        patrolRadius = 1f;
        chaseRadius = 10f;
        AttackRadius = 6.0f;
        hit = false;
        timePassed = 0;
        evadeTime = 50;

        
        FindNextPoint();
    }

    private void FindNextPoint() 
    {
        print("Finding next point");
        index = (index+1)%pointList.Count; //Random.Range(0, pointList.Count);
        destPos = pointList[index].transform.position;
    }
 

    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }

        switch(currentState)
        {
            case FSMStates.Patrol:
                UpdatePatrol();
                break;
                
            case FSMStates.Chase:
                UpdateChase();
                break;
                
            case FSMStates.Aim:
                UpdateAim();
                break;

            case FSMStates.Shoot:
                UpdateShoot();
                break;

            case FSMStates.Evade:
                UpdateEvade();
                break;
        }
    }

    void UpdatePatrol()
    {
        //Find another random patrol point if the current point is reached
        if (Vector3.Distance(transform.position, destPos) <= patrolRadius) 
        {
            print("Reached the destination point -- calculating the next point");
            FindNextPoint();
        }
        //Check the distance with player tank, when the distance is near, transition to chase state
        else if (Vector3.Distance(transform.position, playerTransform.position) <= chaseRadius) 
        {
            print("Switch to Chase state");
            currentState = FSMStates.Chase;
        }

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);

        //Go Forward
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    void UpdateChase()
    {   
        //Check if get hitted
        if(hit)
        {
            evade = Random.Range(0,2);
            print("Switch to Evade state");
            currentState = FSMStates.Evade;
        }
        //Check the distance with player tank, when the distance is near, transition to aim state
        else if (Vector3.Distance(transform.position, playerTransform.position) <= AttackRadius) 
        {
            print("Switch to Aim state");
            currentState = FSMStates.Aim;
        }
        // Check the distance with player tank, when the distance is far, transition to patrol state
        else if (Vector3.Distance(transform.position, playerTransform.position) >= chaseRadius) 
        {
            print("Switch to Patrol state");
            currentState = FSMStates.Patrol;
        }

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);

        //Go Forward
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    void UpdateAim()
    {
        print("Aiming");
        Quaternion targetRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
        print("Switch to Shoot state");
        currentState = FSMStates.Shoot;
    } 

    void UpdateShoot()
    {
        // Shoot
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shootRate) 
        {
            //Reset the time
            elapsedTime = 0.0f;

            Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
        }
        print("Switch to Chase state");
        currentState = FSMStates.Chase;
    }

    void UpdateEvade()
    {
        timePassed += 1; 
        if (timePassed < evadeTime)
        {
            print("Evading");
            //Rotate the turret
            Quaternion targetTurretRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
            turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetTurretRotation, Time.deltaTime * turretRotSpeed);

            if(evade == 0)
            {
                // Go left
                transform.Translate(Vector3.left * Time.deltaTime * curSpeed);
            }
            if(evade == 1)
            {
                // Go right
                transform.Translate(Vector3.right * Time.deltaTime * curSpeed);
            }
        }
        else
        {
            hit = false;
            timePassed = 0;
            print("Switch to Chase state");
            currentState = FSMStates.Chase;
        }
    }

    private void FixedUpdate() 
    {
        
    }
}
