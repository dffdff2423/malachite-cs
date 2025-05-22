// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

namespace Malachite.App;

/// <summary>
/// Global application info
/// </summary>
// TODO: Localization
public static class ApplicationInfo {
    public const string Name = "Untitiled Factory Game";

    public const string Version = "indev";

    public const string Identifier = "io.github.dffdff2423.malachite";

    public const string Url = "https://github.com/dffdff2423/malachite-cs";


#if DEBUG
    public const bool Debug = true;
#else
    public const bool Debug = false;
#endif
}
