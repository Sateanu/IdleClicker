using System.Collections.Generic;
using System.Diagnostics;
using Urho;
using Urho.Gui;
using Urho.IO;
using Urho.Resources;

namespace IdleClicker
{
    public class MyGame : Application
    {
        Node cameraNode;
        Scene scene;
        Camera MainCamera;
        Button goldButton;
        int gold = 0;
        Window goldButtonWindow;
        Text goldText;
        Text goldPerSecText;
        ListView BuildingsList;

        float DT = 0;

        bool m_Loading;
        File m_SaveFile;

        public UIElement BuildingsWindow { get; private set; }

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
            scene.AsyncLoadFinished += Scene_AsyncLoadFinished;

            Node rootNode = scene.CreateChild();
            
            rootNode.Position = new Vector3(0, 0, 20);

//             var m_Geometry = rootNode.CreateChild();
//             var model = m_Geometry.CreateComponent<StaticModel>();
//             model.Model = ResourceCache.GetModel(IdleClicker.Assets.Models.Mountain);
//             model.SetMaterial(ResourceCache.GetMaterial(IdleClicker.Assets.Materials.House));
//             m_Geometry.SetScale(0.4f);

            //UI.UIMouseClick += UI_UIMouseClick;

            //GUI

            goldButtonWindow = new Window();
            goldButtonWindow.SetLayout(LayoutMode.Vertical, 6, new IntRect(6, 6, 6, 6));

            //var listView = goldButtonWindow.CreateListView();
            //listView.SetMinAnchor(0, 0);
            //listView.SetMaxAnchor(1, 1);
            //listView.SetColor(Color.Magenta);
            //listView.EnableAnchor = true;

            goldButtonWindow.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            goldButtonWindow.SetMinSize(300, 300);
            
            goldButtonWindow.EnableAnchor = true;
            goldButtonWindow.Name = "GoldWindow";
            goldButtonWindow.SetStyleAuto();
            //UI.Root.AddChild(goldButtonWindow);

            goldText = new Text(Context);
            goldText.Value = string.Format(Assets.FormatStrings.Gold, gold);
            goldText.HorizontalAlignment = HorizontalAlignment.Center;
            goldText.VerticalAlignment = VerticalAlignment.Top;
            goldText.SetColor(new Color(r: 1.0f, g: 1.0f, b: 0.0f));
            goldText.SetFont(CoreAssets.Fonts.AnonymousPro, 30);
            UI.Root.AddChild(goldText);

            goldPerSecText = new Text(Context);
            goldPerSecText.HorizontalAlignment = HorizontalAlignment.Center;
            goldPerSecText.VerticalAlignment = VerticalAlignment.Top;
            goldPerSecText.SetPosition(goldText.Position.X, goldText.Position.Y + 35);
            goldPerSecText.SetColor(new Color(r: 1.0f, g: 1.0f, b: 0.0f));
            goldPerSecText.SetFont(CoreAssets.Fonts.AnonymousPro, 30);
            UI.Root.AddChild(goldPerSecText);

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
            cameraNode = scene.CreateChild("MainCamera");
            cameraNode.Position = new Vector3(45, 40, -45);
            cameraNode.LookAt(rootNode.Position, Vector3.Up);
            //cameraNode.Rotation = new Quaternion(30f, -30.0f, 0.0f);
            MainCamera = cameraNode.CreateComponent<Camera>();
            MainCamera.Orthographic = true;
            MainCamera.FarClip = 300.0f;
            
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

            Input.MultiGesture += Input_MultiGesture;
            Input.TouchMove += Input_TouchMove;
            Input.TouchBegin += Input_TouchBegin;
        }

        private void Scene_AsyncLoadFinished(AsyncLoadFinishedEventArgs obj)
        {
            cameraNode = scene.GetChild("MainCamera");
            MainCamera = cameraNode.GetComponent<Camera>();

            var viewport = new Viewport(Context, scene, MainCamera, null);
            Renderer.SetViewport(0, viewport);

            m_Loading = false;
            m_SaveFile = null;
        }

        private void Input_TouchBegin(TouchBeginEventArgs obj)
        {
            
        }

        private void Input_TouchMove(TouchMoveEventArgs obj)
        {
            
        }

        const float PinchFactor = 15.0f;
        const float TouchMovementFactor = 1f;

        float m_PreviousDDist = 0.0f;
        float m_PreviousTouchX = -1.0f;
        float m_PreviousTouchY = -1.0f;

        private void Input_MultiGesture(MultiGestureEventArgs obj)
        {
            if (obj.NumFingers >= 2)
            {
                Debug.WriteLine("DDist: {0}", obj.DDist);

                if(obj.NumFingers == 2)
                {
                    MainCamera.OrthoSize -= obj.DDist * PinchFactor;
                }
                else if (obj.NumFingers >= 3)
                {
                    //if(m_PreviousTouchX == -1 && m_PreviousTouchY == -1)
                    //{
                    //    m_PreviousTouchX = obj.CenterX;
                    //    m_PreviousTouchY = obj.CenterY;
                    //}

                    //float deltaX = obj.CenterX - m_PreviousTouchX;
                    //float deltaY = obj.CenterY - m_PreviousTouchY;

                    //float cameraX = -TouchMovementFactor * deltaX * DT;
                    //float cameraY = TouchMovementFactor * deltaY * DT;

                    ////cameraNode.Position = cameraNode.LocalToWorld(new Vector3(cameraX, cameraY, 0f));
                    //cameraNode.Position = new Vector3(cameraNode.Position.X + cameraX, cameraNode.Position.Y + cameraY, cameraNode.Position.Z);

                    //Debug.WriteLine("Camera " + cameraY + " " + cameraX);

                    //m_PreviousTouchX = obj.CenterX;
                    //m_PreviousTouchY = obj.CenterY;
                }

                m_PreviousDDist = obj.DDist;
            }
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
            BuildingsWindow = UI.Root.GetChild("BuildingsWindow");
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
                        OpenBuildingUpgradeMenu(m_CurrentSelectedTile.Building as Building);
                        GameManager.RemoveResourceValue(buildingProperties.ResourceType, buildingProperties.Cost);
                    };

                    addBuildingButton.Enabled = false;
                    addBuildingButton.SetColor(Color.Red);
                }
                BuildingsList.SetScrollBarsVisible(false, false);

            }
            BuildingsWindow.Visible = false;
        }

        Dictionary<UIElement, BuildingProperties> UIBuildingProperties;

        void UpdateUIBuildingsWindow()
        {
            if (!BuildingsWindow.Visible)
            {
                return;
            }

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

        protected override void OnUpdate(float timeStep)
        {
            if (m_Loading)
            {
                return;
            }

            DT = timeStep;
            if (Input.GetKeyPress(Key.Esc))
            {
                Exit();
                return;
            }

            if (Input.GetKeyPress(Key.F5))
            {
                scene.SaveXml(FileSystem.ProgramDir + "Data/IdleClicker.xml", "\t");
            }
            if (Input.GetKeyPress(Key.F7))
            {
                m_Loading = true;
                m_SaveFile = new File(Context, FileSystem.ProgramDir + "Data/IdleClicker.xml");
                scene.LoadAsyncXml(m_SaveFile);
                return;
            }

            UpdateUI();
            MoveCameraByTouches(timeStep);
            SimpleMoveCamera3D(timeStep);
            base.OnUpdate(timeStep);
        }

        private void UpdateUI()
        {
            UpdateGoldPerSecText();
            UpdateUIBuildingsWindow();
            UpdateBuildingUpgradeMenu();
            goldText.Value = string.Format(Assets.FormatStrings.Gold, Helpers.GetNumberSuffixed(GameManager.GetResourceValue(IdlePlayerResourceType.Gold), "{0:0.00}"));
        }

        private void UpdateGoldPerSecText()
        {
            goldPerSecText.Value = string.Format(Assets.FormatStrings.GoldPerSecond, Helpers.GetNumberSuffixed(GameManager.GetGoldPerSecond(), "{0:0.00}"));
        }

        bool InputRaycastCollided(IntVector2 position, out RayQueryResult? raycastResult)
        {
            Vector2 normScreenPos = Helpers.GetNormalizedScreenPosition(Graphics, position);

            Ray ray = MainCamera.GetScreenRay(normScreenPos);
            raycastResult = scene.GetComponent<Octree>().RaycastSingle(ray, maxDistance: 1000);

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
            
            if (UIClicked || m_Loading)
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

                        CloseBuildingUpgradeMenu();
                        CloseBuildingSelectionMenu();

                        if (m_CurrentSelectedTile.HasBuildingBuilt())
                        {
                            OpenBuildingUpgradeMenu(m_CurrentSelectedTile.Building as Building);
                        }
                        else if (m_CurrentSelectedTile.IsBuildable)
                        {
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
                /*MainCamera.NearClip = -1000;*/ //TODO
            }
        }

        UIElement BuildingUpgradeWindow = null;
        XmlFile BuildingDetailsXML = null;
        private void CloseBuildingUpgradeMenu()
        {
            if(BuildingUpgradeWindow != null)
            {
                BuildingUpgradeWindow.Visible = false;
                UI.Root.RemoveChild(BuildingUpgradeWindow);
                BuildingUpgradeWindow.Dispose();
                BuildingUpgradeWindow = null;
            }
        }

        private void UpdateBuildingUpgradeUIFromBuilding(UIElement buildingWindow, Building building)
        {
            if (building == null)
            {
                return;
            }

            var properties = building.BuildingProperties;

            var BuildingName = buildingWindow.GetChild("BuildingName", true) as Text;
            BuildingName.Value = properties.Name;

            var BuildingLevel = buildingWindow.GetChild("BuildingLevel", true) as Text;
            BuildingLevel.Value = string.Format(Assets.FormatStrings.BuildingWindowLevel, building.Level);

            var BuildingPrice = buildingWindow.GetChild("BuildingPrice", true) as Text;
            float cost = properties.GetCostForLevel(building.Level + 1);
            BuildingPrice.Value = Helpers.GetNumberSuffixed(cost, "-{0:0.00}");

            var BuildingReward = buildingWindow.GetChild("BuildingReward", true) as Text;
            BuildingReward.Value = string.Format("{0}/{1:0.0s}", Helpers.GetNumberSuffixed(properties.GetRewardForLevel(building.Level + 1, building.Neighbors), "+{0:0.00}"), properties.TimeForReward);

            var BuildingSellReward = buildingWindow.GetChild("SellReward", true) as Text;
            float sellReward = 0.0f;
            if (building.Level == 1)
            {
                sellReward = properties.Cost;
            }
            else
            {
                sellReward = properties.GetCostForLevel(building.Level) / 2;
            }

            BuildingSellReward.Value = Helpers.GetNumberSuffixed(sellReward, "+{0:0.00}");

            var BuildingType = buildingWindow.GetChild("BuildingType", true) as Text;
            BuildingType.Value = properties.ResourceType.ToString();

            var addBuildingButton = buildingWindow.GetChild("UpgradeButton") as Button;
            if (GameManager.GetResourceValue(properties.ResourceType) >= cost)
            {
                SetButtonEnabled(addBuildingButton);
            }
            else
            {
                SetButtonDisabled(addBuildingButton);
            }
        }

        private void UpdateBuildingUpgradeMenu()
        {
            if (BuildingUpgradeWindow == null)
            {
                return;
            }

            UpdateBuildingUpgradeUIFromBuilding(BuildingUpgradeWindow, m_CurrentSelectedTile.Building as Building);
        }

        private void OpenBuildingUpgradeMenu(Building building)
        {
            CloseBuildingUpgradeMenu();
            if (building == null)
            {
                return;
            }

            if(BuildingDetailsXML == null)
            {
                BuildingDetailsXML = ResourceCache.GetXmlFile("UI/BuildingDetails.xml");
            }

            BuildingUpgradeWindow = Helpers.CreateBuildingUpgradeUIFromBuilding(UI, BuildingDetailsXML, building);
            Debug.Assert(BuildingUpgradeWindow != null);

            UI.Root.AddChild(BuildingUpgradeWindow);

            var upgradeButton = BuildingUpgradeWindow.GetChild("UpgradeButton") as Button;
            if(upgradeButton != null)
            upgradeButton.Pressed += (o) =>
            {
                building.Level++;
                GameManager.RemoveResourceValue(building.BuildingProperties.ResourceType, building.BuildingProperties.GetCostForLevel(building.Level));
            };

            var deleteButton = BuildingUpgradeWindow.GetChild("DeleteButton") as Button;
            if(deleteButton != null)
            deleteButton.Pressed += (o) =>
            {
                m_CurrentSelectedTile.QueueDestroyBuilding();
                if(building.Level == 1)
                {
                    GameManager.AddResourceValue(building.BuildingProperties.ResourceType, building.BuildingProperties.Cost);
                }
                else
                {
                    GameManager.AddResourceValue(building.BuildingProperties.ResourceType, building.BuildingProperties.GetCostForLevel(building.Level) / 2);
                }
                CloseBuildingUpgradeMenu();
                m_CurrentSelectedTile.Selected = false;
                m_lastSelectedTile = null;
            };
        }

        private void CloseBuildingSelectionMenu()
        {
            BuildingsWindow.Visible = false;
        }

        private void OpenBuildingSelectionMenu()
        {
            BuildingsWindow.Visible = true;
        }

        protected void MoveCameraByTouches(float timeStep)
        {
            const float touchSensitivity = 0.2f;
            RayQueryResult? raycastResult = null;

            var input = Input;

            if (UIClicked || m_Loading)
                return;

            if (Input.MouseMove.LengthSquared > 0)
            {
                if (InputRaycastCollided(Input.MousePosition, out raycastResult))
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
                if (InputRaycastCollided(Input.MousePosition, out raycastResult))
                {
                    m_CurrentSelectedTile = InterpretRaycastResult(raycastResult.Value);

                    if (m_CurrentSelectedTile != m_lastSelectedTile)
                    {
                        m_CurrentSelectedTile.Selected = true;
                        if (m_lastSelectedTile != null)
                            m_lastSelectedTile.Selected = false;
                        m_lastSelectedTile = m_CurrentSelectedTile;

                        CloseBuildingUpgradeMenu();
                        CloseBuildingSelectionMenu();

                        if (m_CurrentSelectedTile.HasBuildingBuilt())
                        {
                            OpenBuildingUpgradeMenu(m_CurrentSelectedTile.Building as Building);
                        }
                        else if (m_CurrentSelectedTile.IsBuildable)
                        {
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
            if(input.NumTouches == 1)
            {
                TouchState state = input.GetTouch(0);
                if (state.Delta.X != 0 || state.Delta.Y != 0)
                {
                    var camera = cameraNode.GetComponent<Camera>();
                    if (camera == null)
                        return;

                    float cameraX = -TouchMovementFactor * state.Delta.X * DT;
                    float cameraY = TouchMovementFactor * state.Delta.Y * DT;

                    cameraNode.Position = cameraNode.LocalToWorld(new Vector3(cameraX, cameraY, 0f));
                    //cameraNode.Position = new Vector3(cameraNode.Position.X + cameraX, cameraNode.Position.Y + cameraY, cameraNode.Position.Z);

                    Debug.WriteLine("Camera " + cameraY + " " + cameraX);
                }
                else if (state.Delta == IntVector2.Zero && InputRaycastCollided(state.Position, out raycastResult))
                {
                    m_CurrentSelectedTile = InterpretRaycastResult(raycastResult.Value);

                    if (m_CurrentSelectedTile != m_lastSelectedTile)
                    {
                        m_CurrentSelectedTile.Selected = true;
                        if (m_lastSelectedTile != null)
                            m_lastSelectedTile.Selected = false;
                        m_lastSelectedTile = m_CurrentSelectedTile;

                        CloseBuildingUpgradeMenu();
                        CloseBuildingSelectionMenu();

                        if (m_CurrentSelectedTile.HasBuildingBuilt())
                        {
                            OpenBuildingUpgradeMenu(m_CurrentSelectedTile.Building as Building);
                        }
                        else if (m_CurrentSelectedTile.IsBuildable)
                        {
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
        }
    }
}
