using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class SliderValueToText : MonoBehaviour {
  public Slider sliderUI;
  private TMP_Text textSliderValue;
 
  public string unit = " Min";
  void Start (){
    textSliderValue = GetComponent<TMP_Text>();
    ShowSliderValue();
  }

  public void ShowSliderValue () {
    string sliderMessage = sliderUI.value + unit;
    textSliderValue.text = sliderMessage;
  }
}