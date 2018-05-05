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
        ListView BuildingsList;

        public UIElement BuildingWindow { get; private set; }

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

            XmlFile style = cache.GetXmlFile("UI/CustomDefaultStyle.xml");

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
// 
//             var m_Geometry = rootNode.CreateChild();
//             var model = m_Geometry.CreateComponent<StaticModel>();
//             model.Model = ResourceCache.GetModel(IdleClicker.Assets.Models.House);
//             model.SetMaterial(ResourceCache.GetMaterial(IdleClicker.Assets.Materials.House));
//             m_Geometry.SetScale(0.1f);

            //UI.UIMouseClick += UI_UIMouseClick;

            //GUI

            goldButtonWindow = new Window();
            goldButtonWindow.SetLayout(LayoutMode.Vertical, 6, new IntRect(6, 6, 6, 6));

            //var listView = goldButtonWindow.CreateListView();
            //listView.SetMinAnchor(0, 0);
            //listView.SetMaxAnchor(1, 1);
            //listView.SetColor(Color.Magenta);
            //listView.EnableAnchor = true;

            goldButtonWindow.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            goldButtonWindow.SetMinSize(300, 300);
            goldButtonWindow.SetMinAnchor(1, 0);
            goldButtonWindow.SetMaxAnchor(1, 1);
            goldButtonWindow.EnableAnchor = true;
            goldButtonWindow.Name = "GoldWindow";
            goldButtonWindow.SetStyleAuto();
            UI.Root.AddChild(goldButtonWindow);

            goldText = new Text(Context);
            goldText.Value = string.Format(Assets.FormatStrings.Gold, gold);
            goldText.HorizontalAlignment = HorizontalAlignment.Center;
            goldText.VerticalAlignment = VerticalAlignment.Top;
            goldText.SetColor(new Color(r: 1.0f, g: 1.0f, b: 0.0f));
            goldText.SetFont(CoreAssets.Fonts.AnonymousPro, 30);
            UI.Root.AddChild(goldText);
            UI.Root.HoverBegin += Root_HoverBegin;
            UI.Root.HoverEnd += Root_HoverEnd;
//             for (int i = 0; i < 7; i++)
//             {
//                 goldButton = new Button();
//                 goldButton.Name = "Gold Button";
//                 var text = goldButton.CreateText();
//                 text.Value = "B " + i;
//                 text.SetColor(new Color(r: 1.0f, g: 1.0f, b: 0.0f));
//                 text.SetFont(font: CoreAssets.Fonts.AnonymousPro, size: 20);
//                 text.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
// 
//                 goldButton.SetSize(80, 80);
//                 goldButton.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
//                 //goldButton.SetLayout(LayoutMode.Horizontal, 5, new IntRect(5, 5, 5, 5));
//                 goldButton.SetStyleAuto();
//                 goldButton.Pressed += GoldButton_Pressed;
// 
//                 goldButtonWindow.AddChild(goldButton);
//                 //listView.AddItem(goldButton);
//             }
            goldButtonWindow.Visible = false;

            InitBuildingsUI();
            //BuildingsList.
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
            UI.UIMouseClick += UI_UIMouseClick;
            UI.UIMouseClickEnd += UI_UIMouseClickEnd;
            // Camera
            cameraNode = scene.CreateChild();
            cameraNode.Position = new Vector3(0f, 10f, 10f);
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

        bool UIClicked = false;

        Color ButtonEnabledColor = Color.Green;
        Color ButtonDisabledColor = Color.Red;

        void InitBuildingsUI()
        {

            UIBuildingProperties = new Dictionary<UIElement, BuildingProperties>();
            UI.LoadLayoutToElement(UI.Root, ResourceCache, "UI/BuildingsWindow.xml");
            XmlFile buildingStyleXml = ResourceCache.GetXmlFile("UI/BuildingWindow.xml");
            BuildingsList = UI.Root.GetChild("BuildingsListView", true) as ListView;
            BuildingWindow = UI.Root.GetChild("BuildingsWindow");
            if (BuildingsList != null)
            {
                foreach (var buildingProperties in BuildingsData.Buildings)
                {
                    var buildingWindow = Helpers.CreateBuildingCreationUIFromProperties(UI, buildingStyleXml, buildingProperties);
                    BuildingsList.AddItem(buildingWindow);

                    UIBuildingProperties.Add(buildingWindow, buildingProperties);

                    var addBuildingButton = buildingWindow.GetChild("CreateButton") as Button;
                    addBuildingButton.Pressed += (o) =>
                    {
                        m_CurrentSelectedTile.AddBuilding(buildingProperties);
                        CloseBuildingSelectionMenu(); // prevent building on same tile
                        GameManager.RemoveResourceValue(buildingProperties.ResourceType, buildingProperties.Cost);
                    };

                    addBuildingButton.Enabled = false;
                    addBuildingButton.SetColor(Color.Red);
                }
                BuildingsList.SetScrollBarsVisible(false, false);

            }
            BuildingWindow.Visible = false;
        }

        Dictionary<UIElement, BuildingProperties> UIBuildingProperties;

        void UpdateUIBuildingsWindow()
        {
            for (uint i = 0; i < BuildingsList.NumItems; i++)
            {
                UpdateUIBuildingWindow(BuildingsList.GetItem(i));
            }
        }

        void UpdateUIBuildingWindow(UIElement buildingWindow)
        {
            BuildingProperties bp = UIBuildingProperties[buildingWindow];

            var addBuildingButton = buildingWindow.GetChild("CreateButton") as Button;
            if(GameManager.GetResourceValue(bp.ResourceType) >= bp.Cost)
            {
                SetButtonEnabled(addBuildingButton);
            }
            else
            {
                SetButtonDisabled(addBuildingButton);
            }

        }

        void SetButtonEnabled(Button button)
        {
            button.Enabled = true;
            button.SetColor(ButtonEnabledColor);
        }

        void SetButtonDisabled(Button button)
        {
            button.Enabled = false;
            button.SetColor(ButtonDisabledColor);
        }

        private void UI_UIMouseClick(UIMouseClickEventArgs obj)
        {
            UIClicked = obj.Element != null;
        }

        private void UI_UIMouseClickEnd(UIMouseClickEndEventArgs obj)
        {
            UIClicked = false;
        }

        bool hoveringUI = false;
        private void Root_HoverEnd(HoverEndEventArgs obj)
        {
            hoveringUI = false;
        }

        private void Root_HoverBegin(HoverBeginEventArgs obj)
        {
            hoveringUI = true;
        }

        private void AddBuildingButton_Pressed(PressedEventArgs obj)
        {
            Debug.WriteLine("apasat buton");
        }

        private void BuildingsList_DragMove(DragMoveEventArgs obj)
        {
            
        }

        private void Input_MouseMoved(MouseMovedEventArgs obj)
        {
            
        }

        private void GoldButton_Pressed(PressedEventArgs obj)
        {
            GameManager.AddResourceValue(IdlePlayerResourceType.Gold, 1);
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
            UpdateUIBuildingsWindow();
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
        BuildingTile m_CurrentSelectedTile;

        BuildingTile m_lastHoveredTile;
        BuildingTile m_CurrentHoveredTile;
        /// <summary>
        /// Move camera for 3D samples
        /// </summary>
        protected void SimpleMoveCamera3D(float timeStep, float moveSpeed = 10.0f)
        {
            RayQueryResult? raycastResult = null;
            
            if (UIClicked)
                return;

            if (Input.MouseMove.LengthSquared > 0)
            {

                if(InputRaycastCollided(Input.MousePosition, out raycastResult))
                {
                    m_CurrentHoveredTile = InterpretRaycastResult(raycastResult.Value);
                    if (m_CurrentHoveredTile != m_lastHoveredTile)
                    {
                        if (m_CurrentHoveredTile != null)
                        {
                            m_CurrentHoveredTile.Hovered = true;
                            Debug.WriteLine("Hovered: " + m_CurrentHoveredTile.Node.Name);
                        }

                        if (m_lastHoveredTile != null)
                            m_lastHoveredTile.Hovered = false;

                        m_lastHoveredTile = m_CurrentHoveredTile;
                    }
                }
                else
                {
                    if (m_lastHoveredTile != null)
                        m_lastHoveredTile.Hovered = false;
                    m_CurrentHoveredTile = null;
                    m_lastHoveredTile = null;
                }
            }

            if (Input.GetMouseButtonPress(MouseButton.Left))
            {
                if(InputRaycastCollided(Input.MousePosition, out raycastResult))
                {
                    m_CurrentSelectedTile = InterpretRaycastResult(raycastResult.Value);

                    if (m_CurrentSelectedTile != m_lastSelectedTile)
                    {
                        m_CurrentSelectedTile.Selected = true;
                        if (m_lastSelectedTile != null)
                            m_lastSelectedTile.Selected = false;
                        m_lastSelectedTile = m_CurrentSelectedTile;

                        if (m_CurrentSelectedTile.HasBuildingBuilt())
                        {
                            CloseBuildingSelectionMenu();
                            OpenBuildingUpgradeMenu();
                        }
                        else
                        {
                            CloseBuildingUpgradeMenu();
                            OpenBuildingSelectionMenu();
                        }
                    }
                }
                else
                {
                    CloseBuildingUpgradeMenu();
                    CloseBuildingSelectionMenu();

                    if (m_CurrentSelectedTile != null)
                    {
                        m_CurrentSelectedTile.Selected = false;
                    }
                }
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

        private void CloseBuildingUpgradeMenu()
        {

        }

        private void OpenBuildingUpgradeMenu()
        {

        }

        private void CloseBuildingSelectionMenu()
        {
            BuildingWindow.Visible = false;
        }

        private void OpenBuildingSelectionMenu()
        {
            BuildingWindow.Visible = true;
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
