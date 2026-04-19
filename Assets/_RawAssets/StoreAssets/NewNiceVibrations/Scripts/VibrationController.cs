using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationController : MonoBehaviour
{
    private Lofelt.NiceVibrations.RegularPresetsDemoManager DemoManager;

    public static VibrationController Instance;

    private void Awake()
    {
        DemoManager = this.gameObject.AddComponent<Lofelt.NiceVibrations.RegularPresetsDemoManager>();
        Instance = this;
    }

    public void OnLevelClear()
    {
        DemoManager.LevelClear();
    }

    public void OnLevelFailed()
    {
        DemoManager.LevelFailed();
    }

    public void OnHeavyFrequency()
    {
        DemoManager.HardPattern();
    }

    public void Frequency_A()
    {
        DemoManager.A_Frequency();
    }

    public void Frequency_B()
    {
        DemoManager.B_Frequency();
    }

    public void Frequency_C()
    {
        DemoManager.C_Frequency();
    }

    public void Frequency_D()
    {
        DemoManager.D_Frequency();
    }

    public void Frequency_E()
    {
        DemoManager.E_Frequency();
    }

    public void Frequency_F()
    {
        DemoManager.F_Frequency();
    }
}
