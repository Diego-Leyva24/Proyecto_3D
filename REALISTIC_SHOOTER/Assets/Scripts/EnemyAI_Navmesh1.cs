using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_Navmesh : MonoBehaviour
{
    public Transform player;
    public int damage = 50;
    public float attackCooldown = 1.5f;
    public float distanciaMinima = 1.0f; // distancia para detener al enemigo

    private NavMeshAgent agent;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player != null)
        {
            float distancia = Vector3.Distance(transform.position, player.position);

            if (distancia > distanciaMinima)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                agent.isStopped = true; // Detener movimiento para evitar sobrepasar
                AlinearFrenteAlJugador(); // Opcional: que te mire al detenerse
            }
        }
    }

    void AlinearFrenteAlJugador()
    {
        Vector3 direccion = (player.position - transform.position).normalized;
        direccion.y = 0;
        if (direccion != Vector3.zero)
        {
            Quaternion rotacion = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, Time.deltaTime * 5f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time - lastAttackTime >= attackCooldown)
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                lastAttackTime = Time.time;
                Debug.Log("Daño aplicado por colisión al jugador.");
            }
        }
    }
}
