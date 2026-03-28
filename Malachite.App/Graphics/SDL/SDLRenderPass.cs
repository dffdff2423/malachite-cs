// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using SDL;
using static SDL.SDL3;

namespace Malachite.App.Graphics.SDL;

public sealed unsafe class SDLRenderPass(SDL_GPURenderPass* handle) {
    public void BindPipeline(SDLGraphicsPipeline pipeline) {
        SDL_BindGPUGraphicsPipeline(handle, pipeline.Handle);
    }

    public void DrawPrimitives(uint numVertices, uint numInstances, uint firstVertex, uint firstIndex) {
        SDL_DrawGPUPrimitives(handle, numVertices, numInstances, firstVertex, firstIndex);
    }

    public void End() {
        SDL_EndGPURenderPass(handle);
    }
}
