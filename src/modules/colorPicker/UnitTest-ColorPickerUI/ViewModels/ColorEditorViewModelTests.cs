// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
using ColorPicker.Helpers;
using ColorPicker.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ColorPicker.UnitTests.ViewModels;

[TestClass]
public class ColorEditorViewModelTests
{
    [TestMethod]
    public void PinOneColorAddsColorToPinnedColorsTest()
    {
        SessionEventHelper.Start(PowerToys.Settings.UI.Library.Enumerations.ColorPickerActivationAction.OpenEditor);
        var userSettings = new UserSettingsFake();
        var viewModel = new ColorEditorViewModel(userSettings);

        var selectedColors = new List<Color>()
        {
            new Color() { R = 1, G = 2, B = 3 },
        };
        viewModel.PinColorsCommand.Execute(selectedColors);

        Assert.AreEqual(1, viewModel.PinnedColors.Count);
    }

    [TestMethod]
    public void PinMultipleColorAddsColorToPinnedColorsTest()
    {
        SessionEventHelper.Start(PowerToys.Settings.UI.Library.Enumerations.ColorPickerActivationAction.OpenEditor);
        var userSettings = new UserSettingsFake();
        var viewModel = new ColorEditorViewModel(userSettings);

        var selectedColors = new List<Color>()
        {
            new Color() { R = 1, G = 2, B = 3 },
            new Color() { R = 2, G = 2, B = 3 },
        };
        viewModel.PinColorsCommand.Execute(selectedColors);

        Assert.AreEqual(2, viewModel.PinnedColors.Count);
        Assert.AreEqual(2, userSettings.PinnedColors.Count);
        CollectionAssert.AreEquivalent(selectedColors, viewModel.PinnedColors);
    }

    [TestMethod]
    public void RemovePinnedColorsLeavesPinnedColorsEmpty()
    {
        SessionEventHelper.Start(PowerToys.Settings.UI.Library.Enumerations.ColorPickerActivationAction.OpenEditor);
        var userSettings = new UserSettingsFake();
        var viewModel = new ColorEditorViewModel(userSettings);

        var selectedColors = new List<Color>()
        {
            new Color() { R = 255, G = 0, B = 0 },
            new Color() { R = 2, G = 2, B = 3 },
        };
        viewModel.PinColorsCommand.Execute(selectedColors);

        viewModel.ClearAllPinnedColorsCommand.Execute(null);

        Assert.AreEqual(0, viewModel.PinnedColors.Count);
        Assert.AreEqual(0, userSettings.PinnedColors.Count);
    }
}
