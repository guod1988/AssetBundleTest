using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class AssetBundleBuilder
{
	public static readonly string resPath = "Assets/Res/";

	private static AssetBundleBuild[] CollectBundleByFolder()
	{
		AssetDatabase.Refresh();
		string[] all_assets = AssetDatabase.GetAllAssetPaths();

		Dictionary<string, List<string>> assetsCollector = new Dictionary<string, List<string>>();

		for (int i = 0; i < all_assets.Length; ++i)
		{
            string asset_path = all_assets[i];
			if (asset_path.StartsWith(resPath) && !AssetDatabase.IsValidFolder(asset_path))
			{
				string bundlePath = Path.GetDirectoryName(asset_path);
				if (bundlePath.EndsWith("/"))
				{
					bundlePath = bundlePath.Remove(bundlePath.Length - 1);
				}

				bundlePath = string.Format("{0}.unity3d", bundlePath);
				List<string> ls = null;
				if (!assetsCollector.TryGetValue(bundlePath, out ls))
				{
					ls = new List<string>();
					assetsCollector.Add(bundlePath, ls);
				}
				ls.Add(asset_path);
			}
		}

		AssetBundleBuild[] bundleBuilds = new AssetBundleBuild[assetsCollector.Count];
		int count = 0;
		foreach (KeyValuePair<string, List<string>> kv in assetsCollector)
		{
			bundleBuilds[count] = new AssetBundleBuild();
			bundleBuilds[count].assetBundleName = kv.Key;
			bundleBuilds[count].assetNames = kv.Value.ToArray();
			++count;
		}
		return bundleBuilds;
	}

	public static void BuildAssetBundles(BuildTargetGroup buildTargetGroup, BuildTarget targetPlatform)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, targetPlatform);

		AssetBundleBuild[] bundle_builds = CollectBundleByFolder();

		//string output_path = string.Format("{0}/{1}", Application.streamingAssetsPath, targetPlatform.ToString());
		string output_path = Application.streamingAssetsPath;
		if (!Directory.Exists(output_path))
		{
			Directory.CreateDirectory(output_path);
		}
		AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(output_path, bundle_builds, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, targetPlatform);
		AssetDatabase.Refresh();
	}

	[MenuItem("AssetBundle/BuildWindows")]
	public static void BuildAssetBundleWindows()
	{
		BuildAssetBundles(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
	}

	[MenuItem("AssetBundle/BuildIOS")]
	public static void BuildAssetBundleIOS()
	{
		BuildAssetBundles(BuildTargetGroup.iOS, BuildTarget.iOS);
	}

	[MenuItem("AssetBundle/BuildAndroid")]
	public static void BuildAssetBundleAndroid()
	{
		BuildAssetBundles(BuildTargetGroup.Android, BuildTarget.Android);
	}
}
