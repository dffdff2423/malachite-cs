// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using System.Diagnostics;

using JetBrains.Annotations;

namespace Malachite.Core.Resources;

[PublicAPI]
public sealed class ResourceLoader {
    public ResourceLoader(FileInfo resRoot) {
        _resRoot = resRoot;
        var attr = _resRoot.Attributes;
        if (!attr.HasFlag(FileAttributes.Directory))
            throw new IOException($"Provided resource path root `{_resRoot}` is not a directory");
    }

    private FileInfo _resRoot;

}

/// <summary>
/// Path in the resource tree
/// </summary>
[PublicAPI]
public sealed class ResPath {
    /// <summary>
    /// Create a ResPath from a path separated with /
    /// </summary>
    public ResPath(string path) {
        Debug.Assert(!path.EndsWith('/'), message: "ResPaths should always point to a file and should not end in a /");
        Segments = path.Split('/');
        Debug.Assert(Segments.Length > 0);
    }

    /// <summary>
    /// Create a ResPath from segments
    /// </summary>
    public ResPath(string[] segments) {
        Segments = segments;
        Debug.Assert(Segments.Length > 0);
    }

    public static implicit operator ResPath(string path) => new(path);

    /// <summary>
    /// The last segment of the path
    /// </summary>
    public string Filename => Segments.Last();

    /// <summary>
    /// The segments that make up the ResPath
    /// </summary>
    public readonly string[] Segments;
}
