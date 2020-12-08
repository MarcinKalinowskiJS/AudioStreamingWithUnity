using Assets.Scripts.Model.Additional;
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
    public class DropdownStoreTransferProtocolExtension : Dropdown
    {
        List<TransferProtocol> dropdownObjects;

        public DropdownStoreTransferProtocolExtension ()
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

        public static void Copy(Dropdown d, ref DropdownStoreTransferProtocolExtension dsoe) {
            dsoe.template = d.template;
            dsoe.captionText = d.captionText;
            dsoe.captionImage = d.captionImage;
        }

        //Using new copy method
        public static void CopyDropdownWithRelatedComponents(GameObject goNew, GameObject goOld)
        {


            if(1==0)
            goNew.CopyComponentWithGetting(goOld.GetComponent<UnityEngine.UI.Image>(), (original, copy) =>
            {
                copy.sprite = Sprite.Instantiate<Sprite>(original.sprite);
            });



            //Dropdown //Not all
            goNew.CopyComponentWithGetting(goOld.GetComponent<UnityEngine.UI.Dropdown>(), (original, copy) =>
            {
                copy.image = Image.Instantiate<Image>(original.image)
                copy.targetGraphic = original.targetGraphic;
                copy.interactable = original.interactable;
                //copy.transition = Transition.Instantiate<Transition>(original.transition);
                copy.targetGraphic = original.targetGraphic;
            });

        }
    }
}
