void main()
{
    int opcao = 2;

    switch (opcao)
    {
    case 1:
        printf("Opção 1 escolhida\n");
        break;
    case 2:
        printf("Opção 2 escolhida\n");
        break;
    case 3:
        printf("Opção 3 escolhida\n");
    default:
        printf("Opção inválida\n");
    }

    int idade = 20;

    if (idade >= 18)
    {
        printf("Você é maior de idade\n");
    }
    else
    {
        printf("Você é menor de idade\n");
    }
}
