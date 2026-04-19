using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class RegularPresetsDemoManager : MonoBehaviour
    {
        public virtual void LevelClear()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
        }
       
        public virtual void LevelFailed()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
        }
        
        public virtual void HardPattern()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
        }

        public virtual void A_Frequency()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
        }

        public virtual void B_Frequency()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        }

        public virtual void C_Frequency()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }

        public virtual void D_Frequency()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        }

        public virtual void E_Frequency()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        }

        public virtual void F_Frequency()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
        }
    }
}
