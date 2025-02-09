﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ColorPicker.Helpers;
using ColorPicker.Settings;

namespace Microsoft.ColorPicker.UnitTests.ViewModels;

public class AppStateHandlerFake : AppStateHandler
{
    public AppStateHandlerFake(IUserSettings userSettings)
        : base(userSettings)
    {
    }
}
