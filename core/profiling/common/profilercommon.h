// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

#define COM_NO_WINDOWS_H
#define NOMINMAX

#ifndef WIN32
#include "unix/profiler_defines.h"
#else // !WIN32
#include "windows/profiler_defines.h"
#endif // !WIN32

#include "cor.h"
#include "corprof.h"
#include "profilerstring.h"
