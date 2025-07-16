using System;
using UnityEngine;

public class CollitionDetection : MonoBehaviour
{
    public CourtZoneType zoneType;
    public static event Action<CourtZoneType, CollitionDetection, Collision> OnZoneHit;
    public static event Action<CourtZoneType, bool, string> PredictedLandingPoint;
    public static event Action<CourtZoneType, CollitionDetection> OnSecondHit;
    public bool HasCollided = false;
    public bool collided2nd = false;
   // public bool HasCollided { get; private set; }
   

    public GameObject trailBallPrefab;
    
    private void OnCollisionEnter(Collision collision)
    {
        if(!GameManager.Instance.isBallInPlay) return;
        if (!GameManager.Instance.GameStarted) return;

        if (collision.gameObject.CompareTag("Ball"))
        {
            if(!HasCollided) 
            {
                //if (GameManager.Instance.GameStarted)
                //{
                    //Debug.Log($"{gameObject.name} has zoneType: {zoneType}");
                    OnZoneHit?.Invoke(zoneType, this, collision);
                    GameManager.Instance.SetBallTouched(true);
                    HasCollided= true;
                //}
            }
            else if(HasCollided)
            {
                //if (!collided2nd)
                {
                    collided2nd = true;
                    OnSecondHit?.Invoke(zoneType, this);
                }
            }
        }
       

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("BallPos")) return;
        if (GameManager.Instance.hasCollidedFromColliders) return;
        //if (gameObject.CompareTag("CommonCort")) return;

        Debug.Log($"[Trigger] gameObject.name: {gameObject.name}, tag: {gameObject.tag}");
        Debug.Log($"[Trigger] other.name: {other.name}, other.tag: {other.tag}");
        //if (gameObject.CompareTag("ServiceBox"))
        {
            //HasCollided = true;
            PredictedLandingPoint?.Invoke(zoneType, GameManager.Instance.hasCollidedFromColliders, this.gameObject.tag);
            Debug.Log("OnTriggerEnter Success..................." + GameManager.Instance.hasCollidedFromColliders);
            GameManager.Instance.hasCollidedFromColliders = true;
            Debug.Log("OnTriggerEnter Success..............." + GameManager.Instance.hasCollidedFromColliders);

        }
      
    }
   
}
