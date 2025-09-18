using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    [Header("Movement")]
    public float speed = 5f;               // speed (meters per second)
    public float arriveThreshold = 0.1f;   // distance to consider "arrived"
    public bool alignToPath = true;        // align train to path direction
    public float rotateSpeed = 8f;         // rotation interpolation speed

    public enum LoopMode { Loop, PingPong, Once }
    public LoopMode loopMode = LoopMode.Loop;

    [Header("Control")]
    public bool playOnStart = true;        // start automatically when scene runs
    public bool clickableToggle = true;    // click train to start/stop

    int _index = 0;         // current waypoint index
    int _dir = 1;           // direction (1 = forward, -1 = backward for PingPong)
    bool _playing = false;
    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>(); // optional: use physics if Rigidbody exists
    }

    void Start()
    {
        // set initial position and orientation
        if (waypoints.Count > 0)
        {
            transform.position = waypoints[0].position;
            if (alignToPath && waypoints.Count > 1)
                transform.rotation = Quaternion.LookRotation(
                    (waypoints[1].position - waypoints[0].position).normalized, Vector3.up);
        }
        _playing = playOnStart && waypoints.Count > 1;
    }

    void Update()
    {
        if (!_playing || waypoints.Count < 2) return;

        Transform target = waypoints[_index];
        Vector3 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;

        // move towards target
        Vector3 step = toTarget.normalized * speed * Time.deltaTime;
        if (step.magnitude >= dist)
        {
            // snap to target to avoid jitter
            MoveTo(target.position);

            // go to next waypoint
            AdvanceIndex();
        }
        else
        {
            MoveTo(transform.position + step);
        }

        // smooth rotation towards target
        if (alignToPath)
        {
            Vector3 forward = (target.position - transform.position);
            if (forward.sqrMagnitude > 1e-4f)
            {
                Quaternion look = Quaternion.LookRotation(forward.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
            }
        }
    }

    void MoveTo(Vector3 pos)
    {
        if (_rb != null && !_rb.isKinematic)
        {
            // if using non-kinematic rigidbody, move with physics
            _rb.MovePosition(pos);
        }
        else
        {
            transform.position = pos;
        }
    }

    void AdvanceIndex()
    {
        // go to next waypoint index
        _index += _dir;
        if (_index >= waypoints.Count || _index < 0)
        {
            switch (loopMode)
            {
                case LoopMode.Loop:
                    _index = (_index + waypoints.Count) % waypoints.Count;
                    break;
                case LoopMode.PingPong:
                    _dir *= -1;
                    _index += _dir * 2; // bounce back
                    break;
                case LoopMode.Once:
                    _index = Mathf.Clamp(_index, 0, waypoints.Count - 1);
                    _playing = false;   // stop at the end
                    break;
            }
        }
    }

    void OnMouseDown()
    {
        if (!clickableToggle) return;
        _playing = !_playing; // toggle play/stop on click
    }

    void OnDrawGizmos()
    {
        // draw gizmos for waypoints in editor
        if (waypoints == null || waypoints.Count < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] && waypoints[i + 1])
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
