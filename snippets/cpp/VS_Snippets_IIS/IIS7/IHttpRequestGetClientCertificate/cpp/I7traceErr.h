// todo change this to event logging
void 			traceMessage(const char *msg);
void 			traceMessage( const wchar_t *msg);

#define CLEAR_TRACE_WIN(MSG) TRC_MSG("DBGVIEWCLEAR" << " DBG-VIEW-CLEAR " << MSG)

#define LOG_ERR(X) TRC_MSG("FATAL Error " << X << " function : " << __FUNCTION__ )
#define LOG_ERR_HR(X, HRX) LOG_ERR("HR = " << std::hex << HRX << " " << X)

#define TRC_MSG_FULL(MSG) TRC_MSG(MSG << " FILE = " \
	<< __FILE__ << " Build " << __TIMESTAMP__     ) 

#define TRC_MSGW_FULL(MSG) TRC_MSGW(MSG << L" FILE = " \
	<< __FILE__ << L" Build " << __TIMESTAMP__   )

#define TRC_HR_MSG(HRx, MSG) TRC_MSG("HR = " << std::hex << hr << "  " << MSG)

#define TRC_MSG(MSG) { std::ostringstream os;  \
	os << "Line: " << __LINE__             \
	<< " rMsg1638: " << MSG << std::endl;   \
	std::string str = os.str();            \
	traceMessage(str.c_str());            \
}

#define TRC_MSGW(MSG) { std::wostringstream os;  \
	os << L"Line: " << __LINE__             \
	<< L" rMsg1638: " << MSG << std::endl;   \
	std::wstring str = os.str();            \
	traceMessage(str.c_str());            \
}


#define CHK_HR_RTN(HR, MSG, RTN) if(FAILED(HR)) { \
	CHK_HR_LOG(HR, MSG)                  \
	return RTN;                          \
}                                        

// Only logs an error on FAIL
#define CHK_HR_LOG(HR, MSG) if(FAILED(HR)) { \
	std::ostringstream os;               \
	os << "rMsg1638 FAILURE at : " << __LINE__    \
	<< " HR = " << std::hex << HR        \
	<< "      " << MSG ;                 \
	std::string str = os.str();          \
	traceMessage(str.c_str());           \
}                                