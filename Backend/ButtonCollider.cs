using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ModChecker.Backend
{
    internal class ButtonCollider : MonoBehaviour
    {
        static float delay;
        void Awake()
        {
            gameObject.layer = 18;
            gameObject.GetComponent<Collider>().isTrigger = true;
        }
        void OnTriggerEnter(Collider other)
        {
            if(other.name == "RightHandTriggerCollider" && Time.time > delay + 0.24f)
            {
                delay = Time.time;
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, false, 1f);
                if(OnPressed  != null) OnPressed.Invoke();
            }
        }
        public Action OnPressed = null;
    }
}
