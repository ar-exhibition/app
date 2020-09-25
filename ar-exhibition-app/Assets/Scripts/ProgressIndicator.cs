using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressIndicator : MonoBehaviour
{

    public float progress = 0;
    private Image _ovalImage;

    void Start() {
        _ovalImage = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        _ovalImage.fillAmount = Mathf.Lerp(_ovalImage.fillAmount, progress, Time.deltaTime * 5.0f); 
        _ovalImage.color = Color.Lerp(_ovalImage.color, getProgressColor(), Time.deltaTime * 5.0f);
    }

    public Color getProgressColor() {

        if (this.progress > 0.8f) {
            return Color.green;
        }

        if (this.progress > 0.6f) {
            return Color.yellow;
        }

        if (this.progress > 0.4f) {
            return new Color(1.0f, 0.6f, 0.0f);
        }

        return Color.red;

    }

}
