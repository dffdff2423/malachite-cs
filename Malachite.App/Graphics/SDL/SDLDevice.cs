// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

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

        SDL_DestroyGPUDevice(_handle);
        _handle = null;

        foreach (var win in _associatedWindows) {
            SDL_ReleaseWindowFromGPUDevice(_handle, win.Handle);
        }

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

    public SDLCommandBuffer AcquireCommandBuffer() {
        var buf = SDL_AcquireGPUCommandBuffer(_handle);
        if (buf == null)
            throw SDLException.FromLastError("Failed to acquire command buffer");
        return new SDLCommandBuffer(buf);
    }
}
