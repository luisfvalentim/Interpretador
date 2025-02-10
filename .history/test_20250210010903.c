int numeros[5];
int i;

// Inicializa o array
for (i = 0; i < 5; i = i + 1) {
    numeros[i] = i * 2;
}

// Imprime os valores do array
for (i = 0; i < 5; i = i + 1) {
    printf("numeros[%d]: %d\n", i, numeros[i]);
}
