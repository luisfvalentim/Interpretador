void main()
{
    int idade;
    float altura;
    char inicial;
    string nome;

    printf("\n");
    puts("Digite sua idade:");
    scanf("%d", idade);

    puts("Digite sua altura:");
    scanf("%f", altura);

    puts("Digite sua inicial:");
    scanf("%c", inicial);

    puts("Digite seu nome:");
    gets(nome);

    printf("\n");
    puts("--- Dados Atualizados ---");
    printf("\n");
    printf("Nome: %s\n", nome);
    printf("Idade: %d anos\n", idade);
    printf("Altura: %f metros\n", altura);
    printf("Inicial: %c\n", inicial);
}

