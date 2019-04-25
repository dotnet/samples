// todo change this to event logging
void 			traceMessage(const char *msg);
void 			traceMessage( const wchar_t *msg);

//#define TRACING_OFF

#ifndef TRACING_OFF

#define CLEAR_TRACE_WIN(MSG) TRC_MSG("DBGVIEWCLEAR" << " DBG-VIEW-CLEAR " << MSG)

#define LOG_ERR(X) TRC_MSG("FATAL Error " << X << " function : " << __FUNCTION__ )
#define LOG_ERR_HR(X, HRX) LOG_ERR("HR = " << std::hex << HRX << " " << X)

#define TRC_MSG_FULL(MSG) TRC_MSG(MSG << " FILE = " \
	<< __FILE__ << " Build " << __TIMESTAMP__     ) 

#define TRC_MSGW_FULL(MSG) TRC_MSGW(MSG << L" FILE = " \
	<< __FILE__ << L" Build " << __TIMESTAMP__   )

#define TRC_HR_MSG(HRx, MSG) TRC_MSG("HR = " << std::hex << hr << "  " << MSG)

#define TRC_MSGx(MSG) { std::ostringstream os;  \
	os << "Line: " << __LINE__                \
	 << " rMsg: " << MSG << std::endl;   \
	std::string _M_str = os.str();            \
	traceMessage(_M_str.c_str());            \
}

#define TRC_MSGwx(MSG) { std::wostringstream os;  \
	os << L"Line: " << __LINE__                \
	 << L" rMsg: " << MSG << std::endl;   \
	std::wstring _M_str = os.str();            \
	traceMessage(_M_str.c_str());            \
}

#define TRC_MSG(MSG) { std::ostringstream os;  \
	os << "Line: " << __LINE__             \
	<< " Fun: " << __FUNCTION__             \
	<< " rMsg: " << MSG << std::endl;   \
	std::string _M_str = os.str();            \
	traceMessage(_M_str.c_str());            \
}

#define TRC_MSGW(MSG) { std::wostringstream os;  \
	os << L"Line: " << __LINE__             \
	 << L" Fun: " << __FUNCTION__             \
	<< L" rMsg: " << MSG << std::endl;   \
	std::wstring _M_str = os.str();            \
	traceMessage(_M_str.c_str());            \
}


#define CHK_HR_RTN(HR, MSG, RTN) if(FAILED(HR)) { \
	CHK_HR_LOG(HR, MSG)                  \
	return RTN;                          \
}                                        

// Only logs an error on FAIL
#define CHK_HR_LOG(HR, MSG) if(FAILED(HR)) { \
	std::ostringstream os;               \
	os << "rMsg FAILURE at : " << __LINE__    \
	<< " HR = " << std::hex << HR        \
	<< "      " << MSG ;                 \
	std::string _M_str = os.str();          \
	traceMessage(_M_str.c_str());           \
}                           

#else
#define CLEAR_TRACE_WIN(MSG) ;
// Don't nuke Logging
#define LOG_ERR(X) TRC_MSG("FATAL Error " << X << " function : " << __FUNCTION__ )
#define LOG_ERR_HR(X, HRX) LOG_ERR("HR = " << std::hex << HRX << " " << X)

#define TRC_MSG_FULL(MSG) ;
#define TRC_MSGW_FULL(MSG) ;
#define TRC_HR_MSG(HRx, MSG) ;
#define TRC_MSGx(MSG) ;
#define TRC_MSG(MSG) ;
#define TRC_MSGW(MSG) ;
#define CHK_HR_LOG(HR, MSG) ;
#define TRC_MSGwx(X) ;
#endif