cmake_minimum_required (VERSION 3.14)

project(ProfilerSamples)

set(BASE_SOURCES
    ${CMAKE_CURRENT_SOURCE_DIR}/common/guids.cpp)

include_directories(common)
include_directories(common/sdk)

if(NOT WIN32)
    add_compile_options(-Wno-invalid-noreturn -Wno-pragma-pack -Wno-int-to-pointer-cast -fPIC -fms-extensions -DBIT64 -DPAL_STDCPP_COMPAT -DPLATFORM_UNIX -DHOST_64BIT -std=c++17)
    add_link_options(--no-undefined -pthread)
    
    include_directories(common/unix)
endif(NOT WIN32)

add_subdirectory(eventpipe)
add_subdirectory(stacksampling)
