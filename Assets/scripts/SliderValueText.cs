using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueText : MonoBehaviour
{
   private Slider slider;
   private TMP_Text sliderText;

   void Awake()
   {

    slider = GetComponentInParent<Slider>();
    sliderText = GetComponent<TMP_Text>();

   }

   void Start()
   {

    UpdateText(slider.value);
    slider.onValueChanged.AddListener(UpdateText);

   }

   void UpdateText(float val)
   {

    sliderText.text = slider.value.ToString("0.00");

   }
}
