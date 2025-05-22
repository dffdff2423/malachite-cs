// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

using JetBrains.Annotations;

namespace Malachite.Core.Annotations;

[PublicAPI]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class VirtualAttribute : Attribute;
