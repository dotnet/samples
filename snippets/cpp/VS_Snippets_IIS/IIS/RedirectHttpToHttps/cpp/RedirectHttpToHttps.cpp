// <Snippet1>
#define _WIN32_WINNT 0x0400

#include <windows.h>
#include <httpfilt.h>

#define BUFFER_SIZE 2048

BOOL WINAPI GetFilterVersion(PHTTP_FILTER_VERSION pVer)
{
	// Specify the filter version and description.
	pVer->dwFilterVersion = HTTP_FILTER_REVISION;
	lstrcpy(pVer->lpszFilterDesc, "RedirectHttpToHttps");
	// Specify the filter notifications.
	pVer->dwFlags = SF_NOTIFY_ORDER_HIGH | SF_NOTIFY_PREPROC_HEADERS;

	return TRUE;
}

DWORD WINAPI HttpFilterProc(PHTTP_FILTER_CONTEXT pfc, DWORD NotificationType, LPVOID pvNotification )
{
    if (NotificationType == SF_NOTIFY_PREPROC_HEADERS)
    {
		char szServerName[BUFFER_SIZE] = "";
		char szSecure[2] = "";
		char szLocationHeader[BUFFER_SIZE + 32] = "";
		char szRequest[BUFFER_SIZE] = "";
		DWORD dwBuffSize = 0;
    
	    // Determine if request was sent over a secure port.
		dwBuffSize = 2;
		pfc->GetServerVariable(
			pfc, "SERVER_PORT_SECURE",
			szSecure, &dwBuffSize);

		// If the request is on a secure port, do not process further.
		if (szSecure[0] == '1')
			return SF_STATUS_REQ_NEXT_NOTIFICATION;

		// Retrieve the URL for the request.
		dwBuffSize = BUFFER_SIZE;
		pfc->GetServerVariable(
			pfc, "URL",
			szRequest, &dwBuffSize);

		// Retrieve the server name.
		dwBuffSize = BUFFER_SIZE;
		pfc->GetServerVariable(
			pfc, "SERVER_NAME",
			szServerName, &dwBuffSize);
		
		// Specify the redirection header.
		wsprintf(
			szLocationHeader, "Location: https://%s/%s\r\n\r\n",
			szServerName, &szRequest[1]);
		pfc->AddResponseHeaders(
			pfc, szLocationHeader, 0);
		pfc->ServerSupportFunction(
			pfc, SF_REQ_SEND_RESPONSE_HEADER, "302 Object Moved",
			(DWORD)"Please resubmit the request using a secure port.", 0);

		return SF_STATUS_REQ_FINISHED;
	}
    
	return SF_STATUS_REQ_NEXT_NOTIFICATION;
}
// </Snippet1>

// ================================================================================

//DWORD WINAPI HttpFilterProc(PHTTP_FILTER_CONTEXT pfc, DWORD NotificationType, LPVOID pvNotification )
//{
//    switch ( NotificationType )
//    {
//
//		case SF_NOTIFY_PREPROC_HEADERS:
//
//	char szServerName[BUFFER_SIZE] = "";
//	char szSecure[2] = "";
//	char szLocationHeader[BUFFER_SIZE + 32] = "";
//	char szRequest[BUFFER_SIZE] = "";
//	DWORD dwBuffSize = 0;
//    
//			// Retrieve the url for the request.
//			dwBuffSize = BUFFER_SIZE;
//			pfc->GetServerVariable(
//				pfc, "URL", szRequest, &dwBuffSize);
//
//		    // Determine if request was sent over a secure port.
//			dwBuffSize = 2;
//			pfc->GetServerVariable(
//				pfc, "SERVER_PORT_SECURE", szSecure, &dwBuffSize);
//
//			// If the request is on a secure port, do not process further.
//			if (szSecure[0] == '1')
//				return SF_STATUS_REQ_NEXT_NOTIFICATION;
//
//			// Retrieve the server name.
//			dwBuffSize = BUFFER_SIZE;
//			pfc->GetServerVariable(
//				pfc, "SERVER_NAME", szServerName, &dwBuffSize);
//			
//			// Specify the redirection header.
//			wsprintf(
//				szLocationHeader, "Location: https://%s/%s\r\n\r\n",
//				szServerName, &szRequest[1]);
//			pfc->AddResponseHeaders(
//				pfc, szLocationHeader, 0);
//			pfc->ServerSupportFunction(
//				pfc, SF_REQ_SEND_RESPONSE_HEADER, "302 Object Moved",
//				(DWORD)"Please resubmit the request using a secure port.", 0);
//
//			return SF_STATUS_REQ_FINISHED;
//
//		default:
//			break;
//	}
//    
//	return SF_STATUS_REQ_NEXT_NOTIFICATION;
//}

// ================================================================================
