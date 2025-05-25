// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using System.Drawing;

using SDL;
using static SDL.SDL3;

namespace Malachite.App.Graphics.SDL;

/// <summary>
/// A managed wrapper around a <see cref="SDL_GPUCommandBuffer"/>. This class should be created with <see cref="SDLDevice.AcquireCommandBuffer"/>
/// </summary>
/// <remarks>
/// Command buffers should only be used and submitted on the thread they were acquired from.
/// </remarks>
public sealed unsafe class SDLCommandBuffer(SDL_GPUCommandBuffer* handle) {

    /// <summary>
    /// Submits the provided command buffer. After a buffer is submitted it is no longer valid. to write to it.
    /// </summary>
    /// <remarks>This must be called from the same thread <see cref="SDLDevice.AcquireCommandBuffer"/> was called from</remarks>
    /// <exception cref="SDLException">Upon failing to submit the command buffer</exception>
    public void Submit() {
        if (!SDL_SubmitGPUCommandBuffer(handle))
            throw SDLException.FromLastError("Failed to submit command buffer");
    }

    /// <summary>
    /// Acquire the write-only swapchain texture.
    /// </summary>
    /// <remarks>This function must be called on the same thread as the window was created on</remarks>
    public SDLTexture WaitAndAcquireSwapchainTexture(SDLWindow win) {
        SDL_GPUTexture* text;
        if (!SDL_WaitAndAcquireGPUSwapchainTexture(handle, win.Handle, &text, null, null))
            throw SDLException.FromLastError("Failed to acquire swapchain texture");
        return new SDLTexture(text);
    }

    public SDLRenderPass BeginRenderPass(ReadOnlySpan<RenderTargetSpec> specs) {
        var infos = stackalloc SDL_GPUColorTargetInfo[specs.Length];
        for (int i = 0; i < specs.Length; ++i) {
            // TODO: make this less bad and handle resolves
            infos[i].texture = specs[i].Texture.Handle;
            infos[i].mip_level = specs[i].MipLevel;
            infos[i].resolve_mip_level = specs[i].LayerOrDepthPlane;
            infos[i].cycle = specs[i].Cycle;

            if (specs[i].ClearColor != null) {
                infos[i].load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR;
                infos[i].clear_color.r = specs[i].ClearColor!.Value.R / 255.0f;
                infos[i].clear_color.g = specs[i].ClearColor!.Value.G / 255.0f;
                infos[i].clear_color.b = specs[i].ClearColor!.Value.B / 255.0f;
                infos[i].clear_color.a = specs[i].ClearColor!.Value.A / 255.0f;
            } else {
                infos[i].load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_DONT_CARE;
            }

            infos[i].store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE;
        }

        var pass = SDL_BeginGPURenderPass(handle, infos, (uint)specs.Length, null);
        if (pass == null)
            throw SDLException.FromLastError("Failed to start render pass");
        return new SDLRenderPass(pass);
    }
}

/// <summary>
/// Represents a target of a render pass
/// </summary>
/// <param name="Texture">Texture to render to</param>
/// <param name="ClearColor">Color to clear the texture to, or null to not clear</param>
/// <param name="MipLevel">Mip level</param>
/// <param name="LayerOrDepthPlane">Layer or depth plane of the texture</param>
/// <param name="Cycle">If the texture should be cycled if it is in-use</param>
public sealed record RenderTargetSpec(
    SDLTexture Texture,
    Color? ClearColor = null,
    uint MipLevel = 0,
    uint LayerOrDepthPlane = 0,
    bool Cycle = false);
