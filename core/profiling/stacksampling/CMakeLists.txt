set(CMAKE_INCLUDE_CURRENT_DIR ON)

if (WIN32)
    set(BASE_SOURCES
        ${BASE_SOURCES}
        src/SampleProfiler.def)
endif(WIN32)

set(SOURCES
    ${BASE_SOURCES}
    src/SampleProfiler.cpp
    src/dllmain.cpp
    src/sampler.cpp)

add_library(SampleProfiler SHARED ${SOURCES})