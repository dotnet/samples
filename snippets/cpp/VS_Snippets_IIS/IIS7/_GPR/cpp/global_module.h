#ifndef _GLOBAL_MODULE_H_
#define _GLOBAL_MODULE_H_

class GLOBAL_MODULE : public CGlobalModule{

public:
    GLOBAL_MODULE(VOID) { }

    virtual 
    GLOBAL_NOTIFICATION_STATUS
    OnGlobalPreBeginRequest(
        IPreBeginRequestProvider* pProvider
    );

    HRESULT Initialize(VOID);

    virtual VOID Terminate(VOID);
};

#endif
