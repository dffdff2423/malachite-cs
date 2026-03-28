// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using SDL;

namespace Malachite.App.Graphics.SDL;

public sealed unsafe class SDLGraphicsPipeline {
    public required SDL_GPUGraphicsPipeline* Handle { get; init; }
}
