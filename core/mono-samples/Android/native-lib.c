#include <jni.h>

char *(*incrementHandlerPtr)(void);
char *(*greetHandlerPtr)(const char *);

JNIEXPORT jstring JNICALL
Java_net_dot_MainActivity_incrementCounter(
        JNIEnv* env,
        jobject thisObject) {
    char *s = "Hello";
    if (incrementHandlerPtr)
        s = incrementHandlerPtr();
    return (*env) -> NewStringUTF(env, s);
}

JNIEXPORT jstring JNICALL
Java_net_dot_MainActivity_greetName(
        JNIEnv* env,
        jobject thisObject,
        jstring string) {
    char *s = "Hello Android!\nRunning on mono runtime\nUsing C#";
    const char *name = (*env)->GetStringUTFChars(env, string, NULL);
    if (greetHandlerPtr)
        s = greetHandlerPtr(name);
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

void
androidRegisterNameGreet (void* ptr)
{
    greetHandlerPtr = ptr;
}

int myNum() {
    JNIEnv* jniEnv = GetJniEnv();

    jclass mainActivity = (*jniEnv)->FindClass (jniEnv, "net/dot/MainActivity");
    jmethodID getNum = (*jniEnv)->GetStaticMethodID(jniEnv, mainActivity, "getNum", "()I");
    jint val = (*jniEnv)->CallStaticIntMethod(jniEnv, mainActivity, getNum);
    return val;
}