.data
.code
ProcAsm2 proc
xor rax,rax
mov rax, rdx 
mov dl,4 
mul dl 
 
mov eax, [rcx+rax]
 
ret
ProcAsm2 endp
end