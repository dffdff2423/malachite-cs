// SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
//
// SPDX-License-Identifier: GPL-3.0-only

// Needed because this file is used in the analyzer tests which do not have a global using for it.
using System;

namespace Malachite.Core.Annotations;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class VirtualAttribute : Attribute;
