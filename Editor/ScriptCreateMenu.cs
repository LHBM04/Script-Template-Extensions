// ========================================
// Author: ASOBIBA_
// Created: 2025-02-16 10:26:55
// ========================================

#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ProjectAP.Core.Utilities.Editor
{
    [InitializeOnLoad]
    public static class ScriptCreateMenu
    {
        static ScriptCreateMenu()
        {
            #region Remove legacy menu items
            new List<string>() {
                "Assets/Create/MonoBehaviour Script",
                
                "Assets/Create/Scripting/MonoBehaviour Script",
                "Assets/Create/Scripting/ScriptableObject Script",
                "Assets/Create/Scripting/Empty C# Script",
                
                "Assets/Create/Scripting/Playables/Playable Behaviour Script",
                "Assets/Create/Scripting/Playables/Playable Asset Script",

                "Assets/Create/Scene/Scene Template Pipeline",

                "Assets/Create/Testing/C# Test Script",
            }.ForEach(RemoveMenuItem);
            #endregion
            #region Add new menu items
            AddMenuItem("Assets/Create/MonoBehaviour Script", -220, CreateMonoBehaviourScript);

            AddMenuItem("Assets/Create/Scripting/MonoBehaviour Script", -10, CreateMonoBehaviourScript);
            AddMenuItem("Assets/Create/Scripting/ScriptableObject Script", -11, CreateScriptableObjectScript);
            AddMenuItem("Assets/Create/Scripting/Empty C# Script", -12, CreateEmptyCSharpScript);
            
            AddMenuItem("Assets/Create/Scripting/Playables/Playable Behaviour Script", 1, CreatePlayableBehaviourScript);
            AddMenuItem("Assets/Create/Scripting/Playables/Playable Asset Script", 2, CreatePlayableAssetScript);

            AddMenuItem("Assets/Create/Scene/Scene Template Pipeline", 33, CreateSceneTemplatePiplineScript);

            AddMenuItem("Assets/Create/Animation/State Machine Behaviour", 22, CreateStateMachineBehaviourScript);
            AddMenuItem("Assets/Create/Animation/Sub State Machine Behaviour", 23, CreateSubStateMachineBehaviourScript);

            AddMenuItem("Assets/Create/Testing/C# Test Script", 83, CreateTestScript);
            #endregion
        }

        #region Menu Item Utilities
        /// <summary>
        /// 유니티 에디터에 메뉴 아이템을 생성합니다.
        /// (매개변수는 https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MenuItem-ctor.html를 참조.)
        /// </summary>
        /// <param name="menuItem">생성할 메뉴 아이템의 이름(경로).</param>
        /// <param name="priority">생성할 메뉴 아이템의 정렬 순서.</param>
        /// <param name="execute">생성할 메뉴 아이템의 수행 콜백</param>
        /// <param name="isCheck">생성할 메뉴 아이템에 체크 표시 기입 여부. 기본값은 false입니다.
        /// (자세한 것은 https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Menu.SetChecked.html를 참조.)/</param>
        /// <param name="shortcut">생성할 메뉴 아이템의 단축키(숏컷). 기본값은 null입니다. 
        /// (자세한 것은 https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MenuItem.html를 참조.)</param>
        /// <param name="validate">생성할 메뉴 아이템의 비수행 콜백. 기본값은 null입니다.</param>
        private static void AddMenuItem(string menuItem, int priority, Action execute, bool isCheck = false, string[] shortcut = null, Action<bool> validate = null)
        {
            typeof(Menu).GetMethod("AddMenuItem", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] 
            { 
                menuItem,   
                shortcut,
                isCheck,
                priority,
                execute,
                validate,
            });
        }

        /// <summary>
        /// 해당 이름(경로)의 메뉴 아이템을 삭제합니다.
        /// </summary>
        /// <param name="menuItem">지정할 메뉴 아이템의 이름(경로).</param>
        private static void RemoveMenuItem(string menuItem)
        {
            typeof(Menu).GetMethod("RemoveMenuItem", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { menuItem });
        }
        #endregion

        /// <summary>
        /// 스크립트 템플릿 파일을 이용하여 새로운 스크립트를 생성합니다.
        /// </summary>
        /// <param name="templatePath">사용할 스크립트 템플릿.</param>
        /// <param name="defaultName">기본 이름.</param>
        private static void HandleCreateScript(string templatePath, string defaultName)
        {
            if (!File.Exists(templatePath))
            {
                Debug.LogError($"해당 경로({templatePath})에서 스크립트 템플릿 파일을 찾을 수 없습니다!");
                return;
            }

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string directoryPath = Directory.Exists(path) ? path : Path.GetDirectoryName(path);

            string filePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directoryPath, $"{defaultName}"));

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<ScriptCreateAction>(),
                filePath,
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                templatePath
            );
        }

        private static readonly string MonoBehaviourTemplatePath = "Assets/Scripts/Templates/MonoBehaviour.txt";
        private static readonly string DefaultMonoBehaviourScriptName = "NewMonoBehaviourScript.cs";

        /// <summary>
        /// MonoBehaviour 스크립트를 생성합니다.
        /// </summary>
        private static void CreateMonoBehaviourScript()
        {
            HandleCreateScript(MonoBehaviourTemplatePath, DefaultMonoBehaviourScriptName);
        }

        private static readonly string m_ScriptableObjectTemplatePath = "Assets/Scripts/Templates/ScriptableObject.txt";
        private static readonly string m_DefaultScriptableObjectScriptName = "NewScriptableObjectScript.cs";

        /// <summary>
        /// ScriptableObject 스크립트를 생성합니다.
        /// </summary>
        private static void CreateScriptableObjectScript()
        {
            HandleCreateScript(m_ScriptableObjectTemplatePath, m_DefaultScriptableObjectScriptName);
        }

        private static readonly string m_EmptyCSharpTemplatePath = "Assets/Scripts/Templates/C#Script.txt";
        private static readonly string m_DefaultEmptyCSharpScriptName = "NewEmptyCSharpScript.cs";

        /// <summary>
        /// C# 스크립트를 생성합니다.
        /// </summary>
        private static void CreateEmptyCSharpScript()
        {
            HandleCreateScript(m_EmptyCSharpTemplatePath, m_DefaultEmptyCSharpScriptName);
        }

        private static readonly string m_PlayableAssetScriptTemplatePath = "Assets/Scripts/Templates/PlayableAsset.txt";
        private static readonly string m_DefaultPlayableAssetScriptName = "NewPlayableAsset.cs";

        /// <summary>
        /// Playable Asset 스크립트를 생성합니다.
        /// </summary>
        private static void CreatePlayableAssetScript()
        {
            HandleCreateScript(m_PlayableAssetScriptTemplatePath, m_DefaultPlayableAssetScriptName);
        }

        private static readonly string m_PlayableBehaviourScriptTemplatePath = "Assets/Scripts/Templates/PlayableBehaviour.txt";
        private static readonly string m_DefaultPlayableBehaviourScriptName = "NewPlayableBehaviour.cs";

        /// <summary>
        /// Playable Behaviour 스크립트를 생성합니다.
        /// </summary>
        private static void CreatePlayableBehaviourScript()
        {
            HandleCreateScript(m_PlayableBehaviourScriptTemplatePath, m_DefaultPlayableBehaviourScriptName);
        }

        private static readonly string m_SceneTemplatePiplineScriptTemplatePath = "Assets/Scripts/Templates/SceneTemplatePipline.txt";
        private static readonly string m_DefaultSceneTemplatePiplineScriptName = "NewSceneTemplatePipline.cs";

        /// <summary>
        /// Scene Template Pipeline 스크립트를 생성합니다.
        /// </summary>
        private static void CreateSceneTemplatePiplineScript()
        {
            HandleCreateScript(m_SceneTemplatePiplineScriptTemplatePath, m_DefaultSceneTemplatePiplineScriptName);
        }

        private static readonly string m_StateMachineBehaviourScriptTemplatePath = "Assets/Scripts/Templates/StateMachineBehaviour.txt";
        private static readonly string m_DefaultStateMachineBehaviourScriptName = "NewStateMachineBehaviour.cs";

        /// <summary>
        /// Scene Template Pipeline 스크립트를 생성합니다.
        /// </summary>
        private static void CreateStateMachineBehaviourScript()
        {
            HandleCreateScript(m_StateMachineBehaviourScriptTemplatePath, m_DefaultStateMachineBehaviourScriptName);
        }

        private static readonly string m_SubStateMachineBehaviourScriptTemplatePath = "Assets/Scripts/Templates/SubStateMachineBehaviour.txt";
        private static readonly string m_DefaultSubStateMachineBehaviourScriptName = "NewSubStateMachineBehaviour.cs";

        /// <summary>
        /// Sub Scene Template Pipeline 스크립트를 생성합니다.
        /// </summary>
        private static void CreateSubStateMachineBehaviourScript()
        {
            HandleCreateScript(m_SubStateMachineBehaviourScriptTemplatePath, m_DefaultSubStateMachineBehaviourScriptName);
        }

        private static readonly string m_TestScriptTemplatePath = "Assets/Scripts/Templates/TestScript.txt";
        private static readonly string m_DefaultTestScriptName = "NewTestScript.cs";

        /// <summary>
        /// Test 스크립트를 생성합니다.
        /// </summary>
        private static void CreateTestScript()
        {
            HandleCreateScript(m_TestScriptTemplatePath, m_DefaultTestScriptName);
        }
    }
}
#endif
