void main() {
    int a = 10;
    float b = 5.5;
    int soma = a + 5;
    float multiplicacao = b * 2.0;

    printf("a: %d\n", a); // ✅ Deve imprimir: a: 10
    printf("b: %f\n", b); // ✅ Deve imprimir: b: 5.50
    printf("Soma: %d\n", soma); // ✅ Deve imprimir: Soma: 15
    printf("Multiplicação: %f\n", multiplicacao); // ✅ Deve imprimir: Multiplicação: 11.00
}
