using HarmonyLib;
using ModChecker.Backend;
using ModChecker.Libraries;
using ModChecker.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine;
using static ModChecker.Libraries.InputLibrary;

namespace ModChecker
{
    internal class Main: MonoBehaviour
    {
        public static GameObject checkerpad;
        static bool isOpen;
        static bool openar;
        static GameObject LHand;
        static float PlayerListRefreshDelay;
        static float MusicDataDelay;
        static float MusicInfoDelay;
        static float InfoTabRefreshDelay;
        static VRRig Inspecting = null;
        static bool muted;
        enum Pages
        {
            Room,
            Players,
            Info,
            Music
        }
        static Pages page = Pages.Room;
        void Update()
        {
            
            if (GorillaTagger.Instance.myRecorder != null) GorillaTagger.Instance.myRecorder.TransmitEnabled = !muted;
            if(LHand == null) {
                LHand = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/hand.L/palm.01.L");
            }
            if(GetInput(XRButton.Yinput) && openar == false) { isOpen = !isOpen; openar = true; }
            if(!GetInput(XRButton.Yinput)) { openar = false; }
            if (isOpen)
            {
               
                if (checkerpad == null)
                {
                    checkerpad = GameObject.Instantiate(AssetBundleHandler.LoadObjectFromBundle("ModChecker.Resources.checker","CheckerPad"));
                    checkerpad.transform.parent = LHand.transform;
                    checkerpad.transform.localPosition = Vector3.zero;
                    checkerpad.transform.localRotation = Quaternion.Euler(Vector3.zero);

                    checkerpad.transform.Find("Sidebar").transform.Find("Room").AddComponent<ButtonCollider>().OnPressed = delegate { page = Pages.Room; };
                    checkerpad.transform.Find("Sidebar").transform.Find("Players").AddComponent<ButtonCollider>().OnPressed = delegate { page = Pages.Players; };
                    checkerpad.transform.Find("Sidebar").transform.Find("Info").AddComponent<ButtonCollider>().OnPressed = delegate { page = Pages.Info; };
                    checkerpad.transform.Find("Sidebar").transform.Find("Music").AddComponent<ButtonCollider>().OnPressed = delegate { page = Pages.Music; };
                    checkerpad.transform.Find("Sidebar").transform.Find("Microphone").AddComponent<ButtonCollider>().OnPressed = delegate { muted = !muted; };

                    checkerpad.transform.Find("Panel").transform.Find("Room").transform.Find("LobbyHop").AddComponent<ButtonCollider>().OnPressed =() => Utilities.Room.LobbyHop();
                    checkerpad.transform.Find("Panel").transform.Find("Room").transform.Find("QuickScan").AddComponent<ButtonCollider>().OnPressed =() => CheckerUtils.QuickScan();
                   
                    checkerpad.transform.Find("Panel").transform.Find("Info").transform.Find("ReportButton").AddComponent<ButtonCollider>().OnPressed = delegate { if (Inspecting != null) Utilities.Room.ReportPlayer(Inspecting); };
                    checkerpad.transform.Find("Panel").transform.Find("Info").transform.Find("MuteButton").AddComponent<ButtonCollider>().OnPressed = delegate { if (Inspecting != null) Utilities.Room.MutePlayer(Inspecting); };

                    checkerpad.transform.Find("Panel").transform.Find("Players").transform.Find("Next").AddComponent<ButtonCollider>().OnPressed = delegate { isPage2 = true; RefreshPlayerButtons(); };
                    checkerpad.transform.Find("Panel").transform.Find("Players").transform.Find("Prev").AddComponent<ButtonCollider>().OnPressed = delegate { isPage2 = false; RefreshPlayerButtons(); };

                    checkerpad.transform.Find("Panel").transform.Find("Music").transform.Find("SkipBack").AddComponent<ButtonCollider>().OnPressed = () => Utilities.MusicManager.PrevTrack();
                    checkerpad.transform.Find("Panel").transform.Find("Music").transform.Find("SkipForward").AddComponent<ButtonCollider>().OnPressed = () => Utilities.MusicManager.NextTrack();
                    checkerpad.transform.Find("Panel").transform.Find("Music").transform.Find("ChangeState").AddComponent<ButtonCollider>().OnPressed = delegate { Utilities.MusicManager.ChangeSongStatus(); Utilities.MusicManager.paused = !Utilities.MusicManager.paused;  };
                }
                if (checkerpad != null)
                {
                    checkerpad.transform.Find("Panel").transform.Find("Room").gameObject.SetActive((page == Pages.Room));
                    checkerpad.transform.Find("Panel").transform.Find("Players").gameObject.SetActive((page == Pages.Players));
                    checkerpad.transform.Find("Panel").transform.Find("Info").gameObject.SetActive((page == Pages.Info));
                    checkerpad.transform.Find("Panel").transform.Find("Music").gameObject.SetActive((page == Pages.Music));
                    checkerpad.transform.Find("Sidebar").transform.Find("Microphone").Find("Muted").gameObject.SetActive(muted);
                    checkerpad.transform.Find("Sidebar").transform.Find("Microphone").Find("Unmuted").gameObject.SetActive(!muted);
                    if (page == Pages.Room)
                    {
                        if (PhotonNetwork.InRoom) checkerpad.transform.Find("Panel").transform.Find("Room").transform.Find("Room Info").gameObject.GetComponent<TextMeshPro>().text = $"Room: {PhotonNetwork.CurrentRoom.name}\nPlayers: {PhotonNetwork.PlayerList.Count()}/{PhotonNetwork.CurrentRoom.maxPlayers}\nPrivacy: {(PhotonNetwork.CurrentRoom.isVisible ? "Public" : "Private")}";
                        else checkerpad.transform.Find("Panel").transform.Find("Room").transform.Find("Room Info").gameObject.GetComponent<TextMeshPro>().text = "Not In Room";
                    }
                    if(page == Pages.Players)
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            Player[] players1 = PhotonNetwork.PlayerList.Take(5).ToArray();
                            Player[] players2 = PhotonNetwork.PlayerList.Skip(5).ToArray();

                            if(Time.time > PlayerListRefreshDelay + 1)
                            {
                                PlayerListRefreshDelay = Time.time;
                                RefreshPlayerButtons();
                            }
                        }
                    }
                    if(page == Pages.Info)
                    {
                        Gunlib.Gun(true, delegate { if (Gunlib.target != null) Inspecting = Gunlib.target; });
                        if (!PhotonNetwork.InRoom) Inspecting = null;
                        if(Inspecting != null)
                        {
                            var indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            indicator.transform.position = Inspecting.transform.position;
                            indicator.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            indicator.GetComponent<Collider>().enabled = false;
                            indicator.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            indicator.GetComponent<Renderer>().material.color = new Color32(207, 146, 212, 190);
                            GameObject.Destroy(indicator, Time.deltaTime);

                            checkerpad.transform.Find("Panel").transform.Find("Info").transform.Find("MuteButton").transform.Find("Title").GetComponent<TextMeshPro>().text = $"{(Utilities.Room.GetMuteStatus(Inspecting) ? "Unmute" : "Mute")} Player";
                        }
                        if (Time.time > InfoTabRefreshDelay + 1)
                        {
                            InfoTabRefreshDelay = Time.time;
                            if (Inspecting != null)
                            {
                                checkerpad.transform.Find("Panel").transform.Find("Info").transform.Find("Player Info").gameObject.GetComponent<TextMeshPro>().text = $"Name: {Inspecting.Creator.NickName}\nColor: {CheckerUtils.GetColor(Inspecting)}\nPing: {CheckerUtils.GetPing(Inspecting)}\nFPS: {Traverse.Create(Inspecting).Field("fps").GetValue()}\nCreation: {CheckerUtils.GetCreationDate(Inspecting.Creator.UserId)}\nPlatform: {CheckerUtils.GetPlatform(Inspecting)}\nMods: {CheckerUtils.checkPlayer(Inspecting)}";
                            }
                        }
                       
                    }
                    if(page == Pages.Music)
                    {
                        checkerpad.transform.Find("Panel").transform.Find("Music").transform.Find("ChangeState").transform.Find("Play").gameObject.SetActive(Utilities.MusicManager.paused);
                        checkerpad.transform.Find("Panel").transform.Find("Music").transform.Find("ChangeState").transform.Find("Pause").gameObject.SetActive(!Utilities.MusicManager.paused);
                        if (Time.time > MusicDataDelay + 1f)
                        {
                            MusicDataDelay = Time.time;
                            Utilities.MusicManager.UpdatePageData();
                        }
                        if (Utilities.MusicManager.SongData != null)
                        {
                            if(Time.time > MusicInfoDelay + 0.5f)
                            {
                                MusicInfoDelay = Time.time;
                                checkerpad.transform.Find("Panel").transform.Find("Music").transform.Find("SongName").gameObject.GetComponent<TextMeshPro>().text = Utilities.MusicManager.GetSongName();
                                checkerpad.transform.Find("Panel").transform.Find("Music").transform.Find("Author").gameObject.GetComponent<TextMeshPro>().text = Utilities.MusicManager.GetArtist();
                                checkerpad.transform.Find("Panel").transform.Find("Music").transform.Find("Time").gameObject.GetComponent<TextMeshPro>().text = $"{Utilities.MusicManager.FormatTime(Utilities.MusicManager.GetElapsedTime())}/{Utilities.MusicManager.FormatTime(Utilities.MusicManager.GetEndTime())}";
                                checkerpad.transform.Find("Panel").transform.Find("Music").transform.Find("SongIcon").gameObject.GetComponent<Renderer>().material.SetTexture("_BaseMap", Utilities.MusicManager.GetIcon());
                            }
                        }
                    }
                }
            }
            else GameObject.Destroy(checkerpad);
        }
        static bool isPage2 = false;
        static void RefreshPlayerButtons()
        {
            foreach(Transform playersButton in checkerpad.transform.Find("Panel").transform.Find("Players"))
            {
                if(playersButton.gameObject.name.Contains("PlayerButton_")) GameObject.Destroy(playersButton.gameObject);
            }
            Player[] players1 = PhotonNetwork.PlayerList.Take(5).ToArray();
            Player[] players2 = PhotonNetwork.PlayerList.Skip(5).ToArray();
            for(int i = 0; i < (isPage2 ? players2 : players1).Length; i++)
            {
                var newPlayer = GameObject.Instantiate(checkerpad.transform.Find("Panel").transform.Find("Players").Find("PlayerButton").gameObject);
                newPlayer.SetActive(true);
                newPlayer.transform.parent = checkerpad.transform.Find("Panel").transform.Find("Players");
                newPlayer.transform.localPosition = new Vector3(0.331999987f - (0.1f * i), -0.25f, 0f);
                newPlayer.transform.localRotation = Quaternion.identity;
                newPlayer.transform.localScale = checkerpad.transform.Find("Panel").transform.Find("Players").Find("PlayerButton").localScale;
                newPlayer.transform.Find("PlayerName").GetComponent<TextMeshPro>().text = (isPage2 ? players2 : players1)[i].nickName;
                newPlayer.gameObject.name = "PlayerButton_" + (isPage2 ? players2 : players1)[i].nickName;
                var list = (isPage2 ? players2 : players1);
                int netanyahu = i;
                var thisplayer = list[netanyahu];
                newPlayer.AddComponent<ButtonCollider>().OnPressed = delegate
                {
                    Inspecting = GorillaGameManager.instance.FindPlayerVRRig(thisplayer);
                    page = Pages.Info;
                };
            }
        }
    }
}
