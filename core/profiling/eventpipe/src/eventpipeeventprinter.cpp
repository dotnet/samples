// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "eventpipeeventprinter.h"
#include <inttypes.h>

using std::vector;
using std::shared_ptr;

EventPipeEventPrinter::EventPipeEventPrinter()
{

}


LPCWCH EventPipeEventPrinter::TypeCodeToString(EventPipeTypeCode typeCode)
{
    switch (typeCode)
    {
        case EventPipeTypeCode::Object:
            return WCHAR("Object");
        case EventPipeTypeCode::Boolean:
            return WCHAR("Boolean");
        case EventPipeTypeCode::Char:
            return WCHAR("Char");
        case EventPipeTypeCode::Sbyte:
            return WCHAR("Sbyte");
        case EventPipeTypeCode::Byte:
            return WCHAR("Byte");
        case EventPipeTypeCode::Int16:
            return WCHAR("Int16");
        case EventPipeTypeCode::Uint16:
            return WCHAR("Uint16");
        case EventPipeTypeCode::Int32:
            return WCHAR("Int32");
        case EventPipeTypeCode::Uint32:
            return WCHAR("Uint32");
        case EventPipeTypeCode::Int64:
            return WCHAR("Int64");
        case EventPipeTypeCode::Uint64:
            return WCHAR("Uint64");
        case EventPipeTypeCode::Single:
            return WCHAR("Single");
        case EventPipeTypeCode::Double:
            return WCHAR("Double");
        case EventPipeTypeCode::Decimal:
            return WCHAR("Decimal");
        case EventPipeTypeCode::Datetime:
            return WCHAR("Datetime");
        case EventPipeTypeCode::Guid:
            return WCHAR("Guid");
        case EventPipeTypeCode::String:
            return WCHAR("String");
        case EventPipeTypeCode::ArrayType:
            return WCHAR("ArrayType");
        default:
            return WCHAR("Unknown Type");
    }
}

void EventPipeEventPrinter::PrintIndentLevel(ULONG level)
{
    for (ULONG i = 0; i < level; ++i)
    {
        printf("    ");
    }
}

// typedef struct _GUID {
//   unsigned long  Data1;
//   unsigned short Data2;
//   unsigned short Data3;
//   unsigned char  Data4[8];
// } GUID;
void EventPipeEventPrinter::PrintGuid(LPCGUID guid)
{
    if (guid == NULL)
    {
        printf("{NULL Guid}");
    }
    else
    {
        printf("{%8.8lu-%4.4u-%4.4u-%2.2u%2.2u-%2.2u%2.2u%2.2u%2.2u%2.2u%2.2u}",
               guid->Data1,
               guid->Data2,
               guid->Data3,
               guid->Data4[0],
               guid->Data4[1],
               guid->Data4[2],
               guid->Data4[3],
               guid->Data4[4],
               guid->Data4[5],
               guid->Data4[6],
               guid->Data4[7]);
    }
}


void EventPipeEventPrinter::PrintEvent(LPCWSTR providerName,
                                       EventPipeMetadataInstance metadata,
                                       LPCBYTE eventData,
                                       ULONG cbEventData,
                                       LPCGUID pActivityId,
                                       LPCGUID pRelatedActivityId,
                                       ThreadID eventThread,
                                       UINT_PTR stackFrames[],
                                       ULONG numStackFrames)
{
    wprintf(L"Saw EventPipe event from provider %s\n", providerName);
    PrintIndentLevel(1);
    wprintf(L"name: %s\n", metadata.name);
    PrintIndentLevel(1);
    printf("eventId: %d\n", metadata.id);
    PrintIndentLevel(1);
    printf("eventLevel: %d\n", metadata.level);
    PrintIndentLevel(1);
    printf("eventKeywords: %" PRIi64 "\n", metadata.keywords);
    PrintIndentLevel(1);
    printf("eventVersion: %d\n", metadata.version);
    PrintIndentLevel(1);
    printf("eventOpcode: %d\n", metadata.opcode);
    PrintIndentLevel(1);
    printf("activityID: ");
    PrintGuid(pActivityId);
    printf("\n");
    PrintIndentLevel(1);
    printf("relatedActivityID: ");
    PrintGuid(pActivityId);
    printf("\n");
    PrintIndentLevel(1);
    printf("event ThreadID: %p\n", (void *)eventThread);
    PrintIndentLevel(1);
    printf("numStackFrames: %lu\n", numStackFrames);
    PrintIndentLevel(1);
    printf("stackFrames %p\n", (void *)stackFrames);

    PrintIndentLevel(1);
    printf("number of event params: %d\n", (int)metadata.parameters.size());
    PrintIndentLevel(1);
    printf("event params:\n");

    ULONG offset = 0;
    for (auto &&descriptor : metadata.parameters)
    {
        if (!PrintParam(descriptor, 2, eventData, cbEventData, &offset))
        {
            return;
        }
    }
}

bool EventPipeEventPrinter::PrintType(EventPipeDataDescriptor type,
                                      ULONG indentLevel, // number of tabs to put include
                                      LPCBYTE eventData,
                                      ULONG cbEventData,
                                      ULONG *offset)
{
    switch (type.type)
    {
        case EventPipeTypeCode::Boolean:
        {
            // Bools are encoded as 4 bytes
            UINT32 data = ReadFromBuffer<UINT32>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Boolean value=%s\n", data != 0 ? "true" : "false");
        }
        break;

        case EventPipeTypeCode::Char:
        {
            WCHAR data = ReadFromBuffer<WCHAR>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Char value=%c\n", data);
        }
        break;

        case EventPipeTypeCode::Sbyte:
        {
            INT8 data = ReadFromBuffer<INT8>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Sbyte value=%d\n", data);
        }
        break;

        case EventPipeTypeCode::Byte:
        {
            BYTE data = ReadFromBuffer<BYTE>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Byte value=%d\n", data);
        }
        break;

        case EventPipeTypeCode::Int16:
        {
            INT16 data = ReadFromBuffer<INT16>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Int16 value=%d\n", data);
        }
        break;

        case EventPipeTypeCode::Uint16:
        {
            UINT16 data = ReadFromBuffer<UINT16>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Uint16 value=%d\n", data);
        }
        break;

        case EventPipeTypeCode::Int32:
        {
            INT32 data = ReadFromBuffer<INT32>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Int32 value=%d\n", data);
        }
        break;

        case EventPipeTypeCode::Uint32:
        {
            UINT32 data = ReadFromBuffer<UINT32>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Uint32 value=%d\n", data);
        }
        break;

        case EventPipeTypeCode::Int64:
        {
            INT64 data = ReadFromBuffer<INT64>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Int64 value=%" PRIi64 "\n", data);
        }
        break;

        case EventPipeTypeCode::Uint64:
        {
            UINT64 data = ReadFromBuffer<UINT64>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Uint64 value=%" PRIu64 "\n", data);
        }
        break;

        case EventPipeTypeCode::Single:
        {
            float data = ReadFromBuffer<float>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Single value=%f\n", data);
        }
        break;

        case EventPipeTypeCode::Double:
        {
            double data = ReadFromBuffer<double>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            printf("type Double value=%f\n", data);
        }
        break;

        case EventPipeTypeCode::String:
        {
            WCHAR *data = ReadWideStringFromBuffer(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            wprintf(L"type String value=\"%s\"\n", data);
        }
        break;

        case EventPipeTypeCode::ArrayType:
        {
            UINT16 arrayLength = ReadFromBuffer<UINT16>(eventData, cbEventData, offset);
            PrintIndentLevel(indentLevel);
            wprintf(L"Array length=%d param type=%s values:\n", arrayLength, TypeCodeToString(type.elementType->type));
            for (int i = 0; i < arrayLength; ++i)
            {
                if (!PrintType(*(type.elementType),
                               indentLevel + 1,
                               eventData,
                               cbEventData,
                               offset))
                {
                    return false;
                }
            }

            PrintIndentLevel(indentLevel);
            printf("End of array values\n");
        }
        break;

        case EventPipeTypeCode::Object:
        {
            PrintIndentLevel(indentLevel);
            printf("Self describing object param fields:\n");
            for (auto &&fieldDescriptor : type.fields)
            {
                if (!PrintType(fieldDescriptor,
                               indentLevel + 1,
                               eventData,
                               cbEventData,
                               offset))
                {
                    return false;
                }
            }
        }
        break;

        case EventPipeTypeCode::Decimal:
        case EventPipeTypeCode::Datetime:
        case EventPipeTypeCode::Guid:
        default:
        {
            PrintIndentLevel(indentLevel);
            wprintf(L"Unsupported param type code %s\n", TypeCodeToString(type.type));
            return false;
        }
    }

    return true;
}

bool EventPipeEventPrinter::PrintParam(EventPipeDataDescriptor descriptor,
                                       ULONG indentLevel, // number of tabs to put in
                                       LPCBYTE eventData,
                                       ULONG cbEventData,
                                       ULONG *offset)
{
    PrintIndentLevel(indentLevel);
    wprintf(L"Param name: %s\n", descriptor.name);

    return PrintType(descriptor,
                     indentLevel + 1,
                     eventData,
                     cbEventData,
                     offset);
}
