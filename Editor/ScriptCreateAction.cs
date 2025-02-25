// ========================================
// Author: ASOBIBA_
// Created: 2025-02-25 18:47:45
// ========================================

using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace ProjectAP.Core.Utilities.Editor
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ScriptCreateAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string templateContent = File.ReadAllText(resourceFile);
            File.WriteAllText(pathName, templateContent);
            AssetDatabase.ImportAsset(pathName);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(pathName);
        }
    }
}