using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
  Text speedText;

  void Start()
  {
    speedText = GameObject.Find("SpeedText").GetComponent<Text>();
  }

  public void SetSpeed(float speed)
  {
    float speedKPH = speed * 3.6f;
    speedText.text = $"{speedKPH.ToString("0.0")} KM/H";
  }
}
