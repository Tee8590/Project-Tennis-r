using UnityEngine;

public class Utils : MonoBehaviour
{
  public static Vector3 ScreenToWorld(Camera camera, Vector3 position, float zDepth = 10f)
    {
        
        Vector3 pos = new Vector3(position.x, position.y, zDepth);
        return camera.ScreenToWorldPoint(pos);
        //position.z = camera.nearClipPlane;
        //return camera.ScreenToWorldPoint(position); // for 2D ScreenPointToRay();
    }
}
