using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISlider : MonoBehaviour
{
    [HideInInspector] public Slider slider;

    [SerializeField] private Text countText;

    private void Start()
    {
        slider = GetComponent<Slider>();


        SetCountText();
    }

    public void SetCountText()
    {
        countText.text = System.Math.Round(slider.value, 1).ToString();
    }


}
