LDA  #0
STA  $F0    
LDA  #1
STA  $F1    
LDX  #0

LOOP:  
    LDA  $F1
    STA  $0F1B,X
    STA  $F2     
    ADC  $F0
    STA  $F1     
    LDA  $F2
    STA  $F0     
    INX
    CPX  #9    
    BMI  LOOP         
                 