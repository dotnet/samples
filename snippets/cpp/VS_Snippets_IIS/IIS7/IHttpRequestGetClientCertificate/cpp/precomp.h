#ifndef __PRECOMP_H__
#define __PRECOMP_H__

// Include fewer Windows headers 
#define WIN32_LEAN_AND_MEAN

// std streams for tracing
#include <sstream>

//  IIS7 Server API header file
#include <httpserv.h>

//  Project header files 
#include "mymodule.h"
#include "mymodulefactory.h"

// Trace macro definitions for all modules
#include "I7traceErr.h"

// Globals from main.cpp
#ifndef __MAIN_G_
extern int                                 g_requestCnt;
extern  IHttpServer *                       g_pHttpServer ;
extern  IHttpModuleRegistrationInfo*                   g_m;
//extern PVOID                               g_pModuleContext;
#endif      // __MAIN_G_


#endif     // __PRECOMP_H__

