// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using Malachite.App.Graphics.SPIRV;
using Malachite.Core.Maths;

using SDL;

using static SDL.SDL3;

namespace Malachite.App.Graphics.SDL;

/// <summary>
/// Managed wrapper around a <see cref="SDL_GPUDevice"/>
/// </summary>
public sealed unsafe class SDLDevice : IDisposable {
    private bool _disposed;
    private SDL_GPUDevice* _handle;
    private readonly List<SDLWindow> _associatedWindows = [];
    // We just let the device clean up pipelines/shaders for now. This is not a AAA game.
    private readonly List<SDLShader> _shaders = [];
    private readonly List<SDLGraphicsPipeline> _graphicsPipelines = [];

    /// <summary>
    /// Creates a device using SDL API calls
    /// </summary>
    /// <param name="availableShaders">List of available shaders</param>
    /// <exception cref="SDLException">Upon failing to create the device</exception>
    public SDLDevice(SDL_GPUShaderFormat availableShaders = SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV) {
        // We only allow vulkan here because I only have a linux system so I can't test other APIs.
        _handle = SDL_CreateGPUDevice(availableShaders, ApplicationInfo.Debug, "vulkan");
        if (_handle == null)
            throw SDLException.FromLastError("Failed to create Device");
    }

    // ReSharper disable once UnusedParameter.Local
    private void Dispose(bool disposing) {
        if (_disposed)
            return;

        foreach (var pipe in _graphicsPipelines) {
            SDL_ReleaseGPUGraphicsPipeline(_handle, pipe.Handle);
        }

        DisposeAllShaders();

        foreach (var win in _associatedWindows) {
            SDL_ReleaseWindowFromGPUDevice(_handle, win.Handle);
        }

        SDL_DestroyGPUDevice(_handle);
        _handle = null;

        if (disposing) {
            foreach (var win in _associatedWindows) {
                win.Dispose();
            }
        }

        _disposed = true;
    }

    ~SDLDevice() {
        Dispose(disposing: false);
    }

    public void Dispose() {
        Dispose(disposing: true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Creates a window with the provided information, associates it to this device, and creates a swapchain.
    /// </summary>
    /// <returns>Returns the newly bound window. <b>This Window must not be disposed</b>. It is cleaned up automatically when SDLDevice is disposed</returns>
    public SDLWindow CreateAssociatedWindow(string title, Vector2i extent, SDL_WindowFlags flags = 0) {
        var window = new SDLWindow(title, extent, flags);
        _associatedWindows.Add(window);
        if (!SDL_ClaimWindowForGPUDevice(_handle, window.Handle))
            throw SDLException.FromLastError("Failed to bind window to device");

        return window;
    }

    public SDL_GPUTextureFormat GetSwapchainFormat(SDLWindow win)
        => SDL_GetGPUSwapchainTextureFormat(_handle, win.Handle);

    public SDLCommandBuffer AcquireCommandBuffer() {
        var buf = SDL_AcquireGPUCommandBuffer(_handle);
        if (buf == null)
            throw SDLException.FromLastError("Failed to acquire command buffer");
        return new SDLCommandBuffer(buf);
    }

    public SDLShader CreateShader(ShaderFile file) {
        fixed (byte* data = file.Code, entryPoint = file.EntryPoint) {
            var info = new SDL_GPUShaderCreateInfo {
                code_size = (UIntPtr)file.Code.Length,
                code = data,
                entrypoint = entryPoint,
                format = file.Format,
                num_samplers = file.NumSamplers,
                num_storage_buffers = file.NumStorageBuffers,
                num_storage_textures = file.NumStorageTextures,
                num_uniform_buffers = file.NumUniformBuffers,
                props = 0,
                stage = file.Stage,
            };
            var handle = SDL_CreateGPUShader(_handle, &info);
            if (handle == null)
                throw SDLException.FromLastError("Failed to create shader");
            var shader = new SDLShader { Handle = handle };
            _shaders.Add(shader);
            return shader;
        }
    }

    /// <summary>
    /// All shader objects will be invalid after this is called. This method exists since we don't need them anymore
    /// after pipelines are created
    /// </summary>
    public void DisposeAllShaders() {
        foreach (var shader in _shaders) {
            SDL_ReleaseGPUShader(_handle, shader.Handle);
        }
        _shaders.Clear();
    }

    public SDLGraphicsPipeline CreateGraphicsPipeline(SDLGraphicsPipelineCreateInfo info) {
        fixed (SDL_GPUColorTargetDescription* descr = info.ColorTargetDescriptions) {
            var nativeInfo = new SDL_GPUGraphicsPipelineCreateInfo {
                vertex_shader = info.Vertex.Handle,
                fragment_shader = info.Fragment.Handle,
                primitive_type = info.PrimitiveType,
                rasterizer_state = info.RasterizerState,
                multisample_state = info.MultisampleState,
                depth_stencil_state = info.DepthStencilState,
                target_info = new SDL_GPUGraphicsPipelineTargetInfo {
                    color_target_descriptions = descr,
                    num_color_targets = (uint)info.ColorTargetDescriptions.Length,
                    has_depth_stencil_target = info.DepthStencilFormat != null,
                    depth_stencil_format = info.DepthStencilFormat ?? 0,
                },
            };
            var handle = SDL_CreateGPUGraphicsPipeline(_handle, &nativeInfo);
            if (handle == null)
                throw SDLException.FromLastError("Failed to create graphics pipeline");
            var pipe = new SDLGraphicsPipeline { Handle = handle };
            _graphicsPipelines.Add(pipe);
            return pipe;
        }
    }
}
