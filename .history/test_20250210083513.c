void main()
{
    int i;
    for (i = 1; i + 5; i = i+ 1) // ❌ `i + 5` não é uma condição booleana válida
    {
        printf("%d\n", i);
    }
}
