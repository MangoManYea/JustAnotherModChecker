using System;
using System.Collections.Generic;
using System.Text;

namespace ModChecker.Libraries
{
    public class InputLibrary
    {
        public enum XRButton
        {
            Ainput,
            Binput,
            Xinput,
            Yinput,
            RightTrigger,
            LeftTrigger,
            RightGrip,
            LeftGrip,
        }
        static ControllerInputPoller poller() => ControllerInputPoller.instance;
        public static bool GetInput(XRButton button)
        {
            switch (button)
            {
                case XRButton.Ainput:
                    return poller().rightControllerPrimaryButton;
                case XRButton.Binput:
                    return poller().rightControllerSecondaryButton;
                case XRButton.Xinput:
                    return poller().leftControllerPrimaryButton;
                case XRButton.Yinput:
                    return poller().leftControllerSecondaryButton;
                case XRButton.RightTrigger:
                    return poller().rightControllerIndexFloat == 1f;
                case XRButton.LeftTrigger:
                    return poller().leftControllerIndexFloat == 1f;
                case XRButton.RightGrip:
                    return poller().rightControllerGripFloat == 1f;
                case XRButton.LeftGrip:
                    return poller().leftControllerGripFloat == 1f;
            }
            return false;
        }
    }
}
