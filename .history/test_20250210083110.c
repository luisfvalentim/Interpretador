void main()
{
    int numero = 1;

    printf("Números ímpares de 1 a 10:\n");
    do
    {
        printf("%d ", numero); // ✅ Saída esperada: 1 3 5 7 9
        numero = numero + 2;
    } while (numero < 10);
    printf("\n");
}
