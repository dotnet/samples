// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

template<class TInterface>
class CComPtr
{
private:
    TInterface* pointer;
public:
    CComPtr(const CComPtr&) = delete; // Copy constructor
    CComPtr& operator= (const CComPtr&) = delete; // Copy assignment
    CComPtr(CComPtr&&) = delete; // Move constructor
    CComPtr& operator= (CComPtr&&) = delete; // Move assignment

    void* operator new(std::size_t) = delete;
    void* operator new[](std::size_t) = delete;

    void operator delete(void *ptr) = delete;
    void operator delete[](void *ptr) = delete;

    CComPtr()
    {
        this->pointer = nullptr;
    }

    ~CComPtr()
    {
        if (this->pointer)
        {
            this->pointer->Release();
            this->pointer = nullptr;
        }
    }

    operator TInterface*()
    {
        return this->pointer;
    }

    operator TInterface*() const
    {
        return this->pointer;
    }

    TInterface& operator *()
    {
        return *this->pointer;
    }

    TInterface& operator *() const
    {
        return *this->pointer;
    }

    TInterface** operator&()
    {
        return &this->pointer;
    }

    TInterface** operator&() const
    {
        return &this->pointer;
    }

    TInterface* operator->()
    {
        return this->pointer;
    }

    TInterface* operator->() const
    {
        return this->pointer;
    }

    void Release()
    {
        this->~CComPtr();
    }
};
