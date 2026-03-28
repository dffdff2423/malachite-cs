// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using System.Diagnostics;

using JetBrains.Annotations;

namespace Malachite.Core.Resources;

[PublicAPI]
public sealed class ResourceLoader {
    private FileInfo _resRoot;

    public ResourceLoader(FileInfo resRoot) {
        _resRoot = resRoot;
        var attr = _resRoot.Attributes;
        if (!attr.HasFlag(FileAttributes.Directory))
            throw new IOException($"Provided resource path root `{_resRoot}` is not a directory");
    }

    public byte[] GetBytes(ResPath path)
        => File.ReadAllBytes(Path.Join(_resRoot.Name, path.Path));
}

/// <summary>
/// Path in the resource tree
/// </summary>
[PublicAPI]
public sealed class ResPath {
    public string Path { get; init; }

    /// <summary>
    /// Create a ResPath from a path separated with /
    /// </summary>
    public ResPath(string path) {
        Path = path;
    }

    private void AssertVaid() {
        Debug.Assert(!Path.EndsWith('/'), message: "ResPaths should always point to a file and should not end in a /");
        Debug.Assert(!Path.StartsWith('/'), message: "ResPaths should not include a leading slash");
    }

    public static implicit operator ResPath(string path) => new(path);
}
