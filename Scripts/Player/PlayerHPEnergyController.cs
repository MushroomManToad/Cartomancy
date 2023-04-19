using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPEnergyController : MonoBehaviour
{
    public Image hpSlider;
    public Image energySlider;

    public void updateBarRendering(int currHealth, int maxHealth, int currEnergy, int maxEnergy)
    {
        setHPSlider(currHealth, maxHealth);
        setEnergySlider(currEnergy, maxEnergy);
    }

    public void setHPSlider(int currHealth, int maxHealth)
    {
        hpSlider.fillAmount = (float)currHealth / (float)maxHealth;
    }

    public void setEnergySlider(int currEnergy, int maxEnergy)
    {
        energySlider.fillAmount = (float)currEnergy / (float)maxEnergy;
    }
}
