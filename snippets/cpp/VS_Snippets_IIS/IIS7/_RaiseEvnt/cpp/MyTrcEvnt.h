#include "precomp.h"

// <snippet3>

class My_Events
{
public:
	static	LPCGUID	GetAreaGuid( VOID ){ //  GUID for the event class
		static const GUID AreaGuid = 
		{0xacade3b2,0xb7d7,0x4339,{0x95,0x6c,0x81,0x1b,0x4e,0xdb,0x1b,0x24}};
		return &AreaGuid;
	};

	static	LPCGUID	GetProviderGuid( VOID ){ // GUID for the event Provider
		static const GUID PrvderGuid = 
		// {EB881638-214A-4f2a-9B39-933770822D18}
	{ 0xeb881638, 0x214a, 0x4f2a, { 0x9b, 0x39, 0x93, 0x37, 0x70, 0x82, 0x2d, 0x18 } };
;
		return &PrvderGuid;
	};


	class My_COMPLETION
	{
	public:
		static	HRESULT	RaiseEvent(
			IHttpTraceContext * pHttpTraceContext,
			LONG InsertPosition
			)
			//
			// Raise Cmy_COMPLETION Event
			//
		{
			HTTP_TRACE_EVENT Event;
			Event.pProviderGuid = My_Events::GetProviderGuid();
			Event.dwArea = 1;
			Event.pAreaGuid = My_Events::GetAreaGuid();
			Event.dwEvent = 1;
			Event.pszEventName = L"NOTIFY_MY_CSTM_WECBR_EVNT";
			Event.dwEventVersion = 2;
			Event.dwVerbosity = 1;
			Event.cEventItems = 1;
			Event.pActivityGuid = NULL;
			Event.pRelatedActivityGuid = NULL;
			Event.dwTimeStamp = 0;
			Event.dwFlags = HTTP_TRACE_EVENT_FLAG_STATIC_DESCRIPTIVE_FIELDS;

			// pActivityGuid, pRelatedActivityGuid, Timestamp to be filled in by IIS

			HTTP_TRACE_EVENT_ITEM Items[ 1 ];
			Items[ 0 ].pszName = L"InsertPosition";
			Items[ 0 ].dwDataType = HTTP_TRACE_TYPE_LONG; // mof type (object)
#pragma warning (disable:4312)
			Items[ 0 ].pbData = (PBYTE) InsertPosition;
			Items[ 0 ].cbData = 4;
			Items[ 0 ].pszDataDescription = L"Insert Position";
			Event.pEventItems = Items;
			return pHttpTraceContext->RaiseTraceEvent( &Event );
		};

	};
};

// </snippet3>
