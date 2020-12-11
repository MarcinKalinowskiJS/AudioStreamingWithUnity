using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Overall
{
    public static class DropdownUtil
    {
        public static void addBulkSelectRefresh (this UnityEngine.UI.Dropdown d, string selectOption, object objectToGetList, string methodName) {
            d.options.Clear();
            List<string> stringList = (List<string>) objectToGetList.GetType().GetMethod(methodName).Invoke(objectToGetList, null);
            foreach (string s in stringList)
            {
                d.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = s });
            }
            for (int i = 0; i < d.options.Count; i++)
            {
                if (String.Equals(d.options[i].text, selectOption))
                {
                    d.value = i;
                    d.RefreshShownValue();
                    break;
                }
            }
        }
    }
}
