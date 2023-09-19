// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ColorPicker.Helpers;

namespace Microsoft.ColorPicker.UnitTests.ViewModels
{
    public class ClipboardHelperFake : ClipboardHelper
    {
        public new void CopyToClipboard(string colorRepresentationToCopy)
        {
            return;
        }
    }
}
