﻿using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WGestures.Core
{
    public abstract class AbstractApp
    {
        public string Name { get; set; }

        public GestureIntentDict GestureIntents { get; set; }
        /// <summary>
        /// 是否启用手势
        /// </summary>
        public bool IsGesturingEnabled { get; set; }


        protected AbstractApp()
        {
            Name = "Noname";
            GestureIntents = new GestureIntentDict();
            IsGesturingEnabled = true;
        }

        public virtual GestureIntent Find(Gesture key)
        {
           // Console.WriteLine("Find("+key.ToString()+")");
            GestureIntent val;
            GestureIntents.TryGetValue(key,out val);
            //if(val!=null)Console.WriteLine("Found? "+val.ToString());
            return val;
        }

        public virtual void Add(GestureIntent intent)
        {
            GestureIntents.Add(intent.Gesture,intent);
        }

        public virtual void Remove(GestureIntent intent)
        {
            GestureIntents.Remove(intent);
        }

        public virtual void Import(AbstractApp from)
        {
            IsGesturingEnabled = from.IsGesturingEnabled;
            Name = from.Name;
            ImportGestures(from);
        }

        public virtual void ImportGestures(AbstractApp from)
        {
            foreach (var kv in from.GestureIntents)
            {
                GestureIntents.AddOrReplace(kv.Value);
            }
        }
    }

    [JsonArray]
    public class GestureIntentDict : Dictionary<Gesture, GestureIntent>
    {
        public void Add(GestureIntent intent)
        {
            Add(intent.Gesture,intent);
        }

        public void Remove(GestureIntent intent)
        {
            Remove(intent.Gesture);
        }

        public void AddOrReplace(GestureIntent intent)
        {
            this[intent.Gesture] = intent;
        }

        public void Import(GestureIntentDict from)
        {
            foreach (var kv in from)
            {
                this[kv.Key] = kv.Value;
            }
        }
    }

    /// <summary>
    /// 用可执行文件路径来代表的应用程序
    /// </summary>
    public class ExeApp : AbstractApp
    {
        public bool InheritGlobalGestures { get; set; }

        public string ExecutablePath { get; set; }

        public override void Import(AbstractApp @from)
        {
            var asExeApp = @from as ExeApp;
            if (asExeApp != null)
            {
                InheritGlobalGestures = asExeApp.InheritGlobalGestures;
            }

            base.Import(@from);
        }

        public ExeApp()
        {
            InheritGlobalGestures = true;
        }
    }

    /// <summary>
    /// 表示全局有效的特殊应用
    /// </summary>
    public class GlobalApp : AbstractApp
    {
        public GlobalApp()
        {
            Name = "(Global)";
        }
    }
}