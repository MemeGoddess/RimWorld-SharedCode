using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

// ReSharper disable once CheckNamespace
namespace MemeGoddess.RimWorld.TabsHandler;

public abstract class TabsHandler<T> where T : struct, Enum
{
    private List<TabRecord> tabs;
    private T selectedTab;
    public TabsHandler()
    {
        var enumValues = Enum.GetValues<T>();
        tabs =
        [
            .. enumValues.Select(x =>
                new TabRecord(TabTitle(x),
                    () => selectedTab = x, 
                    () => selectedTab.Equals(x)))
        ];
        selectedTab = enumValues.FirstOrDefault();
    }

    public void DoWindowContents(Rect rect)
    {
        rect.SplitHorizontallyWithMargin(out var header, out var body, out _, 4f, Text.LineHeight);
        DrawTabs(header, tabs);
        Widgets.DrawMenuSection(body);
        DoTabRect(selectedTab, body.ContractedBy(15f));
    }

    public abstract void DoTabRect(T selectedTab, Rect rect);
    
    [CanBeNull] public abstract string TabTitle(T selectedTab);
    
    private static Color SelectedColor = new Color(0.5f, 1f, 0.5f, 1f);
    private void DrawTabs(Rect rect, List<TabRecord> tabs)
    {
        var buttons = tabs.Count;
        var rects = SplitRectangle(rect, buttons, 4f);

        var color = GUI.color;
        for (var index = 0; index < rects.Length; index++)
        {
            var button = rects[index];
            var tab = tabs[index];

            if (tab.Selected)
                GUI.color = SelectedColor;
            if (Widgets.ButtonText(button, tab.label))
                tab.clickedAction();
            GUI.color = color;
        }
    }

    private Rect[] SplitRectangle(Rect rect, int count, float margin)
    {
        var rects = new Rect[count];
        var totalMargin = margin * (count - 1);
        var usableWidth = rect.width - totalMargin;
        var rectWidth = usableWidth / count;

        for (var i = 0; i < count; i++)
        {
            var xPosition = rect.x + (i * (rectWidth + margin));
            rects[i] = new Rect(xPosition, rect.y, rectWidth, rect.height);
        }

        return rects;
    }
}