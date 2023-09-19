// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using ColorPicker.Mouse;

namespace Microsoft.ColorPicker.UnitTests.ViewModels;

public class MouseInfoProviderFake : IMouseInfoProvider
{
    public System.Windows.Point CurrentPosition => throw new NotImplementedException();

    public Color CurrentColor { get; set; }

    public event EventHandler<Color> MouseColorChanged = (sender, e) => { };

    public event EventHandler<System.Windows.Point> MousePositionChanged = (sender, e) => { };

    public event EventHandler<Tuple<System.Windows.Point, bool>> OnMouseWheel = (sender, e) => { };

    public event MouseUpEventHandler OnMouseDown = (sender, e) => { };

    public void TriggerColorChange(Color color)
    {
        MouseColorChanged?.Invoke(this, color);
        CurrentColor = color;
        OnMouseDown?.Invoke(this, default);
    }
}
