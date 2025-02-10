int numeros[5] = {2, 4, 6, 8, 10};
int i = 0;
int encontrado = 0;

while (i < 5) {
    if (numeros[i] == 6) {
        printf("Número 6 encontrado na posição %d\n", i);
        encontrado = 1;
        break;
    }
    i = i + 1;
}

if (encontrado == 0) {
    printf("Número não encontrado\n");
}
