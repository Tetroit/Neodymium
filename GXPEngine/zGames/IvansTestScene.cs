﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.AddOns;
using GXPEngine.Core;
using static System.Net.Mime.MediaTypeNames;

namespace GXPEngine
{
    internal class IvansTestScene : Game
    {
        bool showCursor;
        Player player;
        Camera cam;

        ModelRenderer walls;
        Box box;
        public IvansTestScene() : base(800, 600, false, true, false, "actual cool test scene")
        {
            cam = new Camera(new ProjectionMatrix(90, 90 * .75f, .1f, 10), true);
            RenderMain = false;
            AddChild(cam);
            cam.SetXY(0, 1, 0);

            player = new Player();

            SetupScene();

            AddChild(player);
            player.AssignCamera(cam);
            Gizmos.GetCameraSpace(cam);
            box = new Box("cubeTex.png");
            player.colliders.Add(box.collider);
            ((BoxCollider)box.collider).size = new Vector3(0.5f, 0.5f, 0.5f);
            AddChild(box);
            box.DisplayExtents();
            Lighting.SetLight(0, new Vector3(5, 5, 5), new Vector3(.4f, .2f, .2f), new Vector3(.0f, .2f, .7f));
            Lighting.SetLight(1, new Vector3(-5, -5, -0), new Vector3(.0f, .0f, .0f), new Vector3(.5f, .2f, .0f));
        }
        void Update()
        {
            if (Input.GetKeyDown(Key.TAB)) showCursor = !showCursor;
            game.ShowMouse(showCursor);
            ((BoxCollider)walls.collider).DrawExtents();
            ((BoxCollider)box.collider).DrawExtents();
        }
        public void SetupScene()
        {
            for (int i = GetChildren().Count - 1; i >= 0; i--)
            {
                GameObject child = GetChildren()[i];
                if (!(child is Camera))
                    RemoveChild(child);
            }
            string sceneFolder = "testScene/";

            ModelRenderer floor = new ModelRenderer(sceneFolder + "floor.obj", sceneFolder + "floor_texture.png");
            AddChild(floor);
            floor.y = -2;
            walls = new ModelRenderer(sceneFolder + "walls.obj", sceneFolder + "walls_texture.png");
            AddChild(walls);
            walls.y = -2;
            BoxCollider temp = walls.CreateBoxCollider();
            temp.size = new Vector3(0.3f, 0.3f, 0.3f);
            temp.offset = new Vector3(0.5f, 0.5f, 0.5f);
            player.colliders.Add(walls.collider);
            ModelRenderer lights = new ModelRenderer(sceneFolder + "lights.obj", "editor/whitePixel.png");
            AddChild(lights);
            lights.y = -2;
            ModelRenderer ceiling = new ModelRenderer(sceneFolder + "ceiling.obj", sceneFolder + "ceiling_texture.png");
            AddChild(ceiling);
            ceiling.y = -2;
            ModelRenderer tube1 = new ModelRenderer(sceneFolder + "tube1.obj", sceneFolder + "tube1_texture.png");
            AddChild(tube1);
            tube1.y = -2;
            ParticleSystem ps = new ParticleSystem(sceneFolder + "smoke.png", 1, 1, 1, ParticleSystem.EmitterType.rect, ParticleSystem.Mode.velocity, worldSpace: this);
            ps.startPos = new Vector3(0.265f, 1.44f - 2f, 2.2f);
            ps.startSpeedDelta = new Vector3(0.001f, 0.001f, 0.001f);
            ps.startSpeed = new Vector3(0.001f, -0.01f, -0.01f);
            ps.endSpeed = Vector3.zero;
            ps.endSpeedDelta = new Vector3(0, 0, 0);
            ps.startSize = 0.0005f;
            ps.endSize = 0.001f;
            ps.startAlpha = 1f;
            ps.endAlpha = 0f;
            ps.startColor = Color.Gray;
            ps.endColor = Color.White;
            ps.enabled = true;
            AddChild(ps);
        }
    }
}
