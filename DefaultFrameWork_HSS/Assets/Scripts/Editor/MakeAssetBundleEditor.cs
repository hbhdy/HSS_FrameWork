using System.IO;
using UnityEditor;
using UnityEngine;

namespace HSS
{
    public class MakeAssetBundleEditor : EditorWindow
    {
        private static string[] filePath = new string[2]; // [0] create folder ,  [1] flatform folder

        [MenuItem("Build/AssetbundleMaker")]
        private static void init()
        {
            var window = EditorWindow.GetWindow<MakeAssetBundleEditor>();
            window.titleContent = new GUIContent("AssetBundle Builder");
            window.ShowPopup();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("New AssetBundle Create", MessageType.None);

            if (GUILayout.Button("Create new all assetbundle"))
                CreateAssetBundles();
        }

        private static void CreateFolder()
        {
            string name = "AssetBundles";

            filePath[0] = name;
            if (Directory.Exists(filePath[0]) == false)
            {
                Directory.CreateDirectory(name);
                System.Threading.Thread.Sleep(1000);
            }

            // ÇÃ·§Æû ºÐ¸® ( È°¼ºÈ­µÈ ÇÃ·§Æû )
            name += "/" + EditorUserBuildSettings.activeBuildTarget.ToString();
            filePath[1] = name;

            if (Directory.Exists(filePath[1]) == false)
            {
                Directory.CreateDirectory(name);
                System.Threading.Thread.Sleep(1000);
            }
        }

        public static void CreateAssetBundles()
        {
            CreateFolder();

            BuildPipeline.BuildAssetBundles(filePath[1], BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            HSSLog.Log("AssetBundle build complete~!!!", LogColor.cyan);
        }
    }

}