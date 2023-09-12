// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ColorPicker.Common;
using ColorPicker.Settings;
using Microsoft.PowerToys.Settings.UI.Library.Enumerations;

namespace Microsoft.ColorPicker.UnitTests.ViewModels;

public class UserSettingsFake : IUserSettings
{
    public SettingItem<string> ActivationShortcut => throw new NotImplementedException();

    public SettingItem<bool> ChangeCursor => throw new NotImplementedException();

    public SettingItem<string> CopiedColorRepresentation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public SettingItem<string> CopiedColorRepresentationFormat { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public SettingItem<ColorPickerActivationAction> ActivationAction => throw new NotImplementedException();

    public RangeObservableCollection<string> ColorHistory => throw new NotImplementedException();

    public RangeObservableCollection<string> PinnedColors { get; set; } = new RangeObservableCollection<string>();

    public SettingItem<int> ColorHistoryLimit => new SettingItem<int>(20);

    public ObservableCollection<KeyValuePair<string, string>> VisibleColorFormats => throw new NotImplementedException();

    public SettingItem<bool> ShowColorName => throw new NotImplementedException();

    ObservableCollection<KeyValuePair<string, string>> IUserSettings.VisibleColorFormats => new ObservableCollection<KeyValuePair<string, string>>();

    public void SendSettingsTelemetry()
    {
        throw new NotImplementedException();
    }
}
