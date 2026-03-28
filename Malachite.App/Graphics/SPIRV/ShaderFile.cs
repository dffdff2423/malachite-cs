// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using SDL;

namespace Malachite.App.Graphics.SPIRV;

public sealed record ShaderFile {
    public required byte[] Code { get; init; }
    public required SDL_GPUShaderStage Stage { get; init; }

    public byte[] EntryPoint { get; init; } = "main"u8.ToArray();

    // TODO: reflect/load this stuff
    public uint NumSamplers { get; init; } = 0;
    public uint NumStorageTextures { get; init; } = 0;
    public uint NumStorageBuffers { get; init; } = 0;
    public uint NumUniformBuffers { get; init; } = 0;

    // We only support SPIRV/vulkan
    public SDL_GPUShaderFormat Format => SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV;
}
