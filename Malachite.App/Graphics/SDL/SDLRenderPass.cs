// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using SDL;
using static SDL.SDL3;

namespace Malachite.App.Graphics.SDL;

public sealed unsafe class SDLRenderPass(SDL_GPURenderPass* handle) {
    public void End() {
        SDL_EndGPURenderPass(handle);
    }
}
