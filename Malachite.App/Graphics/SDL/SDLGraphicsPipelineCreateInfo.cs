// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using SDL;

namespace Malachite.App.Graphics.SDL;

public sealed record SDLGraphicsPipelineCreateInfo {
    public required SDLShader Vertex { get; init; }
    public required SDLShader Fragment { get; init; }

    public SDL_GPUPrimitiveType PrimitiveType { get; init; } = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST;

    public readonly static SDL_GPURasterizerState DefaultRasterizerState = new SDL_GPURasterizerState {
        fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_FILL,
        cull_mode = SDL_GPUCullMode.SDL_GPU_CULLMODE_NONE,
        front_face = SDL_GPUFrontFace.SDL_GPU_FRONTFACE_CLOCKWISE,
        enable_depth_bias = false,
        enable_depth_clip = false,
    };

    public SDL_GPURasterizerState RasterizerState { get; init; } = DefaultRasterizerState;

    public SDL_GPUMultisampleState MultisampleState { get; init; } = new() {
        sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
    };

    public SDL_GPUDepthStencilState DepthStencilState { get; init; } = new();

    public SDL_GPUTextureFormat? DepthStencilFormat { get; init; }

    public SDL_GPUColorTargetDescription[] ColorTargetDescriptions { get; init; } = [];

    // TODO: vertex buffers
}
