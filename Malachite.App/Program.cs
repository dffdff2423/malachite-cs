// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using Malachite.App.Graphics;

namespace Malachite.App;

internal static class Program {
    private static void Main(string[] _) {
        using var gfx = new GraphicsContext();

        bool shouldQuit = false;
        gfx.ShouldQuitApp += () => shouldQuit = true;

        while (!shouldQuit) {
            gfx.ProcessEvents();
        }
    }
}
