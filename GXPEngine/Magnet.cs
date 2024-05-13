﻿using GXPEngine.Core;
using GXPEngine.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GXPEngine
{
    public class Magnet : PhysicsMesh
    {
        //how to get a gf, 1 step tutorial:
        public bool isAttracting;
        private List<PhysicsObject> toAttract = new List<PhysicsObject>();
        private PhysicsObject attached;
        public bool picked = false;
        public float strength = 5f;
        public Vector3 grabOffset = new Vector3(0, 0, 5);

        public Magnet(string modelFilename, string textureFilename, Vector3 pos, bool simulated = true) : base(modelFilename, textureFilename, pos, simulated)
        {
        }
        public override void OnCollision(Collision col)
        {
            if (!isAttracting) return;
            base.OnCollision(col);
            PhysicsObject toPick = (PhysicsObject)((col.self is Magnet) ? col.other : col.self);
            if (toPick.simulated && toAttract.Contains(toPick))
            {
                Glue(toPick);
                attached = toPick;
                picked = true;
                toPick.pos = grabOffset;
            }
        }
        public void Update()
        {
            if (Input.GetKeyDown (Key.T))
            {
                isAttracting = !isAttracting;
                if (picked)
                {
                    picked = false;
                    Unglue(attached);
                }
            }
            if (isAttracting) (renderAs as ModelRenderer).color = 0xffffff;
            else (renderAs as ModelRenderer).color = 0x555555;

            if (!picked && isAttracting)
            {
                //looking for a new victim
                foreach (PhysicsObject obj in toAttract)
                {
                    Vector3 r = TransformPoint(Vector3.zero) - obj.TransformPoint(Vector3.zero);
                    float rl = r.Magnitude();
                    obj.AddForce("magnet", new Force(strength / rl / rl / rl * r));
                }
            }
            else
            {
                foreach (PhysicsObject obj in toAttract)
                    obj.AddForce("magnet", new Force(Vector3.zero));
            }
        }
        public void AddAttract(PhysicsObject po)
        {
            if (!toAttract.Contains(po))
                toAttract.Add(po);
        }   
        public void RemoveAttract(PhysicsObject po) 
        {
            if (toAttract.Contains(po))
                toAttract.Add(po);
        }
        public void DetectAttractable()
        {
            foreach (PhysicsObject box in collection)
            {
                if (box is MetalBox)
                {
                    AddAttract(box);
                }
            }
        }
    }
}
