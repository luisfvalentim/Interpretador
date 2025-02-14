void main()
{
    int a = 10;
    int b = 5;
    int c = 10;

    printf("Igualdade (a == c): %d\n", a == c);   // 1 (true)
    printf("Diferença (a != b): %d\n", a != b);   // 1 (true)
    printf("Maior que (a > b): %d\n", a > b);     // 1 (true)
    printf("Menor que (b < c): %d\n", b < c);     // 1 (true)
    printf("Maior ou igual (a >= c): %d\n", a >= c); // 1 (true)
    printf("Menor ou igual (b <= a): %d\n", b <= a); // 1 (true)
    printf("Operador AND (&&): %d\n", (a > b) && (a == b)); // 0 (false)
    printf("Operador OR (||): %d\n", (a < b) || (b > c)); // 0 (false)
    printf("Operador NOT (!): %d\n", !(a != c)); // 0 (true)
}
