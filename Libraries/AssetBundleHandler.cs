using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

namespace ModChecker.Libraries
{
    internal class AssetBundleHandler
    {
        public static AssetBundle LoadAssetBundle(string path)
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (bundle == null)
            {
                bundle = AssetBundle.LoadFromStream(manifestResourceStream);
            }
            manifestResourceStream.Close();
            return bundle;
        }
        public static AudioClip LoadAudioClipFromBundle(string path, string name)
        {
            AssetBundle assetBundle = LoadAssetBundle(path);
            return assetBundle.LoadAsset<AudioClip>(name);
        }
        public static Font LoadFontFromBundle(string path, string name)
        {
            AssetBundle assetBundle = LoadAssetBundle(path);
            return assetBundle.LoadAsset<Font>(name);
        }
        public static TMP_FontAsset LoadTMPFontFromBundle(string path, string name)
        {
            AssetBundle assetBundle = LoadAssetBundle(path);
            return assetBundle.LoadAsset<TMP_FontAsset>(name);
        }
        public static GameObject LoadObjectFromBundle(string path, string name)
        {
            AssetBundle assetBundle = LoadAssetBundle(path);
            return assetBundle.LoadAsset<GameObject>(name);
        }
        public static AssetBundle bundle;
    }
}

