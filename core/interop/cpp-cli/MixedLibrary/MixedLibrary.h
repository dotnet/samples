#pragma once

using namespace System;

namespace MixedLibrary {

	public ref class ManagedClass
	{
	public:
		void Hello();
		void CallNative(String^ msg);
	};

#pragma managed(push, off)
    public struct NativeClass
    {
    public:
        void Hello(const wchar_t *msg);
    };
#pragma managed(pop)
}
