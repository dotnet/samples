#include "precomp.h"

//  IIS7 Server API header file
#include <httpserv.h>

class My_Events
{
public:
    static
    LPCGUID
    GetAreaGuid( VOID )
    // return GUID for the current event class
    {
        static const GUID AreaGuid = 
          {0xacade3b2,0xb7d7,0x4339,{0x95,0x6c,0x81,0x1b,0x4e,0xdb,0x1b,0x24}};
        return &AreaGuid;
    };

	 static
    LPCGUID
    GetProviderGuid( VOID )
    // return GUID for the current event Provider
    {
        static const GUID PrvderGuid = 
          {0xdead1638,0xb666,0x4339,{0x95,0x69,0xde,0xad,0xbe,0xef,0x69,0x24}};
        return &PrvderGuid;
    };


class My_COMPLETION
    {
    public:
        static
        HRESULT
        RaiseEvent(
            IHttpTraceContext * pHttpTraceContext,
            LPCGUID    pContextId
        )
        //
        // Raise Cmy_COMPLETION Event
        //
        {
            HTTP_TRACE_EVENT Event;
			Event.pProviderGuid = My_Events::GetProviderGuid();
            // no areas defined
            Event.dwArea = 0;
			Event.pAreaGuid = My_Events::GetAreaGuid();
            Event.dwEvent = 3;
            Event.pszEventName = L"NOTIFY_MY_COMPLETION";
            Event.dwEventVersion = 1;
            Event.dwVerbosity = 4;
            Event.cEventItems = 1;
            Event.pActivityGuid = NULL;
            Event.pRelatedActivityGuid = NULL;
            Event.dwTimeStamp = 0;
            Event.dwFlags = HTTP_TRACE_EVENT_FLAG_STATIC_DESCRIPTIVE_FIELDS;
    
            // pActivityGuid, pRelatedActivityGuid, Timestamp to be filled in by IIS
    
            HTTP_TRACE_EVENT_ITEM Items[ 1 ];
            Items[ 0 ].pszName = L"ContextId";
            Items[ 0 ].dwDataType = HTTP_TRACE_TYPE_LPCGUID; // mof type (object)
            Items[ 0 ].pbData = (PBYTE) pContextId;
            Items[ 0 ].cbData = 16;
            Items[ 0 ].pszDataDescription = NULL;
            Event.pEventItems = Items;
            pHttpTraceContext->RaiseTraceEvent( &Event );
            return S_OK;
        };
    
    };
};