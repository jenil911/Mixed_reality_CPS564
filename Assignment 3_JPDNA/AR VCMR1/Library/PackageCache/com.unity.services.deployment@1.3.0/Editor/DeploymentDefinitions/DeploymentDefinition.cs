using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Unity.Services.Deployment.Editor.Shared.Assets;
using Unity.Services.Deployment.Editor.Shared.EditorUtils;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using UnityEngine;
using Logger = Unity.Services.Deployment.Editor.Shared.Logging.Logger;

namespace Unity.Services.Deployment.Editor.DeploymentDefinitions
{
    class DeploymentDefinition : ScriptableObject, ICopyable<DeploymentDefinition>, IEditorDeploymentDefinition, IPath, ISerializationCallbackReceiver
    {
        static readonly JsonSerializerSettings k_JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public event PropertyChangedEventHandler PropertyChanged;

        [SerializeField]
        string m_DefinitionName;
        [SerializeField]
        List<string> m_ExcludePaths = new List<string>();
        string m_Path;

        ObservableCollection<string> m_ObservableExcludePaths = new ObservableCollection<string>();

        public string Path { get => m_Path; set => SetField(ref m_Path, value); }
        public string Name { get => m_DefinitionName; set => SetField(ref m_DefinitionName, value); }
        public ObservableCollection<string> ExcludePaths { get => m_ObservableExcludePaths; set => SetField(ref m_ObservableExcludePaths, value); }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(
                new
                {
                    Name,
                    ExcludePaths
                },
                k_JsonSerializerSettings);
        }

        public void FromJson(string json)
        {
            m_ObservableExcludePaths.CollectionChanged -= ObservableExcludePathsOnCollectionChanged;
            JsonConvert.PopulateObject(json, this, k_JsonSerializerSettings);
            m_ObservableExcludePaths.CollectionChanged += ObservableExcludePathsOnCollectionChanged;
        }

        void ObservableExcludePathsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<string>()
                        .ForEach(p => m_ExcludePaths
                            .Insert(
                                Mathf.Max(ExcludePaths.IndexOf(p), m_ExcludePaths.Count),
                                p));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.Cast<string>()
                        .ForEach(p => m_ExcludePaths.Remove(p));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    var path = e.OldItems.Cast<string>().First();
                    var index = m_ExcludePaths.IndexOf(path);
                    m_ExcludePaths[index] = e.NewItems.Cast<string>().First();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    m_ExcludePaths.Clear();
                    break;
                default:
                    Logger.LogError($"Deployment Definition does not support action `{e.Action}`");
                    break;
            }
        }

        public void SaveChanges()
        {
            var json = ToJson();
            File.WriteAllText(Path, json);
        }

        public void ReloadFromFile()
        {
            m_ObservableExcludePaths.CollectionChanged -= ObservableExcludePathsOnCollectionChanged;
            FromJson(File.ReadAllText(Path));
            m_ObservableExcludePaths.CollectionChanged += ObservableExcludePathsOnCollectionChanged;
        }

        public void CopyTo(DeploymentDefinition value)
        {
            value.Name = Name;
            value.ExcludePaths.Clear();
            ExcludePaths.ForEach(p => value.ExcludePaths.Add(p));
            value.m_ExcludePaths.Clear();
            value.m_ExcludePaths.AddRange(m_ExcludePaths);
        }

        public void OnBeforeSerialize()
        {
            m_ObservableExcludePaths.CollectionChanged -= ObservableExcludePathsOnCollectionChanged;
            m_ExcludePaths.Clear();
            m_ExcludePaths.AddRange(ExcludePaths);
            m_ObservableExcludePaths.CollectionChanged += ObservableExcludePathsOnCollectionChanged;
        }

        public void OnAfterDeserialize()
        {
            m_ObservableExcludePaths.CollectionChanged -= ObservableExcludePathsOnCollectionChanged;
            ExcludePaths.Clear();
            m_ExcludePaths.ForEach(p => ExcludePaths.Add(p));
            m_ObservableExcludePaths.CollectionChanged += ObservableExcludePathsOnCollectionChanged;
            Sync.RunNextUpdateOnMain(() =>
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Path));
            });
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
