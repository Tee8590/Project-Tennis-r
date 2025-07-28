using System;
using UnityEngine;

public class CollitionDetection : MonoBehaviour
{
    public CourtZoneType zoneType;
    public static event Action<CourtZoneType, CollitionDetection, Collider> OnZoneHit;
    public static event Action<CourtZoneType, bool, string> PredictedLandingPoint;
    //public static event Action<CourtZoneType, CollitionDetection> OnSecondHit;
    public bool HasCollided = false;
    public bool collided2nd = false;
   // public bool HasCollided { get; private set; }
   

    public GameObject trailBallPrefab;


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
    private void OnTriggerEnter(Collider collider)
    {
     
        if (GameManager.Instance.isBallInPlay && GameManager.Instance.GameStarted)
        {
            if (collider.gameObject.CompareTag("Ball"))
            {
                //if (!HasCollided)
                {
                    //if (GameManager.Instance.GameStarted)
                    //{
                    //Debug.Log($"{gameObject.name} has zoneType: {zoneType}");
                    OnZoneHit?.Invoke(zoneType, this, collider);
                    GameManager.Instance.SetBallTouched(true); Debug.Log($"[CollisionEnter] Setting HasCollided = true for {gameObject.name}");
                    HasCollided = true;
                    //}
                }

            }
        }

        //Ball landing point marker ||||
        //                          VVVV

        //Debug.Log($"[Trigger] gameObject.name: {gameObject.name}, tag: {gameObject.tag}");
        //Debug.Log($"[Trigger] collider.name: {collider.name}, collider.tag: {collider.tag}");

        //if (gameObject.CompareTag("ServiceBox"))
        if (collider.gameObject.CompareTag("BallPos") && !GameManager.Instance.hasCollidedFromColliders)
        {
            //HasCollided = true;
            PredictedLandingPoint?.Invoke(zoneType, GameManager.Instance.hasCollidedFromColliders, this.gameObject.tag);
            //Debug.Log("OnTriggerEnter Success..................." + GameManager.Instance.hasCollidedFromColliders);
            GameManager.Instance.hasCollidedFromColliders = true;
            //Debug.Log("OnTriggerEnter Success..............." + GameManager.Instance.hasCollidedFromColliders);

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