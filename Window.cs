﻿using Devdog.General.UI;
using System;
using System.Collections;
using UIWindowPageFramework;
using UnityEngine;
using UnityEngine.UI;

namespace BonfireWarp
{
    internal class Window: MonoBehaviour
    {
        internal static GameObject Viewport;
        internal static GameObject RegisteredViewport;
        private void Awake()
        {
            Plugin.Log.LogInfo("Starting RegisterWindow coroutine.");
            StartCoroutine(RegisterWindow());
        }

        private void Update()
        {
            RegisteredViewport = GameObject.Find("MAINMENU/Canvas/Pages/VISITED STATIONS/Viewport");
        }

        public IEnumerator AddButton(string name, string OriginalName)
        {
            Plugin.Log.LogInfo($"Starting coroutine for button {name} with origin {OriginalName}.");
            while (Viewport == null || ComponentUtils.GetFont("Orbitron-Regular") == null)
            {
                yield return null;
            }
            Plugin.Log.LogInfo($"Registering bonfire.{name}");
            GameObject button = ComponentUtils.CreateButton(name, OriginalName);
            button.Find("ItemName").GetComponent<RectTransform>().anchoredPosition = new Vector2(-0.8826f, -2.7624f);
            Text text = button.Find("ItemName").GetComponent<Text>();
            text.resizeTextForBestFit = true;
            text.font = ComponentUtils.GetFont("Orbitron-Regular");
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.resizeTextMaxSize = 20;
            button.SetParent(Viewport, false);
            if (RegisteredViewport != null)
            {
                Plugin.Log.LogInfo($"Adding bonfire {name} to registered viewport.");
                GameObject RegisteredButton = ComponentUtils.CreateButton(name, OriginalName);
                RegisteredButton.AddComponent<LayoutElement>();
                Text RegisteredButtonText = RegisteredButton.Find("ItemName").GetComponent<Text>();
                RegisteredButton.Find("ItemName").GetComponent<RectTransform>().anchoredPosition = new Vector2(-0.8826f, -2.7624f);
                RegisteredButtonText.resizeTextForBestFit = true;
                RegisteredButtonText.font = ComponentUtils.GetFont("Orbitron-Regular");
                RegisteredButtonText.alignment = TextAnchor.MiddleCenter;
                RegisteredButtonText.horizontalOverflow = HorizontalWrapMode.Overflow;
                RegisteredButtonText.resizeTextMaxSize = 20;
                RegisteredButton.SetParent(RegisteredViewport, false);
            }
        }

        private IEnumerator RegisterWindow()
        {
            while (!Framework.Ready)
            {
                Plugin.Log.LogInfo("Waiting for framework to be ready.");
                yield return null;
            }
            GameObject window = Framework.CreateWindow("VISITED STATIONS");
            Viewport = window.AddObject("Viewport");
            RectTransform ViewportRect = Viewport.AddComponent<RectTransform>();
            Viewport.AddComponent<CanvasRenderer>();
            Viewport.AddComponent<Animator>();
            CanvasGroup canvas = Viewport.AddComponent<CanvasGroup>();
            canvas.blocksRaycasts = true;
            canvas.interactable = true;
            Viewport.AddComponent<Mask>();
            Viewport.AddComponent<AnimatorHelper>();
            ViewportRect.anchoredPosition = new Vector2(34.5819f, -85.4401f);
            ViewportRect.sizeDelta = new Vector2(1500, 700);
            GridLayoutGroup group = Viewport.AddComponent<GridLayoutGroup>();
            group.childAlignment = TextAnchor.UpperLeft;
            group.spacing = new Vector2(20, 20);
            group.cellSize = new Vector2(300, 50);
            group.startCorner = GridLayoutGroup.Corner.UpperLeft;
            group.startAxis = GridLayoutGroup.Axis.Horizontal;
            Framework.RegisterWindow(window, (GameObject win) =>
            {
                GameObject RegisteredViewport = win.Find("Viewport");
                foreach (Button button in RegisteredViewport.GetComponentsInChildren<Button>())
                {
                    button.onClick.AddListener(() =>
                    {
                        GameObject @obj = GameObject.Find($"Saves/{button.name}");
                        Station station = obj.GetComponent<Station>();
                        station.Context.Invoke();
                        GameObject.FindObjectOfType<Inventory>().transform.position = station.spawn.position;
                    });
                }
            });
            Plugin.Log.LogInfo("RegisterWindow coroutine complete.");
        }
    }
}
