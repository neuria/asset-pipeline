using System.IO;
using Daihenka.AssetPipeline.Import;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Daihenka.AssetPipeline.Processors
{
    [AssetProcessorDescription(".prefab", ImportAssetTypeFlag.Models)]
    public class CreatePrefab : AssetProcessor
    {
        [SerializeField] TargetPathType prefabPathType = TargetPathType.SameAsAsset;
        [SerializeField] string prefabPath;
        [SerializeField] DefaultAsset targetFolder;
        [SerializeField] bool applyTag;
        [SerializeField] string tag = "Untagged";
        [SerializeField] bool applyLayer;
        [SerializeField] bool applyLayerRecursively = true;
        [SerializeField] LayerMask layer;

        [SerializeField] LightProbeUsage mrLightProbeUsage = LightProbeUsage.BlendProbes;
        [SerializeField] ReflectionProbeUsage mrReflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
        [SerializeField] ShadowCastingMode mrShadowCastingMode = ShadowCastingMode.On;
        [SerializeField] bool mrAllowOcclusionWhenDynamic;
        [SerializeField] bool mrReceiveShadows = true;
        [SerializeField] bool mrContributeGlobalIllumination;
        [SerializeField] bool mrCreateAnchorOverride;
        [SerializeField] ReceiveGI mrReceivedGlobalIllumination = ReceiveGI.LightProbes;
        [SerializeField] MotionVectorGenerationMode mrMotionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

        public override void OnPostprocess(Object asset, string assetPath)
        {
            var targetPath = Path.Combine(GetDestinationPath(assetPath), $"{Path.GetFileNameWithoutExtension(assetPath)}.prefab");

            var prefabInstance = PrefabUtility.InstantiatePrefab(asset) as GameObject;

            if (applyTag)
            {
                prefabInstance.tag = tag;
            }

            if (applyLayer)
            {
                prefabInstance.SetLayer(layer, applyLayerRecursively);
            }

            var renderers = prefabInstance.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.lightProbeUsage = mrLightProbeUsage;
                renderer.reflectionProbeUsage = mrReflectionProbeUsage;
                renderer.shadowCastingMode = mrShadowCastingMode;
                renderer.allowOcclusionWhenDynamic = mrAllowOcclusionWhenDynamic;
                renderer.receiveShadows = mrReceiveShadows;
                renderer.receiveGI = mrReceivedGlobalIllumination;
                renderer.motionVectorGenerationMode = mrMotionVectorGenerationMode;
                if (mrContributeGlobalIllumination)
                {
                    GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, StaticEditorFlags.ContributeGI);
                }

                if (mrCreateAnchorOverride)
                {
                    var anchor = new GameObject($"{renderer.name}-LightProbeAnchor");
                    anchor.transform.SetParent(renderer.transform);
                    renderer.probeAnchor = anchor.transform;
                }
            }

            PrefabUtility.SaveAsPrefabAsset(prefabInstance, targetPath);
            DestroyImmediate(prefabInstance);
            ImportProfileUserData.AddOrUpdateProcessor(assetPath, this);
            Debug.Log($"[{GetName()}] Prefab created for <b>{assetPath}</b>");
        }

        string GetDestinationPath(string assetPath)
        {
            var destinationPath = prefabPath;
            if (prefabPathType == TargetPathType.Absolute || prefabPathType == TargetPathType.Relative)
            {
                destinationPath = ReplaceVariables(destinationPath, assetPath);
            }

            destinationPath = prefabPathType.GetFolderPath(assetPath, destinationPath, targetFolder);
            if (prefabPathType == TargetPathType.TargetFolder && !targetFolder)
            {
                Debug.LogWarning($"[{GetName()}] Target Folder is not set.  Creating prefab at <b>{destinationPath}</b>");
            }

            destinationPath = destinationPath.FixPathSeparators();
            PathUtility.CreateDirectoryIfNeeded(destinationPath);

            return destinationPath;
        }
    }
}