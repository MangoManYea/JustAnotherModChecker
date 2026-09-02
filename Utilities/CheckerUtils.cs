using GorillaNetworking;
using HarmonyLib;
using ModChecker.Libraries;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Modio.API.ModioAPI;

namespace ModChecker.Utilities
{
    internal class CheckerUtils
    {
        static Dictionary<string, string> modprops = new Dictionary<string, string>()
        {
            {"Atlas", "Atlas Menu" },
            {"ØƦƁƖƬ","Orbit Menu" },
            {"ORBIT","Orbit Menu" },
            {"DTASLOI", "DTASLOI" },
            {"DTAOI", "DTAOI" },
            {"untitled", "Untitled" },
            {"ZlothYdances", "Fortnite Emote Wheel (Zlothy)" },
            {"Deez's ForniteEmoteWheel", "Fortnite Emote Wheel (Deez)" },
            {"github.com/ZlothY29IQ/MonkeRealism", "Monke Realism" },
            {"github.com/ZlothY29IQ/TooMuchInfo", "TooMuchInfo" },
            {"FPS-Nametags for Zlothy", "FPS Nametags" },
            {"asteroidlite", "Asteroid Lite Menu" },
            {"genesis", "ShibaGT Genesis" },
            {"elux", "Elux Menu" },
            {"obsidianmc", "Obsidian.LOL" },
            {"cronos", "Cronos Menu" },
            {"ShirtProperties", "Gorilla Shirts" },
            {"GorillaShirts", "Gorilla Shirts" },
            {"GS", "Gorilla Shirts" },
            {"GTrials", "Gorilla Trials" },
            {"hgrehngio889584739_hugb", "Resurgence Menu" },
            {"PulsarMenu", "Pulsar Menu" },
            {"BingusNametags++", "Bingus Nametags" },
            {"WalkSimulator", "Zlothy Walk Sim" },
            {"github.com/ZlothY29IQ/MonkeClick-CI", "Monke Click" },
            {"github.com/ZlothY29IQ/MonkeClick", "Monke Click" },
            {"Elixir", "Elixir Menu" },
            {"Vivid", "Vivid Menu" },
            {"cheese is gouda", "Who Is That Monke" },
            {"InfoWatch", "info Watch" },
            {"MonkePhone", "Monke Phone" },
            {"void", "Monke Phone" },
            {"BANANAOS", "BananaOS" },
            {"HP_Left", "Holdable Pad" },
            {"GrateVersion", "Grate" },
            {"Violet On Top", "Violet" },
            {"violetfree", "Violet Free" },
            {"violetpaiduser", "Violet Paid" },
            {"github.com/maroon-shadow/SimpleBoards", "Simple Boards" },
            {"Body Tracking", "Body Tracking" },
            {"Body Estimation", "Body Tracking" },
            {"Gorilla Track", "Body Tracking" },
            {"msp", "Monke Smartphone" },
            {"Deez's Gorilla Media", "Gorilla Media" },
            {"ThatUtilsPad", "That Utils Pad" },
            
        };
        public static string[] baseGameProps = new string[]
        {
            "didTutorial",
            "platform",
            "mothershipId"
        };
        public static string[] blacklistedProps = new string[]
        {
            "FUCK MOD CHECKERS",
            "@zlothyy on discord",
            "Gorilla ButtPlug",
            "Gorilla Buttplug",
            "Femboy Client",
            "WE RUN THIS GAME",
        };
        static Dictionary<string, object> keywordPairs = new Dictionary<string, object>
        {
            { "GorillaShirts", "Gorilla Shirts" },
            { "GorillaBody", "Gorilla Body" },
            { "67ur actually so weird for making a mod checker LMAO get a life kid", "Malachi's Menu Reborn" },
            { "juul_", "Juul Mod Menu" },
        };
        public static string CheckForKeywords(VRRig rig)
        {
            string foundkey = "";
            var props = rig.Creator.GetPlayerRef().CustomProperties;
            foreach(var prop in props)
            {
                foreach(var keypair in keywordPairs)
                {
                    if (prop.Key.ToString().ToLower().Contains(keypair.Key.ToString().ToLower())) foundkey += foundkey.Length > 0 ? ", " + keypair.Value : keypair.Value;
                }
            }
            return foundkey;
        }

        static bool CheckForCosmeticX(VRRig rig)
        {
            string cosmeticsowned = String.Concat(rig._playerOwnedCosmetics);
            CosmeticsController.CosmeticSet cosmeticSet = rig.cosmeticSet;
            if (cosmeticSet.items.Any(cosmetic => !cosmetic.isNullItem && !cosmeticsowned.Contains(cosmetic.itemName)))
               return true;
            return false;
        }
        public static string checkPlayer(VRRig rig)
        {
            var ispropspoofed = false;
            string mods = "";
            var playerprops = rig.Creator.GetPlayerRef().CustomProperties;
            
            foreach (DictionaryEntry entry in playerprops)
            {
                if (blacklistedProps.Contains(entry.Key)) ispropspoofed = true;
                foreach (KeyValuePair<string, string> modentry in modprops)
                {
                    if (ispropspoofed == true)
                    {
                        if (entry.Key.ToString().ToLower() != "untitled" && entry.Key.ToString().ToLower() != "github.com/ZlothY29IQ/MonkeRealism".ToLower() && entry.Key.ToString().ToLower() != "DTASLOI".ToLower() && entry.Key.ToString().ToLower() != "ZlothYdances".ToLower())
                        {
                            if (entry.Key.ToString().ToLower() == modentry.Key.ToString().ToLower())
                            {
                                mods += mods.Length > 0 ? ", " + modentry.Value : modentry.Value;
                            }
                        }
                    }
                    else
                    {
                        if (entry.Key.ToString().ToLower() == modentry.Key.ToString().ToLower())
                        {
                            mods += mods.Length > 0 ? ", " + modentry.Value : modentry.Value;
                        }
                    }
                }
            }
            if (CheckForCosmeticX(rig)) mods += mods.Length > 0 ? ", " + "CosmeticX" : "CosmeticX";
            if(CheckForKeywords(rig).IsNullOrEmpty() == false) mods += mods.Length > 0 ? ", " + CheckForKeywords(rig) : CheckForKeywords(rig);
            if (ReportSusProps(rig).IsNullOrEmpty() == false) mods += mods.Length > 0 ? ", " + ReportSusProps(rig) : ReportSusProps(rig);
            return mods;
        }
        public static void QuickScan()
        {
            foreach(VRRig rig in VRRigCache.ActiveRigs)
            {
                string checkedstring = checkPlayer(rig);
                if(!checkedstring.IsNullOrEmpty()) NotifiLib.SendNotification($"[<color=blue>SCAN</color>] {rig.Creator.NickName} Flagged! ({checkedstring})");
                if(isLowHZ(rig)) NotifiLib.SendNotification($"[<color=blue>SCAN</color>] {rig.Creator.NickName} Flagged! (Low FPS)");
            }
        }
        public static string GetPlatform(VRRig rig)
        {
            
            string result;
            string cosmetics = string.Concat(rig._playerOwnedCosmetics);
            if (cosmetics.Contains("S. FIRST LOGIN"))
            {
                result = "STEAM";
            }
            else
            {
                if (cosmetics.Contains("FIRST LOGIN"))
                {
                    result = "PC";
                }
                else
                {
                    result = "Quest";
                }
            }
            return result;
        }
        public static bool isLowHZ(VRRig rig)
        {
            var framerate = Traverse.Create(rig).Field("fps").GetValue().ToString();
            int.TryParse(framerate,out int formattedFps);
            bool ischeating = (formattedFps < 60);
            return ischeating;
        }
        public static string GetColor(VRRig rig)
        {
            string fetchedColor = $"{Mathf.RoundToInt(rig.playerColor.r * 9)},{Mathf.RoundToInt(rig.playerColor.g * 9)},{Mathf.RoundToInt(rig.playerColor.b * 9)}";
            return fetchedColor;
        }
        public static int GetPing(VRRig rig)
        {
            try
            {
                CircularBuffer<VRRig.VelocityTime> history = rig.velocityHistoryList;
                if (history != null && history.Count > 0)
                {
                    double ping = Math.Abs((history[0].time - PhotonNetwork.Time) * 1000);
                    return (int)Math.Clamp(Math.Round(ping), 0, int.MaxValue);
                }
            }
            catch { }
            return int.MaxValue;
        }
        //ripped from librepad [sorry ii]
        public static readonly Dictionary<string, float> waitingForCreationDate = new Dictionary<string, float>();
        public static readonly Dictionary<string, string> creationDateCache = new Dictionary<string, string>();
        public static string GetCreationDate(string input, Action<string> onTranslated = null, string format = "MM/dd/yyyy")
        {
            if (creationDateCache.TryGetValue(input, out string date))
                return date;
            if (!waitingForCreationDate.ContainsKey(input))
            {
                waitingForCreationDate[input] = Time.time + 10f;
                GetCreationCoroutine(input, onTranslated, format);
            }
            else
            {
                if (!(Time.time > waitingForCreationDate[input])) return "Loading...";
                waitingForCreationDate[input] = Time.time + 10f;
                GetCreationCoroutine(input, onTranslated, format);
            }

            return "Loading...";
        }
        public static string ReportSusProps(VRRig rig)
        {
            string susProps = "";
            var props = rig.Creator.GetPlayerRef().CustomProperties;
            foreach(DictionaryEntry entry in props)
            {
                if(!baseGameProps.Contains(entry.Key.ToString()) && !blacklistedProps.Contains(entry.Key.ToString()) && !isAKeyword(entry.Key.ToString()))
                {
                    if (!modprops.ContainsKey(entry.Key.ToString())) 
                        susProps += susProps.Length > 0 ? ", " + $"[SUSPROP] {entry.Key}" : $"[SUSPROP] {entry.Key}";
                }
            }
            return susProps;
        }
        public static bool isAKeyword(string toCheck)
        {
            foreach(string key in keywordPairs.Keys)
            {
                if(toCheck.Contains(key)) return true;
            }
            return false;
        }
        public static void GetCreationCoroutine(string userId, Action<string> onTranslated = null, string format = "MM/dd/yyyy")
        {
            if (creationDateCache.TryGetValue(userId, out string date))
            {
                onTranslated?.Invoke(date);
                return;
            }

            PlayFab.PlayFabClientAPI.GetAccountInfo(
                new PlayFab.ClientModels.GetAccountInfoRequest { PlayFabId = userId },
                result =>
                {
                    string creationDate = result.AccountInfo.Created.ToString(format);
                    creationDateCache[userId] = creationDate;
                    waitingForCreationDate.Remove(userId);
                    onTranslated?.Invoke(creationDate);
                },
                error =>
                {
                    creationDateCache[userId] = "Null";
                    waitingForCreationDate.Remove(userId);
                    onTranslated?.Invoke("Null");
                }
            );
        }
    }
}
