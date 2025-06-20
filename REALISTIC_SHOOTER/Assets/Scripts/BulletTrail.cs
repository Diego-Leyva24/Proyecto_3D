using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public void Init(Vector3 target, float speed)
    {
        StartCoroutine(MoveTowards(target, speed));
    }

    private System.Collections.IEnumerator MoveTowards(Vector3 target, float speed)
    {
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, target);
        float duration = distance / speed;
        float time = 0f;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}