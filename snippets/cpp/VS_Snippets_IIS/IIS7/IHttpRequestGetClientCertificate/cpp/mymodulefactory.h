#ifndef __MODULE_FACTORY_H__
#define __MODULE_FACTORY_H__

#include "I7traceErr.h"

extern int g_requestCnt;
// g_requestCnt is reset to zero on each new request

// Factory class for CMyHttpModule.
// This class is responsible for creating instances
// of CMyHttpModule for each request.
class CMyHttpModuleFactory : public IHttpModuleFactory
{
public:
	virtual
		HRESULT
		GetHttpModule(
		CHttpModule            **ppModule, 
		IModuleAllocator        *  // NotUsed
		)
	{
		HRESULT                    hr = S_OK;
		g_requestCnt = 0;         // reset the counter to zero

		//TRC_MSG_FULL(" CMyHttpModuleFactory");

		*ppModule = new CMyHttpModule();
		if ( *ppModule == NULL )
		{
			hr = HRESULT_FROM_WIN32( ERROR_NOT_ENOUGH_MEMORY );
			LOG_ERR_HR(hr,"ERROR_NOT_ENOUGH_MEMORY");
		}

		return hr;

	}

	virtual     void    Terminate()    {    }
};

#endif    /// #define __MODULE_FACTORY_H__
