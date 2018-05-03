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
            //Input.MouseMoved += Input_MouseMoved;

            // 3D scene with Octree
            scene = new Scene(Context);
            scene.CreateComponent<Octree>();

            rootNode = scene.CreateChild();
            
            rootNode.Position = new Vector3(0, 0, 20);
            //UI.UIMouseClick += UI_UIMouseClick;

            //GUI

            goldButtonWindow = new Window();
            goldButtonWindow.SetLayout(LayoutMode.Free, 6, new IntRect(6, 6, 6, 6));

            //var listView = goldButtonWindow.CreateListView();
            //listView.SetMinAnchor(0, 0);
            //listView.SetMaxAnchor(1, 1);
            //listView.SetColor(Color.Magenta);
            //listView.EnableAnchor = true;

            goldButtonWindow.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            goldButtonWindow.SetMinSize(300, 300);
            goldButtonWindow.SetMinAnchor(1, 0);
            goldButtonWindow.SetMaxAnchor(1, 1);
            goldButtonWindow.EnableAnchor = true;
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

            for (int i = 0; i < 7; i++)
            {
                goldButton = new Button();
                goldButton.Name = "Gold Button";
                var text = goldButton.CreateText();
                text.Value = "B " + i;
                text.SetColor(new Color(r: 1.0f, g: 1.0f, b: 0.0f));
                text.SetFont(font: CoreAssets.Fonts.AnonymousPro, size: 20);
                text.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);

                goldButton.SetSize(80, 80);
                goldButton.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
                //goldButton.SetLayout(LayoutMode.Horizontal, 5, new IntRect(5, 5, 5, 5));
                goldButton.SetStyleAuto();
                goldButton.Pressed += GoldButton_Pressed;

                goldButtonWindow.AddChild(goldButton);
                //listView.AddItem(goldButton);
            }
            goldButtonWindow.Visible = true;
            //END GUI
            
            var tileManager = rootNode.CreateChild();
            tileManager.SetScale(3f);
            var tileComp = tileManager.CreateComponent<TileManager>();

            // Light
            Node lightNode = scene.CreateChild();
            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Directional;
            light.Range = 20;
            light.Brightness = 1f;
            lightNode.SetDirection(new Vector3(0, -0.25f, .0f));

            // Camera
            cameraNode = scene.CreateChild();
            cameraNode.Position = new Vector3(0f, 10f, 10.0f);
            // cameraNode.Rotation = Quaternion.FromRotationTo(cameraNode.Position, rootNode.Position);
            cameraNode.Rotation = new Quaternion(30f, -30f, 0f);
            MainCamera = cameraNode.CreateComponent<Camera>();
            MainCamera.Orthographic = true;

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

        private void Input_MouseMoved(MouseMovedEventArgs obj)
        {
            
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
            if (Input.GetKeyPress(Key.Esc))
            {
                Exit();
                return;
            }

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

        BuildingTile InterpretRaycastResult(RayQueryResult raycastResult)
        {
            BuildingTile buildingTile = raycastResult.Node.GetComponent<BuildingTile>();
            if (buildingTile == null)
            {
                buildingTile = raycastResult.Node.Parent.GetComponent<BuildingTile>();
            }

            return buildingTile;
        }

        BuildingTile m_lastSelectedTile;

        /// <summary>
        /// Move camera for 3D samples
        /// </summary>
        protected void SimpleMoveCamera3D(float timeStep, float moveSpeed = 10.0f)
        {
            RayQueryResult? raycastResult = null;

            if (Input.MouseMove.LengthSquared > 0 && InputRaycastCollided(Input.MousePosition, out raycastResult))
            {
                var currentTile = InterpretRaycastResult(raycastResult.Value);
                if (currentTile != m_lastSelectedTile)
                {
                    if (currentTile != null)
                    {
                        currentTile.Selected = true;
                        Debug.WriteLine("Selected: " + currentTile.Node.Name);
                    }

                    if (m_lastSelectedTile != null)
                        m_lastSelectedTile.Selected = false;

                    m_lastSelectedTile = currentTile;
                }
            }

            if (Input.GetMouseButtonPress(MouseButton.Left) && InputRaycastCollided(Input.MousePosition, out raycastResult))
            {
                var currentTile = InterpretRaycastResult(raycastResult.Value);
                currentTile.AddBuilding(null /*new Building()*/); // TODO
            }
            
            if (Input.GetMouseButtonDown(MouseButton.Right))
            {
                // TODO: screen to world sync move

                const float mouseSensitivity = .01f;
                var mouseMove = Input.MouseMove;
                float cameraX = -mouseSensitivity * mouseMove.X;
                float cameraY = mouseSensitivity * mouseMove.Y;

                cameraNode.Position = cameraNode.LocalToWorld(new Vector3(cameraX, cameraY, 0f));

                Debug.WriteLine("Camera " + cameraY + " " + cameraX);
            }

            if (Input.MouseMoveWheel != 0)
            {
                MainCamera.OrthoSize += Input.MouseMoveWheel;
            }
        }

        protected void MoveCameraByTouches(float timeStep)
        {
            const float touchSensitivity = 0.2f;

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
                        var mouseMove = Input.MouseMove;
                        float cameraX = -touchSensitivity * camera.Fov / Graphics.Height * state.Delta.X;
                        float cameraY = touchSensitivity * camera.Fov / Graphics.Height * state.Delta.Y;

                        cameraNode.Position = cameraNode.LocalToWorld(new Vector3(cameraX, cameraY, 0f));
                    }
                }
            }
        }
    }
}
