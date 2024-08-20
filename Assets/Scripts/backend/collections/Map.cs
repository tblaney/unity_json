// custom dictionary, with any type of data as long as 
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;
using System.IO.Compression;
using System.Text;
using System.Collections.Generic;

namespace snorri
{
    [System.Serializable]
    public class Map : INameable
    {   
        public string Name { get; set; }
        public Dictionary<string, object> Elements {get; set;}

        public TriggerLocal Trigger {
            get {
                if (triggerLocal == null) triggerLocal = new TriggerLocal();
                return triggerLocal;
            } 
            set {
                triggerLocal = value;
            }
        }
        TriggerLocal triggerLocal;
        
        public Map()
        {
            Elements = new Dictionary<string, object>();
        }
        public Map(string name) { 
            this.Name = name;
            Elements = new Dictionary<string, object>();
        }

        [JsonIgnore]
        public int Length 
        {
            get { if (Elements == null) return 0; return Elements.Count; }
        }

        public T Get<T>(string name, T defaultVal = default(T))
        {
            if (Elements == null)
            {
                return defaultVal;
            }
     
            if (name.Contains(":"))
            {
                string[] nameComponents = name.Split(":");
                Map m = Get<Map>(nameComponents[0], null);
                if (m == null)
                    return defaultVal;

                Bag<string> nameComponentsRemaining = new Bag<string>();
                for (int k = 1; k < nameComponents.Length; k++)
                {
                    nameComponentsRemaining.Append(nameComponents[k]);
                }
                string newName = FormName(nameComponentsRemaining);
                return m.Get<T>(newName, defaultVal);
            }   
                
            bool hasVal = Elements.TryGetValue(name, out object o);

            if (hasVal && o is T val)
            {
                return val;
            } else
            {
                return defaultVal;
            }
        }
        public void Set<T>(string name, T val)
        {
            if (Elements == null)
            {
                Elements = new Dictionary<string, object>();
            }

            if (name.Contains(":"))
            {
                string[] nameComponents = name.Split(":");
                Map m = Get<Map>(nameComponents[0], null);
                if (m == null)
                {
                    m = new Map();
                    Set<Map>(nameComponents[0], m);
                }
                
                Bag<string> nameComponentsRemaining = new Bag<string>();
                for (int k = 1; k < nameComponents.Length; k++)
                {
                    nameComponentsRemaining.Append(nameComponents[k], false);
                }
                string newName = FormName(nameComponentsRemaining);
                m.Set<T>(newName, val);

                if (triggerLocal != null) {
                    Map logMap = new Map();
                    triggerLocal.Trigger("change", logMap);
                }

                return;
            }    

            bool hasVal = Elements.TryGetValue(name, out object o);
            if (hasVal)
            {
                Elements.Remove(name);
            }

            Elements.Add(name, val as object);

            if (triggerLocal != null) {
                Map logMap = new Map();
                logMap.Set<string>("name", name);
                logMap.Elements.Add(name, val as object);
                triggerLocal.Trigger("change", logMap);
            }
        }

        string FormName(Bag<string> nameComponents)
        {
            if (nameComponents.Length == 1)
                return nameComponents[0];

            string newName = "";
            int i = 0;
            foreach (string s in nameComponents)
            {
                newName += s;
                if (i < nameComponents.Length - 1)
                {
                    newName+=":";
                }
                i++;    
            }

            return newName;
        }

        public bool Has(string name)
        {
            bool hasVal = Elements.ContainsKey(name);
            return hasVal;
        }
        public void Remove(string name)
        {
            if (Elements == null)
                return;

            if (name.Contains(":"))
            {
                string[] nameComponents = name.Split(":");
                Map m = Get<Map>(nameComponents[0], null);
                if (m == null)
                    return;

                Bag<string> nameComponentsRemaining = new Bag<string>();
                for (int k = 1; k < nameComponents.Length; k++)
                {
                    nameComponentsRemaining.Append(nameComponents[k]);
                }
                string newName = FormName(nameComponentsRemaining);
                m.Remove(newName);
                return;
            }  

            Elements.Remove(name);
        }
        public void Remove(List<string> keys)
        {
            foreach (string k in keys) {
                Remove(k);
            }
        }
        public void Log(string identifierText = "")
        {
            if (Elements == null)
                return;

            LOG.Console($"::map - {Name}, {identifierText}::\n {JSON.ToJson(this)}");
            //LOG.Console(JSON.ToJson(this));
        }
        public void Write(string name, bool isCompress = true, bool isNotify = true)
        {
            JSON.SaveMap(name, this, isCompress, isNotify);
        }
        public void WriteToPath(string name, string path, bool isCompress = true, bool isNotify = true) {
            JSON.SaveMapToPath(name, path, this, isCompress, isNotify);
        }

        public object GetObject(string name)
        {
            bool hasVal = Elements.TryGetValue(name, out object o);
            if (hasVal)
                return o;
            
            return default(object);
        }

        public void Sync(Map other, Bag<string> exceptions = null)
        {
            foreach (var kvp in other.Elements)
            {
                if (exceptions != null && exceptions.Contains(kvp.Key))
                    continue;
                    
                if (this.Has(kvp.Key))
                {
                    object o = this.Elements[kvp.Key];
                    object objectOther = kvp.Value;
                    //LOG.Console($"map sync found match '{kvp.Key}', this has type {o.GetType().Name}, other has type {objectOther.GetType().Name}");
                    if (o is Map m)
                    {
                        if (objectOther is Map mOther)
                        {
                            m.Sync(mOther);
                        } else
                        {
                            // other is not a map but ours is
                            this.Remove(kvp.Key);
                            this.Elements.Add(kvp.Key, objectOther);
                        }
                    } else
                    {
                        // not a map, so we remove our version
                        this.Remove(kvp.Key);
                        this.Elements.Add(kvp.Key, objectOther);
                    }
                } else
                {
                    //LOG.Console($"map sync did not find match for key {kvp.Key}");
                    this.Elements.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public static Map FromJson(string name, string typeName = "")
        {
            Map map = JSON.GetResourceMap(name, typeName);
        
            return map;
        }
        public static Map FromJsonPath(string path) {
            return JSON.GetMapFromPath(path);
        }
        public static Map FromJsonSave(string name, bool isCompress = true)
        {
            Map map = JSON.GetSaveMap(name, isCompress);

            return map;
        }
    }
}