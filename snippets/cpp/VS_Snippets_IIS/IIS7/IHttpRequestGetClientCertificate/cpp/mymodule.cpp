#include "precomp.h"
//34567890123456789012345678901234567890123456789012345678901234567890
//                20        30        40        50        60        70
//34567890123456789012345678901234567890123456789012345678901234567890
//
// You must set client certificates to either accept or require
// from  the SSL Settings page
//
unsigned long genCRC(unsigned char *data_blk_ptr, int data_blk_size);

//#define __snippet1__ 

#ifndef __snippet1__ 

// <snippet2>
void   checkForClientCert(IHttpContext*  pHttpContext)
{
   static long cnt;     
   // keep track of how many times we are called
   InterlockedIncrement (&cnt);  
   HRESULT hr = S_OK;
   IHttpRequest *pIHTTPR = pHttpContext->GetRequest();
   HTTP_REQUEST * pRawRequest = pIHTTPR->GetRawHttpRequest();
   HTTP_COOKED_URL hcu = pRawRequest->CookedUrl;

   // Send URL and count to the trace window
   TRC_MSGW_FULL(L"cnt = " << cnt << " URI: " << hcu.pFullUrl);

   // return immediately if not a HTTPS request
   if ( pRawRequest->pSslInfo == NULL ){
      TRC_MSG( "connection is not using SSL");
      return;
   }

   HTTP_SSL_CLIENT_CERT_INFO *pClientCertInfo=NULL;
   BOOL fccNeg;
   hr = pIHTTPR->GetClientCertificate(&pClientCertInfo,&fccNeg);

   // If you have not selected "Require Client Certificates" or called
   // NegotiateClientCertificate(), you  may get  ERROR_NOT_FOUND 

   if( hr == HRESULT_FROM_WIN32(ERROR_NOT_FOUND) ||
      pClientCertInfo == NULL ){
         TRC_HR_MSG(hr, "Cert not found" );
         return;
   }
   if(FAILED(hr)){
      LOG_ERR_HR("GetClientCertificate", hr);
      return;
   }	

   // You must verify pClientCertInfo != NULL

   if( fccNeg && pClientCertInfo != NULL){
      ULONG uSiz = pClientCertInfo->CertEncodedSize;
      TRC_MSG( "cert size: " << uSiz \
         << " Previously negotiated " << fccNeg );
      // compute the CRC check sum of the certificate
      unsigned long certCrc = genCRC(pClientCertInfo->pCertEncoded,
         pClientCertInfo->CertEncodedSize);
      TRC_MSG( "cert crc: " << certCrc );
   }
   else
      TRC_MSG( "No client certificate. fccNeg = " << fccNeg );
}

REQUEST_NOTIFICATION_STATUS
CMyHttpModule::OnBeginRequest(
                              IHttpContext*       pHttpContext,
                              IHttpEventProvider* // pProvider  
                              )
{
   checkForClientCert(pHttpContext);
   return RQ_NOTIFICATION_CONTINUE;
}

// </snippet2>
#else                       // #ifndef __snippet1__

// <snippet1>
REQUEST_NOTIFICATION_STATUS
CMyHttpModule::OnBeginRequest(
                              IHttpContext*       pHttpContext,
                              IHttpEventProvider* // pProvider  
                              )
{
   static long cnt;     
   InterlockedIncrement (&cnt);  // keep track of how many times we are called
   HRESULT hr = E_FAIL;
   IHttpRequest *pIHTTPR = pHttpContext->GetRequest();

   HTTP_REQUEST * pRawRequest = pIHTTPR->GetRawHttpRequest();
   HTTP_COOKED_URL hcu = pRawRequest->CookedUrl;

   // Send URL and count to the trace window
   TRC_MSGW_FULL( L"cnt = " << cnt << " URI: " << hcu.pFullUrl );

   // return immediately if not a HTTPS request
   if ( pRawRequest->pSslInfo == NULL ){
      TRC_MSG( "connection is not using SSL");
      return RQ_NOTIFICATION_CONTINUE;
   }

   // change the bForce flag to test behavior of forcing load of client cert
   bool bForce=true;
   if(bForce){
      TRC_MSG("forcing load of client cert");
      hr = pIHTTPR->NegotiateClientCertificate(FALSE);
      if(FAILED(hr)){
         LOG_ERR_HR(hr, "NegotiateClientCertificate : HR = ");
         return RQ_NOTIFICATION_CONTINUE;
      }
   }
   else
      TRC_MSG("Not forcing load of client cert");

   HTTP_SSL_CLIENT_CERT_INFO *pClientCertInfo;
   BOOL fccNeg;
   hr = pIHTTPR->GetClientCertificate(&pClientCertInfo,&fccNeg);

   // If you have not selected "Require Client Certificates" or called
   // NegotiateClientCertificate(), you  may get  ERROR_NOT_FOUND 

   if( hr == HRESULT_FROM_WIN32(ERROR_NOT_FOUND)){
      TRC_MSG( "Cert not found" );
      return RQ_NOTIFICATION_CONTINUE;
   }
   if(FAILED(hr)){
      LOG_ERR_HR("GetClientCertificate", hr);
      return RQ_NOTIFICATION_CONTINUE;
   }	

   if( fccNeg && pClientCertInfo != NULL){
      ULONG uSiz = pClientCertInfo->CertEncodedSize;
      TRC_MSG( "cert size: " << uSiz  \
         << " Previously negotiated " << fccNeg );
      unsigned long certCrc = genCRC(pClientCertInfo->pCertEncoded,
         pClientCertInfo->CertEncodedSize);
      TRC_MSG( "cert crc: " << certCrc );
   }
   else
      TRC_MSG( "No client certificate.");

   return RQ_NOTIFICATION_CONTINUE;
}

// </snippet1> 

#endif  // #ifndef __snippet1__



///
/// Random notes
/*

cannot get client cert without https (makes sense)
clear SSL state on IE doesn't seem to change things (I cleared IE ssl cache, restarted IIS and app pool and it still
loads a cert - even when not forced as long as I use SSL).

On debugView, you must Capture\Captue Global Win32
http://www.iis.net/default.aspx?tabid=2&subtabid=25&i=938&p=3
In order to install a native module, you have several options:

Use APPCMD.EXE command line tool
APPCMD makes module installation simple. Go to Start>Programs>Accessories, 
Right click on the Command Line Prompt, and choose “Run As Administrator”. 
In the command line window, execute the following:
%systemroot%\system32\inetsrv\appcmd.exe install module /name:MyModule /image:[FULL_PATH_TO_DLL]
Where [FULL_PATH_TO_DLL] is the full path to the compiled DLL containing module you just built.

Use the IIS7 Administration Tool 
This enables you to add a module by using a GUI interface. 
Go to Start>Run, type in inetmgr, and press Enter. 
Connect to localhost, locate the Modules task, and double-click to open it. 
Then, click the “Add a Native Module” task on the right pane. 

Install the module manually
You can install the module manually, by adding it to the <system.webServer>/<globalModules> configuration section in applicationHost.config configuration file, and adding a reference to it in the <system.webServer>/<modules> configuration section in the same file in order to enable it. We recommend that you use one of the previous two options to install the module instead of editing the configuration directly.


*/
