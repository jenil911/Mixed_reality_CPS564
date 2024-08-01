// WARNING: Auto generated code. Modifications will be lost!
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using UnityEditor;

namespace Unity.Services.Deployment.Editor.Shared.Assets
{
    class ObservableAssets<T> : ObservableCollection<T>, IDisposable where T : UnityEngine.Object, IPath
    {
        readonly AssetPostprocessorProxy m_Postprocessor;
        protected readonly Dictionary<string, T> m_AssetPaths = new Dictionary<string, T>();

        public ObservableAssets() : this(new AssetPostprocessorProxy(), true) {}

        public ObservableAssets(AssetPostprocessorProxy assetPostprocessor, bool loadAssets)
        {
            m_Postprocessor = assetPostprocessor;
            m_Postprocessor.AllAssetsPostprocessed += AllAssetsPostprocessed;
            if (loadAssets)
            {
                LoadAllAssets();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void LoadAllAssets()
        {
            var assetPaths = AssetDatabase
                .FindAssets($"t:{typeof(T).Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !string.IsNullOrEmpty(path));
            foreach (var assetPath in assetPaths)
            {
                if (m_AssetPaths.ContainsKey(assetPath))
                {
                    continue;
                }

                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset == null)
                {
                    continue;
                }

                AddForPath(assetPath, asset);
            }
        }

        void AllAssetsPostprocessed(object sender, PostProcessEventArgs args)
        {
            if (args.DidDomainReload)
            {
                LoadAllAssets();
            }

            foreach (var imported in args.ImportedAssetPaths)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(imported);
                if (asset != null)
                {
                    if (!Contains(asset))
                    {
                        AddForPath(imported, asset);
                    }
                    else
                    {
                        UpdateForPath(imported, asset);
                    }
                }
            }

            args.DeletedAssetPaths
                .Where(m_AssetPaths.ContainsKey)
                .ForEach(d => RemoveForPath(d, m_AssetPaths[d]));

            foreach (var(movedToPath, movedFromPath) in args.MovedAssetPaths.Select((a, i) => (a, args.MovedFromAssetPaths[i])))
            {
                if (m_AssetPaths.ContainsKey(movedFromPath))
                {
                    MovePath(movedToPath, movedFromPath);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            m_Postprocessor.AllAssetsPostprocessed -= AllAssetsPostprocessed;
        }

        protected virtual void AddForPath(string path, T asset)
        {
            m_AssetPaths.Add(path, asset);
            m_AssetPaths[path].Path = path;
            Add(asset);
        }

        protected virtual void UpdateForPath(string path, T asset)
        {
            m_AssetPaths[path] = asset;
        }

        protected virtual void RemoveForPath(string path, T asset)
        {
            m_AssetPaths.Remove(path);
            Remove(asset);
        }

        protected virtual void MovePath(string toPath, string fromPath)
        {
            if (toPath != fromPath)
            {
                m_AssetPaths[toPath] = m_AssetPaths[fromPath];
                m_AssetPaths[toPath].Path = toPath;
                m_AssetPaths.Remove(fromPath);
            }
        }
    }
}
