using UnityEngine;
using System.Collections;

public class MapTransition : MonoBehaviour
{
    public Transform position2;
    public GameObject player;
    public GameObject blackoutScreen; // �ο���ĺ���UIԪ��
    public float transitionDelay = 1.0f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning)
        {
            return;
        }

        if (!isTransitioning && other.CompareTag("Player"))
        {
            StartCoroutine(TransitionWithDelay(position2.position));
        }
    }

    private IEnumerator TransitionWithDelay(Vector3 targetPosition)
    {
        isTransitioning = true;

        // �������UIԪ��
        blackoutScreen.SetActive(true);

        // �ȴ������ӳ�
        yield return new WaitForSeconds(transitionDelay);

        // �������
        player.transform.position = targetPosition;

        // �ٴεȴ������ӳ�
        yield return new WaitForSeconds(transitionDelay);

        // ͣ�ú���UIԪ��
        blackoutScreen.SetActive(false);

        isTransitioning = false;
    }
}
