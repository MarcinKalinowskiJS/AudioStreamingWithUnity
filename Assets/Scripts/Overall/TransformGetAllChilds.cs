using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Overall
{
    public static class TransformGetAllChilds
    {
        //Some objects may not have GameObject attached!
        public static List<Transform> getAllChilds(this Transform t) {
            List<Transform> childsList = new List<Transform>();
            foreach (Tuple<int, Transform> tuple in getAllChildsWithLevel(t)) {
                childsList.Add(tuple.Item2);
            }
            return childsList;
        }

        public static List<Tuple<int, Transform>> getAllChildsWithLevel(this Transform t) {
            List<Tuple<int, Transform>> childsList = new List<Tuple<int, Transform>>();
            return getAllChildsHelper(t, childsList, 0);
        }

        public static List<string> getAllChildsNames(this Transform t)
        {
            List<string> childsStringList = new List<string>();
            foreach (Tuple<int, Transform> tuple in getAllChildsWithLevel(t)) {
                childsStringList.Add(tuple.Item2.gameObject.name);
            }
            return childsStringList;
        }

        private static List<Tuple<int, Transform>> getAllChildsHelper(Transform t, List<Tuple<int, Transform>> childsList, int level)
        {
            //Middle
            childsList.Add(new Tuple<int, Transform>(level, t));

            ++level;
            //Recursive call
            for (int i = 0; i < t.childCount; i++)
            {
                getAllChildsHelper(t.GetChild(i), childsList, level);
            }

            //End
            return childsList;
        }
    }
}
