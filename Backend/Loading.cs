using System;
using BepInEx;
using GorillaLocomotion;
using ModChecker.Utilities;
using UnityEngine;

namespace ModChecker
{
    [BepInPlugin("org.Mango.ModChecker","Mango's Mod Checker","1.0.0")]
    public class Loading:BaseUnityPlugin
    {
        static bool init;
        static GameObject load;
        void Update()
        {
            if(init == false && GTPlayer.hasInstance)
            {
                load = new GameObject("sdfdsff");
                load.AddComponent<Main>();
                load.AddComponent<CoroutineManager>();
                init = true;
            }
        }
    }
}
