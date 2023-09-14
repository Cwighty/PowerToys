// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using ColorPicker.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ColorPicker.UnitTests.ViewModels;

[TestClass]
public class MainViewModelTests
{
    [TestMethod]
    public void AddColorsOnMouseDownTest()
    {
        var mouseInfoProvider = new MouseInfoProviderFake();
        var userSettings = new UserSettingsFake();
        var viewModel = new MainViewModel(mouseInfoProvider, null, null, null, userSettings, System.Threading.CancellationToken.None);

        mouseInfoProvider.TriggerColorChange(Color.Red);

        Assert.Equals(userSettings.ColorHistory.Count, 1);
    }
}
