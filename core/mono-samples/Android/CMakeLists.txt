cmake_minimum_required(VERSION 3.10)

project(buildnativelib)

add_library(
    native-lib
    SHARED
    native-lib.c)

target_link_libraries(
    native-lib
    log)