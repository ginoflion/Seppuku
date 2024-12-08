using UnityEngine;

public class FinishLineArrow : MonoBehaviour
{
    [SerializeField] private RectTransform arrowRectTransform; 
    [SerializeField] private Transform finishLine; 
    [SerializeField] private Transform player; 

    void Update()
    {
        if (finishLine == null || player == null || arrowRectTransform == null)
            return;

        Vector3 direction = (finishLine.position - player.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
