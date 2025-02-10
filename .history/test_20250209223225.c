int numeros[5];
int i;

for (i = 0; i < 5; i = i + 1) {
    numeros[i] = i * 3;  // Multiplica o índice por 3
}

for (i = 0; i < 5; i = i + 1) {
    printf("numeros[%d]: %d\n", i, numeros[i]);
}
