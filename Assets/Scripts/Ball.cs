using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    
    public static Ball Instance { get; private set; }
    /* public GameObject ballObj = null;*/
    public Rigidbody ballRb;
    //[SerializeField]
    //private float speedThreshold = 10f;
    [SerializeField]
    public float speed;
    [SerializeField]
    public float maxSpeed;

    [SerializeField]
    public Transform ballSpawnPoint;
    public Vector3 launchDirection = Vector3.forward;
    public float launchForce;
   
    public GameObject trailBallPrefab; // Prefab with Rigidbody and Trail Renderer
    public int noOfPoints = 50;
    public float timeStep = 0.1f;

    public static event Action<Rigidbody, Vector3, Vector3> BallStartAndEndpositions;
    public event Action<Vector3> OnLandingCalculated;


    private Vector3 ballDirection;
    private Vector3 directionToPlayer;
    private float dot;
    public Vector3 landingPos;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
        trailBallPrefab = Instantiate(trailBallPrefab, new Vector3(999, 999, 999), Quaternion.AngleAxis(-90, new Vector3(-90, 0, 0)));

    }

    void Start()
    {
        speed = 9f;
        maxSpeed = speed;
       
        /*if(ballObj == null)
        {
            ballObj = Instantiate(gameObject); ballObj.name = "Ball";
            ballRb = ballObj.GetComponent<Rigidbody>();
        }*/
        ballSpawnPoint = gameObject.transform;
        
       
        //ballRb.AddForce(-Vector3.forward * 50f * Time.deltaTime, ForceMode.Impulse);
    }
    void FixedUpdate()
    {if(speed>maxSpeed)
        {
            SpeedControl();
        }

    }
  
    public Vector3 CreateBallVelocity(Vector3 startPoint, Vector3 direction, float swipeTime, float swipeDistance)
    {
        //Debug.Log("directionvelocity " + direction);
        Vector3  ogDirection = new Vector3(direction.x, direction.y, direction.y * 2f);
        ballRb = gameObject.GetComponent<Rigidbody>();
        
        // 
        Debug.Log(speed + "--------------------------Speed");
        //float calculatedSpeed = Mathf.Min(swipeTime * swipeDistance, maxSpeed);
        //Vector3 velocity = ogDirection.normalized * calculatedSpeed;
        //
        ballRb.Sleep();
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
            
        speed += swipeTime * swipeDistance;
        Vector3 ballDir = new Vector3(ogDirection.x, ogDirection.y, ogDirection.z);
        Vector3 velocity = ogDirection.normalized * speed ;
        ballRb.WakeUp();
        //CalculateLandingPoint(startPoint, velocity, 18.24f);
        /*ballRb.AddForce(velocity, ForceMode.Impulse);*/
        /*  Debug.Log("velocity " + velocity);*/
        // BallStartAndEndpositions?.Invoke(ballRb, startPoint, CalculateLandingPoint(startPoint, velocity, 18.24f));
        //SpeedControl();
        return velocity;
    }

    public void SpeedControl()
    {
        Vector3 ballVel = new Vector3(ballRb.linearVelocity.x, ballRb.linearVelocity.y, ballRb.linearVelocity.z);

        if (ballVel.magnitude > maxSpeed)
        {
            speed = maxSpeed;
            Vector3 limitBallVel = ballVel.normalized * speed;
            ballRb.linearVelocity = new Vector3(limitBallVel.x, limitBallVel.y, limitBallVel.z);
        }

    }

    

    public Vector3 CalculateLandingPoint(Vector3 startPos, Vector3 startVelocity, float groundY)
    {
        float y0 = startPos.y;                  // start height
        float vy = startVelocity.y;             // initial vertical velocity
        Vector3 vHorizontal = new Vector3(startVelocity.x, 0, startVelocity.z);

        // Use Physics.gravity for g (negative value, e.g. -9.81)
        float g = Physics.gravity.y;
        float a = 0.5f * g;
        float b = vy;
        float c = y0 - groundY;

        // Solve quadratic a*t^2 + b*t + c = 0 for time t
        float discriminant = Mathf.Abs(b * b - 4f * a * c);
        if (discriminant < 0f)
        {
            // No real solution: does not hit the plane
            return Vector3.zero;
        }
        float sqrtDisc = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtDisc) / (2f * a);
        float t2 = (-b - sqrtDisc) / (2f * a);
        // Choose the positive (future) time
        float t = Mathf.Max(t1, t2);
        if (t < 0f)
        {
            // Both times negative: impact is in the past
            return Vector3.zero;
        }

        // Calculate impact position using P = start + v*t + 0.5*g*t^2
        landingPos = startPos
                            + startVelocity * t
                            + 0.5f * Physics.gravity * t * t;
        // Ensure Y is exactly groundY (prevent tiny float errors)
        landingPos.y = groundY;
        BallLandingPositionMarker(landingPos);
        //Debug.Log("landingPos"+landingPos);
        OnLandingCalculated?.Invoke(landingPos);
        return landingPos;
    }
    public Vector3 BallLandingPositionMarker(Vector3 lp)
    {
        if(lp !=  Vector3.zero)
        {
            Vector3 landingPosSpot = new Vector3(lp.x, lp.y + .02f, lp.z);
           // Instantiate(trailBallPrefab, landingPosSpot, Quaternion.AngleAxis(-90, new Vector3(-90, 0, 0)));
           trailBallPrefab.transform.position = landingPosSpot;

            return lp;
        }
        else
        {
            Debug.LogError("Ball landingPos is " + lp);
            return Vector3.zero;
        }
    }
    public void AddForceAtTheEnd(List<Vector3> path)
    {
        Vector3 start = path[path.Count - 15];
        Vector3 end = path[path.Count - 10];
        Vector3 ballPos = transform.position;
        Vector3 direction = (end - start);
      direction.z = direction.z * 2f; // Adjusting the z component for a more realistic trajectory
        
        ballRb.AddForce(direction * 4f, ForceMode.VelocityChange); 
        ballRb.AddTorque(Vector3.Cross(direction, Vector3.up) * 4f, ForceMode.VelocityChange);

    }
}
