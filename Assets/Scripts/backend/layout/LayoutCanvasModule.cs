using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace snorri
{
    public class LayoutCanvasModule : Module
    {
        Canvas canvas;
        CanvasScaler canvasScaler;
        GraphicRaycaster graphicRaycaster;
        EventSystem eventSystem;
        PointerEventData pointerEventData;

        bool isMain;
        

        protected override void AddClasses()
        {
            canvas = ComponentCheck<Canvas>();
            canvasScaler = ComponentCheck<CanvasScaler>();
            graphicRaycaster = ComponentCheck<GraphicRaycaster>();
        }
        protected override void Setup()
        {
            base.Setup();

            Configure();
        }
        protected override void Launch() 
        {
            base.Launch();
            if (Vars.Get<bool>("is_main", false)) {
                GAME.Vars.Set<Canvas>("canvas", this.canvas);
                isMain = true;
            }
            eventSystem = GAME.Vars.Get<EventSystem>("event_system");
        }
        public override void Tick() {
            base.Tick();

            if (isMain)
                RaycastLayoutElements();
        }
        private void RaycastLayoutElements()
        {
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            LayoutButtonModule button = null;
            foreach (RaycastResult result in results)
            {
                NodeEntity entity = result.gameObject.GetComponent<NodeEntity>();
                if (entity != null) {
                    button = entity.Node.GetActor<LayoutButtonModule>();
                    if (button != null)
                        break;
                }
            }
            LayoutButtonModule buttonPrevious = GAME.Vars.Get<LayoutButtonModule>("layout:active_button", null);
            LayoutButtonModule buttonPreviousClicked = GAME.Vars.Get<LayoutButtonModule>("layout:clicked_button", null);

            if (button != null) {
                GAME.Vars.Set<LayoutButtonModule>("layout:active_button", button);

                if (buttonPrevious != button) {
                    if (buttonPrevious != null) {
                        buttonPrevious.InformState(LayoutState.HoverOut);
                    }
                    button.InformState(LayoutState.HoverIn);
                }
                if (Input.GetMouseButtonDown(0)) {
                    if (buttonPreviousClicked != button) {
                        if (buttonPreviousClicked != null) {
                            buttonPreviousClicked.InformState(LayoutState.ClickOut);
                            buttonPreviousClicked.IsSelected = false;
                        }
                        button.InformState(LayoutState.ClickIn);
                        button.IsSelected = true;

                        GAME.Vars.Set<LayoutButtonModule>("layout:clicked_button", button);
                    } else if (!button.IsClickable) {
                        button.InformState(LayoutState.ClickIn);
                    }
                }
            } else {
                // we need to clear
                GAME.Vars.Set<LayoutButtonModule>("layout:active_button", null);
                if (buttonPrevious != null) {
                    buttonPrevious.InformState(LayoutState.HoverOut);
                }

                if (Input.GetMouseButtonDown(0)) {
                    if (buttonPreviousClicked != null) {
                        buttonPreviousClicked.InformState(LayoutState.ClickOut);
                        buttonPreviousClicked.IsSelected = false;
                    }
                    GAME.Vars.Set<LayoutButtonModule>("layout:clicked_button", null);
                }
            }
        }

        void Configure()
        {
            ConfigureCanvas();
            ConfigureScaler();
        }

        void ConfigureCanvas()
        {
            string renderMode = Vars.Get<string>("render_mode", "overlay");
            int sortOrder = Vars.Get<int>("sort_order", 0);

            ConfigureRenderMode(renderMode);
            canvas.sortingOrder = sortOrder;
        }
        void ConfigureRenderMode(string renderMode)
        {
            switch (renderMode)
            {
                case "overlay":
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    break;
                case "world":
                    canvas.renderMode = RenderMode.WorldSpace;
                    break;
                case "screen":
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    break;
            }
            
        }
        void ConfigureScaler()
        {
            string scalerMode = Vars.Get<string>("scale_mode", "screen");
            switch (scalerMode)
            {
                case "pixel":
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                    break;
                case "screen":
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    
                    canvasScaler.referenceResolution = new Vector2(1920, 1080); // Example resolution
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    canvasScaler.matchWidthOrHeight = 0.5f; // 0 is width, 1 is height, 0.5 is an equal balance
                    break;
                case "constant":
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
                    break;
            }
        }
    }
}