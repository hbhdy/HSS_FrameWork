using HSS;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

// ���¹����� �ʿ���ų� ��巹���� ����� �� �ֱ⿡ ���� �̱��� ó��
namespace HSS
{
    public class AssetBundleManager : SingletonMono<GameCore>
    {
        private Dictionary<string, AssetBundle> dicLoadBundle = new();

        // Todo ���� ���� ���� ����
        // ��... ���¹����� �ܺο��� �޾ƿ� ���, �̹� ���� �Ͱ� �ؽ� �Ǵ�? CRC? 
        // �޾Ƽ� �޸������� �ٷ� ��� ?? �ƴϸ� ���� ����� ���ÿ� ���� ��� ? 
        // �׷��ٸ� ��ġ�� ���¹����� �޾ƾ��ϴ� ����� ���� �ִ� ���� ���� ? 

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