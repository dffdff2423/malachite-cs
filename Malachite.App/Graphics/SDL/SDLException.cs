// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using System.Diagnostics.Contracts;

using static SDL.SDL3;

namespace Malachite.App.Graphics.SDL;

/// <summary>
/// An exception representing an error from SDL.
/// </summary>
public sealed class SDLException : Exception {
    private SDLException(string message) : base(message) { }


    /// <summary>
    /// Creates a SDLException of the last error, or the text "No SDL Error" if there is no one.
    /// </summary>
    /// <returns>The created exception</returns>
    [Pure]
    public static SDLException FromLastError(string context) {
        var message = SDL_GetError();
        SDL_ClearError();
        return new SDLException($"{context}: {message ?? "No SDL Error"}");
    }
}
