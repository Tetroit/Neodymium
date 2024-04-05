﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using GXPEngine.UI;
using GXPEngine.Core;
using GXPEngine.Editor;
using System.Drawing;

namespace GXPEngine.Editor
{
    public class Editor : Game
    {
        EditorCamera mainCam;
        EditorGameObject _mainGameObject;
        public EditorGameObject mainGameObject
        {
            get { return _mainGameObject; }
        }
        public EditorGameObject selectedGameobject;

        EditorUIHandler uiHandler;

        Type[] gameObjectTypes;
        ConstructorInfo[] constructors;

        public Editor() : base(1200, 600, false, true, true, "GXP Editor")
        { 
            SetupCam();
            uiHandler = new EditorUIHandler();

            uiHandler.SetupMainUI();
        }

        public void AddGameObject(object consInfo)
        {
            uiHandler.SetActiveSideMenu(null);
            if (consInfo == null || !(consInfo is ConstructorInfo)) return;
            ConstructorInfo constructorInfo = (ConstructorInfo) consInfo;

            Type gameObjectType = constructorInfo.DeclaringType;
            EditorGameObject newObject = new EditorGameObject(gameObjectType, constructorInfo);
            if (selectedGameobject != null)
            {
                selectedGameobject.AddChild(newObject);
            }
            else if (_mainGameObject == null || !_mainGameObject.InHierarchy())
            {
                _mainGameObject = newObject;
                AddChild(newObject);
            }
            else _mainGameObject.AddChild(newObject);
            selectedGameobject = newObject;
        }

        void Update()
        {
            DrawEditorGizmos();
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 start = mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 0.001f);
                Vector3 end = mainCam.ScreenPointToGlobal(Input.mouseX, Input.mouseY, 1);
                
                if(mainGameObject != null)
                {
                    raycastResult slop = raycastThroughChildren(mainGameObject, start, end);
                    selectedGameobject = slop.hitObject;
                }

            }
            uiHandler.UpdateHierarchy();
        }

        static raycastResult raycastThroughChildren(EditorGameObject toCast, Vector3 rayStart, Vector3 rayEnd)
        {
            raycastResult result = new raycastResult();
            result.hitObject = null;
            result.distance = float.MaxValue;
            Vector3 normal;
            foreach(GameObject go in toCast.GetChildren())
            {
                if(go.collider == null) continue;
             
                float point = float.MaxValue;
                bool hit = false;
                if (go.collider is BoxCollider)
                    hit = ((BoxCollider)go.collider).RayCast(rayStart, rayEnd, out point, out normal);
                else hit = go.collider.RayCastTest(rayStart, rayEnd);

                if (hit)
                    result.setIfCloser((go is EditorGameObject ? (EditorGameObject)go : toCast), point < float.MaxValue ? point : (go.TransformPoint(0, 0, 0) - rayStart).Magnitude());

                if (go is EditorGameObject) result.setIfCloser(raycastThroughChildren((EditorGameObject)go, rayStart, rayEnd));
            }
            float d = float.MaxValue;
            if(toCast.collider != null)
                ((BoxCollider)toCast.collider).RayCast(rayStart, rayEnd, out d, out normal);
            result.setIfCloser(toCast, d);
            return result;
        }
        struct raycastResult
        {
            public EditorGameObject hitObject;
            public float distance;

            public void setIfCloser(EditorGameObject hitObject, float distance)
            {
                if(this.distance > distance)
                {
                    this.distance = distance;
                    this.hitObject = hitObject;
                }
            }
            public void setIfCloser(raycastResult result)
            {
                setIfCloser(result.hitObject, result.distance);
            }
        }

        void SetupCam()
        {
            RenderMain = false;
            mainCam = new EditorCamera();
            mainCam.position = new Vector3(1, 1, 3);
            AddChild(mainCam);
        }

        void DrawEditorGizmos()
        {
            Gizmos.DrawLine(0, 0, 0, 1f, 0, 0, this, 0xFFFF0000, 5);
            Gizmos.DrawLine(0, 0, 0, 0, 1f, 0, this, 0xFF00FF00, 5);
            Gizmos.DrawLine(0, 0, 0, 0, 0, 1f, this, 0xFF0000FF, 5);
            for (int i = 0; i < 22; i++)
            {
                //dw abt it
                uint col = i == 5 || i == 16 ? 0xFFFFFFFF : 0x77FFFFFF;
                if (i < 11) Gizmos.DrawLine(-6, 0, i - 5, 6, 0, i - 5, this, col, 1);
                else Gizmos.DrawLine(i - 16, 0, -6, i - 16, 0, 6, this, col, 1);
            }
            if (selectedGameobject != null && typeof(Box).IsAssignableFrom(selectedGameobject.ObjectType))
                Gizmos.DrawBox(0, 0, 0, 2, 2, 2, selectedGameobject, 0xFFFF9900, 8);
        }

        public override void Add(GameObject gameObject)
        {
            if(gameObject.GetType().Namespace == typeof(Editor).Namespace)
            base.Add(gameObject);
        }
    }
}