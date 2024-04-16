using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    private void Awake()
    {
        if((float)Screen.height / (float)Screen.width > 9f / 16f)
        {
            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        }
        else
        {
            aspectRatioFitter.aspectMode= AspectRatioFitter.AspectMode.WidthControlsHeight;
        }
        aspectRatioFitter.aspectRatio = 1.75f;
    }
}
