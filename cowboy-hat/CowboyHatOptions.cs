using System.Linq;
using BepInEx;
using UnityEngine;
using RWCustom;
using Menu.Remix.MixedUI;

namespace CowboyHat
{
    class CowboyHatOptions : OptionInterface
    {
	public readonly Configurable<CowboyHatMod.HatMode> HatMode;
	private UIelement[] options_on_tab;

        public CowboyHatOptions(){
	    HatMode = config.Bind("HatMode", CowboyHatMod.HatMode.ScavOnly);
        }

	public override void Initialize(){
	    OpTab options_tab = new OpTab(this, "Options"); 
	    options_on_tab = new UIelement[3]{
	      new OpLabel(10f, 550f, "Configurations", bigText: true),
	      new OpLabel(10f, 490f, "Change who can have the cowboy hat"),
	      new OpComboBox(HatMode, new Vector2(10f, 460f), 200f, OpResourceSelector.GetEnumNames(null, typeof(CowboyHatMod.HatMode)).ToList()),
	    };
	    options_tab.AddItems(options_on_tab);

	    Tabs = new OpTab[1] { options_tab };
	}
    }
}
