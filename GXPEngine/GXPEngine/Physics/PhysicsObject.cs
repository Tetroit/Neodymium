﻿using GXPEngine.Core;
using System;
using System.Collections.Generic;


namespace GXPEngine.Physics
{
    public struct Material
    {
        public float friction;
        public float density;
        public float restitution;
        public readonly bool isSet;
        public Material(float friction, float density, float restitution)
        {
            this.isSet = true;
            this.friction = friction;
            this.density = density;
            this.restitution = restitution;
        }
    }

    public class PhysicsObject : GameObject
    {
        protected static List<PhysicsObject> collection = new List<PhysicsObject>();
        public static float gravity = -2;
        public static int substeps = 10;
        public static Material defaultMaterial = new Material
        (
            friction: 0.1f,
            density: 0.02f,
            restitution: 0.9f
        );



        public float mass {
            get { return material.density * GetVolume(); }
            set { material.density = value/ GetVolume(); }
        }
        public bool simulated;
        public Vector3 prevPos, pos, velocity;
        public Dictionary<string,Force> staticForces = new Dictionary<string, Force>();
        private List<Force> forces;
        public readonly Vector3 freeNetForce;
        public readonly Vector3 netForce;
        public Material material;
        public GameObject renderAs;

        public PhysicsObject(Vector3 pos, bool simulated = true, bool addCollider = true, bool enable = true) : base(addCollider)
        {
            renderAs = new Box("cubeTex.png");
            this.pos = pos;
            prevPos = pos;
            position = pos;
            this.simulated = simulated;
            material = defaultMaterial;
            if (simulated )
                AddForce("gravity", new Force(new Vector3(0, gravity * mass, 0)));
            if (enable)
                collection.Add(this);
        }
        public virtual void PhysicsUpdate()
        {
            if (simulated)
            {
                float freemoveTime = Time.deltaTimeS / substeps;
                int iterations = 0;
                //CalculateAcceleration();
                while (freemoveTime > 0)
                {
                    iterations++;
                    PhysicsStep(freemoveTime, ref freemoveTime);
                    bool collided = false;
                    
                    //if there is no collider, object becomes a ghost (no collision check
                    if (collider == null) return;

                    //resolving collision
                    foreach (PhysicsObject other in collection)
                    {
                        if (other == this || other == null)
                            continue;
                        Collision collision = collider.GetCollisionInfo(other.collider);
                        if (collision == null)
                            continue;
                        Vector3 relativeVelocity = velocity - other.velocity;
                        Vector3 r = collision.point - pos;
                        Vector3 pnormal = mass * (relativeVelocity);
                        if (other.simulated)
                        {
                            if (pnormal * collision.normal < 0)
                            {
                                OnCollision(collision);
                                other.OnCollision(collision);
                                Vector3 deltaP = (1 + material.restitution * other.material.restitution) * (relativeVelocity * (mass * other.mass) / (mass + other.mass));
                                Vector3 normalDeltaP = deltaP.Project(collision.normal);
                                Vector3 angularDeltaP = deltaP - normalDeltaP;
                                if (deltaP * collision.normal < 0)
                                {
                                    ApplyMomentum(-normalDeltaP);
                                    other.ApplyMomentum(normalDeltaP);
                                    DisplacePoint(r, collision.penetrationDepth * collision.normal * 0.5f);
                                }
                                float frictionP = mass * normalDeltaP.Magnitude() * material.friction * other.material.friction;
                                float angularDeltaPLen = angularDeltaP.Magnitude();
                                if (frictionP < angularDeltaPLen)
                                    ApplyMomentum(frictionP * angularDeltaP / angularDeltaPLen);
                                else
                                    ApplyMomentum(angularDeltaP);
                                //velocity -= deltaP / mass * material.restitution;
                                //other.velocity += deltaP / other.mass * material.restitution;

                            }
                        }
                        if (!other.simulated)
                        {
                            if (pnormal * collision.normal < 0)
                            {
                                OnCollision(collision);
                                Vector3 deltaP = -(1 + material.restitution * other.material.restitution) * pnormal;
                                Vector3 normalDeltaP = deltaP.Project(collision.normal);
                                Vector3 angularDeltaP = deltaP - normalDeltaP;
                                //friction force value is defined as -mu*N, where N is reaction force and mu is friction coefficient, but in order to apply it we have to convert it to momentum
                                //so P = dF/dt = mu*dN/dt = mu*normalDeltaP
                                //please keep in mind that this equation is not in the vector form, because friction force is perpendicular to the normal
                                float frictionP = normalDeltaP.Magnitude() * material.friction * other.material.friction;
                                float angularDeltaPLen = angularDeltaP.Magnitude();
                                if (frictionP < angularDeltaPLen)
                                {
                                    ApplyMomentum(frictionP * angularDeltaP / angularDeltaPLen);
                                }
                                else
                                    ApplyMomentum(angularDeltaP);
                                if (normalDeltaP * collision.normal > 0)
                                {
                                    ApplyMomentum(normalDeltaP);
                                    DisplacePoint(r, collision.penetrationDepth * collision.normal * 0.5f);
                                }
                            }
                        }
                    }

                    //PhysicsStep(freemoveTime, ref freemoveTime);

                    //if (PhysicsEngine.showGizmos)
                    //{
                    //    Gizmos.DrawArrow(pos.x + MyGame.main.width / 2, pos.y + MyGame.main.height / 2, velocity.x, velocity.y);
                    //    Gizmos.DrawArrow(pos.x + MyGame.main.width / 2, pos.y + MyGame.main.height / 2, acceleration.x / 10, acceleration.y / 10, color: 0xffffff00);
                    //}

                }
                //Console.WriteLine(iterations);
            }
        }
        public void PhysicsStep(float time, ref float totalTime)
        {
            CalculateForces();
            totalTime -= time;
            Vector3 startVel = velocity;
            Vector3 startPos = pos;
            foreach (Force f in forces)
            {
                ApplyMomentum(f.force * time);
            }
            Vector3 deltaV = velocity - startVel;
            velocity += deltaV / 2;
            pos += velocity * time;
            velocity += deltaV / 2;
            position = pos;
        }
        public void CalculateForces()
        {
            forces = new List<Force>();
            foreach (Force f in staticForces.Values)
            {
                forces.Add(f);
            }
        }
        public void AddForce(string name, Force force)
        {
            Force dst;
            if (staticForces.TryGetValue(name, out dst))
            {
                dst.force = force.force;
            }
            else
                staticForces.Add(name, force);
        }
        public float GetFE()
        {
            //Console.WriteLine("Full Energy: " + (GetKE() + GetPE()) + "\t" + "Kinetic Energy: " + GetKE() + "\t" + "Potential Energy: " + GetPE());
            return GetKE() + GetPE();
        }
        public float GetPE()
        {
            return -mass * pos.y * gravity;
        }
        public float GetKE()
        {
            float speed = velocity.Magnitude();
            return mass * speed * speed / 2f;
        }
        public void SetMass(float value)
        {
            mass = value;
            AddForce("gravity", new Force(new Vector3(0, gravity * mass, 0)));
        }
        public float GetVolume()
        {
            if (collider != null)
                return collider.GetArea();
            return 1;
        }
        public void ApplyMomentum(Vector3 momentum)
        {
            velocity += momentum / mass;
        }

        public void DisplacePoint(Vector3 r, Vector3 s)
        {
            if (r.x == 0 && r.y == 0 && r.z == 0)
            {
                pos += s;
            }
            Vector3 normal = s.Project(r);
            pos += s;
        }
        public virtual void OnCollision(Collision col)
        {
            
        }
        protected override void RenderSelf(GLContext gLContext)
        {
            renderAs.Render(gLContext);
        }
        public static void UndateAll()
        {
            foreach(PhysicsObject po in collection)
            {
                for (int i=0; i<substeps; i++)
                    po.PhysicsUpdate();
            }
        }
        public void Disable()
        {
            if (collection.Contains(this))
                collection.Remove(this);
        }
        public void Enable()
        {
            if (!collection.Contains(this))
                collection.Add(this);
        }
    }
}
