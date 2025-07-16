using System;
using System.Collections;
using UnityEngine;

public class BallHitDetection : MonoBehaviour
{
    public static event Action<BallHitDetection, Collider> OnBallHit;
    public static event Action<Collider> OnPlayer2Hit;
    public static event Action<Collider> OnPlayer1Hit;
    public BallHitDetection ballHit;
    private Rigidbody ballRb;
    private GameObject ballPrefab;
    public float slowdownFactor = 0.7f;
    public LayerMask layerMask;

    public Transform parent;
    public GameObject player2;

    private void OnEnable()
    {
        
    }
    void Start()
    {
        parent = transform.parent;
        //layerMask = ~LayerMask.GetMask("Ground");
        ballHit = gameObject.GetComponent<BallHitDetection>();
    }
   
    public void OnTriggerEnter(Collider collider)
    {
        if (!GameManager.Instance.isBallInPlay) return;
        //Debug.Log("Name : " + collider.gameObject.name);
        if (collider.gameObject.name == "Ball")
        {
           // Rigidbody ballRb = collider.gameObject.GetComponent<Rigidbody>();

            Debug.Log("Name : " + parent.gameObject.name);
            if (parent.gameObject.name == "Player2")
            {
                //Debug.Log(collider.gameObject.name.ToString());
                OnPlayer2Hit?.Invoke(collider);
            }
            else if (parent.gameObject.name == "Player1")
            {
                GameManager.Instance.SetServer(true);
                Player1Hit(collider);
                    OnBallHit?.Invoke(ballHit, collider); 
              

            }
            //Debug.Log(collider.gameObject.name); return true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == "Ball"){
            if (parent.gameObject.name == "Player1")
            {
                GameManager.Instance.SetServer(false);
            }
        }
    }
    public bool Player1Hit(Collider collider)
    {
        if(collider.gameObject.name == "Ball")
        {
            return true;
        }
        return false;
    }
    public IEnumerator SlowBall(Rigidbody rb, float slowFactor, float duration)
    {
       
        Vector3 originalVelocity = rb.linearVelocity;

        //  slowdown
        rb.useGravity = false; rb.linearVelocity = originalVelocity * slowFactor;
      
        yield return new WaitForSeconds(duration);

        rb.useGravity = true; rb.linearVelocity = originalVelocity;
    }
    
}
