int opcao;
do {
    printf("1 - Opção A\n");
    printf("2 - Opção B\n");
    printf("3 - Sair\n");
    printf("Escolha uma opção: ");
    scanf("%d", &opcao);

    if (opcao == 1) {
        printf("Você escolheu a Opção A\n");
    } else if (opcao == 2) {
        printf("Você escolheu a Opção B\n");
    } else if (opcao != 3) {
        printf("Opção inválida!\n");
    }
} while (opcao != 3);

printf("Programa encerrado.\n");
