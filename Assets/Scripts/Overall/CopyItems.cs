using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Overall
{
    public static class CopyItems
    {
        //OLD
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        //NEW https://forum.unity.com/threads/copy-component-fields-but-maintain-local-references.433654/
        public static T CopyComponentWithAdding<T>(this GameObject gameObject, T component, Action<T, T> action) where T : Component
        {
            var result = gameObject.AddComponent<T>();
            action(component, result);
            return result;
        }

        public static T CopyComponentWithGetting<T>(this GameObject gameObject, T component, Action<T, T> action) where T : Component
        {
            var result = gameObject.GetComponent<T>();
            action(component, result);
            return result;
        }
    }
    //Other: https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html?_ga=2.212327249.415498715.1607092553-1589858521.1607092553
    //Other: https://answers.unity.com/questions/458207/copy-a-component-at-runtime.html
    //Other: https://answers.unity.com/questions/28705/is-there-a-way-to-enumerate-fields-namevalue-on-an.html
}
