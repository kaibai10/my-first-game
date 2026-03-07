using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResoureController : MonoBehaviour
{
    public static ResoureController instance;

    private void Awake()
    {
        instance = this;
    }

    public float food, medicalProducts, energy, mineral;
    public TMP_Text foodText, medicalProductsText, energyText, mineralText;

    void Getfood(float amountToGet) 
    {
        food += amountToGet;
    }
    void GetmedicalProducts(float amountToGet)
    {
        medicalProducts += amountToGet;
    }
    void Getenergy(float amountToGet)
    {
        energy += amountToGet;
    }
    void Getmineral(float amountToGet)
    {
        mineral += amountToGet;
    }

    public void UpdataResoure() 
    {
        foodText.text = "Food: " + food.ToString();
        medicalProductsText.text = "medicalProducts: " + medicalProducts.ToString();
        energyText.text = "energy: " + energy.ToString();
        mineralText.text = "mineral: " + mineral.ToString();
    }
}
