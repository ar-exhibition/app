using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    public void Toggle(GameObject container)
    {
        if (container.activeInHierarchy)
        {
            StartCoroutine(LerpToScale(container.transform, new Vector3(0, 1, 1), 0.1f));
            StartCoroutine(LerpToPos(this.transform, Vector3.zero, 0.1f));           
        }
        else
        {
            StartCoroutine(LerpToScale(container.transform, new Vector3(1, 1, 1), 0.2f));
            StartCoroutine(LerpToPos(this.transform, new Vector3(385f, 0f, 0f), 0.2f));
        }
    }

    public IEnumerator LerpToPos(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.GetComponent<RectTransform>().anchoredPosition;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        transform.GetChild(0).GetComponent<RectTransform>().Rotate(0, 0, 180f);
    }

    public IEnumerator LerpToScale(Transform transform, Vector3 scale, float timeToMove)
    {
        bool scaled = false;

        if (!transform.gameObject.activeInHierarchy)
        {
            transform.gameObject.SetActive(true);
            scaled = true;
        }

        var currentScale = transform.GetComponent<RectTransform>().localScale;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.GetComponent<RectTransform>().localScale = Vector3.Lerp(currentScale, scale, t);
            yield return null;
        }

        if (transform.gameObject.activeInHierarchy && !scaled)
            transform.gameObject.SetActive(false);
    }
}
