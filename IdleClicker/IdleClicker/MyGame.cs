﻿using System;
using System.Diagnostics;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Urho.Shapes;

namespace IdleClicker
{
    public class MyGame : Application
    {
        Node cameraNode;
        Node earthNode;
        Node rootNode;
        Scene scene;
        float yaw, pitch;

        [Preserve]
        public MyGame(ApplicationOptions options) : base(options) { }

        static MyGame()
        {
            UnhandledException += (s, e) =>
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                e.Handled = true;
            };
        }

        protected override async void Start()
        {
            base.Start();

            var cache = ResourceCache;

            // UI text 
            var helloText = new Text(Context);
            helloText.Value = "Hello World from UrhoSharp";
            helloText.HorizontalAlignment = HorizontalAlignment.Center;
            helloText.VerticalAlignment = VerticalAlignment.Top;
            helloText.SetColor(new Color(r: 0.5f, g: 1f, b: 1f));
            helloText.SetFont(font: CoreAssets.Fonts.AnonymousPro, size: 30);
            UI.Root.AddChild(helloText);

            // 3D scene with Octree
            scene = new Scene(Context);
            scene.CreateComponent<Octree>();

            rootNode = scene.CreateChild();
            rootNode.Position = new Vector3(0, 0, 20);

            var tile = rootNode.CreateChild();
            tile.SetScale(3f);
            //tile.Scale = new Vector3(40f, 0.0001f, 40f);
            tile.Rotation = new Quaternion(-90, 0, 0);
            //var tileModel = tile.CreateComponent<StaticModel>();
            var tileModel = tile.CreateComponent<Urho.Shapes.Plane>();
            //tileModel.Model = cache.GetModel(Assets.Models.Plane);
            tileModel.SetMaterial(cache.GetMaterial(Assets.Materials.Grass));

            // Light
            Node lightNode = scene.CreateChild();
            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Directional;
            light.Range = 20;
            light.Brightness = 1f;
            lightNode.SetDirection(new Vector3(1f, -0.25f, 1.0f));

            // Camera
            cameraNode = scene.CreateChild();
            cameraNode.Position = (new Vector3(0.0f, 0.0f, -10.0f));
            var camera = cameraNode.CreateComponent<Camera>();
            camera.Orthographic = true;

            // Viewport
            var viewport = new Viewport(Context, scene, camera, null);
            Renderer.SetViewport(0, viewport);
            //viewport.RenderPath.Append(CoreAssets.PostProcess.FXAA2);

            Input.Enabled = true;
            // FPS
            new MonoDebugHud(this).Show(Color.Green, 25);

            // Stars (Skybox)
            var skyboxNode = scene.CreateChild();
            var skybox = skyboxNode.CreateComponent<Skybox>();
            skybox.Model = CoreAssets.Models.Box;
            skybox.SetMaterial(Material.SkyboxFromImage("Textures/Space.png"));

            // Run a an action to spin the Earth (7 degrees per second)
            rootNode.RunActions(new RepeatForever(new RotateBy(duration: 1f, deltaAngleX: 0, deltaAngleY: 0, deltaAngleZ: -30)));
            // Spin clouds:
            //cloudsNode.RunActions(new RepeatForever(new RotateBy(duration: 1f, deltaAngleX: 0, deltaAngleY: 1, deltaAngleZ: 0)));
            // Zoom effect:
            await rootNode.RunActionsAsync(new EaseOut(new MoveTo(12f, new Vector3(0, 0, 12)), 0.1f));

            //AddCity(0, 0, "(0, 0)");
            //AddCity(53.9045f, 27.5615f, "Minsk");
            //AddCity(51.5074f, 0.1278f, "London");
            //AddCity(40.7128f, -74.0059f, "New-York");
            //AddCity(37.7749f, -122.4194f, "San Francisco");
            //AddCity(39.9042f, 116.4074f, "Beijing");
            //AddCity(-31.9505f, 115.8605f, "Perth");

        }
        public void AddCity(float lat, float lon, string name)
        {
            var height = earthNode.Scale.Y / 2f;

            lat = (float)Math.PI * lat / 180f - (float)Math.PI / 2f;
            lon = (float)Math.PI * lon / 180f;

            float x = height * (float)Math.Sin(lat) * (float)Math.Cos(lon);
            float z = height * (float)Math.Sin(lat) * (float)Math.Sin(lon);
            float y = height * (float)Math.Cos(lat);

            var markerNode = rootNode.CreateChild();
            markerNode.Scale = Vector3.One * 0.1f;
            markerNode.Position = new Vector3((float)x, (float)y, (float)z);
            markerNode.CreateComponent<Sphere>();
            markerNode.RunActionsAsync(new RepeatForever(
                new TintTo(0.5f, Color.White),
                new TintTo(0.5f, Randoms.NextColor())));

            var textPos = markerNode.Position;
            textPos.Normalize();

            var textNode = markerNode.CreateChild();
            textNode.Position = textPos * 2;
            textNode.SetScale(3f);
            textNode.LookAt(Vector3.Zero, Vector3.Up, TransformSpace.Parent);
            var text = textNode.CreateComponent<Text3D>();
            text.SetFont(CoreAssets.Fonts.AnonymousPro, 150);
            text.EffectColor = Color.Black;
            text.TextEffect = TextEffect.Shadow;
            text.Text = name;
        }

        protected override void OnUpdate(float timeStep)
        {
            MoveCameraByTouches(timeStep);
            SimpleMoveCamera3D(timeStep);
            base.OnUpdate(timeStep);
        }

        /// <summary>
        /// Move camera for 3D samples
        /// </summary>
        protected void SimpleMoveCamera3D(float timeStep, float moveSpeed = 10.0f)
        {
            if (!Input.GetMouseButtonDown(MouseButton.Left))
                return;

            const float mouseSensitivity = .1f;
            var mouseMove = Input.MouseMove;
            yaw += mouseSensitivity * mouseMove.X;
            pitch += mouseSensitivity * mouseMove.Y;
            pitch = MathHelper.Clamp(pitch, -90, 90);
            cameraNode.Rotation = new Quaternion(pitch, yaw, 0);
        }

        protected void MoveCameraByTouches(float timeStep)
        {
            const float touchSensitivity = 2f;

            var input = Input;
            for (uint i = 0, num = input.NumTouches; i < num; ++i)
            {
                TouchState state = input.GetTouch(i);
                if (state.Delta.X != 0 || state.Delta.Y != 0)
                {
                    var camera = cameraNode.GetComponent<Camera>();
                    if (camera == null)
                        return;

                    yaw += touchSensitivity * camera.Fov / Graphics.Height * state.Delta.X;
                    pitch += touchSensitivity * camera.Fov / Graphics.Height * state.Delta.Y;
                    cameraNode.Rotation = new Quaternion(pitch, yaw, 0);
                }
            }
        }
    }
}
