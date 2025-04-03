set(CMAKE_INCLUDE_CURRENT_DIR ON)

if (WIN32)
    set(BASE_SOURCES
        ${BASE_SOURCES}
        src/EventPipeProfiler.def)
    add_compile_options(/std:c++17)
endif(WIN32)

set(SOURCES
    ${BASE_SOURCES}
    src/EventPipeProfiler.cpp
    src/dllmain.cpp
    src/eventpipemetadatareader.cpp
    src/eventpipeeventprinter.cpp)

add_library(EventPipeProfiler SHARED ${SOURCES})