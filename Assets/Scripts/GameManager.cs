using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CourtZoneType
{
    None,
    P1Cort,
    P2Cort,
    LeftServiceBox,
    RightServiceBox,
    Net,
    BackRunOff,
    SideRunOff,
    Backcourt
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int playerOneScore = 0;
    private int playerTwoScore = 0;
    public Transform netTransform;

    public bool isPlayerOneServing = true;
    public bool isBallTouched = false;

    [SerializeField]
    private GameObject plane;
    [SerializeField]
    private TextMeshPro scoreText;
    [SerializeField]
    private TextMeshPro infoText;
 
    public GameObject player1Position;
    private Vector3 p1InitPos;
    private Vector3 p2InitPos;
    
    private GameObject player2Position;

    [SerializeField]
    public Transform player1BallPosition;
    
    [SerializeField]
    private Transform player2BallPosition;
    [SerializeField]
    private GameObject ball;
    public bool GameStarted { get; private set; }
    public int P1serveCount = 0;
    private int PreviousP1serveCount =0;    
    private int PreviousP2serveCount =0;    
    public int P2serveCount = 0;
    public bool isBallInPlay = false;
    private bool IsServerRightSide = true;
    public Vector3 ballLandingPoint;
    private int switchValue = 0;
    public bool hasCollidedFromColliders = false;
    private int ballCollitions =1;
    private void OnEnable()
    {
        CollitionDetection.OnZoneHit += HandleZoneHit;
        BallHitDetection.OnBallHit   += HandleServeRotation;
        SwipeControl.OnSwipe         += HandleSwipesRotation;
        CollitionDetection.PredictedLandingPoint += HandlePredictedLandingPoint;
       //CollitionDetection.OnSecondHit += SecondBallHitOrOut;

    }
    private void OnDisable()
    {
        CollitionDetection.OnZoneHit -= HandleZoneHit;
        BallHitDetection.OnBallHit -= HandleServeRotation;
        SwipeControl.OnSwipe -= HandleSwipesRotation;
        CollitionDetection.PredictedLandingPoint -= HandlePredictedLandingPoint;
      //CollitionDetection.OnSecondHit -= SecondBallHitOrOut;
    }
    private bool CheckBallHit()
    {
        if (isPlayerOneServing && P1serveCount > PreviousP1serveCount)
        {
            PreviousP1serveCount++;
            return true; 
        
        }
            return true;
    }
    private void SecondBallHitOrOut(CourtZoneType type, CollitionDetection col)
    {
        if (CheckForSecondCollition(col))
        //Debug.Log("zone.zoneType" + zone.zoneType );
        {
            Debug.Log(ballCollitions + "::ballCollitions-------__SecondBallHitOrOut-------------_----------");
            Debug.Log(col.collided2nd + "::cortObj.collided2nd-------__SecondBallHitOrOut-------------_----------");
            StartCoroutine(ShowStatus("Out!!"));
        AwardPointToCurrentPlayer();
        SwitchServer();
        SwichingPlayerPositionsToInitial();
            ResetServeCount();
            col.collided2nd = false;
            col.HasCollided = false;
        }
    }

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
        
    }

    private void Start()
    {
        
        player1Position = GameObject.FindGameObjectWithTag("Player1");
        player2Position = GameObject.FindGameObjectWithTag("Player2");
        Debug.Log("-------player1Position  " + player1Position.ToString());
        //Ball.Instance.OnLandingCalculated += HandlePredictedLandingPoint;
        p1InitPos = player1Position.transform.position;
        p2InitPos = player2Position.transform.position;
        CreateBall(player1Position);
        Debug.Log("-------p1InitPos  " + p1InitPos.ToString());
        IsServerRightSide = true;
    }
    public void SetGameStarted(bool value)
    {
        GameStarted = value;
    }
    public void IncrementP1Serve()
    {
        P1serveCount++;
    }
    public void IncrementP2Serve()
    {
        P2serveCount++;
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }

    private void HandleSwipesRotation(SwipeControl control)
    {
       
    }
    private void Update()
    {
        UpdateUI();
    }
    /*public void OpponentPlayerServing(Collider collider)
    {
        Rigidbody ballRb = collider.gameObject.GetComponent<Rigidbody>();
        //Todo Randomize Swing Direction
        Vector3 enemy = new Vector3(-9, 45, -100).normalized;
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.useGravity = true;
        ballRb.AddForce(enemy * 3f, ForceMode.Impulse);
        isPlayerOneServing = false;


    }*/
    //public Transform PlayersSpawnPosition()
    // {
    //     //return player1Position;
    // }
    private void HandleServeRotation(BallHitDetection detection, Collider collider)
    {
        
        
    }
    public bool CheckForSecondCollition(CollitionDetection cortObj)
    {
       
        if(!cortObj.collided2nd && ballCollitions==1)//ball only need to be checked if it is 2 collitions or 1 or 0 
                                                   //it is staying as 0 always, need it to check that?????????? 
        {
            Debug.Log(cortObj.collided2nd + "cortObj.collided2nd-------__-------------_----------");
            Debug.Log(ballCollitions + "ballCollitions-------__-------------_----------");
            ballCollitions++;
            return false;
        }
        else
        {
            Debug.Log("Ball has collided more that 1");
            ResetBallCollitions();
            return true;
        }
    }
    public void ResetBallCollitions()
    {
        ballCollitions = 1;
    }
    void HandleZoneHit(CourtZoneType zoneType, CollitionDetection cortObj , Collision collition)
    {
       // if (!isBallInPlay) return;
        
        switch (zoneType)
        {
            case CourtZoneType.LeftServiceBox:
                
                Debug.Log("LeftServiceBox Valid serve zone");
                if (CheckServeLegality(zoneType))
                {
                    //UpdateServeCount();
                    if (P1serveCount > 2 || P2serveCount >2)
                    {
                        Debug.Log(P1serveCount + "P1serveCount && " + P2serveCount + " P2serveCount");
                        //AwardPointToPlayers();
                        //ResetServeCount();
                        SwitchServer();
                    }
                }
                else
                {
                    Debug.Log(P1serveCount + "P1serveCount && " + P2serveCount + " P2serveCount");
                    // Valid serve: reset serve count and start rally.
                    ResetServeCount();
                   // isBallInPlay = false;
                }
                break;

            case CourtZoneType.Net:
                
                Debug.Log("Ball hit the net - fault or let");
                // Ball hit the net: fault or point for opponent.
                StartCoroutine(ShowStatus("Net"));
               
               
               
                //SecondBallHitOrOut( zoneType);
                break;

            case CourtZoneType.BackRunOff:

                Debug.Log(P1serveCount + "P1serveCount && " + P2serveCount + " P2serveCount");
                Debug.Log("Ball hit BackRunOff - checking if in or out");
                // Ball landed beyond the baseline (out-of-bounds): point to opponent.
                StartCoroutine(ShowStatus("Out-BackRunOff"));
                //SecondBallHitOrOut(zoneType);
                break;

            case CourtZoneType.SideRunOff:

                Debug.Log(P1serveCount + "P1serveCount && " + P2serveCount + " P2serveCount");
                Debug.Log("Ball hit sideline - checking for out call");
                // Ball landed beyond the sidelines (out-of-bounds): point to opponent.
             
                StartCoroutine(ShowStatus("Out-SideRunOff"));
                //SecondBallHitOrOut(zoneType);
                break;

            case CourtZoneType.Backcourt:

                Debug.Log(P1serveCount + "P1serveCount && " + P2serveCount + " P2serveCount");
               /* Debug.Log("Backcourt - point to opponent");*/
                //StartCoroutine(ShowStatus());
                //AwardPointToPlayers();
                SwitchServer();
                Debug.Log("Ball hit backcourt - checking for in bounds");
                // TODO: Apply point logic based on rally
                break;

            case CourtZoneType.RightServiceBox:
                Debug.Log("RightServiceBox Valid serve zone");
               
                if (CheckServeLegality(zoneType))
                {

                    // Serve fault: increment serve count and check for double fault.
                    //UpdateServeCount();
                    if (P1serveCount > 2 || P2serveCount > 2)
                    {
                        Debug.Log(P1serveCount + "P1serveCount && " + P2serveCount + " P2serveCount");
                        // Double fault: opponent wins the point.
                        //AwardPointToPlayers();
                        //ResetServeCount();
                        SwitchServer();
                    }
                }
                else
                {
                    
                    // Valid serve: reset serve count and start rally.
                    ResetServeCount();
                    //isBallInPlay = true;
                }
                break;
            case CourtZoneType.None:
            default:
                // Ball did not hit any recognized zone: treat as out-of-bounds.
                Debug.Log(P1serveCount + "P1serveCount && " + P2serveCount + " P2serveCount");
                //AwardPointToPlayers();
                SwitchServer();
                break;
        }
        UpdateUI();
    }
    // Stub: Check if the serve landed in the correct service box.
    public bool CheckServeLegality(CourtZoneType zoneType)// aading legality Rules
    {
        //zone.zoneType = zoneType;
        if(isPlayerOneServing && tag == "ServiceBox" || !isPlayerOneServing && tag == "P2ServiceBox")
        {
            Debug.Log("Condition Met-----(isPlayerOneServing && zoneType == CourtZoneType.P1Cort " +
                "|| !isPlayerOneServing && zoneType == CourtZoneType.P2Cort)");
            StartCoroutine(ShowStatus("Out!"));
           
        }
        if (P1serveCount >= 1 || P2serveCount >= 1) return true;
        else
        {
        //if ball lands on the servies box 
            if (IsServerRightSide && zoneType == CourtZoneType.LeftServiceBox)
            {
            return false;

            }
            else if (IsServerRightSide && zoneType == CourtZoneType.RightServiceBox)
            {
            IsServerRightSide = false;
            return true;
            }

            if (!IsServerRightSide && zoneType == CourtZoneType.RightServiceBox)
                return false;
            else if (!IsServerRightSide && zoneType == CourtZoneType.LeftServiceBox)
                return true;
            else
            {
                Debug.LogError("Invalid Serve");
                return false;
            }
        }
    }
    private void SwitchServer()
    {
        IsServerRightSide = !IsServerRightSide;
        isBallInPlay = false;
        Debug.Log("Server switched");
    }
    private void ResetServeCount()
    {
        P1serveCount = 0;
        P2serveCount = 0;

    }
    public void UpdateServeCount()
    {
        if(isPlayerOneServing)
        { P2serveCount++; }
        if(!isPlayerOneServing)
        { P1serveCount++; }
    }

    void AwardPointToCurrentPlayer()
    {
        if (isPlayerOneServing)
            playerOneScore++;
        else
            playerTwoScore++;

       Debug.Log($"Score - Player1SwingAction 1: {playerOneScore}, Player1SwingAction 2: {playerTwoScore}");
       
    }
    
    private void HandlePredictedLandingPoint(CourtZoneType zoneType, bool hasCollided, string tag)
    {
       switch(zoneType)
       {
            case CourtZoneType.RightServiceBox:
                ValidatePredictedLandingPoint(zoneType, tag);
                break;
            case CourtZoneType.LeftServiceBox:
                ValidatePredictedLandingPoint(zoneType, tag);
                break;
            default:
                ValidatePredictedLandingPoint(zoneType, tag);
                break;

                
       }
    }
    private void ValidatePredictedLandingPoint(CourtZoneType zones, string tag)
    {
        Debug.Log($"ZoneType: {zones}, IsServerRightSide: {IsServerRightSide}, tag:{tag}");
        //if ( tag == "ServiceBox")
        {
            if ((isPlayerOneServing && tag == "ServiceBox" )|| !isPlayerOneServing && tag == "P2ServiceBox")
            {
                Debug.Log("++++++++++Wrong ServiceBox");
                StartCoroutine(SwitchBallPositions());
                StartCoroutine(ShowStatus("Wrong ServiceBox - Invalid Serve!!"));
                /*AwardPointToCurrentPlayer();*/
                //SwichingPlayerPositionsToInitial();

            }
            else  if (!IsServerRightSide && zones == CourtZoneType.LeftServiceBox)
            {
                Debug.Log("<<<<<<<<<!IsServerRightSid");
                return;
            }
            else if (IsServerRightSide && zones == CourtZoneType.RightServiceBox)
            {
                Debug.Log("++++++++++IsServerRightSid");
                return;
            }
            else
            {
                StartCoroutine(SwitchBallPositions());
                StartCoroutine(ShowStatus("Invalid Serve!!"));
                /*AwardPointToCurrentPlayer();*/
                //SwichingPlayerPositionsToInitial();
            }
        }

        //if (tag.ToString() == ("CommonCort") || tag.ToString() == ("ServiceBox"))
        //{
        //    Debug.Log("---------------------CommonCort");
        //    return zones; /*CourtZoneType.None;*/ // or handle this appropriately
        //}
        return ;
    }
    public void SwichingPlayerPositionsToInitial()
    {

        if (switchValue == 0)
        {
            Debug.Log("(((((((((((((((((((((((((((((((");
            switchValue = 5;
            StartCoroutine(SetPlayerPositionToInitial(switchValue, player2Position));
            IsServerRightSide = !IsServerRightSide;
        }
        else
        {
            Debug.Log(")))))))))))))))))))))))))))))))))))))))))))))))))))))))))))");
            switchValue = 0;
            StartCoroutine(SetPlayerPositionToInitial(switchValue, player1Position));
            IsServerRightSide = !IsServerRightSide;
        }
    }
    void CheckZoneType(CourtZoneType zoneType)
    {
        //if(zoneType.)
    }
    private void AwardPointToPlayers(string status)
    {// show if its fault or out
        StartCoroutine(ShowStatus(status));
        // move player to the position

        // award points
        if (isPlayerOneServing & isBallTouched )
            playerTwoScore++;
        else if(!isPlayerOneServing & isBallTouched)
            playerOneScore++;

        //Debug.Log($"Score - Player1SwingAction 1: {playerOneScore}, Player1SwingAction 2: {playerTwoScore}");
    }
    public IEnumerator SwitchBallPositions()
    {
        isBallInPlay = false;
        yield return new WaitForSeconds(3f);
        if (IsEven(switchValue))
        {
            CreateBall(player1Position); 
        }
        else
        {
            CreateBall(player2Position);
            StartCoroutine(ResetIsBallInPlay());
        }
       
    }
    public IEnumerator SetPlayerPositionToInitial(float val, GameObject pos)
    {
        Vector3 p1pos  = p1InitPos;
        Vector3 p2pos  = p2InitPos;
        p1pos.x -= val;
        p2pos.x += val;
       isBallInPlay = false;////////////////////////////////////////////
        
        yield return new WaitForSeconds(3f);
        Ball.Instance.gameObject.SetActive(false);
        player1Position.transform.position = p1pos;
        player2Position.transform.position = p2pos;
        Ball.Instance.gameObject.SetActive(true);
        CreateBall(pos);
    }
    public GameObject CreateBall(GameObject obj)
    {
        if (Ball.Instance == null)
        {
            ball.transform.position = new Vector3(obj.transform.position.x,
               obj.transform.position.y + 1f,
               obj.transform.position.z + 1f);
            ball = Instantiate(ball); ball.name = "Ball";
           
            switchValue++;

            return ball;
        }
        else
        {
            StartCoroutine(MoveBallPos(obj));

            return ball;
        }
    }
    private void UpdateUI()
    {
        scoreText.text = "player1 : " + playerOneScore + "   player2 : " + playerTwoScore;
        //  infoText.text = "P1 " + P1serveCount + "--P2  " + P2serveCount /*+ "     player1 : " + playerOneScore + "   player2: " + playerTwoScore + "HasCollided "*/ + hasCollidedFromColliders + " isBallInPlay" + isBallInPlay + "   isBallTouched " + isBallTouched + " RightSide?=" + IsServerRightSide;
        infoText.text = "P1 " + P1serveCount + "--P2  " + P2serveCount + "----ballCollitions: " + ballCollitions + " isBallInPlay" + isBallInPlay;
        // TODO: Implement UI update logic, like setting text fields or scoreboards
    }
    private IEnumerator ShowStatus(string status)
    {
        scoreText.gameObject.SetActive(false);
        infoText.gameObject.SetActive(true);
        infoText.text = status;

        yield return new WaitForSeconds(5f);

        infoText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
    }
    
    public void SetBallTouched(bool value)
    {
        isBallTouched = value;
    }

    public void ToggleServer()
    {
        isPlayerOneServing = !isPlayerOneServing;
    }

    public void SetServer(bool isP1)
    {
        isPlayerOneServing = isP1;
    }
    public bool IsEven(int value)
    {
        int number = value;

        if (number % 2 == 0)
        {
            return true;
        }
        else
        {
            Debug.Log("Odd");
            return false;
        }
    }
    private IEnumerator MoveBallPos(GameObject obj)
    {
        Rigidbody rb = Ball.Instance.ballRb;

        Debug.Log("ball Instance is not null");

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        if(obj == player1Position)
        {
            float val = 2f;
            Ball.Instance.transform.position = new Vector3(obj.transform.position.x,
                obj.transform.position.y + 1f,
                obj.transform.position.z + val);
        }
        else
        {
            float val = -2f;
            Ball.Instance.transform.position = new Vector3(obj.transform.position.x,
                obj.transform.position.y + 1f,
                obj.transform.position.z + val);
        }
            yield return null;

        rb.isKinematic = false;
        rb.useGravity = true;

        switchValue++;
    }
    private IEnumerator ResetIsBallInPlay()
    {
        if (!isBallInPlay)
        {
            yield return new WaitForSeconds(3f);
            isBallInPlay = true;
        }
        else
            Debug.LogWarning("isBallInPlay is true - continuing without resetting");
       
    }//Reset the count so that it won't show out on the Ist ball touch////////////////////
}
