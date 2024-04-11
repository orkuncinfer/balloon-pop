﻿namespace SolidUtilities.Editor
{
    using System.IO;
    using System.Text;
    using UnityEditor;

    /// <summary>
    /// A class responsible for importing a generated DLL with appropriate settings.
    /// </summary>
    public static class AssemblyGeneration
    {
        public static void ImportAssemblyAsset(string assemblyPath, string assemblyGUID, bool editorOnly = false, bool excludeEditor = false)
        {
            // Sometimes, we need to know GUID of a future DLL before we start creating and importing it.
            // That's why it's passed as a parameter.
            string metaContent = editorOnly ? GetEditorMetaContent(assemblyGUID) : GetMetaFileContent(assemblyGUID, excludeEditor);

            // Starting from Unity 2019, .meta files are hidden and their direct editing via File.WriteAllText can
            // raise exceptions. Operating on a file through FileStream is safer and allows to be sure the file will be
            // edited without issues.
            using (var fs = new FileStream($"{assemblyPath}.meta", FileMode.Create))
            {
                using (TextWriter tw = new StreamWriter(fs, Encoding.UTF8, 1024, true))
                {
                    tw.Write(metaContent);
                }

                fs.SetLength(fs.Position);
            }

            AssetDatabase.ImportAsset(assemblyPath);
        }

        private static string GetMetaFileContent(string guid, bool excludeEditor)
        {
            // Auto Reference is disabled, Validate References is disabled, all platforms are enabled, and Any CPU/OS is chosen for all platforms.
            return $@"fileFormatVersion: 2
guid: {guid}
PluginImporter:
  externalObjects: {{}}
  serializedVersion: 2
  iconMap: {{}}
  executionOrder: {{}}
  defineConstraints: []
  isPreloaded: 0
  isOverridable: 0
  isExplicitlyReferenced: 1
  validateReferences: 0
  platformData:
  - first:
      : Any
    second:
      enabled: 0
      settings:
        Exclude Editor: {(excludeEditor ? "1" : "0")}
        Exclude Linux64: 0
        Exclude OSXUniversal: 0
        Exclude Win: 0
        Exclude Win64: 0
  - first:
      Any:
    second:
      enabled: 1
      settings: {{}}
  - first:
      Editor: Editor
    second:
      enabled: 1
      settings:
        CPU: AnyCPU
        DefaultValueInitialized: true
        OS: AnyOS
  - first:
      Standalone: Linux64
    second:
      enabled: 1
      settings:
        CPU: AnyCPU
  - first:
      Standalone: OSXUniversal
    second:
      enabled: 1
      settings:
        CPU: AnyCPU
  - first:
      Standalone: Win
    second:
      enabled: 1
      settings:
        CPU: x86
  - first:
      Standalone: Win64
    second:
      enabled: 1
      settings:
        CPU: x86_64
  - first:
      Windows Store Apps: WindowsStoreApps
    second:
      enabled: 0
      settings:
        CPU: AnyCPU
  userData:
  assetBundleName:
  assetBundleVariant:
";
        }

        private static string GetEditorMetaContent(string guid)
        {
            // Auto Reference is disabled, Validate References is disabled, only Editor platform is enabled.
            return $@"fileFormatVersion: 2
guid: {guid}
PluginImporter:
  externalObjects: {{}}
  serializedVersion: 2
  iconMap: {{}}
  executionOrder: {{}}
  defineConstraints: []
  isPreloaded: 0
  isOverridable: 0
  isExplicitlyReferenced: 1
  validateReferences: 0
  platformData:
  - first:
      : Any
    second:
      enabled: 0
      settings:
        Exclude Editor: 0
        Exclude Linux64: 1
        Exclude OSXUniversal: 1
        Exclude Win: 1
        Exclude Win64: 1
  - first:
      Any: 
    second:
      enabled: 0
      settings: {{}}
  - first:
      Editor: Editor
    second:
      enabled: 1
      settings:
        CPU: AnyCPU
        DefaultValueInitialized: true
        OS: AnyOS
  - first:
      Standalone: Linux64
    second:
      enabled: 0
      settings:
        CPU: None
  - first:
      Standalone: OSXUniversal
    second:
      enabled: 0
      settings:
        CPU: None
  - first:
      Standalone: Win
    second:
      enabled: 0
      settings:
        CPU: None
  - first:
      Standalone: Win64
    second:
      enabled: 0
      settings:
        CPU: None
  - first:
      Windows Store Apps: WindowsStoreApps
    second:
      enabled: 0
      settings:
        CPU: AnyCPU
  userData:
  assetBundleName:
  assetBundleVariant:
";
        }
    }
}