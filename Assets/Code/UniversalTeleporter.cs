using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows player object to be teleported instantly to a new location.
/// Single teleport target mode travels only to one defined target (By reference in inspector).
/// Multiple teleport target mode allows defining a list of objects (By reference in inspector). 
/// A single object can be targeted by index or a random object selected.
/// Base case allows teleport to a selected vector defined as three floats. 
/// Authored by S.Watson(PlanetBridging) --/--/24
/// Updated by C.Bidner 6/3/25
/// </summary>
public class UniversalTeleporter : MonoBehaviour {
    [Header("Teleport Target Coordinates")]
    public float targetX; // X coordinate for teleport
    public float targetY; // Y coordinate for teleport
    public float targetZ; // Z coordinate for teleport

    [Space(10)]
    [Header("Single teleport target")]
    [Space(5)]
    [Tooltip("If false, use intial vector target implementation.")]
    public bool teleportToTarget = false;           //If false, use intial vector target implementation.
    [Tooltip("Gameobject to track, only requires a transform.")]
    public GameObject teleportTarget;               //Gameobject to track, only requires a transform.
    
    [Space(10)]
    [Header("Multiple teleport targets")]
    [Space(5)]
    [Tooltip("If selected, chooses a target from the below list.")]
    public bool useMultipleTargetsList = false;     //If true, will allow picking target from a list of objects
    [Tooltip("Allow Unity to pick a target at random.")]
    public bool pickRandomTargetFromList = false;   //If true, will disregard the index option below and pick target at random
    public int selectedTargetIndex = 0;             //Allows choosing a target from list by index
    [Space(5)]
    public List<GameObject> teleportTargets = new List<GameObject>();   //List of Targets(GameObjects)
    

    /// <summary>
    /// Checks any object colliding with the UniversalTeleporter.
    /// We are checking for the 'Player' tag to further perform operations. 
    /// GameObjects without this tag have no effect. 
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col){
        if (col.CompareTag("Player")){              //If Player object has collided with UniversalTeleporter
            if(useMultipleTargetsList){             //Check if we are using the multiple targets option
                if(pickRandomTargetFromList){       //-- If we ARE and we are allowing the game to pick at random
                    //Create a random number
                    int rando = Random.Range(0, teleportTargets.Count); 
                    //Set player position to the random objects position.    
                    col.transform.position = teleportTargets[rando].transform.position;
                    //Output log
                    Debug.Log("Success. New player position is: " + teleportTargets[rando].transform.position);
                }else{
                    //If we aren't using random, set player position to object with the selected index (default = 0)
                    col.transform.position = teleportTargets[selectedTargetIndex].transform.position;
                    //Output log
                    Debug.Log("Success. New player position is: " + teleportTargets[selectedTargetIndex].transform.position);
                }
            }else{                                  //-- If we are NOT using the multiple targets option
                if(teleportToTarget){               //Check if we are using the single target option
                    // Set the player's position to the target coordinates
                    col.transform.position = teleportTarget.transform.position;
                    // Output log
                    Debug.Log("Success. New player position is: " + teleportTarget.transform.position);
                }else{
                    //Finally, if all other options fail, use inital vector implementation to set player position
                    // Set the player's position to the target coordinates
                    col.transform.position = new Vector3(targetX, targetY, targetZ);
                    // Log the teleportation for debugging
                    Debug.Log($"Teleported {col.name} to: ({targetX}, {targetY}, {targetZ})");
                }
            }
        }
    }
}