#include <jni.h>

char *(*incrementHandlerPtr)(void);

JNIEXPORT jstring JNICALL
Java_net_dot_MainActivity_sayHello(
        JNIEnv* env,
        jobject thisObject) {
    char *s = "Hello";
    if (incrementHandlerPtr)
        s = incrementHandlerPtr();
    return (*env) -> NewStringUTF(env, s);
}

static JavaVM *gJvm;

JNIEXPORT jint JNICALL
JNI_OnLoad(JavaVM *vm, void *reserved)
{
    (void)reserved;
    gJvm = vm;
    return JNI_VERSION_1_6;
}

static JNIEnv* GetJniEnv()
{
    JNIEnv *env;
    (*gJvm)->GetEnv(gJvm, (void**)&env, JNI_VERSION_1_6);
    if (env)
        return env;
    (*gJvm)->AttachCurrentThread(gJvm, &env, NULL);
    return env;
}

void
androidRegisterCounterIncrement (void* ptr)
{
    incrementHandlerPtr = ptr;
}

int myNum() {
    JNIEnv* jniEnv = GetJniEnv();

    jclass mainActivity = (*jniEnv)->FindClass (jniEnv, "net/dot/MainActivity");
    jmethodID getNum = (*jniEnv)->GetStaticMethodID(jniEnv, mainActivity, "getNum", "()I");
    jint val = (*jniEnv)->CallStaticIntMethod(jniEnv, mainActivity, getNum);
    return val;
}