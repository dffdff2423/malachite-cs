// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using SDL;

using static SDL.SDL3;

namespace Malachite.App.Graphics.SDL;

/// <summary>
/// Manages the overarching state of the application. Only one of these should ever be created.
/// </summary>
public sealed class SDLApplication : IDisposable {
    private bool _disposed;

    private const SDL_InitFlags Flags = SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_EVENTS;

    public SDLApplication() {
        if (!SDL_Init(Flags))
            throw SDLException.FromLastError("Failed to initialize SDL");

        if (ApplicationInfo.Debug && !SDL_SetHint(SDL_HINT_LOGGING, "*=trace"))
            throw SDLException.FromLastError("Failed to set logging hint");

        if (!SDL_SetAppMetadata(ApplicationInfo.Name, ApplicationInfo.Version, ApplicationInfo.Identifier)
            || !SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_URL_STRING, ApplicationInfo.Url)
            || !SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_TYPE_STRING, "game"))
            throw SDLException.FromLastError("Failed to set app metadata");
    }

    // ReSharper disable once UnusedParameter.Local
    private void Dispose(bool disposing) {
        if (_disposed)
            return;

        SDL_Quit();

        _disposed = true;
    }

    ~SDLApplication() {
        Dispose(disposing: false);
    }

    public void Dispose() {
        Dispose(disposing: true);

        GC.SuppressFinalize(this);
    }
}
