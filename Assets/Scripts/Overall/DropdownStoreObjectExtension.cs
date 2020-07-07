﻿using Assets.Scripts.Model.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.Scripts.Overall
{
    public class DropdownStoreObjectExtension : Dropdown
    {
        List<TransferProtocol> dropdownObjects;

        public DropdownStoreObjectExtension ()
        {
            dropdownObjects = new List<TransferProtocol>();
        }

        public void AddOptionWithObject(TransferProtocol methodObject, Dropdown.OptionData od) {
            dropdownObjects.Add(methodObject);
            base.options.Add(od);
        }

        public void ClearOptionsWithObjects() {
            dropdownObjects = new List<TransferProtocol>();
            base.options.Clear();
        }
    }
}
