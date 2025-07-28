using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeControl : MonoBehaviour
{
    public static event Action<SwipeControl> OnSwipe;
    [SerializeField]
    private InputManager inputManager;

    private Vector2 direction2D;
    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    public Vector3 points;
    public List<Vector3> path = new List<Vector3>();
    /// <summary>
     private int currentPathIndex = 0;
    private float moveSpeed = 14f; // Adjust as needed
    private bool isMoving = false;
    /// </summary>

    private Rigidbody ballrb;
    [SerializeField]
    private float minimumDistance =.2f;
    [SerializeField]
    private float maxTime = 1f;
    [SerializeField]
    private float directionalThreshold = 0.9f;
    [SerializeField]
    private GameObject trail;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private GameObject player;
    private Coroutine coroutine;
    private Coroutine slowBallCoroutine;
    private BallHitDetection ballHitDetection;
    
    private InputAction fireAction;
    private GameObject createdBallPrefab = null;
    private float swipeTime;

    private Vector3 direction;
    private Vector3 middlePosition;
    private Vector3 velocity;
    private Vector3 landingPos;
    private Vector2 swipStart;
    private Vector2 swipEnd;
    private Vector3 target;
    private float swipeDistance;
    //[SerializeField]
    //private Camera mainCamera;
    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
        BallHitDetection.OnPlayer1Hit += Player1SwingAction;
        fireAction = inputManager.FireAction;
        //  Ball.BallStartAndEndpositions += MakeBallMovement;
        BallHitDetection.OnPlayer2Hit += OpponentPlayerServing;
    }
    private void OnDisable()
    {

        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
        BallHitDetection.OnPlayer1Hit -= Player1SwingAction;
        // Ball.BallStartAndEndpositions -= MakeBallMovement;
        BallHitDetection.OnPlayer2Hit -= OpponentPlayerServing;
    }
    private void Awake()
    {
      // ballHitDetection =  player.GetComponentInChildren<BallHitDetection>();

    }
    private void Start()
    {
     
    }
   
    public void Player1SwingAction(Collider collider)
    {
        
        ballrb = collider.GetComponent<Rigidbody>();
    }
   
    private void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        startTime = time;
        trail.SetActive(true);
        trail.transform.position = position;
        coroutine =  StartCoroutine(Trail());
    }
    private void Update()
    {
     
         if (fireAction != null && fireAction.WasPressedThisFrame())
        {
            /* CreateBall();*/
            /*//SimulateSwipeRHS();*/
            //OpponentBallDebug();
            //GameManager.Instance.SetPlayerPositionToInitial(0f);
        }
    }
    private IEnumerator Trail()
    {
        while (true)
        {
            trail.transform.position = inputManager.PrimaryPosition(10f);
            yield return null;
        }
        
    }
    private void SwipeEnd(Vector2 position, float time)
    {
      
        trail.SetActive(false);
        StopCoroutine(coroutine);
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }
    public void OpponentPlayerServing(Collider collider)
    {
        if (Vector2.Distance(swipStart, swipEnd) >= minimumDistance && (endTime - startTime) <= maxTime)
        {
            GameManager gameManager = GameManager.Instance;
            Ball ball = Ball.Instance;

            float swipeTime = (endTime - startTime);
            direction = swipEnd - swipStart;
            direction2D = new Vector2(direction.x, direction.y).normalized;

            ball.ballRb.useGravity = true;
            velocity =ball.CreateBallVelocity(swipStart, direction, swipeTime, swipeDistance);
            landingPos = ball.CalculateLandingPoint(swipStart, velocity, 18.24f);
            landingPos.z = landingPos.z / 8;
            ball.BallLandingPositionMarker(landingPos);
           gameManager.IncrementP2Serve();
            //GameManager.Instance.UpdateServeCount();
            gameManager.isBallInPlay = true;
            MakeBallMovement(ball.transform.position,  landingPos);
            //zone.ResetHasCollided();
            gameManager.isPlayerOneServing = false;
            gameManager.isBallTouched = false;

           
        }
    }
   
    public bool DetectSwipe()
    {
        //comment
        //Debug.Log("IIIIIIIIIIIIxxxxxxxxxxxxxxxxxxxxxxxxxxstartPosition --" + startPosition+ " --endPosition "+ endPosition);
        //Debug.Log("IIIIIIIIIIIIIxxxxxxxxxxxxxxxxxxstartTime --" + startTime+ "--endTime " + endTime);
        if (Vector2.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maxTime)
        {
            GameManager gameManager = GameManager.Instance;
            Ball ball = Ball.Instance;

            float swipeTime = (endTime - startTime);
            swipEnd   = startPosition;
            swipStart = endPosition;
            //direction of the ballPrefab in 2D, z is 0 currently
           direction = endPosition - startPosition;
          // direction2D = new Vector2(direction.x, direction.y).normalized;

            swipeDistance = Vector2.Distance(startPosition, endPosition);
            swipeDistance *= 2f;
            SwipeDirection(direction2D); 

            //ballRb.useGravity = true;
            velocity = ball.CreateBallVelocity(startPosition, direction, swipeTime, swipeDistance);
            landingPos = ball.CalculateLandingPoint(startPosition, velocity, 18.24f);
            gameManager.SetGameStarted(true);

            gameManager.isBallInPlay = true;
            //Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxxisBallInPlay " + GameManager.Instance.isBallInPlay);
            MakeBallMovement(ball.transform.position,  landingPos);
            //zone.ResetHasCollided();
            gameManager.IncrementP1Serve();
            gameManager.isPlayerOneServing = true;
            gameManager.isBallTouched = false;
           
            return true;
        }
        else {
            Debug.Log("This Cndition is not Met =>> Vector2.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maxTime");
            return false; 
        }
    }
    public void MakeBallMovement( Vector3 startPoint,   Vector3 landingPoint)
    {
       
        if (GameManager.Instance.isPlayerOneServing)
        {
            
            DrawQuadraticBezierPoint(startPoint,  landingPoint);
            GameManager.Instance.SetServer(false);
        }
        if (!GameManager.Instance.isPlayerOneServing)
        {
           
            DrawQuadraticBezierPoint(startPoint,  landingPoint);
            GameManager.Instance.SetBallTouched(true);
        }
       
    }
    //public void SimulateSwipeLHS()
    //{
    //    UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    //    Vector2 simStartSwipe = new Vector2(UnityEngine.Random.Range(291, 294.5f), UnityEngine.Random.Range(11f, 14f));
    //    Vector2 simEndSwipe   = new Vector2(UnityEngine.Random.Range(293, 296.5f), UnityEngine.Random.Range(0.5f, .75f));
    //    float simSwipeLength = UnityEngine.Random.Range(0.12f, 0.16f);

    //    SwipeStart(simStartSwipe, simSwipeLength);
    //    SwipeEnd(simEndSwipe, simSwipeLength);

    //}

    public void DrawQuadraticBezierPoint(Vector3 start, Vector3 end)
    {

       List<Vector3> path = new List<Vector3>();
        int noOfPoints = 40;

        float arcFactor = 0.7f; //  control arc height 
        float swipeDistance = Vector2.Distance(startPosition, endPosition);
       
        middlePosition = (start + end)  / 2f + Vector3.up * swipeDistance * arcFactor;
        middlePosition.y += 2f;
         //Debug.Log("middlePosition" + middlePosition);
        for (int i = 0; i <= noOfPoints; i++)
        {
            float t = i / (float)noOfPoints;

            //Vector3 point
            path.Add(CalculateQuadraticBezierPoint(start, middlePosition, end, t));

        }
        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(path[i - 1], path[i], Color.red, 2f);
        }

        currentPathIndex = 0;
        isMoving = true;
       StartCoroutine(MoveAlongPath(Ball.Instance.ballRb.gameObject, path, 1.0f));
    }

   
    public IEnumerator MoveAlongPath(GameObject ball, List<Vector3> path, float duration)
    {
        Rigidbody rb = Ball.Instance.ballRb;
        rb.useGravity = true;

        float totalLength = path.Count - 1;
        float elapsed = 0f;
        while (elapsed < duration)
        {

            float t = elapsed / duration * totalLength;
            int i = Mathf.FloorToInt(t);
            float u = t - i;             
            if (i < path.Count - 1)
                  rb.MovePosition(Vector3.Lerp(path[i], path[i + 1], u));
           
            elapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate(); ;
        }

        Ball.Instance.AddForceAtTheEnd(path);

    }
    private Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        //B(t) = (1 - t)²P0  +   2(1 - t)tP1        + t²P2 
        //          u               u                 tt
        //          uu * p0  +   u * 2 * t * p1     + tt * p2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

         points = uu * p0;
        points += u * 2 * t * p1;
        points += tt * p2;
        //point.z = point.z * t; //adding the z axis for depth
        return points;
    }
    //public void AddForceAtTheEnd(List<Vector3> path)
    //{
    //    Vector3 start = path[path.Count - 15];
    //    Vector3 end = path[path.Count - 10];
    //    Vector3 ballPos = Ball.Instance.transform.position;
    //   Vector3 direction = (end - start);
    //    //  Vector3 direction = ballPrefab.transform.LookAt(ballPos, Vector3.forward ); //addforce tryed but stil shit
        
    //   // Ball.Instance.ballRb.WakeUp();
    //    //Debug.Log("Force direction: " + direction);
    //    Ball.Instance.ballRb.AddForce(direction * 4f, ForceMode.VelocityChange);
    //    /* Debug.Log("Force direction: " + direction);*/

    //}
  
    
    private void SwipeDirection(Vector2 direction)
    {

        if(Vector3.Dot(Vector3.up, direction) > directionalThreshold)
        {
          //  Debug.Log("swipedUp");
        }
        else if(Vector3.Dot(Vector3.down, direction) > directionalThreshold)
        {
            //Debug.Log("swipedDown");
        }
        else if(Vector3.Dot(Vector3.left, direction) > directionalThreshold)
        {
            //Debug.Log("swipedLeft");
        }
        else if(Vector3.Dot(Vector3.right, direction) > directionalThreshold)
        {
            //Debug.Log("swipedRight");
        }
    }
   
    
}

