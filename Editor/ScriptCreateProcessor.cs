// ========================================
// Author: ASOBIBA_
// Created: 2025-02-16 10:26:55
// ========================================

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace ProjectAP.Core.Utilities.Editor
{
    /// <summary>
    /// 생성된 스크립트 내의 모든 스니펫을 그에 대응하는 값으로 변경합니다.
    /// </summary>
    public sealed class ScriptCreateProcessor : AssetModificationProcessor
    {
        /// <summary>
        /// 검사할 모든 스크립트의 확장자.
        /// </summary>
        private static readonly string[] m_ScriptFileSuffixes = { ".cs", ".js", ".boo" };

        /// <summary>
        /// .meta 파일의 확장자.
        /// </summary>
        private static readonly string m_MetaFileSuffix = ".meta";

        private static void OnWillCreateAsset(string path)
        {
            path = path.Replace(m_MetaFileSuffix, string.Empty);

            if (m_ScriptFileSuffixes.Any(path.EndsWith))
            {
                string fileContent = File.ReadAllText(path);
                string updatedContent = ReplaceSnippets(fileContent, path);

                File.WriteAllText(path, updatedContent);

                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 스크립트 내 모든 스니펫을 그에 대응하는 값으로 변경합니다.
        /// </summary>
        /// <param name="content">스크립트.</param>
        /// <param name="path">경로.</param>
        /// <returns>모든 값이 적용된 스크립트.</returns>
        private static string ReplaceSnippets(string content, string path)
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>()
            {
                { "#AUTHORNAME#", GetAuthorName() },
                { "#CREATEDDATE#", GetCreatedDate("yyyy-MM-dd HH:mm:ss") },
                { "#NAMESPACENAME#", GetNamespaceName(path) },
                { "#SCRIPTNAME#", GetScriptName(path) },
                { "#NOTRIM#", string.Empty },
                { "ALIGN", "\n" }
            };

            // 플레이스홀더 치환
            return Regex.Replace(content, "#(AUTHORNAME|CREATEDDATE|NAMESPACENAME|SCRIPTNAME|NOTRIM|ALIGN)#", match =>
            {
                return replacements.TryGetValue(match.Value, out var replacement)
                    ? replacement
                    : match.Value;
            });
        }

        /// <summary>
        /// 작성자 이름을 Git으로부터 가져옵니다.
        /// </summary>
        /// <returns>작성자의 Git 닉네임.</returns>
        private static string GetAuthorName()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "config --get user.name",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    return string.IsNullOrEmpty(output) ? "UnknownUser" : output;
                }
            }
            catch (Exception)
            {
                return "UnknownUser";
            }
        }

        /// <summary>
        /// 현재 날짜를 반환합니다.
        /// </summary>
        /// <param name="format">시간 표기 방식.</param>
        /// <returns>시간.</returns>
        private static string GetCreatedDate(string format)
        {
            return DateTime.Now.ToString(format);
        }

        /// <summary>
        /// 스크립트 경로를 바탕으로 네임스페이스 이름을 반환합니다.
        /// </summary>
        /// <param name="path">스크립트 경로.</param>
        /// <returns>네임스페이스 이름.</returns>
        private static string GetNamespaceName(string path)
        {
            string directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryName))
            {
                return directoryName
                    .Replace($"Assets{Path.DirectorySeparatorChar}Scripts{Path.DirectorySeparatorChar}", "")
                    .Replace(Path.DirectorySeparatorChar, '.');
            }

            return string.Empty;
        }

        /// <summary>
        /// 스크립트 경로를 바탕으로 스크립트 이름을 반환합니다.
        /// </summary>
        /// <param name="path">스크립트 경로.</param>
        /// <returns>스크립트 이름.</returns>
        private static string GetScriptName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
    }
}
#endif