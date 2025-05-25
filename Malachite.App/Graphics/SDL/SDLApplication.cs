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

    ~SDLApplication() {
        Dispose();
    }

    public void Dispose() {
        if (_disposed)
            return;

        SDL_Quit();

        _disposed = true;

        GC.SuppressFinalize(this);
    }

    public event Action? ShouldQuit;
    public event Action<SDL_WindowID>? WindowShouldCLose;

    /// <summary>
    /// Processes all SDL events, triggers any relevant events on this class.
    /// </summary>
    public unsafe void ProcessEvents() {
        SDL_Event evt;
        while (SDL_PollEvent(&evt)) {
            switch (evt.Type) {
            case SDL_EventType.SDL_EVENT_QUIT:
                ShouldQuit?.Invoke();
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                WindowShouldCLose?.Invoke(evt.window.windowID);
                break;
            case SDL_EventType.SDL_EVENT_KEY_UP:
                var key = evt.key.key;
                var mod = evt.key.mod;
                switch (Input.KeyActions[new KeyBind(key, mod)]) {
                case InputEvent.ShouldCloseApp:
                    ShouldQuit?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                }

                break;
            }
        }
    }
}
