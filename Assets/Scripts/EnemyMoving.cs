using UnityEngine;

public class EnemyMoving : MonoBehaviour
{
    public float speed = 1f;

    public Vector2 patrolMinBounds = new Vector2(-250, -250);
    public Vector2 patrolMaxBounds = new Vector2(250, 250);
    private Vector3 targetPosition;

    void Start()
    {
        ChooseTarget();
    }

    void Update()
    {
        MoveOnTarget();
    }

    void ChooseTarget()
    {

        float randomX = Random.Range(patrolMinBounds.x, patrolMaxBounds.x);
        float randomY = Random.Range(patrolMinBounds.y, patrolMaxBounds.y);
        targetPosition = new Vector3(randomX, randomY, transform.position.z);
    }

    void MoveOnTarget()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            ChooseTarget();
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(patrolMaxBounds.x*2, patrolMaxBounds.y*2, 0));
    }
}
