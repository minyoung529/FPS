using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    //[SerializeField] private Text currentAmmoText;
    [SerializeField] private TextMeshProUGUI currentAmmoText;

    private void Awake()
    {
        Instance = this;
    }
    
    public void ChangeCurrentAmmoText(int current)
    {
        currentAmmoText.text = current.ToString();
    }
}
