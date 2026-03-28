// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using System.CommandLine;

using Malachite.App.Graphics;
using Malachite.Core.Resources;

namespace Malachite.App;

internal static class Program {
    private static void Main(string[] argv) {
        var cmd = new RootCommand("Malachite game");

        var resDirOpt = new Option<FileInfo>(
            name: "--res-dir",
            description: "Path to the resource directory",
            getDefaultValue: () => new FileInfo("./res"));
        resDirOpt.AddAlias("-r");
        cmd.Add(resDirOpt);

        cmd.SetHandler(resDir => {
            var resLoader = new ResourceLoader(resDir);

            using var gfx = new GraphicsContext(resLoader);

            bool shouldQuit = false;
            gfx.ShouldQuitApp += () => shouldQuit = true;

            while (!shouldQuit) {
                gfx.ProcessEvents();
                gfx.Draw();
            }
        }, resDirOpt);
        cmd.Invoke(argv);
    }
}
