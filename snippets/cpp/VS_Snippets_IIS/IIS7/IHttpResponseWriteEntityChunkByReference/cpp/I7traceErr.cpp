#include "precomp.h"

void 			traceMessage(const char *msg){

#ifdef USING_ETW      // ETW is the preferred approach
	CallETW_API(msg)
#else
	OutputDebugStringA(msg);
#endif

}

void 			traceMessage(const wchar_t *msg){

#ifdef USING_ETW      // ETW is the preferred approach
	CallETW_API(msg)
#else
	OutputDebugStringW(msg);
#endif

}
