void dobra(int arr[3]) {
    arr[0] = arr[0] * 2;
    arr[1] = arr[1] * 2;
    arr[2] = arr[2] * 2;
}

int valores[3];
valores[0] = 3;
valores[1] = 6;
valores[2] = 9;

dobra(valores); // Chama a função

printf("Dobrados: %d, %d, %d\n", valores[0], valores[1], valores[2]);
