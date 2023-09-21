// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using ColorPicker.Common;
using ColorPicker.Settings;
using Microsoft.PowerToys.Settings.UI.Library.Enumerations;

namespace Microsoft.ColorPicker.UnitTests.ViewModels;

public class UserSettingsFake : IUserSettings
{
    public SettingItem<string> ActivationShortcut => throw new NotImplementedException();

    public SettingItem<bool> ChangeCursor => throw new NotImplementedException();

    public SettingItem<string> CopiedColorRepresentation { get => new SettingItem<string>("Red"); set => throw new NotImplementedException(); }

    public SettingItem<string> CopiedColorRepresentationFormat { get => new SettingItem<string>(string.Empty); set => throw new NotImplementedException(); }

    public SettingItem<ColorPickerActivationAction> ActivationAction => new SettingItem<ColorPickerActivationAction>(ColorPickerActivationAction.OpenOnlyColorPicker);

    public RangeObservableCollection<string> ColorHistory { get; set; } = new RangeObservableCollection<string>();

    public RangeObservableCollection<string> PinnedColors { get; set; } = new RangeObservableCollection<string>();

    public SettingItem<int> ColorHistoryLimit => new SettingItem<int>(20);

    public ObservableCollection<KeyValuePair<string, string>> VisibleColorFormats => throw new NotImplementedException();

    public SettingItem<bool> ShowColorName => new SettingItem<bool>(true);

    ObservableCollection<KeyValuePair<string, string>> IUserSettings.VisibleColorFormats => new ObservableCollection<KeyValuePair<string, string>>();

    public void SendSettingsTelemetry()
    {
        throw new NotImplementedException();
    }
}
