using System.Linq;
using BepInEx;
using System.Security.Permissions;
using UnityEngine;
using RWCustom;

namespace CowboyHat
{
    class CowboyHat : CosmeticSprite
    {
	private GraphicsModule gm;
	private int anchorSprite;
	private float rotation;
	private float headRadius;
	private float heightOffset = 5f;
	private bool flip;

	private float parentRotation;
	private bool parentFlipX;
	private bool parentFlipY;

        public CowboyHat(GraphicsModule graphicsModule, int anchorSprite, float rotation, float headRadius, bool flip){
            this.gm = graphicsModule;
            this.anchorSprite = anchorSprite;
            this.rotation = rotation;
            this.headRadius = headRadius;
            this.flip = flip;
	    gm.owner.room.AddObject(this);
        }

        public override void Update(bool eu){
            if (gm.owner.slatedForDeletetion){
                Destroy();
            }

            base.Update(eu);
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam){
            // set label background properties (sans color)
            sLeaser.sprites = new FSprite[1];
	    sLeaser.sprites[0] = new FSprite("cowboy-hat");
            AddToContainer(sLeaser, rCam, null);
        }

	public void MoveWithParentSprite(RoomCamera.SpriteLeaser parentSLeaser){
	    if(parentSLeaser.sprites.Length <= anchorSprite) return;
	    var parentSprite = parentSLeaser.sprites[anchorSprite];
	    pos.Set(parentSprite.x, parentSprite.y);
	    parentRotation = parentSprite.rotation;
	    parentFlipY = parentSprite.scaleY < 0f;
	    parentFlipX = parentSprite.scaleX >= 0f;
	}

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker,
            Vector2 camPos){
            if (gm is null) return;
	    float totalRot = rotation + parentRotation;
	    float totalRotRadians = -totalRot * Mathf.PI / 180f;
	    Vector2 headDirection = new Vector2(Mathf.Cos(totalRotRadians), Mathf.Sin(totalRotRadians));
	    Vector2 headNormal = Custom.PerpendicularVector(headDirection);

            sLeaser.sprites[0].SetPosition(pos + (headRadius + heightOffset)*headNormal);
	    sLeaser.sprites[0].rotation = totalRot;
	    sLeaser.sprites[0].scaleY = (flip ^ parentFlipY ? -1f : 1f);
	    sLeaser.sprites[0].scaleX = (flip ^ parentFlipX ? -1f : 1f);
	    
	    if (gm.culled && !gm.lastCulled) sLeaser.sprites[0].isVisible = !gm.culled;

            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner){
            base.AddToContainer(sLeaser, rCam, null);
            sLeaser.sprites[0].RemoveFromContainer();
            rCam.ReturnFContainer("Items").AddChild(sLeaser.sprites[0]);
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette){
            // set colour of label background
            Color color = Color.white;
            color.a = 1f;
            sLeaser.sprites[0].color = color;
        }
    }
}
