using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Timberborn.AssetSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.FactionSystemGame;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace Code
{
    [BepInPlugin("Elec.DoubleLevee", "Double Levee", "0.1.0")]
    public class DoubleLeveePlugin : BaseUnityPlugin
    {
        private static GameObject modded_levee = null;
        private static GameObject cliff_platform = null;
        private static GameObject arch_4 = null;
        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(DoubleLeveePlugin));
            
            Logger.LogInfo($"Loaded!");
        }
        
        static void print_block_spec(String path, IResourceAssetLoader assetLoader)
        {
            var obj = assetLoader.Load<GameObject>(path);
            var block_object = obj.GetComponent<BlockObject>();
            Console.WriteLine($"{path} {block_object.BaseZ} {block_object.BlocksSpecification.Size}");
            foreach (var spec in block_object.BlocksSpecification.BlockSpecifications)
            {
                Console.WriteLine($"{spec.MatterBelow} {spec.Occupation} {spec.Stackable}");
            }    
            var placeableBlockObject = obj.GetComponent<PlaceableBlockObject>();
            Console.WriteLine($"{placeableBlockObject.CustomPivot.HasCustomPivot} {placeableBlockObject.CustomPivot.Coordinates}");
        }
        
        [HarmonyPatch(typeof(FactionObjectCollection), "GetObjects")]
        [HarmonyPostfix]
        static void FactionObjectCollection_GetObjects_Postfix(ref IEnumerable<Object> __result, FactionObjectCollection __instance)
        {
            if (modded_levee == null)
            {
                var assetLoaderInfo = typeof(FactionObjectCollection).GetField("_resourceAssetLoader", 
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
                if (assetLoaderInfo == null)
                {
                    throw new Exception("_resourceAssetLoader doesn't exist!");
                }
                var assetLoader = (IResourceAssetLoader) assetLoaderInfo.GetValue(__instance);

                print_block_spec("Buildings/Paths/SuspensionBridge3x1/SuspensionBridge3x1.Folktails", assetLoader);
                print_block_spec("Buildings/Paths/SuspensionBridge6x1/SuspensionBridge6x1.Folktails", assetLoader);
                var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Paths.PluginPath, "Elec.DoubleLevee/elec.doublelevee.bundle"));
                modded_levee = myLoadedAssetBundle.LoadAsset<GameObject>("Elec.DoubleLevee");
                cliff_platform = myLoadedAssetBundle.LoadAsset<GameObject>("Elec.3xCliffPlatform");
                arch_4 = myLoadedAssetBundle.LoadAsset<GameObject>("Elec.4x1Arch");
                
                var triple_platform = assetLoader.Load<GameObject>("Buildings/Paths/TriplePlatform/TriplePlatform.Folktails");
                arch_4.GetComponent<Building>().BuildingCost = triple_platform.GetComponent<Building>().BuildingCost;
                arch_4.GetComponent<Building>().BuildingCost[0].Amount = 8;
                arch_4.GetComponent<Building>().BuildingCost[1].Amount = 8;
                
                var levee_model = Instantiate(assetLoader.Load<GameObject>("Buildings/Landscaping/Levee/Levee.Folktails.Model"));
                levee_model.transform.parent = modded_levee.transform.Find("__Finished");
                levee_model.transform.localScale = new Vector3(1, 2, 1);
                
                var levee_model_2 = Instantiate(assetLoader.Load<GameObject>("Buildings/Landscaping/Levee/Levee.Folktails.Model"));
                levee_model_2.transform.parent = cliff_platform.transform.Find("__Finished");
                levee_model_2.transform.localScale = new Vector3(3, 1, 1);
                levee_model_2.transform.localPosition = new Vector3(0, 0, 0);

                var platform_model = assetLoader.Load<GameObject>("Buildings/Paths/Platform/Platform.Full.Folktails");

                arch_4.transform.Find("__Finished/model/default").gameObject.GetComponent<MeshRenderer>().materials =
                    platform_model.GetComponent<MeshRenderer>().materials;
                arch_4.transform.Find("__FinishedUncovered/model/default").gameObject.GetComponent<MeshRenderer>().materials =
                    levee_model_2.GetComponent<MeshRenderer>().materials;
            } else {
                __result = __result.Concat(new []{ modded_levee, cliff_platform, arch_4 });
            }
        } 

    }
}
