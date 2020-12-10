using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Overall
{
    public static class DropdownUtil
    {

        //HERETODO: invoking the method of object. Maybe the arguments should have been rethinked.
        public static void addBulkSelectRefresh (this UnityEngine.UI.Dropdown d, List<string> options, string selectOption, Type type, string methodName) {
            d.options.Clear();
            foreach (string s in type.GetMethod(methodName).Invoke(methodName))
            {
                d.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = s });
            }
            for (int i = 0; i < this.options.Count; i++)
            {
                if (String.Equals(this.options[i].text, selectOption))
                {
                    d.value = i;
                    d.RefreshShownValue();
                    break;
                }
            }
        }
    }
}
