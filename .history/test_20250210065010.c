void main() {
    int nota1, nota2, nota3;

    printf("Digite a primeira nota: ");
    scanf("%d", nota1);

    printf("Digite a segunda nota: ");
    scanf("%d", nota2);

    printf("Digite a terceira nota: ");
    scanf("%d", nota3);

    calcularMedia(nota1, nota2, nota3);
}

void calcularMedia(int nota1, int nota2, int nota3) {
    float media = (nota1 + nota2 + nota3) / 3.0;
    
    printf("Média do aluno: %f\n", media);
    
    if (media >= 7.0) {
        printf("Aluno aprovado!\n");
    } 
    else if (media >= 5.0) {
        printf("Aluno em recuperação.\n");
    } 
    else {
        printf("Aluno reprovado.\n");
    }
}


