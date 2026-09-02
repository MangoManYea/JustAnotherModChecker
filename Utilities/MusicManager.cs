using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace ModChecker.Utilities
{
    internal class MusicManager
    {
        static float UpdateDelay;
        static string quicksongpath;
        public static bool paused;
        public static Dictionary<string, object> SongData;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);
        static void SendKeyEvent(uint key) => keybd_event(key, 0, 0,0);
        static async Task InstallQuickSong()
        {
            if (!Directory.Exists("JustAnotherModChecker")) Directory.CreateDirectory("JustAnotherModChecker");
            quicksongpath = Path.Combine("JustAnotherModChecker", "QuickSong.exe");
            string resource = "ModChecker.Resources.QuickSong.exe";
            if (!File.Exists(quicksongpath))
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
                FileStream fs = new FileStream(quicksongpath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fs);
            }
        }
        public static async void UpdatePageData()
        {
            if(quicksongpath.IsNullOrEmpty()) await InstallQuickSong();
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = quicksongpath,Arguments = "-all", CreateNoWindow = true,UseShellExecute = false,RedirectStandardOutput = true };
            Process proc = Process.Start(startInfo);
            string output = await proc.StandardOutput.ReadToEndAsync();
            try {
                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(output);
                SongData = data;
                paused = IsPaused();
            }
            catch { SongData = null; }
        }
        public static string GetSongName()
        {
            string Title = "";
            if(SongData != null) Title = (string)SongData["Title"];
            return Title;
        }
        public static bool IsPaused()
        {
            if (SongData != null) return (string)SongData["Status"] == "Paused";
            return false;
        }
        public static string GetArtist()
        {
            string Title = "";
            if (SongData != null) Title = (string)SongData["Artist"];
            return Title;
        }
        public static Texture2D GetIcon()
        {
            var icon = new Texture2D(2, 2);
            if(SongData != null) icon.LoadImage(Convert.FromBase64String((string)SongData["ThumbnailBase64"]));
            return icon;
        }
        public static Single GetElapsedTime()
        {
            Single et = 0;
            if (SongData != null) et = Convert.ToSingle(SongData["ElapsedTime"]);
            return et;
        }
        public static Single GetEndTime()
        {
            Single et = 0;
            if (SongData != null) et = Convert.ToSingle(SongData["EndTime"]);
            return et;
        }
        public static Single GetStartTime()
        {
            Single et = 0;
            if (SongData != null) et = Convert.ToSingle(SongData["StartTime"]);
            return et;
        }
        public static string FormatTime(Single Time)
        {
            if(SongData != null)
            {
                float clampedTime = Mathf.Clamp(Time,GetStartTime(), GetEndTime());
                string formattedTime = $"{Mathf.Floor(clampedTime / 60)}:{Mathf.Floor(clampedTime % 60):00}";
                return formattedTime;
            }
            string formattedTimeBackup = $"{Mathf.Floor(Time / 60)}:{Mathf.Floor(Time % 60):00}";
            return formattedTimeBackup;
        }
        public static void ChangeSongStatus() 
        {
            SendKeyEvent(0xB3);
        } 
        public static void NextTrack() => SendKeyEvent(0xB0);
        public static void PrevTrack() => SendKeyEvent(0xB1);

    }
}
