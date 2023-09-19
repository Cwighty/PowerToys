// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace Microsoft.ColorPicker.UnitTests.ViewModels;

public class ApplicationFake : Application
{
    public static new Application Current => new Application();
}
