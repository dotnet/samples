#ifndef __MY_MODULE_H__
#define __MY_MODULE_H__

//  The module definition. implementation in .cpp file  // ricka declaration , not defn
//  This class is responsible for implementing the 
//  module functionality for each of the server events
//  that it registers for.
class CMyHttpModule : public CHttpModule
{
public:
	REQUEST_NOTIFICATION_STATUS
    OnBeginRequest(
      IHttpContext*       pHttpContext,
   IHttpEventProvider* pProvider
		);


	//  TODO: override additional event handler methods below.
};

#endif


