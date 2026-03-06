using UnityEngine;

public class LookAtPlayerAnimation : MonoBehaviour
{
    public Transform player;
    public Animator animator;
    public float viewAngle = 45f;

    private int idleHash;
    private int lookHash;

    void Start()
    {
        idleHash = Animator.StringToHash("Idle");
        lookHash = Animator.StringToHash("Waving");
        animator.Play(idleHash);
    }

    void Update()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle <= viewAngle)
        {
            animator.Play(lookHash);
        }
        else
        {
            animator.Play(idleHash);
        }
    }
}