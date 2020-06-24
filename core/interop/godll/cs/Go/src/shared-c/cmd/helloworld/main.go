package main

// #cgo CFLAGS: -g -Wall
// #include <stdlib.h>
import "C"

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"io"
	"unsafe"
)

func main() {}

//export HelloWorld
func HelloWorld(s string) unsafe.Pointer {
	helloStr := fmt.Sprintf("Hello, %s", s)
	buf := new(bytes.Buffer)
	writeInt32(buf, int32(len(helloStr)+4))
	writeString(buf, &helloStr)

	return C.CBytes(buf.Bytes())

}

func writeInt32(w io.Writer, value int32) {
	err := binary.Write(w, binary.LittleEndian, value)
	if err != nil {
		panic(err)
	}
}

func writeString(buf *bytes.Buffer, value *string) {
	if value == nil {
		err := binary.Write(buf, binary.LittleEndian, int32(-1))
		if err != nil {
			panic(err)
		}

	} else {
		err := binary.Write(buf, binary.LittleEndian, int32(len(*value)))
		if err != nil {
			panic(err)
		}
		_, err = buf.WriteString(*value)
		if err != nil {
			panic(err)
		}
	}
}
