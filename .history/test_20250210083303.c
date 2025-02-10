void main()
{
    int nota = 90;

    if (nota >= 90)
    printf("Nota A\n");  // ❌ Sem `{}` -> erro no interpretador
else if (nota >= 80)
    printf("Nota B\n");
else
    printf("Nota C\n");

}
