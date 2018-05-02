using System;
using System.Collections.Generic;
using System.Diagnostics;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Urho.Resources;
using Urho.Shapes;

namespace IdleClicker
{
    public class MyGame : Application
    {
        Node cameraNode;
        Node earthNode;
        Node rootNode;
        Scene scene;
        Camera MainCamera;
        float yaw, pitch;
        Button goldButton;
        int gold = 0;
        Window goldButtonWindow;
        Text goldText;

        IdlePlayerManager GameManager { get { return IdlePlayerManager.Instance; } }

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

            XmlFile style = cache.GetXmlFile("UI/DefaultStyle.xml");

            // Set the loaded style as default style
            UI.Root.SetDefaultStyle(style);

            Input.SetMouseMode(MouseMode.Free);
            Input.SetMouseVisible(true);

            // 3D scene with Octree
            scene = new Scene(Context);
            scene.CreateComponent<Octree>();

            rootNode = scene.CreateChild();
            
            rootNode.Position = new Vector3(0, 0, 20);
            UI.UIMouseClick += UI_UIMouseClick;

            //GUI

            goldButtonWindow = new Window();
            goldButtonWindow.SetLayout(LayoutMode.Vertical, 6, new IntRect(6, 6, 6, 6));
            goldButtonWindow.SetMinSize(300, 300);
            goldButtonWindow.Name = "GoldWindow";
            goldButtonWindow.SetStyleAuto();
            UI.Root.AddChild(goldButtonWindow);

            goldText = new Text(Context);
            goldText.Value = string.Format(Assets.FormatStrings.Gold, gold);
            goldText.HorizontalAlignment = HorizontalAlignment.Right;
            goldText.VerticalAlignment = VerticalAlignment.Top;
            goldText.SetColor(new Color(r: 1.0f, g: 1.0f, b: 0.0f));
            goldText.SetFont(font: CoreAssets.Fonts.AnonymousPro, size: 30);
            UI.Root.AddChild(goldText);

            goldButton = new Button();
            goldButton.Name = "Gold Button";
            goldButton.MinHeight = 40;
            goldButton.SetStyleAuto();
            goldButton.Pressed += GoldButton_Pressed;
            goldButtonWindow.AddChild(goldButton);
            goldButtonWindow.Visible = true;
            //END GUI
            

            var tile = rootNode.CreateChild();
            tile.SetScale(3f);
            //tile.Scale = new Vector3(40f, 0.0001f, 40f);
            tile.Rotation = new Quaternion(0, 0, 0);
            //var tileModel = tile.CreateComponent<StaticModel>();
            var tileModel = tile.CreateComponent<Urho.Shapes.Box>();
            //tileModel.Model = cache.GetModel(Assets.Models.Plane);
            tileModel.SetMaterial(cache.GetMaterial(Assets.Materials.Grass));
            var tileBuilding = tile.CreateComponent<BuildingTile>();

            // Light
            Node lightNode = scene.CreateChild();
            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Directional;
            light.Range = 20;
            light.Brightness = 1f;
            lightNode.SetDirection(new Vector3(0, -0.25f, .0f));

            // Camera
            cameraNode = scene.CreateChild();
            cameraNode.Position = (new Vector3(0.0f, 10, 10.0f));
            cameraNode.Rotation = Quaternion.FromRotationTo(cameraNode.Position, rootNode.Position);
            MainCamera = cameraNode.CreateComponent<Camera>();
            MainCamera.Orthographic = false;

            // Viewport
            var viewport = new Viewport(Context, scene, MainCamera, null);
            Renderer.SetViewport(0, viewport);
            //viewport.RenderPath.Append(CoreAssets.PostProcess.FXAA2);

            Input.Enabled = true;
            // FPS
            //new MonoDebugHud(this).Show(Color.Green, 25);

            // Stars (Skybox)
            var skyboxNode = scene.CreateChild();
            var skybox = skyboxNode.CreateComponent<Skybox>();
            skybox.Model = CoreAssets.Models.Box;
            skybox.SetMaterial(Material.SkyboxFromImage("Textures/Space.png"));

            // Run a an action to spin the Earth (7 degrees per second)
            //tile.RunActions((new RotateBy(duration: 1f, deltaAngleX: , deltaAngleY: 45, deltaAngleZ: 0)));
            // Spin clouds:
            //cloudsNode.RunActions(new RepeatForever(new RotateBy(duration: 1f, deltaAngleX: 0, deltaAngleY: 1, deltaAngleZ: 0)));
            // Zoom effect:
            //await rootNode.RunActionsAsync(new EaseOut(new MoveTo(12f, new Vector3(0, 0, 12)), 0.1f));

            //AddCity(0, 0, "(0, 0)");
            //AddCity(53.9045f, 27.5615f, "Minsk");
            //AddCity(51.5074f, 0.1278f, "London");
            //AddCity(40.7128f, -74.0059f, "New-York");
            //AddCity(37.7749f, -122.4194f, "San Francisco");
            //AddCity(39.9042f, 116.4074f, "Beijing");
            //AddCity(-31.9505f, 115.8605f, "Perth");

        }

        private void GoldButton_Pressed(PressedEventArgs obj)
        {
            GameManager.AddResourceValue(IdlePlayerResourceType.Gold, 1);
        }

        private void UI_UIMouseClick(UIMouseClickEventArgs obj)
        {
            Debug.WriteLine("Clicked on UI");
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
            UpdateUI();
            MoveCameraByTouches(timeStep);
            SimpleMoveCamera3D(timeStep);
            base.OnUpdate(timeStep);
        }

        private void UpdateUI()
        {
            goldText.Value = string.Format(Assets.FormatStrings.Gold, GameManager.GetResourceValue(IdlePlayerResourceType.Gold));
        }

        bool InputRaycastCollided(IntVector2 position, out RayQueryResult? raycastResult)
        {
            Vector2 normScreenPos = Helpers.GetNormalizedScreenPosition(Graphics, position);

            Ray ray = MainCamera.GetScreenRay(normScreenPos);
            raycastResult = scene.GetComponent<Octree>().RaycastSingle(ray, maxDistance: 100);

            return raycastResult.HasValue;
        }

        void InterpretRaycastResult(RayQueryResult raycastResult)
        {
            BuildingTile buildingTile = raycastResult.Node.GetComponent<BuildingTile>();
            if(buildingTile != null)
            {
                buildingTile.Building = new Building();
            }
        }

        /// <summary>
        /// Move camera for 3D samples
        /// </summary>
        protected void SimpleMoveCamera3D(float timeStep, float moveSpeed = 10.0f)
        {
            RayQueryResult? raycastResult = null;

            if (Input.GetMouseButtonPress(MouseButton.Left) && InputRaycastCollided(Input.MousePosition, out raycastResult))
            {
                InterpretRaycastResult(raycastResult.Value);
            }
            else if(Input.GetMouseButtonDown(MouseButton.Left))
            {
                //const float mouseSensitivity = .1f;
                //var mouseMove = Input.MouseMove;
                //yaw += mouseSensitivity * mouseMove.X;
                //pitch += mouseSensitivity * mouseMove.Y;
                //pitch = MathHelper.Clamp(pitch, -90, 90);
                //cameraNode.Rotation = new Quaternion(pitch, yaw, 0);
            }

        }

        protected void MoveCameraByTouches(float timeStep)
        {
            const float touchSensitivity = 2f;

            var input = Input;

            for (uint i = 0, num = input.NumTouches; i < num; ++i)
            {
                TouchState state = input.GetTouch(i);

                RayQueryResult? raycastResult = null;

                if (InputRaycastCollided(state.Position, out raycastResult))
                {
                    InterpretRaycastResult(raycastResult.Value);
                }
                else
                {
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
}
