// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

#define NOMINMAX

#include <vector>
#include <assert.h>
#include "eventpipemetadatareader.h"
#include "profilerstring.h"

// Pretty prints a binary blob according to the description in an EventPipeMetadataInstance
class EventPipeEventPrinter
{
    LPCWCH TypeCodeToString(EventPipeTypeCode typeCode);

    void PrintIndentLevel(ULONG level);
    void PrintGuid(LPCGUID guid);

    bool PrintType(EventPipeDataDescriptor type,
                   ULONG indentLevel, // number of tabs to put in
                   LPCBYTE eventData,
                   ULONG cbEventData,
                   ULONG *offset);

    bool PrintParam(EventPipeDataDescriptor descriptor,
                    ULONG indentLevel, // number of tabs to put in
                    LPCBYTE eventData,
                    ULONG cbEventData,
                    ULONG *offset);

public:
    EventPipeEventPrinter();
    ~EventPipeEventPrinter() = default;

    void PrintEvent(LPCWSTR providerName,
                    EventPipeMetadataInstance metadata,
                    LPCBYTE eventData,
                    ULONG cbEventData,
                    LPCGUID pActivityId,
                    LPCGUID pRelatedActivityId,
                    ThreadID eventThread,
                    UINT_PTR stackFrames[],
                    ULONG numStackFrames);
};
