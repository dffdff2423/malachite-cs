// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using System.Drawing;

using Malachite.App.Graphics.SDL;
using Malachite.App.Graphics.SPIRV;
using Malachite.Core.Maths;
using Malachite.Core.Resources;

using SDL;

namespace Malachite.App.Graphics;

public sealed class GraphicsContext : IDisposable {
    private bool _disposed;

    private readonly ResourceLoader _loader;

    private readonly SDLApplication _app;
    private readonly SDLDevice _dev;
    private readonly SDLWindow _mainWindow;

    private readonly SDLGraphicsPipeline _trianglePipeline;

    public event Action? ShouldQuitApp;

    public GraphicsContext(ResourceLoader resourceLoader) {
        _loader = resourceLoader;
        _app = new SDLApplication();
        _dev = new SDLDevice();
        _mainWindow = _dev.CreateAssociatedWindow(ApplicationInfo.Name, new Vector2i(1440, 720));

        _app.ShouldQuit += () => {
            ShouldQuitApp?.Invoke();
        };

        _app.WindowShouldCLose += id => {
            if (id == _mainWindow.Id()) {
                ShouldQuitApp?.Invoke();
            }
        };

        var testvertBytes = _loader.GetBytes("spirv/triangle.vert.spv");
        var testvert = _dev.CreateShader(new ShaderFile
            { Code = testvertBytes, Stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX });

        var testfragBytes = _loader.GetBytes("spirv/triangle.frag.spv");
        var testfrag = _dev.CreateShader(new ShaderFile
            { Code = testfragBytes, Stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT });

        _trianglePipeline = _dev.CreateGraphicsPipeline(new SDLGraphicsPipelineCreateInfo {
            Vertex = testvert,
            Fragment = testfrag,
            ColorTargetDescriptions = [
                new SDL_GPUColorTargetDescription {
                    format = _dev.GetSwapchainFormat(_mainWindow),
                },
            ],
        });

        _dev.DisposeAllShaders();
    }

    public void Dispose() {
        if (_disposed)
            return;

        _dev.Dispose();
        _app.Dispose();

        _disposed = true;
    }

    public void ProcessEvents() {
        _app.ProcessEvents();
    }

    public void Draw() {
        var cmd = _dev.AcquireCommandBuffer();
        {
            var text = cmd.WaitAndAcquireSwapchainTexture(_mainWindow);
            var pass = cmd.BeginRenderPass([new RenderTargetSpec(Texture: text, ClearColor: Color.Aquamarine)]);
            {
                pass.BindPipeline(_trianglePipeline);
                pass.DrawPrimitives(3, 1, 0, 0);
            }
            pass.End();
        }
        cmd.Submit();
    }
}
