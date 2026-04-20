using UnityEngine;
using System;

namespace Crystal
{
    public class SafeAreaDemo : MonoBehaviour
    {
        [SerializeField] KeyCode KeySafeArea = KeyCode.A;
        SafeArea.SimDevice[] Sims;
        int SimIdx;

        void Awake ()
        {
            if (!Application.isEditor)
                Destroy (this);

            Sims = (SafeArea.SimDevice[])Enum.GetValues (typeof (SafeArea.SimDevice));
           
            if (Screen.width == 1125|| Screen.height== 1125)
            {
                SafeArea.Sim = Sims[1];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[1], KeySafeArea);
                //ToggleSafeArea();
            }
            else if (Screen.width == 1242 || Screen.height == 1242)
            {
                SafeArea.Sim = Sims[2];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[2], KeySafeArea);
            }
            else if (Screen.width == 1440 || Screen.height == 1440)
            {
                SafeArea.Sim = Sims[3];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[3], KeySafeArea);
            }
            else if (Screen.width == 2160 || Screen.height == 2160)
            {
                SafeArea.Sim = Sims[4];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[4], KeySafeArea);
            }
            else if (Screen.width == 828 || Screen.height == 828)
            {
                SafeArea.Sim = Sims[1];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[1], KeySafeArea);
            }
            else if (Screen.width == 2280 || Screen.height == 2280)
            {
                SafeArea.Sim = Sims[3];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[3], KeySafeArea);
            }
            else if (Screen.width == 3040 || Screen.height == 3040)
            {
                SafeArea.Sim = Sims[3];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[3], KeySafeArea);
            }
            else if (Screen.width == 2340 || Screen.height == 2340)
            {
                SafeArea.Sim = Sims[3];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[3], KeySafeArea);
            }
            else if (Screen.width == 2244 || Screen.height == 2244)
            {
                SafeArea.Sim = Sims[3];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[3], KeySafeArea);
            }
            else if (Screen.width == 3120 || Screen.height == 3120)
            {
                SafeArea.Sim = Sims[3];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[3], KeySafeArea);
            }
            else if (Screen.width == 2240 || Screen.height == 2240)
            {
                SafeArea.Sim = Sims[3];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[3], KeySafeArea);
            }
            else if (Screen.width == 2312 || Screen.height == 2312)
            {
                SafeArea.Sim = Sims[3];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[3], KeySafeArea);
            }
            else if (Screen.width == 3040 || Screen.height == 3040)
            {
                SafeArea.Sim = Sims[3];
                Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[3], KeySafeArea);
            }
        }

        void Update ()
        {
           // if (Input.GetKeyDown (KeySafeArea))
              //  ToggleSafeArea ();
        }

        /// <summary>
        /// Toggle the safe area simulation device.
        /// </summary>
        void ToggleSafeArea ()
        {
           SimIdx++;

            if (SimIdx >= Sims.Length)
                SimIdx = 0;

            SafeArea.Sim = Sims[SimIdx];
            Debug.LogFormat ("Switched to sim device {0} with debug key '{1}'", Sims[SimIdx], KeySafeArea);
        }
    }
}
