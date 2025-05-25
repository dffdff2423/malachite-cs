// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using Malachite.App.Graphics.SDL;
using Malachite.Core.Maths;

namespace Malachite.App.Graphics;

public sealed class GraphicsContext : IDisposable {
    private bool _disposed;

    private readonly SDLApplication _app;
    private readonly SDLDevice _dev;
    private readonly SDLWindow _mainWindow;

    public event Action? ShouldQuitApp;

    public GraphicsContext() {
        _app = new SDLApplication();
        _dev = new SDLDevice();
        _mainWindow = _dev.CreateAssociatedWindow(ApplicationInfo.Name, new Vector2i(1440, 720));

        _app.ShouldQuit += () => {
            ShouldQuitApp?.Invoke();
        };

        _app.WindowShouldCLose += id => {
            if (id == _mainWindow.Id()) {
                ShouldQuitApp?.Invoke();
            }
        };
    }

    public void Dispose() {
        if (_disposed)
            return;

        _dev.Dispose();
        _app.Dispose();

        _disposed = true;
    }

    public void ProcessEvents() {
        _app.ProcessEvents();
    }
}
