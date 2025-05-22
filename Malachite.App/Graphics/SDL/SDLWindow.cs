// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using Malachite.Core.Maths;

using SDL;

using static SDL.SDL3;

namespace Malachite.App.Graphics.SDL;

/// <summary>
/// Managed wrapper around a SDL Window
/// </summary>
public sealed unsafe class SDLWindow : IDisposable {
    private bool _disposed;
    public SDL_Window* Handle;

    /// <summary>
    /// Create a new window using the SDL api.
    /// </summary>
    /// <param name="title">The title given to the OS</param>
    /// <param name="extent">The size of the window</param>
    /// <param name="flags">Flags</param>
    /// <exception cref="SDLException">Upon failing to create the window</exception>
    public SDLWindow(string title, Vector2i extent, SDL_WindowFlags flags = 0) {
        Handle = SDL_CreateWindow(title, extent.X, extent.Y, flags);
        if (Handle == null)
            throw SDLException.FromLastError($"Failed to create window \"{title}\" with extent ({extent.X}, {extent.Y})");
    }

    // ReSharper disable once UnusedParameter.Local
    private void Dispose(bool disposing) {
        if (_disposed)
            return;

        SDL_DestroyWindow(Handle);
        Handle = null;

        _disposed = true;
    }

    ~SDLWindow() {
        Dispose(disposing: false);
    }

    public void Dispose() {
        Dispose(disposing: true);

        GC.SuppressFinalize(this);
    }
}
