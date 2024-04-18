﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GXPEngine.Core;
using GXPEngine.Editor;

namespace GXPEngine.UI
{
    public class EditorPropertyInput : Panel
    {
        public delegate void propertyValueChanged(object newValue);
        public propertyValueChanged onValueChanged = null;
        TextPanel propertyName;
        Panel propertyValueSetter;
        object _value;
        Type valueType;

        static Texture2D transparent276x15;
        static Texture2D transparent276x150;
        public object Value
        {
            set { 
                if (value.GetType() != valueType) return;
                _value = value;
                onValueChanged?.Invoke(_value);
            }
            get { return _value; }
        }
        public EditorPropertyInput(Type property, string propertyName, object defaultValue, float x = 0, float y = 0) : base(1, 1, x, y, true)
        {
            setupTextures();
            valueType = property;
            this.propertyName = new TextPanel(110, 15, property.Name +" "+ propertyName, 10, false);
            AddChild(this.propertyName);
            if(property == typeof(string))
            {
                string defaul;
                if (defaultValue != null) defaul = defaultValue.ToString();
                else defaul = (string)TypeHandler.GetDefaultPropertyValue(typeof(string));
                _value = defaul;

                propertyValueSetter = new InputField(150, 15, 125, 0, defaul, 10);
                ((InputField)propertyValueSetter).OnTextChanged += delegate (string newtext) { Value = newtext; };

                AddChild(propertyValueSetter);
                initializeFromTexture(transparent276x15);
            }
            if(property == typeof(float))
            {
                float defaul;
                if (defaultValue != null) defaul = (float)defaultValue;
                else defaul = (float)TypeHandler.GetDefaultPropertyValue(typeof(float));
                _value = defaul;

                propertyValueSetter = new InputField(150, 15, 125, 0, defaul.ToString(), 10);
                ((InputField)propertyValueSetter).OnTextChanged += delegate (string newtext) {
                    float next;
                    if(float.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                    Value = next; 
                };

                AddChild(propertyValueSetter);
                initializeFromTexture(transparent276x15);
            }
            if (property == typeof(int))
            {
                int defaul;
                if (defaultValue != null) defaul = (int)defaultValue;
                else defaul = (int)TypeHandler.GetDefaultPropertyValue(typeof(int));
                _value = defaul;

                propertyValueSetter = new InputField(150, 15, 125, 0, defaul.ToString(), 10);
                ((InputField)propertyValueSetter).OnTextChanged += delegate (string newtext) {
                    int next;
                    if(int.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                    Value = next;
                };

                AddChild(propertyValueSetter);
                initializeFromTexture(transparent276x15);
            }
            if (property == typeof(uint))
            {
                uint defaul;
                if (defaultValue != null) defaul = (uint)defaultValue;
                else defaul = (uint)TypeHandler.GetDefaultPropertyValue(typeof(uint));
                _value = defaul;

                propertyValueSetter = new InputField(150, 15, 125, 0, defaul.ToString(), 10);
                ((InputField)propertyValueSetter).OnTextChanged += delegate (string newtext) {
                    uint next;
                    if(uint.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                    Value = next;
                };

                AddChild(propertyValueSetter);
                initializeFromTexture(transparent276x15);
            }
            if (property == typeof(bool))
            {
                bool defaul;
                if (defaultValue != null) defaul = (bool)defaultValue;
                else defaul = (bool)TypeHandler.GetDefaultPropertyValue(typeof(bool));
                _value = defaul;

                propertyValueSetter = new CheckBox(15, 15, 125, 0);
                ((CheckBox)propertyValueSetter).state = defaul;
                ((CheckBox)propertyValueSetter).OnSwitch += delegate (bool newValue) { Value = newValue; };

                AddChild(propertyValueSetter);
                initializeFromTexture(transparent276x15);
            }
            if (property == typeof(Vector3))
            {
                Vector3 defaul;
                if (defaultValue != null) defaul = (Vector3)defaultValue;
                else defaul = (Vector3)TypeHandler.GetDefaultPropertyValue(typeof(Vector3));
                _value = defaul;

                propertyValueSetter = new Panel(1, 1, invisible: true);
                InputField inX = new InputField(45, 15, 125, 0, defaul.x.ToString(), 10);
                InputField inY = new InputField(45, 15, 175, 0, defaul.y.ToString(), 10);
                InputField inZ = new InputField(45, 15, 225, 0, defaul.z.ToString(), 10);

                (inX).OnTextChanged += delegate (string newtext) {
                    float next;
                    if (float.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                        Value = new Vector3(next, ((Vector3)_value).y, ((Vector3)_value).z);
                };
                propertyValueSetter.AddChild(inX);

                inY.OnTextChanged += delegate (string newtext) {
                    float next;
                    if (float.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                        Value = new Vector3(((Vector3)_value).x, next, ((Vector3)_value).z);
                };
                propertyValueSetter.AddChild(inY);

                (inZ).OnTextChanged += delegate (string newtext) {
                    float next;
                    if (float.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                        Value = new Vector3(((Vector3)_value).x, ((Vector3)_value).y, next);
                };
                propertyValueSetter.AddChild(inZ);

                AddChild(propertyValueSetter);
                initializeFromTexture(transparent276x15);
            }
            if (property == typeof(Quaternion))
            {
                Quaternion defaul;
                if (defaultValue != null) defaul = (Quaternion)defaultValue;
                else defaul = (Quaternion)TypeHandler.GetDefaultPropertyValue(typeof(Quaternion));
                _value = defaul;

                propertyValueSetter = new Panel(1, 1, invisible: true);
                InputField inR = new InputField(32, 15, 125, 0, defaul.r.ToString(), 10);
                InputField inI = new InputField(32, 15, 162.5f, 0, defaul.i.ToString(), 10);
                InputField inJ = new InputField(32, 15, 200, 0, defaul.j.ToString(), 10);
                InputField inK = new InputField(32, 15, 237.5f, 0, defaul.k.ToString(), 10);

                (inR).OnTextChanged += delegate (string newtext) {
                    float next;
                    if (float.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                        Value = new Quaternion(next, ((Quaternion)_value).i, ((Quaternion)_value).j, ((Quaternion)_value).k);
                };
                propertyValueSetter.AddChild(inR);

                inI.OnTextChanged += delegate (string newtext) {
                    float next;
                    if (float.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                        Value = new Quaternion(((Quaternion)_value).r, next, ((Quaternion)_value).j, ((Quaternion)_value).k);
                };
                propertyValueSetter.AddChild(inI);

                (inJ).OnTextChanged += delegate (string newtext) {
                    float next;
                    if (float.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                        Value = new Quaternion(((Quaternion)_value).r, ((Quaternion)_value).i, next, ((Quaternion)_value).k);
                };
                propertyValueSetter.AddChild(inJ);

                (inK).OnTextChanged += delegate (string newtext) {
                    float next;
                    if (float.TryParse(newtext, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out next))
                        Value = new Quaternion(((Quaternion)_value).r, ((Quaternion)_value).i, ((Quaternion)_value).j, next);
                };
                propertyValueSetter.AddChild(inK);

                AddChild(propertyValueSetter);
                initializeFromTexture(transparent276x15);
            }
            if(property == typeof(Texture2D))
            {
                Texture2D defaul;
                if (defaultValue != null) defaul = (Texture2D)defaultValue;
                else defaul = (Texture2D)TypeHandler.GetDefaultPropertyValue(typeof(Texture2D));
                _value = defaul;

                propertyValueSetter = new Button(150, 150, 125);
                Sprite texrenderer = new Sprite(defaul, false);
                texrenderer.width = 144;
                texrenderer.height = 144;
                texrenderer.position = new Vector3(3, 3, 0);
                propertyValueSetter.AddChild(texrenderer);
                ((Button)propertyValueSetter).OnRelease += delegate
                {
                    string filepath = "";
                    //quite remarkable
                    Thread STAThread = new Thread(
                        delegate ()
                        {
                            using (OpenFileDialog ofd = new OpenFileDialog())
                            {
                                ofd.InitialDirectory = "";
                                ofd.Filter = "Image Files (*.PNG;*.JPG)|*.PNG;*.JPG";
                                ofd.FilterIndex = 1;
                                ofd.Multiselect = false;
                                ofd.RestoreDirectory = true;
                                ofd.Title = "Select an image texture...";

                                if (ofd.ShowDialog() != DialogResult.OK) return;
                                try { filepath = ofd.FileName.Substring(Directory.GetCurrentDirectory().Length + 1).Replace('\\', '/'); } catch { }
                            }
                        });
                    STAThread.SetApartmentState(ApartmentState.STA);
                    STAThread.Start();
                    STAThread.Join();

                    try { Value = Texture2D.GetInstance(filepath); } catch { }
                };

                AddChild(propertyValueSetter);
                initializeFromTexture(transparent276x150);
            }
        }
        void setupTextures()
        {
            if (transparent276x15 != null) return;
            transparent276x15 = new Panel(276, 15, invisible: true).texture.Clone();
            transparent276x150 = new Panel(276, 150, invisible:true).texture.Clone();
        }
    }
}
