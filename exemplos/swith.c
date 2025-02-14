void main()
{
    int opcao;

    printf("Escolha uma opção:\n");
    printf("1 - Iniciar\n");
    printf("2 - Configurações\n");
    printf("3 - Sair\n");
    opcao = 1;

    switch (opcao)
    {
    case 1:
        printf("Iniciando o programa...\n");
        break;
    case 2:
        printf("Abrindo configurações...\n");
        break;
    case 3:
        printf("Saindo do programa...\n");
        break;
    default:
        printf("Opção inválida!\n");
    }
}