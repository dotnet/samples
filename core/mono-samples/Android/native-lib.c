#include <jni.h>

// JNIEXPORT jstring JNICALL
// android_greet_name(
//         JNIEnv* env,
//         jobject /* this */) {
//     const char *s = "Hello from C";
//     return s;
// }

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

int myNum() {
    JNIEnv* jniEnv = GetJniEnv();

    jclass mainActivity = (*jniEnv)->FindClass (jniEnv, "net/dot/MainActivity");
    jmethodID getNum = (*jniEnv)->GetStaticMethodID(jniEnv, mainActivity, "getNum", "()I");
    jint val = (*jniEnv)->CallStaticIntMethod(jniEnv, mainActivity, getNum);
    return val;
}