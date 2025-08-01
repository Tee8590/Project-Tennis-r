using System;
using UnityEngine;

public class CollitionDetection : MonoBehaviour
{
    public CourtZoneType zoneType;
    public static event Action<CourtZoneType, CollitionDetection> OnZoneHit;
   
    public static event Action<CourtZoneType, bool, string> PredictedLandingPoint;
    //public static event Action<CourtZoneType, CollitionDetection> OnSecondHit;
    public bool HasCollided = false;
    public bool collided2nd = false;
    // public bool HasCollided { get; private set; }


    public GameObject trailBallPrefab;




    private void OnTriggerEnter(Collider collider)
    {

        if (GameManager.Instance.isBallInPlay && GameManager.Instance.GameStarted)
        {
            if (collider.gameObject.CompareTag("Ball"))
            {
                
                    OnZoneHit?.Invoke(zoneType, this);
                    GameManager.Instance.SetBallTouched(true); Debug.Log($"[CollisionEnter] Setting HasCollided = true for {gameObject.name}");
                    HasCollided = true;


            }
        }

      
        if (collider.gameObject.CompareTag("BallPos") && !GameManager.Instance.hasCollidedFromColliders)
        {
            PredictedLandingPoint?.Invoke(zoneType, GameManager.Instance.hasCollidedFromColliders, this.gameObject.tag);

            GameManager.Instance.hasCollidedFromColliders = true;
           
        }

    }
   private void OnCollisionEnter(Collision collision)
    {
        if (!this.gameObject.CompareTag("Wall")) return;
        //    if (!GameManager.Instance.isBallInPlay) return;
        if (!GameManager.Instance.GameStarted) return;
        if(!GameManager.Instance.isValidServe) return;
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log($"[CollisionEnter] Setting HasCollided = true for {gameObject.name}"); //if (!HasCollided)
                                                                                             //{
                                                                                             //if (GameManager.Instance.GameStarted)
                                                                                             //{
                                                                                             //Debug.Log($"{gameObject.name} has zoneType: {zoneType}");

            {
                GameManager.Instance.SwichingPlayerPositionsToInitial(); 
                StartCoroutine(GameManager.Instance.ShowStatus("Out"));
                StartCoroutine(GameManager.Instance.SwitchBallPositions());
                if(this.gameObject.name == "Wall_F")
                    GameManager.Instance.playerOneScore++;
                else if(this.gameObject.name == "Wall_B")
                    GameManager.Instance.playerTwoScore++;
                GameManager.Instance.SwitchServer();
                GameManager.Instance.ResetServeCount();
            }
               // OnZoneHit?.Invoke(zoneType, this);
                GameManager.Instance.SetBallTouched(true);
                HasCollided = true;
                //}
            }
           
        }

    }

//private void OnCollisionEnter(Collision collision)
//{
//    if (!GameManager.Instance.isBallInPlay) return;
//    if (!GameManager.Instance.GameStarted) return;

//    if (collision.gameObject.CompareTag("Ball"))
//    {
//        if (!HasCollided)
//        {
//            //if (GameManager.Instance.GameStarted)
//            //{
//            //Debug.Log($"{gameObject.name} has zoneType: {zoneType}");
//            OnZoneHit?.Invoke(zoneType, this, collision);
//            GameManager.Instance.SetBallTouched(true);
//            HasCollided = true;
//            //}
//        }
//        //else if (HasCollided)
//        //{
//        //    if (GameManager.Instance.isBallTouched)
//        //    {
//        //        collided2nd = true;
//        //        OnSecondHit?.Invoke(zoneType, this);
//        //    }
//        //}
//    }


//}
//private void OnTriggerEnter(Collider collider)
//{
//    if (!collider.gameObject.CompareTag("BallPos")) return;
//    if (GameManager.Instance.hasCollidedFromColliders) return;
//    //if (gameObject.CompareTag("CommonCort")) return;

//    Debug.Log($"[Trigger] gameObject.name: {gameObject.name}, tag: {gameObject.tag}");
//    Debug.Log($"[Trigger] collider.name: {collider.name}, collider.tag: {collider.tag}");
//    //if (gameObject.CompareTag("ServiceBox"))
//    {
//        //HasCollided = true;
//        PredictedLandingPoint?.Invoke(zoneType, GameManager.Instance.hasCollidedFromColliders, this.gameObject.tag);
//        Debug.Log("OnTriggerEnter Success..................." + GameManager.Instance.hasCollidedFromColliders);
//        GameManager.Instance.hasCollidedFromColliders = true;
//        Debug.Log("OnTriggerEnter Success..............." + GameManager.Instance.hasCollidedFromColliders);

//    }

//}
//private void OnCollisionEnter(Collision collision)
//{
//    if (!GameManager.Instance.isBallInPlay) return;
//    if (!GameManager.Instance.GameStarted) return;

//    if (collision.gameObject.CompareTag("Ball"))
//    {
//        if (!HasCollided)
//        {
//            //if (GameManager.Instance.GameStarted)
//            //{
//            //Debug.Log($"{gameObject.name} has zoneType: {zoneType}");
//            OnZoneHit?.Invoke(zoneType, this, collision);
//            GameManager.Instance.SetBallTouched(true);
//            HasCollided = true;
//            //}
//        }
//        //else if (HasCollided)
//        //{
//        //    if (GameManager.Instance.isBallTouched)
//        //    {
//        //        collided2nd = true;
//        //        OnSecondHit?.Invoke(zoneType, this);
//        //    }
//        //}
//    }


//}
//private void OnTriggerEnter(Collider collider)
//{
//    if (!collider.gameObject.CompareTag("BallPos")) return;
//    if (GameManager.Instance.hasCollidedFromColliders) return;
//    //if (gameObject.CompareTag("CommonCort")) return;

//    Debug.Log($"[Trigger] gameObject.name: {gameObject.name}, tag: {gameObject.tag}");
//    Debug.Log($"[Trigger] collider.name: {collider.name}, collider.tag: {collider.tag}");
//    //if (gameObject.CompareTag("ServiceBox"))
//    {
//        //HasCollided = true;
//        PredictedLandingPoint?.Invoke(zoneType, GameManager.Instance.hasCollidedFromColliders, this.gameObject.tag);
//        Debug.Log("OnTriggerEnter Success..................." + GameManager.Instance.hasCollidedFromColliders);
//        GameManager.Instance.hasCollidedFromColliders = true;
//        Debug.Log("OnTriggerEnter Success..............." + GameManager.Instance.hasCollidedFromColliders);

//    }

//}