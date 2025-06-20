using UnityEngine;

public class TorretaRaycast : MonoBehaviour
{
    [Header("Rotación")]
    public Transform cabezaRotatoria;
    public float velocidadRotacion = 5f;

    [Header("Disparo")]
    public Transform puntoDisparo;
    public float tiempoEntreDisparos = 1.5f;
    public int daño = 10;
    public float tiempoAviso = 0.5f;

    [Header("Visión")]
    public float rangoDisparo = 30f;
    [Range(0, 180)] public float anguloVision = 60f;
    public LayerMask capaObstaculos;
    public LayerMask capaJugador;

    private Transform objetivo;
    private float temporizador;
    private LineRenderer lineaLáser;
    private bool apuntando = false;

    void Start()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null) objetivo = jugador.transform;

        lineaLáser = GetComponent<LineRenderer>();
        if (lineaLáser != null)
        {
            lineaLáser.enabled = false;
            lineaLáser.positionCount = 2;
        }
    }

    void Update()
    {
        if (objetivo == null) return;

        float distancia = Vector3.Distance(transform.position, objetivo.position);
        if (distancia > rangoDisparo)
        {
            CancelarLaser();
            return;
        }

        Vector3 direccionJugador = (objetivo.position - transform.position).normalized;
        float angulo = Vector3.Angle(transform.forward, direccionJugador);
        if (angulo > anguloVision / 2f)
        {
            CancelarLaser();
            return;
        }

        Vector3 direccionRotacion = (objetivo.position - cabezaRotatoria.position).normalized;
        Quaternion rotacionDeseada = Quaternion.LookRotation(direccionRotacion);
        cabezaRotatoria.rotation = Quaternion.Slerp(cabezaRotatoria.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);

        Vector3 direccionDisparo = objetivo.position - puntoDisparo.position;
        if (Physics.Raycast(puntoDisparo.position, direccionDisparo.normalized, out RaycastHit hit, rangoDisparo, capaObstaculos | capaJugador))
        {
            if (((1 << hit.collider.gameObject.layer) & capaJugador) != 0)
            {
                temporizador += Time.deltaTime;

                if (!apuntando && temporizador >= (tiempoEntreDisparos - tiempoAviso))
                {
                    MostrarLaser(hit.point);
                    apuntando = true;
                }

                if (temporizador >= tiempoEntreDisparos)
                {
                    Disparar(hit.point);
                    temporizador = 0f;
                    CancelarLaser();
                }
            }
            else
            {
                CancelarLaser();
            }
        }
        else
        {
            CancelarLaser();
        }
    }

    void MostrarLaser(Vector3 puntoObjetivo)
    {
        if (lineaLáser == null) return;

        lineaLáser.SetPosition(0, puntoDisparo.position);
        lineaLáser.SetPosition(1, puntoObjetivo);
        lineaLáser.enabled = true;
    }

    void CancelarLaser()
    {
        apuntando = false;
        if (lineaLáser != null)
            lineaLáser.enabled = false;
    }

    void Disparar(Vector3 puntoImpacto)
    {
        PlayerHealth saludJugador = objetivo.GetComponent<PlayerHealth>();
        if (saludJugador != null)
        {
            saludJugador.TakeDamage(daño);
            Debug.DrawLine(puntoDisparo.position, puntoImpacto, Color.red, 0.2f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoDisparo == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDisparo);

        Vector3 frente = transform.forward;
        Quaternion rotacionIzquierda = Quaternion.AngleAxis(-anguloVision / 2f, Vector3.up);
        Quaternion rotacionDerecha = Quaternion.AngleAxis(anguloVision / 2f, Vector3.up);

        Vector3 limiteIzquierdo = rotacionIzquierda * frente;
        Vector3 limiteDerecho = rotacionDerecha * frente;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, limiteIzquierdo * rangoDisparo);
        Gizmos.DrawRay(transform.position, limiteDerecho * rangoDisparo);
    }
}
