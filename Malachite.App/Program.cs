// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using Malachite.App.Graphics.SDL;
using Malachite.Core.Maths;

namespace Malachite.App;

internal static class Program {
    private static void Main(string[] _) {
        using var app = new SDLApplication();
        using var dev = new SDLDevice();
        var window = dev.CreateAssociatedWindow(ApplicationInfo.Name, new Vector2i(1440, 720));
    }
}
