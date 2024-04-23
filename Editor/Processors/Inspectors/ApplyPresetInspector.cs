using Daihenka.AssetPipeline.Import;
using UnityEditor;
using UnityEngine;

namespace Daihenka.AssetPipeline.Processors
{
    [CustomEditor(typeof(ApplyPreset))]
    internal class ApplyPresetInspector : AssetProcessorInspector
    {
        private SerializedProperty m_Preset;
        private GUIContent m_buttonContent = new GUIContent("Edit Preset");
        private GUILayoutOption m_buttonHeight = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f);

        protected override void OnEnable()
        {
            m_Preset = serializedObject.FindProperty("preset");
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.HelpBox("This processor will only execute when a new asset is added.", MessageType.Warning);
            EditorGUILayout.Space();

            if (GUILayout.Button(m_buttonContent, m_buttonHeight))
            {
                EditorUtility.OpenPropertyEditor(m_Preset.objectReferenceValue);
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}