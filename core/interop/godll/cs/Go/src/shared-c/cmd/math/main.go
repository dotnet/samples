package main

// #cgo CFLAGS: -g -Wall
// #include <stdlib.h>
import "C"

import (
	"math"
	"sort"
)

func main() {}

//export Add
func Add(a, b int) int {
	return a + b
}

//export Sub
func Sub(a, b int) int {
	return a - b
}

//export Cosine
func Cosine(x float64) float64 {
	return math.Cos(x)
}

//export Sum
func Sum(vals []int) int {
	var sum int
	for _, value := range vals {
		sum = sum + value
	}

	return sum
}

//export Sort
func Sort(vals []int) {
	sort.Ints(vals)
}
