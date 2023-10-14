using System.Collections.Generic;
using System.Linq;
using BepInEx;
using System.Security.Permissions;
using UnityEngine;

#pragma warning disable CS0618 //Do not remove the following line.
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace CowboyHat
{
    [BepInPlugin("locochoco.cowboyhat", "CowboyHat", "0.1.1")] // (GUID, mod name, mod version)
    public class CowboyHatMod : BaseUnityPlugin
    {
	private static CowboyHatOptions options;
	private static HatMode hatMode = HatMode.Both;
	public enum HatMode{
          Both,
	  PlayerOnly,
	  ScavOnly
	};

	private Dictionary<GraphicsModule, List<CowboyHat>> hatsOfGraphics = new Dictionary<GraphicsModule, List<CowboyHat>>();
        public void OnEnable(){
	    Logger.LogInfo("Enabled!");
	    On.GraphicsModule.InitiateSprites += GraphicsModuleOnInitiateSprites;
	    On.GraphicsModule.DrawSprites += GraphicsModuleOnDrawSprites;
	    On.PhysicalObject.DisposeGraphicsModule += PhysicalObjectOnDisposeGraphicsModule;
	    On.RainWorldGame.ctor += RainWorldGamector;
	    On.RainWorld.OnModsInit += RainWorldOnModsInit;
	    On.RainWorld.PostModsInit += RainWorldPostModsInit;
        }
	private void RainWorldGamector(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager){
	    orig(self, manager);
	    ReadSettings();
	}
	private void RainWorldPostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self){
	    orig(self);
	    ReadSettings();
	}
	private void RainWorldOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self){
	    orig(self);
	    InitOptions();
	    LoadResources();
	}
	private void InitOptions(){
	    if(options == null) options = new CowboyHatOptions();
	    MachineConnector.SetRegisteredOI("cowboyhat", options);
	}
	private void LoadResources(){
	    Futile.atlasManager.ActuallyLoadAtlasOrImage("cowboy-hat", "sprites/cowboyhat", "");
	    Logger.LogInfo("Adding Cowboy hat sprite to atlas!");
	}
	private void ReadSettings(){
	    hatMode = options.HatMode.Value;
	}
        private void GraphicsModuleOnInitiateSprites(On.GraphicsModule.orig_InitiateSprites orig, GraphicsModule self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam){
            orig(self, sLeaser, rCam);
	    Logger.LogInfo("Initting Sprites");
	    if(hatsOfGraphics.TryGetValue(self, out var hats)){
	    	Logger.LogInfo("New room!");
	        InitiateHatOnNewRoom(hats, rCam);
		return;
	    }
	    Logger.LogInfo("New hat!");
	    AddHatToGraphics(self);
        }
        private void GraphicsModuleOnDrawSprites(On.GraphicsModule.orig_DrawSprites orig, GraphicsModule self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos){
            orig(self, sLeaser, rCam, timeStacker, camPos);
	    if(hatsOfGraphics.TryGetValue(self, out var hats)){
	    	//Logger.LogInfo("Moving Sprite!"); 
	        hats.ForEach(hat => hat.MoveWithParentSprite(sLeaser));
	    }
        }
        private void PhysicalObjectOnDisposeGraphicsModule(On.PhysicalObject.orig_DisposeGraphicsModule orig, PhysicalObject self){
	    //after orig, self.graphicsModule will be disposed, so we need to remove it from the dictionary now
	    Logger.LogInfo("Disposing Sprites!");
	    if(self.graphicsModule != null) hatsOfGraphics.Remove(self.graphicsModule);
            orig(self);
        }
	private void InitiateHatOnNewRoom(List<CowboyHat> hats, RoomCamera rCam){
	    hats.ForEach(hat => rCam.room.AddObject(hat));
	}
	private void AddHatToGraphics(GraphicsModule graphics){
	    if(graphics == null)
	        return;
	    Logger.LogInfo("Adding hat to someone!");
	    List<CowboyHat> hats = new List<CowboyHat>();
	    if(hatMode != HatMode.ScavOnly && graphics is PlayerGraphics){
	    	Logger.LogInfo("Adding hat to player!");
		hats.Add(new CowboyHat(graphics, 3, 0f, 5f, false));
		hatsOfGraphics.Add(graphics, hats);
	    }
	    else if(hatMode != HatMode.PlayerOnly && graphics is ScavengerGraphics scavGraph){
	    	Logger.LogInfo("Adding hat to scav!");
		
		hats.Add(new CowboyHat(graphics, scavGraph.HeadSprite, 180f, 7f, false));
		hatsOfGraphics.Add(graphics, hats);
	    }
	}
    }
}
