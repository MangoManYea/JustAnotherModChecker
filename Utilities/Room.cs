using GorillaLocomotion;
using GorillaNetworking;
using ModChecker.Libraries;
using Oculus.Platform;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModChecker.Utilities
{
    internal class Room
    {
        public static void LobbyHop()
        {
            if (PhotonNetwork.InRoom) { PhotonNetwork.Disconnect(); }
            CoroutineManager.instance.SendDelayed(1000, delegate
            {
                GorillaNetworkJoinTrigger trigger = PhotonNetworkController.Instance.currentJoinTrigger == null ? GorillaComputer.instance.GetJoinTriggerForZone("forest") : PhotonNetworkController.Instance.currentJoinTrigger;
                PhotonNetworkController.Instance.AttemptToJoinPublicRoom(trigger);
            });
        }
        public static void ReportPlayer(VRRig tar)
        {
            GorillaPlayerScoreboardLine.ReportPlayer(tar.Creator.UserId, GorillaPlayerLineButton.ButtonType.Cheating, tar.Creator.NickName);
            NotifiLib.SendNotification($"[<color=blue>REPORTER</color>] {tar.Creator.NickName} Reported For Cheating!");
        }
        public static void MutePlayer(VRRig tar)
        {
            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.playerActorNumber == tar.Creator.ActorNumber)
                {
                    line.muteButton.isOn = !line.muteButton.isOn;
                    line?.PressButton(line.muteButton.isOn, GorillaPlayerLineButton.ButtonType.Mute);
                }
            }
        }
        public static bool GetMuteStatus(VRRig tar)
        {
            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.playerActorNumber == tar.Creator.ActorNumber)
                {
                    return line.muteButton.isOn;
                }
            }
            return false;
        }
    }
}
