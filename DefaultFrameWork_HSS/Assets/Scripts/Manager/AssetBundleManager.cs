using HSS;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

// 에셋번들이 필요없거나 어드레서블 사용할 수 있기에 따로 싱글톤 처리
namespace HSS
{
    public class AssetBundleManager : SingletonMono<GameCore>
    {
        private Dictionary<string, AssetBundle> dicLoadBundle = new();

        // Todo 에셋 번들 수정 사항
        // 음... 에셋번들을 외부에서 받아올 경우, 이미 받은 것과 해시 판단? CRC? 
        // 받아서 메모리저장후 바로 사용 ?? 아니면 로컬 저장과 동시에 저장 사용 ? 
        // 그렇다면 패치된 에셋번들을 받아야하는 경우라면 원래 있던 것은 삭제 ? 

        public IEnumerator Co_LoadAssetBundle(string url, string bundleName)
        {
            if (dicLoadBundle.ContainsKey(bundleName))
            {
                HSSLog.Log($"AssetBundle already exist: {bundleName}");
                yield break;
            }

            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                HSSLog.LogError($"Fail download AssetBundle: {www.error}");
                yield break;
            }

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            if (bundle != null)
            {
                dicLoadBundle[bundleName] = bundle;
                HSSLog.Log($"Load AssetBundle: {bundleName}");
            }
            else
                HSSLog.LogError($"Fail AssetBundle: {bundleName}");
        }
    }
}