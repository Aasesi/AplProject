;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Project Title: Image Binarization
; Description  : This algorithm is designed for the process of converting pixels
;                data from an image into binary image with main goal to separate
;                foreground from background. The process involves determining whether
;                each pixel should be classified as white or black based on individual
;                RGB channel values.
;
; 
; Implementation Date: 15.01.2024
; Semester/Academic Year: Semester 5, Winter 2023/2024
; Author: Miko≥aj WilczyÒski, Konrad Kie≥tyka, Weronika èeraÒska
; 
; Version: 1.0
; 
; 
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;



.data
red dq  0
green dq  0
blue dq  0
result dq  0
threshold dq  0
len dq  0
 

 
.code
AsmBinarize proc
mov rax,rcx

mov rbx, [rcx]			; Load first elment of the struct - red array
mov red, rbx			; Assign to a pointer

mov rbx, [rcx + 8]		; Load green array element
mov green, rbx			; Assign

mov rbx, [rcx + 16]		; Load blue array values element
mov blue, rbx			; Assign

mov rbx, [rcx + 24]		; Load result element
mov result, rbx			; Assign 

mov rbx, [rcx + 32]		; Load threshold array
mov threshold, rbx		; Assign

mov rbx, [rcx + 40]		; Load size of the array
mov len, rbx			; Assign

; Calculating loop iterations
xor rdx, rdx			; Zero rdx for division
mov rsi, len			; Lenght in rsi
mov rax, rsi			; Lenght
mov rbx, 4				
div rbx					; Length rbx
mov r11, rax			; Move number of loops into r11 


; Zero registers and setup for the loop
mov rsi, threshold		; put threshold pointer into rsi register
movups xmm0, [rsi]		; Put threshold array into xmm0 register
xor r8, r8				; Array elements index for four array elments
xor r9, r9				; Loop index
xor r10, r10			; Additional array for smaller index
xor r12, r12			; Smaller loop index


; Loop for thresholding
@thresholdloop:

; Load four array elements of red, blue, green arrays into apropriate xmm registers then compare with threshold value
mov rsi, red			; Load red
movups xmm1, [rsi + r8]	; Load first four ints to xmm1
cmpps xmm1, xmm0, 1		; Compare with the threshold values

mov rsi, blue			; Load blue
movups xmm2, [rsi + r8] ; Load first four ints to xmm1
cmpps xmm2, xmm0, 1		; Compare with the threshold values

mov rsi, green			; Load green
movups xmm3, [rsi + r8] ; Load first four ints to xmm1
cmpps xmm3, xmm0, 1		; Compare with the threshold values


; Perform bit operations and put it into results array
movaps xmm4, xmm1		; Copy changed red into xmm4
andps xmm1, xmm2		; And on red and green 
andps xmm2, xmm3		; And on blue and green
andps xmm3, xmm4		; And on red and blue
orps xmm1, xmm2			; Or operation on red-green and blue-green
orps xmm1, xmm3			; or operation on red-green-blue-green and red-blue
mov rsi, result			; Put result array pointer into rsi register
movups [rsi + r8], xmm1 ; Copy results to result array


jmp @Assign_binary_values	; Assign correct values for pixels


; Assign black or white pixels
@Assign_binary_values:
mov rsi, result			; Load result pointer to rsi
mov eax, [rsi + r10]	; Load n-th array element to eax
cmp eax, 4294967295		; Compare with 0xFFFFFFFF
je @equal_case			; Jump to equal case
jmp @not_equal			


@equal_case:
mov ebx, 0
mov [rsi + r10], ebx	; Assign 0
add r10, 4				; Increment array by one int
add r12, 1				; Increment loop counter by 1
cmp r12, 4				; Compare with the loop iteration 
jl @Assign_binary_values
jmp @increment_loop


@not_equal:
mov ebx, 255
mov [rsi + r10], ebx			; Assign 255
add r10, 4						; Increment array by one int
add r12, 1						; Increment loop counter by 1
cmp r12, 4						; Compare with the loop iteration
jl @Assign_binary_values
jmp @increment_loop


@increment_loop:
xor r12, r12		; Zero smaller loop
add r8, 16			; Get next four array elements 
add r9, 1			; Increment main loop
cmp r9, r11			; Compare loop index with the size
jl @thresholdloop	; Jump less to threshold loop
jmp @end_program	; Jump to program end


@end_program:
ret

AsmBinarize endp

end