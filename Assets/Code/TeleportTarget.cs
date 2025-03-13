using UnityEngine;

/// <summary>
/// Class exists only to visualise Teleport targets in the Editor (NOT VIEWABLE IN GAME SCENE). 
/// Change to OnDrawGizmosSelected if causing too much confusion/clutter.
/// Authored by C.Bidner 6/3/25
/// </summary>
public class TeleportTarget : MonoBehaviour {
    /// <summary>
    /// Draws a wire sphere at the teleport target position. 
    /// </summary>
    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }
}