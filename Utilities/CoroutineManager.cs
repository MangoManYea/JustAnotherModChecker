using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModChecker.Utilities
{
    internal class CoroutineManager : MonoBehaviour
    {
        public static CoroutineManager instance;
        void Awake() { 
            instance = this;
        }
        public async void SendDelayed(int miliseconds,Action action)
        {
            await Task.Delay(miliseconds);
            action();
        }
    }
}
