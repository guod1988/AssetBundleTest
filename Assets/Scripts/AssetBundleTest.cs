using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleTest : MonoBehaviour
{
	private AssetBundleManifest mainifest = null;
    // Start is called before the first frame update
    void Start()
    {
		AssetBundle ab = AssetBundle.LoadFromFile(string.Format("{0}/StreamingAssets", Application.streamingAssetsPath));
		mainifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		string[] bundles = mainifest.GetAllAssetBundles();
		ab.Unload(false);
	}

	private Dictionary<string, AssetBundle> assetbundles = new Dictionary<string, AssetBundle>();

	private void OnGUI()
	{
		if (GUI.Button(new Rect(100.0f, 100.0f, 220.0f, 100.0f), "Load Materials"))
		{
			AssetBundle ab = LoadBundleAndDeps("assets/res/m.unity3d");
			Material[] mats = ab.LoadAllAssets<Material>();
			foreach (Material mat in mats)
			{
				Debug.LogFormat("Material:{0} with Texture:{1}", mat, (mat.mainTexture == null ? "null" : mat.mainTexture.ToString()));
			}
		}
	}

	private AssetBundle LoadBundleAndDeps(string bundleName)
	{
		string[] deps = mainifest.GetAllDependencies(bundleName);
		foreach (string dep in deps)
		{
			if (!assetbundles.ContainsKey(dep))
			{
				AssetBundle dep_ab = AssetBundle.LoadFromFile(string.Format("{0}/{1}", Application.streamingAssetsPath, dep));
				assetbundles.Add(dep, dep_ab);
			}
		}
		AssetBundle ab = null;
		if (!assetbundles.TryGetValue(bundleName, out ab))
		{
			ab = AssetBundle.LoadFromFile(string.Format("{0}/{1}", Application.streamingAssetsPath, bundleName));
			assetbundles.Add(bundleName, ab);
		}

		return ab;
	}
}
