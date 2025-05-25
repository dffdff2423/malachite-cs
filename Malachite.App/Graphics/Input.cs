// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using SDL;

namespace Malachite.App.Graphics;

public sealed record KeyBind(SDL_Keycode Code, SDL_Keymod Mods = SDL_Keymod.SDL_KMOD_NONE);

public enum InputEvent {
    ShouldCloseApp
}

public static class Input {
    // TODO: Config system
    public static readonly Dictionary<KeyBind, InputEvent> KeyActions = new([
        KeyValuePair.Create(new KeyBind(SDL_Keycode.SDLK_ESCAPE), InputEvent.ShouldCloseApp)]);
}
