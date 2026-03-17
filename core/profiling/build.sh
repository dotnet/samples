#!/bin/bash

printf '  Building ...'

if [ ! -d "bin/" ]; then
    mkdir bin/
fi

pushd bin

export CC=/usr/bin/clang
export CXX=/usr/bin/clang++
cmake ../ -DCMAKE_BUILD_TYPE=Debug

make -j8

popd

printf '  Copying binaries to main directory\n'
cp bin/eventpipe/libEventPipeProfiler.so .
cp bin/stacksampling/libSampleProfiler.so .

printf 'Done.\n'
