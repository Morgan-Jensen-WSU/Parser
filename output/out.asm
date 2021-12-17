;-------------------
; initialized data
;-------------------
		section .data
	fmtstr:	db "%s", 10, 0
	fmtuint:	db "%d", 10, 0
	fmtuintin:	db "%d", 0
	fmtfloatin:	db "%f", 0

	x:	dd	10	; a(n) num
	message0:	db	"Enter a number for y", 0	; a const string
	message1:	db	"The value of x is:", 0	; a const string
	message2:	db	"The value of y is:", 0	; a const string

;-------------------
; uninitialized data
;-------------------
		section .bss
	y:	resd	1	; an int

		section .text
;-------------------
; imports 
;-------------------
	extern printf
	extern scanf
	global main

main:
	push rbp	; Push base pointer onto stack to save it

	; write string to stdout
	mov	rsi, message0
	mov	rdi, fmtstr
	mov	rax, 0
	call printf

	; read in a number
	lea	rdi, [fmtuintin]
	lea	rsi, [y]
	mov	rax, 0
	call scanf

	; write string to stdout
	mov	rsi, message1
	mov	rdi, fmtstr
	mov	rax, 0
	call printf

	; write num to stdout
	mov	rsi, [x]
	mov	rdi, fmtuint
	mov	rax, 0
	call printf

	; write string to stdout
	mov	rsi, message2
	mov	rdi, fmtstr
	mov	rax, 0
	call printf

	; write num to stdout
	mov	rsi, [y]
	mov	rdi, fmtuint
	mov	rax, 0
	call printf

	; end program
	mov	rax, 60
	xor	rdi, rdi
	syscall

