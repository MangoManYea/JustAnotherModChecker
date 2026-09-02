using GorillaLocomotion;
using ModChecker;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ModChecker.Libraries.InputLibrary;

namespace ModChecker.Libraries
{
    internal class Gunlib
    {
        public static VRRig target;
        public static RaycastHit gunpoint;
        static bool islocked;

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand()
        {
            Quaternion rot = GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handRotOffset;
            return (GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }
        public static void Gun(bool playergun = false, Action onShoot = null, Action onRelease = null)
        {
            if (GetInput(XRButton.RightGrip))
            {
                Physics.Raycast(TrueRightHand().position, TrueRightHand().forward, out gunpoint);
                GameObject line = new GameObject("femboy master 67");
                var theline = line.AddComponent<LineRenderer>();
                theline.material.shader = Shader.Find("GUI/Text Shader");
                theline.positionCount = 2;
                theline.startWidth = 0.005f;
                theline.endWidth = 0.005f;
                theline.startColor = new Color32(207, 146, 212, 190);
                theline.endColor = new Color32(207, 146, 212, 190);
                theline.SetPosition(0, TrueRightHand().position);
                theline.SetPosition(1, gunpoint.point);
                GameObject.Destroy(line, Time.deltaTime);
                if (GetInput(XRButton.RightTrigger))
                {
                    if (playergun)
                    {
                        if (gunpoint.collider.GetComponentInParent<VRRig>() != null && !islocked)
                        {
                            target = gunpoint.collider.GetComponentInParent<VRRig>();
                            islocked = true;
                        }
                    }
                    if (onShoot != null) onShoot();
                }
                else
                {
                    if (onRelease != null) onRelease();
                    target = null;
                    islocked = false;
                }
            }
        }
    }
}
