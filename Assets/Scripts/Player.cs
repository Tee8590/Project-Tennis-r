using UnityEngine;

public class Player : MonoBehaviour
{
    //private int Colliders = 10;
    private GameObject ball;
    public float moveSpeed = 20.0f;
    private Rigidbody ballRb;
    [SerializeField]
    private BallHitDetection ballHitDetection;
    public Vector3 ballPos;
    private  float dot;
    void FixedUpdate()
    {

        if (Ball.Instance != null)
        {
            ball = Ball.Instance.ballRb.gameObject;
            ballRb = Ball.Instance.ballRb;
        }
        else Debug.LogError("Null Ball.Instance");
        if (!GameManager.Instance.isBallInPlay) return;
        StartFollowTheBall();
        
    }
    public void StartFollowTheBall()
    {
      if(ballRb != null)  
      {
            Vector3 ballDirection = ballRb.linearVelocity.normalized;
            Vector3 directionToPlayer = (transform.position - ball.transform.position).normalized;
            dot = Vector3.Dot(ballDirection, directionToPlayer);

            if (dot < 0)
            {
                return;
                // Debug.Log("Ball is moving away from the player");
            }
            ////Ball is moving toward the player
            else if (dot > 0)
            {
                PlayerMovement();
                //Debug.Log("Ball is moving toward the player");
            }
           
        }
    }
    void PlayerMovement()
    {
        if(!GameManager.Instance.isBallInPlay) return;

        ballPos = Ball.Instance.landingPos;
        if(transform.gameObject.name == "Player1") ballPos.z /= 6;

        if (IsTargetInRange(ballPos) & ball != null)
        {
            Vector3 currentPosition = transform.position;
            float targetZ = ballPos.z;
            if (transform.gameObject.name == "Player1")
                targetZ = ballPos.z -5f;
            if (transform.gameObject.name == "Player2")
                targetZ = ballPos.z + 5f;
            targetZ = Mathf.Lerp(currentPosition.z, targetZ, moveSpeed * Time.deltaTime);
            float targetX = Mathf.Lerp(currentPosition.x, ball.transform.position.x, moveSpeed * Time.deltaTime);

            transform.position = new Vector3(targetX, currentPosition.y, targetZ);
            Vector3 movepos = new Vector3(targetX, currentPosition.y, targetZ);
        }
    }
    public bool IsTargetInRange(Vector3 target)
    {
        Vector3 center = transform.position;

        float minX = center.x - 10f;
        float maxX = center.x + 10f;
        float minZ = center.z - 10f;
        float maxZ = center.z + 10f;


        if (target.x >= minX && target.x <= maxX &&
                target.z >= minZ && target.z <= maxZ)
            return true;
        return false;
    }


}
