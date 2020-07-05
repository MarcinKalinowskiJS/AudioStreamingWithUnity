using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.Scripts.Overall
{
    public class DropdownStoreObjectExtension<T> : Dropdown
    {
        Dropdown parent;
        List<T> dropdownObjects;

        public DropdownStoreObjectExtension (Dropdown d)
        {
            parent = d;
            dropdownObjects = new List<T>();
        }

        public void AddOptionWithObject(T methodObject, Dropdown.OptionData od) {
            dropdownObjects.Add(methodObject);
            base.options.Add(od);
        }

        public void ClearOptionsWithObjects() {
            dropdownObjects = new List<T>();
            base.options.Clear();
        }
    }
}
